using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using static SwishIdentity.Tools.DependencyService.ServiceRegistrationTarget;
using static System.Reflection.BindingFlags;

namespace SwishIdentity.Tools.DependencyService
{
    public static class ServiceCollectionExtensions
    {
        private static Func<IServiceProvider, object> AsServiceFactory(MethodInfo staticFactory)
        {
            var parameterTypes = staticFactory
                .GetParameters()
                .Select(x => x.ParameterType);

            return provider =>
                staticFactory.Invoke(null, parameterTypes.Select(t => provider.GetRequiredService(t)).ToArray());
        }

        public static void AddByAttributes(this IServiceCollection services, Assembly assembly)
        {
            IEnumerable<(Type, ServiceAttribute, Type)> ToTypeRegistrations(
                (Type Type, ServiceAttribute ServiceAttribute) serviceInfo)
            {
                return serviceInfo.ServiceAttribute.Target switch
                {
                    Auto => serviceInfo.Type.GetInterfaces() switch
                    {
                        Type[] interfaces when interfaces.Any() =>
                            interfaces.Select(i => serviceInfo.With(i)),
                        _ =>
                            new[] {serviceInfo.With(serviceInfo.Type)}
                    },
                    Self =>
                        new[] {serviceInfo.With(serviceInfo.Type)},
                    ImplementedInterfaces =>
                        serviceInfo.Type.GetInterfaces().Select(i => serviceInfo.With(i)),
                    _ =>
                        throw new InvalidOperationException(
                            $"Unsupported {nameof(ServiceRegistrationTarget)} {serviceInfo.ServiceAttribute.Target}")
                };
            }

            IEnumerable<(MethodInfo, ServiceAttribute, Type)> ToMethodRegistrations(
                (MethodInfo Method, ServiceAttribute ServiceAttribute) serviceInfo)
            {
                return serviceInfo.ServiceAttribute.Target switch
                {
                    ImplementedInterfaces => serviceInfo.Method.ReturnType.GetInterfaces() switch
                    {
                        Type[] interfaces when interfaces.Any() =>
                            interfaces.Select(@interface => serviceInfo.With(@interface)),
                        _ =>
                            throw new InvalidOperationException(
                                $"Method {serviceInfo.Method.Name} cannot be registered as {nameof(ImplementedInterfaces)}, because {serviceInfo.Method.ReturnType} does not implement any interfaces.")
                    },
                    var target when target == Auto || target == Self =>
                        new[] {serviceInfo.With(serviceInfo.Method.ReturnType)},
                    _ =>
                        throw new InvalidOperationException(
                            $"Unsupported {nameof(ServiceRegistrationTarget)} {serviceInfo.ServiceAttribute.Target}")
                };
            }

            var assemblyTypes = assembly.GetTypes();

            var serviceTypes = assemblyTypes
                .Select(t => (Type: t, ServiceAttribute: t.GetCustomAttribute<ServiceAttribute>(true)))
                .Where(x => x.ServiceAttribute != null)
                .SelectMany(ToTypeRegistrations);

            var serviceFactoryMethods = assemblyTypes
                .SelectMany(t => t.GetMethods(Static | Public | NonPublic)) // TODO: Scream at instance usages
                .Select(m => (Method: m, ServiceAttribute: m.GetCustomAttribute<ServiceAttribute>(true)))
                .Where(x => x.ServiceAttribute != null)
                .SelectMany(ToMethodRegistrations);

            foreach (var (method, ServiceAttribute, registerAsType) in serviceFactoryMethods)
                services.Add(new ServiceDescriptor(registerAsType, AsServiceFactory(method),
                    ServiceAttribute.Lifetime));

            foreach (var (type, serviceAttribute, registerAsType) in serviceTypes)
                services.Add(new ServiceDescriptor(registerAsType, type, serviceAttribute.Lifetime));
        }
    }
}
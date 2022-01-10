using System;
using Microsoft.Extensions.DependencyInjection;

namespace SwishIdentity.Tools.DependencyService
{
    public enum ServiceRegistrationTarget
    {
        Auto = 0,
        Self = 1,
        ImplementedInterfaces = 2
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    // <summary>
    // Signifies that the type should be registered as a dependency.
    // </summary>
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped,
            ServiceRegistrationTarget target = ServiceRegistrationTarget.Auto)
        {
            Lifetime = lifetime;
            Target = target;
        }

        public ServiceLifetime Lifetime { get; }
        public ServiceRegistrationTarget Target { get; }
    }
}
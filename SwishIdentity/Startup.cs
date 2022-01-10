using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SwishIdentity.Data;
using SwishIdentity.Tools.DependencyService;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace SwishIdentity
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                services.AddDbContext<SwishDbContext>(opt =>
                    opt.UseNpgsql(Configuration.GetConnectionString("DevPub")));
            }
            else
            {
                services.AddDbContext<SwishDbContext>(opt =>
                    opt.UseNpgsql(Configuration.GetConnectionString("VpcPub")));
            }
            services.AddIdentity<SwishUser, IdentityRole>(options =>
                { 
                    options.SignIn.RequireConfirmedEmail = false;
                })
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<SwishDbContext>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "SwishIdentityCookie";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Account/Login";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });
            services.AddByAttributes(typeof(Startup).Assembly);
            services.AddByAttributes(typeof(SwishDbContext).Assembly);
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
                options.Cookie.Name = "SwishAntiForgery";
                options.SuppressXFrameOptionsHeader = false;
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            });
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = (int) HttpStatusCode.TemporaryRedirect;
                options.HttpsPort = 443;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/Home/Error");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
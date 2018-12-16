using AutoMapper;
using CoreApp.Data.EF;
using CoreApp.Data.Entities;
using CoreApp.Web.Extensions;
using CoreApp.Web.SignalR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace CoreApp.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts =>
                        {
                            opts.ResourcesPath = "Resources";
                        })
                    .AddDataAnnotationsLocalization()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.Formatting = Formatting.Indented;
                        options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    });

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@".\wwwroot\shared"))
                .SetApplicationName("CoreApp.Web");

            services.AddSignalR().AddJsonProtocol(options => {
                options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("vi-VN"),
                    new CultureInfo("en-US")
                };

                options.DefaultRequestCulture = new RequestCulture("vi-VN");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddMemoryCache();

            services.AddMinResponse();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                    o => o.MigrationsAssembly("CoreApp.Data.EF")));

            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    facebookOptions.Events.OnRemoteFailure = (context) =>
                    {
                        context.Response.Redirect("/Account/Login");
                        context.HandleResponse();
                        return Task.FromResult(0);
                    };
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    googleOptions.ClaimActions.MapJsonSubKey("profile-image-url", "image", "url");
                    googleOptions.Events.OnRemoteFailure = (context) =>
                    {
                        context.Response.Redirect("/Account/Login");
                        context.HandleResponse();
                        return Task.FromResult(0);
                    };
                });

            services.AddConfigureIdentity();

            // Add recaptcha
            services.AddRecaptcha(new RecaptchaOptions()
            {
                SiteKey = Configuration["Recaptcha:SiteKey"],
                SecretKey = Configuration["Recaptcha:SecretKey"],
                ValidationMessage = "Xác nhận rằng bạn không phải là robot."
            });

            services.AddImageResizer();

            // Add AutoMapper
            services.AddAutoMapper();

            services.AddDependencyInjection();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IMapper autoMapper)
        {
            loggerFactory.AddFile("Logs/core-{Date}.txt");
            autoMapper.ConfigurationProvider.AssertConfigurationIsValid();

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
            app.UseImageResizer();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMinResponse();
            app.UseAuthentication();

            app.UseStatusCodePagesWithReExecute("/statuscode/{0}");

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseSignalR(routes =>
            {
                routes.MapHub<CoreHub>("/coreHub");
            });

            app.UseRoute();
        }
    }
}

using AutoMapper;
using DFC.App.FindACourse.Data.Contracts;
using DFC.App.FindACourse.Data.Domain;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Framework;
using DFC.App.FindACourse.HostedServices;
using DFC.App.FindACourse.Repository;
using DFC.App.FindACourse.Services;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Extensions;
using DFC.Content.Pkg.Netcore.Services;
using DFC.Content.Pkg.Netcore.Services.ApiProcessorService;
using DFC.Content.Pkg.Netcore.Services.CmsApiProcessorService;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using DFC.Logger.AppInsights.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System;
using System.Collections.Generic;

namespace DFC.App.FindACourse
{
    public class Startup
    {
        public const string CourseSearchAppSettings = "Configuration:CourseSearch";
        public const string CourseSearchClientSvcSettings = "Configuration:CourseSearchClient:CourseSearchSvc";
        public const string CourseSearchClientAuditSettings = "Configuration:CourseSearchClient:CosmosAuditConnection";
        public const string CourseSearchClientPolicySettings = "Configuration:CourseSearchClient:Policies";
        public const string StaticCosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:StaticContent";

        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAutoMapper(typeof(Startup).Assembly, typeof(DFC.FindACourseClient.FindACourseProfile).Assembly);
            services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
            services.AddScoped<IFindACourseService, FindACourseService>();
            services.AddScoped<IFindACourseRepository, FindACourseRepository>();

            services.AddApplicationInsightsTelemetry();
            services.AddDFCLogging(Configuration["ApplicationInsights:InstrumentationKey"]);
            var courseSearchSettings = Configuration.GetSection(CourseSearchAppSettings).Get<CourseSearchSettings>();
            services.AddSingleton(courseSearchSettings ?? new CourseSearchSettings());

            var courseSearchClientSettings = new CourseSearchClientSettings
            {
                CourseSearchSvcSettings = Configuration.GetSection(CourseSearchClientSvcSettings).Get<CourseSearchSvcSettings>() ?? new CourseSearchSvcSettings(),
                CourseSearchAuditCosmosDbSettings = Configuration.GetSection(CourseSearchClientAuditSettings).Get<CourseSearchAuditCosmosDbSettings>() ?? new CourseSearchAuditCosmosDbSettings(),
                PolicyOptions = Configuration.GetSection(CourseSearchClientPolicySettings).Get<PolicyOptions>() ?? new PolicyOptions(),
            };
            services.AddSingleton(courseSearchClientSettings);
            services.AddScoped<ICourseSearchApiService, CourseSearchApiService>();
            services.AddFindACourseServicesWithoutFaultHandling(courseSearchClientSettings);

            services.AddSingleton(Configuration.GetSection(nameof(CmsApiClientOptions)).Get<CmsApiClientOptions>() ?? new CmsApiClientOptions());
            var staticContentDbConnection = Configuration.GetSection(StaticCosmosDbConfigAppSettings).Get<CosmosDbConnection>();
            services.AddDocumentServices<StaticContentItemModel>(staticContentDbConnection, env.IsDevelopment());
            services.AddTransient<IStaticContentReloadService, StaticContentReloadService>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<ICmsApiService, CmsApiService>();
            services.AddTransient<IApiDataProcessorService, ApiDataProcessorService>();
            services.AddTransient<IApiCacheService, ApiCacheService>();
            services.AddTransient<IWebhooksService, WebhooksService>();
            services.AddTransient<MemoryCache>();

            var policyRegistry = services.AddPolicyRegistry();
            var policyOptions = Configuration.GetSection(CourseSearchClientPolicySettings).Get<PolicyOptions>() ?? new PolicyOptions();
            services.AddFindACourseTransientFaultHandlingPolicies(courseSearchClientSettings, policyRegistry);

            services.AddHostedServiceTelemetryWrapper();
            services.AddSubscriptionBackgroundService(Configuration);
            services.AddHostedService<StaticContentReloadBackgroundService>();

            services.AddApiServices(Configuration, policyRegistry);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        private static void AddPolicies(IPolicyRegistry<string> policyRegistry)
        {
            var policyOptions = new PolicyOptions() { HttpRetry = new RetryPolicyOptions(), HttpCircuitBreaker = new CircuitBreakerPolicyOptions() };
            policyRegistry.Add(nameof(PolicyOptions.HttpRetry), HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(policyOptions.HttpRetry.Count, retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));
            policyRegistry.Add(nameof(PolicyOptions.HttpCircuitBreaker), HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking, policyOptions.HttpCircuitBreaker.DurationOfBreak));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                // add the site map route
                endpoints.MapControllerRoute(
                    name: "Sitemap",
                    pattern: "Sitemap.xml",
                    new { controller = "Sitemap", action = "Sitemap" });

                // add the robots.txt route
                endpoints.MapControllerRoute(
                    name: "Robots",
                    pattern: "Robots.txt",
                    new { controller = "Robot", action = "Robot" });

                // add the default route
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "find-a-course/search/{controller=Course}/{action=Index}");
                endpoints.MapRazorPages();
            });
        }
    }
}
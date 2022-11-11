using AutoMapper;
using DFC.App.FindACourse.Data.Contracts;
using DFC.App.FindACourse.Data.Domain;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Framework;
using DFC.App.FindACourse.Helpers;
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
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public const string CourseSearchAppSettings = "Configuration:CourseSearch";
        public const string CourseSearchClientSvcSettings = "Configuration:CourseSearchClient:CourseSearchSvc";
        public const string CourseSearchClientAuditSettings = "Configuration:CourseSearchClient:CosmosAuditConnection";
        public const string CourseSearchClientPolicySettings = "Configuration:CourseSearchClient:Policies";
        public const string StaticCosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:StaticContent";
        private const string AzureSearchAppSettings = "AzureSearch";

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
            var cosmosRetryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };
            services.AddDocumentServices<StaticContentItemModel>(staticContentDbConnection, env.IsDevelopment(), cosmosRetryOptions);
            services.AddTransient<IStaticContentReloadService, StaticContentReloadService>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<ICmsApiService, CmsApiService>();
            services.AddTransient<IApiDataProcessorService, ApiDataProcessorService>();
            services.AddTransient<IApiCacheService, ApiCacheService>();
            services.AddTransient<IWebhooksService, WebhooksService>();
            services.AddTransient<IViewHelper, ViewHelper>();
            services.AddTransient<MemoryCache>();

            var policyRegistry = services.AddPolicyRegistry();
            services.AddFindACourseTransientFaultHandlingPolicies(courseSearchClientSettings, policyRegistry);

            services.AddHostedServiceTelemetryWrapper();
            services.AddSubscriptionBackgroundService(Configuration);
            services.AddHostedService<StaticContentReloadBackgroundService>();

            services.AddApiServices(Configuration, policyRegistry);

            var azureSearchOptions = Configuration.GetSection(AzureSearchAppSettings).Get<AzureSearchIndexConfig>() ?? new AzureSearchIndexConfig();
            services.AddSingleton(azureSearchOptions);
            services.AddSingleton<ILocationService, LocationService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

                //endpoints.MapControllerRoute(
                //    name: "findacourse",
                //    pattern: "find-a-course/{controller=Course}/{action=Index}");
                //endpoints.MapRazorPages();
            });
        }
    }
}
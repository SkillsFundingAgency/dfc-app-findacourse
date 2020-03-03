using AutoMapper;
using DFC.App.FindACourse.Data.Domain;
using DFC.App.FindACourse.Repository;
using DFC.App.FindACourse.Services;
using DFC.FindACourseClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace DFC.App.FindACourse
{
    public class Startup
    {
        public const string CourseSearchAppSettings = "Configuration:CourseSearch";
        public const string CourseSearchClientSvcSettings = "Configuration:CourseSearchClient:CourseSearchSvc";
        public const string CourseSearchClientAuditSettings = "Configuration:CourseSearchClient:CosmosAuditConnection";
        public const string CourseSearchClientPolicySettings = "Configuration:CourseSearchClient:Policies";

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
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

            services.AddScoped<IFindACourseService, FindACourseService>();
            services.AddScoped<IFindACourseRepository, FindACourseRepository>();

            var courseSearchSettings = this.Configuration.GetSection(CourseSearchAppSettings).Get<CourseSearchSettings>();
            services.AddSingleton(courseSearchSettings ?? new CourseSearchSettings());

            var courseSearchClientSettings = new CourseSearchClientSettings
            {
                CourseSearchSvcSettings = this.Configuration.GetSection(CourseSearchClientSvcSettings).Get<CourseSearchSvcSettings>() ?? new CourseSearchSvcSettings(),
                CourseSearchAuditCosmosDbSettings = this.Configuration.GetSection(CourseSearchClientAuditSettings).Get<CourseSearchAuditCosmosDbSettings>() ?? new CourseSearchAuditCosmosDbSettings(),
                PolicyOptions = this.Configuration.GetSection(CourseSearchClientPolicySettings).Get<PolicyOptions>() ?? new PolicyOptions(),
            };
            services.AddSingleton(courseSearchClientSettings);
            services.AddScoped<ICourseSearchApiService, CourseSearchApiService>();
            services.AddFindACourseServices(courseSearchClientSettings);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddSingleton(serviceProvider =>
            {
                return new MapperConfiguration(cfg =>
                {
                    cfg.AddProfiles(
                        new List<Profile>
                        {
                            new FindACourseProfile(),
                        });
                }).CreateMapper();
            });
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

         //   app.UseCors();
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
                    pattern: "find-a-course/{controller=Course}/{action=Index}");
                endpoints.MapRazorPages();
            });
        }
    }
}
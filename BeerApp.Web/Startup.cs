using System;
using BeerApp.Bll.Beers;
using BeerApp.Entities;
using BeerApp.Service;
using BeerApp.Service.Common;
using BeerApp.Service.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BeerApp.Web
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

	       services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
	        services.Configure<CookiePolicyOptions>(options =>
	        {
		        options.CheckConsentNeeded = context => true;
		        options.MinimumSameSitePolicy = SameSiteMode.None;
	        });

	        services.AddDistributedMemoryCache();

	        services.AddSession(options =>
	        {
		        // Set a short timeout for easy testing.
		        options.IdleTimeout = TimeSpan.FromSeconds(10);
		        options.Cookie.HttpOnly = true;
	        });

	        services.Configure<ApiBehaviorOptions>(options =>
	        {
		        options.SuppressConsumesConstraintForFormFileParameters = true;
		        options.SuppressInferBindingSourcesForParameters = true;
		        options.SuppressModelStateInvalidFilter = true;
	        });
	        // init API connection settings
	        var apiSection = Configuration.GetSection(nameof(BreweryDBSettings));
	        services.Configure<BreweryDBSettings>(apiSection);
	        services.AddScoped(sp => sp.GetService<IOptionsSnapshot<BreweryDBSettings>>().Value);
	        services.AddOptions();

	        services.AddHttpClient<BreweryDBClient>(httpClient =>
	        {
		        httpClient.BaseAddress = new Uri(apiSection.Get<BreweryDBSettings>().ApiUrl);
		        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
	        });

			 services.AddCors(opts =>
	        {
		        opts.AddPolicy("CorsPolicy",
			        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
	        });

	        services.AddScoped<IBeerManager, BeerManager>();
	        services.AddScoped<IBeerService, BeerService>();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
	        app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}

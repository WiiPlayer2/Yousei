using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StrawberryShake;
using StrawberryShake.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;
using Yousei.Web.Api;
using Yousei.Web.Api.Serialization;
using Blazorise;
using Blazorise.Icons.Material;
using Blazorise.Bulma;

namespace Yousei.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddHostedService<MainService>();

            services
                .AddBlazorise()
                .AddBulmaProviders()
                .AddMaterialIcons();

            services
                .AddSingleton(Core.Serialization.Yaml.YamlUtil.BuildDeserializer());

            // Api
            services
                .Configure<ApiOptions>(Configuration.GetSection("Api"))
                .AddSingleton<ISerializer, UnitSerializer>()
                .AddYouseiApi(ExecutionStrategy.CacheFirst)
                .ConfigureHttpClient((sp, client) =>
                {
                    var apiOptions = sp.GetService<IOptions<ApiOptions>>();
                    client.BaseAddress = apiOptions?.Value.Url;
                });
        }
    }
}
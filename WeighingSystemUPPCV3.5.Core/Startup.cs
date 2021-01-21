using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using WeighingSystemUPPCV3_5_Repository;
using SysUtility.Config;
using SysUtility.Config.Models;
using SysUtility.Helpers;
using SysUtility.Logging;
using WeighingSystemUPPCV3_5_Core.Extensions;

namespace WeighingSystemUPPCV3_5_Core
{
    public class Startup
    {
        public IHostEnvironment HostEnvironment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.RegisterTraceSourceService(Configuration);

            services.AllowCORS();

            services.ConfigureWritable<AppConfig>(Configuration.GetSection("ApplicationSettings"), defaultValues: AppConfig.GetDefault());

            services.AddDbContextService(Configuration);

            services.AddRepositoryService();

            services.DisableAutoValidate();


            /**
             * HAS ERROR
             */
            //services.ConfigureSessions();

            //services.AddMvc(setupAction =>
            //{
            //    setupAction.EnableEndpointRouting = false;
            //}).AddJsonOptions(jsonOptions =>
            //{
            //    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            //})
            //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            services.AddControllers(setupAction =>
            {
                setupAction.EnableEndpointRouting = false;
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseHsts();
            }

            CustomCultureHelpers.SetCustomCulture();

            app.UseHttpsRedirection();

            app.UseCors();


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}

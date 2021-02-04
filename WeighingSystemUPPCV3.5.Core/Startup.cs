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
using SysUtility.Config.Interfaces;
using SysUtility.Models;
using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;

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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IAppConfigRepository appConfigRepository, ILogger<Startup> logger)
        {
            appConfigRepository.LoadJSON();

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

            app.Use(async (context, next) =>
            {
                //logger.LogInformation($"Header: {JsonConvert.SerializeObject(context.Request.Headers, Formatting.Indented)}");

                context.Request.EnableBuffering();
                logger.LogInformation($"Path: {context.Request.Host}/{context.Request.Path}");

                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                if (body != null) logger.LogInformation($"Body: {body}");
                var queryString = context.Request.QueryString;
                if (queryString != null) logger.LogInformation($"Query: {queryString}");
                context.Request.Body.Position = 0;
                logger.LogInformation($"Client IP: {context.Connection.RemoteIpAddress}");
                await next.Invoke();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}

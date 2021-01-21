using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WeighingSystemUPPCV3_5_Core.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureAntiForgery(this IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.FormFieldName = "__RequestVerificationToken";
                options.HeaderName = "X-CSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });
        }

        public static void DisableAutoValidate(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        public static void ConfigureSessions(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(30));
        }

        public static void AllowCORS(this IServiceCollection services)
        {
            services.AddCors(o => o.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            }));
        }
    }
}

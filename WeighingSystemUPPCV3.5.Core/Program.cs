using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net;
using WeighingSystemUPPCV3_5_Core.Extensions;

namespace WeighingSystemUPPCV3_5_Core
{
    public class Program
    {


        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            try
            {
                //   var config = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("hostsettings.json", optional: true)
                //.AddCommandLine(args)
                //.Build();

                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
      .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true).AddCommandLine(args).Build();

                return Host.CreateDefaultBuilder(args)
                      .ConfigureWebHostDefaults(webBuilder =>
                      {
                          //webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                          //webBuilder.UseConfiguration(config);
                          //webBuilder.UseIISIntegration();
                          //webBuilder.ConfigureKestrel(serverOptions =>
                          //{
                          //    // Set properties and call methods on options
                          //    serverOptions.Limits.MaxConcurrentConnections = 100;
                          //    serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
                          //    serverOptions.Limits.MaxRequestBodySize = 10 * 1024;
                          //    serverOptions.Limits.MinRequestBodyDataRate =
                          //        new MinDataRate(bytesPerSecond: 100,
                          //            gracePeriod: TimeSpan.FromSeconds(10));
                          //    serverOptions.Limits.MinResponseDataRate =
                          //        new MinDataRate(bytesPerSecond: 100,
                          //            gracePeriod: TimeSpan.FromSeconds(10));
                          //    serverOptions.Listen(IPAddress.Loopback, 5000);

                          //    serverOptions.Limits.KeepAliveTimeout =
                          //        TimeSpan.FromMinutes(2);
                          //    serverOptions.Limits.RequestHeadersTimeout =
                          //        TimeSpan.FromMinutes(1);
                          //});
                          webBuilder.UseKestrel(a => a.ConfigureEndpoints());
                          webBuilder.UseContentRoot(Directory.GetCurrentDirectory());              webBuilder.UseUrls(config.GetSection("urls").Value);
                          webBuilder.UseIISIntegration();
                          // //
                          //webBuilder.ConfigureKestrel(serverOptions =>
                          //{
                          //    serverOptions.ConfigureHttpsDefaults(listenOptions =>
                          //    {
                          //        // certificate is an X509Certificate2
                          //        listenOptions.ServerCertificate = certificate;
                          //    });
                          //});
                          webBuilder.UseStartup<Startup>();
                      });
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw ex;
            }

        }


    }
}

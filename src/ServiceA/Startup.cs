using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microphone.AspNet;
using Microphone.Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServiceA
{
    public class Startup
    {
        public const string DefaultHost = "172.17.0.1";
        public const string DefaultPort = "5000";
        public const string DefaultConsulHost = "172.17.0.1";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddMicrophone<ConsulProvider>();

            services.Configure<ConsulOptions>(o=>
            {
                o.HealthCheckPath = "/api/hello";
                o.Host = "172.17.0.1";
                o.Heartbeat = 5;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var logger = loggerFactory.CreateLogger("Startup");

            logger.LogInformation("Reading environment variables...");
            var appHost = Configuration.GetValue<string>("DEMO_HOST") ?? DefaultHost;
            var appPort = Configuration.GetValue<string>("DEMO_PORT") ?? DefaultPort;
            logger.LogInformation($"Setting up Microphone with URI: http://{appHost}:{appPort}");

            app.UseMvc()
            .UseMicrophone("ServiceA", "1.0", new Uri($"http://{appHost}:{appPort}"))
            ;
        }
    }
}

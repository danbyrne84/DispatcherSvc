using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Xml.Schema;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using Owin;
using Quartz.Impl;
using SaleCycle.Svc.Dispatcher.Service;
using Topshelf;
using Topshelf.SimpleInjector;
using Swashbuckle.Application;
using RestSharp;
using Quartz;
using SaleCycle.Svc.Dispatcher.Contract;
using SaleCycle.Dispatchers.Sms.Dynmark;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using NLog;
using Microsoft.Owin;
using SaleCycle.Svc.Dispatcher.Factories;

namespace SaleCycle.Svc.Dispatcher
{
    class Program
    {
        public static Container Container { get; set; }
        public static List<Type> Dispatchers { get; set; }
        public static ILogger Logger;

        private static void Main()
        {
            Container = new Container();

            // Configure the Container
            ConfigureContainer(Container, new AppSettingsReader());

            // Preload dispatchers
            SmsDispatcherFactory.PreloadDispatchers();

            // Optionally verify the container's configuration to check for configuration errors.
            Container.Verify();

            HostFactory.Run(config =>
            {
                // Pass it to Topshelf
                config.UseSimpleInjector(Container);

                config.Service<DispatcherService>(s =>
                {
                    // Let Topshelf use it
                    s.ConstructUsingSimpleInjector();
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });

                config.SetServiceName("SaleCycle.Svc.Dispatcher");
                config.SetDisplayName("SaleCycle Dispatcher Service");
                config.SetDescription("SaleCycle Dispatcher Service");
            });
        }

        private static void ConfigureContainer(Container container, AppSettingsReader settingsReader)
        {
            // Default Quartz scheduler
            container.Register<IScheduler>(() =>
            {
                var scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();
                return scheduler;

            }, Lifestyle.Singleton);

            // RestSharp rest client
            container.Register<IRestClient>(() => new RestClient(), Lifestyle.Transient);

            // Register default logger
            container.Register<ILogger>(() => new NLog.LogFactory().GetCurrentClassLogger());

            // retrieve service settings from App.config
            container.Register<ServiceSettings>(() =>
            {
                string apiUrl = settingsReader.GetValue("ApiUrl", typeof(string)) as string;
                Uri configServiceUrl = null;
                Uri pollingUrl = null;

                Uri.TryCreate(settingsReader.GetValue("ConfigServiceUrl", typeof(string)) as string, UriKind.Absolute, out configServiceUrl);
                Uri.TryCreate(settingsReader.GetValue("PollingUrl", typeof(string)) as string, UriKind.Absolute, out pollingUrl);

                return new ServiceSettings
                {
                    ConfigServiceUrl = configServiceUrl,
                    PollingUrl = pollingUrl,
                    ApiUrl = apiUrl,
                };
            });

            // set up Quartz logging
            Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };
        }

    }

    [assembly: OwinStartup(typeof(Startup))]
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureWebApi(app);
        }

        public void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.EnableSwagger(c => c.SingleApiVersion("v1", "SaleCycle Dispatcher API")).EnableSwaggerUi();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, controller = "Default", action = "Index" }
            );

            app.UseWebApi(config);

            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Program.Container);
        }
    }

}

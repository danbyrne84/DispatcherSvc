using System.Web.Http;
using System.Xml.Schema;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using Owin;
using Quartz.Impl;
using SaleCycle.Svc.Dispatcher.Dispatchers;
using SaleCycle.Svc.Dispatcher.Service;
using Topshelf;
using Topshelf.SimpleInjector;
using Swashbuckle.Application;
using RestSharp;
using Quartz;
using SaleCycle.Svc.Dispatcher.Models;

namespace SaleCycle.Svc.Dispatcher
{
    class Program
    {
        public static Container Container { get; set; }

        private static void Main()
        {
            Container = new Container();

            // Configure the Container
            ConfigureContainer(Container);

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

        /// <summary>
        /// Register services here
        /// </summary>
        /// <param name="container"></param>
        private static void ConfigureContainer(Container container)
        {
            // Default Quartz scheduler
            container.Register<IScheduler>(() => StdSchedulerFactory.GetDefaultScheduler(), Lifestyle.Singleton);

            // RestSharp rest client
            container.Register<IRestClient>(() => new RestClient(), Lifestyle.Transient);

            // Sms dispatcher
            container.Register<IDispatcher<SmsDispatch>, SmsDispatcher>();

            container.Register<ServiceSettings>(() => new ServiceSettings
            {
                ConfigServiceUrl = "",
                EndpointUrl = "",
                PollingUrl = ""
            });
        }

    }

    [assembly: Microsoft.Owin.OwinStartup(typeof(Startup))]
    public partial class Startup
    {
        public void Configuration(Owin.IAppBuilder app)
        {
            ConfigureWebApi(app);
        }

        public void ConfigureWebApi(Owin.IAppBuilder app)
        {
            var config = new HttpConfiguration();

            // enable Swagger document generation
            config.EnableSwagger(c => c.SingleApiVersion("v1", "SaleCycle Dispatcher API")).EnableSwaggerUi();

            // map routes define by attributes (all of them really)
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

            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Program.Container);
            app.UseWebApi(config);
        }
    }

}

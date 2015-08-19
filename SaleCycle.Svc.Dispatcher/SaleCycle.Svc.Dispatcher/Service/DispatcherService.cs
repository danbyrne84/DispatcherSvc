using System.Collections.Generic;
using System.Configuration;
using Microsoft.Owin.Hosting;
using RestSharp;
using SaleCycle.Svc.Dispatcher.Dispatchers;
using SaleCycle.Svc.Dispatcher.Models;

namespace SaleCycle.Svc.Dispatcher.Service
{
    public class DispatcherService
    {
        private ServiceSettings Settings { get; set; }
        private PollingService<SmsDispatcher, SmsDispatch> PollingService { get; set; }
        private readonly AppSettingsReader _settingsReader = new AppSettingsReader();

        public DispatcherService(ServiceSettings settings)
        {
            Settings = settings;
        }

        public bool Start()
        {
            if (Settings.IsValid() == false) { return false; }

            // set up endpoints for push support
            WebApp.Start<Startup>(Settings.EndpointUrl);

            // set up polling service
            PollingService = Program.Container.GetInstance<PollingService<SmsDispatcher, SmsDispatch>>();

            return true;
        }

        public List<PollingSettings> GetPollingSettings(string configServiceUrl)
        {
            var settings = new List<PollingSettings>();
            
            var client = new RestClient(configServiceUrl);
            var baseUrl = _settingsReader.GetValue("PollingUrl", typeof(string)).ToString();

            var configurations = client.Get<List<DispatchConfig>>(new RestRequest("/dispatch/configs", Method.GET));

            configurations.Data.ForEach(config =>
            {
                settings.Add(new PollingSettings
                {
                    BaseUrl = baseUrl,
                    ClientId = config.ClientId.ToString(),
                    PollingIntervalMinutes = config.PollingIntervalMinutes
                });
            });

            return settings;
        }

        public bool Stop()
        {
            PollingService.Stop();

            return true;
        }
    }
}

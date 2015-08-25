using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Hosting;
using Quartz.Impl;
using RestSharp;
using SaleCycle.Svc.Dispatcher.Contract;
using SaleCycle.Svc.Dispatcher.Service.Polling;
using NLog;
using Quartz;
using SaleCycle.Svc.Dispatcher.Contract.Extensions;

namespace SaleCycle.Svc.Dispatcher.Service
{
    public class DispatcherService
    {
        private readonly ILogger _logger;
        private readonly IRestClient _restClient;
        private readonly List<PollingService> _pollingJobs = new List<PollingService>();

        public DispatcherService(ServiceSettings settings, IRestClient restClient, ILogger logger)
        {
            Settings = settings;

            _restClient = restClient;
            _logger = logger;
        }

        private ServiceSettings Settings { get; set; }

        public bool Start()
        {
            if (Settings.IsValid() == false)
            {
                _logger.Error("Dispatcher service settings are invalid, please check App.config settings");
                return false;
            }

            // set up endpoints for push support
            WebApp.Start<Startup>(Settings.ApiUrl);

            // polling - this can be deprecated once the compilation process is a service
            SetupPolling();

            return true;
        }

        private void SetupPolling()
        {
            _logger.Info("Setting up polling service");
            var pollingClients = RetrieveJobs(Settings.ConfigServiceUrl).ToList();

            _logger.Info("{0} clients configured".Fmt(pollingClients.Count()));

            // retrieve configured clients
            foreach (var clientSettings in pollingClients)
            {
                _logger.Info("Setting up polling service");

                // create service
                var pollingSvc = new PollingService(Program.Container.GetInstance<IScheduler>(), Program.Container.GetInstance<ILogger>());
                _pollingJobs.Add(pollingSvc);

                // start the service
                pollingSvc.Start(clientSettings);
            }
        }

        private IEnumerable<PollingSettings> RetrieveJobs(Uri configServiceUrl)
        {            
            _logger.Info("Retrieving polling settings from {0}".Fmt(configServiceUrl));

            var jobs = new List<PollingSettings>();
            
            var authority = Settings.ConfigServiceUrl.GetLeftPart(UriPartial.Authority);
            _restClient.BaseUrl = new Uri(authority);

            try
            {
                var request = new RestRequest(configServiceUrl.PathAndQuery, Method.GET);
                var response =_restClient.Get<List<DispatchConfig>>(request);
                var clientConfigurations = response.Data ?? new List<DispatchConfig>();

                // retrieve settings for configured clients
                clientConfigurations.ForEach(config =>
                {
                    _logger.Info("Retrieved configuration for client {0} with provider {1} and polling interval {2}"
                        .Fmt(config.ClientId, config.Provider, config.PollingIntervalMinutes));

                    jobs.Add(new PollingSettings
                    {
                        PollingUrl = Settings.PollingUrl,
                        ClientId = config.ClientId.ToString(),
                        PollingIntervalMinutes = config.PollingIntervalMinutes,
                        Provider = config.Provider,
                        DispatcherSettings = config.Settings
                    });
                });
            }
            catch (Exception ex) { _logger.Error("Error retrieving polling configurations - {0}".Fmt(ex)); }

            return jobs;
        }

        public bool Stop()
        {
            _logger.Info("Stopping dispatcher service");

            if (_pollingJobs == null || _pollingJobs.Any() == false) { return true; }

            _pollingJobs.ForEach(x =>
            {
                _logger.Info("Stopping polling job for client {0} and provider {1}".Fmt(x.Settings.ClientId, x.Settings.Provider));
                x.Stop();
            });

            // finally shutdown the quartz scheduler
            Program.Container.GetInstance<IScheduler>().Shutdown();

            _logger.Info("Dispatcher service shut down.");

            return true;
        }
    }
}
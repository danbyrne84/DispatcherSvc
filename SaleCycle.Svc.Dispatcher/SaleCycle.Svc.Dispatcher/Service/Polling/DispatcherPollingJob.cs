using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NLog;
using Quartz;
using RestSharp;
using SaleCycle.Svc.Dispatcher.Contract;
using SaleCycle.Svc.Dispatcher.Contract.Extensions;
using SaleCycle.Svc.Dispatcher.Factories;

namespace SaleCycle.Svc.Dispatcher.Service.Polling
{
    public class DispatcherPollingJob : IJob
    {
        private readonly IRestClient _client;
        private readonly ILogger _logger;

        public DispatcherPollingJob()
        {
            // this is here because Quartz/IJob requires a parameterless constructor
            _client = Program.Container.GetInstance<IRestClient>();
            _logger = Program.Container.GetInstance<ILogger>();
        }

        public DispatcherPollingJob(IRestClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public void Execute(IJobExecutionContext context)
        {
            var settings = context.JobDetail.JobDataMap["settings"] as PollingSettings;
            if (settings == null)
            {
                _logger.Error("Error retrieving settings for job {0}".Fmt(context.JobDetail.Key));
                return;
            }

            _logger.Info("Executing polling job for client {0} provider {1}".Fmt(settings.ClientId, settings.Provider));

            // retrieve dispatcher
            var dispatcher = SmsDispatcherFactory.GetDispatcherByName(settings.Provider);
            
            // build request
            var request = new RestRequest(settings.PollingUrl);
            request.AddUrlSegment("id", settings.ClientId);

            // retrieve response
            var response = _client.Execute<List<ISmsDispatch>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                _logger.Error("Error retrieving jobs from {0}".Fmt(request.ToString()));
                return;
            }

            // dispatch
            try
            {
                dispatcher.Dispatch(response.Data);
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to dispatch {0} dispatches - error {1}".Fmt(response.Data.Count(), ex));
            }
        }
    }
}

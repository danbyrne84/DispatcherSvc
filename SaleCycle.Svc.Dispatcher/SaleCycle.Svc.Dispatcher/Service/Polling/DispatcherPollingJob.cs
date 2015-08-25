using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            var dispatcher = SmsDispatcherFactory.GetDispatcherByName(settings.Provider, settings.DispatcherSettings);

            // setup rest client - no need for validation here, it's already in a valid URI
            _client.BaseUrl = new Uri(settings.PollingUrl.Scheme + "://" + settings.PollingUrl.Authority + "/");

            // build request
            var path = HttpUtility.UrlDecode(settings.PollingUrl.PathAndQuery);
            var request = new RestRequest(path, Method.GET);
            request.AddUrlSegment("clientId", settings.ClientId);

            // retrieve response
            var response = _client.Execute<List<SmsDispatch>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                _logger.Error("Error retrieving jobs from {0}".Fmt(request.ToString()));
                return;
            }

            try
            {
                // dispatch
                var results = dispatcher.Dispatch(response.Data).ToList();
                var stats = new
                {
                    Processed = results.Count(),
                    Successful = results.Count(x => x.Error == false),
                    Failed = results.Count(x => x.Error)
                };
                
                _logger.Info("{0} records processed - {1} successful and {2} failed".Fmt(stats.Processed, stats.Successful, stats.Failed ));

                UpdateDispatchRecords(results);
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to dispatch {0} dispatches - error {1}".Fmt(response.Data.Count(), ex));
            }
        }

        private void UpdateDispatchRecords(IEnumerable<DispatchResult<ISmsDispatch>> dispatches)
        {
            // @todo
        }
    }
}

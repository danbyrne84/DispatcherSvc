using System.Collections.Generic;
using NLog.LayoutRenderers.Wrappers;
using Quartz;
using Quartz.Impl;
using RestSharp;
using SaleCycle.Svc.Dispatcher.Dispatchers;

namespace SaleCycle.Svc.Dispatcher.Service
{
    public class PollingService<T, TR>
        where T : IDispatcher<TR>
        where TR: class
    {
        private readonly IScheduler _scheduler;

        private PollingSettings _settings;

        public PollingService(IScheduler scheduler)
        {
            _scheduler = scheduler;           
            _scheduler.Start();
        }

        public bool Start(PollingSettings settings)
        {
            _settings = settings;

            var job = JobBuilder.Create<DispatcherPollingJob<T,TR>>().Build();
            
            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(_settings.PollingIntervalMinutes)
                .RepeatForever())
                .Build();

            _scheduler.ScheduleJob(job, trigger);
            _scheduler.Start();

            return true;
        }

        public void Stop()
        {
            _scheduler.Shutdown();
        }


    }

    public class DispatcherPollingJob<T,TR> : IJob
        where T : IDispatcher<TR>
        where TR : class
    {
        private readonly IRestClient _client;
        private readonly T _dispatcher;

        public DispatcherPollingJob(IRestClient client, T dispatcher)
        {
            _client = client;
            _dispatcher = dispatcher;
        }

        public void Execute(IJobExecutionContext context)
        {
            var settings = context.JobDetail.JobDataMap["settings"] as PollingSettings;

            if (settings == null)
            {
                //@todo write fail log
                return;
            }

            var request = new RestRequest("sms/{id}/responses");
            request.AddUrlSegment("id", settings.ClientId);
            
            var response = _client.Execute<List<TR>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                //@todo write fail log
                return;
            }

            _dispatcher.Dispatch(response.Data);
        }
    }
}

using System;
using Common.Logging;
using NLog;
using Quartz;
using SaleCycle.Svc.Dispatcher.Contract;
using SaleCycle.Svc.Dispatcher.Contract.Extensions;

namespace SaleCycle.Svc.Dispatcher.Service.Polling
{
    public class PollingService
    {
        public PollingSettings Settings { get; private set; }
        
        private readonly IScheduler _scheduler;
        private readonly ILogger _logger;
        
        public PollingService(IScheduler scheduler, ILogger log)
        {
            _scheduler = scheduler;
            _logger = log;
        }

        public bool Start(PollingSettings settings)
        {
            _logger.Info("Polling service started for client {0} and provider {1}".Fmt(settings.ClientId, settings.Provider));

            Settings = settings;

            try
            {
                var job = JobBuilder.Create<DispatcherPollingJob>().Build();
                job.JobDataMap.Add("settings", settings);

                var trigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(Settings.PollingIntervalMinutes)
                    .RepeatForever())
                    .Build();

                var nextRun = _scheduler.ScheduleJob(job, trigger);
                _logger.Info("Next run scheduled for {0}".Fmt(nextRun));

                if (_scheduler.IsStarted == false)
                {
                    _scheduler.Start();
                }

            }
            catch(Exception ex)
            {
                _logger.Error("Unable to start service for client {0} and provider {1} - exception: {2}".Fmt(settings.ClientId, settings.Provider, ex));    
            }

            return true;
        }

        public bool Stop()
        {
            _logger.Info("Polling service for client {0} and provider {1} shutting down".Fmt(Settings.ClientId, Settings.Provider));
            _scheduler.Shutdown();

            return true;
        }
    }
}

using System;
using System.Collections.Generic;

namespace SaleCycle.Svc.Dispatcher.Service
{
    public class PollingSettings
    {
        public string ClientId { get; set; }
        public string Provider { get; set; }

        public Dictionary<string, object> DispatcherSettings { get; set; }

        public Uri PollingUrl { get; set; }
        public int PollingIntervalMinutes { get; set; }
    }
}

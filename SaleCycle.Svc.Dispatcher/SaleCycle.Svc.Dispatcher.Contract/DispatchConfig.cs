using System.Collections.Generic;

namespace SaleCycle.Svc.Dispatcher.Contract
{
    public class DispatchConfig
    {
        public int ClientId { get; set; }
        public string Provider { get; set; }
        public int PollingIntervalMinutes { get; set; }

        public Dictionary<string, object> Settings { get; set; }
    }
}

using System;

namespace SaleCycle.Svc.Dispatcher.Service
{
    public class ServiceSettings
    {
        public string ApiUrl { get; set; }
        public Uri PollingUrl { get; set; }
        public Uri ConfigServiceUrl { get; set; }

        public bool IsValid()
        {
            return (ApiUrl != null && PollingUrl != null && ConfigServiceUrl != null);
        }
    }
}

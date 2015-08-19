using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleCycle.Svc.Dispatcher
{
    public class ServiceSettings
    {
        public string EndpointUrl { get; set; }
        public string PollingUrl { get; set; }
        public string ConfigServiceUrl { get; set; }

        public bool IsValid()
        {
            // todo IsNotNullOrWhiteSpace extension
            return (EndpointUrl != null && EndpointUrl.Trim().Length > 0
                    && PollingUrl != null && PollingUrl.Trim().Length > 0
                    && ConfigServiceUrl != null && ConfigServiceUrl.Trim().Length > 0);
        }
    }
}

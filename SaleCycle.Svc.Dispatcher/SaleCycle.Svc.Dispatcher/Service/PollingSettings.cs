using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleCycle.Svc.Dispatcher
{
    public class PollingSettings
    {
        public string BaseUrl { get; set; }

        public string ClientId { get; set; }
        public int PollingIntervalMinutes { get; set; }
    }
}

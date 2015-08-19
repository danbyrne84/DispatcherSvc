using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleCycle.Svc.Dispatcher.Models
{
    public class DispatchConfig
    {
        public int ClientId { get; set; }
        public int PollingIntervalMinutes { get; set; }
        public string Provider { get; set; }
    }
}

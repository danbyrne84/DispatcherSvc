using System;

namespace SaleCycle.Svc.Dispatcher.Models
{
    public class SmsDispatch
    {
        public Guid ApiKey { get; set; }
        public string ProviderId { get; set; }

        public string To { get; set; }
        public string From { get; set; }
        public string Text { get; set; }

        public SmsDispatch()
        {
            
        }
    }
}

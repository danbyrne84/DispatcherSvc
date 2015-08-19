using System;

namespace SaleCycle.Svc.Dispatcher.Models
{
    public class EmailDispatch
    {
        public Guid ApiKey { get; set; }

        public string ProviderId { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        
        public bool Retry { get; set; }
        public int Retries { get; set; }
    }
}

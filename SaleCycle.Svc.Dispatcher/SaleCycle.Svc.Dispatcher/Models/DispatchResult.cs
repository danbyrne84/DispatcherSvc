using System;

namespace SaleCycle.Svc.Dispatcher.Models
{
    public class DispatchResult<T> where T : class
    {
        public bool Processed { get; set; }
        public bool Dispatched { get; set; }
        public Guid Reference { get; set; }
    }
}

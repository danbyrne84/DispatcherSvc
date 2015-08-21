using System;

namespace SaleCycle.Svc.Dispatcher.Contract
{
    public class DispatchResult<T> where T : IDispatch
    {
        public bool Processed { get; set; }
        public bool Dispatched { get; set; }
        
        public Guid Reference { get; set; }

        public bool Error { get; set; }
        public string ErrorText { get; set; }
    }
}

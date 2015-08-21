using SaleCycle.Svc.Dispatcher.Contract.Extensions;

namespace SaleCycle.Svc.Dispatcher.Contract
{
    public class SmsDispatch : ISmsDispatch
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Text { get; set; }
        public string Provider { get; set; }

        public bool IsValid()
        {
            return (To.IsNotNullOrEmpty() && From.IsNotNullOrEmpty() && Text.IsNotNullOrEmpty() &&
                   Provider.IsNotNullOrEmpty());
        }
    }
}

namespace SaleCycle.Svc.Dispatcher.Contract
{
    public class DispatchConfig
    {
        public int ClientId { get; set; }
        public int PollingIntervalMinutes { get; set; }
        public string Provider { get; set; }
    }
}

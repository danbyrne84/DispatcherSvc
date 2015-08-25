namespace SaleCycle.Svc.Dispatcher.Contract
{
    public interface ISmsDispatch : IDispatch
    {
        string To { get; set; }
        string From { get; set; }
        string Text { get; set; }
    }
}
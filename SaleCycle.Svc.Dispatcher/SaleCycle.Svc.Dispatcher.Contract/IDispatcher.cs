using System.Collections.Generic;

namespace SaleCycle.Svc.Dispatcher.Contract
{
    public interface IDispatcher<T> where T : ISmsDispatch
    {
        IEnumerable<DispatchResult<T>> Dispatch(IEnumerable<T> dispatches);
        DispatchResult<T> Dispatch(T dispatch);
    }
}

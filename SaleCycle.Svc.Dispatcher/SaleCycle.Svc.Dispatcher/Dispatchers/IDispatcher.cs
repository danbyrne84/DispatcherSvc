using System.Collections.Generic;
using SaleCycle.Svc.Dispatcher.Models;

namespace SaleCycle.Svc.Dispatcher.Dispatchers
{
    public interface IDispatcher<T> where T : class
    {
        IEnumerable<DispatchResult<T>> Dispatch(IEnumerable<T> dispatches);
        DispatchResult<T> Dispatch(T dispatch);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaleCycle.Svc.Dispatcher.Models;

namespace SaleCycle.Svc.Dispatcher.Dispatchers
{
    public class SmsDispatcher : IDispatcher<SmsDispatch>
    {
        public IEnumerable<DispatchResult<SmsDispatch>> Dispatch(IEnumerable<SmsDispatch> dispatches)
        {
            throw new NotImplementedException();
        }

        public DispatchResult<SmsDispatch> Dispatch(SmsDispatch dispatch)
        {
            throw new NotImplementedException();
        }
    }
}

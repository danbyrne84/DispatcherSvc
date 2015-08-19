using System;
using System.Collections.Generic;
using SaleCycle.Svc.Dispatcher.Dispatchers;
using SaleCycle.Svc.Dispatcher.Models;

namespace SaleCycle.Dispatchers.Sms.Dynmark
{
    public class Dynmark : IDispatcher<SmsDispatch>
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

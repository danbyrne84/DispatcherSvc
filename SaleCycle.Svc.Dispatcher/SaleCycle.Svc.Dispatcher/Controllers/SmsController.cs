using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SaleCycle.Svc.Dispatcher.Contract;
using SaleCycle.Svc.Dispatcher.Factories;

namespace SaleCycle.Svc.Dispatcher.Controllers
{
    public class SmsController : ApiController
    {
        [Route("api/dispatch/sms")]
        public HttpResponseMessage Post(SmsDispatch dispatch)
        {
            try
            {
                // validate
                if (dispatch.IsValid() == false)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                // retrieve dispatcher
                var dispatcher = SmsDispatcherFactory.GetDispatcherByName(dispatch.Provider, dispatch.Settings);
                if (dispatcher == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                
                dispatcher.Dispatch(dispatch);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError); //@todo better response code
            }

            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SaleCycle.Svc.Dispatcher.Models;

namespace SaleCycle.Svc.Dispatcher.Controllers
{
    public class SmsController : ApiController
    {
        [Route("api/dispatch/sms")]
        public HttpResponseMessage Post(List<SmsDispatch> dispatches)
        {
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}

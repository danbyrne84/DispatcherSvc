using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SaleCycle.Svc.Dispatcher.Models;

namespace SaleCycle.Svc.Dispatcher.Controllers
{
    public class EmailController : ApiController
    {
        [Route("api/dispatch/email")]
        public HttpResponseMessage Post(List<EmailDispatch> email)
        {
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}

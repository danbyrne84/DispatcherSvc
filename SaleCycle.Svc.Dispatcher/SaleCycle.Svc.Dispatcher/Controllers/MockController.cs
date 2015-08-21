using System.Collections.Generic;
using System.Web.Http;
using SaleCycle.Svc.Dispatcher.Contract;

namespace SaleCycle.Svc.Dispatcher.Controllers
{
    public class MockController : ApiController
    {
        // mock config endpoint - until PB is done with data layer
        [HttpGet]
        [Route("api/dispatch/configs")]
        public List<DispatchConfig> GetConfigs()
        {
            return new List<DispatchConfig>
            {
                new DispatchConfig
                {
                    ClientId = 1000,
                    PollingIntervalMinutes = 10,
                    Provider = "dynmark"
                }
            };
        }

        [HttpGet]
        [Route("api/dispatch")]
        public List<ISmsDispatch> GetSmsForDispatch()
        {
            return new List<ISmsDispatch>
            {
                new SmsDispatch
                {

                }
            };
        }
    }
}
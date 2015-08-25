using System.Collections.Generic;
using System.Web.Http;
using SaleCycle.Dispatchers.Sms.Dynmark;
using SaleCycle.Svc.Dispatcher.Contract;

namespace SaleCycle.Svc.Dispatcher.Controllers
{
    public class MockController : ApiController
    {
        private readonly Dictionary<string, object> _dynmarkSettings;

        public MockController()
        {
            _dynmarkSettings = new Dictionary<string, object>
            {
                {"User", "salescycletest"},
                {"Password", ".S@l3Cycl3"},
                {"Url", "https://services.dynmark.com/HttpServices/"}
            };
        }

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
                    Provider = "dynmark",
                    PollingIntervalMinutes = 10,
                    Settings = _dynmarkSettings
                }
            };
        }

        [HttpGet]
        [Route("api/dispatch/{clientId}")]
        public List<SmsDispatch> GetSmsForDispatch(int clientId)
        {
            var dispatches = new List<SmsDispatch>
            {
                new SmsDispatch
                {
                    Provider = "dynmark",
                    Settings = _dynmarkSettings,
                    To = "447891832416",
                    From = "SaleCycle",
                    Text = "testing"
                }
            };

            return dispatches;
        }
    }
}
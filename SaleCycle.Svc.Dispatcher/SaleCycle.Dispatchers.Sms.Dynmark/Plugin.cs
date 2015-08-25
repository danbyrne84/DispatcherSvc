using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SaleCycle.Svc.Dispatcher.Contract;

namespace SaleCycle.Dispatchers.Sms.Dynmark
{
    public class Plugin : ISmsDispatcher
    {
        private UriBuilder Uri { get; set; }
        private Dictionary<string, object> Settings { get; set; }

        public Plugin(Dictionary<string, object> settings)
        {
            Settings = settings;

            Uri = new UriBuilder(Settings["Url"].ToString());
            Uri.Path += "SendMessage.ashx";

            Uri.Query = AppendQueryValue(Uri.Query, "user", Settings["User"].ToString());
            Uri.Query = AppendQueryValue(Uri.Query, "password", Settings["Password"].ToString());
        }

        public IEnumerable<DispatchResult<ISmsDispatch>> Dispatch(IEnumerable<ISmsDispatch> dispatches)
        {
            var results = new List<DispatchResult<ISmsDispatch>>();

            Parallel.ForEach(dispatches, dispatch =>
            {
                results.Add(Dispatch(dispatch));
            });

            return results;
        }

        public DispatchResult<ISmsDispatch> Dispatch(ISmsDispatch dispatch)
        {
            string returnValue = null;

            Uri.Query = AppendQueryValue(Uri.Query, "to", dispatch.To);
            Uri.Query = AppendQueryValue(Uri.Query, "from", dispatch.From);
            Uri.Query = AppendQueryValue(Uri.Query, "text", dispatch.Text);

            var request = (HttpWebRequest)WebRequest.Create(Uri.ToString());
            request.Method = "GET";
            request.ContentLength = 0;
            request.Timeout = 130000; // Set the Dynmark recommended timeout of 130 seconds 

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                /**
                // Read the XML from the response - don't bother doing this if successful
                using (var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    returnValue = sr.ReadToEnd();
                }
                **/

                return new DispatchResult<ISmsDispatch> { Dispatched = true, Processed = true, Error = false, Reference = Guid.NewGuid() };
            }
            catch (WebException ex)
            {
                return DispatchResultFromWebEx(ex);
            }
        }

        private DispatchResult<ISmsDispatch> DispatchResultFromWebEx(WebException ex)
        {
            var response = String.Empty;

            // An error has occurred; read the error XML if possible. 
            if (ex.Response != null)
            {
                var stream = ex.Response.GetResponseStream();

                if (stream == null)
                {
                    response = "No response from API call";
                }
                else
                {
                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        response = sr.ReadToEnd();
                    }
                }
            }
            else
            {
                response = ex.Status.ToString();
            }

            return new DispatchResult<ISmsDispatch>{ Dispatched = false, Processed = true, Error = true, ErrorText = response };
        }

        private static string AppendQueryValue(string currentQueryString, string name, string value)
        {
            var returnQueryString = currentQueryString;
            
            if (!string.IsNullOrEmpty(currentQueryString))
            {
                if (returnQueryString.StartsWith("?"))
                {
                    returnQueryString = returnQueryString.Substring(1);
                }
                returnQueryString += string.Format("&{0}={1}", HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(value));
            }
            else
            {
                returnQueryString = string.Format("{0}={1}", HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(value));
            }
            return returnQueryString;
        }
    }
}

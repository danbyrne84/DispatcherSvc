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

        public Plugin()
        {
            Uri = new UriBuilder("https://services.dynmark.com/HttpServices/");
            Uri.Path += "SendMessage.ashx";
            Uri.Query = AppendQueryValue(Uri.Query, "user", "secret");
            Uri.Query = AppendQueryValue(Uri.Query, "password", "secret");
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
            //Uri.Query = AppendQueryValue(Uri.Query, "to", "447534470501"); // !!Michael Batty's Mobile number

            HttpStatusCode lastHttpStatus = HttpStatusCode.Accepted; // Add the required method page name.
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
                lastHttpStatus = response.StatusCode; // Get the HTTP return code. 

                // Read the XML from the response. 
                using (var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    returnValue = sr.ReadToEnd();
                }

                return new DispatchResult<ISmsDispatch> { Dispatched = true, Processed = true, Reference = Guid.NewGuid() };
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
            var returnQueryString = currentQueryString; // Add new name value pair. 
            if (!string.IsNullOrEmpty(currentQueryString))
            {
                // Trim any leading ?'s 
                if (returnQueryString.StartsWith("?"))
                {
                    returnQueryString = returnQueryString.Substring(1);
                } // Append Query String value. 
                returnQueryString += string.Format("&{0}={1}", HttpUtility.UrlEncode(name),
                    HttpUtility.UrlEncode(value));
            }
            else
            {
                // First Query String value. 
                returnQueryString = string.Format("{0}={1}", HttpUtility.UrlEncode(name),
                    HttpUtility.UrlEncode(value));
            }
            return returnQueryString;
        }
    }
}

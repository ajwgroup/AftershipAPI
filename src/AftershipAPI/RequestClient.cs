using System;
using System.IO;
using System.Net;

namespace AftershipAPI
{
    public class RequestClient : IRequestClient
    {
        private readonly string _aftershipApiToken;

        public RequestClient(string aftershipApiToken)
        {
            _aftershipApiToken = aftershipApiToken;
        }

        public string RunRequest(string method, string body, string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Timeout = 150000;

            var header = new WebHeaderCollection
            {
                { "aftership-api-key", _aftershipApiToken }
            };

            request.Headers = header;
            request.ContentType = "application/json";
            request.Method = method;

            if (body != null)
            {
                using var streamWriter = new StreamWriter(request.GetRequestStream());
                streamWriter.Write(body);
            }

            string json_response = JsonResponseFromWebResponse(request);

            Console.WriteLine($"Response request: {json_response}*");
            return json_response;
        }

        internal static string JsonResponseFromWebResponse(HttpWebRequest request)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                var reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                    throw e;
                    //timeout or bad internet conexion
                }
                else
                {
                    //probably Aftership will give more information of the problem, so we read the response
                    HttpWebResponse response = e.Response as HttpWebResponse;
                    using var stream = response.GetResponseStream();
                    using var reader = new StreamReader(stream);
                    string text = reader.ReadToEnd();
                    throw new WebException(text, e);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

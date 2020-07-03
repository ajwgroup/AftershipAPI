using AftershipAPI.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AftershipAPITests")]
namespace AftershipAPI
{
    /// <summary>
    /// Connection API. Connect with the API of Aftership
    /// </summary>
    public class ConnectionAPI : IConnectionAPI
    {
        readonly string _url;
        private readonly IRequestClient _requestClient;
        private const string URL_SERVER = "https://api.aftership.com/";
        private const string VERSION_API = "v4";

        internal ConnectionAPI(string aftershipApiToken, string url, IRequestClient requestClient)
        {
            _url = url ?? URL_SERVER;
            _requestClient = requestClient ?? new RequestClient(aftershipApiToken);
        }

        /// <summary>
        /// Constructor ConnectionAPI
        /// </summary>
        /// <param name="aftershipApiToken"> Afthership token for the connection</param>
        /// <param name="url">Override the aftership url. default is "https://api.aftership.com/"</param>
        /// <returns></returns>
        public ConnectionAPI(string aftershipApiToken, string url) : this(aftershipApiToken, url, new RequestClient(aftershipApiToken)){
            if (string.IsNullOrWhiteSpace(aftershipApiToken))
            {
                throw new ArgumentException($"'{nameof(aftershipApiToken)}' cannot be null or whitespace", nameof(aftershipApiToken));
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException($"'{nameof(url)}' cannot be null or empty", nameof(url));
            }
        }

        /// <summary>
        /// Constructor ConnectionAPI
        /// </summary>
        /// <param name="aftershipApiToken"> Afthership token for the connection</param>
        /// <returns></returns>
        public ConnectionAPI(string aftershipApiToken) : this(aftershipApiToken, URL_SERVER)
        {
            if (string.IsNullOrWhiteSpace(aftershipApiToken))
            {
                throw new ArgumentException($"'{nameof(aftershipApiToken)}' cannot be null or whitespace", nameof(aftershipApiToken));
            }
        }

        /// <summary>
        /// Updates a tracking of your account
        /// </summary>
        /// <param name="tracking">  
        /// A Tracking object with the information to update
        /// The fields TrackingNumber and Slug SHOULD be informed, otherwise an exception will be thrown
        /// The fields an user can update are: smses, emails, title, customerName, orderID, orderIDPath,
        /// customFields</param>
        /// <returns>The last Checkpoint object</returns>
        public Tracking PutTracking(Tracking tracking)
        {
            return Request("PUT", $"/trackings/{ParametersExtra(tracking)}", JsonConvert.SerializeObject(tracking)).Data.Tracking;
        }

        private string ParametersExtra(Tracking tracking)
        {
            string queryParameters = tracking.GetQueryRequiredFields();
            return (!string.IsNullOrEmpty(tracking.Id)) ? tracking.Id : $"{tracking.Slug}/{tracking.TrackingNumber}{queryParameters}";
        }

        /// <summary>
        /// Return the tracking information of the last checkpoint of a single tracking
        /// </summary>
        /// <param name="tracking"> A Tracking to get the last checkpoint of, it should have tracking number and Slug at least</param>
        /// <returns>The last Checkpoint object</returns>
        public Checkpoint GetLastCheckpoint(Tracking tracking)
        {
            string parametersExtra = ParametersExtra(tracking);

            return Request("GET", $"/last_checkpoint/{parametersExtra}", null).Data.Checkpoint;
        }

        /// <summary>
        /// Return the tracking information of the last checkpoint of a single tracking
        /// </summary>
        /// <param name="tracking"> A Tracking to get the last checkpoint of, it should have tracking number and Slug at least.</param>
        /// <param name="fields"> A list of fields of checkpoint wanted to be in the response</param>
        /// <param name="lang"> A String with the language desired. Support Chinese to English translation
        ///                      for china-ems and china-post only</param>
        /// <returns>The last Checkpoint object</returns>
        public Checkpoint GetLastCheckpoint(Tracking tracking, List<FieldCheckpoint> fields, string lang)
        {
            var qs = new Querystring();

            if (fields != null && fields.Any()) qs.Add("fields", string.Join(",", fields));
            if (!string.IsNullOrEmpty(lang)) qs.Add("lang", lang);

            string parameters = qs.ToString();

            string parametersExtra = UseTrackingIdOrExtraParameters(tracking, parameters);

            var response = Request("GET", $"/last_checkpoint/{parametersExtra}", null);

            return response.Data.Checkpoint;
        }

        private static string UseTrackingIdOrExtraParameters(Tracking tracking, string parameters)
        {
            return (!string.IsNullOrEmpty(tracking.Id)) ?
                $"{tracking.Id}{parameters}" :
                $"{tracking.Slug}/{tracking.TrackingNumber}{parameters}{tracking.GetQueryRequiredFields()}";
        }

        /// <summary>
        /// Retrack an expired tracking once
        /// </summary>
        /// <param name="tracking"> tracking A Tracking to reactivate, it should have tracking number and Slug at least.</param>
        /// <param name="fields"> A list of fields of checkpoint wanted to be in the response</param>
        /// <param name="lang"> A String with the language desired. Support Chinese to English translation
        ///                      for china-ems and china-post only</param>
        /// <returns> A JSONObject with the response. It will contain the status code of the operation, TrackingNumber,
        ///         Slug and active (to true)</returns>
        public bool Retrack(Tracking tracking)
        {
            var response = Request("POST", $"/trackings/{tracking.Slug}/{tracking.TrackingNumber}/retrack{tracking.GetQueryRequiredFields()}", null);

            return IsOKResponse(response) && response.Data.Tracking.Active;
        }

        private static bool IsOKResponse(Response response) => response.Meta.Code == 200;

        /// <summary>
        ///Get as much as 100 trackings from your account, created less than 30 days ago. If you delete right before,
        ///         you may obtain less than 100 trackings.
        /// </summary>
        /// <param name="param">page Indicated the page of 100 trackings to return, if page is 1 will return the first 100,
        ///  if is 2 -> 100-200 etc</param> 
        /// <returns> A List of Tracking Objects from your account. Max 100 trackings
        public List<Tracking> GetTrackings(int page)
        {
            return Request("GET", $"/trackings?limit=100&page={page}", null)?.Data?.Trackings;
        }

        /// <summary>
        ///Get trackings from your account with the ParametersTracking defined in the params
        /// </summary>
        /// <param name="parameters"> ParametersTracking Object, with the information to get
        /// <returns> A List of Tracking Objects from your account.
        public List<Tracking> GetTrackings(ParametersTracking parameters)
        {
            var response = Request("GET", $"/trackings?{parameters.GenerateQueryString()}", null);
            var trackings = response.Data.Trackings;

            if (trackings.Count != 0)
            {
                parameters.Total = response.Data.Count;
                return trackings.ToList();
            }

            return Enumerable.Empty<Tracking>().ToList();
        }

        /// <summary>
        ///Return a list of couriers supported by AfterShip along with their names, URLs and slugs
        /// </summary>
        /// <returns>A list of Object Courier, with all the couriers supported by the API
        public List<Courier> GetAllCouriers()
        {
            var response = Request("GET", "/couriers/all", null);

            return ListOfCouriersFromJsonDataCouriers(response);
        }

        /// <summary>
        ///Return a list of user couriers enabled by user's account along their names, URLs, slugs, required fields.
        /// </summary>
        /// <returns>A list of Object Courier, with all the couriers supported by the API
        public List<Courier> GetCouriers()
        {
            var response = Request("GET", "/couriers", null);
            return ListOfCouriersFromJsonDataCouriers(response);
        }

        private static List<Courier> ListOfCouriersFromJsonDataCouriers(Response response)
        {
            return response.Data.Couriers;
        }

        /// <summary>
        /// Get a list of matched couriers for a tracking number based on the tracking number format 
        /// Note, only check the couriers you have defined in your account
        /// </summary>
        /// <param name="trackingNumber"> tracking number to match with couriers
        /// <returnsA List of Couriers objects that match the provided TrackingNumber
        public List<Courier> DetectCouriers(string trackingNumber)
        {
            return DetectCouriers(trackingNumber, "", "", "", new List<string>());
        }

        /// <summary>
        /// Get a list of matched couriers for a tracking number based on the tracking number format Note, only check the couriers you have defined in your account
        /// Note, only check the couriers you have defined in your account
        /// </summary>
        /// <param name="trackingNumber"> Tracking number to match with couriers (mandatory)</param>
        /// <param name="trackingPostalCode"> tracking number to match with couriers
        /// <param name="trackingShipDate">sually it is refer to the posting date of the shipment, format in YYYYMMDD.
        /// Required by some couriers, such as `deutsch-post`.(optional)</param>
        /// <param name="trackingAccountNumber"> The account number for particular courier. Required by some couriers, 
        /// such as `dynamic-logistics`.(optional)</param>
        /// <param name="slugs"> The Slug of couriers to detect.
        /// <returns>A List of Couriers objects that match the provided TrackingNumber</returns>
        public List<Courier> DetectCouriers(string trackingNumber, string trackingPostalCode, string trackingShipDate,
            string trackingAccountNumber, List<string> slugs)
        {
            if (string.IsNullOrEmpty(trackingNumber))
                throw new ArgumentException("Tracking number should always be provided to the method detectCouriers");

            var tracking = new JObject()
            {
                { "tracking_number", new JValue(trackingNumber) }
            };

            if (!string.IsNullOrEmpty(trackingPostalCode))
                tracking.Add("tracking_postal_code", new JValue(trackingPostalCode));

            if (!string.IsNullOrEmpty(trackingShipDate))
                tracking.Add("tracking_ship_date", new JValue(trackingShipDate));

            if (!string.IsNullOrEmpty(trackingAccountNumber))
                tracking.Add("tracking_account_number", new JValue(trackingAccountNumber));

            if (slugs != null && slugs.Any())
                tracking.Add("Slug", new JArray(slugs));

            var body = new JObject
            {
                { "tracking", tracking }
            };

            var response = Request("POST", "/couriers/detect", body.ToString());

            return ListOfCouriersFromJsonDataCouriers(response);
        }

        /// <summary>
        /// Get next page of Trackings from your account with the ParametersTracking defined in the params
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>The next page of Tracking Objects from your account</returns>
        public List<Tracking> GetTrackingsNext(ParametersTracking parameters)
        {
            parameters.Page += 1;
            return GetTrackings(parameters);
        }

        /// <summary>
        /// A Tracking object with the information to creates
        ///	The field TrackingNumber SHOULD be informed, otherwise an exception will be thrown
        /// The fields an user can add are: Slug, smses, emails, title, customerName, orderID, orderIDPath,
        /// customFields, destinationCountryISO3 (the others are provided by the Server)
        /// </summary>
        /// <param name="tracking"></param>
        /// <returns> A Tracking object with the fields in the same state as the server, if a field has an error,
        ///          it won't be added, and won't be shown in the response (for example if the smses
        ///		     phone number is not valid). This response doesn't have checkpoints informed!</returns>
        public Tracking CreateTracking(Tracking tracking)
        {
            string tracking_json = JsonConvert.SerializeObject(tracking);

            var response = Request("POST", "/trackings", tracking_json);

            return response.Data.Tracking;
        }

        /// <summary>
        /// Delete a tracking from your account, if the tracking.Id property is defined
        /// it will delete that tracking from the system, if not it will take the 
        /// tracking tracking.number and the tracking.Slug for identify the tracking
        /// </summary>
        /// <param name="tracking"> A Tracking to delete</param>
        /// <returns>A boolean, true if delete correctly, and false otherwise</returns>
        public bool DeleteTracking(Tracking tracking)
        {
            string parametersAll;
            if (!string.IsNullOrEmpty(tracking.Id))
            {
                parametersAll = tracking.Id;
            }
            else
            {
                //get the require fields if any (postal_code, tracking_account etc..)
                parametersAll = $"{tracking.Slug}/{tracking.TrackingNumber}";
            }

            return IsOKResponse(Request("DELETE", $"/trackings/{parametersAll}", null));
        }

        /// <summary>
        /// Get a specific tracking from your account. If the trackingGet.Id property 
        /// is defined it will get that tracking from the system, if not it will take 
        /// the tracking tracking.number and the tracking.Slug for identify the tracking
        /// </summary>
        /// <param name="trackingGet">A Tracking to get.</param></param>
        /// <returns> A Tracking object with the response</returns>
        public Tracking GetTrackingByNumber(Tracking trackingGet)
        {
            string parametersExtra;

            if (!string.IsNullOrEmpty(trackingGet.Id))
            {
                parametersExtra = trackingGet.Id;
            }
            else
            {
                //get the require fields if any (postal_code, tracking_account etc..)
                parametersExtra = $"{trackingGet.Slug}/{trackingGet.TrackingNumber}{trackingGet.GetQueryRequiredFields()}";
            }
            
            var response = Request("GET", $"/trackings/{parametersExtra}", null);

            return response.Data.Tracking;
        }

        /// <summary>
        ///  Get a specific tracking from your account. If the trackingGet.Id property 
        /// is defined it will get that tracking from the system, if not it will take 
        /// the tracking tracking.number and the tracking.Slug for identify the tracking
        /// </summary>
        /// <param name="trackingGet">A Tracking to get</param>
        /// <param name="fields">A list of fields wanted to be in the response</param>
        /// <param name="lang">A String with the language desired. Support Chinese to 
        /// English translation for china-ems and china-post only</param>
        /// <returns></returns>
        public Tracking GetTrackingByNumber(Tracking trackingGet, List<FieldTracking> fields, string lang)
        {
            string parametersAll;

            var qs = new Querystring();

            if (fields.Any())
                qs.Add("fields", ParseListFieldTracking(fields));

            if (!string.IsNullOrEmpty(lang))
                qs.Add("lang", lang);

            //encode the fields required
            string params_query = qs.ToString();

            if (!string.IsNullOrEmpty(trackingGet.Id))
            {
                parametersAll = $"{trackingGet.Id}{params_query}";
            }
            else
            {
                //get the require fields if any (postal_code, tracking_account etc..)
                string paramRequiredFields = trackingGet.GetQueryRequiredFields();
                parametersAll = $"{trackingGet.Slug}/{trackingGet.TrackingNumber}{params_query}{paramRequiredFields}";
            }

            var response = Request("GET", $"/trackings/{parametersAll}", null);

            return response.Data.Tracking;
        }

        /// <summary>
        /// Make a request to the HTTP API of Aftership
        /// </summary>
        ///<param name="method">String with the method of the request: GET, POST, PUT, DELETE</param> 
        ///<param name="urlResource">String with the URL of the request</param> 
        ///<param name="body">String JSON with the body of the request, 
        /// if the request doesn't need body "GET/DELETE", the bodywould be null</param> 
        /// <returns>A String JSON with the response of the request</returns>
        public Response Request(string method, string urlResource, string body)
        {
            string url = $"{_url}{VERSION_API}{urlResource}";
            string json_response = _requestClient.RunRequest(method, body, url);

            return JsonConvert.DeserializeObject<Response>(json_response, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }); ;
        }      

        /// Parse a List<FieldTracking> to List<String>
        public List<string> ParseListFieldTracking(List<FieldTracking> list) => list.Select(elemement => elemement.ToString()).ToList();
    }
}
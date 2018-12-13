using AftershipAPI.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace AftershipAPI
{
    /// <summary>
    /// Connection API. Connect with the API of Afthership
    /// </summary>
    public class ConnectionAPI
    {
        readonly string _tokenAftership;
        readonly string _url;
        private static readonly string URL_SERVER = "https://api.aftership.com/";

        //private static String URL_SERVER = "http://localhost:3001/";
        private static readonly string VERSION_API = "v4";

        /// <summary>
        /// Constructor ConnectionAPI
        /// </summary>
        /// <param name="tokenAfthership"> Afthership token for the connection</param>
        /// <returns></returns>
        public ConnectionAPI(string tokenAfthership, string url = null)
        {
            _tokenAftership = tokenAfthership;
            _url = url ?? URL_SERVER;
        }

        /// <summary>
        /// Updates a tracking of your account
        /// </summary>
        /// <param name="tracking">  A Tracking object with the information to update
        ///                The fields TrackingNumber and Slug SHOULD be informed, otherwise an exception will be thrown
        ///               The fields an user can update are: smses, emails, title, customerName, orderID, orderIDPath,
        ///               customFields</param>
        /// <returns>The last Checkpoint object</returns>
        public Tracking PutTracking(Tracking tracking)
        {
            string parametersExtra = "";

            if (tracking.Id != null && !(tracking.Id.CompareTo("") == 0))
            {
                parametersExtra = tracking.Id;
            }
            else
            {
                string paramRequiredFields = ReplaceFirst(tracking.GetQueryRequiredFields(), "&", "?");
                parametersExtra = tracking.Slug + "/" + tracking.TrackingNumber + paramRequiredFields;
            }

            JObject response = Request("PUT", "/trackings/" + parametersExtra, tracking.GeneratePutJSON());

            return new Tracking((JObject)response["data"]["tracking"]);

        }
        /// <summary>
        /// Return the tracking information of the last checkpoint of a single tracking
        /// </summary>
        /// <param name="tracking"> A Tracking to get the last checkpoint of, it should have tracking number and Slug at least</param>
        /// <returns>The last Checkpoint object</returns>
        public Checkpoint GetLastCheckpoint(Tracking tracking)
        {
            string parametersExtra = "";

            if (!string.IsNullOrEmpty(tracking.Id))
            {
                parametersExtra = tracking.Id;
            }
            else
            {
                string paramRequiredFields = ReplaceFirst(tracking.GetQueryRequiredFields(), "&", "?");
                parametersExtra = tracking.Slug + "/" + tracking.TrackingNumber + paramRequiredFields;
            }

            JObject response = Request("GET", "/last_checkpoint/" + parametersExtra, null);

            JObject checkpointJSON = (JObject)response["data"]["checkpoint"];

            return (checkpointJSON.Count != 0) ? new Checkpoint(checkpointJSON) : null;
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

            string parameters = null;
            Querystring qs = new Querystring();

            if (fields != null) qs.Add("fields", string.Join(",", fields));
            if (lang != null && !lang.Equals("")) qs.Add("lang", lang);
            parameters = ReplaceFirst(qs.ToString(), "&", "?");

            string parametersExtra = "";
            if (tracking.Id != null && !tracking.Id.Equals(""))
            {
                parametersExtra = tracking.Id + parameters;
            }
            else
            {
                string paramRequiredFields = tracking.GetQueryRequiredFields();
                parametersExtra = tracking.Slug + "/" + tracking.TrackingNumber + parameters + paramRequiredFields;
            }

            JObject response = Request("GET", "/last_checkpoint/" + parametersExtra, null);

            JObject checkpointJSON = (JObject)response["data"]["checkpoint"];

            return (checkpointJSON.Count != 0) ? new Checkpoint(checkpointJSON) : null;
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
            string paramRequiredFields = ReplaceFirst(tracking.GetQueryRequiredFields(), "&", "?");

            JObject response = Request("POST", "/trackings/" + tracking.Slug +
                "/" + tracking.TrackingNumber + "/retrack" + paramRequiredFields, null);

            if ((int)response["meta"]["code"] == 200)
            {
                return (bool)response["data"]["tracking"]["active"];
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        ///Get as much as 100 trackings from your account, created less than 30 days ago. If you delete right before,
        ///         you may obtain less than 100 trackings.
        /// </summary>
        /// <param name="param">page Indicated the page of 100 trackings to return, if page is 1 will return the first 100,
        ///  if is 2 -> 100-200 etc</param> 
        /// <returns> A List of Tracking Objects from your account. Max 100 trackings

        public List<Tracking> GetTrackings(int page)
        {
            JObject response = Request("GET", "/trackings?limit=100&page=" + page, null);
            JArray trackingJSON = (JArray)response["data"]["trackings"];
            if (trackingJSON.Count != 0)
            {
                var trackingList = new List<Tracking>();

                for (int i = 0; i < trackingJSON.Count; i++)
                {
                    trackingList.Add(new Tracking((JObject)trackingJSON[i]));
                }

                return trackingList;
            }
            return null;

        }

        /// <summary>
        ///Get trackings from your account with the ParametersTracking defined in the params
        /// </summary>
        /// <param name="parameters"> ParametersTracking Object, with the information to get
        /// <returns> A List of Tracking Objects from your account.
        public List<Tracking> GetTrackings(ParametersTracking parameters)
        {
            List<Tracking> trackingList = null;
            int size = 0;
            JObject response = Request("GET", "/trackings?" + parameters.GenerateQueryString(), null);
            JArray trackingJSON = (JArray)response["data"]["trackings"];
            if (trackingJSON.Count != 0)
            {
                size = (int)response["data"]["count"];
                trackingList = new List<Tracking>();
                for (int i = 0; i < trackingJSON.Count; i++)
                {
                    trackingList.Add(new Tracking((JObject)trackingJSON[i]));
                }
                parameters.Total = size;
            }
            return trackingList;
        }


        /// <summary>
        ///Return a list of couriers supported by AfterShip along with their names, URLs and slugs
        /// </summary>
        /// <returns>A list of Object Courier, with all the couriers supported by the API
        public List<Courier> GetAllCouriers()
        {
            JObject response = Request("GET", "/couriers/all", null);

            JArray couriersJSON = (JArray)response["data"]["couriers"];
            List<Courier> couriers = new List<Courier>(couriersJSON.Count);

            for (int i = 0; i < couriersJSON.Count; i++)
            {
                JObject element = (JObject)couriersJSON[i];

                Courier newCourier = new Courier(element);
                couriers.Add(newCourier);
            }
            return couriers;
        }

        /// <summary>
        ///Return a list of user couriers enabled by user's account along their names, URLs, slugs, required fields.
        /// </summary>
        /// <returns>A list of Object Courier, with all the couriers supported by the API
        public List<Courier> GetCouriers()
        {
            JObject response = Request("GET", "/couriers", null);

            JArray couriersJSON = (JArray)response["data"]["couriers"];
            List<Courier> couriers = new List<Courier>(couriersJSON.Count);

            for (int i = 0; i < couriersJSON.Count; i++)
            {
                JObject element = (JObject)couriersJSON[i];

                var newCourier = new Courier(element);
                couriers.Add(newCourier);
            }
            return couriers;
        }

        /// <summary>
        ///Get a list of matched couriers for a tracking number based on the tracking number format 
        /// Note, only check the couriers you have defined in your account
        /// </summary>
        /// <param name="TrackingNumber"> tracking number to match with couriers
        /// <returnsA List of Couriers objects that match the provided TrackingNumber
        public List<Courier> DetectCouriers(string TrackingNumber)
        {
            // trackingJSON.Add("Slug",new JValue(_slug));

            if (TrackingNumber == null || TrackingNumber.Equals(""))
                throw new ArgumentException("The tracking number should be always informed for the method detectCouriers");

            var tracking = new JObject
            {
                { "tracking_number", new JValue(TrackingNumber) }
            };

            var body = new JObject
            {
                { "tracking", tracking }
            };
            JObject response = Request("POST", "/couriers/detect", body.ToString());
            List<Courier> couriers = new List<Courier>();

            JArray couriersJSON = (JArray)response["data"]["couriers"];

            for (int i = 0; i < couriersJSON.Count; i++)
            {
                JObject element = (JObject)couriersJSON[i];

                Courier newCourier = new Courier(element);
                couriers.Add(newCourier);
            }
            return couriers;
        }


        /// <summary>
        ///Get a list of matched couriers for a tracking number based on the tracking number format Note, only check the couriers you have defined in your account
        /// Note, only check the couriers you have defined in your account
        /// </summary>
        /// <param name="TrackingNumber"> Tracking number to match with couriers (mandatory)</param>
        /// <param name="trackingPostalCode"> tracking number to match with couriers
        /// <param name="trackingShipDate">sually it is refer to the posting date of the shipment, format in YYYYMMDD.
        /// Required by some couriers, such as `deutsch-post`.(optional)</param>
        /// <param name="trackingAccountNumber"> The account number for particular courier. Required by some couriers, 
        /// such as `dynamic-logistics`.(optional)</param>
        /// <param name="slugs"> The Slug of couriers to detect.
        /// <returns>A List of Couriers objects that match the provided TrackingNumber</returns>

        public List<Courier> DetectCouriers(string TrackingNumber, string trackingPostalCode, string trackingShipDate,
            string trackingAccountNumber, List<string> slugs)
        {
            JObject body = new JObject();
            JObject tracking = new JObject();

            if (TrackingNumber == null || TrackingNumber.Equals(""))
                throw new ArgumentException("Tracking number should be always informed for the method detectCouriers");
            tracking.Add("tracking_number", new JValue(TrackingNumber));

            if (trackingPostalCode != null && !trackingPostalCode.Equals(""))
                tracking.Add("tracking_postal_code", new JValue(trackingPostalCode));
            if (trackingShipDate != null && !trackingShipDate.Equals(""))
                tracking.Add("tracking_ship_date", new JValue(trackingShipDate));
            if (trackingAccountNumber != null && !trackingAccountNumber.Equals(""))
                tracking.Add("tracking_account_number", new JValue(trackingAccountNumber));

            if (slugs != null && slugs.Count != 0)
            {
                JArray slugsJSON = new JArray(slugs);
                tracking.Add("Slug", slugsJSON);
            }

            body.Add("tracking", tracking);

            JObject response = Request("POST", "/couriers/detect", body.ToString());
            List<Courier> couriers = new List<Courier>();

            JArray couriersJSON = (JArray)response["data"]["couriers"];
            JObject element;

            for (int i = 0; i < couriersJSON.Count; i++)
            {
                element = (JObject)couriersJSON[i];

                Courier newCourier = new Courier(element);
                couriers.Add(newCourier);
            }
            return couriers;
        }

        /// <summary>
        ///Get next page of Trackings from your account with the ParametersTracking defined in the params
        /// </summary>
        /// <param name="parameters"> ParametersTracking Object, with the information to get
        /// <returns> The next page of Tracking Objects from your account


        public List<Tracking> GetTrackingsNext(ParametersTracking parameters)
        {
            parameters.Page = parameters.Page + 1;
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
            string tracking_json = tracking.GetJSONPost();

            JObject response = Request("POST", "/trackings", tracking_json);

            return new Tracking((JObject)response["data"]["tracking"]);
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
            if (tracking.Id != null && !tracking.Id.Equals(""))
            {
                parametersAll = tracking.Id;

            }
            else
            {
                //get the require fields if any (postal_code, tracking_account etc..)
                parametersAll = tracking.Slug + "/" + tracking.TrackingNumber;
            }
            JObject response = Request("DELETE", "/trackings/" + parametersAll, null);


            if (Convert.ToInt32(response["meta"]["code"].ToString()) == 200)
                return true;
            else
                return false;

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

            if (trackingGet.Id != null && !trackingGet.Id.Equals(""))
            {
                parametersExtra = trackingGet.Id;
            }
            else
            {
                //get the require fields if any (postal_code, tracking_account etc..)
                string paramRequiredFields = ReplaceFirst(trackingGet.GetQueryRequiredFields(), "&", "?");
                parametersExtra = trackingGet.Slug + "/" + trackingGet.TrackingNumber +
                    paramRequiredFields;
            }
            JObject response = Request("GET", "/trackings/" + parametersExtra, null);
            JObject trackingJSON = (JObject)response["data"]["tracking"];

            return (trackingJSON.Count != 0) ? new Tracking(trackingJSON) : null;
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

            if (fields != null)
                qs.Add("fields", ParseListFieldTracking(fields));

            if (lang != null && !lang.Equals("")) qs.Add("lang", lang);

            //encode the fields required
            string params_query = ReplaceFirst(qs.ToString(), "&", "?");

            if (trackingGet.Id != null && !trackingGet.Id.Equals(""))
            {
                parametersAll = trackingGet.Id + params_query;
            }
            else
            {
                //get the require fields if any (postal_code, tracking_account etc..)
                string paramRequiredFields = trackingGet.GetQueryRequiredFields();
                parametersAll = trackingGet.Slug +
                "/" + trackingGet.TrackingNumber + params_query + paramRequiredFields;
            }

            JObject response = Request("GET", "/trackings/" + parametersAll, null);
            JObject trackingJSON = (JObject)response["data"]["tracking"];

            return (trackingJSON.Count != 0) ? new Tracking(trackingJSON) : null;
        }

        /// <summary>
        /// Make a request to the HTTP API of Aftership
        /// </summary>
        ///<param name="method">String with the method of the request: GET, POST, PUT, DELETE</param> 
        ///<param name="urlResource">String with the URL of the request</param> 
        ///<param name="body">String JSON with the body of the request, 
        /// if the request doesn't need body "GET/DELETE", the bodywould be null</param> 
        /// <returns>A String JSON with the response of the request</returns>
        /// 
        public JObject Request(string method, string urlResource, string body)
        {
            // Console.WriteLine ("Start Request "+DateTime.Now);
            string url = _url + VERSION_API + urlResource;


            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Timeout = 150000;
            WebHeaderCollection header = new WebHeaderCollection
            {
                { "aftership-api-key", _tokenAftership }
            };
            request.Headers = header;
            request.ContentType = "application/json";
            request.Method = method;
            // Console.WriteLine(method+" Requesting the URL :"+ url);

            if (body != null)
            {
                // Console.WriteLine ("body: " + body);
                //is a POST or PUT  
                using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            string json_response = JsonResponseFromWebResponse(request);

            Console.WriteLine("Response request: " + json_response + "*");
            return JObject.Parse(json_response); ;
        }

        private static string JsonResponseFromWebResponse(HttpWebRequest request)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //                if(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created){
                StreamReader reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
                //                }else if (response.StatusCode == HttpStatusCode.RequestTimeout){
                //                    throw new TimeoutException();
                //                }else{
                //                    throw new WebException(response.StatusCode.ToString());
                //                }
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
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string text = reader.ReadToEnd();
                        throw new WebException(text, e);
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            //            }finally{
            //                Console.WriteLine ("Finish Request "+DateTime.Now);
            //            }
        }


        /// Parse a List<FieldTracking> to List<String>
        public List<string> ParseListFieldTracking(List<FieldTracking> list)
        {
            return list.Select(elemement => elemement.ToString()).ToList();
        }

        /// Replace first ocurrence from a String
        public string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
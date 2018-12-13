using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AftershipAPI
{
    public class Courier
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Other_name { get; set; }
        public string Web_url { get; set; }
        public List<string> RequireFields { get; set; }


        /** Default constructor with all the fields of the class */
        public Courier(string web_url, string slug, string name, string phone, string other_name)
        {
            Web_url = web_url;
            Slug = slug;
            Name = name;
            Phone = phone;
            Other_name = other_name;
        }

        /**
     * Constructor, creates a Courier from a JSONObject with the information of the Courier,
     * if any field is not specified it will be ""
     *
     * @param jsonCourier   A JSONObject with information of the Courier
     * by the API.
     **/           // _trackingNumber = trackingJSON["tracking_number"]==null?null:(String)trackingJSON["tracking_number"];

        
        public Courier(JObject jsonCourier)
        {
            Web_url = jsonCourier["web_url"] == null ? null : (string)jsonCourier["web_url"];
            Slug = jsonCourier["slug"] == null ? null : (string)jsonCourier["slug"];
            Name = jsonCourier["name"] == null ? null : (string)jsonCourier["name"];
            Phone = jsonCourier["phone"] == null ? null : (string)jsonCourier["phone"];
            Other_name = jsonCourier["other_name"] == null ? null : (string)jsonCourier["other_name"];

            JArray requireFieldsArray = jsonCourier["required_fields"] == null ? null : (JArray)jsonCourier["required_fields"];
            if (requireFieldsArray != null && requireFieldsArray.Count != 0)
            {
                RequireFields = new List<string>();
                for (int i = 0; i < requireFieldsArray.Count; i++)
                {
                    RequireFields.Add(requireFieldsArray[i].ToString());
                }
            }
        }

        public string TooString()
        {
            return "Courier{" +
                "slug='" + Slug + '\'' +
                ", name='" + Name + '\'' +
                ", phone='" + Phone + '\'' +
                ", other_name='" + Other_name + '\'' +
                ", web_url='" + Web_url + '\'' +
                '}';
        }

        
        public void AddRequireField(string requierField)
        {
            if (RequireFields == null)
            {
                RequireFields = new List<string>
                {
                    requierField
                };
            }
            else
            {
                RequireFields.Add(requierField);
            }
        }

        public void DeleteRequireField(string requireField)
        {
            if (RequireFields != null)
            {
                RequireFields.Remove(requireField);
            }
        }

        public void DeleteRequireFields()
        {
            RequireFields = null;
        }
    }
}
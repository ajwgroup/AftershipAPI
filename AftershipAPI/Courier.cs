using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace AftershipAPI
{
    public class Courier
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Other_name { get; set; }
        public string Web_url { get; set; }
        public List<string> RequireFields { get; set; } = new List<string>();
        /// <summary>
        /// Default constructor with all the fields of the class
        /// </summary>
        /// <param name="web_url"></param>
        /// <param name="slug"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <param name="other_name"></param>
        public Courier(string web_url, string slug, string name, string phone, string other_name)
        {
            Web_url = web_url;
            Slug = slug;
            Name = name;
            Phone = phone;
            Other_name = other_name;
        }

        /// <summary>
        ///  Constructor, creates a Courier from a JSONObject with the information of the Courier,
        ///  if any field is not specified it will be ""
        /// </summary>
        /// <param name="jsonCourier">A JSONObject with information of the Courier by the API.</param>
        public Courier(JObject jsonCourier)
        {
            Web_url = jsonCourier["web_url"] == null ? null : (string)jsonCourier["web_url"];
            Slug = jsonCourier["slug"] == null ? null : (string)jsonCourier["slug"];
            Name = jsonCourier["name"] == null ? null : (string)jsonCourier["name"];
            Phone = jsonCourier["phone"] == null ? null : (string)jsonCourier["phone"];
            Other_name = jsonCourier["other_name"] == null ? null : (string)jsonCourier["other_name"];

            JArray requireFieldsArray = jsonCourier["required_fields"] == null ? null : (JArray)jsonCourier["required_fields"];

            if (requireFieldsArray != null && requireFieldsArray.Count != 0)
                RequireFields = requireFieldsArray.Select(field => field.ToString()).ToList();
        }

        public override string ToString()
        {
            return $"Courier{{slug='{Slug}', name='{Name}', phone='{Phone}', other_name='{Other_name}', web_url='{Web_url}'}}";
        }

        public void AddRequireField(string requierField) => RequireFields.Add(requierField);

        public void DeleteRequireField(string requireField) => RequireFields.Remove(requireField);

        public void DeleteRequireFields() => RequireFields.Clear();
    }
}
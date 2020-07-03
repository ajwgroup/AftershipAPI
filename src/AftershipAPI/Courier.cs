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
        public List<string> RequireFields { get; } = new List<string>();
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

        public override string ToString()
        {
            return $"Courier{{slug='{Slug}', name='{Name}', phone='{Phone}', other_name='{Other_name}', web_url='{Web_url}'}}";
        }

        public void AddRequireField(string requierField) => RequireFields.Add(requierField);

        public void DeleteRequireField(string requireField) => RequireFields.Remove(requireField);

        public void DeleteRequireFields() => RequireFields.Clear();
    }
}
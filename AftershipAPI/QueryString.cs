using System;
using System.Collections.Generic;

namespace AftershipAPI
{
    /// <summary>
	/// Creates a url friendly string
	/// </summary>
	public class Querystring
    {
        public string Query { get; set; } = "";

        public Querystring() { }

        public Querystring(string name, string value)
        {
            Encode(name, value);
        }

        public void Add(string name, List<string> list)
        {
            if(!string.IsNullOrEmpty(Query))
                Query += "&";

            Encode(name, string.Join(",", list.ToArray()));
        }

        public void Add(string name, string value)
        {
            if (!string.IsNullOrEmpty(Query))
                Query += "&";

            Encode(name, value);
        }

        private void Encode(string name, string value)
        {
            Query += $"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value)}";
        }

        public string GetQuery() => Query;

        public override string ToString() => GetQuery();
    }
}

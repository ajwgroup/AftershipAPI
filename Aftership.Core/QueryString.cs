using System;
using System.Collections.Generic;

namespace AftershipAPI
{
    /// <summary>
	/// Creates a url friendly string
	/// </summary>
	public class Querystring
    {
        private string query = "";

        //careful, this constructor creates the first element with &
        public Querystring() { }

        public Querystring(string name, string value)
        {
            Encode(name, value);
        }

        public void Add(string name, List<string> list)
        {
            query += "&";

            string value = string.Join(",", list.ToArray());
            Encode(name, value);
        }

        public void Add(string name, string value)
        {
            query += "&";
            Encode(name, value);
        }

        private void Encode(string name, string value)
        {
            query += Uri.EscapeDataString(name);
            query += "=";
            query += Uri.EscapeDataString(value);
        }

        public string GetQuery() => query;

        public override string ToString() => GetQuery();
    }
}

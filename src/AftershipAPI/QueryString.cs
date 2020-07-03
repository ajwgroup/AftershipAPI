using System;
using System.Collections.Generic;
using System.Text;

namespace AftershipAPI
{
    /// <summary>
	/// Creates a url friendly string
	/// </summary>
	public class Querystring
    {
        private readonly StringBuilder _query = new StringBuilder();

        public string Query => _query.ToString();

        public Querystring() { }

        public Querystring(string name, string value) => Encode(name, value);

        public void Add(string name, List<string> list)
        {
            Add(name, string.Join(",", list.ToArray()));
        }

        public void Add(string name, string value)
        {
            _query.Append(NewQueryOrAdd(Query.Length));

            Encode(name, value);
        }

        private string NewQueryOrAdd(int queryLength) => queryLength == 0 ? "?" : "&";

        private void Encode(string name, string value) => _query.Append($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value)}");

        public string GetQuery() => Query.ToString();

        public override string ToString() => GetQuery();
    }
}

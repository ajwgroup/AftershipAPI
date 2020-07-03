using AftershipAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AftershipAPI
{
    public class ParametersTracking
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public string Keyword { get; set; }
        public DateTime CreatedAtMin { get; set; }
        public DateTime CreatedAtMax { get; set; }
        public string Lang { get; set; }
        public int Total { get; set; }

        /// <summary>
        ///  Unique courier code Use comma for multiple values. (Example: dhl,ups,usps) 
        /// </summary>
        private readonly List<string> _slugs = new List<string>();

        /// <summary>
        /// Origin country of trackings. Use ISO Alpha-3 (three letters).
        /// (Example: USA,HKG)
        /// </summary>
        private readonly List<ISO3Country> _origins = new List<ISO3Country>();

        /// <summary>
        /// Destination country of trackings. Use ISO Alpha-3 (three letters).
        /// (Example: USA,HKG)
        /// </summary>
        private readonly List<ISO3Country> _destinations = new List<ISO3Country>();

        /// <summary>
        /// Current status of tracking.
        /// </summary>
        private readonly List<StatusTag> _tags = new List<StatusTag>();

        /// <summary>
        /// List of fields to include in the response. Fields to include: title, orderId, tag, checkpoints,
        /// checkpointTime, message, countryName. (Defaults: none, Example: title,orderId)
        /// </summary>
        private readonly List<FieldTracking> _fields = new List<FieldTracking>();

        public ParametersTracking()
        {
            Page = 1;
            Limit = 100;
        }

        public void AddSlug(string slug) => _slugs.Add(slug);

        public void DeleteRequireField(string slug) => _slugs.Remove(slug);

        public void DeleteSlugs() => _slugs.Clear();

        public void AddOrigin(ISO3Country origin) => _origins.Add(origin);

        public void DeleteOrigin(ISO3Country origin) => _origins.Remove(origin);

        public void DeleteOrigins() => _origins.Clear();

        public void AddDestination(ISO3Country destination) => _destinations.Add(destination);

        public void DeleteDestination(ISO3Country destination) => _destinations.Remove(destination);

        public void DeleteDestinations() => _destinations.Clear();

        public void AddTag(StatusTag tag) => _tags.Add(tag);

        public void DeleteTag(StatusTag tag) => _tags.Remove(tag);

        public void DeleteTags() => _tags.Clear();

        public void AddField(FieldTracking field) => _fields.Add(field);

        public void DeleteField(FieldTracking field) => _fields.Remove(field);

        public void DeleteFields() => _fields.Clear();

        /// <summary>
        /// Create a QueryString with all the fields of this class different of Null
        /// </summary>
        /// <returns>The string with the param codified in the QueryString</returns>
        public string GenerateQueryString()
        {
            var qs = new Querystring("page", Page.ToString());
            qs.Add("limit", Limit.ToString());

            if (Keyword != null) qs.Add("keyword", Keyword);
            if (CreatedAtMin != default) qs.Add("created_at_min", DateMethods.ToString(CreatedAtMin));
            if (CreatedAtMax != default) qs.Add("created_at_max", DateMethods.ToString(CreatedAtMax));
            if (Lang != null) qs.Add("lang", Lang);
            if (_slugs.Any()) qs.Add("slug", _slugs);
            if (_origins.Any()) qs.Add("origin", string.Join(",", _origins));
            if (_destinations.Any()) qs.Add("destination", string.Join(",", _destinations));
            if (_tags.Any()) qs.Add("tag", string.Join(",", _tags));
            if (_fields.Any()) qs.Add("fields", string.Join(",", _fields));

            return qs.GetQuery();
        }
    }
}
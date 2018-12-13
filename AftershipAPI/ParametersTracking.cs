using AftershipAPI.Enums;
using System;
using System.Collections.Generic;

namespace AftershipAPI
{
    /**
 * Keep the information for get trackings from the server, and interact with the results
 * Created by User on 13/6/14.
 */
    public class ParametersTracking
    {

        public int Page { get; set; }
        public int Limit { get; set; }
        public string Keyword { get; set; }
        public DateTime CreatedAtMin { get; set; }
        public DateTime CreatedAtMax { get; set; }
        public string Lang { get; set; }
        public int Total { get; set; }


        /** Unique courier code Use comma for multiple values. (Example: dhl,ups,usps) */
        private List<string> _slugs;

        /**  Origin country of trackings. Use ISO Alpha-3 (three letters).
     * (Example: USA,HKG) */
        private List<ISO3Country> _origins;

        /** Destination country of trackings. Use ISO Alpha-3 (three letters).
     * (Example: USA,HKG) */
        private List<ISO3Country> _destinations;

        /** Current status of tracking. */
        private List<StatusTag> _tags;

        /** List of fields to include in the response. Fields to include: title, orderId, tag, checkpoints,
     * checkpointTime, message, countryName. (Defaults: none, Example: title,orderId) */
        private List<FieldTracking> _fields;

        public ParametersTracking()
        {
            Page = 1;
            Limit = 100;
        }

        public void AddSlug(string slug)
        {
            if (_slugs == null)
            {
                _slugs = new List<string>
                {
                    slug
                };
            }
            else
            {
                _slugs.Add(slug);
            }
        }

        public void DeleteRequireField(string slug)
        {
            if (_slugs != null)
            {
                _slugs.Remove(slug);
            }
        }

        public void DeleteSlugs()
        {
            _slugs = null;
        }

        public void AddOrigin(ISO3Country origin)
        {
            if (_origins == null)
            {
                _origins = new List<ISO3Country>
                {
                    origin
                };
            }
            else
            {
                _origins.Add(origin);
            }
        }

        public void DeleteOrigin(ISO3Country origin)
        {
            if (_origins != null)
            {
                _origins.Remove(origin);
            }
        }

        public void DeleteOrigins()
        {
            _origins = null;
        }

        public void AddDestination(ISO3Country destination)
        {
            if (_destinations == null)
            {
                _destinations = new List<ISO3Country>
                {
                    destination
                };
            }
            else
            {
                _destinations.Add(destination);
            }
        }

        public void DeleteDestination(ISO3Country destination)
        {
            if (_destinations != null)
            {
                _destinations.Remove(destination);
            }
        }

        public void DeleteDestinations()
        {
            _destinations = null;
        }

        public void AddTag(StatusTag tag)
        {
            if (_tags == null)
            {
                _tags = new List<StatusTag>
                {
                    tag
                };
            }
            else
            {
                _tags.Add(tag);
            }
        }

        public void DeletTag(StatusTag tag)
        {
            if (_tags != null)
            {
                _tags.Remove(tag);
            }
        }

        public void DeleteTags()
        {
            _tags = null;
        }

        public void AddField(FieldTracking field)
        {
            if (_fields == null)
            {
                _fields = new List<FieldTracking>
                {
                    field
                };
            }
            else
            {
                _fields.Add(field);
            }
        }

        public void DeletField(FieldTracking field)
        {
            if (_fields != null)
            {
                _fields.Remove(field);
            }
        }

        public void DeleteFields()
        {
            _fields = null;
        }

        /// <summary>
        /// Create a QueryString with all the fields of this class different of Null
        /// </summary>
        /// <returns>The string with the param codified in the QueryString</returns>
        public string GenerateQueryString()
        {

            var qs = new Querystring("page", Page.ToString());
            qs.Add("limit", Limit.ToString());

            if (Keyword != null) qs.Add("keyword", Keyword);
            if (CreatedAtMin != default(DateTime)) qs.Add("created_at_min", DateMethods.ToString(CreatedAtMin));
            if (CreatedAtMax != default(DateTime)) qs.Add("created_at_max", DateMethods.ToString(CreatedAtMax));
            if (Lang != null) qs.Add("lang", Lang);

            if (_slugs != null) qs.Add("slug", _slugs);

            if (_origins != null) qs.Add("origin", string.Join(",", _origins));

            if (_destinations != null) qs.Add("destination", string.Join(",", _destinations));

            if (_tags != null) qs.Add("tag", string.Join(",", _tags));

            if (_fields != null) qs.Add("fields", string.Join(",", _fields));

            return qs.GetQuery();
        }
    }
}
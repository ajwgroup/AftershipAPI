using AftershipAPI.Enums;
using Newtonsoft.Json.Linq;
using System;

namespace AftershipAPI
{
    public class Checkpoint
    {
        /// Date and time of the tracking created. 
        public DateTime CreatedAt { get; set; }

        /// Date and time of the checkpoint, provided by courier. Value may be:
        ///Empty String,
        ///YYYY-MM-DD,
        ///YYYY-MM-DDTHH:MM:SS, or
        ///YYYY-MM-DDTHH:MM:SS+TIMEZONE 
        public string CheckpointTime { get; set; }

        /// Location info (if any) 
        public string City { get; set; }

        /// Country ISO Alpha-3 (three letters) of the checkpoint 
        public ISO3Country CountryISO3 { get; set; }

        /// Country name of the checkpoint, may also contain other location info. 
        public string CountryName { get; set; }

        /// Checkpoint message 
        public string Message { get; set; }

        /// Location info (if any) 
        public string State { get; set; }

        /// Status of the checkpoint 
        public string Tag { get; set; }

        /// Location info (if any) 
        public string Zip { get; set; }

        public Checkpoint(JObject checkpointJSON)
        {
            // Console.WriteLibe(typeof(checkpointJSON["created_at"]));
            CreatedAt = checkpointJSON["created_at"] == null ? DateTime.MinValue : (DateTime)checkpointJSON["created_at"];
            CheckpointTime = checkpointJSON["checkpoint_time"] == null ? null : (string)checkpointJSON["checkpoint_time"];
            City = checkpointJSON["city"] == null ? null : (string)checkpointJSON["city"];
            CountryISO3 = checkpointJSON["country_iso3"] == null ? 0 : (ISO3Country)Enum.Parse(typeof(ISO3Country), (string)checkpointJSON["country_iso3"]);
            CountryName = checkpointJSON["country_name"] == null ? null : (string)checkpointJSON["country_name"];
            Message = checkpointJSON["message"] == null ? null : (string)checkpointJSON["message"];
            State = checkpointJSON["state"] == null ? null : (string)checkpointJSON["state"];
            Tag = checkpointJSON["tag"] == null ? null : (string)checkpointJSON["tag"];
            Zip = checkpointJSON["zip"] == null ? null : (string)checkpointJSON["zip"];
        }
    }
}
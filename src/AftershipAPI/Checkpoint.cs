using AftershipAPI.Enums;
using Newtonsoft.Json;
using System;

namespace AftershipAPI
{
    public class Checkpoint
    {
        /// Date and time of the tracking created. 
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// Date and time of the checkpoint, provided by courier. Value may be:
        ///Empty String,
        ///YYYY-MM-DD,
        ///YYYY-MM-DDTHH:MM:SS, or
        ///YYYY-MM-DDTHH:MM:SS+TIMEZONE 
        [JsonProperty("checkpoint_time")]
        public string CheckpointTime { get; set; }

        /// Location info (if any) 
        [JsonProperty("city")]
        public string City { get; set; }

        /// Country ISO Alpha-3 (three letters) of the checkpoint 
        [JsonProperty("country_iso3")]
        public ISO3Country CountryISO3 { get; set; }

        /// Country name of the checkpoint, may also contain other location info. 
        [JsonProperty("country_name")]
        public string CountryName { get; set; }

        /// Checkpoint message 
        [JsonProperty("message")]
        public string Message { get; set; }

        /// Location info (if any) 
        [JsonProperty("state")]
        public string State { get; set; }

        /// Status of the checkpoint 
        [JsonProperty("tag")]
        public string Tag { get; set; }

        /// Location info (if any) 
        [JsonProperty("zip")]
        public string Zip { get; set; }
    }
}
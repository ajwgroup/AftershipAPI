using Newtonsoft.Json;
using System.Collections.Generic;

namespace AftershipAPI
{
    public class Data
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("trackings")]
        public List<Tracking> Trackings { get; set; }
        [JsonProperty("tracking")]
        public Tracking Tracking { get; set; }
        [JsonProperty("checkpoint")]
        public Checkpoint Checkpoint { get; set; }
        [JsonProperty("couriers")]
        public List<Courier> Couriers { get; set; }
    }
}
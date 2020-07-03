using System.Collections.Generic;

namespace AftershipAPI
{
    public class Data
    {
        public int Count { get; set; }
        public List<Tracking> Trackings { get; set; }
        public Tracking Tracking { get; set; }
        public Checkpoint Checkpoint { get; set; }
        public List<Courier> Couriers { get; set; }
    }
}
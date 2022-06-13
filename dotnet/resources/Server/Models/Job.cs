using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Player { get; set; }
        public int Warehouse { get; set; }
        public int Debriscleaner { get; set; }
        public int Lawnmowing { get; set; }
        public int Forklifts { get; set; }
        public int Diver { get; set; }
        public int Gardener { get; set; }
        public int Towtruck { get; set; }
        public int Refinery { get; set; }
        public int Fisherman { get; set; }
        public int Hunter { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class Org
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Owner { get; set; }
        public string Members { get; set; }
        public string Requests { get; set; }
        public string Vehicles { get; set; }
        public string VehicleRequests { get; set; }
    }
}

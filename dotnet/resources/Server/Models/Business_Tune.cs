using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class Business_Tune
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string PaidTo { get; set; }
        public string Wheels { get; set; }
        public string WheelOrders { get; set; }
    }
}

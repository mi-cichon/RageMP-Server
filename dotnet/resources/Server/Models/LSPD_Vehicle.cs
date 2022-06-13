using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class LSPD_Vehicle
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public int Power { get; set; }
        public string Wheels { get; set; }
    }
}

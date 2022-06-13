using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string Color1 { get; set; }
        public string Color2 { get; set; }
        public string Spawned { get; set; }
        public string Lastpos { get; set; }
        public string Lastrot { get; set; }
        public string Damage { get; set; }
        public string Used { get; set; }
        public string Tune { get; set; }
        public string Petrol { get; set; }
        public string Speedometer { get; set; }
        public int Dirt { get; set; }
        public string Washtime { get; set; }
        public string Trunk { get; set; }
        public string Mechtune { get; set; }
        public string Wheels { get; set; }
        public string Drivers { get; set; }
        public string Parkingbrake { get; set; }
        public float Trip { get; set; }
    }
}

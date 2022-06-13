using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class Penalty
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Ban { get; set; }
        public string Mute { get; set; }
        public string DrivingLicence { get; set; }
    }
}

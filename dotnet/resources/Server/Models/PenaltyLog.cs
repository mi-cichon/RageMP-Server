using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class PenaltyLog
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Admin { get; set; }
        public string Type { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public string Reason { get; set; }
    }
}

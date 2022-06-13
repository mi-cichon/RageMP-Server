using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Target { get; set; }
        public string Amount { get; set; }
        public string Title { get; set; }
        public string Time { get; set; }
    }
}

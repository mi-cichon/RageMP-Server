using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class House
    {
        public int Id { get; set; }
        public string Pos { get; set; }
        public string Interior { get; set; }
        public string Price { get; set; }
        public string Owner { get; set; }
        public string Time { get; set; }
        public string Storage { get; set; }
    }
}

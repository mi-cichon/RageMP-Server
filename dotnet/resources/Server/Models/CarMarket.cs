using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class CarMarket
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
    }
}

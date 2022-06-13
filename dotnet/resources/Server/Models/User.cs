using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Type { get; set; }
        public string Username { get; set; }
        public string Character { get; set; }
        public string Lastpos { get; set; }
        public string Money { get; set; }
        public string Bank { get; set; }
        public int Exp { get; set; }
        public string Registered { get; set; }
        public int Playtime { get; set; }
        public string Equipment { get; set; }
        public string Collectibles { get; set; }
        public int Skillpoints { get; set; }
        public string Skills { get; set; }
        public string Clothes { get; set; }
        public string Settings { get; set; }
        public string Authcode { get; set; }
        public string Vehsold { get; set; }
        public string Accnumber { get; set; }
    }
}

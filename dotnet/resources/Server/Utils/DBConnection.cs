using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using MySqlConnector;

namespace ServerSide
{
    public class DBConnection
    {
        public MySqlConnection connection;
        public MySqlCommand command;
        public DBConnection()
        {
            var cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText("logs/config.json"));
            connection = new MySqlConnection($"SERVER={cfg.Server}; DATABASE={cfg.Database}; UID={cfg.User}; PASSWORD={cfg.Password}");
            connection.Open();
            command = connection.CreateCommand();
        }




        public class Config
        {
            public string Server { get; set; }
            public string Database { get; set; }
            public string User { get; set; }
            public string Password { get; set; }

            public Config(string server, string database, string user, string password)
            {
                this.Server = server;
                this.Database = database;
                this.User = user;
                this.Password = password;
            }
        }
    }
}
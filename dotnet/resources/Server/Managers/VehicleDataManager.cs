using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GTANetworkAPI;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using System.Text.Json;
//using System.Text.Json.Serialization;
namespace ServerSide
{
    public class VehicleDataManager
    {
        public string defaultDamage = "{\"body\":1000,\"engine\":1000,\"fldoor\":1,\"frdoor\":1,\"hood\":1,\"trunk\":1,\"bldoor\":1,\"brdoor\":1,\"blwindow\":1,\"brwindow\":1,\"flwindow\":1,\"frwindow\":1,\"fwindow\":1,\"bwindow\":1,\"flwheel\":1,\"frwheel\":1,\"blwheel\":1,\"brwheel\":1}";
        public string wreckedDamage = "{\"body\":50,\"engine\":50,\"fldoor\":0,\"frdoor\":0,\"hood\":0,\"trunk\":0,\"bldoor\":0,\"brdoor\":0,\"blwindow\":0,\"brwindow\":0,\"flwindow\":0,\"frwindow\":0,\"fwindow\":0,\"bwindow\":0,\"flwheel\":0,\"frwheel\":0,\"blwheel\":0,\"brwheel\":0}";
        public string defaultTune = "{}";
        public CustomVehicles customVehicles = new CustomVehicles();
        public Wheels wheels = new Wheels();
        HandlingManager handlingManager = new HandlingManager();
        public VehicleDataManager()
        {

        }
        public Vehicle CreatePersonalVehicle(int id, Vector3 position, float rotation, bool spawned)
        {
            DBConnection dataBase = new DBConnection();
            Vehicle vehicle = null;
            dataBase.command.CommandText = $"SELECT * FROM vehicles WHERE id = {id} AND spawned = '{spawned.ToString()}'";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    vehicle = NAPI.Vehicle.CreateVehicle(Convert.ToUInt32(reader.GetString(2)), position, rotation, 0, 0, numberPlate: "B " + reader.GetInt32(0).ToString());
                    vehicle.SetSharedData("invincible", false);
                    vehicle.SetSharedData("type", "personal");
                    vehicle.SetSharedData("id", reader.GetInt32(0));
                    vehicle.SetSharedData("owner", ulong.Parse(reader.GetString(1)));
                    vehicle.SetSharedData("model", reader.GetString(2));
                    vehicle.SetSharedData("name", reader.GetString(3));
                    vehicle.SetSharedData("color1", reader.GetString(4));
                    vehicle.SetSharedData("color1mod", JsonToColorMod(reader.GetString(4)));
                    vehicle.SetSharedData("color2", reader.GetString(5));
                    vehicle.SetSharedData("color2mod", JsonToColorMod(reader.GetString(5)));
                    vehicle.SetSharedData("spawned", Convert.ToBoolean(reader.GetString(6)));
                    vehicle.SetSharedData("lastpos", JsonToVector(reader.GetString(7)));
                    vehicle.SetSharedData("lastrot", JsonToVector(reader.GetString(8)));
                    vehicle.SetSharedData("damage", reader.GetString(9));
                    vehicle.SetSharedData("used", reader.GetString(10));
                    vehicle.SetSharedData("tune", reader.GetString(11));
                    vehicle.SetSharedData("petrol", float.Parse(reader.GetString(12)));
                    vehicle.SetSharedData("speedometer", reader.GetString(13));
                    vehicle.SetSharedData("towed", false);
                    vehicle.SetSharedData("locked", false);
                    int[] PandS = GetVehicleStockPowerAndSpeed(vehicle);
                    vehicle.SetSharedData("power", PandS[0]);
                    vehicle.SetSharedData("speed", PandS[1]);
                    vehicle.SetSharedData("market", false);
                    vehicle.SetSharedData("dirt", reader.GetInt32(14));
                    vehicle.SetSharedData("washtime", reader.GetString(15));
                    vehicle.SetSharedData("trunk", reader.GetString(16));
                    vehicle.SetSharedData("mechtune", reader.GetString(17));
                    vehicle.SetSharedData("wheels", reader.GetString(18));
                    vehicle.SetSharedData("drivers", reader.GetString(19));
                    bool brake = bool.Parse(reader.GetString(20));
                    vehicle.SetSharedData("veh_trip", reader.GetFloat(21));
                    int[] color1 = JsonToColor(reader.GetString(4));
                    int[] color2 = JsonToColor(reader.GetString(5));
                    NAPI.Vehicle.SetVehicleCustomPrimaryColor(vehicle.Handle, color1[0], color1[1], color1[2]);
                    NAPI.Vehicle.SetVehicleCustomSecondaryColor(vehicle.Handle, color2[0], color2[1], color2[2]);
                    setVehiclesPetrolAndTrunk(vehicle);
                    vehicle.SetSharedData("veh_engine", false);
                    vehicle.SetSharedData("veh_lights", false);
                    vehicle.SetSharedData("veh_locked", false);

                    NAPI.Task.Run(() =>
                    {
                        if (vehicle != null && vehicle.Exists)
                        {
                            SetVehiclesWheels(vehicle);
                            applyTuneToVehicle(vehicle, vehicle.GetSharedData<string>("tune"), vehicle.GetSharedData<string>("mechtune"));
                            SetVehiclesExtra(vehicle);
                            vehicle.SetSharedData("veh_brake", brake);
                        }
                        
                    }, 1000);

                    

                }
            }
            if(vehicle != null)
            {
                dataBase.command.CommandText = $"UPDATE vehicles SET spawned = '{bool.TrueString}' WHERE id = {id}";
                if (dataBase.command.CommandText != "")
                {
                    dataBase.command.ExecuteNonQuery();
                    dataBase.connection.Close();
                    return vehicle;
                }
            }
            dataBase.connection.Close();
            return null;
        }

        public void CreatePersonalVehicleFromDealer(Player player, Vehicle vehicle)
        {
            vehicle.SetSharedData("type", "personal");
            vehicle.SetSharedData("owner", player.SocialClubId);
            vehicle.SetSharedData("model", vehicle.Model.ToString());
            vehicle.SetSharedData("color1", ColorToJson(NAPI.Vehicle.GetVehicleCustomPrimaryColor(vehicle), 0));
            vehicle.SetSharedData("color1mod", 0);
            vehicle.SetSharedData("color2", ColorToJson(NAPI.Vehicle.GetVehicleCustomSecondaryColor(vehicle), 0));
            vehicle.SetSharedData("color2mod", 0);
            vehicle.SetSharedData("spawned", true);
            vehicle.SetSharedData("lastpos", vehicle.Position);
            vehicle.SetSharedData("lastrot", vehicle.Rotation);
            vehicle.SetSharedData("invincible", false);
            vehicle.SetSharedData("used", DateTime.Now.ToString());
            int[] PandS = GetVehicleStockPowerAndSpeed(vehicle);
            vehicle.SetSharedData("power", PandS[0]);
            vehicle.SetSharedData("speed", PandS[1]);
            vehicle.SetSharedData("tune", "{}");
            vehicle.SetSharedData("market", false);
            vehicle.SetSharedData("damage", defaultDamage);
            vehicle.SetSharedData("dirt", 0);
            vehicle.SetSharedData("washtime", DateTime.Now.ToString());
            vehicle.SetSharedData("speedometer", "#0c9");
            vehicle.SetSharedData("trunk", "[]");
            vehicle.SetSharedData("mechtune", "[0,0,0,0,0]");
            vehicle.SetSharedData("wheels", "[0, -1, 0]");
            vehicle.SetSharedData("offroad", handlingManager.IsCarOffroadType(vehicle));
            vehicle.SetSharedData("petrol", 20.0f);
            vehicle.SetSharedData("drivers", "[]");
            vehicle.SetSharedData("veh_brake", false);
            vehicle.SetSharedData("veh_engine", false);
            vehicle.SetSharedData("veh_lights", false);
            vehicle.SetSharedData("veh_locked", false);

            InsertPersonalVehicleToDB(vehicle);
            SetVehiclesWheels(vehicle);
            SetVehiclesExtra(vehicle);
        }

        public void InsertPersonalVehicleToDB(Vehicle vehicle)
        {
            DBConnection dataBase = new DBConnection();
            Vector3 position = vehicle.GetSharedData<Vector3>("lastpos");
            string owner = vehicle.GetSharedData<Int64>("owner").ToString();
            string model = vehicle.GetSharedData<string>("model");
            string name = vehicle.GetSharedData<string>("name");
            string color1 = ColorToJson(NAPI.Vehicle.GetVehicleCustomPrimaryColor(vehicle), 0);
            string color2 = ColorToJson(NAPI.Vehicle.GetVehicleCustomSecondaryColor(vehicle), 0);
            string spawned = vehicle.GetSharedData<bool>("spawned") is true ? bool.TrueString : bool.FalseString;
            string lastpos = VectorToJson(vehicle.Position);
            string lastrot = VectorToJson(vehicle.Rotation);
            string trunk = vehicle.GetSharedData<string>("trunk");
            string petrol = vehicle.GetSharedData<float>("petrol").ToString();
            string speedometer = vehicle.GetSharedData<string>("speedometer");
            dataBase.command.CommandText = $"INSERT INTO vehicles (owner, model, name, color1, color2, spawned, lastpos, lastrot, damage, used, tune, petrol, speedometer, dirt, washtime, trunk, mechtune, wheels, drivers, trip) VALUES ('{owner}', '{model}', '{name}', '{color1}', '{color2}', '{spawned}', '{lastpos}', '{lastrot}', '{defaultDamage}', '{DateTime.Now.ToString()}', '{defaultTune}', '{petrol}', '{speedometer}', '{0}', '{DateTime.Now.ToString()}', '{trunk}', '[0,0,0,0,0]', '[0, -1, 0]', '[]', {vehicle.GetSharedData<float>("veh_trip")});";
            dataBase.command.ExecuteNonQuery();
            dataBase.command.CommandText = $"SELECT * FROM vehicles WHERE id = (SELECT LAST_INSERT_ID())";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    vehicle.SetSharedData("id", reader.GetInt32(0));
                    vehicle.NumberPlate = "B " + reader.GetInt32(0).ToString();
                }
            }
            dataBase.connection.Close();
        }
        public void SavePersonalVehicleDataToDB(Vehicle vehicle, string dataName)
        {
            if (vehicle != null)
            {
                DBConnection dataBase = new DBConnection();
                switch (dataName)
                {
                    case "owner":
                        dataBase.command.CommandText = $"UPDATE vehicles SET owner = '{vehicle.GetSharedData<Int64>("owner").ToString()}' WHERE id = '{vehicle.GetSharedData<Int32>("id")}'";
                        break;
                    case "model":
                        dataBase.command.CommandText = $"UPDATE vehicles SET model = '{vehicle.GetSharedData<string>("model")}' WHERE id = '{vehicle.GetSharedData<Int32>("id")}'";
                        break;
                    case "spawned":
                        string spawned = vehicle.GetSharedData<bool>("spawned") is true ? bool.TrueString : bool.FalseString;
                        dataBase.command.CommandText = $"UPDATE vehicles SET spawned = '{spawned}' WHERE id = '{vehicle.GetSharedData<Int32>("id")}'";
                        break;
                    case "veh_brake":
                        string brake = vehicle.GetSharedData<bool>("veh_brake") is true ? bool.TrueString : bool.FalseString;
                        dataBase.command.CommandText = $"UPDATE vehicles SET parkingbrake = '{brake}' WHERE id = '{vehicle.GetSharedData<Int32>("id")}'";
                        break;
                    case "petrol":
                        dataBase.command.CommandText = $"UPDATE vehicles SET petrol = '{vehicle.GetSharedData<float>(dataName).ToString()}' WHERE id = '{vehicle.GetSharedData<Int32>("id")}'";
                        break;
                    case "lastpos":
                        dataBase.command.CommandText = $"UPDATE vehicles SET lastpos = '{VectorToJson(vehicle.GetSharedData<Vector3>("lastpos"))}', lastrot = '{VectorToJson(vehicle.GetSharedData<Vector3>("lastrot"))}' WHERE id = '{vehicle.GetSharedData<Int32>("id")}'";
                        break;
                    case "market":
                        string market = vehicle.GetSharedData<bool>("market") ? "Market" : bool.TrueString;
                        dataBase.command.CommandText = $"UPDATE vehicles SET spawned = '{market}' WHERE id = '{vehicle.GetSharedData<Int32>("id")}'";
                        break;
                    case "dirt":
                        dataBase.command.CommandText = $"UPDATE vehicles SET dirt = '{vehicle.GetSharedData<Int32>("dirt")}' WHERE id = '{vehicle.GetSharedData<Int32>("id")}'";
                        break;
                    case "veh_trip":
                        dataBase.command.CommandText = $"UPDATE vehicles SET trip = '{vehicle.GetSharedData<float>("veh_trip").ToString().Replace(",", ".")}' WHERE id = '{vehicle.GetSharedData<Int32>("id")}'";
                        break;
                    default:
                        if (vehicle.HasSharedData(dataName) && vehicle.HasSharedData("id"))
                            dataBase.command.CommandText = $"UPDATE vehicles SET {dataName} = '{vehicle.GetSharedData<string>(dataName)}' WHERE id = '{vehicle.GetSharedData<Int32>("id")}'";
                        break;
                }
                dataBase.command.ExecuteNonQuery();
                dataBase.connection.Close();
            }

        }
        public void UpdateVehiclesTrunk(Vehicle vehicle, string trunk)
        {
            vehicle.SetSharedData("trunk", trunk);
            SavePersonalVehicleDataToDB(vehicle, "trunk");
        }

        public void UpdateVehiclesBrake(Vehicle vehicle, bool brake)
        {
            vehicle.SetSharedData("veh_brake", brake);
            if (vehicle.HasSharedData("owner"))
            {
                SavePersonalVehicleDataToDB(vehicle, "veh_brake");
            }
        }

        public void UpdateVehiclesSpeedometer(Vehicle vehicle, string speedometer)
        {
            vehicle.SetSharedData("speedometer", speedometer);
            SavePersonalVehicleDataToDB(vehicle, "speedometer");
        }
        public void UpdateVehiclesWashTime(Vehicle vehicle, string time)
        {
            vehicle.SetSharedData("washtime", time);
            SavePersonalVehicleDataToDB(vehicle, "washtime");
        }
        public void UpdateVehiclesWheels(Vehicle vehicle, string wheels)
        {
            vehicle.SetSharedData("wheels", wheels);
            SavePersonalVehicleDataToDB(vehicle, "wheels");
            vehicle.SetSharedData("offroad", handlingManager.IsCarOffroadType(vehicle));
        }
        public void UpdateVehiclesDirtLevel(Vehicle vehicle, float dirt)
        {
            if(vehicle != null && vehicle.Exists)
            {
                vehicle.SetSharedData("dirt", Convert.ToInt32(dirt));
                SavePersonalVehicleDataToDB(vehicle, "dirt");
            }
        }
        public void UpdateVehiclesType(Vehicle vehicle, string type)
        {
            vehicle.SetSharedData("type", type);
        }
        public void UpdateVehiclesOwner(Vehicle vehicle, ulong owner)
        {
            vehicle.SetSharedData("owner", owner);
            SavePersonalVehicleDataToDB(vehicle, "owner");
        }
        public void UpdateVehiclesDrivers(Vehicle vehicle, string drivers)
        {
            vehicle.SetSharedData("drivers", drivers);
            SavePersonalVehicleDataToDB(vehicle, "drivers");
        }
        public void SaveVehiclesDriver(Vehicle vehicle, Player player)
        {
            if(player.Exists && player.HasSharedData("username") && vehicle.HasSharedData("drivers"))
            {
                List<string> drivers = JsonConvert.DeserializeObject<List<string>>(vehicle.GetSharedData<string>("drivers"));
                if(drivers.Count > 0 && drivers[0] == player.GetSharedData<string>("username"))
                {
                    return;
                }
                drivers.Insert(0, player.GetSharedData<string>("username"));
                if(drivers.Count > 5)
                {
                    drivers = drivers.GetRange(0, 5);
                }
                UpdateVehiclesDrivers(vehicle, JsonConvert.SerializeObject(drivers));
            }
        }
        public string GetVehiclesLastDrivers(Vehicle vehicle)
        {
            string drivers = "";
            if(vehicle.HasSharedData("drivers"))
            {
                List<string> d = JsonConvert.DeserializeObject<List<string>>(vehicle.GetSharedData<string>("drivers"));
                if(d.Count > 0)
                {
                    for(int i = 0; i < d.Count; i++)
                    {
                        if (i < d.Count - 1)
                            drivers += d[i] + ", ";
                        else
                            drivers += d[i];
                    }
                    return drivers;
                }
            }
            return "";
        }
        public void UpdateVehiclesColor1(Vehicle vehicle, int r, int g, int b, int mod)
        {
            vehicle.SetSharedData("color1", ColorToJson(new Color(r, g, b), mod));
            vehicle.SetSharedData("color1mod", mod);
            SavePersonalVehicleDataToDB(vehicle, "color1");
            try
            {
                NAPI.Vehicle.SetVehicleCustomPrimaryColor(vehicle.Handle, r, g, b);
            }
            catch (Exception) { }
        }

        public void UpdateVehiclesColor2(Vehicle vehicle, int r, int g, int b, int mod)
        {
            vehicle.SetSharedData("color2", ColorToJson(new Color(r, g, b), mod));
            vehicle.SetSharedData("color2mod", mod);
            SavePersonalVehicleDataToDB(vehicle, "color2");
            try
            {
                NAPI.Vehicle.SetVehicleCustomSecondaryColor(vehicle.Handle, r, g, b);
            }
            catch (Exception) { }
        }
        public void UpdateVehicleSpawned(Vehicle vehicle, bool spawned)
        {
            if (vehicle != null)
            {
                vehicle.SetSharedData("spawned", spawned);
                SavePersonalVehicleDataToDB(vehicle, "spawned");
            }

        }
        public void SetVehicleAsMarket(Vehicle vehicle, bool market)
        {
            if (vehicle != null)
            {
                vehicle.SetSharedData("market", market);
                SavePersonalVehicleDataToDB(vehicle, "market");
            }

        }
        public void UpdateVehiclesLastPos(Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                vehicle.SetSharedData("lastpos", vehicle.Position);
                if (vehicle.HasSharedData("owner"))
                {
                    vehicle.SetSharedData("lastrot", vehicle.Rotation);
                    SavePersonalVehicleDataToDB(vehicle, "lastpos");
                }
                
            }
        }
        public void UpdateVehiclesDamage(Vehicle vehicle, string damage)
        {
            if (vehicle != null)
            {
                vehicle.SetSharedData("damage", damage);
                if(vehicle.HasSharedData("owner"))
                    SavePersonalVehicleDataToDB(vehicle, "damage");
            }
        }

        public void UpdateVehiclesUsedTime(Vehicle vehicle)
        {
            string date = DateTime.Now.ToString();
            vehicle.SetSharedData("used", date);
            SavePersonalVehicleDataToDB(vehicle, "used");
        }

        public void UpdateVehiclesTune(Vehicle vehicle, string tune)
        {
            vehicle.SetSharedData("tune", tune);
            SavePersonalVehicleDataToDB(vehicle, "tune");
        }
        public void UpdateVehiclesMechTune(Vehicle vehicle, string tune)
        {
            vehicle.SetSharedData("mechtune", tune);
            SavePersonalVehicleDataToDB(vehicle, "mechtune");
        }
        public void UpdateVehiclesPetrol(Vehicle vehicle, float petrol)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("petrol"))
            {
                petrol = MathF.Round(petrol, 2);
                vehicle.SetSharedData("petrol", petrol);
                if(vehicle.HasSharedData("owner"))
                    SavePersonalVehicleDataToDB(vehicle, "petrol");
            }

        }
        public void UpdateVehiclesTrip(Vehicle vehicle, float dist)
        {
            
            if(vehicle != null && vehicle.Exists && vehicle.HasSharedData("veh_trip"))
            {
                vehicle.SetSharedData("veh_trip", vehicle.GetSharedData<float>("veh_trip") + dist);
                if (vehicle.HasSharedData("owner"))
                {
                    SavePersonalVehicleDataToDB(vehicle, "veh_trip");
                }
            }
        }
        public bool SwitchVehiclesParkingBrake(Vehicle vehicle)
        {
            if (vehicle != null)
            {
                vehicle.SetSharedData("veh_brake", !vehicle.GetSharedData<bool>("veh_brake"));
                return vehicle.GetSharedData<bool>("veh_brake");
            }
            else
                return false;
        }
        public string GetPlayersVehicles(Player player, bool spawned, List<int> orgIds)
        {
            List<string[]> vehicles = new List<string[]>();
            string vehString = "";
            DBConnection dataBase = new DBConnection();

            dataBase.command.CommandText = $"SELECT * FROM vehicles WHERE owner = '{player.SocialClubId.ToString()}'" + (spawned ? " AND spawned = 'False';" : ";");
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    vehicles.Add(new string[]
                    {
                        reader.GetInt32(0).ToString(), reader.GetString(3),reader.GetString(2), "0"
                    });
                    
                }
                reader.Close();
            }

            if(player.HasSharedData("orgId") && player.GetSharedData<Int32>("orgId") != 0 && orgIds != null && orgIds.Count > 0)
            {
                string m = "";
                foreach (int vehicle in orgIds)
                {
                    m += $" vehicles.id = {vehicle}";
                    if (orgIds.IndexOf(vehicle) != orgIds.Count - 1)
                    {
                        m += " OR ";
                    }
                }
                dataBase.command.CommandText = $"SELECT vehicles.id, vehicles.name, vehicles.model FROM vehicles LEFT JOIN users ON vehicles.owner = users.login WHERE vehicles.spawned = 'False' AND vehicles.owner NOT LIKE '{player.SocialClubId}' AND(" + m + ");";
                using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vehicles.Add(new string[]
                        {
                            reader.GetInt32(0).ToString(),
                            reader.GetString(1),
                            reader.GetString(2),
                            "1"
                        });
                    }
                }
            }
            if (vehicles.Count > 0)
            {
                vehString = JsonConvert.SerializeObject(vehicles);
            }
            dataBase.connection.Close();
            return vehString;
        }


        public void GiveSpecialVehicleToPlayer(Player player, string vehType)
        {
            switch (vehType)
            {
                case "bonus":
                    DBConnection dataBase = new DBConnection();
                    dataBase.command.CommandText = $"INSERT INTO vehicles (owner, model, name, color1, color2, spawned, lastpos, lastrot, damage, used, tune, petrol, speedometer, dirt, washtime, trunk, mechtune, wheels, drivers, trip) VALUES ('{player.SocialClubId}', '3025077634', 'Bonusowy Blazer', '[0,0,0,0]', '[0,0,0,0]', 'False', '[119.26427,-1069.8783,28.48527]', '[-0.006499935,0.11400143,3.0413654]', '{defaultDamage}', '{DateTime.Now.ToString()}', '{defaultTune}', '10', '{"licznik":0}', '{0}', '{DateTime.Now.ToString()}', '[]', '[0,0,0,0,0]', '[0, -1, 0]', '[]', 0);";
                    dataBase.command.ExecuteNonQuery();
                    dataBase.connection.Close();
                    break;
            }
        }

        public bool IsVehicleDamaged(Vehicle vehicle)
        {
            string damageString = vehicle.GetSharedData<string>("damage");
            Dictionary<string, float> damageDict = JsonToDamage(damageString);
            foreach (KeyValuePair<string, float> damagePair in damageDict)
            {
                string part = damagePair.Key;
                float value = damagePair.Value;

                if (part == "engine" && value <= 900.0f)
                {
                    return true;
                }
                if (part == "body" && value <= 900.0f)
                {
                    return true;
                }
                if ((part.Contains("door") && value == 0.0f) || (part == "hood" & value == 0.0f) || (part == "trunk" & value == 0.0f))
                {
                    return true;
                }
            }
            return false;
        }

        public void LoadPersonalVehiclesFromDB(ref OrgManager orgManager)
        {
            DBConnection dbc = new DBConnection();
            dbc.command.CommandText = $"SELECT * FROM vehicles WHERE spawned = '{bool.TrueString}'";
            using (MySqlDataReader reader = dbc.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Vector3 position = JsonToVector(reader.GetString(7));
                    Vector3 rotation = JsonToVector(reader.GetString(8));
                    Vehicle veh = CreatePersonalVehicle(reader.GetInt32(0), position, 0.0f, true);
                    veh.Rotation = rotation;
                    orgManager.SetVehiclesOrg(veh);
                }
                reader.Close();
            }
            dbc.connection.Close();
        }

        public int GetVehicleModelPrice(Vehicle vehicle)
        {
            int price = 200000;
            foreach(CustomVehicle cV in customVehicles.AllVehicles)
            {
                if(vehicle != null && vehicle.Exists && cV.model == vehicle.Model)
                {
                    price = cV.price;
                    break;
                }
            }
            return price;
        }
        public Vehicle GetVehicleById(string vehicleId)
        {
            Vehicle vehicle = null;
            foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
            {
                if (veh.HasSharedData("id"))
                {
                    int id = -10000;
                    Int32.TryParse(vehicleId, out id);
                    if (id != -10000)
                    {
                        if(veh.GetSharedData<Int32>("id") == id)
                        {
                            vehicle = veh;
                            break;
                        }
                            
                    }
                }
            }
            return vehicle;
        }

        public Vehicle GetVehicleByRemoteId(ushort id)
        {
            foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
            {
                if (veh.Id == id)
                {
                    return veh;
                }
            }
            return null;
        }


        public void RefreshVehiclesTune(Vehicle vehicle)
        {
            for (int i = 0; i < 70; i++)
            {
                if (i != 12 && i != 18 && i != 23 && i != 24)
                    vehicle.SetMod(i, -1);
            }
            if (vehicle.HasSharedData("tune"))
            {
                Dictionary<int, int> tune = JsonConvert.DeserializeObject<Dictionary<int, int>>(vehicle.GetSharedData<string>("tune"));
                foreach (KeyValuePair<int, int> pair in tune)
                {
                    vehicle.SetMod(pair.Key, pair.Value);
                }
            }
            if (vehicle.HasSharedData("mechtune")){
                List<int> tuneList = JsonConvert.DeserializeObject<List<int>>(vehicle.GetSharedData<string>("mechtune"));
                CustomVehicle customVehicle = null;
                foreach (CustomVehicle cV in customVehicles.AllVehicles)
                {
                    if (cV.model == vehicle.Model)
                    {
                        customVehicle = cV;
                        break;
                    }
                }
                if (customVehicle != null)
                {
                    int stockSpeed = customVehicle.speed;
                    int speed = customVehicle.speed;
                    int power = customVehicle.power;
                    int combustion = customVehicle.combustion;
                    if (tuneList.Count > 0)
                    {
                        //Turbosprężarka - 8 powera i 5% do vmaxa
                        if (tuneList[0] == 1)
                        {
                            vehicle.SetMod(18, 0);
                            power += 8;
                            speed += Convert.ToInt32(stockSpeed * 0.05);
                        }
                        //Sportowy rozrząd - 6 powera i 5% do vmaxa
                        if (tuneList[1] == 1)
                        {
                            power += 6;
                            speed += Convert.ToInt32(stockSpeed * 0.05);
                            combustion += 1;
                        }
                        //Mapowanie ECU - 6 powera i 6% do vmaxa
                        if (tuneList[2] == 1)
                        {
                            power += 6;
                            speed += Convert.ToInt32(stockSpeed * 0.06);
                        }
                        //Wydajniejsze wtryski paliwa - 4 powera i 4% do vmaxa
                        if (tuneList[3] == 1)
                        {
                            power += 4;
                            speed += Convert.ToInt32(stockSpeed * 0.04);
                            combustion += 1;
                        }
                        //
                        //Sportowe hamulce
                        if (tuneList[4] == 1)
                        {
                            vehicle.SetMod(12, 2);
                        }
                        vehicle.SetSharedData("speed", speed);
                        vehicle.SetSharedData("power", power);
                        vehicle.SetSharedData("combustion", combustion);
                    }
                } 
            }
            SetVehiclesWheels(vehicle);
        }
        public string GetVehicleAvailableTune(Vehicle vehicle)
        {
            foreach(CustomVehicle cV in customVehicles.AllVehicles)
            {
                if(cV.model == vehicle.Model)
                {
                    List<string[]> tune = new List<string[]>()
                    {
                        new string[]{"0", "Spoiler", (4000 + cV.visufactor).ToString()},
                        new string[]{"1", "Przedni zderzak", (4300 + cV.visufactor).ToString()},
                        new string[]{"2", "Tylni zderzak", (4200 + cV.visufactor).ToString()},
                        new string[]{"3", "Progi", (2800 + cV.visufactor).ToString()},
                        new string[]{"4", "Wydech", (4600 + cV.visufactor).ToString()},
                        new string[]{"5", "Rama", (4500 + cV.visufactor).ToString()},
                        new string[]{"6", "Grill", (1200 + cV.visufactor).ToString()},
                        new string[]{"7", "Maska", (3600 + cV.visufactor).ToString()},
                        new string[]{"8", "Nadkola", (2600 + cV.visufactor).ToString()},
                        new string[]{"9", "Tylne nadkola", (2400 + cV.visufactor).ToString()},
                        new string[]{"10", "Dach", (1500 + cV.visufactor).ToString()},
                        new string[]{"15", "Zawieszenie", (5500 + cV.visufactor).ToString()},
                        new string[]{"22", "Xenony", (4500 + cV.visufactor).ToString()},
                        new string[]{"25", "Uchwyt na rejestracje", (600 + cV.visufactor).ToString()},
                        new string[]{"27", "Kolor wykończenia", (1800 + cV.visufactor).ToString()},
                        new string[]{"28", "Ozdoby", (2400 + cV.visufactor).ToString()},
                        new string[]{"29", "Deska rozdzielcza", (1600 + cV.visufactor).ToString()},
                        new string[]{"30", "Tarcza licznika", (600 + cV.visufactor).ToString()},
                        new string[]{"31", "Głośniki w drzwiach", (1100 + cV.visufactor).ToString()},
                        new string[]{"32", "Siedzenia", (2400 + cV.visufactor).ToString()},
                        new string[]{"33", "Kierownica", (800 + cV.visufactor).ToString()},
                        new string[]{"34", "Gałka do zmiany biegów", (600 + cV.visufactor).ToString()},
                        new string[]{"36", "Głośniki", (1500 + cV.visufactor).ToString()},
                        new string[]{"37", "Bagażnik", (1600 + cV.visufactor).ToString()},
                        new string[]{"38", "Hydraulika", (10000 + cV.visufactor).ToString()},
                        new string[]{"39", "Blok silnika", (500 + cV.visufactor).ToString()},
                        new string[]{"40", "Filtr powietrza", (1000 + cV.visufactor).ToString()},
                        new string[]{"41", "Rozpórki", (600 + cV.visufactor).ToString()},
                        new string[]{"42", "Nadkola", (2600 + cV.visufactor).ToString()},
                        new string[]{"43", "Anteny", (1200 + cV.visufactor).ToString()},
                        new string[]{"48", "Oznaczenia", (8000 + cV.visufactor).ToString()},
                        new string[]{"55", "Szyby", (4000 + cV.visufactor).ToString()}
                    };
                    List<string[]> availableTune = new List<string[]>();
                    Dictionary<int, int> currentTune = JsonConvert.DeserializeObject<Dictionary<int, int>>(vehicle.GetSharedData<string>("tune"));
                    foreach(string[] str in tune)
                    {
                        if(!currentTune.ContainsKey(int.Parse(str[0])))
                        {
                            availableTune.Add(str);
                        }
                    }
                    return JsonConvert.SerializeObject(availableTune);
                }
            }
            return "[]";
        }

        public string GetAvailableWheelsForVehicle(Vehicle vehicle)
        {
            if(NAPI.Vehicle.GetVehicleClass((VehicleHash)vehicle.Model) != 8)
            {
                List<List<string[]>> availableWheels = new List<List<string[]>>();
                availableWheels.Add(GetWheelListOfType(wheels.sport));
                availableWheels.Add(GetWheelListOfType(wheels.muscle));
                availableWheels.Add(GetWheelListOfType(wheels.lowrider));
                availableWheels.Add(GetWheelListOfType(wheels.suv));
                availableWheels.Add(GetWheelListOfType(wheels.offroad));
                availableWheels.Add(GetWheelListOfType(wheels.tuner));
                availableWheels.Add(GetWheelListOfType(wheels.highend));
                availableWheels.Add(GetWheelListOfType(wheels.benny_o));
                availableWheels.Add(GetWheelListOfType(wheels.benny_b));

                return JsonConvert.SerializeObject(availableWheels);
            }
            else
            {
                List<List<string[]>> availableWheels = new List<List<string[]>>();
                availableWheels.Add(GetWheelListOfType(wheels.bike));

                return JsonConvert.SerializeObject(availableWheels);
            }
        }

        public string GetVehiclesMechTune(Vehicle vehicle)
        {
            int mechfactor = 0;
            foreach(CustomVehicle customVehicle in customVehicles.AllVehicles)
            {
                if(customVehicle.model == vehicle.Model)
                {
                    mechfactor = customVehicle.mechfactor;
                    break;
                }
            }
            List<string[]> tune = new List<string[]>();
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("mechtune"))
            {
                int[] mechtune = JsonConvert.DeserializeObject<int[]>(vehicle.GetSharedData<string>("mechtune"));
                tune.Add(new string[] { mechtune[0] == 1 ? "1" : "0", "Turbosprężarka", (24000 + mechfactor).ToString()});
                tune.Add(new string[] { mechtune[1] == 1 ? "1" : "0", "Sportowy rozrząd", (16000 + mechfactor).ToString()});
                tune.Add(new string[] { mechtune[2] == 1 ? "1" : "0", "Mapowanie ECU", (18000 + mechfactor).ToString()});
                tune.Add(new string[] { mechtune[3] == 1 ? "1" : "0", "Wydajniejsze wtryski paliwa", (12000 + mechfactor).ToString()});
                tune.Add(new string[] { mechtune[4] == 1 ? "1" : "0", "Sportowe hamulce", (8000 + mechfactor).ToString()});
            }
            return JsonConvert.SerializeObject(tune);              
        }

        public string GetAllAvailableWheels()
        {
            List<string[]> availableWheels = new List<string[]>();
            foreach(Wheel wheel in wheels.allCurrentWheels)
            {
                availableWheels.Add(new string[]
                {
                    wheel.name,
                    wheel.type.ToString(),
                    wheel.id.ToString(),
                    wheel.price.ToString()
                });
            }
            return JsonConvert.SerializeObject(availableWheels);
        }
        public List<string[]> GetWheelListOfType(List<Wheel> wheels)
        {
            List<string[]> tempWheel = new List<string[]>();
            foreach (Wheel wheel in wheels)
            {
                tempWheel.Add(new string[]
                {
                    wheel.name,
                    wheel.id.ToString(),
                    wheel.price.ToString()
                });

            }
            return tempWheel;
        }
        public string GetVehiclesCurrentTune(Vehicle vehicle)
        {
            foreach (CustomVehicle cV in customVehicles.AllVehicles)
            {
                if (cV.model == vehicle.Model)
                {
                    List<string[]> names = new List<string[]>()
                    {
                        new string[]{"0", "Spoiler", (4000 + cV.visufactor).ToString()},
                        new string[]{"1", "Przedni zderzak", (4300 + cV.visufactor).ToString()},
                        new string[]{"2", "Tylni zderzak", (4200 + cV.visufactor).ToString()},
                        new string[]{"3", "Progi", (2800 + cV.visufactor).ToString()},
                        new string[]{"4", "Wydech", (4600 + cV.visufactor).ToString()},
                        new string[]{"5", "Rama", (4500 + cV.visufactor).ToString()},
                        new string[]{"6", "Grill", (1200 + cV.visufactor).ToString()},
                        new string[]{"7", "Maska", (3600 + cV.visufactor).ToString()},
                        new string[]{"8", "Nadkola", (2600 + cV.visufactor).ToString()},
                        new string[]{"9", "Tylne nadkola", (2400 + cV.visufactor).ToString()},
                        new string[]{"10", "Dach", (1500 + cV.visufactor).ToString()},
                        new string[]{"15", "Zawieszenie", (5500 + cV.visufactor).ToString()},
                        new string[]{"22", "Xenony", (4500 + cV.visufactor).ToString()},
                        new string[]{"25", "Uchwyt na rejestracje", (600 + cV.visufactor).ToString()},
                        new string[]{"27", "Kolor wykończenia", (1800 + cV.visufactor).ToString()},
                        new string[]{"28", "Ozdoby", (2400 + cV.visufactor).ToString()},
                        new string[]{"29", "Deska rozdzielcza", (1600 + cV.visufactor).ToString()},
                        new string[]{"30", "Tarcza licznika", (600 + cV.visufactor).ToString()},
                        new string[]{"31", "Głośniki w drzwiach", (1100 + cV.visufactor).ToString()},
                        new string[]{"32", "Siedzenia", (2400 + cV.visufactor).ToString()},
                        new string[]{"33", "Kierownica", (800 + cV.visufactor).ToString()},
                        new string[]{"34", "Gałka do zmiany biegów", (600 + cV.visufactor).ToString()},
                        new string[]{"36", "Głośniki", (1500 + cV.visufactor).ToString()},
                        new string[]{"37", "Bagażnik", (1600 + cV.visufactor).ToString()},
                        new string[]{"38", "Hydraulika", (10000 + cV.visufactor).ToString()},
                        new string[]{"39", "Blok silnika", (500 + cV.visufactor).ToString()},
                        new string[]{"40", "Filtr powietrza", (1000 + cV.visufactor).ToString()},
                        new string[]{"41", "Rozpórki", (600 + cV.visufactor).ToString()},
                        new string[]{"42", "Nadkola", (2600 + cV.visufactor).ToString()},
                        new string[]{"43", "Anteny", (1200 + cV.visufactor).ToString()},
                        new string[]{"48", "Oznaczenia", (8000 + cV.visufactor).ToString()},
                        new string[]{"55", "Szyby", (4000 + cV.visufactor).ToString()}
                    };

                    List<string[]> currentTune = new List<string[]>();
                    Dictionary<int, int> tune = JsonConvert.DeserializeObject<Dictionary<int, int>>(vehicle.GetSharedData<string>("tune"));
                    foreach (KeyValuePair<int, int> pair in tune)
                    {
                        string price = "";
                        string name = "";
                        foreach (string[] n in names)
                        {
                            if(n[0] == pair.Key.ToString())
                            {
                                price = n[2];
                                name = n[1];
                                break;
                            }
                        }
                        currentTune.Add(new string[]
                        {
                            name + " " + (pair.Value + 1).ToString(),
                            pair.Key.ToString(),
                            pair.Value.ToString(),
                            price
                        });
                    }
                    return JsonConvert.SerializeObject(currentTune);
                }
            }
            return "[]";
                    
        }
        public decimal[] getVehiclesFixPriceAndTime(Vehicle vehicle)
        {
            float modelPrice = GetVehicleModelPrice(vehicle);
            if(modelPrice == 0)
            {
                return new decimal[] {0, 0};
            }
            else
            {
                float maxCarPrice = 2000000.0f; //dwa miliony max
                float maxPartPrice = 150.0f;
                double priceScale = Convert.ToDouble(modelPrice) / Convert.ToDouble(maxCarPrice);
                priceScale *= 50;
                double partPriced = (10.0 * (-1 * Math.Pow((1.0 / 2.0), priceScale / 6.0)) + 10.0) / 10.0 * maxPartPrice;
                partPriced = Convert.ToSingle(Math.Round(Convert.ToDouble(partPriced), 2));
                float partPrice = Convert.ToSingle(partPriced);
                float doorMultiplier = 0.8f;
                float windowMultiplier = 0.6f;
                float engineMultiplier = 3f;
                float bodyMultiplier = 2f;
                float price = 0.0f;
                float time = 0.0f;
                string damageString = vehicle.GetSharedData<string>("damage");
                Dictionary<string, float> damageDict = JsonToDamage(damageString);
                foreach (KeyValuePair<string, float> damagePair in damageDict)
                {
                    string part = damagePair.Key;
                    float value = damagePair.Value;

                    if (part == "engine" && value != 1000.0f)
                    {
                        price += partPrice * engineMultiplier * (Math.Abs(value - 1000) / 1000);
                        time += 5.0f;
                    }
                    if (part == "body" && value != 1000.0f)
                    {
                        price += partPrice * bodyMultiplier * (Math.Abs(value - 1000) / 1000);
                        time += 5.0f;
                    }
                    if (part.Contains("window") && value == 0.0f)
                    {
                        price += partPrice * windowMultiplier;
                        time += 3.0f;
                    }
                    if ((part.Contains("door") && value == 0.0f) || (part == "hood" & value == 0.0f) || (part == "trunk" & value == 0.0f))
                    {
                        price += partPrice * doorMultiplier;
                        time += 3.0f;
                    }
                    if (part.Contains("wheel") && value == 0.0f)
                    {
                        price += 50;
                        time += 5;
                    }
                }
                decimal decprice = Math.Round((decimal)price);
                decimal[] priceandtime = new decimal[] { decprice, (decimal)time };
                return priceandtime;
            }
        }

        public void setRepairingInterval(Vehicle vehicle, decimal time, Player player, string partsToRepair)
        {
            List<string> parts = JsonConvert.DeserializeObject<List<string>>(partsToRepair);
            string damageString = vehicle.GetSharedData<string>("damage");
            Dictionary<string, float> newDamage = new Dictionary<string, float>();
            Dictionary<string, float> damageDict = JsonToDamage(damageString);
            foreach (KeyValuePair<string, float> damagePair in damageDict)
            {
                string name = damagePair.Key;
                float newValue = damagePair.Value;
                if(parts.Contains(damagePair.Key))
                {
                    if(damagePair.Key == "engine" || damagePair.Key == "body")
                    {
                        newValue = 1000.0f;
                    }
                    else
                    {
                        newValue = 1.0f;
                    }
                }
                newDamage.Add(name, newValue);
            }


            vehicle.SetSharedData("mech", true);
            vehicle.Locked = true;
            vehicle.SetSharedData("veh_brake", true);
            var label = NAPI.TextLabel.CreateTextLabel("Pojazd w trakcie naprawy", new Vector3(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z + 1.2), 4, 0.7f, 4, new Color(0, 204, 153), entitySeethrough: false);
            if (vehicle.Occupants.Contains(player))
            {
                player.WarpOutOfVehicle();
            }
            System.Threading.Tasks.Task.Run(() =>
            {
                NAPI.Task.Run(() =>
                {
                    if (label.Exists)
                    {
                        label.Delete();
                    }
                    if (vehicle.Exists)
                    {
                        vehicle.Locked = false;
                        vehicle.Repair();
                        UpdateVehiclesDamage(vehicle, DamageToJson(newDamage));
                        vehicle.SetSharedData("mech", false);
                        vehicle.SetSharedData("veh_brake", false);
                    }
                }, delayTime: Convert.ToInt32(time) * 1000);
            });
        }

        public void RepairVehicle(Vehicle vehicle)
        {
            UpdateVehiclesDamage(vehicle, defaultDamage);
            vehicle.Repair();
            vehicle.SetSharedData("mech", false);
        }

        public Vehicle GetRandomVehicleToTow()
        {
            List<Vehicle> vehicles = new List<Vehicle>();
            Vehicle vehicle = null;
            foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
            {
                if (veh != null && veh.HasSharedData("type") && veh.GetSharedData<string>("type") == "personal" && !(veh.HasSharedData("towed") && veh.GetSharedData<bool>("towed")) && veh.Occupants.Count == 0 && !(veh.HasSharedData("market") && veh.GetSharedData<bool>("market")))
                    if (DateTime.Compare(DateTime.Parse(veh.GetSharedData<string>("used")).AddHours(32), DateTime.Now) < 0)
                    {
                        vehicles.Add(veh);
                    }
            }
            if (vehicles.Count > 0)
            {
                Random r = new Random();
                int rand = r.Next(0, vehicles.Count);
                vehicle = vehicles[rand];
                vehicle.SetSharedData("towed", true);
            }
            return vehicle;
        }

        public int[] GetVehicleStockPowerAndSpeed(Vehicle vehicle)
        {
            int[] PandS = new int[2];
            foreach(CustomVehicle cV in customVehicles.AllVehicles)
            {
                if(cV.model == vehicle.Model)
                {
                    PandS[0] = cV.power;
                    PandS[1] = cV.speed;
                    break;
                }
            }

            return PandS;
        }
        public void applyTuneToVehicle(Vehicle vehicle, string vizuString, string mechString)
        {
            Dictionary<int, int> tune = JsonToTune(vizuString);

            foreach (KeyValuePair<int, int> tunePair in tune)
            {
                switch (tunePair.Key)
                {
                    case 500:
                        NAPI.Vehicle.SetVehicleWheelType(vehicle.Handle, tunePair.Value);
                        break;
                    case 501:
                        NAPI.Vehicle.SetVehicleWheelColor(vehicle.Handle, tunePair.Value);
                        break;
                    default:
                        NAPI.Vehicle.SetVehicleMod(vehicle.Handle, tunePair.Key, tunePair.Value);
                        break;
                }
            }
            List<int> tuneList = JsonConvert.DeserializeObject<List<int>>(mechString);
            CustomVehicle customVehicle = null;
            foreach (CustomVehicle cV in customVehicles.AllVehicles)
            {
                if (cV.model == vehicle.Model)
                {
                    customVehicle = cV;
                    break;
                }
            }
            if (customVehicle != null)
            {
                int stockSpeed = customVehicle.speed;
                int speed = customVehicle.speed;
                int power = customVehicle.power;
                int combustion = customVehicle.combustion;
                if (tuneList.Count > 0)
                {
                    //Turbosprężarka - 8 powera i 5% do vmaxa
                    if (tuneList[0] == 1)
                    {
                        vehicle.SetMod(18, 0);
                        power += 8;
                        speed += Convert.ToInt32(stockSpeed * 0.05);
                    }
                    //Sportowy rozrząd - 6 powera i 5% do vmaxa
                    if (tuneList[1] == 1)
                    {
                        power += 6;
                        speed += Convert.ToInt32(stockSpeed * 0.05);
                        combustion += 1;
                    }
                    //Mapowanie ECU - 6 powera i 6% do vmaxa
                    if (tuneList[2] == 1)
                    {
                        power += 6;
                        speed += Convert.ToInt32(stockSpeed * 0.06);
                    }
                    //Wydajniejsze wtryski paliwa - 4 powera i 4% do vmaxa
                    if (tuneList[3] == 1)
                    {
                        power += 4;
                        speed += Convert.ToInt32(stockSpeed * 0.04);
                        combustion += 1;
                    }
                    //
                    //Sportowe hamulce
                    if (tuneList[4] == 1)
                    {
                        vehicle.SetMod(12, 2);
                    }
                    vehicle.SetSharedData("speed", speed);
                    vehicle.SetSharedData("power", power);
                    vehicle.SetSharedData("combustion", combustion);


                }
            }


        }

        public string GetVehiclesOwner(string id)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM vehicles WHERE id = {id};";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.Read())
                {
                    string owner = reader.GetString(1);
                    dataBase.connection.Close();
                    return owner;
                }
                else
                {
                    dataBase.connection.Close();
                    return "";
                }
            }

        }
        public string GetVehiclesOwnerName(string id)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM vehicles WHERE id = {id};";
            string owner = "";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.Read())
                {
                    owner = reader.GetString(1);
                }
            }
            if (owner == "")
            {
                dataBase.connection.Close();
            }
            else
            {
                dataBase.command.CommandText = $"SELECT * FROM users WHERE login = {owner};";
                using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string ow = reader.GetString(3);
                        dataBase.connection.Close();
                        return ow;
                    }
                    else
                    {
                        return "";
                    }

                }
            }
            return "";
        }
        public string GetVehiclesNameById(string id)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM vehicles WHERE id = {id};";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.Read())
                {
                    string name = reader.GetString(3);
                    dataBase.connection.Close();
                    return name;
                }
                else
                {
                    dataBase.connection.Close();
                    return "";
                }
            }
        }

        public string GetVehiclesTuneString(Vehicle vehicle)
        {
            List<string> stringTune = new List<string>();
            int[] mechTune = JsonConvert.DeserializeObject<int[]>(vehicle.GetSharedData<string>("mechtune"));
            //Turbosprężarka - 8 powera i 5% do vmaxa
            if (mechTune[0] == 1)
            {
                stringTune.Add("Turbosprężarka");
            }
            if (mechTune[1] == 1)
            {
                stringTune.Add("Sportowy rozrząd");
            }
            if (mechTune[2] == 1)
            {
                stringTune.Add("Mapowanie ECU");
            }
            if (mechTune[3] == 1)
            {
                stringTune.Add("Wydajniejsze wtryski paliwa");
            }
            if (mechTune[4] == 1)
            {
                stringTune.Add("Sportowe hamulce");
            }

            //visu
            Dictionary<string, string> names = new Dictionary<string, string>()
            {
                ["0"] = "Spoiler",
                ["1"] = "Przedni zderzak",
                ["2"] = "Tylni zderzak",
                ["3"] = "Progi",
                ["4"] = "Wydech",
                ["5"] = "Rama",
                ["6"] = "Grill",
                ["7"] = "Maska",
                ["8"] = "Nadkola",
                ["9"] = "Tylne nadkola",
                ["10"] = "Dach",
                ["15"] = "Zawieszenie",
                ["22"] = "Xenony",
                ["25"] = "Uchwyt na rejestracje",
                ["27"] = "Kolor wykończenia",
                ["28"] = "Ozdoby",
                ["29"] = "Deska rozdzielcza",
                ["30"] = "Tarcza licznika",
                ["31"] = "Głośniki w drzwiach",
                ["32"] = "Siedzenia",
                ["33"] = "Kierownica",
                ["34"] = "Gałka do zmiany biegów",
                ["36"] = "Głośniki",
                ["37"] = "Bagażnik",
                ["38"] = "Hydraulika",
                ["39"] = "Blok silnika",
                ["40"] = "Filtr powietrza",
                ["41"] = "Rozpórki",
                ["42"] = "Nadkola",
                ["43"] = "Anteny",
                ["48"] = "Oznaczenia",
                ["55"] = "Szyby"
            };

            Dictionary<int, int> visuTune = JsonConvert.DeserializeObject<Dictionary<int, int>>(vehicle.GetSharedData<string>("tune"));
            foreach (KeyValuePair<int, int> pair in visuTune)
            {
                stringTune.Add(names[pair.Key.ToString()] + " " + (pair.Value + 1).ToString());
            }
            return JsonConvert.SerializeObject(stringTune);
        }

        public void UpdateVehiclesDBOwner(int carId, string newOwner)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"UPDATE vehicles SET owner = '{newOwner}' WHERE id = {carId.ToString()}";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }

        public void setVehiclesPetrolAndTrunk(Vehicle vehicle)
        {
            foreach(CustomVehicle cV in customVehicles.AllVehicles)
            {
                if(cV.model == vehicle.Model)
                {
                    vehicle.SetSharedData("petroltank", cV.tank);
                    vehicle.SetSharedData("combustion", cV.combustion);
                    vehicle.SetSharedData("trunksize", JsonConvert.SerializeObject(cV.trunk));
                    break;
                }
            }
        }

        public void SetVehiclesExtra(Vehicle vehicle)
        {
            foreach(CustomVehicle customVehicle in customVehicles.AllVehicles)
            {
                if(customVehicle.model == vehicle.Model)
                {
                    vehicle.SetSharedData("extra", customVehicle.extra);
                    for (int i = 0; i < 20; i++)
                    {
                        vehicle.SetExtra(i, false);
                    }
                    vehicle.SetExtra(customVehicle.extra, true);
                    break;
                }
            }
        }

        public void SetVehiclesWheels(Vehicle vehicle)
        {
            if (vehicle.HasSharedData("wheels")){
                int[] wheels = JsonConvert.DeserializeObject<int[]>(vehicle.GetSharedData<string>("wheels"));
                NAPI.Vehicle.SetVehicleWheelType(vehicle.Handle, wheels[0]);
                vehicle.SetMod(23, wheels[1]);
                if (NAPI.Vehicle.GetVehicleClass((VehicleHash)vehicle.Model) == 8)
                {
                    vehicle.SetMod(24, wheels[1]);
                }
                NAPI.Vehicle.SetVehicleWheelColor(vehicle.Handle, 156);
                NAPI.Vehicle.SetVehicleCustomTires(vehicle.Handle, wheels[2] == 1 ? true : false);
                vehicle.SetSharedData("offroad", handlingManager.IsCarOffroadType(vehicle));
            }
        }

        public Vehicle GetClosestVehicle(Player player)
        {
            Vehicle closest = null;
            foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
            {
                if (veh.Position.DistanceTo(player.Position) < 4)
                {
                    if (closest == null)
                    {
                        closest = veh;
                    }
                    else if (veh.Position.DistanceTo(player.Position) < closest.Position.DistanceTo(player.Position))
                    {
                        closest = veh;
                    }
                }
            }
            return closest;
        }

        public string GetPlayersVehicles(Player player)
        {
            List<List<string>> vehicles = new List<List<string>>();
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT id, name, model FROM vehicles WHERE owner = '{player.SocialClubId}';";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while(reader.Read())
                {
                    List<string> veh = new List<string>()
                    {
                        reader.GetInt32(0).ToString(),
                        reader.GetString(1),
                        reader.GetString(2)
                    };
                    vehicles.Add(veh);
                }
            }
            dataBase.connection.Close();
            return JsonConvert.SerializeObject(vehicles);
        }

        public string[] GatherVehiclesInfo(int id)
        {
            string[] tune = new string[3];
            List<string> vehInfo = new List<string>();
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT id, name, model, mechtune, tune, spawned, trip FROM vehicles WHERE id = {id};";
            string model = "";
            string mechtune = "";
            string visutune = "";
            string trip = "";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.Read())
                {
                    vehInfo.Add(reader.GetString(1));
                    vehInfo.Add(reader.GetInt32(0).ToString());
                    switch (reader.GetString(5))
                    {
                        case "True":
                            vehInfo.Add("na zewnątrz");
                            break;
                        case "False":
                            vehInfo.Add("w przechowalni");
                            break;
                    }
                    model = reader.GetString(2);
                    mechtune = reader.GetString(3);
                    visutune = reader.GetString(4);
                    trip = reader.GetFloat(6).ToString();
                }
                else
                {
                    dataBase.connection.Close();
                    return tune;
                }
            }
            dataBase.connection.Close();

            int combustionAdd = 0;

            int[] mech = JsonConvert.DeserializeObject<int[]>(mechtune);
            List<string> mtune = new List<string>();

            if (mech[0] == 1)
                mtune.Add("Turbosprężarka");
            if (mech[1] == 1)
            {
                mtune.Add("Sportowy rozrząd");
                combustionAdd++;
            }
            if (mech[2] == 1)
                mtune.Add("Mapowanie ECU");
            if (mech[3] == 1)
            {
                mtune.Add("Wydajniejsze wtryski paliwa");
                combustionAdd++;
            }
            if (mech[4] == 1)
                mtune.Add("Sportowe hamulce");

            foreach (CustomVehicle cV in customVehicles.AllVehicles)
            {
                if (cV.model.ToString() == model)
                {
                    vehInfo.Add((cV.combustion + combustionAdd).ToString());
                    vehInfo.Add(cV.tank.ToString());
                    vehInfo.Add((cV.trunk[0] * cV.trunk[1]).ToString());
                    break;
                }
            }
            vehInfo.Add(trip);
            tune[0] = JsonConvert.SerializeObject(vehInfo);
            tune[1] = mtune.Count > 0 ? JsonConvert.SerializeObject(mtune) : "";
            
            Dictionary<string, string> names = new Dictionary<string, string>()
            {
                ["0"] = "Spoiler",
                ["1"] = "Przedni zderzak",
                ["2"] = "Tylni zderzak",
                ["3"] = "Progi",
                ["4"] = "Wydech",
                ["5"] = "Rama",
                ["6"] = "Grill",
                ["7"] = "Maska",
                ["8"] = "Nadkola",
                ["9"] = "Tylne nadkola",
                ["10"] = "Dach",
                ["15"] = "Zawieszenie",
                ["22"] = "Xenony",
                ["25"] = "Uchwyt na rejestracje",
                ["27"] = "Kolor wykończenia",
                ["28"] = "Ozdoby",
                ["29"] = "Deska rozdzielcza",
                ["30"] = "Tarcza licznika",
                ["31"] = "Głośniki w drzwiach",
                ["32"] = "Siedzenia",
                ["33"] = "Kierownica",
                ["34"] = "Gałka do zmiany biegów",
                ["36"] = "Głośniki",
                ["37"] = "Bagażnik",
                ["38"] = "Hydraulika",
                ["39"] = "Blok silnika",
                ["40"] = "Filtr powietrza",
                ["41"] = "Rozpórki",
                ["42"] = "Nadkola",
                ["43"] = "Anteny",
                ["48"] = "Oznaczenia",
                ["55"] = "Szyby"
            };

            Dictionary<int, int> visuTune = JsonConvert.DeserializeObject<Dictionary<int, int>>(visutune);
            List<string> visuList = new List<string>();
            foreach (KeyValuePair<int, int> pair in visuTune)
            {
                visuList.Add(names[pair.Key.ToString()] + " " + (pair.Value + 1).ToString());
            }
            tune[2] = visuList.Count > 0 ? JsonConvert.SerializeObject(visuList) : "";

            return tune;
        }

        public int GetVehiclesSellPrice(Vehicle vehicle)
        {
            int price = 0;
            double baseFactor = 0.5;
            foreach(CustomVehicle customVehicle in customVehicles.AllVehicles)
            {
                if(customVehicle.model == vehicle.Model)
                {
                    price = customVehicle.price;
                    double factor = 0.25 * (Math.Clamp(Convert.ToInt32(vehicle.GetSharedData<float>("veh_trip")), 0, 100000) / 100000);
                    baseFactor -= factor;
                    break;
                }
            }
            price = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(price) * baseFactor));
            return price;
        }

        public Vector3 JsonToVector(string pos)
        {
            float[] a = JsonConvert.DeserializeObject<float[]>(pos);
            return new Vector3(a[0], a[1], a[2]);
        }

        public string VectorToJson(Vector3 pos)
        {
            float[] a = new float[] { pos.X, pos.Y, pos.Z };
            return JsonConvert.SerializeObject(a);
        }

        public string ColorToJson(Color color, int colormod)
        {
            int[] colors = new int[] { color.Red, color.Green, color.Blue, colormod };
            return JsonConvert.SerializeObject(colors);
        }

        public int[] JsonToColor(string colorString)
        {
            int[] color = JsonConvert.DeserializeObject<int[]>(colorString);
            Int32[] onlycolor = new Int32[] { color[0], color[1], color[2] };
            return onlycolor;
        }
        public int JsonToColorMod(string colorString)
        {
            int[] color = JsonConvert.DeserializeObject<int[]>(colorString);
            return color[3];
        }

        public Dictionary<string, float> JsonToDamage(string damageString)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, float>>(damageString);
        }
        public string DamageToJson(Dictionary<string, float> damageDict)
        {
            return JsonConvert.SerializeObject(damageDict);
        }
        public Dictionary<int, int> JsonToTune(string tuneString)
        {
            if (tuneString == "{}")
            {
                return new Dictionary<int, int>();
            }
            return JsonConvert.DeserializeObject<Dictionary<int, int>>(tuneString);
        }
        public string TuneToJson(Dictionary<int, int> tune)
        {
            return JsonConvert.SerializeObject(tune);
        }

        public string GetVehiclesWheels(Vehicle vehicle)
        {
            int[] wheels = JsonConvert.DeserializeObject<int[]>(vehicle.GetSharedData<string>("wheels"));
            if(wheels[0] == 0 && wheels[1] == -1 && wheels[2] == 0)
            {
                string[] str = new string[]
                {
                    "",
                    " ",
                    " "
                };
                return JsonConvert.SerializeObject(str);
            }
            else{
                foreach (Wheel wheel in this.wheels.allWheels)
                {
                    if(wheel.type == wheels[0] && wheel.id == wheels[1])
                    {
                        string[] str = new string[]
                        {
                            wheel.name,
                            wheel.type.ToString(),
                            wheel.id.ToString(),
                            wheel.price.ToString()
                        };
                        return JsonConvert.SerializeObject(str);
                    }
                }
                return "[]";
            }
        }
        public string GetWheelNameById(int type, int id)
        {
            string name = "";
            foreach(Wheel wheel in wheels.allWheels)
            {
                if(wheel.type == type && wheel.id == id)
                {
                    name = wheel.name;
                    break;
                }
            }
            return name;
        }
        public int GetWheelPriceById(int type, int id)
        {
            int price = 0;
            foreach (Wheel wheel in wheels.allWheels)
            {
                if (wheel.type == type && wheel.id == id)
                {
                    price = wheel.price;
                    break;
                }
            }
            return price;
        }
        public string GetAllVehiclesModels()
        {
            List<string> models = new List<string>();
            foreach(CustomVehicle vehicle in customVehicles.AllVehicles)
            {
                models.Add(vehicle.model.ToString());
            }
            return JsonConvert.SerializeObject(models);
        }
    }

    public class CustomVehicles
    {
        public List<CustomVehicle> AllVehicles = new List<CustomVehicle>();
        public List<CustomVehicle> Bikes = new List<CustomVehicle>();
        public List<CustomVehicle> Junks = new List<CustomVehicle>();
        public List<CustomVehicle> Hypers = new List<CustomVehicle>();
        public List<CustomVehicle> Offroads = new List<CustomVehicle>();
        public List<CustomVehicle> Regulars = new List<CustomVehicle>();
        public List<CustomVehicle> Regulars2 = new List<CustomVehicle>();
        public List<CustomVehicle> Sports = new List<CustomVehicle>();
        public List<CustomVehicle> Classics = new List<CustomVehicle>();
        public List<CustomVehicle> Specials = new List<CustomVehicle>();
        public List<CustomVehicle> SUVs = new List<CustomVehicle>();

        public CustomVehicles()
        {
            Bikes = JsonConvert.DeserializeObject<List<CustomVehicle>>(File.ReadAllText(@"Vehicles/bike.json"));
            Hypers = JsonConvert.DeserializeObject<List<CustomVehicle>>(File.ReadAllText(@"Vehicles/hyper.json"));
            Junks = JsonConvert.DeserializeObject<List<CustomVehicle>>(File.ReadAllText(@"Vehicles/junk.json"));
            Offroads = JsonConvert.DeserializeObject<List<CustomVehicle>>(File.ReadAllText(@"Vehicles/offroad.json"));
            Regulars = JsonConvert.DeserializeObject<List<CustomVehicle>>(File.ReadAllText(@"Vehicles/regular.json"));
            Regulars2 = JsonConvert.DeserializeObject<List<CustomVehicle>>(File.ReadAllText(@"Vehicles/regular2.json"));
            Classics = JsonConvert.DeserializeObject<List<CustomVehicle>>(File.ReadAllText(@"Vehicles/classics.json"));
            Sports = JsonConvert.DeserializeObject<List<CustomVehicle>>(File.ReadAllText(@"Vehicles/sport.json"));
            SUVs = JsonConvert.DeserializeObject<List<CustomVehicle>>(File.ReadAllText(@"Vehicles/suv.json"));
            Specials = JsonConvert.DeserializeObject<List<CustomVehicle>>(File.ReadAllText(@"Vehicles/special.json"));

            AllVehicles.AddRange(Bikes);
            AllVehicles.AddRange(Hypers);
            AllVehicles.AddRange(Offroads);
            AllVehicles.AddRange(Regulars);
            AllVehicles.AddRange(Regulars2);
            AllVehicles.AddRange(Classics);
            AllVehicles.AddRange(Sports);
            AllVehicles.AddRange(SUVs);
            AllVehicles.AddRange(Specials);
            AllVehicles.AddRange(Junks);
        }

        public List<CustomVehicle> GetAllVehiclesOfProb(List<CustomVehicle> vehicles, int prob)
        {
            List<CustomVehicle> probVehs = new List<CustomVehicle>();
            foreach(CustomVehicle cV in vehicles)
            {
                if(cV.probability == prob)
                {
                    probVehs.Add(cV);
                }
            }
            return probVehs;
        }
    }

    public class CustomVehicle
    {
        public string name { get; set; }
        public ulong model { get; set; }
        public int power { get; set; }
        public int speed { get; set; }
        public int combustion { get; set; }
        public int tank { get; set; }
        public int[] trunk { get; set; }
        public int probability { get; set; }
        public int price { get; set; }
        public int mechfactor { get; set; }
        public int visufactor { get; set; }
        public int extra { get; set; }
        public CustomVehicle(string name, ulong model, int power, int speed, int combustion, int tank, int[] trunk, int probability, int price, int mechfactor, int visufactor, int extra)
        {
            this.name = name;
            this.model = model;
            this.power = power;
            this.speed = speed;
            this.combustion = combustion;
            this.tank = tank;
            this.trunk = trunk;
            this.probability = probability;
            this.price = price;
            this.mechfactor = mechfactor;
            this.visufactor = visufactor;
            this.extra = extra;
        }
    }
    public class Wheels
    {
        public List<Wheel> allWheels;
        public List<Wheel> sport = new List<Wheel>(), muscle = new List<Wheel>(), lowrider = new List<Wheel>(), suv = new List<Wheel>(), offroad = new List<Wheel>(), tuner = new List<Wheel>(), bike = new List<Wheel>(), highend = new List<Wheel>(), benny_o = new List<Wheel>(), benny_b = new List<Wheel>();
        public List<Wheel> allCurrentWheels = new List<Wheel>();

        public Wheels()
        {
            allWheels = JsonConvert.DeserializeObject<List<Wheel>>(File.ReadAllText(@"Vehicles/Wheels/allWheels.json"));
            Dictionary<int, int[]> rot = JsonConvert.DeserializeObject<Dictionary<int, int[]>>(File.ReadAllText(@"Vehicles/Wheels/currentWheels.json"));
            foreach(KeyValuePair<int, int[]> r in rot)
            {
                switch(r.Key)
                {
                    case 0:
                        foreach(Wheel wheel in allWheels)
                        {
                            if(wheel.type == 0 && r.Value.Contains<int>(wheel.id))
                            {
                                sport.Add(wheel);
                                allCurrentWheels.Add(wheel);
                            }
                        }
                        break;
                    case 1:
                        foreach (Wheel wheel in allWheels)
                        {
                            if (wheel.type == 1 && r.Value.Contains<int>(wheel.id))
                            {
                                muscle.Add(wheel);
                                allCurrentWheels.Add(wheel);
                            }
                        }
                        break;
                    case 2:
                        foreach (Wheel wheel in allWheels)
                        {
                            if (wheel.type == 2 && r.Value.Contains<int>(wheel.id))
                            {
                                lowrider.Add(wheel);
                                allCurrentWheels.Add(wheel);
                            }
                        }
                        break;
                    case 3:
                        foreach (Wheel wheel in allWheels)
                        {
                            if (wheel.type == 3 && r.Value.Contains<int>(wheel.id))
                            {
                                suv.Add(wheel);
                                allCurrentWheels.Add(wheel);
                            }
                        }
                        break;
                    case 4:
                        foreach (Wheel wheel in allWheels)
                        {
                            if (wheel.type == 4 && r.Value.Contains<int>(wheel.id))
                            {
                                offroad.Add(wheel);
                                allCurrentWheels.Add(wheel);
                            }
                        }
                        break;
                    case 5:
                        foreach (Wheel wheel in allWheels)
                        {
                            if (wheel.type == 5 && r.Value.Contains<int>(wheel.id))
                            {
                                tuner.Add(wheel);
                                allCurrentWheels.Add(wheel);
                            }
                        }
                        break;
                    case 6:
                        foreach (Wheel wheel in allWheels)
                        {
                            if (wheel.type == 6 && r.Value.Contains<int>(wheel.id))
                            {
                                bike.Add(wheel);
                                allCurrentWheels.Add(wheel);
                            }
                        }
                        break;
                    case 7:
                        foreach (Wheel wheel in allWheels)
                        {
                            if (wheel.type == 7 && r.Value.Contains<int>(wheel.id))
                            {
                                highend.Add(wheel);
                                allCurrentWheels.Add(wheel);
                            }
                        }
                        break;
                    case 8:
                        foreach (Wheel wheel in allWheels)
                        {
                            if (wheel.type == 8 && r.Value.Contains<int>(wheel.id))
                            {
                                benny_o.Add(wheel);
                                allCurrentWheels.Add(wheel);
                            }
                        }
                        break;
                    case 9:
                        foreach (Wheel wheel in allWheels)
                        {
                            if (wheel.type == 9 && r.Value.Contains<int>(wheel.id))
                            {
                                benny_b.Add(wheel);
                                allCurrentWheels.Add(wheel);
                            }
                        }
                        break;
                }
                
            }
        }
    }
    public class Wheel
    {
        public string name { get; set; }
        public int type { get; set; }
        public int id { get; set; }
        public int price { get; set; }

        public Wheel(int type, string name, int id, int price)
        {
            this.name = name;
            this.type = type;
            this.id = id;
            this.price = price;
        }
    }
}

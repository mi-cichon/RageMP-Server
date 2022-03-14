using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using MySqlConnector;
using Newtonsoft.Json;
using Object = GTANetworkAPI.Object;
using System.Threading;

namespace ServerSide
{
    public class LSPD
    {
        public List<KeyValuePair<Object, string>> Barriers = new List<KeyValuePair<Object, string>>();
        public List<LSPD_Member> Members = new List<LSPD_Member>();
        public Vector3 Position = new Vector3(441.0825f, -981.2163f, 30.689596f);
        public Vector3 ArrestPosition = new Vector3(461.79675f, -989.22266f, 24.914873f);
        public List<LSPD_Vehicle> Vehicles = new List<LSPD_Vehicle>();
        public List<Arrest> Arrests = new List<Arrest>()
        {
            new Arrest(new Vector3(444.75952f, -1006.8055f, 9.975761f)),
            new Arrest(new Vector3(444.81738f, -1001.7633f, 9.975763f)),
            new Arrest(new Vector3(444.71503f, -997.25854f, 9.975763f)),
            new Arrest(new Vector3(444.76993f, -992.3338f, 9.975763f)),
            new Arrest(new Vector3(444.8222f, -987.688f, 9.975763f)),

            new Arrest(new Vector3(436.09015f, -987.64667f, 9.975763f)),
            new Arrest(new Vector3(436.13513f, -992.50464f, 9.975763f)),
            new Arrest(new Vector3(436.11902f, -997.2568f, 9.975763f)),
            new Arrest(new Vector3(436.22205f, -1002.0277f, 9.975763f)),
            new Arrest(new Vector3(436.22693f, -1006.9028f, 9.975763f))
        };

        PlayerDataManager playerDataManager;
        VehicleDataManager vehicleDataManager;
        public ColShape DutyShape, StorageCenter;

        Vector3 rightMainGatePos = new Vector3(419.8508, -1026.436, 28.00148), leftMainGatePos = new Vector3(419.8508, -1024.071, 28.00148);
        Vector3 rightBackGatePos = new Vector3(488.9158, -1017.352, 26.99018), leftBackGatePos = new Vector3(488.9158, -1019.838, 26.99018);
        Object rightMainGate, leftMainGate, rightBackGate, leftBackGate;
        public LSPD(ref PlayerDataManager playerDataManager, ref VehicleDataManager vehicleDataManager)
        {
            NAPI.Blip.CreateBlip(60, new Vector3(441.0825f, -981.2163f, 30.689596f), 0.7f, 38, name: "Komenda LSPD", shortRange: true);
            DutyShape = NAPI.ColShape.CreateCylinderColShape(new Vector3(447.16238f, -975.6481f, 30.6896f), 1.0f, 2.0f);
            DutyShape.SetSharedData("type", "lspd_duty");
            new LSPDTeleport(new Vector3(464.9664f, -990.0205f, 24.914873f), -94.75534f, new Vector3(440.56226f, -1008.80707f, 9.975761f), -0.5668101f, "Areszt");
            new LSPDTeleport(new Vector3(458.89932f, -1008.0039f, 28.266079f), 93.27793f, new Vector3(445.9707f, -996.62897f, 30.68959f), -1.7872145f, "Komenda LSPD");
            this.playerDataManager = playerDataManager;
            this.vehicleDataManager = vehicleDataManager;
            LoadDataFromDB();
            ColShape takeout = NAPI.ColShape.CreateCylinderColShape(new Vector3(458.48907f, -1017.26117f, 28.211643f), 1.0f, 2.0f);
            takeout.SetSharedData("type", "lspd_storage");
            StorageCenter = NAPI.ColShape.CreateCylinderColShape(new Vector3(458.9818f, -1017.2543f, 28.161114f), 10.0f, 2.0f);
            StorageCenter.SetSharedData("type", "lspd_storageCenter");

            rightMainGate = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_fncsec_01b"), rightMainGatePos, new Vector3(0, 0, 270));
            leftMainGate = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_fncsec_01b"), leftMainGatePos, new Vector3(0, 0, 270));

            ColShape mainGate = NAPI.ColShape.CreateCylinderColShape(new Vector3(419.8152f, -1024.0074f, 29.030754f), 6.0f, 2.0f);
            mainGate.SetSharedData("type", "lspd_mainGate");

            rightBackGate = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_fncsec_01b"), rightBackGatePos, new Vector3(0, 0, 90));
            leftBackGate = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_fncsec_01b"), leftBackGatePos, new Vector3(0, 0, 90));

            ColShape backGate = NAPI.ColShape.CreateCylinderColShape(new Vector3(489.08044f, -1019.98956f, 28.213854f), 6.0f, 2.0f);
            backGate.SetSharedData("type", "lspd_backGate");
        }
        
        public bool AddPlayerToGroup(Player player)
        {
            foreach (LSPD_Member member in Members)
            {
                if (member.Login == player.SocialClubId)
                {
                    return false;
                }
            }
            Members.Add(new LSPD_Member(player.SocialClubId, 1));
            player.SetSharedData("lspd_power", 1);
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"INSERT INTO lspd_members (login, power) VALUES ('{player.SocialClubId}', 1)";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
            return true;
        }

        public bool RemovePlayerFromGroup(Player player)
        {
            foreach (LSPD_Member member in Members)
            {
                if (member.Login == player.SocialClubId)
                {
                    Members.Add(new LSPD_Member(player.SocialClubId, 1));
                    DBConnection dataBase = new DBConnection();
                    dataBase.command.CommandText = $"DELETE FROM lspd_members WHERE login = '{player.SocialClubId}'";
                    dataBase.command.ExecuteNonQuery();
                    dataBase.connection.Close();
                    Members.Remove(member);
                    SwitchPlayersDuty(player, false);
                    return true;
                }
            }
            return false;
        }

        public bool SwitchPlayersDuty(Player player, bool state)
        {
            foreach (LSPD_Member member in Members)
            {
                if (!state)
                {
                    player.SetSharedData("lspd_duty", false);
                    player.SetSharedData("lspd_power", 0);
                    player.SetSharedData("job", "");
                    player.RemoveAllWeapons();
                    playerDataManager.LoadPlayersClothes(player);
                    playerDataManager.NotifyPlayer(player, "Zakończono służbę");
                    return true;
                }
                else
                {
                    if (member.Login == player.SocialClubId)
                    {
                        if (player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty"))
                        {
                            player.SetSharedData("lspd_duty", false);
                            player.SetSharedData("job", "");
                            player.RemoveAllWeapons();
                            playerDataManager.LoadPlayersClothes(player);
                            playerDataManager.NotifyPlayer(player, "Zakończono służbę");
                        }
                        else if(player.HasSharedData("job") && player.GetSharedData<string>("job") == "")
                        {
                            player.SetSharedData("lspd_duty", true);
                            player.SetSharedData("lspd_power", member.Power);
                            player.SetSharedData("job", "lspd");
                            player.GiveWeapon(WeaponHash.Stungun, 1000);
                            player.GiveWeapon(WeaponHash.Flashlight, 1000);
                            Character character = JsonConvert.DeserializeObject<Character>(player.GetSharedData<string>("character").Replace("\\", ""));
                            switch (character.gender)
                            {
                                case 0:
                                    player.SetClothes(3, 0, 0);
                                    player.SetClothes(4, 35, 0);
                                    player.SetClothes(6, 25, 0);
                                    player.SetClothes(8, 58, 0);
                                    player.SetClothes(11, 55, 0);
                                    break;
                                case 1:
                                    player.SetClothes(3, 14, 0);
                                    player.SetClothes(4, 34, 0);
                                    player.SetClothes(6, 25, 0);
                                    player.SetClothes(8, 35, 0);
                                    player.SetClothes(11, 48, 0);
                                    break;
                            }

                            playerDataManager.NotifyPlayer(player, "Rozpoczęto służbę");
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }
        public bool IsPlayerInGroup(Player player)
        {
            foreach(LSPD_Member member in Members)
            {
                if(member.Login == player.SocialClubId)
                {
                    player.SetSharedData("lspd_power", member.Power);
                    player.TriggerEvent("createLSPDMarkers");
                    return true;
                }
            }
            return false;
        }
        public void LoadDataFromDB()
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM lspd_members";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Members.Add(new LSPD_Member(ulong.Parse(reader.GetString(1)), reader.GetInt32(2)));
                }
                reader.Close();
            }

            dataBase.command.CommandText = $"SELECT * FROM lspd_vehicles";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Vehicles.Add(new LSPD_Vehicle(reader.GetInt32(0), uint.Parse(reader.GetString(1)), reader.GetString(2), reader.GetInt32(3), reader.GetString(4)));
                }
                reader.Close();
            }
            dataBase.connection.Close();
        }
        public void CreateBarrier(string name, Vector3 position, Vector3 rotation)
        {
            KeyValuePair<Object, string> barrier = new KeyValuePair<Object, string>(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_mp_barrier_02b"), position, rotation), name);
            Barriers.Add(barrier);
            
            barrier.Key.SetSharedData("barrierId", NAPI.Util.GetHashKey(DateTime.Now.ToString() + DateTime.Now.Millisecond.ToString()));
        }

        public string RemoveBarrier(KeyValuePair<Object, string> bar)
        {
            foreach(KeyValuePair<Object, string> barrier in Barriers)
            {
                if(barrier.Key.Exists && barrier.Equals(bar))
                {
                    string name = barrier.Value;
                    barrier.Key.Delete();
                    Barriers.Remove(barrier);
                    return name;
                }
            }
            return "";
        }

        public void SetPlayerIntoArrest(Player player, int time)
        {
            Arrest playersArrest = GetFreeArrest();
            playersArrest.Inmates.Add(new KeyValuePair<ulong, int>(player.SocialClubId, time));
            player.SetSharedData("arrested", true);
            player.SetSharedData("arrested_time", time);
            player.SetSharedData("arrest_id", Arrests.IndexOf(playersArrest));
            player.Position = playersArrest.Position;
        }

        public bool IsPlayerArrested(Player player)
        {
            foreach (Arrest arrest in Arrests)
            {
                foreach(KeyValuePair<ulong, int> inmate in arrest.Inmates)
                {
                    if(inmate.Key == player.SocialClubId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ReturnPlayerIntoArrest(Player player)
        {
            foreach (Arrest arrest in Arrests)
            {
                bool found = false;
                foreach (KeyValuePair<ulong, int> inmate in arrest.Inmates)
                {
                    if (inmate.Key == player.SocialClubId)
                    {
                        player.SetSharedData("arrested", true);
                        player.SetSharedData("arrested_time", inmate.Value);
                        player.SetSharedData("arrest_id", Arrests.IndexOf(arrest));
                        player.Position = arrest.Position;
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
        }

        public void RemovePlayerFromArrest(Player player)
        {
            player.SetSharedData("arrested", false);
            player.SetSharedData("arrested_time", 0);
            Arrest playersArrest = Arrests[player.GetSharedData<int>("arrest_id")];
            player.SetSharedData("arrest_id", -1);
            playersArrest.Inmates.Remove(new KeyValuePair<ulong, int>(player.SocialClubId, 0));
            player.Position = Position;
            playerDataManager.NotifyPlayer(player, "Twój czas aresztu dobiegł końca!");
            
        }

        public void SpawnPlayerInArrest(Player player)
        {
            Arrest playersArrest = Arrests[player.GetSharedData<int>("arrest_id")];
            player.Position = playersArrest.Position;
        }
        private Arrest GetFreeArrest()
        {
            foreach(Arrest arrest in Arrests)
            {
                if(arrest.Inmates.Count == 0)
                {
                    return arrest;
                }
            }
            Arrest leastInmates = Arrests[0];
            foreach(Arrest arrest in Arrests)
            {
                if(arrest.Inmates.Count < leastInmates.Inmates.Count)
                {
                    leastInmates = arrest;
                }
            }
            return leastInmates;
        }


        //VEHICLES

        public string GetAvailableVehicles(int power)
        {
            List<string[]> vehicles = new List<string[]>();
            foreach (LSPD_Vehicle veh in Vehicles)
            {
                if(!veh.Spawned && veh.Power <= power)
                {
                    vehicles.Add(new string[] {veh.Id.ToString(), veh.Model.ToString(), veh.Name.ToString() });
                }
            }

            if(vehicles.Count > 0)
            {
                return JsonConvert.SerializeObject(vehicles);
            }
            else
            {
                return "";
            }
        }

        List<KeyValuePair<Vector3, float>> SpawnPositions = new List<KeyValuePair<Vector3, float>>()
        {
            new KeyValuePair<Vector3,float>(new Vector3(454.65915f, -1020.06256f, 28.334692f), 89.439125f),
            new KeyValuePair<Vector3,float>(new Vector3(454.7544f, -1014.8806f, 28.434828f), 89.439125f)
        };
        public bool SpawnVehicle(int id)
        {
            foreach(LSPD_Vehicle veh in Vehicles)
            {
                if(veh.Id == id && !veh.Spawned)
                {
                    KeyValuePair<Vector3, float> pos = GetAvailableSpawnPoint();
                    if(!pos.Equals(new KeyValuePair<Vector3, float>()))
                    {
                        veh.Spawned = true;
                        Vehicle vehicle = NAPI.Vehicle.CreateVehicle(veh.Model, pos.Key, pos.Value, 134, 134, numberPlate: "LSPD");
                        vehicle.Rotation = new Vector3(0, 0, pos.Value);
                        SetVehiclesData(vehicle);
                        vehicle.SetSharedData("washtime", DateTime.Now.AddYears(1).ToString());
                        vehicle.SetSharedData("id", veh.Id);
                        vehicle.SetSharedData("name", veh.Name);
                        vehicle.SetSharedData("wheels", veh.Wheels);
                        vehicleDataManager.SetVehiclesWheels(vehicle);

                    return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private void SetVehiclesData(Vehicle vehicle)
        {
            vehicle.SetSharedData("type", "lspd");
            vehicle.SetSharedData("locked", false);
            vehicle.SetSharedData("veh_brake", false);
            vehicle.SetSharedData("damage", vehicleDataManager.defaultDamage);
            int[] pands = vehicleDataManager.GetVehicleStockPowerAndSpeed(vehicle);
            if(!pands.Equals(new int[2]))
            {
                vehicle.SetSharedData("power", pands[0]);
                vehicle.SetSharedData("speed", pands[1]);
            }
            vehicleDataManager.setVehiclesPetrolAndTrunk(vehicle);
            vehicle.SetSharedData("petrol", 0.95 * vehicle.GetSharedData<int>("petroltank"));

        }

        private KeyValuePair<Vector3, float> GetAvailableSpawnPoint()
        {
            foreach (KeyValuePair<Vector3, float> pos in SpawnPositions)
            {
                if (!isAnyVehicleNearPoint(pos.Key))
                {
                    return pos;
                }
            }
            return new KeyValuePair<Vector3, float>();
        }
        private bool isAnyVehicleNearPoint(Vector3 point)
        {
            foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
            {
                if (veh.Position.DistanceTo(point) < 1.5f)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetVehicleSpawned(int vehId, bool state)
        {
            foreach (LSPD_Vehicle vehicle in Vehicles)
            {
                if (vehicle.Id == vehId)
                {
                    vehicle.Spawned = state;
                    break;
                }
            }
        }
        public void SwitchMainGate(bool state)
        {
            float lowPos = 25.9f, highPos = 28.0f;

            if (state)
            {
                leftMainGate.Position = new Vector3(leftMainGate.Position.X, leftMainGate.Position.Y, lowPos);
                rightMainGate.Position = new Vector3(rightMainGate.Position.X, rightMainGate.Position.Y, lowPos);
            }
            else
            {
                leftMainGate.Position = new Vector3(leftMainGate.Position.X, leftMainGate.Position.Y, highPos);
                rightMainGate.Position = new Vector3(rightMainGate.Position.X, rightMainGate.Position.Y, highPos);
            }
        }

        public void SwitchBackGate(bool state)
        {
            float lowPos = 24.84911f, highPos = 26.99018f;

            if (state)
            {
                leftBackGate.Position = new Vector3(leftBackGate.Position.X, leftBackGate.Position.Y, lowPos);
                rightBackGate.Position = new Vector3(rightBackGate.Position.X, rightBackGate.Position.Y, lowPos);
            }
            else
            {
                leftBackGate.Position = new Vector3(leftBackGate.Position.X, leftBackGate.Position.Y, highPos);
                rightBackGate.Position = new Vector3(rightBackGate.Position.X, rightBackGate.Position.Y, highPos);
            }
        }
    }
    
    

    public class LSPD_Member
    {
        public ulong Login { get; set; }
        public int Power { get; set; }

        public LSPD_Member(ulong login, int power)
        {
            Login = login;
            Power = power;
        }
    }

    public class Arrest
    {
        public List<KeyValuePair<ulong, int>> Inmates { get; set; }
        public Vector3 Position { get; set; }

        public Arrest(Vector3 position)
        {
            Position = position;
            Inmates = new List<KeyValuePair<ulong, int>>();
        }
    }

    public class LSPD_Vehicle
    {
        public int Id { get; set; }
        public uint Model { get; set; }
        public string Name { get; set; }
        public int Power { get; set; }
        public bool Spawned { get; set; }
        public string Wheels { get; set; }

        public LSPD_Vehicle(int id, uint model, string name, int power, string wheels)
        {
            Id = id;
            Model = model;
            Name = name;
            Power = power;
            Wheels = wheels;
            Spawned = false;
        }
    }
}

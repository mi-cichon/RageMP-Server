using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class Houses
    {
        public List<House> houses = new List<House>();
        CustomMarkers customMarkers = new CustomMarkers();
        PlayerDataManager playerDataManager = new PlayerDataManager();
        public Houses()
        {
            InstantiateHouses();
        }

        public void InstantiateHouses()
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = "SELECT * FROM houses";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while(reader.Read())
                {
                    ColShape cl = NAPI.ColShape.CreateCylinderColShape(stringToVec(reader.GetString(1)), 1.0f, 2.0f);
                    cl.SetSharedData("type", "house");
                    cl.SetSharedData("id", reader.GetInt32(0));
                    cl.SetSharedData("price", Convert.ToInt32(reader.GetString(3)));
                    cl.SetSharedData("owner", reader.GetString(4));
                    cl.SetSharedData("time", reader.GetString(5));
                    cl.SetSharedData("housepos", stringToVec(reader.GetString(1)));
                    CreateHouseColshapes(cl, reader.GetString(2), stringToVec(reader.GetString(1)), (uint)reader.GetInt32(0), reader.GetString(4), reader.GetString(6));
                }
            }
            dataBase.connection.Close();
        }

        public void CreateHouseColshapes(ColShape house, string interiorstr, Vector3 housepos, uint id, string owner, string storageStr)
        {
            Marker m;
            Blip b;
            Vector3 interior = new Vector3(), storage = new Vector3();
            ColShape inside, houseStorage;
            int[] storageSize = new int[] { };
            switch(interiorstr)
            {
                case "motel":
                    interior = new Vector3(151.37843f, -1007.68475f, -100.000015f);
                    storage = new Vector3(151.54074f, -1003.0848f, -98.999985f);
                    storageSize = new int[] {15, 3};
                    break;

                case "low":
                    interior = new Vector3(266.06122f, -1007.61694f, -102.00855f);
                    storage = new Vector3(259.7117f, -1003.7793f, -99.008575f);
                    storageSize = new int[] { 15, 4 };
                    break;

                case "medium":
                    interior = new Vector3(346.4534f, -1012.5689f, -100.19618f);
                    storage = new Vector3(351.32233f, -993.7749f, -99.19618f);
                    storageSize = new int[] { 15, 5 };
                    break;

                case "high":
                    interior =  new Vector3(-784.98035f, 323.7073f, 210.99724f);
                    storage = new Vector3(-793.38055f, 325.91455f, 210.79665f);
                    storageSize = new int[] { 15, 7 };
                    break;

                case "high2":
                    interior = new Vector3(-1452.2866f, -540.56165f, 73.044334f);
                    storage = new Vector3(-1449.2948f, -548.67084f, 72.843735f);
                    storageSize = new int[] { 15, 7 };
                    break;

                case "high3":
                    interior = new Vector3(-1289.8689f, 449.71527f, 96.90251f);
                    storage = new Vector3(-1286.1342f, 438.15445f, 94.09481f);
                    storageSize = new int[] { 15, 7 };
                    break;

                case "veryhigh":
                    interior = new Vector3(-18.293047f, -591.3661f, 89.11487f);
                    storage = new Vector3(-39.01896f, -583.1879f, 83.91833f);
                    storageSize = new int[] { 15, 8 };
                    break;
            }

            house.SetSharedData("interior", interior);
            inside = NAPI.ColShape.CreateCylinderColShape(interior, 1.0f, 2.0f, id + 500);
            inside.SetSharedData("type", "houseout");
            inside.SetSharedData("housepos", housepos);
            houseStorage = NAPI.ColShape.CreateCylinderColShape(storage, 1.0f, 2.0f, id + 500);
            houseStorage.SetSharedData("type", "housestorage");
            houseStorage.SetSharedData("houseid", id);
            customMarkers.CreateHouseStorageMarker(storage, id);
            NAPI.Blip.CreateBlip(587, storage, 0.7f, 39, name: "Schowek na przedmioty", shortRange: true, dimension: 500 + id);
            customMarkers.CreateHouseMarker(interior, "Wyjście", id);

            //Marker minside4 = NAPI.Marker.CreateMarker(1, interior, new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255), dimension: id + 500);

            if(interior != new Vector3())
            {
                if(house.GetSharedData<string>("owner") == "" || house.GetSharedData<string>("owner") == null)
                {
                    m = customMarkers.CreateHouseMarker(housepos, "Wejście", owned: false);
                    b = NAPI.Blip.CreateBlip(40, housepos, 0.6f, 2, name: "Wolny dom", shortRange: true);
                }
                else
                {
                    m = customMarkers.CreateHouseMarker(housepos, "Wejście", owned: true);
                    b = NAPI.Blip.CreateBlip(40, housepos, 0.6f, 55, name: "Zajęty dom", shortRange: true);
                }
                m.SetSharedData("ownername", playerDataManager.GetPlayerNameById(owner));
                b.SetSharedData("houseid", house.GetSharedData<Int32>("id"));
                houses.Add(new House(house, m, b, owner, Convert.ToInt32(id), storageStr, storageSize));
            }
        }

        public void AddHouse(Vector3 pos, string type, int price)
        {
            int id = 0;
            ColShape cl = NAPI.ColShape.CreateCylinderColShape(pos, 1.0f, 2.0f);
            cl.SetSharedData("type", "house");
            cl.SetSharedData("price", price);
            cl.SetSharedData("owner", "");
            cl.SetSharedData("time", "");
            cl.SetSharedData("housepos", pos);
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"INSERT INTO houses(pos, interior, price, owner, time, storage) VALUES ('{pos.X.ToString() + "." + pos.Y.ToString() + "." + pos.Z.ToString()}', '{type}', '{price.ToString()}', '', '', '')";
            dataBase.command.ExecuteNonQuery();
            dataBase.command.CommandText = $"SELECT * FROM houses WHERE id = (SELECT LAST_INSERT_ID())";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if(reader.Read())
                {
                    id = reader.GetInt32(0);
                }
            }
            cl.SetSharedData("id", id);
            CreateHouseColshapes(cl, type, pos, (uint)id, "", "[]");
            dataBase.connection.Close();
        }

        public Vector3 stringToVec(string str)
        {
            return new Vector3(float.Parse(str.Split('.')[0]), float.Parse(str.Split('.')[1]), float.Parse(str.Split('.')[2]));
        }

        public bool SetPlayersHouse(Player player)
        {
            foreach(House house in houses)
            {
                if(player.SocialClubId.ToString() == house.owner)
                {
                    player.SetSharedData("houseid", house.houseColShape.GetSharedData<Int32>("id"));
                    player.SetSharedData("housepos", house.houseColShape.GetSharedData<Vector3>("housepos"));
                    player.TriggerEvent("setHouseAsOwn", house.houseBlip, house.houseMarker);
                    return true;
                }
            }
            player.SetSharedData("houseid", -1);
            return false;
        }
    }

    public class House
    {
        public string owner;
        public string storage;
        public ColShape houseColShape;
        public Marker houseMarker;
        public Blip houseBlip;
        PlayerDataManager playerDataManager = new PlayerDataManager();
        public int id;
        public int[] storageSize;
        public House(ColShape houseColShape, Marker houseMarker, Blip houseBlip, string owner, int id, string storage, int[] storageSize)
        {
            this.houseColShape = houseColShape;
            this.houseMarker = houseMarker;
            this.houseBlip = houseBlip;
            this.owner = owner;
            this.id = id;
            this.storage = storage;
            this.storageSize = storageSize;
        }
        public void setOwner(Player player, string time)
        {
            owner = player.SocialClubId.ToString();
            houseMarker.Color = new Color(255, 60, 60);
            houseBlip.Color = 55;
            houseBlip.Name = "Zajęty dom";
            player.SetSharedData("houseid", houseColShape.GetSharedData<Int32>("id"));
            houseColShape.SetSharedData("owner", owner.ToString());
            houseColShape.SetSharedData("time", time);
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"UPDATE houses SET owner = '{owner}', time = '{time}' WHERE id = {houseColShape.GetSharedData<Int32>("id")}";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
            houseMarker.SetSharedData("ownername", player.GetSharedData<string>("username"));
            player.TriggerEvent("setHouseAsOwn", houseBlip, houseMarker);
        }

        public void UpdateStorage(string newStorage)
        {
            this.storage = newStorage;
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"UPDATE houses SET storage = '{storage}' WHERE id = {id}";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }

        public void clearOwner()
        {
            houseColShape.SetSharedData("owner", "");
            houseColShape.SetSharedData("time", "");
            houseMarker.Color = new Color(60, 255, 60);
            houseBlip.Color = 2;
            houseBlip.Name = "Wolny dom";
            Player player = playerDataManager.GetPlayerBySocialId(ulong.Parse(owner));
            if (player != null){
                player.SetSharedData("houseid", -1);
                player.TriggerEvent("setHouseAsNotOwn", houseBlip, houseMarker);
            }
            owner = "";
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"UPDATE houses SET owner = '', time = '' WHERE id = {houseColShape.GetSharedData<Int32>("id")}";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
            houseMarker.SetSharedData("ownername", "");

        }

        public void extendTime(string time)
        {
            houseColShape.SetSharedData("time", time);
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"UPDATE houses SET time = '{time}' WHERE id = {houseColShape.GetSharedData<Int32>("id")}";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }
    }
}

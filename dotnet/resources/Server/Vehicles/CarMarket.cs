using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using MySqlConnector;
using System.Linq;

namespace ServerSide
{
    public class CarMarket
    {
        private OrgManager orgManager;
        private VehicleDataManager vehicleDataManager;
        private PlayerDataManager playerDataManager;
        static Dictionary<Vector3, Vector3> marketSpaces = new Dictionary<Vector3, Vector3>()
        {
            [new Vector3(-111.74768f, -1984.1511f, 17.299364f)] = new Vector3(0.0f, 0.0f, 171.32246f),
            [new Vector3(-108.02157f, -1984.8048f, 17.299639f)] = new Vector3(0.0f, 0.0f, 172.33827f),
            [new Vector3(-104.45475f, -1985.1237f, 17.299685f)] = new Vector3(0.0f, 0.0f, 172.03462f),
            [new Vector3(-100.80559f, -1985.6147f, 17.30035f)] = new Vector3(0.0f, 0.0f, 173.01018f),
            [new Vector3(-97.03171f, -1986.0447f, 17.299843f)] = new Vector3(0.0f, 0.0f, 171.31287f),
            [new Vector3(-93.38181f, -1986.6514f, 17.299688f)] = new Vector3(0.0f, 0.0f, 172.27556f),
            [new Vector3(-89.613914f, -1986.9893f, 17.299734f)] = new Vector3(0.0f, 0.0f, 171.711f),
            [new Vector3(-86.06512f, -1987.2463f, 17.29948f)] = new Vector3(0.0f, 0.0f, 172.21065f),
            [new Vector3(-82.31342f, -1987.9755f, 17.300497f)] = new Vector3(0.0f, 0.0f, 172.11339f),
            [new Vector3(-78.684135f, -1988.3263f, 17.29936f)] = new Vector3(0.0f, 0.0f, 172.69174f),
            [new Vector3(-74.91921f, -1988.7253f, 17.299984f)] = new Vector3(0.0f, 0.0f, 172.98955f),
            [new Vector3(-71.342735f, -1989.3452f, 17.300114f)] = new Vector3(0.0f, 0.0f, 173.27644f),
            [new Vector3(-67.61882f, -1989.7432f, 17.299694f)] = new Vector3(0.0f, 0.0f, 172.00171f),
            [new Vector3(-63.757f, -1990.525f, 17.300089f)] = new Vector3(0.0f, 0.0f, 167.76425f),
            [new Vector3(-59.74621f, -1991.527f, 17.300047f)] = new Vector3(0.0f, 0.0f, 159.91367f),
            [new Vector3(-52.42465f, -1996.9814f, 17.300144f)] = new Vector3(0.0f, 0.0f, 123.71243f),
            [new Vector3(-50.760536f, -2001.2263f, 17.300213f)] = new Vector3(0.0f, 0.0f, 109.37992f),
            [new Vector3(-49.652515f, -2004.6785f, 17.300213f)] = new Vector3(0.0f, 0.0f, 108.73302f),
            [new Vector3(-48.331234f, -2008.17f, 17.300186f)] = new Vector3(0.0f, 0.0f, 108.48951f),
            [new Vector3(-47.09941f, -2011.726f, 17.300322f)] = new Vector3(0.0f, 0.0f, 108.669395f),
            [new Vector3(-50.587734f, -2020.0997f, 17.300222f)] = new Vector3(0.0f, 0.0f, 19.232994f),
            [new Vector3(-53.962013f, -2021.2288f, 17.299824f)] = new Vector3(0.0f, 0.0f, 19.656414f),
            [new Vector3(-57.425247f, -2022.645f, 17.300735f)] = new Vector3(0.0f, 0.0f, 19.01801f),
            [new Vector3(-60.942707f, -2023.9042f, 17.300095f)] = new Vector3(0.0f, 0.0f, 19.587797f),
            [new Vector3(-64.4898f, -2024.9025f, 17.299843f)] = new Vector3(0.0f, 0.0f, 19.033564f),
            [new Vector3(-67.92286f, -2026.3235f, 17.300028f)] = new Vector3(0.0f, 0.0f, 19.396591f),
            [new Vector3(-71.393936f, -2027.4412f, 17.300138f)] = new Vector3(0.0f, 0.0f, 20.366938f),
            [new Vector3(-77.09738f, -2027.9321f, 17.29989f)] = new Vector3(0.0f, 0.0f, -8.446811f),
            [new Vector3(-80.76057f, -2027.4791f, 17.29975f)] = new Vector3(0.0f, 0.0f, -7.7451687f),
            [new Vector3(-84.42027f, -2026.9358f, 17.299799f)] = new Vector3(0.0f, 0.0f, -7.2502933f),
            [new Vector3(-88.09247f, -2026.4753f, 17.299747f)] = new Vector3(0.0f, 0.0f, -7.3600254f),
            [new Vector3(-91.79042f, -2025.9973f, 17.299671f)] = new Vector3(0.0f, 0.0f, -8.151579f),
            [new Vector3(-95.42479f, -2025.591f, 17.299994f)] = new Vector3(0.0f, 0.0f, -7.8606157f),
            [new Vector3(-99.1304f, -2025.263f, 17.29998f)] = new Vector3(0.0f, 0.0f, -7.8654943f),
            [new Vector3(-102.85416f, -2024.7289f, 17.299994f)] = new Vector3(0.0f, 0.0f, -8.163139f),
            [new Vector3(-100.16766f, -2001.8302f, 17.299858f)] = new Vector3(0.0f, 0.0f, -7.847099f),
            [new Vector3(-96.32694f, -2002.1558f, 17.299772f)] = new Vector3(0.0f, 0.0f, -8.340074f),
            [new Vector3(-92.635376f, -2002.8641f, 17.300182f)] = new Vector3(0.0f, 0.0f, -7.8247385f),
            [new Vector3(-88.73853f, -2003.2227f, 17.300125f)] = new Vector3(0.0f, 0.0f, -7.6013656f),
            [new Vector3(-84.97688f, -2003.7808f, 17.300201f)] = new Vector3(0.0f, 0.0f, -7.026321f),
            [new Vector3(-81.34172f, -2004.2069f, 17.300394f)] = new Vector3(0.0f, 0.0f, -7.730642f),
            [new Vector3(-77.523094f, -2004.5167f, 17.299652f)] = new Vector3(0.0f, 0.0f, -8.030596f),
            [new Vector3(-64.07285f, -2007.853f, 17.300255f)] = new Vector3(0.0f, 0.0f, -160.13278f),
            [new Vector3(-67.181145f, -2009.19f, 17.300245f)] = new Vector3(0.0f, 0.0f, -160.4254f),
            [new Vector3(-70.19462f, -2010.3855f, 17.300346f)] = new Vector3(0.0f, 0.0f, -160.50215f),
            [new Vector3(-73.0479f, -2011.5142f, 17.300198f)] = new Vector3(0.0f, 0.0f, -159.68474f),
            [new Vector3(-78.389694f, -2011.8806f, 17.299822f)] = new Vector3(0.0f, 0.0f, 172.27466f),
            [new Vector3(-83.290085f, -2013.007f, 17.299152f)] = new Vector3(0.0f, 0.0f, 152.29092f),
            [new Vector3(-82.11317f, -2011.4438f, 17.299726f)] = new Vector3(0.0f, 0.0f, 171.91855f),
            [new Vector3(-85.88326f, -2010.8486f, 17.300085f)] = new Vector3(0.0f, 0.0f, 172.18645f),
            [new Vector3(-89.76057f, -2010.3999f, 17.299551f)] = new Vector3(0.0f, 0.0f, 172.64517f),
            [new Vector3(-93.4451f, -2009.8171f, 17.300121f)] = new Vector3(0.0f, 0.0f, 172.20331f),
            [new Vector3(-97.20616f, -2009.3722f, 17.300383f)] = new Vector3(0.0f, 0.0f, 171.88817f),
            [new Vector3(-100.96196f, -2008.8458f, 17.299793f)] = new Vector3(0.0f, 0.0f, 172.56445f)

        };


        Vehicle[] marketVehicles = new Vehicle[marketSpaces.Count];
        public CarMarket(Vector3 colshapePosition, ref OrgManager orgManager, ref VehicleDataManager vehicleDataManager, ref PlayerDataManager playerDataManager)
        {
            ColShape colshape = NAPI.ColShape.CreateCylinderColShape(colshapePosition - new Vector3(0, 0, 1), 2.0f, 2.0f);
            colshape.SetSharedData("type", "market1");
            NAPI.Marker.CreateMarker(25, colshapePosition - new Vector3(0, 0, 0.8), new Vector3(), new Vector3(), 5.0f, new Color(0, 204, 153));
            NAPI.Blip.CreateBlip(380, colshapePosition, 0.8f, 75, name: "Giełda pojazdów", shortRange: true);
            NAPI.TextLabel.CreateTextLabel("Wystaw pojazd na giełdę", colshapePosition + new Vector3(0,0,1.5), 15.0f, 2.0f, 4, new Color(255, 255, 255));
            this.orgManager = orgManager;
            this.vehicleDataManager = vehicleDataManager;
            this.playerDataManager = playerDataManager;
        }



        public void CreateMarketVehicles()
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM carmarket RIGHT OUTER JOIN vehicles ON carmarket.carId = vehicles.id WHERE carmarket.price != 0";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int price = reader.GetInt32(2);
                    string description = reader.GetString(3);
                    KeyValuePair<Vector3, Vector3> vehiclePos = marketSpaces.ElementAt(id - 1);
                    marketVehicles[id-1] = NAPI.Vehicle.CreateVehicle(Convert.ToUInt32(reader.GetString(6)), vehiclePos.Key, 0, 0, 0, numberPlate: "B " + reader.GetInt32(4).ToString());
                    marketVehicles[id - 1].Rotation = vehiclePos.Value;
                    Vehicle vehicle = marketVehicles[id - 1];
                    marketVehicles[id - 1].SetSharedData("invincible", true);
                    marketVehicles[id - 1].SetSharedData("type", "personal");
                    marketVehicles[id - 1].SetSharedData("id", reader.GetInt32(4));
                    marketVehicles[id - 1].SetSharedData("owner", ulong.Parse(reader.GetString(5)));
                    marketVehicles[id - 1].SetSharedData("model", reader.GetString(6));
                    marketVehicles[id - 1].SetSharedData("name", reader.GetString(7));
                    marketVehicles[id - 1].SetSharedData("color1", reader.GetString(8));
                    marketVehicles[id - 1].SetSharedData("color1mod", vehicleDataManager.JsonToColorMod(reader.GetString(8)));
                    marketVehicles[id - 1].SetSharedData("color2", reader.GetString(9));
                    marketVehicles[id - 1].SetSharedData("color2mod", vehicleDataManager.JsonToColorMod(reader.GetString(9)));
                    marketVehicles[id - 1].SetSharedData("spawned", true);
                    marketVehicles[id - 1].SetSharedData("lastpos", vehiclePos.Key);
                    marketVehicles[id - 1].SetSharedData("lastrot", vehiclePos.Value);
                    marketVehicles[id - 1].SetSharedData("damage", reader.GetString(13));
                    marketVehicles[id - 1].SetSharedData("used", reader.GetString(14));
                    marketVehicles[id - 1].SetSharedData("tune", reader.GetString(15));
                    marketVehicles[id - 1].SetSharedData("petrol", float.Parse(reader.GetString(16)));
                    marketVehicles[id - 1].SetSharedData("speedometer", reader.GetString(17));
                    marketVehicles[id - 1].SetSharedData("towed", false);
                    marketVehicles[id - 1].SetSharedData("locked", false);
                    int[] PandS = vehicleDataManager.GetVehicleStockPowerAndSpeed(marketVehicles[id - 1]);
                    marketVehicles[id - 1].SetSharedData("power", PandS[0]);
                    marketVehicles[id - 1].SetSharedData("speed", PandS[1]);
                    marketVehicles[id - 1].SetSharedData("dirt", reader.GetInt32(18));
                    marketVehicles[id - 1].SetSharedData("washtime", reader.GetString(19));
                    marketVehicles[id - 1].SetSharedData("trunk", reader.GetString(20));
                    marketVehicles[id - 1].SetSharedData("mechtune", reader.GetString(21));
                    marketVehicles[id - 1].SetSharedData("wheels", reader.GetString(22));
                    marketVehicles[id - 1].SetSharedData("drivers", reader.GetString(23));
                    bool brake = bool.Parse(reader.GetString(24));
                    marketVehicles[id - 1].SetSharedData("veh_trip", reader.GetFloat(25));
                    int[] color1 = vehicleDataManager.JsonToColor(reader.GetString(8));
                    int[] color2 = vehicleDataManager.JsonToColor(reader.GetString(9));
                    NAPI.Vehicle.SetVehicleCustomPrimaryColor(marketVehicles[id - 1].Handle, color1[0], color1[1], color1[2]);
                    NAPI.Vehicle.SetVehicleCustomSecondaryColor(marketVehicles[id - 1].Handle, color2[0], color2[1], color2[2]);

                    marketVehicles[id - 1].SetSharedData("veh_engine", false);
                    marketVehicles[id - 1].SetSharedData("veh_lights", false);
                    marketVehicles[id - 1].SetSharedData("veh_locked", false);

                    vehicleDataManager.setVehiclesPetrolAndTrunk(marketVehicles[id - 1]);

                    NAPI.Task.Run(() =>
                    {
                        if (vehicle != null && vehicle.Exists)
                        {
                            vehicleDataManager.SetVehiclesWheels(marketVehicles[id - 1]);
                            vehicleDataManager.applyTuneToVehicle(marketVehicles[id - 1], marketVehicles[id - 1].GetSharedData<string>("tune"), marketVehicles[id - 1].GetSharedData<string>("mechtune"));
                            vehicleDataManager.SetVehiclesExtra(marketVehicles[id - 1]);
                            marketVehicles[id - 1].SetSharedData("veh_brake", brake);
                        }

                    }, 1000);

                    marketVehicles[id - 1].SetSharedData("market", true);
                    marketVehicles[id - 1].SetSharedData("marketprice", price);
                    marketVehicles[id - 1].SetSharedData("marketdescription", description);
                    marketVehicles[id - 1].SetSharedData("marketowner", playerDataManager.GetPlayerNameById(marketVehicles[id - 1].GetSharedData<Int64>("owner").ToString()));

                    marketVehicles[id - 1].SetSharedData("markettune", vehicleDataManager.GetVehiclesTuneString(marketVehicles[id - 1]));
                    orgManager.SetVehiclesOrg(marketVehicles[id - 1]);
                }
            }
            dataBase.connection.Close();
        }   
        public bool AddVehicleToMarket(Vehicle vehicle, int price, string description, string ownerName)
        {
            KeyValuePair<Vector3, Vector3> freeSpace = new KeyValuePair<Vector3, Vector3>();
            int freeIndex = -1;
            for(int i = 0; i < marketVehicles.Length; i++)
            {
                if(marketVehicles[i] == null)
                {
                    freeSpace = marketSpaces.ElementAt(i);
                    freeIndex = i;
                    break;
                }
            }
            if(freeIndex == -1)
            {
                return false;
            }
            else
            {
                marketVehicles[freeIndex] = vehicle;
                vehicleDataManager.SetVehicleAsMarket(vehicle, true);
                marketVehicles[freeIndex].SetSharedData("invincible", true);
                marketVehicles[freeIndex].SetSharedData("veh_brake", true);
                marketVehicles[freeIndex].SetSharedData("marketprice", price);
                marketVehicles[freeIndex].SetSharedData("marketdescription", description);
                marketVehicles[freeIndex].SetSharedData("marketowner", ownerName);
                marketVehicles[freeIndex].SetSharedData("markettune", vehicleDataManager.GetVehiclesTuneString(marketVehicles[freeIndex]));
                vehicle.SetSharedData("lastpos", freeSpace.Key);
                vehicle.SetSharedData("lastrot", freeSpace.Value);
                vehicleDataManager.UpdateVehiclesLastPos(vehicle);
                foreach (Player occupant in NAPI.Pools.GetAllPlayers())
                {
                    if(occupant.Vehicle == vehicle)
                        occupant.WarpOutOfVehicle();
                }
                clearMarketSpace(freeSpace.Key);
                NAPI.Task.Run(() =>
                {
                    vehicle.SetSharedData("lastpos", freeSpace.Key);
                    vehicle.SetSharedData("lastrot", freeSpace.Value);
                    vehicleDataManager.SavePersonalVehicleDataToDB(vehicle, "lastpos");
                    marketVehicles[freeIndex].Position = freeSpace.Key;
                    marketVehicles[freeIndex].Rotation = freeSpace.Value;
                    DBConnection dataBase = new DBConnection();
                    dataBase.command.CommandText = $"UPDATE carmarket SET carId = {marketVehicles[freeIndex].GetSharedData<Int32>("id")}, price = {price}, description = '{description}' WHERE id = {(freeIndex + 1).ToString()}";
                    dataBase.command.ExecuteNonQuery();
                    dataBase.connection.Close();
                }, 1000);
                return true;
            }
        }
        public void RemoveVehicleFromMarket(Vehicle vehicle)
        {
            for (int i = 0; i < marketVehicles.Length; i++)
            {
                if (marketVehicles[i] == vehicle)
                {
                    marketVehicles[i] = null;
                    vehicleDataManager.SetVehicleAsMarket(vehicle, false);
                    vehicle.ResetSharedData("marketprice");
                    vehicle.ResetSharedData("marketdescription");
                    DBConnection dataBase = new DBConnection();
                    dataBase.command.CommandText = $"UPDATE carmarket SET carId = 0, price = 0, description = '' WHERE id = {(i + 1).ToString()}";
                    dataBase.command.ExecuteNonQuery();
                    dataBase.connection.Close();
                    break;
                }
            }
        }

        public void clearMarketSpace(Vector3 spacePosition)
        {
            foreach(Vehicle vehicle in NAPI.Pools.GetAllVehicles())
            {
                if(vehicle.Position.DistanceTo(spacePosition) < 5.0f)
                {
                    if ((vehicle.HasSharedData("market") && !vehicle.GetSharedData<bool>("market")) || (!vehicle.HasSharedData("market") && vehicle.HasSharedData("owner")))
                    {
                        vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                        vehicle.Delete();
                    }
                    else if (!vehicle.HasSharedData("market"))
                    {
                        vehicle.Delete();
                    }
                }
            }
        }
    }
}

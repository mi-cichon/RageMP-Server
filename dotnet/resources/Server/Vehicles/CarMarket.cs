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
        private VehicleDataManager vehicleDataManager = new VehicleDataManager();
        private PlayerDataManager playerDataManager = new PlayerDataManager();
        static Dictionary<Vector3, Vector3> marketSpaces = new Dictionary<Vector3, Vector3>()
        {
            [new Vector3(-1660.3975f, 75.768585f, 63.096954f)] = new Vector3(-2.5515187f, -0.024857108f, 171.60942f),
            [new Vector3(-1663.4309f, 77.66963f, 63.18264f)] = new Vector3(-1.6698343f, -1.454717f, 171.82664f),
            [new Vector3(-1666.3972f, 79.54597f, 63.34355f)] = new Vector3(-1.8131912f, -2.102983f, 170.94264f),
            [new Vector3(-1669.3682f, 81.459724f, 63.507935f)] = new Vector3(-1.8272717f, -1.8946568f, 171.14427f),
            [new Vector3(-1672.4214f, 83.41394f, 63.676113f)] = new Vector3(-2.2614527f, -1.9438019f, 171.91887f),
            [new Vector3(-1662.5223f, 58.05098f, 62.762398f)] = new Vector3(-2.0348525f, 2.0119479f, -67.66503f),
            [new Vector3(-1665.1193f, 60.496086f, 62.92683f)] = new Vector3(-2.0160308f, 1.9336531f, -67.00597f),
            [new Vector3(-1667.8501f, 62.90902f, 63.093815f)] = new Vector3(-2.0311034f, 1.8381752f, -67.923645f),
            [new Vector3(-1670.4642f, 65.33553f, 63.25178f)] = new Vector3(-2.043394f, 1.8712406f, -67.38274f),
            [new Vector3(-1676.9515f, 71.011024f, 63.637695f)] = new Vector3(-2.0429354f, 1.8291256f, -67.60504f),
            [new Vector3(-1679.6012f, 73.44213f, 63.79656f)] = new Vector3(-2.1180103f, 1.760149f, -67.99475f),
            [new Vector3(-1682.2053f, 75.91569f, 63.956635f)] = new Vector3(-2.206373f, 1.8140001f, -67.89762f),
            [new Vector3(-1684.8925f, 78.300835f, 64.12595f)] = new Vector3(-2.3799796f, 1.9120064f, -67.6262f),
            [new Vector3(-1674.5236f, 39.703323f, 62.953056f)] = new Vector3(0.958325f, -3.335317f, 162.84875f),
            [new Vector3(-1677.1959f, 42.17818f, 63.13934f)] = new Vector3(0.9105963f, -3.6978416f, 162.95093f),
            [new Vector3(-1679.9701f, 44.377537f, 63.337543f)] = new Vector3(0.9731165f, -3.9298391f, 162.64865f),
            [new Vector3(-1682.4164f, 47.008507f, 63.516014f)] = new Vector3(1.0448257f, -3.8583288f, 162.8877f),
            [new Vector3(-1685.0686f, 49.466442f, 63.70723f)] = new Vector3(1.1529713f, -3.8833401f, 163.00932f),
            [new Vector3(-1691.3937f, 55.30671f, 64.170906f)] = new Vector3(1.6439137f, -4.0667996f, 163.02744f),
            [new Vector3(-1693.9268f, 57.759514f, 64.34481f)] = new Vector3(1.8327662f, -4.2021246f, 163.14223f),
            [new Vector3(-1696.6824f, 60.047867f, 64.550255f)] = new Vector3(2.123845f, -4.5051794f, 161.71445f),
            [new Vector3(-1699.3699f, 62.493984f, 64.75054f)] = new Vector3(2.094574f, -4.5549498f, 163.84482f),
            [new Vector3(-1701.905f, 64.922966f, 64.94242f)] = new Vector3(2.1455429f, -4.7578526f, 162.86208f),
            [new Vector3(-1687.0161f, 32.01877f, 64.0249f)] = new Vector3(-5.5452456f, 0.6742893f, -69.734f),
            [new Vector3(-1689.538f, 34.517f, 64.204315f)] = new Vector3(-5.521828f, 0.42666802f, -69.73653f),
            [new Vector3(-1692.203f, 37.062244f, 64.373436f)] = new Vector3(-5.253664f, 0.19860145f, -69.79381f),
            [new Vector3(-1694.7937f, 39.623394f, 64.556114f)] = new Vector3(-5.0705743f, 1.2627268f, -69.37615f),
            [new Vector3(-1697.3141f, 42.141502f, 64.76075f)] = new Vector3(-5.096835f, 1.2104542f, -69.87135f),
            [new Vector3(-1699.772f, 44.606174f, 64.95754f)] = new Vector3(-5.059108f, 1.2074596f, -69.77242f),
            [new Vector3(-1702.3812f, 47.10533f, 65.1717f)] = new Vector3(-4.988152f, 1.2833557f, -69.883194f),
            [new Vector3(-1704.9132f, 49.70311f, 65.377495f)] = new Vector3(-4.9087043f, 1.384987f, -69.77913f),
            [new Vector3(-1707.5194f, 52.222908f, 65.586044f)] = new Vector3(-4.805593f, 1.1978905f, -70.61577f),
            [new Vector3(-1709.9949f, 54.639362f, 65.76976f)] = new Vector3(-4.494418f, 1.0467129f, -69.30628f),
            [new Vector3(-1712.5532f, 57.26692f, 65.962524f)] = new Vector3(-4.6607614f, 0.9294835f, -68.668205f),
            [new Vector3(-1715.1489f, 59.80899f, 66.13056f)] = new Vector3(-4.2401f, 0.44166258f, -68.85777f),
            [new Vector3(-1717.6793f, 62.263546f, 66.30142f)] = new Vector3(-3.3859067f, 1.6522483f, -70.524475f)
        };


        Vehicle[] marketVehicles = new Vehicle[marketSpaces.Count];
        public CarMarket(Vector3 colshapePosition, ref OrgManager orgManager)
        {
            ColShape colshape = NAPI.ColShape.CreateCylinderColShape(colshapePosition - new Vector3(0, 0, 1), 2.0f, 2.0f);
            colshape.SetSharedData("type", "market1");
            NAPI.Marker.CreateMarker(25, colshapePosition - new Vector3(0, 0, 0.8), new Vector3(), new Vector3(), 5.0f, new Color(255, 128, 0));
            NAPI.Blip.CreateBlip(380, colshapePosition, 1.0f, 45, name: "Giełda pojazdów", shortRange: true);
            NAPI.TextLabel.CreateTextLabel("Wystaw pojazd na giełdę", colshapePosition + new Vector3(0,0,1.5), 15.0f, 2.0f, 4, new Color(255, 255, 255));
            this.orgManager = orgManager;
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
                    marketVehicles[id - 1].SetSharedData("parkingbrake", true);
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

                    int[] color1 = vehicleDataManager.JsonToColor(reader.GetString(8));
                    int[] color2 = vehicleDataManager.JsonToColor(reader.GetString(9));
                    NAPI.Vehicle.SetVehicleCustomPrimaryColor(marketVehicles[id - 1].Handle, color1[0], color1[1], color1[2]);
                    NAPI.Vehicle.SetVehicleCustomSecondaryColor(marketVehicles[id - 1].Handle, color2[0], color2[1], color2[2]);
                    vehicleDataManager.applyTuneToVehicle(marketVehicles[id - 1], marketVehicles[id - 1].GetSharedData<string>("tune"), marketVehicles[id - 1].GetSharedData<string>("mechtune"));
                    vehicleDataManager.setVehiclesPetrolAndTrunk(marketVehicles[id - 1]);

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
                marketVehicles[freeIndex].SetSharedData("marketprice", price);
                marketVehicles[freeIndex].SetSharedData("marketdescription", description);
                marketVehicles[freeIndex].SetSharedData("marketowner", ownerName);
                marketVehicles[freeIndex].SetSharedData("markettune", vehicleDataManager.GetVehiclesTuneString(marketVehicles[freeIndex]));
                foreach (Player occupant in NAPI.Pools.GetAllPlayers())
                {
                    if(occupant.Vehicle == vehicle)
                        occupant.WarpOutOfVehicle();
                }
                clearMarketSpace(freeSpace.Key);
                vehicle.SetSharedData("lastpos", freeSpace.Key);
                vehicle.SetSharedData("lastrot", freeSpace.Value);
                marketVehicles[freeIndex].Position = freeSpace.Key;
                marketVehicles[freeIndex].Rotation = freeSpace.Value;
                vehicleDataManager.SetVehicleAsMarket(vehicle, true);
                marketVehicles[freeIndex].SetSharedData("invincible", true);
                marketVehicles[freeIndex].SetSharedData("veh_brake", true);
                DBConnection dataBase = new DBConnection();
                dataBase.command.CommandText = $"UPDATE carmarket SET carId = {marketVehicles[freeIndex].GetSharedData<Int32>("id")}, price = {price}, description = '{description}' WHERE id = {(freeIndex + 1).ToString()}";
                dataBase.command.ExecuteNonQuery();
                dataBase.connection.Close();
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

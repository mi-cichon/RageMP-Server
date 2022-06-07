using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using MySqlConnector;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace ServerSide
{
    public class TuneBusiness
    {
        public Vector3 StartPos { get; set; }
        public int Id { get; set; }
        public GTANetworkAPI.Object Gate { get; set; }
        public ColShape StartShape { get; set; }
        public ColShape WheelsStation { get; set; }
        public ColShape VehColshape { get; set; }
        public ColShape MechStation { get; set; }
        public ColShape CenterShape { get; set; }
        public Blip StartBlip { get; set; }
        public CustomMarker StartMarker { get; set; }

        TextLabel mechLabel;

        public ulong Owner { get; set; }
        public string OwnerName { get; set; }
        public DateTime PaidTo { get; set; }
        public List<int[]> AvailableWheels { get; set; }
        public List<KeyValuePair<int[], DateTime>> WheelOrders { get; set; }

        public Vehicle StationVeh { get; set; }

        Vector3 mechText;
        public TuneBusiness(int id, Vector3 startPosition, Vector3 vehPosition, Vector3 wheelsStation, Vector3 mechStation, Vector3 centerPosition)
        {
            Id = id;
            StartPos = startPosition;
            StartShape = NAPI.ColShape.CreateCylinderColShape(startPosition, 1.5f, 2.0f);
            StartShape.SetSharedData("type", "business-tune");
            StartShape.SetSharedData("bus-id", Id);
            StartMarker = CustomMarkers.CreateBusinessMarker(startPosition, "Biznes: Tuner", "");
            StartBlip = NAPI.Blip.CreateBlip(490, startPosition, 0.8f, 22, name: "Biznes: Tuner (wolny)", shortRange: true);


            WheelsStation = NAPI.ColShape.CreateCylinderColShape(wheelsStation, 1.0f, 2.0f);
            WheelsStation.SetSharedData("type", "business-wheels-station");
            WheelsStation.SetSharedData("business-id", Id);

            VehColshape = NAPI.ColShape.CreateCylinderColShape(vehPosition, 2.0f, 2.0f);
            VehColshape.SetSharedData("type", "business-veh");
            VehColshape.SetSharedData("business-id", Id);

            MechStation = NAPI.ColShape.CreateCylinderColShape(mechStation, 1.0f, 2.0f);
            MechStation.SetSharedData("type", "business-mech-station");
            MechStation.SetSharedData("business-id", Id);

            CenterShape = NAPI.ColShape.CreateCylinderColShape(centerPosition, 15.0f, 4.0f);
            CenterShape.SetSharedData("type", "business-tune-center");
            CenterShape.SetSharedData("business-id", Id);

            mechText = vehPosition;

            LoadBusinessFromDB();
        }

        public void LoadBusinessFromDB()
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM `business_tune` WHERE id = {Id}";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetString(1) != "")
                    {
                        Owner = ulong.Parse(reader.GetString(1));
                        PaidTo = DateTime.ParseExact(reader.GetString(2), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        AvailableWheels = JsonConvert.DeserializeObject<List<int[]>>(reader.GetString(3));
                        WheelOrders = JsonConvert.DeserializeObject<List<KeyValuePair<int[], DateTime>>>(reader.GetString(4), new JsonSerializerSettings
                        {
                            DateFormatString = "dd.MM.yyyy HH:mm:ss"
                        });
                        SetData(Owner);
                    }
                    else
                    {
                        Owner = 0;
                    }
                }
            }
            dataBase.connection.Close();
        }

        public void SetData(ulong ownerId)
        {
            if (Owner != 0)
            {
                DBConnection dataBase = new DBConnection();
                dataBase.command.CommandText = $"SELECT username FROM `users` WHERE login = '{ownerId}'";
                using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OwnerName = reader.GetString(0);
                    }
                }
                dataBase.connection.Close();
                StartMarker.Delete();
                StartMarker = CustomMarkers.CreateBusinessMarker(StartPos, "Biznes: Tuner", OwnerName);
                StartBlip.Color = 6;
                StartBlip.Name = $"Biznes: Tuner ({OwnerName})";
            }
            else
            {
                StartMarker.Delete();
                StartMarker = CustomMarkers.CreateBusinessMarker(StartPos, "Biznes: Tuner", "");
                StartBlip.Color = 22;
                StartBlip.Name = $"Biznes: Tuner (wolny)";
            }
        }

        public void SetNewOwner(ulong ownerId)
        {
            Owner = ownerId;
            PaidTo = DateTime.Now.AddDays(1);
            SetData(ownerId);
            SaveBusinessToDB();
        }

        public void ResetOwner()
        {
            Owner = 0;
            PaidTo = DateTime.Now;
            StartMarker.Delete();
            StartMarker = CustomMarkers.CreateBusinessMarker(StartPos, "Biznes: Tuner", "");
            StartBlip.Color = 22;
            StartBlip.Name = $"Biznes: Tuner (wolny)";
            SaveBusinessToDB();
        }

        public void SetOwnerWorking(bool state)
        {
            if (state)
            {
                StartBlip.Color = 15;
                CreateTextLabel();
            }
            else
            {
                StartBlip.Color = 6;
                DeleteTextLabel();
            }
        }

        public void IncreasePayTime(int days)
        {
            PaidTo = PaidTo.AddDays(days);
            SaveBusinessToDB();
        }

        public void ClearBusiness()
        {
            Owner = 0;
            OwnerName = "";

        }

        public void SaveBusinessToDB()
        {
            string paid = PaidTo == null ? "" : PaidTo.ToString(), wheels = JsonConvert.SerializeObject(AvailableWheels), orders = JsonConvert.SerializeObject(WheelOrders, new IsoDateTimeConverter { DateTimeFormat = "dd.MM.yyyy HH:mm:ss" });
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"UPDATE `business_tune` SET owner = '{Owner}', paidto = '{paid}', wheels = '{wheels}', wheelorders = '{orders}' WHERE id = {Id}";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }

        private void CreateTextLabel()
        {
            mechLabel = NAPI.TextLabel.CreateTextLabel("Tuning mechaniczny i montaż felg", mechText, 6.0f, 2.0f, 4, new Color(255, 255, 255), dimension: 0, entitySeethrough: true);
        }

        private void DeleteTextLabel()
        {
            if(mechLabel.Exists)
            {
                mechLabel.Delete();
            }
        }
    }
}

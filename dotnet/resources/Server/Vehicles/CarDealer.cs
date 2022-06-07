using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using System.Xml;
using System.IO;
using System.Linq;

namespace ServerSide
{
    public class CarDealer
    {
        public Vector3 position;
        private float rotation;
        List<CustomVehicle> cars;
        public CustomVehicle customVehicle;
        public string type;
        public Vehicle vehicle;
        TextLabel nameLabel;
        TextLabel priceLabel;
        public ColShape colShape;
        public DateTime rollTime;
        int[] probability = new int[] { 1, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };
        const int blipColor = 28;
        int[] tripExtr;

        private List<Vector3> colors = new List<Vector3>()
        {
            new Vector3(251, 226, 18),
            new Vector3(17, 37, 82),
            new Vector3(222, 15, 24),
            new Vector3(153, 157, 160),
            new Vector3(50, 56, 61),
            new Vector3(255, 255, 251)
        };
        public CarDealer(Vector3 position, float rotation, string type, bool blip, int[] tripExt)
        {
            this.position = new Vector3(position.X, position.Y, position.Z - 0.5f);
            this.rotation = rotation;
            this.type = type;
            this.tripExtr = tripExt;
            getVehiclesOfType(type);
            if(blip)
            {
                createBlip(type);
            }
            rollTime = DateTime.Now.AddMinutes(30);
            Create();
        }

        public void Create()
        {
            Random rnd = new Random();
            int prob = probability[rnd.Next(0, probability.Length)];
            List<CustomVehicle> probVehs = CustomVehicles.GetAllVehiclesOfProb(cars, prob);
            while(probVehs.Count == 0)
            {
                prob = probability[rnd.Next(0, probability.Length)];
                probVehs = CustomVehicles.GetAllVehiclesOfProb(cars, prob);
            }
            customVehicle = probVehs[rnd.Next(0, probVehs.Count)];

            vehicle = NAPI.Vehicle.CreateVehicle((uint)customVehicle.model, position, rotation, 55, 55, numberPlate: "BASICRPG", locked: false, engine: false);
            var color = colors[rnd.Next(0, colors.Count)];
            NAPI.Vehicle.SetVehicleCustomPrimaryColor(vehicle.Handle, (int)color.X, (int)color.Y, (int)color.Z);
            NAPI.Vehicle.SetVehicleCustomSecondaryColor(vehicle.Handle, (int)color.X, (int)color.Y, (int)color.Z);
            vehicle.SetSharedData("dealerposition", position);
            vehicle.SetSharedData("veh_brake", true);
            vehicle.SetSharedData("type", "dealer");
            vehicle.SetSharedData("name", customVehicle.name);
            vehicle.SetSharedData("veh_trip", (float)rnd.Next(tripExtr[0], tripExtr[1] + 1));
            if (nameLabel != null && nameLabel.Exists)
            {
                nameLabel.Text = customVehicle.name;
            }
            else
            {
                nameLabel = NAPI.TextLabel.CreateTextLabel(customVehicle.name, new Vector3(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z + 2.0), 7.0f, 0.7f, 4, new Color(123, 179, 46, 255), entitySeethrough: false);
            }
            priceLabel = NAPI.TextLabel.CreateTextLabel("$" + customVehicle.price.ToString(), new Vector3(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z + 1.8), 7.0f, 0.7f, 4, new Color(255, 153, 0, 255), entitySeethrough: false);
            colShape = NAPI.ColShape.CreateCylinderColShape(vehicle.Position, 2.4f, 3.0f);
            colShape.SetSharedData("type", "cardealer");
            VehicleDataManager.setVehiclesPetrolAndTrunk(vehicle);
            VehicleDataManager.SetVehiclesExtra(vehicle);


        }

        public void createBlip(string type)
        {
            int blipType;
            switch (type)
            {
                case "junk":
                    blipType = 561;
                    NAPI.Blip.CreateBlip(blipType, position, 0.8f, blipColor, name: "Komis gruzów", shortRange: true);
                    break;
                case "regular":
                    blipType = 225;
                    NAPI.Blip.CreateBlip(blipType, position, 0.8f, blipColor, name: "Komis pojazdów osobowych", shortRange: true);
                    break;
                case "sport":
                    blipType = 596;
                    NAPI.Blip.CreateBlip(blipType, position, 0.8f, blipColor, name: "Komis pojazdów sportowych", shortRange: true);
                    break;
                case "offroad":
                    blipType = 734;
                    NAPI.Blip.CreateBlip(blipType, position, 0.8f, blipColor, name: "Komis pojazdów terenowych", shortRange: true);
                    break;
                case "hyper":
                    blipType = 595;
                    NAPI.Blip.CreateBlip(blipType, position, 0.8f, blipColor, name: "Komis supersamochodów", shortRange: true);
                    break;
                case "suv":
                    blipType = 532;
                    NAPI.Blip.CreateBlip(blipType, position, 0.8f, blipColor, name: "Komis SUV'ów", shortRange: true);
                    break;
                case "bike":
                    blipType = 522;
                    NAPI.Blip.CreateBlip(blipType, position, 0.8f, blipColor, name: "Salon motocykli", shortRange: true);
                    break;
                case "classic":
                    blipType = 664;
                    NAPI.Blip.CreateBlip(blipType, new Vector3(-803.1636f, -223.88596f, 37.225594f), 0.8f, blipColor, name: "Komis samochodów klasy premium", shortRange: true);
                    break;
                case "regular2":
                    blipType = 663;
                    NAPI.Blip.CreateBlip(blipType, position, 0.8f, blipColor, name: "Komis samochodów klasy średniej", shortRange: true);
                    break;
                default:
                    blipType = 0;
                    break;
            }
        }
        private void getVehiclesOfType(string type)
        {
            switch(type)
            {
                case "bike":
                    cars = CustomVehicles.Bikes;
                    break;
                case "classic":
                    cars = CustomVehicles.Classics;
                    break;
                case "hyper":
                    cars = CustomVehicles.Hypers;
                    break;
                case "junk":
                    cars = CustomVehicles.Junks;
                    break;
                case "regular":
                    cars = CustomVehicles.Regulars;
                    break;
                case "regular2":
                    cars = CustomVehicles.Regulars2;
                    break;
                case "sport":
                    cars = CustomVehicles.Sports;
                    break;
                case "suv":
                    cars = CustomVehicles.SUVs;
                    break;
                case "offroad":
                    cars = CustomVehicles.Offroads;
                    break;
            }
            
        }

        public void SpawnNew(bool deleteVeh = false, bool instant = false)
        {
            if (deleteVeh && vehicle != null && vehicle.Exists)
            {
                vehicle.Delete();
            }
            colShape.Delete();
            priceLabel.Delete();
            nameLabel.Text = "Oczekuje na spawn...";
            vehicle = null;
            customVehicle = null;
            rollTime = DateTime.Now.AddMinutes(30);
            if (!instant){
                System.Threading.Tasks.Task.Run(() =>
                {
                    NAPI.Task.Run(() =>
                    {
                        foreach(Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                        {
                            if(vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "personal")
                            {
                                if (vehicle.Position.DistanceTo(position) < 3.0f)
                                {
                                    Player player = PlayerDataManager.GetPlayerBySocialId(Convert.ToUInt64(vehicle.GetSharedData<Int64>("owner")));
                                    if(player != null)
                                        PlayerDataManager.NotifyPlayer(player, $"Pojazd o ID: {vehicle.GetSharedData<Int32>("id").ToString()} został przeniesiony do przechowalni!");
                                    VehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                                    vehicle.Delete();
                                }
                            }
                            else if(vehicle.HasSharedData("type") && !(vehicle.GetSharedData<string>("type") == "dealer"))
                            {
                                if(vehicle.Position.DistanceTo(position) < 3.0f)
                                {
                                    vehicle.Delete();
                                }
                            }
                        }
                        Create();
                    }, delayTime: 60000);
                });
            }
            else{
                foreach(Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                {
                    if(vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "personal")
                    {
                        if (vehicle.Position.DistanceTo(position) < 3.0f)
                        {
                            Player player = PlayerDataManager.GetPlayerBySocialId(Convert.ToUInt64(vehicle.GetSharedData<Int64>("owner")));
                            if(player != null)
                                PlayerDataManager.NotifyPlayer(player, $"Pojazd o ID: {vehicle.GetSharedData<Int32>("id").ToString()} został przeniesiony do przechowalni!");
                            VehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                            vehicle.Delete();
                        }
                    }
                    else if(vehicle.HasSharedData("type") && !(vehicle.GetSharedData<string>("type") == "dealer"))
                    {
                        if(vehicle.Position.DistanceTo(position) < 3.0f)
                        {
                            vehicle.Delete();
                        }
                    }
                }
                Create();
            }
        }
    }
}

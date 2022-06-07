using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("openCarDealer")]
        public void OpenCarDealer(Player player)
        {
            foreach (CarDealer cd in carDealers)
            {
                if (cd.colShape.IsPointWithin(player.Position))
                {
                    if (cd.vehicle != null && cd.vehicle.Exists)
                    {
                        int[] t = new int[] { 1, 1 };
                        if (cd.vehicle.HasSharedData("trunksize"))
                            t = JsonConvert.DeserializeObject<int[]>(cd.vehicle.GetSharedData<string>("trunksize"));
                        int trunk = t[0] * t[1];
                        string[] data = new string[]
                        {
                            cd.customVehicle.name,
                            cd.customVehicle.price.ToString(),
                            cd.customVehicle.combustion.ToString(),
                            cd.customVehicle.tank.ToString(),
                            trunk.ToString(),
                            cd.vehicle.GetSharedData<float>("veh_trip").ToString()
                        };

                        player.TriggerEvent("openVehBuyBrowser", JsonConvert.SerializeObject(data), cd.vehicle);
                    }
                    break;
                }

            }
        }



        [RemoteEvent("confirmVehicleBuy")]
        public void ConfirmVehicleBuy(Player player, Vehicle vehicle)
        {
            foreach (CarDealer cd in carDealers)
            {
                if (cd.vehicle == vehicle)
                {
                    if (PlayerDataManager.HasPlayerFreeSlot(player))
                    {
                        if (PlayerDataManager.UpdatePlayersMoney(player, -1 * cd.customVehicle.price))
                        {
                            cd.vehicle = null;
                            VehicleDataManager.CreatePersonalVehicleFromDealer(player, vehicle);
                            cd.SpawnNew(false, false);
                            if (cd.type == "hyper" || cd.type == "classic" || cd.type == "suv")
                            {
                                VehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                                vehicle.Delete();
                                PlayerDataManager.NotifyPlayer(player, "Pomyślnie zakupiono pojazd. Czeka on na Ciebie w przechowalni!");
                            }
                            PlayerDataManager.NotifyPlayer(player, "Pomyślnie zakupiono pojazd!");
                            player.TriggerEvent("closeVehBuyBrowser");
                            break;
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Niestety, nie stać Cię na ten pojazd!");
                            break;
                        }
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Niestety, nie masz wolnych slotów na pojazdy!");
                        break;
                    }
                }
            }
        }

        public void CarDealerRoll(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (CarDealer cD in carDealers)
                {
                    if (DateTime.Compare(DateTime.Now, cD.rollTime) >= 0)
                    {
                        cD.SpawnNew(true, false);
                    }
                }

            });
        }

        private void CreateCarDealers()
        {
            carDealers.Add(new CarDealer(new Vector3(-47.854286f, -1102.2466f, 26.422354f), 69.04183f, "sport", false, new int[] { 0, 200 }));
            carDealers.Add(new CarDealer(new Vector3(-44.30936f, -1094.234f, 26.422354f), 118.58548f, "sport", true, new int[] { 0, 200 }));
            carDealers.Add(new CarDealer(new Vector3(-48.185146f, -1092.8115f, 26.422354f), 118.58548f, "sport", false, new int[] { 0, 200 }));

            carDealers.Add(new CarDealer(new Vector3(2553.4504f, 4672.2476f, 33.93297f), 15f, "offroad", true, new int[] { 1000, 10000 }));

            carDealers.Add(new CarDealer(new Vector3(-240.60123f, 6199.235f, 31.489218f), 144f, "regular", false, new int[] { 15000, 35000 }));
            carDealers.Add(new CarDealer(new Vector3(-243.02003f, 6201.7427f, 31.489218f), 144f, "regular", true, new int[] { 15000, 35000 }));
            carDealers.Add(new CarDealer(new Vector3(-245.30743f, 6203.9624f, 31.489218f), 144f, "regular", false, new int[] { 15000, 35000 }));

            carDealers.Add(new CarDealer(new Vector3(1616.0236f, 3788.4014f, 34.725067f), -140f, "junk", false, new int[] { 50000, 75000 }));
            carDealers.Add(new CarDealer(new Vector3(1617.5266f, 3791.165f, 34.73369f), -140f, "junk", true, new int[] { 50000, 75000 }));

            carDealers.Add(new CarDealer(new Vector3(-806.7707f, -210.8126f, 58.910355f), -150f, "hyper", false, new int[] { 0, 100 }));
            carDealers.Add(new CarDealer(new Vector3(-812.215f, -201.61041f, 58.88827f), -150f, "hyper", false, new int[] { 0, 100 }));

            carDealers.Add(new CarDealer(new Vector3(1252.9817f, -1141.6354f, 38.757774f), 75.5f, "bike", false, new int[] { 1000, 10000 }));
            carDealers.Add(new CarDealer(new Vector3(1251.4142f, -1137.7074f, 38.75776f), 75.5f, "bike", true, new int[] { 1000, 10000 }));
            carDealers.Add(new CarDealer(new Vector3(1250.1792f, -1133.866f, 38.757716f), 75.5f, "bike", false, new int[] { 1000, 10000 }));

            carDealers.Add(new CarDealer(new Vector3(-786.7027f, -202.84512f, 58.89831f), 30f, "suv", false, new int[] { 0, 200 }));
            carDealers.Add(new CarDealer(new Vector3(-792.28204f, -193.24438f, 58.8983f), 30f, "suv", false, new int[] { 0, 200 }));
            carDealers.Add(new CarDealer(new Vector3(-803.0028f, -194.12485f, 58.910297f), 120f, "classic", true, new int[] { 20000, 50000 }));

            carDealers.Add(new CarDealer(new Vector3(1010.34125f, -1869.964f, 30.88981f), -8f, "regular2", false, new int[] { 10000, 40000 }));
            carDealers.Add(new CarDealer(new Vector3(1005.42834f, -1869.6227f, 30.88981f), -3, "regular2", true, new int[] { 10000, 40000 }));
            carDealers.Add(new CarDealer(new Vector3(1000.40985f, -1868.8093f, 30.88981f), -3f, "regular2", false, new int[] { 10000, 40000 }));


        }

    }
}

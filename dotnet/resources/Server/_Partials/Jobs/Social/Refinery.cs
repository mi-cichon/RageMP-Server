using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("refinery_startJob")]
        public void Refinery_StartJob(Player player)
        {
            refinery.StartJob(player);
        }

        [RemoteEvent("refinery_selectJobType")]
        public void Refinery_SelectJobType(Player player, int jobLevel)
        {
            int stationsCount = jobLevel == 1 ? 3 : jobLevel == 2 ? 5 : jobLevel == 3 ? 7 : 0;
            Dictionary<int, float> stationIndexes = new Dictionary<int, float>();
            List<Vector3> stations = new List<Vector3>();
            for (int i = 0; i < stationsCount; i++)
            {
                Random rnd = new Random();
                Vector3 petrolStation = petrolStations[rnd.Next(0, petrolStations.Count)].vehiclePosition;
                while (stations.Contains(petrolStation))
                {
                    petrolStation = petrolStations[rnd.Next(0, petrolStations.Count)].vehiclePosition;
                }
                stations.Add(petrolStation);
            }
            Dictionary<Vector3, float> stationValues = new Dictionary<Vector3, float>();
            foreach (Vector3 stationPos in stations)
            {
                Random rnd = new Random();
                var value = (float)Math.Round((rnd.NextDouble() * (1500 - 1000) + 1000));
                stationValues.Add(stationPos, value);
                stationIndexes.Add(petrolStations.IndexOf(petrolStations.Find(ps => ps.vehiclePosition == stationPos)), value);
            }
            player.TriggerEvent("refinery_setNewStations", JsonConvert.SerializeObject(stationValues), jobLevel, JsonConvert.SerializeObject(stationIndexes));
        }

        [RemoteEvent("refinery_openPumpingPanel")]
        public void Refinery_OpenPumpingPanel(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("oiltank"))
            {
                if (vehicle.GetSharedData<float>("oiltank") < 5000)
                {
                    foreach (OilPump oilPump in refinery.oilPumps)
                    {
                        if (oilPump.Colshape.IsPointWithin(player.Position))
                        {
                            if (oilPump.OilAmount > 0)
                            {
                                player.TriggerEvent("refinery_openBrowser", vehicle.GetSharedData<float>("oiltank"), vehicle, refinery.oilPumps.IndexOf(oilPump));
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Ten szyb jest pusty!");
                            }
                            return;
                        }
                    }
                    playerDataManager.NotifyPlayer(player, "Nie jesteś w pobliżu żadnego szybu naftowego!");
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Pojazd jest pełny!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Wystąpił błąd z otwarciem panelu pompowania!");
            }
        }

        [RemoteEvent("refinery_addOil")]
        public void Refinery_AddOil(Player player, Vehicle vehicle, int pumpIndex, float amount)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("oiltank"))
            {
                int maxFuel = 5000;
                if (player.GetSharedData<bool>("jobBonus_17"))
                {
                    maxFuel *= 2;
                }
                else if (player.GetSharedData<bool>("jobBonus_16"))
                {
                    maxFuel = Convert.ToInt32(maxFuel * 1.5);
                }
                else if (player.GetSharedData<bool>("jobBonus_15"))
                {
                    maxFuel *= Convert.ToInt32(maxFuel * 1.25);
                }

                if (vehicle.GetSharedData<float>("oiltank") < maxFuel)
                {
                    var oilPump = refinery.oilPumps[pumpIndex];
                    if (oilPump.OilAmount > 0)
                    {
                        var oil = vehicle.GetSharedData<float>("oiltank");
                        if (oilPump.OilAmount - amount <= 0)
                        {
                            amount = oilPump.OilAmount;
                            oilPump.UpdateOilAmount(0);
                        }
                        else
                        {
                            oilPump.UpdateOilAmount(oilPump.OilAmount - amount);
                        }
                        if (oil + amount >= maxFuel)
                        {
                            oil = maxFuel;
                        }
                        else
                        {
                            oil += amount;
                        }
                        vehicle.SetSharedData("oiltank", oil);
                        player.TriggerEvent("refinery_refreshTank", oil);
                        player.TriggerEvent("refinery_refreshStationValues");
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Ten szyb jest pusty!");
                        player.TriggerEvent("refinery_closeBrowser");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Pojazd jest pełny!");
                    player.TriggerEvent("refinery_closeBrowser");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Wystąpił błąd z zatankowaniem pojazdu!");
                player.TriggerEvent("refinery_closeBrowser");
            }
        }
        [RemoteEvent("refinery_updateTruckOil")]
        public void Refinery_UpdateTruckOil(Player player, Vehicle vehicle, float amount)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("oiltank"))
            {
                vehicle.SetSharedData("oiltank", amount);
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Wystąpił błąd z ustawieniem poziomu zatankowania!");
                player.TriggerEvent("refinery_closeBrowser");
            }
        }

        [RemoteEvent("refinery_payment")]
        public void Refinery_Payment(Player player, int liters, int type, string currentOrder)
        {
            payoutManager.RefineryPayment(player, liters, type);
            dataManager.UpdatePetrolStationValues(currentOrder);
        }
    }
}

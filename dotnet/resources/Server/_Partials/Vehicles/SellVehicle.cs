using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("sellVeh_open")]
        public void SellVeh_Open(Player player)
        {
            //if(player.GetSharedData<int>("power") >= 2)
            //{
            if (player.Vehicle != null)
            {
                DateTime last = DateTime.Parse(player.GetSharedData<string>("vehsold"));
                if (last.AddHours(12) <= DateTime.Now)
                {
                    if (player.Vehicle.HasSharedData("owner") && player.Vehicle.GetSharedData<Int64>("owner").ToString() == player.SocialClubId.ToString())
                    {
                        int price = vehicleDataManager.GetVehiclesSellPrice(player.Vehicle);
                        player.TriggerEvent("sellVeh_openBrowser", player.Vehicle, player.Vehicle.GetSharedData<string>("name"), price.ToString(), player.Vehicle.GetSharedData<float>("veh_trip").ToString());
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie jesteś właścicielem tego pojazdu!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie możesz jeszcze sprzedać pojazdu, będzie to możliwe " + last.AddHours(12).ToString() + ".");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Aby z tego korzystać musisz być w pojeździe!");
            }
            //}
            //else
            //{
            //    playerDataManager.NotifyPlayer(player, "Ta funkcja chwilowo jest dostępna tylko dla testerów!");
            //}

        }

        [RemoteEvent("sellVeh_sellVehicle")]
        public void SellVeh_SellVehicle(Player player, Vehicle vehicle, int price)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("owner") && vehicle.GetSharedData<Int64>("owner").ToString() == player.SocialClubId.ToString())
                {
                    vehicleDataManager.UpdateVehiclesOwner(vehicle, 0);
                    vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                    int carId = vehicle.GetSharedData<int>("id");
                    vehicle.Delete();
                    foreach (Organization org in orgManager.orgs)
                    {
                        if (org.vehicles.Contains(carId) || org.vehicleRequests.Contains(carId))
                        {
                            if (org.vehicles.Contains(carId))
                            {
                                org.vehicles.Remove(carId);
                            }
                            if (org.vehicleRequests.Contains(carId))
                            {
                                org.vehicleRequests.Remove(carId);
                            }
                            org.SaveOrgToDataBase();
                            break;
                        }
                    }
                    if (playerDataManager.UpdatePlayersMoney(player, price))
                    {
                        playerDataManager.NotifyPlayer(player, "Pomyślnie sprzedano pojazd! Kolejna sprzedaż będzie możliwa " + DateTime.Now.AddHours(12).ToString() + ".");
                        playerDataManager.UpdateVehicleSold(player, DateTime.Now.ToString());
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Wystąpił błąd!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie jesteś właścicielem tego pojazdu!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }
    }
}

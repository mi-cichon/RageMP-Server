﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        public void GlobalTimer(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (House house in houses.houses)
                {
                    if (house.owner != "")
                    {
                        string time = house.houseColShape.GetSharedData<string>("time");
                        if (DateTime.Compare(DateTime.Now, DateTime.Parse(time)) > 0)
                        {
                            house.clearOwner();
                        }
                    }
                }
                foreach (Player player in NAPI.Pools.GetAllPlayers())
                {
                    if (player.Exists)
                    {
                        player.SetSharedData("ping", player.Ping);
                        if (player.HasSharedData("playtime"))
                        {
                            player.SetSharedData("playtime", player.GetSharedData<Int32>("playtime") + 1);
                            playerDataManager.UpdatePlayersPlaytime(player, player.GetSharedData<Int32>("playtime"));
                        }

                        if (!(player.HasSharedData("afk") && player.GetSharedData<bool>("afk")) && player.HasSharedData("bonustime"))
                        {
                            if (player.GetSharedData<Int32>("bonustime") < 60)
                            {
                                player.SetSharedData("bonustime", player.GetSharedData<Int32>("bonustime") + 1);
                                if(player.GetSharedData<Int32>("bonustime") == 60)
                                {
                                    playerDataManager.NotifyPlayer(player, "Bonus godzinny jest gotowy do odbioru w aplikacji Bonusy!");
                                }
                            }
                            autoSave.SavePlayersBonusData(player, player.GetSharedData<Int32>("bonustime"));
                        }

                        if (playerDataManager.isPlayersPenaltyExpired(player, "muted"))
                        {
                            player.SetSharedData("muted", false);
                            playerDataManager.SendInfoToPlayer(player, "Twoja kara wyciszenia wygasła!");
                        }
                        if (playerDataManager.isPlayersPenaltyExpired(player, "nodriving"))
                        {
                            player.SetSharedData("nodriving", false);
                            playerDataManager.SendInfoToPlayer(player, "Twoje prawo jazdy odzyskało ważność!");
                        }
                    }
                }
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Owner != 0 && tuneBusiness.PaidTo <= DateTime.Now)
                    {
                        tuneBusiness.ResetOwner();
                    }
                    else if (tuneBusiness.WheelOrders != null && tuneBusiness.WheelOrders.Count > 0)
                    {
                        List<KeyValuePair<int[], DateTime>> idsToRemove = new List<KeyValuePair<int[], DateTime>>();
                        foreach (KeyValuePair<int[], DateTime> order in tuneBusiness.WheelOrders)
                        {
                            if (order.Value <= DateTime.Now)
                            {
                                idsToRemove.Add(order);
                            }
                        }
                        if (idsToRemove.Count > 0)
                        {
                            foreach (KeyValuePair<int[], DateTime> id in idsToRemove)
                            {
                                tuneBusiness.AvailableWheels.Add(id.Key);
                                tuneBusiness.WheelOrders.Remove(id);
                            }
                            tuneBusiness.SaveBusinessToDB();
                        }
                    }
                }
            });
        }

        public void SetNewBonus(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                string[] bonus = payoutManager.SetNewBonus();
                string time = DateTime.Now.AddHours(1).ToString();
                payoutManager.bonusTime = DateTime.Now.AddHours(1);
                playerDataManager.SendInfoMessageToAllPlayers($"Wylosowano nowy bonus {float.Parse(bonus[1]) * 100}% na: {bonus[0]} do: {time}");
            });
        }

        public void InfoMessages(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                playerDataManager.SendInfoMessageToAllPlayers("Serwer jest w trakcie przygotowań. Wszystkie postępy zostaną usunięte tuż przed startem. Wszelkie błędy prosimy zgłaszać na Discordzie:  https://discord.gg/Yr6JeUtEM7");
            });
        }

        private void UpdateArrestTimes(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (Player player in NAPI.Pools.GetAllPlayers())
                {
                    if (player.HasSharedData("arrested") && player.GetSharedData<bool>("arrested"))
                    {
                        foreach (Arrest arrest in lspd.Arrests)
                        {
                            bool found = false;
                            foreach (KeyValuePair<ulong, int> inmate in arrest.Inmates)
                            {
                                if (inmate.Key == player.SocialClubId)
                                {
                                    int time = inmate.Value;
                                    time -= 1;
                                    arrest.Inmates[arrest.Inmates.IndexOf(inmate)] = new KeyValuePair<ulong, int>(player.SocialClubId, time);
                                    if (time == 0)
                                    {
                                        lspd.RemovePlayerFromArrest(player);
                                    }
                                    else if (time % 5 == 0)
                                    {
                                        playerDataManager.SendInfoToPlayer(player, "Pozostało " + time.ToString() + " minut aresztu!");
                                    }

                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                                break;
                        }
                    }
                }
            });
        }
        private void ValuesToDB(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (Player player in NAPI.Pools.GetAllPlayers())
                {
                    if (player.HasSharedData("spawned"))
                    {
                        if (player.Dimension == 0 && !(player.HasSharedData("spec") && player.GetSharedData<bool>("spec")))
                        {
                            playerDataManager.UpdatePlayersLastPos(player);
                        }
                        player.TriggerEvent("updatePlayerBlips", NAPI.Pools.GetAllBlips().ToArray());
                        player.TriggerEvent("getVehicleDamage");
                        player.TriggerEvent("updateVehicleBlips");
                    }
                }
                foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                {
                    if (vehicle.GetSharedData<string>("type") == "personal" && vehicle.Occupants.Count > 0)
                    {
                        (vehicle.Occupants[0] as Player).TriggerEvent("updateDirtLevel");
                    }
                    if ((vehicle.GetSharedData<string>("type") == "personal" && !(vehicle.HasSharedData("market") && vehicle.GetSharedData<bool>("market")) && vehicle.HasSharedData("veh_brake") && !vehicle.GetSharedData<bool>("veh_brake")) || (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "lspd"))
                    {
                        vehicleDataManager.UpdateVehiclesLastPos(vehicle);
                    }
                }
            });
        }

        private void SavePlayersJobData(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach(Player player in NAPI.Pools.GetAllPlayers())
                {
                    if(player.HasSharedData("job"))
                    {
                        switch(player.GetSharedData<string>("job"))
                        {
                            case "lawnmowing":
                                player.TriggerEvent("saveData_lawnmowing_save");
                                break;
                            case "gardener":
                                player.TriggerEvent("saveData_gardener_save");
                                break;
                            case "refinery":
                                player.TriggerEvent("saveData_refinery_save");
                                break;
                        }
                    }
                }
            });
        }
        private void UpdateCarWashAndStorage(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                {
                    if (vehicle.HasSharedData("washtime") && vehicle.GetSharedData<string>("washtime") != "" && DateTime.Compare(DateTime.Parse(vehicle.GetSharedData<string>("washtime")), DateTime.Now) < 0)
                    {
                        vehicleDataManager.UpdateVehiclesWashTime(vehicle, "");
                    }
                    if (vehicle.HasSharedData("storageTime") && vehicle.GetSharedData<string>("storageTime") != "")
                    {
                        DateTime time = DateTime.Parse(vehicle.GetSharedData<string>("storageTime"));
                        if (time.AddMinutes(1) <= DateTime.Now)
                        {
                            if (vehicle != null && vehicle.Exists)
                            {
                                vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                                vehicle.Delete();
                            }
                        }

                    }
                    if ((vehicle.HasSharedData("publicTime") && vehicle.GetSharedData<string>("publicTime") != ""))
                    {
                        DateTime time = DateTime.Parse(vehicle.GetSharedData<string>("publicTime"));
                        if (time.AddMinutes(1) <= DateTime.Now)
                        {
                            if (vehicle != null && vehicle.Exists)
                            {
                                vehicle.Delete();
                            }
                        }
                    }
                    if (vehicle.HasSharedData("publicTime") && vehicle.GetSharedData<string>("publicTime") == "" && vehicle.Occupants.Count == 0)
                    {
                        vehicle.Delete();
                    }
                }
            });
        }
    }
}
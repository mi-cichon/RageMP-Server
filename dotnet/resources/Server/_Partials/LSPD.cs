using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("lspd_StartDuty")]
        public void LSPDStartDuty(Player player)
        {
            lspd.SwitchPlayersDuty(player, true);
        }
        [RemoteEvent("lspd_createBarrier")]
        public void LSPDCreateBarrier(Player player, Vector3 position, Vector3 rotation)
        {
            lspd.CreateBarrier(player.GetSharedData<string>("username"), position, rotation);
        }
        [RemoteEvent("lspd_removeClosestBarrier")]
        public void LSPDRemoveClosestBarrier(Player player)
        {
            if (player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty") && player.Vehicle == null)
            {
                if (lspd.Barriers.Count > 0)
                {
                    KeyValuePair<GTANetworkAPI.Object, string> closestBarrier = new KeyValuePair<GTANetworkAPI.Object, string>();

                    foreach (KeyValuePair<GTANetworkAPI.Object, string> obj in lspd.Barriers)
                    {
                        if (closestBarrier.Equals(new KeyValuePair<GTANetworkAPI.Object, string>()) && obj.Key.Exists && obj.Key.Position.DistanceTo(player.Position) < 2)
                        {
                            closestBarrier = obj;
                        }
                        else if (closestBarrier.Key != null && closestBarrier.Key.Exists && obj.Key != null && obj.Key.Exists && player.Position.DistanceTo(obj.Key.Position) < player.Position.DistanceTo(closestBarrier.Key.Position))
                        {
                            closestBarrier = obj;
                        }
                    }
                    if (!closestBarrier.Equals(new KeyValuePair<GTANetworkAPI.Object, string>()))
                    {
                        string name = lspd.RemoveBarrier(closestBarrier);
                        playerDataManager.SendInfoToPlayer(player, "Usunięto barierkę stworzoną przez: " + name);
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "W pobliżu nie ma żadnej barierki!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "W pobliżu nie ma żadnej barierki!");
                }
            }
        }
        [RemoteEvent("lspd_cuffClosestPlayer")]
        public void LSPDCuffClosestPlayer(Player player)
        {
            Player p = playerDataManager.GetClosestCivilianToCuff(player.Position, 1);
            if (p != null)
            {
                if (p.HasSharedData("handCuffed") && p.GetSharedData<bool>("handCuffed") && p.GetSharedData<Player>("cuffedBy") == player)
                {
                    p.TriggerEvent("handCuff", player, false);
                    p.SetSharedData("handCuffed", false);
                    player.SetSharedData("cuffedPlayer", 0);
                    playerDataManager.NotifyPlayer(player, "Rozkuto  " + p.GetSharedData<string>("username"));
                    playerDataManager.NotifyPlayer(p, "Zostałeś rozkuty");
                }
                else if (!(p.HasSharedData("handCuffed") && p.GetSharedData<bool>("handCuffed")))
                {
                    if (!(player.HasSharedData("cuffedPlayer") && player.GetSharedData<Player>("cuffedPlayer") != null && player.GetSharedData<Player>("cuffedPlayer").Exists))
                    {
                        p.TriggerEvent("handCuff", player, true);
                        playerDataManager.NotifyPlayer(p, "Zostałeś zakuty przez " + player.GetSharedData<string>("username"));
                        playerDataManager.NotifyPlayer(player, "Zakuto " + p.GetSharedData<string>("username"));
                        p.SetSharedData("cuffedBy", player.Handle);
                        p.SetSharedData("handCuffed", true);
                        player.SetSharedData("cuffedPlayer", p.Handle);
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Zakuć możesz tylko jednego gracza!");
                    }
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono gracza lub jest on za daleko!");
            }
        }

        [RemoteEvent("setCuffedPlayerIntoVeh")]
        public void SetCuffedPlayerIntoVeh(Player player, Vehicle vehicle, Player cuffed, int seat)
        {
            if (vehicle != null && vehicle.Exists && cuffed != null && cuffed.Exists)
            {
                cuffed.SetIntoVehicle(vehicle.Handle, seat);
            }
        }

        [RemoteEvent("warpCuffedPlayerOutOfVeh")]
        public void WarpCuffedPlayerOutOfVeh(Player player, Player cuffed)
        {
            if (cuffed != null && cuffed.Exists && cuffed.Vehicle != null)
            {
                cuffed.WarpOutOfVehicle();
                cuffed.TriggerEvent("setCuffed", player);
                NAPI.ClientEvent.TriggerClientEventForAll("setPlayerCuffed", cuffed, player);
            }
        }

        [RemoteEvent("lspd_openStorage")]
        public void LSPDOpenStorage(Player player)
        {
            if (player.HasSharedData("lspd_power") && player.GetSharedData<int>("lspd_power") > 0)
            {
                string stor = lspd.GetAvailableVehicles(player.GetSharedData<int>("lspd_power"));
                if (stor.Length > 0)
                {
                    player.TriggerEvent("openLspdStorageBrowser", stor);
                }

            }
        }
        [RemoteEvent("lspd_CreateVehicle")]
        public void LSPDCreateVehicle(Player player, int id)
        {
            if (lspd.SpawnVehicle(id))
            {
                playerDataManager.NotifyPlayer(player, "Pomyślnie wyjęto pojazd!");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie udało się wyjąć pojazdu!");
            }
        }


    }
}

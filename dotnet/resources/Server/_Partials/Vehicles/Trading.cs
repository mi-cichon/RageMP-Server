using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {

        [RemoteEvent("openCarTraderBrowser")]
        public void OpenCarTraderBrowser(Player player)
        {
            string cars = vehicleDataManager.GetPlayersVehicles(player, false, null);
            Dictionary<int, string> players = new Dictionary<int, string>();
            foreach (Player p in NAPI.Pools.GetAllPlayers())
            {
                if (player.Position.DistanceTo(p.Position) < 10.0f && p != player)
                {
                    players.Add(p.GetSharedData<Int32>("id"), p.GetSharedData<string>("username"));
                }
            }
            string playerstr = "";
            if (players.Count != 0)
            {
                playerstr = JsonConvert.SerializeObject(players);
            }
            player.TriggerEvent("openCarTraderBrowser", playerstr, cars);
        }

        [RemoteEvent("sendTrade")]
        public void SendTrade(Player player, int playerId, int carId, int price)
        {
            if (vehicleDataManager.GetVehiclesOwner(carId.ToString()) == player.GetSharedData<string>("socialclub"))
            {
                Player p = playerDataManager.GetPlayerByRemoteId(playerId.ToString());
                if (p != null && p.Position.DistanceTo(player.Position) < 10.0f)
                {
                    if ((p.HasSharedData("tradeOffer") && !p.GetSharedData<bool>("tradeOffer")) || !p.HasSharedData("tradeOffer"))
                    {
                        p.SetSharedData("tradeOffer", true);
                        string[] vehicleData = vehicleDataManager.GatherVehiclesInfo(carId);
                        p.TriggerEvent("carTrade_openBrowser", player, carId, price, vehicleData[0], vehicleData[1], vehicleData[2]);
                        playerDataManager.NotifyPlayer(player, "Oferta wysłana!");
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Gracz otrzymał już inną ofertę!");
                    }

                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Gracz oddalił się od punktu sprzedaży pojazdów!");
                }


            }
        }

        [RemoteEvent("carTrade_confirmTrade")]
        public void CarTrade_ConfirmTrade(Player player, Player seller, int carId, int price)
        {
            if (seller != null && seller.Exists && vehicleDataManager.GetVehiclesOwner(carId.ToString()) == seller.GetSharedData<string>("socialclub"))
            {
                if (player.Position.DistanceTo(seller.Position) < 10.0f)
                {
                    if (playerDataManager.HasPlayerFreeSlot(player))
                    {
                        if (player != null && seller != null && player.Exists && seller.Exists && playerDataManager.UpdatePlayersMoney(player, -1 * price))
                        {
                            playerDataManager.UpdatePlayersMoney(seller, price);
                            vehicleDataManager.UpdateVehiclesDBOwner(carId, player.GetSharedData<string>("socialclub"));
                            Vehicle veh = vehicleDataManager.GetVehicleById(carId.ToString());
                            if (veh != null)
                            {
                                veh.SetSharedData("owner", player.SocialClubId);
                            }
                            playerDataManager.NotifyPlayer(player, "Pomyślnie zakupiono pojazd!");
                            playerDataManager.NotifyPlayer(seller, "Pomyślnie sprzedano pojazd!");
                            player.SetSharedData("tradeOffer", false);
                            logManager.LogVehicleTrade(player.SocialClubId.ToString(), $"Zakupiono pojazd o ID {carId.ToString()} za {price.ToString()}$ od {seller.SocialClubId.ToString()}");
                            logManager.LogVehicleTrade(seller.SocialClubId.ToString(), $"Sprzedano pojazd o ID {carId.ToString()} za {price.ToString()}$ graczowi {player.SocialClubId.ToString()}");
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
                            if (veh != null)
                            {
                                veh.SetSharedData("orgId", 0);
                            }
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie stać cię na to!");
                        }
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie masz wolnych slotów na pojazdy!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Gracz się oddalił!");
                }
            }
        }
        [RemoteEvent("setTradeOffer")]
        public void SetTradeOffer(Player player, bool state)
        {
            player.SetSharedData("tradeOffer", state);
        }

    }
}

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
            string cars = VehicleDataManager.GetPlayersVehicles(player, false, null);
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
            if (VehicleDataManager.GetVehiclesOwner(carId.ToString()) == player.GetSharedData<string>("socialclub"))
            {
                Player p = PlayerDataManager.GetPlayerByRemoteId(playerId.ToString());
                if (p != null && p.Position.DistanceTo(player.Position) < 10.0f)
                {
                    if ((p.HasSharedData("tradeOffer") && !p.GetSharedData<bool>("tradeOffer")) || !p.HasSharedData("tradeOffer"))
                    {
                        p.SetSharedData("tradeOffer", true);
                        string[] vehicleData = VehicleDataManager.GatherVehiclesInfo(carId);
                        p.TriggerEvent("carTrade_openBrowser", player, carId, price, vehicleData[0], vehicleData[1], vehicleData[2]);
                        PlayerDataManager.NotifyPlayer(player, "Oferta wysłana!");
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Gracz otrzymał już inną ofertę!");
                    }

                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Gracz oddalił się od punktu sprzedaży pojazdów!");
                }


            }
        }

        [RemoteEvent("carTrade_confirmTrade")]
        public void CarTrade_ConfirmTrade(Player player, Player seller, int carId, int price)
        {
            if (seller != null && seller.Exists && VehicleDataManager.GetVehiclesOwner(carId.ToString()) == seller.GetSharedData<string>("socialclub"))
            {
                if (player.Position.DistanceTo(seller.Position) < 10.0f)
                {
                    if (PlayerDataManager.HasPlayerFreeSlot(player))
                    {
                        if (player != null && seller != null && player.Exists && seller.Exists && PlayerDataManager.UpdatePlayersMoney(player, -1 * price))
                        {
                            PlayerDataManager.UpdatePlayersMoney(seller, price);
                            VehicleDataManager.UpdateVehiclesDBOwner(carId, player.GetSharedData<string>("socialclub"));
                            Vehicle veh = VehicleDataManager.GetVehicleById(carId.ToString());
                            if (veh != null)
                            {
                                veh.SetSharedData("owner", player.SocialClubId);
                            }
                            PlayerDataManager.NotifyPlayer(player, "Pomyślnie zakupiono pojazd!");
                            PlayerDataManager.NotifyPlayer(seller, "Pomyślnie sprzedano pojazd!");
                            player.SetSharedData("tradeOffer", false);
                            LogManager.LogVehicleTrade(player.SocialClubId.ToString(), $"Zakupiono pojazd o ID {carId.ToString()} za {price.ToString()}$ od {seller.SocialClubId.ToString()}");
                            LogManager.LogVehicleTrade(seller.SocialClubId.ToString(), $"Sprzedano pojazd o ID {carId.ToString()} za {price.ToString()}$ graczowi {player.SocialClubId.ToString()}");
                            foreach (Organization org in OrgManager.orgs)
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
                            PlayerDataManager.NotifyPlayer(player, "Nie stać cię na to!");
                        }
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Nie masz wolnych slotów na pojazdy!");
                    }
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Gracz się oddalił!");
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

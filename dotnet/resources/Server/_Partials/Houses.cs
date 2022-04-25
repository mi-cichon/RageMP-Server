using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("openHouseStorage")]
        public void OpenHouseStorage(Player player, int houseId)
        {
            foreach (House house in houses.houses)
            {
                if (house.id == houseId)
                {
                    player.TriggerEvent("openSecondEquipmentBrowser", player.GetSharedData<string>("equipment"), house.storage, "h" + house.id, JsonConvert.SerializeObject(house.storageSize));
                    break;
                }
            }
        }
        [RemoteEvent("enterHouse")]
        public void EnterHouse(Player player, ColShape house)
        {
            player.Position = house.GetSharedData<Vector3>("interior");
            player.Dimension = (uint)(house.GetSharedData<Int32>("id") + 500);
        }

        [RemoteEvent("leaveHouse")]
        public void LeaveHouse(Player player, ColShape house)
        {
            player.Position = house.GetSharedData<Vector3>("housepos");
            player.Dimension = 0;
        }

        [RemoteEvent("confirmHouseBuy")]
        public void ConfirmHouseBuy(Player player, int houseId, int time)
        {
            foreach (House house in houses.houses)
            {
                if (house.houseColShape.GetSharedData<Int32>("id") == houseId)
                {
                    if (house.owner != "")
                    {
                        playerDataManager.NotifyPlayer(player, "Dom został już wynajęty przez kogoś innego!");
                        player.TriggerEvent("closeHousePanelBrowser");
                    }
                    else
                    {
                        if (player.GetSharedData<Int32>("houseid") != -1)
                        {
                            playerDataManager.NotifyPlayer(player, "Jesteś już w posiadaniu innego domku!");
                            player.TriggerEvent("closeHousePanelBrowser");
                        }
                        else
                        {
                            int price = house.houseColShape.GetSharedData<Int32>("price");
                            if (playerDataManager.UpdatePlayersMoney(player, -1 * price * time))
                            {
                                house.setOwner(player, DateTime.Now.AddDays(time).ToString());
                                player.TriggerEvent("closeHousePanelBrowser");
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Nie stać cię na ten domek!");
                                player.TriggerEvent("closeHousePanelBrowser");
                            }
                        }
                    }
                }
            }
        }

        [RemoteEvent("confirmHouseExtend")]
        public void ConfirmHouseExtend(Player player, int houseId, int time)
        {
            foreach (House house in houses.houses)
            {
                if (house.houseColShape.GetSharedData<Int32>("id") == houseId)
                {
                    if (house.owner != player.GetSharedData<string>("socialclub"))
                    {
                        playerDataManager.NotifyPlayer(player, "Dom został już wynajęty przez kogoś innego!");
                        player.TriggerEvent("closeHousePanelBrowser");
                    }
                    else
                    {
                        int price = house.houseColShape.GetSharedData<Int32>("price");
                        DateTime datetime = DateTime.Parse(house.houseColShape.GetSharedData<string>("time"));
                        if (DateTime.Compare(datetime.AddDays(time), DateTime.Now.AddDays(14)) > 0)
                        {
                            player.TriggerEvent("callHousePanelError", "Dom można wynająć na maksymalnie 14 dni naprzód");
                        }
                        else
                        {
                            if (playerDataManager.UpdatePlayersMoney(player, -1 * price * time))
                            {
                                house.extendTime(datetime.AddDays(time).ToString());
                                player.TriggerEvent("updateHousePanelTime", datetime.AddDays(time).ToString());
                                playerDataManager.NotifyPlayer(player, "Przedłużono wynajem domu do: " + datetime.AddDays(time).ToString());
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Nie stać cię na ten domek!");
                                player.TriggerEvent("closeHousePanelBrowser");
                            }
                        }


                    }
                }
            }
        }

        [RemoteEvent("giveHouseUp")]
        public void GiveUpHouse(Player player, int houseId)
        {
            foreach (House house in houses.houses)
            {
                if (house.houseColShape.GetSharedData<Int32>("id") == houseId)
                {
                    if (house.owner != player.SocialClubId.ToString())
                    {
                        playerDataManager.NotifyPlayer(player, "Nie jesteś właścicielem domu!");
                        player.TriggerEvent("closeHousePanelBrowser");
                    }
                    else
                    {
                        house.clearOwner();
                        playerDataManager.NotifyPlayer(player, "Nie jesteś już właścicielem tego domu!");
                        player.TriggerEvent("closeHousePanelBrowser");
                    }
                }
            }
        }
    }
}

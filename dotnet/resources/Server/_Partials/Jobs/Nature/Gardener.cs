using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("gardener_pickupPlant")]
        public void Gardener_PickupPlant(Player player, int plant_id, int groundId)
        {
            foreach (Ground ground in gardener.Grounds)
            {
                if (ground.GroundId == groundId)
                {
                    foreach (KeyValuePair<Vector3, Plant> pair in ground.Plants)
                    {
                        if (pair.Value.Id == plant_id)
                        {
                            ground.PickPlantUp(player, pair);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        [RemoteEvent("gardener_startJob")]
        public void Gardener_StartJob(Player player)
        {
            gardener.StartJob(player);
        }

        [RemoteEvent("gardener_selectOrder")]
        public void Gardener_SelectOrder(Player player, int orderId)
        {
            foreach (GardenerOrder order in gardener.Orders)
            {
                if (order.Id == orderId)
                {
                    player.TriggerEvent("closeGardenerOrdersBrowser");
                    string o = JsonConvert.SerializeObject(order.Plants);
                    player.TriggerEvent("gardener_setNewOrder", o);
                    gardener.NewOrder(gardener.Orders.IndexOf(order));
                    return;
                }
            }
            playerDataManager.NotifyPlayer(player, "To zlecenie jest już nieaktualne!");
        }

        [RemoteEvent("gardener_refreshOrders")]
        public void Gardener_RefreshOrders(Player player)
        {
            player.TriggerEvent("gardener_insertData", gardener.GetOrders());
        }

        [RemoteEvent("gardener_cancelOrder")]
        public void Gardener_CancelOrder(Player player)
        {
            EndJob(player);
        }

        [RemoteEvent("gardener_sellPlants")]
        public void Gardener_SellPlants(Player player, string baseOrder, string orderState)
        {
            int[] order = JsonConvert.DeserializeObject<int[]>(orderState);
            List<EqItem> equipment = JsonConvert.DeserializeObject<List<EqItem>>(player.GetSharedData<string>("equipment"));
            List<int> itemsToRemove = new List<int>();
            int exp = 0;
            foreach (EqItem item in equipment)
            {
                switch (item.TypeID)
                {
                    case 900:
                        if (order[0] > 0)
                        {
                            order[0]--;
                            itemsToRemove.Add(item.Id);
                            exp += 3;
                        }
                        break;
                    case 901:
                        if (order[1] > 0)
                        {
                            order[1]--;
                            itemsToRemove.Add(item.Id);
                            exp += 3;
                        }
                        break;
                    case 902:
                        if (order[2] > 0)
                        {
                            order[2]--;
                            itemsToRemove.Add(item.Id);
                            exp += 3;
                        }
                        break;
                    case 903:
                        if (order[3] > 0)
                        {
                            order[3]--;
                            itemsToRemove.Add(item.Id);
                            exp += 5;
                        }
                        break;
                    case 904:
                        if (order[4] > 0)
                        {
                            order[4]--;
                            itemsToRemove.Add(item.Id);
                            exp += 5;
                        }
                        break;
                }
            }

            if (itemsToRemove.Count > 0)
            {
                itemsToRemove.ForEach(item =>
                {
                    player.TriggerEvent("removeEqItem", item);
                });
                payoutManager.GardenerPlantsSold(player, exp);
                playerDataManager.NotifyPlayer(player, "Pomyślnie oddano rośliny!");
                player.TriggerEvent("gardener_updateOrder", JsonConvert.SerializeObject(order));
                NAPI.Task.Run(() =>
                {
                    if (player != null && player.Exists)
                        player.TriggerEvent("saveEquipment");
                }, 500);


                if (order.All(o => o == 0))
                {
                    playerDataManager.NotifyPlayer(player, "Pomyślnie skompletowano zamówienie!");
                    payoutManager.GardenerOrderCompleted(player, baseOrder);
                    player.TriggerEvent("closeGardenerHUDBrowser");
                    player.TriggerEvent("openGardenerOrdersBrowser", gardener.GetOrders());
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie posiadasz żadnych roślin do oddania!");
            }


        }
    }
}

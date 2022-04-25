using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("buyFishingRod")]
        public void BuyFishingRod(Player player)
        {
            fisherMan.BuyFishingRod(player);
        }

        [RemoteEvent("startFishing")]
        public void StartFishing(Player player)
        {
            fisherMan.StartJob(player);
        }

        [RemoteEvent("fishingDone")]
        public void FishingDone(Player player, int size, int type)
        {
            fisherMan.Done(player, size, type);
        }

        [RemoteEvent("sellFishes")]
        public void SellFishes(Player player)
        {
            List<string> fishes = new List<string>();
            string eq = player.GetSharedData<string>("equipment");
            List<Dictionary<string, string>> items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(eq);
            foreach (Dictionary<string, string> item in items)
            {
                switch (Int32.Parse(item["typeID"]))
                {
                    case 1001:
                    case 1002:
                    case 1003:
                    case 1004:
                    case 1005:
                    case 1006:
                    case 1007:
                    case 1008:
                    case 1009:
                    case 1010:
                    case 1011:
                    case 1012:
                    case 1013:
                    case 1014:
                    case 1015:
                    case 1016:
                    case 1017:
                    case 1018:
                    case 1019:
                    case 1020:
                    case 1021:
                    case 1022:
                        fishes.Add(item["typeID"]);
                        player.TriggerEvent("removeEqItem", Int32.Parse(item["id"]));
                        break;
                }
            }
            if (fishes.Count == 0)
            {
                playerDataManager.NotifyPlayer(player, "Nie masz żadnych ryb do sprzedania!");
            }
            else
            {
                payoutManager.FisherSold(player, fishes);
                NAPI.Task.Run(() =>
                {
                    if (player != null && player.Exists)
                        player.TriggerEvent("saveEquipment");
                }, 500);
            }
        }

        [RemoteEvent("sellJunk")]
        public void SellJunk(Player player)
        {
            List<string> junks = new List<string>();
            string eq = player.GetSharedData<string>("equipment");
            if (eq != "[]")
            {
                List<Dictionary<string, string>> items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(eq);
                foreach (Dictionary<string, string> item in items)
                {
                    switch (Int32.Parse(item["typeID"]))
                    {
                        case 1050:
                        case 1051:
                        case 1052:
                        case 1053:
                        case 1054:
                        case 1055:
                        case 1056:
                        case 1057:
                        case 1058:
                            junks.Add(item["typeID"]);
                            player.TriggerEvent("removeEqItem", Int32.Parse(item["id"]));
                            break;
                    }
                }
                if (junks.Count == 0)
                {
                    playerDataManager.NotifyPlayer(player, "Nie masz żadnych przedmiotów do sprzedania!");
                }
                else
                {
                    payoutManager.FisherJunkSold(player, junks);
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie masz żadnych przedmiotów do sprzedania!");
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("collectiblePickedUp")]
        public void CollectiblePickedUp(Player player, int id)
        {
            if (!CollectibleManager.IsCollectiblePickedUp(player, id))
            {
                System.Object[] vals = CollectibleManager.RemovePlayersCollectible(player, id);
                Dictionary<int, bool> collectible = (Dictionary<int, bool>)vals[0];
                int found = (int)vals[1];
                PlayerDataManager.UpdatePlayersCollectibles(player, JsonConvert.SerializeObject(collectible));
                PlayerDataManager.NotifyPlayer(player, $"Znalazłeś jajo {found} z {CollectibleManager.collectibleCount}!");
                int money = 10000 * (PlayerDataManager.GetPlayersCollectiblesAmount(player) + 1);
                PlayerDataManager.UpdatePlayersMoney(player, money);
                PlayerDataManager.UpdatePlayersExp(player, 1000);
            }
            else
            {
                Console.WriteLine("podwójna znajdźka: " + player.SocialClubId.ToString() + " " + id.ToString());
            }
        }
    }
}

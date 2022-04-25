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
            if (!collectibleManager.IsCollectiblePickedUp(player, id))
            {
                System.Object[] vals = collectibleManager.RemovePlayersCollectible(player, id);
                Dictionary<int, bool> collectible = (Dictionary<int, bool>)vals[0];
                int found = (int)vals[1];
                playerDataManager.UpdatePlayersCollectibles(player, JsonConvert.SerializeObject(collectible));
                playerDataManager.NotifyPlayer(player, $"Znalazłeś jajo {found} z {collectibleManager.collectibleCount}!");
                playerDataManager.UpdatePlayersMoney(player, 100);
                playerDataManager.UpdatePlayersExp(player, 100);
            }
            else
            {
                Console.WriteLine("podwójna znajdźka: " + player.SocialClubId.ToString() + " " + id.ToString());
            }
        }
    }
}

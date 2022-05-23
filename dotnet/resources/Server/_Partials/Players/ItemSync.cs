using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {

        [RemoteEvent("putItemInHand")]
        public void PutItemInHand(Player player, string item)
        {
            if (player.HasSharedData("handObj") && player.GetSharedData<string>("handObj") != "")
            {
                player.SetSharedData("handObj", "");
            }
            else
            {
                player.SetSharedData("handObj", item);
            }
        }
        [RemoteEvent("removeItemFromHand")]
        public void RemoveItemFromHand(Player player)
        {
            if (player.HasSharedData("handObj") && player.GetSharedData<string>("handObj") != "")
            {
                player.SetSharedData("handObj", "");
            }
        }

    }
}

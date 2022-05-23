using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("itemShopBuyItem")]
        public void ItemShopBuyItem(Player player, int itemId)
        {
            player.TriggerEvent("checkIfItemFits", itemId, "shop");
        }
    }
}

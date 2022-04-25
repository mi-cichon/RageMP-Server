using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("afk_setAFK")]
        public void AFK_setAFK(Player player, bool state)
        {
            if(player.Exists)
            {
                player.SetSharedData("afk", state);
            }
        }
    }
}

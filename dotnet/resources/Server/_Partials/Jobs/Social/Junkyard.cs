using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("startJunkyard")]
        public void StartJunkyard(Player player)
        {
            //junkyard.startJob(player);
        }

        [RemoteEvent("junkDelievered")]
        public void JunkDelievered(Player player)
        {
            //payoutManager.JunkyardPayment(player);
        }
    }
}

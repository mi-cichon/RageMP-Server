using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("startDebrisCleaner")]
        public void StartDebrisCleaner(Player player)
        {
            debrisCleaner.startJob(player);
        }

        [RemoteEvent("debrisCleanerReward")]
        public void DebrisCleanerReward(Player player, int weight)
        {
            payoutManager.DebrisCleanerPayment(player, weight);
        }
    }
}

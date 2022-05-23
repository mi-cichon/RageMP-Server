using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("startLawnmowing")]
        public void StartLawnmowing(Player player)
        {
            lawnmowing.startJob(player);
        }

        [RemoteEvent("freezeLawnmower")]
        public void FreezeLawnmower(Player player, Vehicle vehicle, bool val)
        {
            vehicle.SetSharedData("veh_brake", val);
        }

        [RemoteEvent("lawnmowingReward")]
        public void LawnmowingReward(Player player)
        {
            payoutManager.LawnmowingPayment(player);
        }
    }
}

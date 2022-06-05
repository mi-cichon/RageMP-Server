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
        public void LawnmowingReward(Player player, float amount)
        {
            payoutManager.LawnmowingPayment(player, amount);
        }
        [RemoteEvent("lawnmowingRemoveGrass")]
        public void LawnmowingRemoveGrass(Player player, int grassID)
        {
            if(lawnmowing.grassObjects[grassID] != null)
            {
                var grass = lawnmowing.grassObjects[grassID];
                if (grass.Destroy())
                {
                    Random rnd = new Random();
                    player.TriggerEvent("lawnmowingGrassRemoved", rnd.Next(3,11));
                }
            }
        }
    }
}

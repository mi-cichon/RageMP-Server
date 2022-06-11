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
            Lawnmowing.startJob(player);
        }

        [RemoteEvent("freezeLawnmower")]
        public void FreezeLawnmower(Player player, Vehicle vehicle, bool val)
        {
            vehicle.SetSharedData("veh_brake", val);
        }

        [RemoteEvent("lawnmowingReward")]
        public void LawnmowingReward(Player player, float amount)
        {
            PayoutManager.LawnmowingPayment(player, amount);
        }
        [RemoteEvent("lawnmowingRemoveGrass")]
        public void LawnmowingRemoveGrass(Player player, int objId)
        {
            var grassList = Lawnmowing.grassObjects.FindAll(grass => grass.grassObj.Id == objId);
            var grass = grassList.Count > 0 ? grassList[0] : null;
            if (grass != null && grass.Destroy())
            {
                Random rnd = new Random();
                player.TriggerEvent("lawnmowingGrassRemoved", rnd.Next(3,11));
            }
        }
    }
}

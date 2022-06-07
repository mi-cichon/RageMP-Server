using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("startForklifts")]
        public void StartForklifts(Player player)
        {
            Forklifts.StartJob(player);
        }

        [RemoteEvent("forkliftBoxPickedUp")]
        public void ForkliftBoxPickedUp(Player player, GTANetworkAPI.Object obj)
        {
            if (obj.HasSharedData("posID"))
            {
                Forklifts.CreateNewBox(obj.GetSharedData<Int32>("posID"));
            }
            obj.Delete();
        }

        [RemoteEvent("forkliftsBoxDropped")]
        public void ForkliftsBoxDropped(Player player, bool special)
        {
            PayoutManager.ForkliftsPayment(player, special);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("startSupplier")]
        public void StartSupplier(Player player)
        {
            //supplier.startJob(player);
        }
    }
}

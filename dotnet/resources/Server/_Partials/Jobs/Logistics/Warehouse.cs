using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("startWarehouse")]
        public void StartWareHouse(Player player)
        {
            warehouse.startJob(player);
        }

        [RemoteEvent("warehouseBoxDelievered")]
        public void WarehouseBoxDelievered(Player player)
        {
            payoutManager.WarehousePayment(player);
            warehouse.CreateBox();
        }
    }
}

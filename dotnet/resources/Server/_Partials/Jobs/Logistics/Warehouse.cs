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
            Warehouse.startJob(player);
        }

        [RemoteEvent("warehouseBoxDelievered")]
        public void WarehouseBoxDelievered(Player player)
        {
            PayoutManager.WarehousePayment(player);
            Warehouse.CreateBox();
        }
    }
}

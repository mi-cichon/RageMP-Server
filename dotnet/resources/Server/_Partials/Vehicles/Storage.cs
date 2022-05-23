using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("spawnSelectedVehicle")]
        public void spawnSelectedVehicle(Player player, string vehicleId, string storageId)
        {
            Vehicle veh = null;
            foreach (VehicleStorage vs in vehicleStorages)
            {
                if (vs.storageId == Convert.ToInt32(storageId))
                {
                    veh = vs.SpawnCar(player, vehicleId);
                    break;
                }
            }
            if (veh != null)
            {
                orgManager.SetVehiclesOrg(veh);
            }
        }

        [RemoteEvent("requestStorageData")]
        public void RequestStorageData(Player player)
        {
            List<int> orgIds = null;
            if (player.HasSharedData("orgId") && player.GetSharedData<Int32>("orgId") != 0)
            {
                foreach (Organization org in orgManager.orgs)
                {
                    if (org.id == player.GetSharedData<Int32>("orgId"))
                    {
                        orgIds = org.vehicles;
                        break;
                    }
                }
            }
            string vehicles = vehicleDataManager.GetPlayersVehicles(player, true, orgIds);
            player.TriggerEvent("insertStorageVehicles", vehicles);
        }

        private void CreateVehicleStorages()
        {
            VehicleStorage vs = new VehicleStorage(1, new Vector3(1879.1293f, 3760.452f, 33.062183f), 7.0f, new Vector3(1872.2902f, 3757.814f, 33.06228f));
            vs.AddSpawningPoint(new Vector3(1882.7478f, 3760.1377f, 33.06218f), -150f);
            vs.AddSpawningPoint(new Vector3(1877.7385f, 3757.1594f, 33.06218f), -150f);
            vehicleStorages.Add(vs);
            VehicleStorage vsls = new VehicleStorage(2, new Vector3(137.94495f, -1083.2755f, 29.19444f), 7.0f, new Vector3(137.32211f, -1085.9031f, 29.19237f));
            vsls.AddSpawningPoint(new Vector3(132.21432f, -1081.7181f, 29.226038f), 2f);
            vsls.AddSpawningPoint(new Vector3(136.09067f, -1082.0425f, 29.222183f), 2f);
            vsls.AddSpawningPoint(new Vector3(139.804f, -1082.0377f, 29.226038f), 2f);
            vsls.AddSpawningPoint(new Vector3(143.53342f, -1082.0083f, 29.226038f), 2f);
            vehicleStorages.Add(vsls);
            VehicleStorage vspaleto = new VehicleStorage(3, new Vector3(-72.57493f, 6425.9814f, 31.439983f), 5.0f, new Vector3(-70.828255f, 6424.371f, 31.439854f));
            vspaleto.AddSpawningPoint(new Vector3(-69.98739f, 6427.8022f, 31.4392f), 40f);
            vspaleto.AddSpawningPoint(new Vector3(-74.2419f, 6423.5034f, 31.490437f), 40f);
            vehicleStorages.Add(vspaleto);
        }
    }
}

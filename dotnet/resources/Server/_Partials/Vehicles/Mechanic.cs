using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("openMechHUD")]
        public void openMechHUD(Player player, ColShape colshape)
        {
            Vehicle stationVehicle = null;
            foreach (VehicleMechanic vm in vehicleMechanics)
            {
                if (vm.pedColShape == colshape)
                {
                    foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
                    {
                        if (vm.stationColShape.IsPointWithin(veh.Position))
                        {
                            stationVehicle = veh;
                            break;
                        }
                    }
                    if (stationVehicle != null)
                    {
                        if (stationVehicle.HasSharedData("mech") && stationVehicle.GetSharedData<bool>("mech"))
                        {
                            playerDataManager.NotifyPlayer(player, "Pojazd jest już w trakcie naprawy!");
                        }
                        else
                        {
                            player.TriggerEvent("openMechHUD", stationVehicle, vehicleDataManager.GetVehicleModelPrice(stationVehicle), vm.stationColShape);
                        }
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Na stanowisku nie ma żadnego pojazdu!");
                    }
                    break;
                }
            }
        }

        [RemoteEvent("mech_confirmRepair")]
        public void Mech_ConfirmRepair(Player player, Vehicle vehicle, int price, int time, string partsToRepair, ColShape mechColshape)
        {
            if (vehicle != null && vehicle.Exists)
            {
                foreach (VehicleMechanic vehMech in vehicleMechanics)
                {
                    if (vehMech.stationColShape == mechColshape)
                    {
                        if (!vehMech.stationColShape.IsPointWithin(vehicle.Position))
                        {
                            playerDataManager.NotifyPlayer(player, "Pojazd nie znajduje się na stanowisku!");
                            return;
                        }
                    }
                }
                if (vehicle.HasSharedData("mech") && vehicle.GetSharedData<bool>("mech"))
                {
                    playerDataManager.NotifyPlayer(player, "Pojazd jest już w trakcie naprawy!");
                }
                else
                {
                    if (playerDataManager.UpdatePlayersMoney(player, -1 * price))
                    {
                        playerDataManager.NotifyPlayer(player, "Naprawa rozpoczęta!");
                        vehicleDataManager.setRepairingInterval(vehicle, time, player, partsToRepair);
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie stać cię na naprawę tego pojazdu!");
                    }
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }

        }

        private void CreateVehicleMechanics()
        {
            VehicleMechanic vm = new VehicleMechanic(new Vector3(1566.1263f, 3792.468f, 34.645317f), 100f, new Vector3(1560.1971f, 3793.4932f, 34.716663f), true, "poor");
            vehicleMechanics.Add(vm);
            VehicleMechanic vmls1 = new VehicleMechanic(new Vector3(-219.53825f, -1173.263f, 23.615189f), 45f, new Vector3(-223.8401f, -1173.0829f, 23.260616f), false, "rich");
            vehicleMechanics.Add(vmls1);
            VehicleMechanic vmls2 = new VehicleMechanic(new Vector3(-227.4387f, -1173.3578f, 23.615189f), 45f, new Vector3(-231.78595f, -1172.9685f, 23.258411f), true, "rich");
            vehicleMechanics.Add(vmls2);
            VehicleMechanic vmls3 = new VehicleMechanic(new Vector3(-235.41434f, -1173.3915f, 23.615189f), 45f, new Vector3(-239.52097f, -1172.9838f, 23.267845f), false, "rich");
            vehicleMechanics.Add(vmls3);

            VehicleMechanic vmpaleto1 = new VehicleMechanic(new Vector3(-285.28976f, 6049.638f, 31.694714f), 75f, new Vector3(-288.6118f, 6046.74f, 31.7553f), true, "rich");
            vehicleMechanics.Add(vmpaleto1);

            VehicleMechanic vmpaleto2 = new VehicleMechanic(new Vector3(-290.86105f, 6044.035f, 31.694653f), 75f, new Vector3(-294.0558f, 6041.2373f, 31.754717f), false, "rich");
            vehicleMechanics.Add(vmpaleto2);
        }
    }
}

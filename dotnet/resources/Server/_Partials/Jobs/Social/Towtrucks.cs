using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("createTowObject")]
        public void CreateTowObject(Player player, Vehicle towtruck, string type)
        {
            switch (type)
            {
                case "car":
                    towtruck.SetSharedData("towingobj", "imp_prop_covered_vehicle_03a");
                    break;
                default:
                    towtruck.SetSharedData("towingobj", type);
                    break;
            }
        }
        [RemoteEvent("removeTowedVehicle")]
        public void RemoveTowedVehicle(Player player, Vehicle vehicle)
        {
            if (vehicle != null)
            {
                if (vehicle.HasSharedData("market") && vehicle.GetSharedData<bool>("market"))
                {
                    //carMarket.RemoveVehicleFromMarket(vehicle);
                }
                vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                vehicle.Delete();
            }
        }
        [RemoteEvent("markVehicleAsNotTowed")]
        public void MarkVehicleAsNotTowed(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
                vehicle.SetSharedData("towed", false);
        }

        [RemoteEvent("vehicleTowed")]
        public void VehicleTowed(Player player, string type, float distance, float dmg)
        {
            payoutManager.TowtruckPayment(player, type, distance, dmg);
            player.Vehicle.SetSharedData("towingobj", "");
        }

        [RemoteEvent("startTowTrucks")]
        public void StartTowTrucks(Player player)
        {
            towTruck.startJob(player);
        }

        [RemoteEvent("getVehicleToTow")]
        public void GetVehicleToTow(Player player)
        {
            Vehicle veh = null;
            if (player.GetSharedData<bool>("jobBonus_27"))
            {
                veh = vehicleDataManager.GetRandomVehicleToTow();
            }
            player.TriggerEvent("setVehicleToTow", veh);
        }
    }
}

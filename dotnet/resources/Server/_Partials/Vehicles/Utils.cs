using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("freezePublicVehicle")]
        public void FreezePublicVehicle(Player player, Vehicle vehicle, bool value)
        {
            vehicle.SetSharedData("veh_brake", value);

        }

        //DAMAGE
        [RemoteEvent("updateVehicleDamage")]
        public void UpdateVehicleDamage(Player player, Vehicle vehicle, string damageString)
        {
            vehicleDataManager.UpdateVehiclesDamage(vehicle, damageString);
        }

        //horn
        [RemoteEvent("setVehicleHorn")]
        public void SetVehicleHorn(Player player, Vehicle vehicle, bool state)
        {
            if (vehicle != null && vehicle.Exists)
                vehicle.SetSharedData("horn", state);
        }

        //dirt level
        [RemoteEvent("updateVehiclesDirtLevel")]
        public void updateVehiclesDirtLevel(Player player, Vehicle vehicle, float dirtLevel)
        {
            if (vehicle != null && vehicle.Exists)
                vehicleDataManager.UpdateVehiclesDirtLevel(vehicle, dirtLevel);
        }

        //drowned veh
        [RemoteEvent("removeDrowningVehicle")]
        public void RemoveDrowningVehicle(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                vehicleDataManager.UpdateVehiclesDamage(vehicle, vehicleDataManager.wreckedDamage);
                vehicle.Delete();
            }
            if (player != null && player.Exists)
            {
                playerDataManager.NotifyPlayer(player, "Twój pojazd został zatopiony, możesz go odebrać w przechowalni!");
            }
        }

        [RemoteEvent("freezeJobVeh")]
        public void FreezeJobVeh(Player player, bool state, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                vehicle.SetSharedData("veh_brake", state);
            }
        }

        [RemoteEvent("setIntoVeh")]
        public void SetIntoVeh(Player player, Vehicle vehicle)
        {
            if(vehicle != null && vehicle.Exists && player != null && player.Exists)
            {
                player.SetIntoVehicle(vehicle.Handle, 0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("trip_update")]
        public void Trip_Update(Player player, Vehicle vehicle, float dist)
        {
            if (vehicle != null && vehicle.Exists)
            {
                vehicleDataManager.UpdateVehiclesTrip(vehicle, dist);
            }
        }

        [RemoteEvent("vehc_switchLights")]
        public void VehC_SwitchLights(Player player, bool state)
        {
            if (player.Vehicle != null)
            {
                player.Vehicle.SetSharedData("veh_lights", state);
            }
        }

        [RemoteEvent("vehc_switchEngine")]
        public void VehC_SwitchEngine(Player player, bool state)
        {
            if (player.Vehicle != null)
            {
                player.Vehicle.SetSharedData("veh_engine", state);
            }
        }

        [RemoteEvent("vehc_switchParkingbrake")]
        public void VehC_SwitchParkingbrake(Player player, bool state)
        {
            if (player.Vehicle != null)
            {
                vehicleDataManager.UpdateVehiclesBrake(player.Vehicle, state);
            }
        }

        [RemoteEvent("vehc_switchLocks")]
        public void VehC_SwitchLocks(Player player, bool state)
        {
            if (player.Vehicle != null)
            {
                player.Vehicle.SetSharedData("veh_locked", state);
            }
        }

        [RemoteEvent("vehc_kickPassengers")]
        public void VehC_KickPassengers(Player player)
        {
            if (player.Vehicle != null)
            {
                foreach (Player passenger in player.Vehicle.Occupants)
                {
                    if (passenger != player)
                    {
                        passenger.WarpOutOfVehicle();
                        playerDataManager.NotifyPlayer(passenger, "Zostałeś wyrzucony z pojazdu!");
                    }
                }
            }
        }
        [RemoteEvent("setVehiclesLights")]
        public void setVehiclesLights(Player player, Vehicle vehicle)
        {
            if (!vehicle.HasSharedData("veh_lights"))
            {
                vehicle.SetSharedData("veh_lights", true);
                playerDataManager.NotifyPlayer(player, "Swiatła będą zawsze włączone!");
            }
            else
            {
                if (vehicle.GetSharedData<bool>("veh_lights"))
                {
                    vehicle.SetSharedData("veh_lights", false);
                    playerDataManager.NotifyPlayer(player, "Swiatła będą zawsze wyłączone!");
                }
                else
                {
                    vehicle.SetSharedData("veh_lights", true);
                    playerDataManager.NotifyPlayer(player, "Swiatła będą zawsze włączone!");
                }
            }
        }
    }
}

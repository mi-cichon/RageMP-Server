using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("speedoColor_confirm")]
        public void SpeedoColor_Confirm(Player player, Vehicle vehicle, string color)
        {
            float price = 2000.0f;
            if (vehicle != null && vehicle.Exists)
            {
                if (playerDataManager.UpdatePlayersMoney(player, -1 * Convert.ToInt32(price)))
                {
                    vehicleDataManager.UpdateVehiclesSpeedometer(vehicle, color);
                    playerDataManager.NotifyPlayer(player, "Kolor licznika pomyślnie zmieniony!");
                    player.TriggerEvent("speedometer_setColor", color);
                    player.TriggerEvent("speedoColor_closeBrowser");
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie stać cię na to!");
                }
            }
        }
    }
}

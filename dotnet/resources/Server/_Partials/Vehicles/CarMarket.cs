using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("applyCarToMarket")]
        public void ApplyCarToMarket(Player player, Vehicle vehicle, int price, string description)
        {
            if (vehicle != null && vehicle.Exists && player != null && player.Exists)
            {
                if (carMarket.AddVehicleToMarket(vehicle, price, description, player.GetSharedData<string>("username")))
                {
                    playerDataManager.NotifyPlayer(player, "Pomyślnie wystawiono pojazd na giełdę!");
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Na giełdzie nie ma wolnych miejsc!");
                }
                
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Wystąpił błąd!");
            }
        }
    }
}

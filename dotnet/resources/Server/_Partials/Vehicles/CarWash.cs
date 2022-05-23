using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("startCarWash")]
        public void StartCarWash(Player player, Vehicle vehicle, ColShape colshape)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("washtime") && player.VehicleSeat == 0)
            {
                foreach (CarWash carWash in carWashes)
                {
                    if (carWash.shape == colshape)
                    {
                        if (playerDataManager.UpdatePlayersMoney(player, -100))
                        {
                            carWash.WashCar(player, vehicle);
                            break;
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie stać cię na to!");
                            break;
                        }

                    }
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Pojazd nie należy do Ciebie!");
            }
        }

        public void CreateCarWashes()
        {
            carWashes.Add(new CarWash(new Vector3(-699.83606f, -932.6352f, 19.013899f), new Vector3(-699.97925f, -931.32666f, 21.0139f)));

            carWashes.Add(new CarWash(new Vector3(136.39879f, 6650.799f, 31.89376f), new Vector3(137.90192f, 6649.2163f, 33.893246f)));
            carWashes.Add(new CarWash(new Vector3(19.957819f, -1391.9983f, 29.324995f), new Vector3(21.814413f, -1392.196f, 31.328192f)));
            carWashes.Add(new CarWash(new Vector3(1362.914f, 3592.054f, 34.919933f), new Vector3(1364.7023f, 3592.6375f, 36.910748f)));
        }
    }
}

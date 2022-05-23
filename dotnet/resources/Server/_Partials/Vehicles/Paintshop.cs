using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("showVehicleColor")]
        public void ShowVehicleColor(Player player, Vehicle vehicle, string color1string, string color2string, string defaultMods)
        {
            if (color1string != "" && color2string != "" && defaultMods == "")
            {
                int[] color1 = vehicleDataManager.JsonToColor(color1string);
                int colorMod1 = vehicleDataManager.JsonToColorMod(color1string);
                int[] color2 = vehicleDataManager.JsonToColor(color2string);
                int colorMod2 = vehicleDataManager.JsonToColorMod(color2string);
                NAPI.Vehicle.SetVehicleCustomPrimaryColor(vehicle.Handle, color1[0], color1[1], color1[2]);
                NAPI.Vehicle.SetVehicleCustomSecondaryColor(vehicle.Handle, color2[0], color2[1], color2[2]);
                vehicle.SetSharedData("color1mod", colorMod1);
                vehicle.SetSharedData("color2mod", colorMod2);
            }
            else
            {
                try
                {
                    int[] color1 = vehicleDataManager.JsonToColor(vehicle.GetSharedData<string>("color1"));
                    int[] color2 = vehicleDataManager.JsonToColor(vehicle.GetSharedData<string>("color2"));
                    NAPI.Vehicle.SetVehicleCustomPrimaryColor(vehicle.Handle, color1[0], color1[1], color1[2]);
                    NAPI.Vehicle.SetVehicleCustomSecondaryColor(vehicle.Handle, color2[0], color2[1], color2[2]);
                    int[] colorMods = System.Text.Json.JsonSerializer.Deserialize<int[]>(defaultMods);
                    vehicle.SetSharedData("color1mod", colorMods[0]);
                    vehicle.SetSharedData("color2mod", colorMods[1]);

                }
                catch { }
            }
        }

        [RemoteEvent("changeVehicleColor")]
        public void ChangeVehicleColor(Player player, Vehicle vehicle, string color1string, string color2string)
        {
            float price = 3000.0f;
            int[] color1 = vehicleDataManager.JsonToColor(color1string);
            int colorMod1 = vehicleDataManager.JsonToColorMod(color1string);
            int[] color2 = vehicleDataManager.JsonToColor(color2string);
            int colorMod2 = vehicleDataManager.JsonToColorMod(color2string);
            if (playerDataManager.UpdatePlayersMoney(player, -1 * Convert.ToInt32(price)))
            {
                vehicleDataManager.UpdateVehiclesColor1(vehicle, color1[0], color1[1], color1[2], colorMod1);
                vehicleDataManager.UpdateVehiclesColor2(vehicle, color2[0], color2[1], color2[2], colorMod2);
                playerDataManager.NotifyPlayer(player, "Kolor pomyślnie zmieniony!");
                player.TriggerEvent("closePaintShopBrowser");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie stać cię na to!");
            }
        }
        public void CreatePaintShops()
        {
            PaintShop psls = new PaintShop(new Vector3(-327.23065f, -144.6532f, 39.059948f));
        }
    }
}

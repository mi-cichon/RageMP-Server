using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {

        [RemoteEvent("savePlayerSettings")]
        public void SavePlayerSettings(Player player, string set)
        {
            playerDataManager.UpdatePlayersSettings(player, set);
            Settings settings = JsonConvert.DeserializeObject<Settings>(player.GetSharedData<string>("settings"));
            player.TriggerEvent("settings_setSpeedometerScale", settings.SpeedometerSize);
            player.TriggerEvent("settings_setHUDScales", settings.HudSize, settings.ChatSize);
        }

        [RemoteEvent("resetPlayerSettings")]
        public void ResetPlayerSettings(Player player)
        {
            playerDataManager.SetPlayersSettings(player);
            Settings settings = JsonConvert.DeserializeObject<Settings>(player.GetSharedData<string>("settings"));
            player.TriggerEvent("settings_setSpeedometerScale", settings.SpeedometerSize);
            player.TriggerEvent("settings_setHUDScales", settings.HudSize, settings.ChatSize);
        }
    }
}

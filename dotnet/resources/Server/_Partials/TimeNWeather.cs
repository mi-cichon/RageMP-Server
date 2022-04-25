using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        public void TimeHandler(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                secondsPassed++;
                if (secondsPassed == 8)
                {
                    secondsPassed = 0;
                    foreach (Player player in NAPI.Pools.GetAllPlayers())
                    {
                        player.TriggerEvent("setTime", (playerDataManager.time.Hour.ToString().Length == 1 ? ("0" + playerDataManager.time.Hour.ToString()) : playerDataManager.time.Hour.ToString()) + ":" + (playerDataManager.time.Minute.ToString().Length == 1 ? ("0" + playerDataManager.time.Minute.ToString()) : playerDataManager.time.Minute.ToString()));
                    }
                }
                playerDataManager.time = playerDataManager.time.AddSeconds(8);
                NAPI.World.SetTime(playerDataManager.time.Hour, playerDataManager.time.Minute, playerDataManager.time.Second);

            });
        }

        public void WeatherHandler(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                Random rnd = new Random();
                int chance = rnd.Next(0, 30);
                string weather = currentWeather;
                if (chance == 0)
                {
                    weather = "THUNDER";
                }
                else if (chance == 1)
                {
                    weather = "RAIN";
                }
                else if (chance == 2 || chance == 3 || chance == 4)
                {
                    weather = "CLOUDS";
                }
                else if (chance == 5 || chance == 6)
                {
                    weather = "FOGGY";
                }
                else
                {
                    weather = "EXTRASUNNY";
                }
                if (currentWeather != weather)
                {
                    currentWeather = weather;
                    foreach (Player player in NAPI.Pools.GetAllPlayers())
                    {
                        player.TriggerEvent("setWeather", currentWeather, true);
                    }
                }
            });
        }
    }
}

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
                        player.TriggerEvent("setTime", (PlayerDataManager.time.Hour.ToString().Length == 1 ? ("0" + PlayerDataManager.time.Hour.ToString()) : PlayerDataManager.time.Hour.ToString()) + ":" + (PlayerDataManager.time.Minute.ToString().Length == 1 ? ("0" + PlayerDataManager.time.Minute.ToString()) : PlayerDataManager.time.Minute.ToString()));
                    }
                }
                PlayerDataManager.time = PlayerDataManager.time.AddSeconds(8);
                NAPI.World.SetTime(PlayerDataManager.time.Hour, PlayerDataManager.time.Minute, PlayerDataManager.time.Second);

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

using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("startDebrisCleaner")]
        public void StartDebrisCleaner(Player player)
        {
            debrisCleaner.startJob(player);
        }

        [RemoteEvent("debrisCleanerReward")]
        public void DebrisCleanerReward(Player player, int weight)
        {
            payoutManager.DebrisCleanerPayment(player, weight);
        }

        [RemoteEvent("debrisCleaner_luckCheck")]
        public void DebrisCleaner_LuckCheck(Player player)
        {
            int luck = 0;

            if (player.GetSharedData<bool>("jobBonus_52"))
            {
                luck = 10;
            }
            else if (player.GetSharedData<bool>("jobBonus_51"))
            {
                luck = 20;
            }
            if (luck != 0)
            {
                Random rand = new Random();
                int r = rand.Next(0, luck);
                if (r == 0)
                {
                    int itemId = rand.Next(1050, 1059);
                    player.TriggerEvent("fitItemInEquipment", itemId);
                    playerDataManager.NotifyPlayer(player, "Udało cię się odnaleźć przedmiot!");

                }
            }
        }
    }
}

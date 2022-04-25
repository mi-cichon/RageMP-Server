using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("bonus_reward")]
        public void Bonus_Reward(Player player, int rewardId)
        {
            Random rand = new Random();
            switch (rewardId)
            {
                case 0:
                    playerDataManager.GiveMoney(player, 100);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś 100$!");
                    break;
                case 1:
                    playerDataManager.GiveMoney(player, 350);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś 350$!");
                    break;
                case 2:
                    playerDataManager.GiveMoney(player, 500);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś 500$!");
                    break;

                case 3:
                    playerDataManager.UpdatePlayersExp(player, 50);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś 50 EXP!");
                    break;
                case 4:
                    playerDataManager.UpdatePlayersExp(player, 200);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś 200 EXP!");
                    break;
                case 5:
                    playerDataManager.UpdatePlayersExp(player, 500);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś 500 EXP!");
                    break;

                case 6:
                    var fishes = new int[] { 1001, 1022 };
                    player.TriggerEvent("addItemToEquipment", rand.Next(fishes[0], fishes[1] + 1));
                    playerDataManager.NotifyPlayer(player, "Wygrałeś rybę!");
                    break;
                case 7:
                    var junk = new int[] { 1050, 1058 };
                    player.TriggerEvent("addItemToEquipment", rand.Next(junk[0], junk[1] + 1));
                    playerDataManager.NotifyPlayer(player, "Wygrałeś przedmiot!");
                    break;

                case 8:
                    player.TriggerEvent("addItemToEquipment", 2);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś jabłko!");
                    break;
                case 9:
                    player.TriggerEvent("addItemToEquipment", 1);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś chleb!");
                    break;
                case 10:
                    player.TriggerEvent("addItemToEquipment", 0);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś wodę!");
                    break;
                case 11:
                    player.TriggerEvent("addItemToEquipment", 12);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś nitro!");
                    break;

                case 12:
                    playerDataManager.UpdatePlayersSkillPoints(player, player.GetSharedData<int>("skillpoints") + 1);
                    playerDataManager.NotifyPlayer(player, "Wygrałeś punkt umiejętności!");
                    break;

                case 13:
                    vehicleDataManager.GiveSpecialVehicleToPlayer(player, "bonus");
                    playerDataManager.NotifyPlayer(player, "Wygrałeś bonusowy pojazd!");
                    break;

            }
        }
    }
}

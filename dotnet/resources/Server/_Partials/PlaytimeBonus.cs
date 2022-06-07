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

            player.SetSharedData("bonustime", 0);
            Random rand = new Random();
            switch (rewardId)
            {
                case 0:
                    PlayerDataManager.GiveMoney(player, 100);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś 100$!");
                    break;
                case 1:
                    PlayerDataManager.GiveMoney(player, 350);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś 350$!");
                    break;
                case 2:
                    PlayerDataManager.GiveMoney(player, 500);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś 500$!");
                    break;

                case 3:
                    PlayerDataManager.UpdatePlayersExp(player, 50);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś 50 EXP!");
                    break;
                case 4:
                    PlayerDataManager.UpdatePlayersExp(player, 200);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś 200 EXP!");
                    break;
                case 5:
                    PlayerDataManager.UpdatePlayersExp(player, 500);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś 500 EXP!");
                    break;

                case 6:
                    var fishes = new int[] { 1001, 1022 };
                    player.TriggerEvent("addItemToEquipment", rand.Next(fishes[0], fishes[1] + 1));
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś rybę!");
                    break;
                case 7:
                    var junk = new int[] { 1050, 1058 };
                    player.TriggerEvent("addItemToEquipment", rand.Next(junk[0], junk[1] + 1));
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś przedmiot!");
                    break;

                case 8:
                    player.TriggerEvent("addItemToEquipment", 2);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś jabłko!");
                    break;
                case 9:
                    player.TriggerEvent("addItemToEquipment", 1);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś chleb!");
                    break;
                case 10:
                    player.TriggerEvent("addItemToEquipment", 0);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś wodę!");
                    break;
                case 11:
                    player.TriggerEvent("addItemToEquipment", 12);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś nitro!");
                    break;

                case 12:
                    PlayerDataManager.UpdatePlayersSkillPoints(player, player.GetSharedData<int>("skillpoints") + 1);
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś punkt umiejętności!");
                    break;

                case 13:
                    VehicleDataManager.GiveSpecialVehicleToPlayer(player, "bonus");
                    PlayerDataManager.NotifyPlayer(player, "Wygrałeś bonusowy pojazd!");
                    break;

            }
        }
    }
}

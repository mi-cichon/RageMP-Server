using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {
        //[RemoteEvent("mainPanel_requestData")]
        //public void MainPanel_requestData(Player player)
        //{
        //    string vehiclesData = vehicleDataManager.GetPlayersVehicles(player);
        //    string playersData = playerDataManager.GetPlayersInfo(player);
        //    string settingsData = player.GetSharedData<string>("settings");
        //    string skillsData = playerDataManager.GetPlayersSkills(player);
        //    player.TriggerEvent("mainPanel_setData", playersData, skillsData, vehiclesData, settingsData, (playerDataManager.time.Hour.ToString().Length == 1 ? ("0" + playerDataManager.time.Hour.ToString()) : playerDataManager.time.Hour.ToString()) + ":" + (playerDataManager.time.Minute.ToString().Length == 1 ? ("0" + playerDataManager.time.Minute.ToString()) : playerDataManager.time.Minute.ToString()));
        //}

        [RemoteEvent("mainPanel_requestStatsGeneralData")]
        public void MainPanel_requestStatsGeneralData(Player player)
        {
            var playerInfo = playerDataManager.GetPlayersInfo(player);
            player.TriggerEvent("mainPanel_setStatsGeneralData", playerInfo);
        }

        [RemoteEvent("mainPanel_requestStatsJobData")]
        public void MainPanel_requestStatsJobData(Player player)
        {
            var jobInfo = progressManager.GetPlayersJobInfo(player);
            player.TriggerEvent("mainPanel_setStatsJobData", jobInfo);
        }

        [RemoteEvent("mainPanel_requestStatsSkillData")]
        public void MainPanel_requestStatsSkillData(Player player)
        {
            var skillsInfo = playerDataManager.GetPlayersSkills(player);
            player.TriggerEvent("mainPanel_setStatsSkillsData", skillsInfo);
        }

        [RemoteEvent("mainPanel_requestVehiclesData")]
        public void MainPanel_requestVehiclesData(Player player)
        {
            player.TriggerEvent("mainPanel_setVehiclesData", vehicleDataManager.GetPlayersVehicles(player));
        }

        [RemoteEvent("mainPanel_requestVehicleData")]
        public void MainPanel_requestVehicleData(Player player, int id)
        {
            string[] vehicleData = vehicleDataManager.GatherVehiclesInfo(id);
            player.TriggerEvent("mainPanel_setVehicleData", vehicleData[0], vehicleData[1], vehicleData[2]);
        }

        [RemoteEvent("mainPanel_addSkillPoint")]
        public void MainPanel_AddSkillPoint(Player player, int skill)
        {
            playerDataManager.UpgradePlayersSkill(player, skill);
            int[] skills = new int[5];
            for (int i = 0; i < 5; i++)
            {
                skills[i] = player.GetSharedData<Int32>("skill-" + i.ToString());
            }
            player.TriggerEvent("mainPanel_setSkillsToUpgrade", player.GetSharedData<Int32>("skillpoints"), JsonConvert.SerializeObject(skills));
        }

        [RemoteEvent("mainPanel_requestBankingData")]
        public void MainPanel_requestBankingData(Player player)
        {
            string[] data = playerDataManager.GetPlayersBankingData(player);
            player.TriggerEvent("mainPanel_setBankingData", data[0], data[1]);
        }

        [RemoteEvent("mainPanel_requestMoneyTransfer")]
        public void MainPanel_requestMoneyTransfer(Player player, string target, int amount, string desc)
        {
            string login = playerDataManager.GetPlayersIDByAccNumber(target);
            if(login != "")
            {
                if(login != player.SocialClubId.ToString())
                {
                    if (playerDataManager.UpdatePlayersBankMoney(player, -1 * amount))
                    {
                        foreach (Player p in NAPI.Pools.GetAllPlayers())
                        {
                            if (p.SocialClubId.ToString() == login)
                            {
                                playerDataManager.SendInfoToPlayer(p, "Otrzymano przelew od " + player.GetSharedData<string>("username") + " o kwocie $" + amount.ToString() + ". Tytuł: " + desc + ".");
                                playerDataManager.UpdatePlayersBankMoney(p, amount);
                                playerDataManager.SaveTransferToDB(player, login, amount, desc);
                                player.TriggerEvent("mainPanel_transferCompleted");
                                playerDataManager.NotifyPlayer(player, "Przelew został zarejestrowany w systemie!");
                                return;
                            }
                        }

                        playerDataManager.SaveTransferToDB(player, login, amount, desc);
                        playerDataManager.TransferMoneyToOfflinePlayer(login, amount);
                        player.TriggerEvent("mainPanel_transferCompleted");
                        playerDataManager.NotifyPlayer(player, "Przelew został zarejestrowany w systemie!");

                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Błąd przelewu: nie masz tyle środków na koncie!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Błąd przelewu: nie możesz przelać pieniędzy do siebie!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Błąd przelewu: nie odnaleziono odbiorcy!");
            }
        }
        [RemoteEvent("mainPanel_requestProgressData")]
        public void MainPanel_requestProgressData(Player player)
        {
            string[] progress = progressManager.GetPlayersProgressInfo(player);
            player.TriggerEvent("mainPanel_setProgressData", progress[0], progress[1]);
        }
    }
}

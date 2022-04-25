using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("mainPanel_requestData")]
        public void MainPanel_requestData(Player player)
        {
            string vehiclesData = vehicleDataManager.GetPlayersVehicles(player);
            string playersData = playerDataManager.GetPlayersInfo(player);
            string settingsData = player.GetSharedData<string>("settings");
            string skillsData = playerDataManager.GetPlayersSkills(player);
            player.TriggerEvent("mainPanel_setData", playersData, skillsData, vehiclesData, settingsData, (playerDataManager.time.Hour.ToString().Length == 1 ? ("0" + playerDataManager.time.Hour.ToString()) : playerDataManager.time.Hour.ToString()) + ":" + (playerDataManager.time.Minute.ToString().Length == 1 ? ("0" + playerDataManager.time.Minute.ToString()) : playerDataManager.time.Minute.ToString()));
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
    }
}

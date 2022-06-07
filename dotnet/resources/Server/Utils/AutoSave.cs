using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    public static class AutoSave
    {
        public static Dictionary<string, string> jobData = new Dictionary<string, string>();

        public static Dictionary<string, int> bonusData = new Dictionary<string, int>();


        static List<JobName> jobNames = new List<JobName>()
        {
            new JobName("forklifts", "Wózki widłowe", "PL"),
            new JobName("warehouse", "Magazynier", "PL"),
            new JobName("junkyard", "Złomowisko", "PS"),
            new JobName("refinery", "Rafineria", "PS"),
            new JobName("towtruck", "Lawety", "PS"),
            new JobName("gardener", "Ogrodnik", "PN"),
            new JobName("lawnmowing", "Koszenie trawników", "PN"),
            new JobName("hunter", "Myśliwy", "PN"),
            new JobName("debriscleaner", "Zbieranie odpadów", "W"),
            new JobName("diver", "Nurek", "W")
        };

        public static void SavePlayersBonusData(Player player, int data)
        {
            foreach (KeyValuePair<string, int> playerData in bonusData)
            {
                if (playerData.Key == player.SocialClubId.ToString())
                {
                    bonusData[playerData.Key] = data;
                    return;
                }
            }
            bonusData.Add(player.SocialClubId.ToString(), data);
        }

        public static int GetPlayersBonusData(Player player)
        {
            foreach (KeyValuePair<string, int> playerData in bonusData)
            {
                if (playerData.Key == player.SocialClubId.ToString())
                {
                    return playerData.Value;
                }
            }
            return 0;
        }


        public static void SavePlayersJobData(Player player, string data)
        {
            foreach(KeyValuePair<string, string> playerData in jobData)
            {
                if(playerData.Key == player.SocialClubId.ToString())
                {
                    jobData[playerData.Key] = data;
                    return;
                }
            }
            jobData.Add(player.SocialClubId.ToString(), data);
        }

        public static string GetPlayersJobData(Player player)
        {
            foreach (KeyValuePair<string, string> playerData in jobData)
            {
                if (playerData.Key == player.SocialClubId.ToString())
                {
                    return playerData.Value;
                }
            }

            return "";
        }

        public static void LoadPlayersJob(Player player)
        {
            string data = GetPlayersJobData(player);
            if (data != "")
            {
                List<object> jobData = JsonConvert.DeserializeObject<List<object>>(data);

                player.SetSharedData("job", (string)jobData[0]);
                var jobName = jobNames.Find(job => job.JobType == (string)jobData[0]);
                if(jobName.JobType != "")
                {
                    player.TriggerEvent("startJob", jobName.Name, jobName.PointsName);
                }
                player.TriggerEvent($"saveData_{(string)jobData[0]}_load", data);
            }
        }

        public static void RemovePlayersJobData(Player player)
        {
            foreach (KeyValuePair<string, string> playerData in jobData)
            {
                if (playerData.Key == player.SocialClubId.ToString())
                {
                    jobData.Remove(playerData.Key);
                    break;
                }
            }
        }
    }
}

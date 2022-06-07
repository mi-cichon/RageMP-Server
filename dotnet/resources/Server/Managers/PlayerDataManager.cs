﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using GTANetworkAPI;
using MySqlConnector;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ServerSide
{
    public static class PlayerDataManager
    {
        public static DateTime time = new DateTime(2000, 12, 12, 12, 0, 0);
        readonly static int[] maxSkillLevels = new int[] { 4, 15, 10, 20, 18 };
        readonly static int[] skillCosts = new int[] { 5, 2, 1, 1, 18 };
        static string avatarsPath;
        static List<int> levels = new List<int>()
            {
                0,
                83,
                174,
                276,
                388,
                512,
                650,
                801,
                969,
                1154,
                1358,
                1584,
                1833,
                2107,
                2411,
                2746,
                3115,
                3523,
                3973,
                4470,
                5018,
                5624,
                6291,
                7028,
                7842,
                8740,
                9730,
                10824,
                12031,
                13363,
                14833,
                16456,
                18247,
                20224,
                22406,
                24815,
                27473,
                30408,
                33648,
                37224,
                41171,
                45529,
                50339,
                55649,
                61512,
                67983,
                75127,
                83014,
                91721,
                101333,
                111945,
                123660,
                136594,
                150872,
                166636,
                184040,
                203254,
                224466,
                247886,
                273742,
                302288,
                333804,
                368599,
                407015,
                449428,
                496254,
                547953,
                605032,
                668051,
                737627,
                814445,
                899257,
                992895,
                1096278,
                1210421,
                1336443,
                1475581,
                1629200,
                1798808,
                1986068,
                2192818,
                2421087,
                2673114,
                2951373,
                3258594,
                3597792,
                3972294,
                4385776,
                4842295,
                5346332,
                5902831,
                6517253,
                7195629,
                7944614,
                8771558,
                9684577,
                10692629,
                11805606,
                13034431
        };

        public static void SetPaths()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                avatarsPath = @"D:/xampp/htdocs/avatars/";
            }
            else
            {
                avatarsPath = @"/var/www/html/avatars/";
            }
        }

        public static void SetPlayerDataFromDB(Player player)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM `users` WHERE login = '{player.SocialClubId.ToString()}'";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    reader.Close();
                    RegisterUser(player);
                }
                else
                {
                    player.SetSharedData("id", Convert.ToInt32(player.Id));
                    player.SetSharedData("socialclub", player.SocialClubId.ToString());
                    player.SetSharedData("type", reader.GetString(2));
                    player.SetSharedData("username", reader.GetString(3));
                    player.SetSharedData("character", reader.GetString(4));
                    player.SetSharedData("lastpos", JsonToVector(reader.GetString(5)));
                    player.SetSharedData("money", Convert.ToInt32(reader.GetString(6)));
                    player.SetSharedData("bank", Convert.ToInt32(reader.GetString(7)));
                    player.SetSharedData("exp", reader.GetInt32(8));
                    player.SetSharedData("skillpoints", reader.GetInt32(13));
                    player.SetSharedData("controlsblocked", false);
                    player.SetSharedData("disablecontrols", false);
                    player.SetSharedData("job", "");
                    player.SetSharedData("ping", player.Ping);
                    player.SetSharedData("seatbelt", false);
                    player.SetSharedData("registered", reader.GetString(9));
                    player.SetSharedData("playtime", reader.GetInt32(10));
                    player.SetSharedData("joined", DateTime.Now.ToString());
                    player.SetSharedData("sid", reader.GetInt32(0));
                    player.SetSharedData("gui", false);
                    player.SetSharedData("equipment", reader.GetString(11));
                    player.SetSharedData("skills", reader.GetString(14));
                    player.SetSharedData("clothes", reader.GetString(15));
                    player.SetSharedData("settings", reader.GetString(16));
                    player.SetSharedData("jobveh", -1111);
                    player.SetSharedData("vehsold", reader.GetString(18));
                    player.SetSharedData("pmoff", "");

                    if (reader.GetString(19) == "")
                    {
                        string number = GenerateRandomCardNumber(player.SocialClubId);
                        player.SetSharedData("accnumber", number);
                        SavePlayerDataToDB(player, "accnumber");
                    }
                    else
                    {
                        player.SetSharedData("accnumber", reader.GetString(19));
                    }

                    player.SetSharedData("afk", false);
                    player.SetSharedData("bonustime", 0);
                    if(reader.GetString(17) == "")
                    {
                        GenerateNewAuthCode(player);
                    }
                    else
                    {
                        player.SetSharedData("authcode", reader.GetString(17));
                    }
                    player.SetSharedData("carkeys", -999999);
                    string collectibles = reader.GetString(12);
                    if (collectibles == "")
                    {
                        //collectibles = CollectibleManager.GetRandomCollectibles();
                        collectibles = CollectibleManager.GetStableCollectibles();
                        UpdatePlayersCollectibles(player, collectibles);
                    }
                    player.SetSharedData("collectibles", collectibles);

                    switch (reader.GetString(2))
                    {
                        case "owner":
                            player.SetSharedData("power", 10);
                            break;
                        case "admin":
                            player.SetSharedData("power", 7);
                            break;
                        case "jadmin":
                            player.SetSharedData("power", 6);
                            break;
                        case "smod":
                            player.SetSharedData("power", 5);
                            break;
                        case "mod":
                            player.SetSharedData("power", 4);
                            break;
                        case "jmod":
                            player.SetSharedData("power", 3);
                            break;
                        case "tester":
                            player.SetSharedData("power", 2);
                            break;
                        default:
                            player.SetSharedData("power", 0);
                            break;
                    }
                    LogManager.LogLoginInfo(player.SocialClubId.ToString(), $"Zalogowano z IP: {player.Address}");
                }
            }
            player.Name = player.GetSharedData<string>("username");
            dataBase.connection.Close();
            GetPlayersJobPoints(player);
            GetPlayersLicences(player);
            SetPlayersLevel(player);
            SetPlayersSkills(player);
            SetPlayersClothes(player);
            SetPlayersSettings(player);
            CheckUsersAvatar(player);
            player.SetSharedData("vehslots", player.GetSharedData<Int32>("skill-3") + 3);
            player.TriggerEvent("instantiateCollectibles");
            //RandomizePlayersCharacter(player);
        }
        private static void RegisterUser(Player player)
        {   
            string startingEq = "[]";
            DBConnection dataBase = new DBConnection();
            string defaultSettings = "{\"hudSize\":50,\"chatSize\":50,\"speedometerSize\":50,\"displayNick\":true,\"displayGlobal\":true,\"voiceChat\":false,\"voiceKey\":88,\"useEmojis\":true}";
            string startSkills = "{\"0\":0,\"1\":0,\"2\":0,\"3\":0,\"4\":0}";
            dataBase.command.CommandText = $"INSERT INTO users (login, type, username, `character`, lastpos, money, bank, exp, registered, playtime, equipment, collectibles, skills, clothes, settings, accnumber) VALUES ('{player.SocialClubId}', 'user', '{player.SocialClubName}', '', '[1894.2115, 3715.0637, 32.762226]', 0, 0, 0, '{DateTime.Now.ToString()}', 0, '{startingEq}', '{CollectibleManager.GetRandomCollectibles()}', '{startSkills}', '', '{defaultSettings}', '{GenerateRandomCardNumber(player.SocialClubId)}')";
            dataBase.command.ExecuteNonQuery();
            dataBase.command.CommandText = $"INSERT INTO penalties (login, ban, mute, drivinglicence) VALUES ('{player.SocialClubId}', '', '', '')";
            dataBase.command.ExecuteNonQuery();
            dataBase.command.CommandText = $"INSERT INTO jobs (player) VALUES ('{player.SocialClubId}')";
            dataBase.command.ExecuteNonQuery();
            dataBase.command.CommandText = $"INSERT INTO licences (bt, bp) VALUES ('False', 'False')";
            dataBase.command.ExecuteNonQuery();
            LogManager.LogLoginInfo(player.SocialClubId.ToString(), $"Zarejestrowano gracza");
            SetPlayerDataFromDB(player);
            dataBase.connection.Close();
        }

        public static void SavePlayerDataToDB(Player player, string dataName)
        {
            DBConnection dataBase = new DBConnection();
            switch (dataName)
            {
                case "lastpos":
                    dataBase.command.CommandText = $"UPDATE users SET lastpos = '{VectorToJson(player.GetSharedData<Vector3>("lastpos"))}' WHERE login = '{player.SocialClubId}'";
                    break;
                case "money":
                    dataBase.command.CommandText = $"UPDATE users SET money = '{player.GetSharedData<Int32>("money").ToString()}' WHERE login = '{player.SocialClubId}'";
                    break;
                case "bank":
                    dataBase.command.CommandText = $"UPDATE users SET bank = '{player.GetSharedData<Int32>("bank").ToString()}' WHERE login = '{player.SocialClubId}'";
                    break;
                case "exp":
                    dataBase.command.CommandText = $"UPDATE users SET exp = {player.GetSharedData<Int32>("exp")} WHERE login = '{player.SocialClubId}'";
                    break;
                case "skillpoints":
                    dataBase.command.CommandText = $"UPDATE users SET skillpoints = {player.GetSharedData<Int32>("skillpoints")} WHERE login = '{player.SocialClubId}'";
                    break;
                case "vehslots":
                    dataBase.command.CommandText = $"UPDATE users SET vehslots = {player.GetSharedData<Int32>("vehslots")} WHERE login = '{player.SocialClubId}'";
                    break;
                case "playtime":
                    dataBase.command.CommandText = $"UPDATE users SET playtime = '{player.GetSharedData<Int32>("playtime")}' WHERE login = '{player.SocialClubId}'";
                    break;
                case "licenceBt":
                case "licenceBp":
                    dataBase.command.CommandText = $"UPDATE licences SET bt = '{player.GetSharedData<bool>("licenceBt").ToString()}', bp = '{player.GetSharedData<bool>("licenceBp").ToString()}' WHERE id = {player.GetSharedData<Int32>("sid")}";
                    break;
                default:
                    dataBase.command.CommandText = $"UPDATE users SET `{dataName}` = '{player.GetSharedData<string>(dataName)}' WHERE login = '{player.SocialClubId}'";
                    break;
            }
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }

        public static Player GetPlayerBySocialId(ulong socialId)
        {
            foreach (Player player in NAPI.Pools.GetAllPlayers())
            {
                try
                {
                    if (player.SocialClubId == socialId)
                    {
                        return player;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static Player GetPlayerByRemoteId(string remoteId)
        {
            Player pl = null;
            int id = -1;
            if (Int32.TryParse(remoteId, out id))
            {
                foreach (Player player in NAPI.Pools.GetAllPlayers())
                {
                    if (player.Id == Convert.ToUInt16(id))
                    {
                        return player;
                    }
                    else if (player.GetSharedData<string>("username").ToLower() == remoteId.ToLower())
                    {
                        pl = player;
                    }
                }
            }
            else
            {
                foreach (Player player in NAPI.Pools.GetAllPlayers())
                {
                    if (player != null && player.Exists && player.HasSharedData("username") && player.GetSharedData<string>("username").ToLower().Contains(remoteId.ToLower()) && pl == null)
                    {
                        pl = player;
                    }
                    else if (player != null && player.Exists && player.HasSharedData("username") && player.GetSharedData<string>("username").ToLower().Contains(remoteId.ToLower()) && pl != null)
                    {
                        return null;
                    }
                }
            }
            return pl;
        }

        public static string GetPlayersSocialByName(string name)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT login FROM `users` WHERE LOWER(username) = '{name}'";
            string social = "";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.Read())
                {
                    if(reader.HasRows)
                    {
                        social = reader.GetString(0);
                    }
                }

            }
            dataBase.connection.Close();
            return social;
        }

        public static int GetPlayersCollectiblesAmount(Player player)
        {
            Dictionary<int, bool> collectibles = JsonConvert.DeserializeObject<Dictionary<int, bool>>(player.GetSharedData<string>("collectibles"));
            int count = 0;
            foreach (KeyValuePair<int, bool> col in collectibles)
            {
                if (col.Value == true)
                {
                    count++;
                }
            }
            return count;
        }
        public static void UpdateVehicleSold(Player player, string vehiclesold)
        {
            player.SetSharedData("vehsold", vehiclesold);
            SavePlayerDataToDB(player, "vehsold");
        }
        public static void UpdatePlayersCollectibles(Player player, string collectiblesString)
        {
            player.SetSharedData("collectibles", collectiblesString);
            SavePlayerDataToDB(player, "collectibles");
        }
        public static void UpdatePlayersEquipment(Player player, string equipmentString)
        {
            player.SetSharedData("equipment", equipmentString);
            SavePlayerDataToDB(player, "equipment");
        }

        public static bool UpdatePlayersMoney(Player player, int money)
        {
            int currentMoney = player.GetSharedData<int>("money");
            if (currentMoney + money < 0.0f)
            {
                return false;
            }
            else
            {
                player.SetSharedData("money", currentMoney + money);
                SavePlayerDataToDB(player, "money");
                SendPlayerDataToMainHUD(player);
                return true;
            }
        }

        public static bool HasPlayerFreeSlot(Player player)
        {
            if (player.GetSharedData<Int32>("skill-3") + 3 > GetPlayersVehiclesCount(player))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DepositMoney(Player player, int amount)
        {
            if(UpdatePlayersMoney(player, -1 * amount))
            {
                UpdatePlayersBankMoney(player, amount);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool WithdrawMoney(Player player, int amount)
        {
            if (UpdatePlayersBankMoney(player, -1 * amount))
            {
                UpdatePlayersMoney(player, amount);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool UpdatePlayersBankMoney(Player player, int money)
        {
            int currentMoney = player.GetSharedData<int>("bank");
            if (currentMoney + money < 0.0f)
            {
                return false;
            }
            else
            {
                player.SetSharedData("bank", currentMoney + money);
                SavePlayerDataToDB(player, "bank");
                return true;
            }
        }
        public static void GiveMoney(Player player, float money)
        {
            int intmoney = Convert.ToInt32(money);
            UpdatePlayersMoney(player, Convert.ToInt32(intmoney));

        }

        public static bool UpdatePlayersExp(Player player, int exp)
        {
            int currentExp = player.GetSharedData<Int32>("exp");
            if (currentExp + exp < 0)
            {
                return false;
            }
            else
            {
                player.SetSharedData("exp", currentExp + exp);
                CheckPlayersLevel(player);
                SavePlayerDataToDB(player, "skillpoints");
                SavePlayerDataToDB(player, "exp");
                SendPlayerDataToMainHUD(player);
                return true;
            }
        }


        public static bool UpdatePlayersNickname(Player player, string nickname)
        {
            if (player.SocialClubName == player.GetSharedData<string>("username"))
            {
                if (Regex.IsMatch(nickname, @"^[a-zA-Z0-9]+$") && nickname.Length <= 18 && nickname.Length >= 4)
                {
                    DBConnection dataBase = new DBConnection();
                    dataBase.command.CommandText = $"SELECT * FROM users WHERE username = '{nickname}'";
                    using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dataBase.connection.Close();
                            return false;
                        }
                        else
                        {
                            player.SetSharedData("username", nickname);
                            SavePlayerDataToDB(player, "username");
                            SendPlayerDataToMainHUD(player);
                            dataBase.connection.Close();
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Regex.IsMatch(nickname, @"^[a-zA-Z0-9]+$") && nickname.Length >= 4 && nickname.Length <= 18 && UpdatePlayersMoney(player, -7500))
                {
                    DBConnection dataBase = new DBConnection();
                    dataBase.command.CommandText = $"SELECT * FROM users WHERE username = '{nickname}'";
                    using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dataBase.connection.Close();
                            return false;
                        }
                        else
                        {
                            player.SetSharedData("username", nickname);
                            SavePlayerDataToDB(player, "username");
                            SendPlayerDataToMainHUD(player);
                            dataBase.connection.Close();
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

        }

        public static void SaveTransferToDB(Player sender, string target, int money, string title)
        {
            string s = sender.GetSharedData<string>("socialclub"), t = target;
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"INSERT INTO transfers (sender, target, amount, title, time) VALUES ('{s}','{t}','{money}','{title}','{DateTime.Now}')";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }
        public static void UpdatePlayersCharacter(Player player, string charStr)
        {
            if (player != null && player.Exists)
            {
                player.SetSharedData("character", charStr);
                SavePlayerDataToDB(player, "character");
            }
        }

        public static void UpdatePlayersPlaytime(Player player, int playtime)
        {
            if (player != null && player.Exists)
            {
                player.SetSharedData("playtime", playtime);
                SavePlayerDataToDB(player, "playtime");
            }

        }
        public static void UpdatePlayersPed(Player player, uint ped)
        {
            player.SetSkin(ped);
            player.SetSharedData("ped", Convert.ToInt64(ped));
            SavePlayerDataToDB(player, "ped");
        }
        public static void UpdatePlayersLastPos(Player player)
        {
            player.SetSharedData("lastpos", player.Position);
            SavePlayerDataToDB(player, "lastpos");
        }

        public static void UpdatePlayersSkillPoints(Player player, int skillpoints)
        {
            player.SetSharedData("skillpoints", skillpoints);
            SavePlayerDataToDB(player, "skillpoints");
        }

        public static void UpgradePlayersSkill(Player player, int skill)
        {
            int sp = player.GetSharedData<Int32>("skillpoints");
            int level = player.GetSharedData<Int32>("skill-" + skill.ToString());
            if (level < maxSkillLevels[skill] && sp >= skillCosts[skill])
            {
                level++;
                sp -= skillCosts[skill];
                UpdatePlayersSkillPoints(player, sp);
                player.SetSharedData("skill-" + skill.ToString(), level);
                UpdatePlayersSkills(player);
                switch (skill)
                {
                    case 0:
                        player.TriggerEvent("changeEquipmentSize", level);
                        break;
                    case 3:
                        player.SetSharedData("vehslots", 3 + level);
                        break;
                }
            }
        }
        public static void UpdatePlayersSkills(Player player)
        {
            Dictionary<int, int> skills = new Dictionary<int, int>();
            for (int i = 0; i < 5; i++)
            {
                skills.Add(i, player.GetSharedData<Int32>("skill-" + i.ToString()));
            }
            player.SetSharedData("skills", JsonConvert.SerializeObject(skills));
            SavePlayerDataToDB(player, "skills");
        }

        public static void SendPlayerDataToMainHUD(Player player)
        {
            int currentLVLEXP = 0;

            for(int i = 0; i<levels.Count; i++)
            {
                if (player.GetSharedData<Int32>("exp") < levels[i])
                {
                    currentLVLEXP = levels[i - 1];
                    break;
                }
            }
            player.TriggerEvent("updateMainHUD", player.GetSharedData<string>("username"), player.GetSharedData<Int32>("money").ToString(), player.GetSharedData<Int32>("level").ToString(), $"{player.GetSharedData<Int32>("exp")-currentLVLEXP}/{player.GetSharedData<Int32>("nextlevel")-currentLVLEXP}");
        }

        public static void NotifyPlayer(Player player, string text)
        {
            player.TriggerEvent("showNotification", text);
        }

        public static void SendGlobalMessage(Player player, string text)
        {
            if(player.HasSharedData("settings_DisplayGlobal") && player.GetSharedData<bool>("settings_DisplayGlobal"))
            {
                LogManager.LogGlobalChatToServer(player.SocialClubId.ToString(), $"{player.GetSharedData<string>("username")}: {text}");
                LogManager.LogGlobalChat(player.SocialClubId.ToString(), $"{text}");
                foreach (Player p in NAPI.Pools.GetAllPlayers())
                {
                    if (p.HasSharedData("settings_DisplayGlobal") && p.GetSharedData<bool>("settings_DisplayGlobal"))
                    {
                        p.TriggerEvent("sendMessage", player.GetSharedData<Int32>("id").ToString(), player.GetSharedData<string>("username"), text, "global", player.GetSharedData<string>("type"), DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], player.SocialClubId.ToString());
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{DateTime.Now.TimeOfDay.ToString().Split(":")[0]}:{DateTime.Now.TimeOfDay.ToString().Split(":")[1]} [{player.GetSharedData<Int32>("id").ToString()}]{player.GetSharedData<string>("username")}: {text}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                NotifyPlayer(player, "Masz wyłączony czat globalny!");
            }
            
        }
        public static void SendMessageToAdmins(Player player, string text)
        {
            foreach (Player p in NAPI.Pools.GetAllPlayers())
            {
                if(p.HasSharedData("power") && p.GetSharedData<int>("power") >= 3)
                {
                    p.TriggerEvent("sendMessage", player.GetSharedData<Int32>("id").ToString(), player.GetSharedData<string>("username"), text, "admin", player.GetSharedData<string>("type"), DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], player.SocialClubId.ToString());
                }
            }
        }
        public static void SendMessageToOrg(Player player, string text)
        {
            if(player.HasSharedData("orgId") && player.GetSharedData<Int32>("orgId") != 0)
            {
                foreach(Player p in NAPI.Pools.GetAllPlayers())
                {
                    if(p.HasSharedData("orgId") && player.GetSharedData<Int32>("orgId") == p.GetSharedData<Int32>("orgId"))
                    {
                        p.TriggerEvent("sendMessage", player.GetSharedData<Int32>("id").ToString(), player.GetSharedData<string>("username"), text, "org", player.GetSharedData<string>("type"), DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], player.SocialClubId.ToString());
                    }
                }
            }
            else
            {
                NotifyPlayer(player, "Nie jesteś w organizacji!");
            }
        }
        public static void SendMessageToNearPlayers(Player player, string message, float range)
        {
            LogManager.LogLocalChat(player.SocialClubId.ToString(), $"{message}");
            foreach (Player p in NAPI.Pools.GetAllPlayers())
            {
                if (player.Position.DistanceTo(p.Position) <= range)
                {
                    p.TriggerEvent("sendMessage", player.GetSharedData<Int32>("id").ToString(), player.GetSharedData<string>("username"), message, "local", player.GetSharedData<string>("type"), DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], player.SocialClubId.ToString());
                }
            }
        }

        public static void SendRemoteMessageToAllPlayers(string message)
        {
            foreach (Player player in NAPI.Pools.GetAllPlayers())
            {
                player.TriggerEvent("sendMessage", "", "", message, "console", "", DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], "");
            }
        }

        public static void SendInfoMessageToAllPlayers(string message)
        {
            foreach (Player player in NAPI.Pools.GetAllPlayers())
            {
                player.TriggerEvent("sendMessage", "", "", message, "info", "", DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], "");
            }
        }

        public static void SendInfoToPlayer(Player player, string message)
        {
            player.TriggerEvent("sendMessage", "", "", message, "info", "", DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], "");
        }

        public static void SendMessageToPlayer(Player player, string message)
        {
            player.TriggerEvent("sendMessage", "", "", message, "info", "", DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], "");
        }

        public static void SendPenaltyToPlayers(string message)
        {
            foreach (Player player in NAPI.Pools.GetAllPlayers())
            {
                player.TriggerEvent("sendMessage", "", "", message, "penalty", "", DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], "");
            }
        }

        public static void SendPrivateMessage(Player player, string to, string message)
        {
            if(player.GetSharedData<string>("pmoff") == "")
            {
                Player plto = GetPlayerByRemoteId(to);
                if (plto != null && plto != player)
                {
                    if(plto.GetSharedData<string>("pmoff") == "")
                    {
                        player.TriggerEvent("sendMessage", plto.GetSharedData<Int32>("id").ToString(), JsonConvert.SerializeObject(new string[] { plto.GetSharedData<string>("username"), "to" }), message, "private", plto.GetSharedData<string>("type"), DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], player.SocialClubId.ToString());
                        plto.TriggerEvent("sendMessage", player.GetSharedData<Int32>("id").ToString(), JsonConvert.SerializeObject(new string[] { player.GetSharedData<string>("username"), "from" }), message, "private", player.GetSharedData<string>("type"), DateTime.Now.TimeOfDay.ToString().Split(":")[0] + ":" + DateTime.Now.TimeOfDay.ToString().Split(":")[1], player.SocialClubId.ToString());
                        LogManager.LogPrivateChat(player.SocialClubId.ToString(), $"DO: {plto.SocialClubId.ToString()}: {message}");
                        LogManager.LogPrivateChat(plto.SocialClubId.ToString(), $"OD: {player.SocialClubId.ToString()}: {message}");
                    }
                    else
                    {
                        SendInfoToPlayer(player, "Gracz ma zablokowane wiadomości prywatne, powód: " + plto.GetSharedData<string>("pmoff") + ".");
                    }
                }
                else
                {
                    SendInfoToPlayer(player, "Nie znaleziono gracza!");
                }
            }
            else
            {
                SendInfoToPlayer(player, "Aby pisać wiadomości prywatne musisz je odblokować!");
            }
            
        }
        public static string GetAvailablePeds()
        {
            string peds = "";
            string currentPed = "";
            using (XmlReader reader = XmlReader.Create(@"availablePeds.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name.ToString())
                        {
                            case "id":
                                currentPed = "|";
                                currentPed += reader.ReadElementContentAsString() + ":";
                                break;
                            case "type":
                                currentPed += reader.ReadElementContentAsString() + ":";
                                break;
                            case "name":
                                currentPed += reader.ReadElementContentAsString() + ":";
                                break;
                            case "hash":
                                currentPed += reader.ReadElementContentAsString();
                                peds += currentPed;
                                break;
                        }
                    }
                }
            }
            if (peds != "")
                peds = peds.Trim('|');
            return peds;
        }

        public static void SetPlayersClothes(Player player)
        {
            if (player.HasSharedData("character") && player.GetSharedData<string>("character") != "")
            {
                if(player.GetSharedData<string>("clothes") == "")
                {
                    Character character = JsonConvert.DeserializeObject<Character>(player.GetSharedData<string>("character").Replace("\\", ""));
                    switch (character.gender)
                    {
                        case 0:
                            player.SetClothes(11, 12, 3);
                            player.SetClothes(3, 1, 0);
                            player.SetClothes(8, 15, 0);
                            player.SetClothes(4, 63, 0);
                            player.SetClothes(6, 31, 0);
                            break;
                        case 1:
                            player.SetClothes(11, 2, 3);
                            player.SetClothes(3, 2, 0);
                            player.SetClothes(8, 15, 0);
                            player.SetClothes(4, 1, 4);
                            player.SetClothes(6, 81, 2);
                            break;
                    }
                    UpdatePlayersClothes(player);
                }
                else
                {
                    LoadPlayersClothes(player);
                }
            }
        }

        public static void LoadPlayersClothes(Player player)
        {
            if(player.HasSharedData("clothes") && player.GetSharedData<string>("clothes") != "")
            {
                List<int[]> clothes = JsonConvert.DeserializeObject<List<int[]>>(player.GetSharedData<string>("clothes"));
                foreach(int[] cloth in clothes)
                {
                    player.SetClothes(cloth[0], cloth[1], cloth[2]);
                }
                player.SetAccessories(0, -1, 0);
                player.SetAccessories(1, -1, 0);
                player.SetClothes(1, 0, 0);
            }
        }

        public static void UpdatePlayersClothes(Player player)
        {
            List<int[]> clothes = new List<int[]>();
            clothes.Add(new int[] {11, player.GetClothesDrawable(11), player.GetClothesTexture(11)});
            clothes.Add(new int[] {8, player.GetClothesDrawable(8), player.GetClothesTexture(8)});
            clothes.Add(new int[] {3, player.GetClothesDrawable(3), player.GetClothesTexture(3)});
            clothes.Add(new int[] {4, player.GetClothesDrawable(4), player.GetClothesTexture(4)});
            clothes.Add(new int[] {6, player.GetClothesDrawable(6), player.GetClothesTexture(6)});
            string clothesStr = JsonConvert.SerializeObject(clothes);

            player.SetSharedData("clothes", clothesStr);
            SavePlayerDataToDB(player, "clothes");
        }


        public static string[] GetClothesById(string clothesId)
        {
            string[] clothes = new string[] {"","",""};
            bool read = false;
            using (XmlReader reader = XmlReader.Create(@"availableClothes.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if(reader.Name.ToString() == clothesId)
                        {
                            read = true;
                        }
                        if(read)
                        {
                            if(clothesId.Contains("top"))
                            {
                                switch(reader.Name.ToString())
                                {
                                    case "a":
                                        clothes[0] = reader.ReadElementContentAsString();
                                        break;
                                    case "b":
                                        clothes[1] = reader.ReadElementContentAsString();
                                        break;
                                    case "c":
                                        clothes[2] = reader.ReadElementContentAsString();
                                        read = false;
                                        break;
                                }
                            }
                            else
                            {
                                if(reader.Name.ToString() == "a")
                                {
                                    clothes[0] = reader.ReadElementContentAsString();
                                    read = false;
                                }
                            }
                        }
                    }
                }
            }
            return clothes;
        }

        public static string GetPedHashById(string id)
        {
            string pedhash = "";
            bool read = false;
            using (XmlReader reader = XmlReader.Create(@"availablePeds.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name.ToString())
                        {
                            case "id":
                                if (reader.ReadElementContentAsString().Equals(id))
                                {
                                    read = true;
                                }
                                else
                                {
                                    read = false;
                                }
                                break;
                            case "hash":
                                if (read)
                                {
                                    pedhash = reader.ReadElementContentAsString();
                                }
                                break;
                        }
                    }
                }
            }
            return pedhash;
        }

        public static bool IsWhiteListed(Player player)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM `whitelist` WHERE socialclubname = '{player.SocialClubName}'";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.Read())
                {
                    dataBase.connection.Close();
                    return true;
                }

            }
            dataBase.connection.Close();
            return false;
        }

        public static void setUsersPenalties(Player player)
        {
            bool banned = true;
            bool muted = true;
            bool nodriving = true;
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM penalties WHERE login = '{player.SocialClubId}'";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.Read())
                {
                    //ban
                    if (reader.GetString(2) != "")
                    {
                        DateTime currentTime = DateTime.Parse(reader.GetString(2));
                        if (DateTime.Compare(currentTime, DateTime.Now) > 0)
                        {
                            player.SetSharedData("banned", true);
                        }
                        else
                        {
                            player.SetSharedData("banned", false);
                            banned = false;
                        }
                    }
                    else
                    {
                        player.SetSharedData("banned", false);
                    }

                    //mute
                    if (reader.GetString(3) != "")
                    {
                        if (DateTime.Compare(DateTime.Parse(reader.GetString(3)), DateTime.Now) > 0)
                        {
                            player.SetSharedData("muted", true);
                            player.SetSharedData("mutedto", reader.GetString(3));
                        }
                        else
                        {
                            player.SetSharedData("muted", false);
                            muted = false;
                        }
                    }
                    else
                    {
                        player.SetSharedData("muted", false);
                    }

                    //driving licence
                    if (reader.GetString(4) != "")
                    {
                        if (DateTime.Compare(DateTime.Parse(reader.GetString(4)), DateTime.Now) > 0)
                        {
                            player.SetSharedData("nodriving", true);
                            player.SetSharedData("nodrivingto", reader.GetString(4));
                        }
                        else
                        {
                            player.SetSharedData("nodriving", false);
                            nodriving = false;
                        }
                    }
                    else
                    {
                        player.SetSharedData("nodriving", false);
                    }
                }
            }
            if (!muted)
            {
                dataBase.command.CommandText = $"UPDATE penalties SET mute = '' WHERE login = '{player.SocialClubId}'";
                dataBase.command.ExecuteNonQuery();
            }
            if (!banned)
            {
                dataBase.command.CommandText = $"UPDATE penalties SET ban = '' WHERE login = '{player.SocialClubId}'";
                dataBase.command.ExecuteNonQuery();
            }
            if (!nodriving)
            {
                dataBase.command.CommandText = $"UPDATE penalties SET drivinglicence = '' WHERE login = '{player.SocialClubId}'";
                dataBase.command.ExecuteNonQuery();
            }
            dataBase.connection.Close();
        }

        public static bool isPlayersPenaltyExpired(Player player, string penalty)
        {
            if(player != null && player.Exists && player.HasSharedData(penalty) && player.GetSharedData<bool>(penalty))
            {
                DateTime penaltyTo = DateTime.Parse(player.GetSharedData<string>(penalty + "to"));
                if(DateTime.Compare(penaltyTo, DateTime.Now) <= 0)
                {
                    player.SetSharedData(penalty, false);
                    switch(penalty)
                    {
                        case "muted":
                            if(player.Exists)
                            {
                                return true;
                            }
                            break;
                        case "nodriving":
                            if(player.Exists)
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            return false;
        }
        public static void banPlayer(Player player, string duration, string reason, Player admin)
        {
            int length = 0;
            DateTime time = DateTime.Now;
            bool set = true;
            if (admin == null || player.GetSharedData<Int32>("power") < admin.GetSharedData<Int32>("power"))
            {
                if (duration.Contains("h"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('h'));
                        time = DateTime.Now.AddHours(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("d"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('d'));
                        time = DateTime.Now.AddDays(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("m"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('m'));
                        time = DateTime.Now.AddMinutes(length);


                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("y"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('y'));
                        time = DateTime.Now.AddYears(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("M"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('M'));
                        time = DateTime.Now.AddMonths(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                if (set)
                {
                    DBConnection dataBase = new DBConnection();
                    dataBase.command.CommandText = $"UPDATE penalties SET ban = '{time.ToString()}' WHERE login = '{player.SocialClubId}'";
                    dataBase.command.ExecuteNonQuery();
                    dataBase.connection.Close();
                    SendPenaltyToPlayers(player.GetSharedData<string>("username") + " został zbanowany przez " + (admin != null ? admin.GetSharedData<string>("username") : "CONSOLE") + " do " + time.ToString() + ". Powód: " + reason);
                    LogManager.LogPenalty(player.SocialClubId.ToString(), $"Gracz został zbanowany do {time.ToString()}, powód: {reason}");
                    AddPenaltyToDB(player.SocialClubId.ToString(), (admin != null ? admin.SocialClubId.ToString() : "CONSOLE"), "ban", DateTime.Now.ToString(), time.ToString(), reason);
                    player.Kick("Zostałeś zbanowany!");
                }
            }
            else if(admin != null)
                NotifyPlayer(admin, "Nie masz do tego uprawnień!");
        }

        public static void SendInfoToConsole(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now.TimeOfDay.ToString().Split(":")[0]}:{DateTime.Now.TimeOfDay.ToString().Split(":")[1]} - {text}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void warnPlayer(Player player, string reason, Player admin)
        {
            if (player.GetSharedData<Int32>("power") < admin.GetSharedData<Int32>("power"))
            {
                LogManager.LogPenalty(player.SocialClubId.ToString(), $"Gracz otrzymał ostrzeżenie, powód: {reason}");
                player.TriggerEvent("warnSound");
                player.TriggerEvent("warnPlayer");
                SendPenaltyToPlayers(player.GetSharedData<string>("username") + " otrzymał ostrzeżenie od " + admin.GetSharedData<string>("username") + ". Powód: " + reason);
                AddPenaltyToDB(player.SocialClubId.ToString(), admin.SocialClubId.ToString(), "warn", DateTime.Now.ToString(), "", reason);
            }
            else
                NotifyPlayer(admin, "Nie masz do tego uprawnień!");
        }
        public static void kickPlayer(Player player, string reason, Player admin)
        {
            if (player.GetSharedData<Int32>("power") < admin.GetSharedData<Int32>("power"))
            {
                LogManager.LogPenalty(player.SocialClubId.ToString(), $"Gracz został wyrzucony, powód: {reason}");
                player.Kick(reason);
                SendPenaltyToPlayers(player.GetSharedData<string>("username") + " został wyrzucony przez " + admin.GetSharedData<string>("username") + ". Powód: " + reason);
                AddPenaltyToDB(player.SocialClubId.ToString(), admin.SocialClubId.ToString(), "kick", DateTime.Now.ToString(), "", reason);
            }
            else
                NotifyPlayer(admin, "Nie masz do tego uprawnień!");

        }

        public static void kickPlayerFromConsole(Player player, string reason)
        {
            LogManager.LogPenalty(player.SocialClubId.ToString(), $"Gracz został wyrzucony, powód: {reason}");
            SendPenaltyToPlayers(player.GetSharedData<string>("username") + " został wyrzucony przez CONSOLE, Powód: " + reason);
            AddPenaltyToDB(player.SocialClubId.ToString(), "", "kick", DateTime.Now.ToString(), "", reason);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now.TimeOfDay.ToString().Split(":")[0]}:{DateTime.Now.TimeOfDay.ToString().Split(":")[1]} [{player.GetSharedData<Int32>("id").ToString()}]{player.GetSharedData<string>("username")} został wyrzucony przez CONSOLE, powód: " + reason);
            Console.ForegroundColor = ConsoleColor.Gray;
            player.Kick(reason);
        }

        public static void mutePlayer(Player player, string duration, string reason, Player admin)
        {
            int length = 0;
            DateTime time = DateTime.Now;
            bool set = true;
            if (player.GetSharedData<Int32>("power") <= admin.GetSharedData<Int32>("power"))
            {
                if (duration.Contains("h"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('h'));
                        time = DateTime.Now.AddHours(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("d"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('d'));
                        time = DateTime.Now.AddDays(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("m"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('m'));
                        time = DateTime.Now.AddMinutes(length);


                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("y"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('y'));
                        time = DateTime.Now.AddYears(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("M"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('M'));
                        time = DateTime.Now.AddMonths(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                if (set)
                {
                    DBConnection dataBase = new DBConnection();
                    dataBase.command.CommandText = $"UPDATE penalties SET mute = '{time.ToString()}' WHERE login = '{player.SocialClubId}'";
                    dataBase.command.ExecuteNonQuery();
                    dataBase.connection.Close();
                    SendPenaltyToPlayers(player.GetSharedData<string>("username") + " został wyciszony przez " + admin.GetSharedData<string>("username") + " do " + time.ToString() + ". Powód: " + reason);
                    LogManager.LogPenalty(player.SocialClubId.ToString(), $"Gracz został wyciszony do {time.ToString()}, powód: {reason}");
                    AddPenaltyToDB(player.SocialClubId.ToString(), admin.SocialClubId.ToString(), "mute", DateTime.Now.ToString(), time.ToString(), reason);
                    player.TriggerEvent("warnSound");
                    player.TriggerEvent("warnPlayer");
                    player.SetSharedData("muted", true);
                    player.SetSharedData("mutedto", time.ToString());
                }
            }
            else
                NotifyPlayer(admin, "Nie masz do tego uprawnień!");
        }

        public static void takeLicence(Player player, string duration, string reason, Player admin)
        {
            int length = 0;
            DateTime time = DateTime.Now;
            bool set = true;
            if (player.GetSharedData<Int32>("power") < admin.GetSharedData<Int32>("power"))
            {
                if (duration.Contains("h"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('h'));
                        time = DateTime.Now.AddHours(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("d"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('d'));
                        time = DateTime.Now.AddDays(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("m"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('m'));
                        time = DateTime.Now.AddMinutes(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("y"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('y'));
                        time = DateTime.Now.AddYears(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                else if (duration.Contains("M"))
                {
                    try
                    {
                        length = Convert.ToInt32(duration.TrimEnd('M'));
                        time = DateTime.Now.AddMonths(length);

                    }
                    catch (Exception)
                    {
                        set = false;
                    }
                }
                if (set)
                {
                    DBConnection dataBase = new DBConnection();
                    dataBase.command.CommandText = $"UPDATE penalties SET drivinglicence = '{time.ToString()}' WHERE login = '{player.SocialClubId}'";
                    dataBase.command.ExecuteNonQuery();
                    dataBase.connection.Close();
                    SendPenaltyToPlayers(player.GetSharedData<string>("username") + " stracił prawo jazdy do " + time.ToString() + ". Powód: " + reason + " (" + admin.GetSharedData<string>("username") + ").");
                    LogManager.LogPenalty(player.SocialClubId.ToString(), $"Gracz stracił uprawenienia do prowadzenia pojazdów do {time.ToString()}, powód: {reason}");
                    AddPenaltyToDB(player.SocialClubId.ToString(), admin.SocialClubId.ToString(), "licence", DateTime.Now.ToString(), time.ToString(), reason);
                    player.SetSharedData("nodriving", true);
                    player.SetSharedData("nodrivingto", time.ToString());
                    player.TriggerEvent("warnSound");
                    player.TriggerEvent("warnPlayer");
                    if (player.Vehicle != null && player.Vehicle.Exists)
                    {
                        player.WarpOutOfVehicle();
                    }
                }
            }
            else
                NotifyPlayer(admin, "Nie masz do tego uprawnień!");
        }

        public static void unLicence(Player player, string reason, Player admin)
        {
            if (player.GetSharedData<Int32>("power") <= admin.GetSharedData<Int32>("power"))
            {
                LogManager.LogPenalty(player.SocialClubId.ToString(), $"Cofnięto Kat B, powód: {reason}");
                player.TriggerEvent("warnSound");
                player.TriggerEvent("warnPlayer");
                SendPenaltyToPlayers(admin.GetSharedData<string>("username") + " unieważnił prawo jazdy kat. B graczowi " + player.GetSharedData<string>("username") + ". Powód: " + reason);
                AddPenaltyToDB(player.SocialClubId.ToString(), admin.SocialClubId.ToString(), "coflic", DateTime.Now.ToString(), "", reason);
                player.SetSharedData("licenceBt", false);
                player.SetSharedData("licenceBp", false);
                SavePlayerDataToDB(player, "licenceBt");
            }
            else
                NotifyPlayer(admin, "Nie masz do tego uprawnień!");

        }

        public static string GetPlayerNameById(string id)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM users WHERE login = '{id}'";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.Read())
                {
                    string name = reader.GetString(3);
                    dataBase.connection.Close();
                    return name;
                }
                else
                {
                    dataBase.connection.Close();
                    return "";
                }
            }

        }

        public static void GetPlayersLicences(Player player)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM licences WHERE id = {player.GetSharedData<Int32>("sid")}";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.Read())
                {
                    player.SetSharedData("licenceBt", Boolean.Parse(reader.GetString(1)));
                    player.SetSharedData("licenceBp", Boolean.Parse(reader.GetString(2)));
                }
            }
            dataBase.connection.Close();
        }

        public static void UpdatePlayersJobPoints(Player player, string type, int points)
        {
            if(player == null && player.Exists)
            {
                if (player.HasSharedData($"jp_{type}"))
                {
                    int amount = player.GetSharedData<int>($"jp_{type}");
                    player.SetSharedData($"jp_{type}", amount + points);
                    DBConnection dataBase = new DBConnection();
                    dataBase.command.CommandText = $"UPDATE jobs SET {type} = {amount+points} WHERE player = '{player.SocialClubId}'";
                    dataBase.command.ExecuteNonQuery();
                    dataBase.connection.Close();
                }
            }
        }

        public static void GetPlayersJobPoints(Player player)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM jobs WHERE player = '{player.SocialClubId.ToString()}'";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.Read())
                {
                    player.SetSharedData("jp_warehouse", reader.GetInt32(2));
                    player.SetSharedData("jp_debriscleaner", reader.GetInt32(3));
                    player.SetSharedData("jp_lawnmowing", reader.GetInt32(4));
                    player.SetSharedData("jp_forklifts", reader.GetInt32(5));
                    player.SetSharedData("jp_diver", reader.GetInt32(6));
                    player.SetSharedData("jp_gardener", reader.GetInt32(7));
                    player.SetSharedData("jp_towtruck", reader.GetInt32(8));
                    player.SetSharedData("jp_refinery", reader.GetInt32(9));
                    player.SetSharedData("jp_fisherman", reader.GetInt32(10));
                    player.SetSharedData("jp_hunter", reader.GetInt32(11));
                }
            }
            dataBase.connection.Close();
        }

        public static void UpdatePlayersJobBonus(Player player, string job, int amount)
        {
            player.SetSharedData($"jp_{job}", player.GetSharedData<int>($"jp_{job}") + amount);

            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"UPDATE jobs SET {job} = {player.GetSharedData<int>($"jp_{job}")} WHERE player = '{player.SocialClubId}'";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();

        }

        public static Vector3 JsonToVector(string pos)
        {
            float[] a = System.Text.Json.JsonSerializer.Deserialize<float[]>(pos);
            return new Vector3(a[0], a[1], a[2]);
        }

        public static string VectorToJson(Vector3 pos)
        {
            float[] a = new float[] { pos.X, pos.Y, pos.Z };
            return System.Text.Json.JsonSerializer.Serialize(a);
        }

        public static List<KeyValuePair<string, string>> GetRacingTimes(string raceType)
        {
            List<KeyValuePair<string, string>> times = new List<KeyValuePair<string, string>>();

            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM {raceType}";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetString(1) != "")
                    {
                        times.Add(new KeyValuePair<string, string>(reader.GetString(1), reader.GetString(2)));
                    }
                }
            }

            dataBase.connection.Close();

            return times;
        }

        public static void UpdateRacingTimes(string racetype, List<KeyValuePair<string, string>> times)
        {
            DBConnection dataBase = new DBConnection();
            foreach (KeyValuePair<string, string> time in times)
            {
                if (times.IndexOf(time) == 10)
                    break;
                dataBase.command.CommandText = $"UPDATE {racetype} SET login = '{time.Key}', time = '{time.Value}' WHERE id = {times.IndexOf(time) + 1}";
                dataBase.command.ExecuteNonQuery();
            }
            dataBase.connection.Close();
        }

        public static Report ReportAPlayer(Player informer, string reported, string description)
        {
            Player reportedPlayer = GetPlayerByRemoteId(reported);
            if (reportedPlayer == null)
            {
                return null;
            }
            else
            {
                return new Report(informer, reportedPlayer, description, DateTime.Now);
            }
        }

        public static int GetPlayersVehiclesCount(Player player)
        {
            int count = 0;

            DBConnection dataBase = new DBConnection();

            dataBase.command.CommandText = $"SELECT COUNT(id) FROM vehicles WHERE owner = '{player.SocialClubId.ToString()}'";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
            }

            dataBase.connection.Close();

            return count;
        }

        public static void NotifyAdmins()
        {
            foreach(Player player in NAPI.Pools.GetAllPlayers())
            {
                if(player.HasSharedData("power") && player.GetSharedData<Int32>("power") >= 3)
                {
                    player.TriggerEvent("reportNotify");
                }
            }
        }

        public static bool HasItem(Player player, int itemId)
        {
            if (player.Exists)
            {
                string eq = player.GetSharedData<string>("equipment");
                List<Dictionary<string, string>> eqList = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, string>>>(eq);
                foreach (Dictionary<string, string> item in eqList)
                {
                    string id = "";
                    item.TryGetValue("typeID", out id);
                    if (id == itemId.ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static void CheckPlayersLevel(Player player)
        {
            int exp = player.GetSharedData<Int32>("exp");
            int nextLevel = player.GetSharedData<Int32>("nextlevel");
            if (exp >= nextLevel)
            {
                SetPlayersLevel(player);
            }
        }

        public static void SetUseEmojis(Player player)
        {
            if(player.Exists)
            {
                if (player.HasSharedData("settings_UseEmojis"))
                {
                    player.TriggerEvent("mainHUD_useEmojis", player.GetSharedData<bool>("settings_UseEmojis"));
                }
            }
        }
        public static void SetPlayersConnectValues(Player player, string currentWeather)
        {
            player.TriggerEvent("freezeFor2Sec");
            NAPI.Entity.SetEntityTransparency(player.Handle, 255);
            player.TriggerEvent("openMainHUD", player.GetSharedData<Int32>("id"));
            player.TriggerEvent("openEquipmentBrowser", player.GetSharedData<string>("equipment"), player.GetSharedData<Int32>("skill-0"));
            SendPlayerDataToMainHUD(player);
            player.TriggerEvent("setTime", (time.Hour.ToString().Length == 1 ? ("0" + time.Hour.ToString()) : time.Hour.ToString()) + ":" + (time.Minute.ToString().Length == 1 ? ("0" + time.Minute.ToString()) : time.Minute.ToString()));
            player.TriggerEvent("setWeather", currentWeather, false);
            player.SetSharedData("spawned", true);
            player.Dimension = 0;
            if (player.GetSharedData<bool>("muted") && player.HasSharedData("mutedto"))
            {
                SendInfoToPlayer(player, "Jesteś wyciszony do: " + player.GetSharedData<string>("mutedto"));
            }
            if (player.GetSharedData<bool>("nodriving") && player.HasSharedData("nodrivingto"))
            {
                SendInfoToPlayer(player, "Nie możesz prowadzić pojazdów do: " + player.GetSharedData<string>("nodrivingto"));
            }
        }
        public static void SetPlayersLevel(Player player)
        {
            if(player.HasSharedData("level"))
            {
                int level = player.GetSharedData<Int32>("level");
                int exp = player.GetSharedData<Int32>("exp");
                int[] levelInfo = getLevelByExp(exp);
                int playersLevel = levelInfo[0];
                int toNextLevel = levelInfo[1];
                int addSP = playersLevel - level;
                UpdatePlayersSkillPoints(player, player.GetSharedData<Int32>("skillpoints") + addSP);
                NotifyPlayer(player, $"Zdobyłeś {addSP} punkt(ów) umiejętności!");
                player.SetSharedData("level", playersLevel);
                player.SetSharedData("nextlevel", toNextLevel);
            }
            else
            {
                int exp = player.GetSharedData<Int32>("exp");
                int[] levelInfo = getLevelByExp(exp);
                int playersLevel = levelInfo[0];
                int toNextLevel = levelInfo[1];
                player.SetSharedData("level", playersLevel);
                player.SetSharedData("nextlevel", toNextLevel);
            }
            
        }

        public static void SetPlayersSkills(Player player)
        {
            Dictionary<int, int> skills = JsonConvert.DeserializeObject<Dictionary<int, int>>(player.GetSharedData<string>("skills"));

            foreach (KeyValuePair<int, int> skill in skills)
            {
                player.SetSharedData("skill-" + skill.Key.ToString(), skill.Value);
            }
        }

        public static void SpawnPlayerAtClosestHospital(Player player){
            Dictionary<Vector3, float> hospitals = new Dictionary<Vector3, float>()
            {
                [new Vector3(298.14047f, -584.45996f, 43.260853f)] = 72.193085f,
                [new Vector3(-449.81253f, -340.8752f, 34.501774f)] = 80.18012f,
                [new Vector3(1151.607f, -1528.7936f, 35.184227f)] = -31.735722f,
                [new Vector3(1839.2878f, 3672.8652f, 34.276737f)] = -149.19778f,
                [new Vector3(-247.96602f, 6330.9976f, 32.426186f)] = -146.07516f
            };

            if(player.HasSharedData("deathpos")){
                Vector3 deathpos = player.GetSharedData<Vector3>("deathpos");
                KeyValuePair<Vector3, float> closestHospital = new KeyValuePair<Vector3, float>(new Vector3(298.14047f, -584.45996f, 43.260853f), 72.193085f);
                foreach(KeyValuePair<Vector3, float> hospital in hospitals){
                    if(hospital.Key.DistanceTo(deathpos) < closestHospital.Key.DistanceTo(deathpos)){
                        closestHospital = hospital;
                    }
                }
                player.Position = closestHospital.Key;
                player.Heading = closestHospital.Value;
            }
        }


        public static void AddPenaltyToDB(string login, string admin, string type, string timefrom, string timeto, string reason)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"INSERT INTO penlist (login, admin, type, `timefrom`, timeto, reason) VALUES ('{login}', '{admin}', '{type}', '{timefrom}', '{timeto}', '{reason}')";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }

        public static List<string[]> GetPlayersPenalties(string social, string name)
        {
            List<string[]> penalties = new List<string[]>();
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT users.username, penlist.* FROM users LEFT JOIN penlist ON users.login = penlist.admin WHERE penlist.login = '{social}';";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    penalties.Add(new string[]
                    {
                        name,
                        reader.GetString(0),
                        reader.GetString(4),
                        reader.GetString(5),
                        reader.GetString(6),
                        reader.GetString(7),
                    });
                }
            }
            dataBase.connection.Close();
            return penalties;
        }

        public static void SetPlayersSettings(Player player)
        {
            if (player.HasSharedData("settings"))
            {
                Settings settings = JsonConvert.DeserializeObject<Settings>(player.GetSharedData<string>("settings"));
                player.SetSharedData("settings_HudSize", settings.HudSize);
                player.SetSharedData("settings_ChatSize", settings.ChatSize);
                player.SetSharedData("settings_SpeedometerSize", settings.SpeedometerSize);
                player.SetSharedData("settings_VoiceChat", settings.VoiceChat);
                player.SetSharedData("settings_DisplayNick", settings.DisplayNick);
                player.SetSharedData("settings_DisplayGlobal", settings.DisplayGlobal);
                player.SetSharedData("settings_VoiceKey", settings.VoiceKey);
                player.SetSharedData("settings_UseEmojis", settings.UseEmojis);
                player.SetSharedData("settings_WallpaperUrl", settings.WallpaperUrl);
            }
        }

        public static void UpdatePlayersSettings(Player player, string set)
        {
            if (player.HasSharedData("settings"))
            {
                Settings settings = JsonConvert.DeserializeObject<Settings>(set);
                player.SetSharedData("settings_HudSize", settings.HudSize);
                player.SetSharedData("settings_ChatSize", settings.ChatSize);
                player.SetSharedData("settings_SpeedometerSize", settings.SpeedometerSize);
                player.SetSharedData("settings_VoiceChat", settings.VoiceChat);
                player.SetSharedData("settings_DisplayNick", settings.DisplayNick);
                player.SetSharedData("settings_DisplayGlobal", settings.DisplayGlobal);
                player.SetSharedData("settings_VoiceKey", settings.VoiceKey);
                player.SetSharedData("settings_UseEmojis", settings.UseEmojis);
                player.SetSharedData("settings_WallpaperUrl", settings.WallpaperUrl);
                player.SetSharedData("settings", set);

                SavePlayerDataToDB(player, "settings");

                SetUseEmojis(player);

            }
        }

        public static string GenerateRandomCardNumber(ulong seed)
        {
            string number1 = seed.ToString();
            string number2 = (seed + 1).ToString();
            string number3 = (seed + 2).ToString();
            number1 = NAPI.Util.GetHashKey(number1).ToString();
            number2 = NAPI.Util.GetHashKey(number2).ToString();
            number3 = NAPI.Util.GetHashKey(number3).ToString();
            string cardNumber = number1[0].ToString() + number1[1] + number1[2] + number1[3] + number2[0] + number2[1] + number2[2] + number2[3] + number3[0] + number3[1] + number3[2] + number3[3] + number1[4] + number1[5] + number2[4] + number3[4];
            return cardNumber;
        }

        public static void CheckUsersAvatar(Player player)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM discord WHERE login = '{player.SocialClubId}'";
            using(MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    string avatar = reader.GetString(3);
                    if (avatar != "")
                    {
                        ChangeUsersAvatar(player, avatar);
                    }
                }
                else
                {
                    CreateUsersAvatar(player);
                }
            }
            dataBase.connection.Close();
        }
        public static void ChangeUsersAvatar(Player player, string avatar)
        {
            if(!Directory.Exists(avatarsPath + @$"{player.SocialClubId}"))
            {
                Directory.CreateDirectory(avatarsPath + @$"{player.SocialClubId}");
            }
            using (var client = new WebClient())
            {
                client.DownloadFile(avatar, avatarsPath + @$"{player.SocialClubId}/avatar.png");
            }

            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"UPDATE discord SET avatar = '' WHERE LOGIN = '{player.SocialClubId}'";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();

        }

        public static void CreateUsersAvatar(Player player)
        {
            if (!Directory.Exists(avatarsPath + @$"{player.SocialClubId}"))
            {
                Directory.CreateDirectory(avatarsPath + @$"{player.SocialClubId}");
            }
            if(File.Exists(avatarsPath + @$"{player.SocialClubId}/avatar.png"))
            {
                File.Delete(avatarsPath + @$"{player.SocialClubId}/avatar.png");
            }
            File.Copy(avatarsPath + "default.png", avatarsPath + @$"{player.SocialClubId}/avatar.png");
        }

        public static Image CreateCarMugshot(List<string> base64Imgs)
        {
            List<Image> images = new List<Image>();
            foreach (string str in base64Imgs)
            {
                byte[] bytes = Convert.FromBase64String(str);

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    images.Add(Image.FromStream(ms));
                }
            }
            List<Image> imageRows = new List<Image>();
            for (int i = 0; i < 6; i++)
            {
                Image img = images[(i * 6)];
                for (int j = 1; j < 6; j++)
                {
                    img = MergeImages(img, images[i * 6 + j], true);
                }
                imageRows.Add(img);
            }
            Image finalImage = imageRows[0];
            for (int i = 1; i < imageRows.Count; i++)
            {
                finalImage = MergeImages(finalImage, imageRows[i], false);
            }
            finalImage.Save("merged.png");
            return finalImage;
        }

        private static Bitmap MergeImages(Image image1, Image image2, bool side)
        {
            if (side)
            {
                Bitmap bitmap = new Bitmap(image1.Width + image2.Width, Math.Max(image1.Height, image2.Height));
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(image1, 0, 0);
                    g.DrawImage(image2, image1.Width, 0);
                }

                return bitmap;
            }
            else
            {
                Bitmap bitmap = new Bitmap(image1.Width, image1.Height + image2.Height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(image1, 0, 0);
                    g.DrawImage(image2, 0, image1.Height);
                }

                return bitmap;
            }

        }

        public static string GetPlayersSkills(Player player)
        {
            int[] skills = new int[5];
            for (int i = 0; i < 5; i++)
            {
                skills[i] = player.GetSharedData<Int32>("skill-" + i.ToString());
            }
            return JsonConvert.SerializeObject(skills);
        }
        public static string GetPlayersInfo(Player player)
        {
            List<string> playerData = new List<string>();
            playerData.Add(player.GetSharedData<string>("username"));
            playerData.Add(player.SocialClubId.ToString());
            playerData.Add(player.GetSharedData<Int32>("sid").ToString());
            playerData.Add(player.GetSharedData<string>("registered"));
            playerData.Add(player.GetSharedData<Int32>("playtime").ToString());
            playerData.Add($"{ player.GetSharedData<Int32>("exp")}/{ player.GetSharedData<Int32>("nextlevel")}");
            playerData.Add((GetPlayersVehiclesCount(player)).ToString() + "/" + player.GetSharedData<Int32>("vehslots").ToString());
            playerData.Add(player.GetSharedData<Int32>("skillpoints").ToString());
            playerData.Add(GetPlayersCollectiblesAmount(player).ToString() + "/" + CollectibleManager.collectibleCount.ToString());
            playerData.Add((60-player.GetSharedData<Int32>("bonustime")).ToString());
            return JsonConvert.SerializeObject(playerData);
        }

        public static int[] getLevelByExp(int exp)
        {
            
            int lvl = 0;
            foreach (int level in levels)
            {
                if (exp >= level)
                {
                    lvl = levels.IndexOf(level);
                }
            }
            lvl++;
            return new int[2] { lvl, levels[lvl] };
        }


        public static List<int> GetPlayersVehiclesById(ulong playerId)
        {
            List<int> vehs = new List<int>();
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT id FROM vehicles WHERE owner = '{playerId}'";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    vehs.Add(reader.GetInt32(0));
                }
            }
            dataBase.connection.Close();
            return vehs;
        }

        public static Player GetClosestCivilianToCuff(Vector3 position, float distance)
        {
            Player closestPlayer = null;
            foreach(Player player in NAPI.Pools.GetAllPlayers())
            {
                if (player.Vehicle == null && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")) && !(player.HasSharedData("arrested") && player.GetSharedData<bool>("arrested")))
                {
                    if (closestPlayer == null && player.Position.DistanceTo(position) <= distance)
                    {
                        closestPlayer = player;
                    }
                    else if(closestPlayer != null)
                    {
                        if (player.Position.DistanceTo(position) < closestPlayer.Position.DistanceTo(position))
                        {
                            closestPlayer = player;
                        }
                    }
                }
            }
            return closestPlayer;
        }
        
        private static void GenerateNewAuthCode(Player player)
        {
            string code = NAPI.Util.GetHashKey(player.SocialClubName + player.SocialClubId).ToString();
            player.SetSharedData("authcode", code);
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"UPDATE users SET authcode = '{code}' WHERE login = '{player.SocialClubId}'";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }

        public static void SetJobClothes(Player player, bool state, string job)
        {
            if(state)
            {
                bool male = JsonConvert.DeserializeObject<Character>(player.GetSharedData<string>("character").Replace("\\", "")).gender == 0;
                switch (job)
                {
                    case "diver":
                        if(male)
                        {
                            player.SetClothes(1, 122, 0);
                            player.SetClothes(4, 94, 0);
                            player.SetClothes(6, 67, 0);
                            player.SetClothes(8, 124, 7);
                            player.SetClothes(11, 243, 0);
                            player.SetClothes(3, 1, 0);
                            player.SetAccessories(1, 26, 0);
                        }
                        else
                        {
                            player.SetClothes(1, 122, 0);
                            player.SetClothes(4, 97, 0);
                            player.SetClothes(6, 70, 0);
                            player.SetClothes(8, 154, 7);
                            player.SetClothes(11, 251, 0);
                            player.SetClothes(3, 2, 0);
                            player.SetAccessories(1, 28, 0);
                        }
                        break;
                }

            }
            else
            {
                LoadPlayersClothes(player);
            }
        }

        public static void MaxPlayersJobPoints(Player player)
        {
            //player.SetSharedData("logisticpoints", 2000);
            //player.SetSharedData("naturepoints", 2000);
            //player.SetSharedData("waterpoints", 2000);
            //player.SetSharedData("socialpoints", 2000);
            //DBConnection dataBase = new DBConnection();
            //dataBase.command.CommandText = $"UPDATE jobs SET logisticpoints = 2000, naturepoints = 2000, waterpoints = 2000, socialpoints = 2000 WHERE player = '{player.SocialClubId}'";
            //dataBase.command.ExecuteNonQuery();
            //dataBase.connection.Close();
        }

        public static string[] GetPlayersBankingData(Player player)
        {
            List<string> data = new List<string>();
            data.Add(player.SocialClubId.ToString());
            data.Add(player.GetSharedData<string>("username"));
            data.Add(player.GetSharedData<string>("accnumber"));
            data.Add(player.GetSharedData<int>("bank").ToString());


            List<string[]> transactions = new List<string[]>();
            List<int> vehs = new List<int>();
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM transfers WHERE sender = '{player.SocialClubId}' OR target = '{player.SocialClubId}'";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    transactions.Add(new string[] { reader.GetString(1) == player.SocialClubId.ToString() ? "to" : "from", reader.GetString(1) == player.SocialClubId.ToString() ? reader.GetString(2) : reader.GetString(1), reader.GetString(3), reader.GetString(4), reader.GetString(5) });
                }
            }
            dataBase.connection.Close();
            for(int i = 0; i < transactions.Count; i++)
            {
                transactions[i][1] = GetPlayerNameById(transactions[i][1]);
            }

            transactions.Reverse();

            return new string[] { JsonConvert.SerializeObject(data), transactions.Count > 0 ? JsonConvert.SerializeObject(transactions) : "" };
        }

        public static string GetPlayersIDByAccNumber(string accnumber)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT login FROM users WHERE accnumber = '{accnumber}'";
            object login = dataBase.command.ExecuteScalar();
            dataBase.connection.Close();
            string loginStr = "";
            if(login != null)
            {
                loginStr = (string)login;
            }

            return loginStr;
        }

        public static void TransferMoneyToOfflinePlayer(string login, int amount)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT bank FROM users WHERE login = '{login}'";
            object bankmoney = dataBase.command.ExecuteScalar();
            int bank = 0;

            if (bankmoney != null)
            {
                bank = int.Parse((string)bankmoney);
                bank += amount;
                dataBase.command.CommandText = $"UPDATE users SET bank = '{bank}' WHERE login = '{login}'";
                dataBase.command.ExecuteNonQuery();
                dataBase.connection.Close();
            }
        }




        //MESSENGER
        public static string GetPlayersConversations(Player player)
        {
            DBConnection dataBase = new DBConnection();
            List<List<string>> conversations = new List<List<string>>();
            dataBase.command.CommandText = $"SELECT * FROM messenger WHERE sender = '{player.SocialClubId}' OR receiver = '{player.SocialClubId}' ORDER BY id DESC";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if(conversations.Count > 0)
                    {
                        for(int i=0; i<conversations.Count; i++)
                        {
                            bool breakLoop = false;
                            var conversation = conversations[i];
                            foreach(List<string> conv in conversations)
                            {
                                if(conv.Contains(reader.GetString(1)) || conv.Contains(reader.GetString(2)))
                                {
                                    breakLoop = true;
                                    break;
                                }
                            }
                            if (!breakLoop)
                            {
                                string playerId = reader.GetString(1) == player.SocialClubId.ToString() ? reader.GetString(2) : reader.GetString(1);
                                conversations.Add(new List<string>()
                                {
                                    playerId
                                });
                            }
                        }
                    }
                    else
                    {
                        string playerId = reader.GetString(1) == player.SocialClubId.ToString() ? reader.GetString(2) : reader.GetString(1);
                        conversations.Add(new List<string>()
                        {
                            playerId
                        });
                    }
                }
            }

            if(conversations.Count > 0)
            {
                for(int i=0; i<conversations.Count; i++)
                {
                    dataBase.command.CommandText = $"SELECT count(id) FROM messenger WHERE sender = '{conversations[i][0]}' AND receiver = '{player.SocialClubId}' AND received = 'False'";
                    Int64 amount = (Int64)dataBase.command.ExecuteScalar();
                    if(amount > 0)
                    {
                        conversations[i].Add("true");
                    }
                    else
                    {
                        conversations[i].Add("false");
                    }

                    dataBase.command.CommandText = $"SELECT username FROM users WHERE login = '{conversations[i][0]}'";
                    string username = (string)dataBase.command.ExecuteScalar();
                    conversations[i].Add(username);
                }
            }
            dataBase.connection.Close();
            return conversations.Count > 0 ? JsonConvert.SerializeObject(conversations) : "";
        }

        public static string GetPlayersMessages(Player player, string playerID)
        {
            DBConnection dataBase = new DBConnection();
            List<List<string>> messages = new List<List<string>>();
            dataBase.command.CommandText = $"SELECT * FROM messenger WHERE (sender = '{player.SocialClubId}' AND receiver = '{playerID}') OR (sender = '{playerID}' AND receiver = '{player.SocialClubId}')";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string type = "to";
                    if(reader.GetString(1) == playerID)
                    {
                        type = "from";
                    }
                    string message = reader.GetString(3);
                    string date = reader.GetString(4);
                    messages.Add(new List<string>()
                    {
                        type, message, date
                    });
                }
            }
            dataBase.command.CommandText = $"UPDATE messenger SET received = 'True' WHERE sender = '{playerID}' AND receiver = '{player.SocialClubId}' AND received = 'False'";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
            
            return messages.Count > 0 ? JsonConvert.SerializeObject(messages) : "";
        }

        public static void SendMessageToPlayer(Player player, string playerToId, string message)
        {
            DBConnection dataBase = new DBConnection();
            List<List<string>> messages = new List<List<string>>();
            dataBase.command.CommandText = $"INSERT INTO messenger (sender, receiver, message, date, received) VALUES ('{player.SocialClubId}', '{playerToId}', '{message}', '{DateTime.Now}', 'False')";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }

        public static string HasPlayerNewMessages(Player player)
        {
            List<int> messageIds = new List<int>();
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT id FROM messenger WHERE receiver = '{player.SocialClubId}' AND received = 'False'";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while(reader.Read())
                {
                    messageIds.Add(reader.GetInt32(0));
                }
            }

            dataBase.connection.Close();
            return messageIds.Count == 0 ? "" : JsonConvert.SerializeObject(messageIds);
        }

        public static string SearchForPlayers(Player player, string keyword)
        {
            List<List<string>> players = new List<List<string>>();

            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT login, username FROM users WHERE login NOT LIKE '{player.SocialClubId}' AND UPPER(username) LIKE UPPER('%{keyword}%');";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    players.Add(new List<string>()
                    {
                        reader.GetString(0),
                        reader.GetString(1)
                    });
                }
            }

            dataBase.connection.Close();
            return players.Count > 0 ? JsonConvert.SerializeObject(players) : "";
        }
    }
}

[JsonObject(ItemRequired = Required.Always)]
public class Character
{
    public int gender { get; set; }
    public float[] blendData { get; set; }
    public float[] headOverlays { get; set; }
    public float[] headOverlaysColors { get; set; }
    public float[] hair { get; set; }
    public float[] beard { get; set; }
    public float[] faceFeatures { get; set; }
}

public class Settings
{
    public int HudSize { get; set; }
    public int ChatSize { get; set; }
    public int SpeedometerSize { get; set; }
    public bool DisplayNick { get; set; }
    public bool DisplayGlobal { get; set; }
    public bool VoiceChat { get; set; }
    public int VoiceKey { get; set; }
    public bool UseEmojis { get; set; }
    public string WallpaperUrl { get; set; }

    public Settings(int HudSize, int ChatSize, int SpeedometerSize, bool DisplayNick, bool DisplayGlobal, bool VoiceChat, int VoiceKey, bool UseEmojis, string WallpaperUrl)
    {
        this.HudSize = HudSize;
        this.ChatSize = ChatSize;
        this.SpeedometerSize = SpeedometerSize;
        this.DisplayNick = DisplayNick;
        this.DisplayGlobal = DisplayGlobal;
        this.VoiceChat = VoiceChat;
        this.VoiceKey = VoiceKey;
        this.UseEmojis = UseEmojis;
        this.WallpaperUrl = WallpaperUrl;
    }
}

public class EqItem
{
    public int Id { get; set; }
    public int Slot { get; set; }
    public int TypeID { get; set; }

    public EqItem(int id, int slot, int typeid)
    {
        Id = id;
        Slot = slot;
        TypeID = typeid;
    }
}
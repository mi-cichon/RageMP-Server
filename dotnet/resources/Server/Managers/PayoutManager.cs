using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    public class PayoutManager
    {
        PlayerDataManager playerDataManager = new PlayerDataManager();
        Dictionary<string, float> bonuses = new Dictionary<string, float>()
        {
            ["debrisCleaner"] = 1.0f,
            ["warehouse"] = 1.0f,
            ["lawnmowing"] = 1.0f,
            ["junkyard"] = 1.0f,

            ["gardener"] = 1.0f,
            ["fisherman"] = 1.0f,
            ["forklifts"] = 1.0f,
            ["hunter"] = 1.0f,
            ["towtrucks"] = 1.0f,
            ["refinery"] = 1.0f,
            ["diver"] = 1.0f
        };
        public string[] currentBonus;
        public DateTime bonusTime;
        public PayoutManager()
        {

        }

        public void DebrisCleanerPayment(Player player, int weight)
        {
            int points = 0;
            if(weight >= 50)
            {
                points = 2;
            }
            else if(weight >= 25)
            {
                points = 1;
            }
            int waterPoints = Convert.ToInt32(Math.Floor(weight / 10.0));
            int money = weight;
            points = Convert.ToInt32(points * bonuses["debrisCleaner"]);
            AddWaterPoints(player, waterPoints);
            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["debrisCleaner"]);
            playerDataManager.GiveMoney(player, money);
            if(points>0)
                playerDataManager.UpdatePlayersExp(player, points * 8);
            player.TriggerEvent("updateJobVars", money, points * 15, waterPoints);

        }

        public void DiverPayment(Player player, int priceMult)
        {
            int reward = 100 * (priceMult / 2);
            int exp = 10;
            switch(priceMult)
            {
                case 1:
                    playerDataManager.NotifyPlayer(player, "Znalazłeś pospolity przedmiot!");
                    break;
                case 2:
                    playerDataManager.NotifyPlayer(player, "Znalazłeś zwyczajny przedmiot!");
                    break;
                case 3:
                    playerDataManager.NotifyPlayer(player, "Znalazłeś niecodzienny przedmiot!");
                    break;
                case 4:
                    playerDataManager.NotifyPlayer(player, "Znalazłeś rzadki przedmiot!");
                    break;
                case 5:
                    playerDataManager.NotifyPlayer(player, "Znalazłeś bardzo rzadki przedmiot!");
                    break;
            }
            var points = new Random().Next(1, 3);
            AddWaterPoints(player, points);
            reward = Convert.ToInt32(addPlayersBonus(player, reward) * bonuses["diver"]);
            playerDataManager.GiveMoney(player, reward);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", reward, exp, points);
        }

        public void WarehousePayment(Player player)
        {
            Random rnd = new Random();
            int luck = rnd.Next(0, 2);
            int money = 4;
            int exp = Convert.ToInt32(1 * bonuses["warehouse"]);
            AddLogisticPoints(player, 1);
            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["warehouse"]);
            playerDataManager.GiveMoney(player, money);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp, 1);
        }
        public void ForkliftsPayment(Player player)
        {
            Random rnd = new Random();
            int money = rnd.Next(20, 41);
            int exp = 10;
            AddLogisticPoints(player, 1);
            exp = Convert.ToInt32(exp * bonuses["forklifts"]);
            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["forklifts"]);
            playerDataManager.GiveMoney(player, money);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp, 1);
        }
        public void LawnmowingPayment(Player player)
        {
            Random rnd = new Random();
            int luck = rnd.Next(0, 2);
            int points = 25;
            int money = rnd.Next(100, 141);
            points = Convert.ToInt32(points * bonuses["lawnmowing"]);
            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["lawnmowing"]);
            playerDataManager.GiveMoney(player, money);
            if (points != 0)
            {
                AddNaturePoints(player, 2);
                playerDataManager.UpdatePlayersExp(player, points);
            }
            else
            {
                AddNaturePoints(player, 1);
            }
            player.TriggerEvent("updateJobVars", money, points, points != 0 ? 4 : 2);
        }

        public void GardenerPlantsSold(Player player, int exp)
        {
            exp = Convert.ToInt32(exp * bonuses["gardener"]);
            int np = exp / 20;
            AddNaturePoints(player, np);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", 0, exp, np);
        }
        public void GardenerOrderCompleted(Player player, string order)
        {
            int[] baseOrder = JsonConvert.DeserializeObject<int[]>(order);
            int exp = Convert.ToInt32(100 * bonuses["gardener"]);
            int money = (15 * (baseOrder[0] + baseOrder[1] + baseOrder[2])) + (35 * (baseOrder[3] + baseOrder[4]));
            money = Convert.ToInt32(money * bonuses["gardener"]);
            AddNaturePoints(player, 15);
            playerDataManager.GiveMoney(player, money);
            player.TriggerEvent("updateJobVars", money, exp, 15);
        }
        
        public void HunterPaymentAnimal(Player player, string pedstr)
        {
            int exp = 100, np = 0;
            exp = Convert.ToInt32(exp * bonuses["hunter"]);
            switch (pedstr)
            {
                case "1682622302":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś kojota!");
                    np = 3;
                    break;
                case "3462393972":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś dzika!");
                    np = 3;
                    break;
                case "3753204865":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś zająca!");
                    np = 4;
                    break;
                case "1641334641":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś SAMSKŁENCZA!");
                    np = 5;
                    break;
                case "307287994":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś pumę!");
                    np = 5;
                    break;
                case "3630914197":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś jelenia!");
                    np = 3;
                    break;
            }
            AddNaturePoints(player, np);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", 0, exp, np);
            //logManager.LogJobTransaction(player.SocialClubId.ToString(), $"Myśliwy: +{reward.ToString()}$, +1PP ({player.GetSharedData<Int32>("money")},{player.GetSharedData<Int32>("pp")})");
        }

        public void HunterPaymentPelts(Player player, List<string> pelts)
        {
            int reward = 0;
            foreach (string pelt in pelts)
            {
                switch (pelt)
                {
                    case "8":
                        reward += 240;
                        break;
                    case "7":
                        reward += 170;
                        break;
                    case "5":
                        reward += 80;
                        break;
                    case "10":
                        reward += 560;
                        break;
                    case "9":
                        reward += 300;
                        break;
                    case "6":
                        reward += 160;
                        break;
                }
            }
            playerDataManager.NotifyPlayer(player, $"Za skóry otrzymałeś {reward}$!");
            reward = Convert.ToInt32(addPlayersBonus(player, reward) * bonuses["hunter"]);
            playerDataManager.GiveMoney(player, reward);
            player.TriggerEvent("updateJobVars", reward, 0, 0);
            //logManager.LogJobTransaction(player.SocialClubId.ToString(), $"Myśliwy: +{reward.ToString()}$, +1PP ({player.GetSharedData<Int32>("money")},{player.GetSharedData<Int32>("pp")})");
        }
        
        public void FisherManPoints(Player player, int price)
        {
            AddWaterPoints(player, price);
            int exp = Convert.ToInt32(48 * bonuses["fisherman"]);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", 0, exp, price);
        }

        public void FisherSold(Player player, List<string> fishes)
        {
            int reward = 0;
            foreach (string fish in fishes)
            {
                switch (fish)
                {
                    case "1001":
                        reward += 35;
                        break;
                    case "1002":
                        reward += 50;
                        break;
                    case "1003":
                        reward += 80;
                        break;
                    case "1004":
                        reward += 100;
                        break;
                    case "1005":
                        reward += 140;
                        break;
                    case "1006":
                        reward += 200;
                        break;
                    case "1007":
                        reward += 280;
                        break;
                    case "1008":
                        reward += 360;
                        break;
                    case "1009":
                        reward += 480;
                        break;

                    case "1010":
                        reward += 30;
                        break;
                    case "1011":
                        reward += 45;
                        break;
                    case "1012":
                        reward += 85;
                        break;
                    case "1013":
                        reward += 120;
                        break;
                    case "1014":
                        reward += 145;
                        break;
                    case "1015":
                        reward += 155;
                        break;
                    case "1016":
                        reward += 195;
                        break;
                    case "1017":
                        reward += 180;
                        break;
                    case "1018":
                        reward += 255;
                        break;
                    case "1019":
                        reward += 290;
                        break;
                    case "1020":
                        reward += 320;
                        break;
                    case "1021":
                        reward += 470;
                        break;
                    case "1022":
                        reward += 500;
                        break;

                }
            }
            reward = Convert.ToInt32(addPlayersBonus(player, reward) * bonuses["fisherman"]);
            playerDataManager.NotifyPlayer(player, $"Za ryby otrzymałeś {reward}$!");
            playerDataManager.GiveMoney(player, reward);
            //logManager.LogJobTransaction(player.SocialClubId.ToString(), $"Myśliwy: +{reward.ToString()}$, +1PP ({player.GetSharedData<Int32>("money")},{player.GetSharedData<Int32>("pp")})");
        }
        public void FisherJunkSold(Player player, List<string> junks)
        {
            int reward = 0;
            foreach (string junk in junks)
            {
                switch (junk)
                {
                    case "1050":
                        reward += 40;
                        break;
                    case "1051":
                        reward += 80;
                        break;
                    case "1052":
                        reward += 300;
                        break;
                    case "1053":
                        reward += 700;
                        break;
                    case "1054":
                        reward += 1000;
                        break;
                    case "1055":
                        reward += 450;
                        break;
                    case "1056":
                        reward += 200;
                        break;
                    case "1057":
                        reward += 55;
                        break;
                    case "1058":
                        reward += 10;
                        break;
                }
            }
            reward = Convert.ToInt32(addPlayersBonus(player, reward) * bonuses["fisherman"]);
            playerDataManager.NotifyPlayer(player, $"Za przedmioty otrzymałeś {reward}$!");
            playerDataManager.GiveMoney(player, reward);
            //logManager.LogJobTransaction(player.SocialClubId.ToString(), $"Myśliwy: +{reward.ToString()}$, +1PP ({player.GetSharedData<Int32>("money")},{player.GetSharedData<Int32>("pp")})");
        }

        public void SupplierPayment(Player player, int supplies, float dmg)
        {
            Random rnd = new Random();
            int reward = Convert.ToInt32((rnd.Next(150, 220) * supplies) * dmg);
            AddLogisticPoints(player, supplies);
            reward = Convert.ToInt32(addPlayersBonus(player, reward) * bonuses["supplier"]);
            playerDataManager.GiveMoney(player, reward);
            player.TriggerEvent("updateJobVars", reward, 0, supplies);

        }

        public void JunkyardPayment(Player player)
        {
            Random rnd = new Random();
            int money = rnd.Next(6, 12);
            int exp = 2;
            exp = Convert.ToInt32(exp * bonuses["junkyard"]);
            AddSocialPoints(player, 1);
            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["junkyard"]);
            playerDataManager.GiveMoney(player, money);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp, 1);
        }

        public void TowtruckPayment(Player player, string type, float distance, float dmg)
        {
            int reward = 0;
            if(type == "car")
            {
                reward = Convert.ToInt32(distance / 12);
            }
            else
            {
                reward = Convert.ToInt32(distance / 17);
            }
            int exp = 65;
            if (type == "wreck")
                exp = 40;
            exp = Convert.ToInt32(exp * bonuses["towtrucks"]);
            reward = Convert.ToInt32(reward * dmg);
            AddSocialPoints(player, 4);
            reward = Convert.ToInt32(addPlayersBonus(player, reward) * bonuses["towtrucks"]);
            playerDataManager.GiveMoney(player, reward);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", reward, exp, 1);
        }
        public void RefineryPayment(Player player, int liters, int type)
        {
            int reward = (int)(liters * 0.25);
            reward = (int)(reward * (1 + (0.05 * (type - 1))));
            
            int exp = liters / 8;
            
            exp = Convert.ToInt32(exp * bonuses["refinery"]);
            AddSocialPoints(player, 4);
            reward = Convert.ToInt32(addPlayersBonus(player, reward) * bonuses["refinery"]);
            playerDataManager.GiveMoney(player, reward);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", reward, exp, type == 1 ? 3 : type == 2 ? 5 : type == 3 ? 7 : 0);
        }


        public void AddWaterPoints(Player player, int points)
        {
            int wp = player.GetSharedData<Int32>("waterpoints") + points;
            DBConnection dataBase = new DBConnection();
            player.SetSharedData("waterpoints", wp);
            dataBase.command.CommandText = $"UPDATE jobs SET waterpoints = {wp} WHERE player = '{player.SocialClubId.ToString()}'";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }

        public void AddLogisticPoints(Player player, int points)
        {
            int lp = player.GetSharedData<Int32>("logisticpoints") + points;
            DBConnection dataBase = new DBConnection();
            player.SetSharedData("logisticpoints", lp);
            dataBase.command.CommandText = $"UPDATE jobs SET logisticpoints = {lp} WHERE player = '{player.SocialClubId.ToString()}'";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }
        public void AddNaturePoints(Player player, int points)
        {
            int np = player.GetSharedData<Int32>("naturepoints") + points;
            DBConnection dataBase = new DBConnection();
            player.SetSharedData("naturepoints", np);
            dataBase.command.CommandText = $"UPDATE jobs SET naturepoints = {np} WHERE player = '{player.SocialClubId.ToString()}'";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }

        public void AddSocialPoints(Player player, int points)
        {
            int np = player.GetSharedData<Int32>("socialpoints") + points;
            DBConnection dataBase = new DBConnection();
            player.SetSharedData("socialpoints", np);
            dataBase.command.CommandText = $"UPDATE jobs SET socialpoints = {np} WHERE player = '{player.SocialClubId.ToString()}'";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
        }

        private int addPlayersBonus(Player player, int money)
        {
            decimal bonus = (decimal)player.GetSharedData<Int32>("skill-1")/100;
            decimal m = Math.Round(money + (money * bonus));
            int intmoney = Convert.ToInt32(m);
            return intmoney;
        }

        public string[] SetNewBonus()
        {
            List<string> bb = new List<string>()
            {
                "Zbieranie odpadów",
                "Magazynier",
                "Koszenie trawników",
                "Złomowisko",
                "Ogrodnik",
                "Wędkarstwo",
                "Wózki widłowe",
                "Myśliwy",
                "Lawety",
                "Rafineria",
                "Nurek"
            };
            foreach(KeyValuePair<string, float> bon in bonuses.ToList())
            {
                bonuses[bon.Key] = 1.0f;
            }
            Random rand = new Random();
            int b = rand.Next(0, bonuses.Count);
            int bAmount = rand.Next(150, 301);
            bonuses[bonuses.ElementAt(b).Key] = bAmount;
            currentBonus = new string[] { bb[b], bAmount.ToString() };
            return currentBonus;
        }
    }
}

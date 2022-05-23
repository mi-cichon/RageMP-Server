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
        PlayerDataManager playerDataManager;
        Dictionary<string, float> bonuses = new Dictionary<string, float>()
        {
            ["debrisCleaner"] = 1.0f,
            ["warehouse"] = 1.0f,
            ["lawnmowing"] = 1.0f,
            //["junkyard"] = 1.0f,
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
        public PayoutManager(ref PlayerDataManager playerDataManager)
        {
            this.playerDataManager = playerDataManager;
        }

        public void WarehousePayment(Player player)
        {
            double bonus = 1;
            if (player.GetSharedData<bool>("jobBonus_3"))
            {
                bonus = 1.6;
            }
            else if (player.GetSharedData<bool>("jobBonus_2"))
            {
                bonus = 1.4;
            }
            else if (player.GetSharedData<bool>("jobBonus_1"))
            {
                bonus = 1.2;
            }

            
            int exp = player.GetSharedData<bool>("jobBonus_6") ? 1 : 2;

            int money = Convert.ToInt32(4 * bonus);
            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["warehouse"]);


            playerDataManager.UpdatePlayersJobBonus(player, "warehouse", exp);

            playerDataManager.GiveMoney(player, money);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }

        public void ForkliftsPayment(Player player, bool special)
        {
            double bonus = 1;
            if (player.GetSharedData<bool>("jobBonus_10"))
            {
                bonus = 1.25;
            }
            else if (player.GetSharedData<bool>("jobBonus_9"))
            {
                bonus = 1.15;
            }
            else if (player.GetSharedData<bool>("jobBonus_8"))
            {
                bonus = 1.1;
            }

            int exp = 8;

            int money = Convert.ToInt32(30 * bonus);

            if (special)
            {
                money *= 5;
            }

            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["forklifts"]);


            playerDataManager.UpdatePlayersJobBonus(player, "forklifts", exp);

            playerDataManager.GiveMoney(player, money);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }

        public void TowtruckPayment(Player player, string type, float distance, float dmg)
        {
            int money = 0;
            if (type == "car")
            {
                money = Convert.ToInt32(distance / 12);
            }
            else
            {
                money = Convert.ToInt32(distance / 17);
            }

            int exp = 65;
            if (type == "wreck")
                exp = 40;


            playerDataManager.UpdatePlayersJobBonus(player, "towtruck", exp);

            money = Convert.ToInt32(money * dmg);
            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["towtrucks"]);

            playerDataManager.GiveMoney(player, money);
            playerDataManager.UpdatePlayersExp(player, exp);

            player.TriggerEvent("updateJobVars", money, exp);
        }

        public void RefineryPayment(Player player, int liters, int type)
        {
            int money = (int)(liters * 0.25);
            money = (int)(money * (1 + (0.05 * (type - 1))));

            int exp = liters / 8;


            playerDataManager.UpdatePlayersJobBonus(player, "refinery", exp);

            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["refinery"]);
            playerDataManager.GiveMoney(player, money);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }

        public void DebrisCleanerPayment(Player player, int weight)
        {
            int money = weight;

            double bonus = 1;
            if (player.GetSharedData<bool>("jobBonus_59"))
            {
                bonus = 1.5;
            }
            else if (player.GetSharedData<bool>("jobBonus_58"))
            {
                bonus = 1.3;
            }
            else if (player.GetSharedData<bool>("jobBonus_57"))
            {
                bonus = 1.15;
            }
            else if (player.GetSharedData<bool>("jobBonus_56"))
            {
                bonus = 1.05;
            }

            int exp = weight / 15;

            money = Convert.ToInt32(money * bonus);

            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["debrisCleaner"]);

            playerDataManager.UpdatePlayersJobBonus(player, "debriscleaner", exp);

            playerDataManager.GiveMoney(player, money);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }
        public void DiverPayment(Player player, int priceMult)
        {
            int money = Convert.ToInt32(100 * (priceMult / 2.0));
            int exp = 10;
            switch (priceMult)
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

            double bonus = 1;
            if (player.GetSharedData<bool>("jobBonus_65"))
            {
                bonus = 1.25;
            }
            else if (player.GetSharedData<bool>("jobBonus_64"))
            {
                bonus = 1.15;
            }
            else if (player.GetSharedData<bool>("jobBonus_63"))
            {
                bonus = 1.1;
            }

            money = Convert.ToInt32(bonus * money);
            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["diver"]);

            playerDataManager.UpdatePlayersJobBonus(player, "diver", exp);

            playerDataManager.GiveMoney(player, money);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }

        public void FisherManPoints(Player player, int type, int size)
        {
            Random rnd = new Random();
            int price = 0;
            int luck = 0;
            if (type == 0)
            {
                switch (size)
                {
                    case 1:
                        luck = rnd.Next(1, 3);
                        switch (luck)
                        {
                            case 1:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Karpia!");
                                player.TriggerEvent("fitItemInEquipment", 1003);
                                break;
                            case 2:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Amura!");
                                player.TriggerEvent("fitItemInEquipment", 1001);
                                break;
                        }
                        price = 1;
                        break;
                    case 2:
                        luck = rnd.Next(1, 3);
                        switch (luck)
                        {
                            case 1:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Jesiotra!");
                                player.TriggerEvent("fitItemInEquipment", 1002);
                                break;
                            case 2:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Lina!");
                                player.TriggerEvent("fitItemInEquipment", 1004);
                                break;
                        }
                        price = 1;
                        break;
                    case 3:
                        luck = rnd.Next(1, 3);
                        switch (luck)
                        {
                            case 1:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Lipienia!");
                                player.TriggerEvent("fitItemInEquipment", 1005);
                                break;
                            case 2:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Karasia!");
                                player.TriggerEvent("fitItemInEquipment", 1006);
                                break;
                        }
                        price = 1;
                        break;
                    case 4:
                        luck = rnd.Next(1, 4);
                        switch (luck)
                        {
                            case 1:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Okonia!");
                                player.TriggerEvent("fitItemInEquipment", 1007);
                                break;
                            case 2:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Suma!");
                                player.TriggerEvent("fitItemInEquipment", 1008);
                                break;
                            case 3:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Szczupaka!");
                                player.TriggerEvent("fitItemInEquipment", 1009);
                                break;
                        }
                        price = 2;
                        break;
                    case 5:
                        luck = rnd.Next(1, 6);
                        switch (luck)
                        {
                            case 1:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś stary gumowiec!");
                                player.TriggerEvent("fitItemInEquipment", 1050);
                                price = 1;
                                break;
                            case 2:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś stary garnek!");
                                player.TriggerEvent("fitItemInEquipment", 1051);
                                price = 1;
                                break;
                            case 3:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś zepsuty telefon!");
                                player.TriggerEvent("fitItemInEquipment", 1052);
                                price = 2;
                                break;
                            case 4:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś złoty zegarek!");
                                player.TriggerEvent("fitItemInEquipment", 1053);
                                price = 2;
                                break;
                            case 5:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś złoty pierścionek!");
                                player.TriggerEvent("fitItemInEquipment", 1054);
                                price = 3;
                                break;
                        }
                        break;
                }
            }
            else
            {
                switch (size)
                {
                    case 1:
                        luck = rnd.Next(1, 4);
                        switch (luck)
                        {
                            case 1:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Tobiasza!");
                                player.TriggerEvent("fitItemInEquipment", 1010);
                                break;
                            case 2:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Szprotę!");
                                player.TriggerEvent("fitItemInEquipment", 1011);
                                break;
                            case 3:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Krąpia!");
                                player.TriggerEvent("fitItemInEquipment", 1012);
                                break;
                        }
                        price = 1;
                        break;
                    case 2:
                        luck = rnd.Next(1, 4);
                        switch (luck)
                        {
                            case 1:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Dorsza!");
                                player.TriggerEvent("fitItemInEquipment", 1013);
                                break;
                            case 2:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Belonę!");
                                player.TriggerEvent("fitItemInEquipment", 1014);
                                break;
                            case 3:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Łososia!");
                                player.TriggerEvent("fitItemInEquipment", 1016);
                                break;
                        }
                        price = 1;
                        break;
                    case 3:
                        luck = rnd.Next(1, 4);
                        switch (luck)
                        {
                            case 1:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Sieję!");
                                player.TriggerEvent("fitItemInEquipment", 1017);
                                break;
                            case 2:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Halibuta!");
                                player.TriggerEvent("fitItemInEquipment", 1018);
                                break;
                            case 3:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Ciernika!");
                                player.TriggerEvent("fitItemInEquipment", 1019);
                                break;
                        }
                        price = 1;
                        break;
                    case 4:
                        luck = rnd.Next(1, 4);
                        switch (luck)
                        {
                            case 1:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Flądrę!");
                                player.TriggerEvent("fitItemInEquipment", 1020);
                                break;
                            case 2:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Węgorzycę!");
                                player.TriggerEvent("fitItemInEquipment", 1021);
                                break;
                            case 3:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś Węgorza!");
                                player.TriggerEvent("fitItemInEquipment", 1022);
                                break;
                        }
                        price = 2;
                        break;
                    case 5:
                        luck = rnd.Next(1, 6);
                        switch (luck)
                        {
                            case 1:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś starą skarpetę!");
                                player.TriggerEvent("fitItemInEquipment", 1058);
                                price = 1;
                                break;
                            case 2:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś stary nóż!");
                                player.TriggerEvent("fitItemInEquipment", 1056);
                                price = 1;
                                break;
                            case 3:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś zepsuty aparat!");
                                player.TriggerEvent("fitItemInEquipment", 1055);
                                price = 2;
                                break;
                            case 4:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś stare okulary!");
                                player.TriggerEvent("fitItemInEquipment", 1057);
                                price = 2;
                                break;
                            case 5:
                                playerDataManager.NotifyPlayer(player, "Złowiłeś złoty pierścionek!");
                                player.TriggerEvent("fitItemInEquipment", 1054);
                                price = 3;
                                break;
                        }
                        break;
                }
            }



            int exp = price * 10;
            playerDataManager.UpdatePlayersExp(player, exp);
            
            playerDataManager.UpdatePlayersJobBonus(player, "fisherman", exp);

            player.TriggerEvent("updateJobVars", 0, exp);
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
        }

        public void LawnmowingPayment(Player player)
        {
            double bonus = 1;
            if (player.GetSharedData<bool>("jobBonus_107"))
            {
                bonus = 1.5;
            }
            else if (player.GetSharedData<bool>("jobBonus_106"))
            {
                bonus = 1.3;
            }
            else if (player.GetSharedData<bool>("jobBonus_105"))
            {
                bonus = 1.15;
            }
            else if (player.GetSharedData<bool>("jobBonus_104"))
            {
                bonus = 1.05;
            }


            int money = 120;
            int exp = 20;

            money = Convert.ToInt32(money * bonus);

            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["lawnmowing"]);

            playerDataManager.UpdatePlayersJobBonus(player, "lawnmowing", exp);

            playerDataManager.GiveMoney(player, money);
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }

        public void GardenerPlantsSold(Player player, int exp)
        {
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", 0, exp);

            playerDataManager.UpdatePlayersJobBonus(player, "gardener", exp);

        }

        public void GardenerOrderCompleted(Player player, string order)
        {
            double bonus = 1;
            if (player.GetSharedData<bool>("jobBonus_116"))
            {
                bonus = 1.25;
            }
            else if (player.GetSharedData<bool>("jobBonus_115"))
            {
                bonus = 1.15;
            }
            else if (player.GetSharedData<bool>("jobBonus_114"))
            {
                bonus = 1.05;
            }

            int[] baseOrder = JsonConvert.DeserializeObject<int[]>(order);

            int exp = 100;
            int money = (15 * (baseOrder[0] + baseOrder[1] + baseOrder[2])) + (35 * (baseOrder[3] + baseOrder[4]));
            money = Convert.ToInt32(money * bonuses["gardener"]);
            money = Convert.ToInt32(money * bonus);



            playerDataManager.GiveMoney(player, money);
            player.TriggerEvent("updateJobVars", money, exp);

            playerDataManager.UpdatePlayersJobBonus(player, "gardener", exp);
        }
        
        public void HunterPaymentAnimal(Player player, string pedstr)
        {
            int exp = 75;
            switch (pedstr)
            {
                case "1682622302":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś kojota!");
                    break;
                case "3462393972":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś dzika!");
                    break;
                case "3753204865":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś zająca!");
                    break;
                case "1641334641":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś SAMSKŁENCZA!");
                    break;
                case "307287994":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś pumę!");
                    break;
                case "3630914197":
                    playerDataManager.NotifyPlayer(player, "Upolowałeś jelenia!");
                    break;
            }
            playerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", 0, exp);

            playerDataManager.UpdatePlayersJobBonus(player, "gardener", exp);
            //logManager.LogJobTransaction(player.SocialClubId.ToString(), $"Myśliwy: +{reward.ToString()}$, +1PP ({player.GetSharedData<Int32>("money")},{player.GetSharedData<Int32>("pp")})");
        }

        public void HunterPaymentPelts(Player player, List<string> pelts)
        {
            int money = 0;
            foreach (string pelt in pelts)
            {
                switch (pelt)
                {
                    case "8":
                        money += 240;
                        break;
                    case "7":
                        money += 170;
                        break;
                    case "5":
                        money += 80;
                        break;
                    case "10":
                        money += 560;
                        break;
                    case "9":
                        money += 300;
                        break;
                    case "6":
                        money += 160;
                        break;
                }
            }
            playerDataManager.NotifyPlayer(player, $"Za skóry otrzymałeś {money}$!");
            money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["hunter"]);
            playerDataManager.GiveMoney(player, money);
            player.TriggerEvent("updateJobVars", money, 0);
            //logManager.LogJobTransaction(player.SocialClubId.ToString(), $"Myśliwy: +{reward.ToString()}$, +1PP ({player.GetSharedData<Int32>("money")},{player.GetSharedData<Int32>("pp")})");
        }
        
        

        

        //public void SupplierPayment(Player player, int supplies, float dmg)
        //{
        //    Random rnd = new Random();
        //    int reward = Convert.ToInt32((rnd.Next(150, 220) * supplies) * dmg);
        //    reward = Convert.ToInt32(addPlayersBonus(player, reward) * bonuses["supplier"]);
        //    playerDataManager.GiveMoney(player, reward);
        //    player.TriggerEvent("updateJobVars", reward, 0);

        //}

        //public void JunkyardPayment(Player player)
        //{
        //    Random rnd = new Random();
        //    int money = rnd.Next(6, 12);
        //    int exp = 2;
        //    exp = Convert.ToInt32(exp * bonuses["junkyard"]);
        //    money = Convert.ToInt32(addPlayersBonus(player, money) * bonuses["junkyard"]);
        //    playerDataManager.GiveMoney(player, money);
        //    playerDataManager.UpdatePlayersExp(player, exp);
        //    player.TriggerEvent("updateJobVars", money, exp);
        //}

        


        //public void AddWaterPoints(Player player, int points)
        //{
        //    int wp = player.GetSharedData<Int32>("waterpoints") + points;
        //    DBConnection dataBase = new DBConnection();
        //    player.SetSharedData("waterpoints", wp);
        //    dataBase.command.CommandText = $"UPDATE jobs SET waterpoints = {wp} WHERE player = '{player.SocialClubId.ToString()}'";
        //    dataBase.command.ExecuteNonQuery();
        //    dataBase.connection.Close();
        //}

        //public void AddLogisticPoints(Player player, int points)
        //{
        //    int lp = player.GetSharedData<Int32>("logisticpoints") + points;
        //    DBConnection dataBase = new DBConnection();
        //    player.SetSharedData("logisticpoints", lp);
        //    dataBase.command.CommandText = $"UPDATE jobs SET logisticpoints = {lp} WHERE player = '{player.SocialClubId.ToString()}'";
        //    dataBase.command.ExecuteNonQuery();
        //    dataBase.connection.Close();
        //}
        //public void AddNaturePoints(Player player, int points)
        //{
        //    int np = player.GetSharedData<Int32>("naturepoints") + points;
        //    DBConnection dataBase = new DBConnection();
        //    player.SetSharedData("naturepoints", np);
        //    dataBase.command.CommandText = $"UPDATE jobs SET naturepoints = {np} WHERE player = '{player.SocialClubId.ToString()}'";
        //    dataBase.command.ExecuteNonQuery();
        //    dataBase.connection.Close();
        //}

        //public void AddSocialPoints(Player player, int points)
        //{
        //    int np = player.GetSharedData<Int32>("socialpoints") + points;
        //    DBConnection dataBase = new DBConnection();
        //    player.SetSharedData("socialpoints", np);
        //    dataBase.command.CommandText = $"UPDATE jobs SET socialpoints = {np} WHERE player = '{player.SocialClubId.ToString()}'";
        //    dataBase.command.ExecuteNonQuery();
        //    dataBase.connection.Close();
        //}

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
                //"Złomowisko",
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

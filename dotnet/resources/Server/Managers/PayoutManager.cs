using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    public static class PayoutManager
    {
        static Dictionary<string, float> bonuses = new Dictionary<string, float>()
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
        public static string[] currentBonus;
        public static DateTime bonusTime;

        public static void WarehousePayment(Player player)
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
            money = Convert.ToInt32(AddPlayersBonus(player, money) * (IsPlayerTesting(player) ? 1 : bonuses["warehouse"]));


            PlayerDataManager.UpdatePlayersJobBonus(player, "warehouse", exp);

            PlayerDataManager.GiveMoney(player, money);
            PlayerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }

        public static void ForkliftsPayment(Player player, bool special)
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

            money = Convert.ToInt32(AddPlayersBonus(player, money) * (IsPlayerTesting(player) ? 1 : bonuses["forklifts"]));


            PlayerDataManager.UpdatePlayersJobBonus(player, "forklifts", exp);

            PlayerDataManager.GiveMoney(player, money);
            PlayerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }

        public static void TowtruckPayment(Player player, string type, float distance, float dmg)
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


            PlayerDataManager.UpdatePlayersJobBonus(player, "towtruck", exp);

            money = Convert.ToInt32(money * dmg);
            money = Convert.ToInt32(AddPlayersBonus(player, money) * (IsPlayerTesting(player) ? 1 : bonuses["towtrucks"]));

            PlayerDataManager.GiveMoney(player, money);
            PlayerDataManager.UpdatePlayersExp(player, exp);

            player.TriggerEvent("updateJobVars", money, exp);
        }

        public static void RefineryPayment(Player player, int liters, int type)
        {
            int money = (int)(liters * 0.25);
            money = (int)(money * (1 + (0.05 * (type - 1))));

            int exp = liters / 8;


            PlayerDataManager.UpdatePlayersJobBonus(player, "refinery", exp);

            money = Convert.ToInt32(AddPlayersBonus(player, money) * (IsPlayerTesting(player) ? 1 : bonuses["refinery"]));
            PlayerDataManager.GiveMoney(player, money);
            PlayerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }

        public static void DebrisCleanerPayment(Player player, int weight)
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

            money = Convert.ToInt32(AddPlayersBonus(player, money) * (IsPlayerTesting(player) ? 1 : bonuses["debrisCleaner"]));

            PlayerDataManager.UpdatePlayersJobBonus(player, "debriscleaner", exp);

            PlayerDataManager.GiveMoney(player, money);
            PlayerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }
        public static void DiverPayment(Player player, int priceMult)
        {
            int money = Convert.ToInt32(100 * (priceMult / 2.0));
            int exp = 10;
            switch (priceMult)
            {
                case 1:
                    PlayerDataManager.NotifyPlayer(player, "Znalazłeś pospolity przedmiot!");
                    break;
                case 2:
                    PlayerDataManager.NotifyPlayer(player, "Znalazłeś zwyczajny przedmiot!");
                    break;
                case 3:
                    PlayerDataManager.NotifyPlayer(player, "Znalazłeś niecodzienny przedmiot!");
                    break;
                case 4:
                    PlayerDataManager.NotifyPlayer(player, "Znalazłeś rzadki przedmiot!");
                    break;
                case 5:
                    PlayerDataManager.NotifyPlayer(player, "Znalazłeś bardzo rzadki przedmiot!");
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
            money = Convert.ToInt32(AddPlayersBonus(player, money) * (IsPlayerTesting(player) ? 1 : bonuses["diver"]));

            PlayerDataManager.UpdatePlayersJobBonus(player, "diver", exp);

            PlayerDataManager.GiveMoney(player, money);
            PlayerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }

        public static void FisherManPoints(Player player, int type, int size)
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
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Karpia!");
                                player.TriggerEvent("fitItemInEquipment", 1003);
                                break;
                            case 2:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Amura!");
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
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Jesiotra!");
                                player.TriggerEvent("fitItemInEquipment", 1002);
                                break;
                            case 2:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Lina!");
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
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Lipienia!");
                                player.TriggerEvent("fitItemInEquipment", 1005);
                                break;
                            case 2:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Karasia!");
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
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Okonia!");
                                player.TriggerEvent("fitItemInEquipment", 1007);
                                break;
                            case 2:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Suma!");
                                player.TriggerEvent("fitItemInEquipment", 1008);
                                break;
                            case 3:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Szczupaka!");
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
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś stary gumowiec!");
                                player.TriggerEvent("fitItemInEquipment", 1050);
                                price = 1;
                                break;
                            case 2:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś stary garnek!");
                                player.TriggerEvent("fitItemInEquipment", 1051);
                                price = 1;
                                break;
                            case 3:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś zepsuty telefon!");
                                player.TriggerEvent("fitItemInEquipment", 1052);
                                price = 2;
                                break;
                            case 4:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś złoty zegarek!");
                                player.TriggerEvent("fitItemInEquipment", 1053);
                                price = 2;
                                break;
                            case 5:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś złoty pierścionek!");
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
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Tobiasza!");
                                player.TriggerEvent("fitItemInEquipment", 1010);
                                break;
                            case 2:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Szprotę!");
                                player.TriggerEvent("fitItemInEquipment", 1011);
                                break;
                            case 3:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Krąpia!");
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
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Dorsza!");
                                player.TriggerEvent("fitItemInEquipment", 1013);
                                break;
                            case 2:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Belonę!");
                                player.TriggerEvent("fitItemInEquipment", 1014);
                                break;
                            case 3:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Łososia!");
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
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Sieję!");
                                player.TriggerEvent("fitItemInEquipment", 1017);
                                break;
                            case 2:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Halibuta!");
                                player.TriggerEvent("fitItemInEquipment", 1018);
                                break;
                            case 3:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Ciernika!");
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
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Flądrę!");
                                player.TriggerEvent("fitItemInEquipment", 1020);
                                break;
                            case 2:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Węgorzycę!");
                                player.TriggerEvent("fitItemInEquipment", 1021);
                                break;
                            case 3:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś Węgorza!");
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
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś starą skarpetę!");
                                player.TriggerEvent("fitItemInEquipment", 1058);
                                price = 1;
                                break;
                            case 2:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś stary nóż!");
                                player.TriggerEvent("fitItemInEquipment", 1056);
                                price = 1;
                                break;
                            case 3:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś zepsuty aparat!");
                                player.TriggerEvent("fitItemInEquipment", 1055);
                                price = 2;
                                break;
                            case 4:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś stare okulary!");
                                player.TriggerEvent("fitItemInEquipment", 1057);
                                price = 2;
                                break;
                            case 5:
                                PlayerDataManager.NotifyPlayer(player, "Złowiłeś złoty pierścionek!");
                                player.TriggerEvent("fitItemInEquipment", 1054);
                                price = 3;
                                break;
                        }
                        break;
                }
            }



            int exp = price * 10;
            PlayerDataManager.UpdatePlayersExp(player, exp);
            
            PlayerDataManager.UpdatePlayersJobBonus(player, "fisherman", exp);

            player.TriggerEvent("updateJobVars", 0, exp);
        }


        public static void FisherSold(Player player, List<string> fishes)
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
            reward = Convert.ToInt32(AddPlayersBonus(player, reward) * (IsPlayerTesting(player) ? 1 : bonuses["fisherman"]));
            PlayerDataManager.NotifyPlayer(player, $"Za ryby otrzymałeś {reward}$!");
            PlayerDataManager.GiveMoney(player, reward);
        }
        public static void FisherJunkSold(Player player, List<string> junks)
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
            reward = Convert.ToInt32(AddPlayersBonus(player, reward) * (IsPlayerTesting(player) ? 1 : bonuses["fisherman"]));
            PlayerDataManager.NotifyPlayer(player, $"Za przedmioty otrzymałeś {reward}$!");
            PlayerDataManager.GiveMoney(player, reward);
        }

        public static void LawnmowingPayment(Player player, float amount)
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


            int money = Convert.ToInt32(amount * 8);
            int exp = 20;

            money = Convert.ToInt32(money * bonus);

            money = Convert.ToInt32(AddPlayersBonus(player, money) * (IsPlayerTesting(player) ? 1 : bonuses["lawnmowing"]));

            PlayerDataManager.UpdatePlayersJobBonus(player, "lawnmowing", exp);

            PlayerDataManager.GiveMoney(player, money);
            PlayerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", money, exp);
        }

        public static void GardenerPlantsSold(Player player, int exp)
        {
            PlayerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", 0, exp);

            PlayerDataManager.UpdatePlayersJobBonus(player, "gardener", exp);

        }

        public static void GardenerOrderCompleted(Player player, string order)
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
            money = Convert.ToInt32(money * (IsPlayerTesting(player) ? 1 : bonuses["gardener"]));
            money = Convert.ToInt32(money * bonus);



            PlayerDataManager.GiveMoney(player, money);
            player.TriggerEvent("updateJobVars", money, exp);

            PlayerDataManager.UpdatePlayersJobBonus(player, "gardener", exp);
        }
        
        public static void HunterPaymentAnimal(Player player, string pedstr)
        {
            int exp = 75;
            switch (pedstr)
            {
                case "1682622302":
                    PlayerDataManager.NotifyPlayer(player, "Upolowałeś kojota!");
                    break;
                case "3462393972":
                    PlayerDataManager.NotifyPlayer(player, "Upolowałeś dzika!");
                    break;
                case "3753204865":
                    PlayerDataManager.NotifyPlayer(player, "Upolowałeś zająca!");
                    break;
                case "1641334641":
                    PlayerDataManager.NotifyPlayer(player, "Upolowałeś SAMSKŁENCZA!");
                    break;
                case "307287994":
                    PlayerDataManager.NotifyPlayer(player, "Upolowałeś pumę!");
                    break;
                case "3630914197":
                    PlayerDataManager.NotifyPlayer(player, "Upolowałeś jelenia!");
                    break;
            }
            PlayerDataManager.UpdatePlayersExp(player, exp);
            player.TriggerEvent("updateJobVars", 0, exp);

            PlayerDataManager.UpdatePlayersJobBonus(player, "gardener", exp);
            //LogManager.LogJobTransaction(player.SocialClubId.ToString(), $"Myśliwy: +{reward.ToString()}$, +1PP ({player.GetSharedData<Int32>("money")},{player.GetSharedData<Int32>("pp")})");
        }

        public static void HunterPaymentPelts(Player player, List<string> pelts)
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
            PlayerDataManager.NotifyPlayer(player, $"Za skóry otrzymałeś {money}$!");
            money = Convert.ToInt32(AddPlayersBonus(player, money) * (IsPlayerTesting(player) ? 1 : bonuses["hunter"]));
            PlayerDataManager.GiveMoney(player, money);
            player.TriggerEvent("updateJobVars", money, 0);
            //LogManager.LogJobTransaction(player.SocialClubId.ToString(), $"Myśliwy: +{reward.ToString()}$, +1PP ({player.GetSharedData<Int32>("money")},{player.GetSharedData<Int32>("pp")})");
        }

        private static bool IsPlayerTesting(Player player)
        {
            if (player != null && player.Exists && player.HasSharedData("tests_testing") && player.GetSharedData<bool>("tests_testing"))
            {
                return true;
            }
            return false;
        }
        private static int AddPlayersBonus(Player player, int money)
        {
            decimal bonus = (decimal)player.GetSharedData<Int32>("skill-1")/100;
            decimal m = Math.Round(money + (money * bonus));
            int intmoney = Convert.ToInt32(m);
            return intmoney;
        }

        public static string[] SetNewBonus()
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

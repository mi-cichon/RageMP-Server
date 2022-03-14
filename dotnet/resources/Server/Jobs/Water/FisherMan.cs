using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class FisherMan
    {

        Dictionary<Vector3, float> lakeFishingSpots = new Dictionary<Vector3, float>()
        {
            [new Vector3(-193.46588f, 789.7725f, 198.11504f)] = 8.0f,
            [new Vector3(713.9002f, 4100.3135f, 32.78519f)] = 10.0f,
            [new Vector3(1685.9248f, 41.874275f, 161.76726f)] = 10.0f,
        };

        Dictionary<Vector3, float> oceanFishingSpots = new Dictionary<Vector3, float>()
        {
            [new Vector3(-3428.4202f, 968.72675f, 8.346693f)] = 25.0f,
            [new Vector3(-1848.2826f, -1252.0848f, 8.615788f)] = 30.0f
        };

        const int rodPrice = 3000;
        ColShape fisherColshape, sellingColshape, paserColshape;
        PlayerDataManager playerDataManager = new PlayerDataManager();
        PayoutManager payoutManager = new PayoutManager();
        public FisherMan()
        {
            Vector3 fisherPosition = new Vector3(11.549497f, -2799.747f, 2.526085f);
            Vector3 sellingPosition = new Vector3(13.508561f, -2799.8037f, 2.5307877f);
            Vector3 paserPosition = new Vector3(-1286.6385f, -833.0574f, 17.099173f);
            float fisherHeading = -8.0f;
            float sellingHeading = 2.0f;
            float paserHeading = -85.0f;

            NAPI.Ped.CreatePed((uint)PedHash.ChiGoon01GMM, fisherPosition, fisherHeading, frozen: true, invincible: true);
            NAPI.TextLabel.CreateTextLabel("Wędkarz", new Vector3(fisherPosition.X, fisherPosition.Y, fisherPosition.Z + 1.3), 10.0f, 0.2f, 0, new Color(255, 255, 255));
            fisherColshape = NAPI.ColShape.CreateCylinderColShape(fisherPosition, 1.0f, 2.0f);
            fisherColshape.SetSharedData("type", "fisherman");
            NAPI.Blip.CreateBlip(762, fisherPosition, 0.8f, 69, name: "Wędkarz i skup ryb (Wędkarstwo)", shortRange: true);

            NAPI.Ped.CreatePed((uint)PedHash.Factory01SMY, sellingPosition, sellingHeading, frozen: true, invincible: true);
            NAPI.TextLabel.CreateTextLabel("Skup ryb", new Vector3(sellingPosition.X, sellingPosition.Y, sellingPosition.Z + 1.3), 10.0f, 0.2f, 0, new Color(255, 255, 255));
            sellingColshape = NAPI.ColShape.CreateCylinderColShape(new Vector3(13.362058f, -2797.8928f, 2.5259519f), 1.0f, 1.0f);
            sellingColshape.SetSharedData("type", "fishseller");

            NAPI.Ped.CreatePed((uint)PedHash.ArmBoss01GMM, paserPosition, paserHeading, frozen: true, invincible: true);
            NAPI.TextLabel.CreateTextLabel("Paser", new Vector3(paserPosition.X, paserPosition.Y, paserPosition.Z + 1.3), 10.0f, 0.2f, 0, new Color(255, 255, 255));
            paserColshape = NAPI.ColShape.CreateCylinderColShape(paserPosition, 1.0f, 1.0f);
            paserColshape.SetSharedData("type", "fisherpaser");
            NAPI.Blip.CreateBlip(408, paserPosition, 0.8f, 69, name: "Paser (Wędkarstwo)", shortRange: true);


            foreach (KeyValuePair<Vector3, float> spot in lakeFishingSpots)
            {
                ColShape fishingSpot = NAPI.ColShape.CreateCylinderColShape(spot.Key, spot.Value, 4.0f);
                fishingSpot.SetSharedData("type", "fishingspot");
                fishingSpot.SetSharedData("spottype", "lake");
                NAPI.Blip.CreateBlip(760, spot.Key, 0.6f, 69, name: "Łowisko słodkowodne (Wędkarstwo)", shortRange: true);
            }

            foreach (KeyValuePair<Vector3, float> spot in oceanFishingSpots)
            {
                ColShape fishingSpot = NAPI.ColShape.CreateCylinderColShape(spot.Key, spot.Value, 4.0f);
                fishingSpot.SetSharedData("type", "fishingspot");
                fishingSpot.SetSharedData("spottype", "ocean");
                NAPI.Blip.CreateBlip(760, spot.Key, 0.6f, 69, name: "Łowisko słonowodne (Wędkarstwo)", shortRange: true);
            }
        }

        public void StartJob(Player player)
        {
            if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                if (player.GetSharedData<Int32>("waterpoints") >= 750)
                {
                    if (playerDataManager.HasItem(player, 1000))
                    {
                        player.SetSharedData("job", "fisherman");
                        player.TriggerEvent("startJob", "Wędkarstwo", "PW");
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie posiadasz wędki! Zakup ją u wędkarza!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie posiadasz wystarczająco PW: 750!");
                }

            }
        }
        public void BuyFishingRod(Player player)
        {
            if (player.GetSharedData<Int32>("waterpoints") >= 150)
            {
                if (player.GetSharedData<Int32>("money") >= 3000)
                {
                    player.TriggerEvent("fitItemInEquipment", 1000);
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie stać Cię na wędkę!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie posiadasz wystarczająco PW: 150!");
            }
        }
        public void ConfirmFishingRod(Player player)
        {
            playerDataManager.UpdatePlayersMoney(player, -3000);
        }

        public void Done(Player player, int size, int type)
        {
            Random rnd = new Random();
            int price = 0;
            int luck = 0;
            if(type == 0)
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
            
            payoutManager.FisherManPoints(player, price);
        }
    }
}

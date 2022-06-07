using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public static class FisherMan
    {

        static Dictionary<Vector3, float> lakeFishingSpots = new Dictionary<Vector3, float>()
        {
            [new Vector3(-193.46588f, 789.7725f, 198.11504f)] = 8.0f,
            [new Vector3(713.9002f, 4100.3135f, 32.78519f)] = 10.0f,
            [new Vector3(1685.9248f, 41.874275f, 161.76726f)] = 10.0f,
        };

        static Dictionary<Vector3, float> oceanFishingSpots = new Dictionary<Vector3, float>()
        {
            [new Vector3(-3428.4202f, 968.72675f, 8.346693f)] = 25.0f,
            [new Vector3(-1848.2826f, -1252.0848f, 8.615788f)] = 30.0f
        };

        static int rodPrice = 3000;
        static ColShape fisherColshape, sellingColshape, paserColshape;
        public static  void InstantiateFisherMan()
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

        public static void StartJob(Player player)
        {
            if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                if (player.GetSharedData<bool>("jobBonus_66"))
                {
                    
                    if (PlayerDataManager.HasItem(player, 1000))
                    {
                        if (player.GetSharedData<bool>("jobBonus_72"))
                        {
                            player.SetSharedData("job", "fisherman");
                            player.TriggerEvent("startJob", "Wędkarstwo", "PW");
                            player.SetSharedData("fisherman_rodType", 4);
                            return;
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Nie możesz używać karbonowej wędki!");
                        }
                    }
                    if (PlayerDataManager.HasItem(player, 999))
                    {
                        if (player.GetSharedData<bool>("jobBonus_71"))
                        {
                            player.SetSharedData("job", "fisherman");
                            player.TriggerEvent("startJob", "Wędkarstwo", "PW");
                            player.SetSharedData("fisherman_rodType", 3);
                            return;
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Nie możesz używać plastikowej wędki!");
                        }
                    }
                    if(PlayerDataManager.HasItem(player, 998))
                    {
                        if (player.GetSharedData<bool>("jobBonus_70"))
                        {
                            player.SetSharedData("job", "fisherman");
                            player.TriggerEvent("startJob", "Wędkarstwo", "PW");
                            player.SetSharedData("fisherman_rodType", 2);
                            return;
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Nie możesz używać drewnianej wędki!");
                        }
                    }
                    if(PlayerDataManager.HasItem(player, 997))
                    {
                        player.SetSharedData("job", "fisherman");
                        player.TriggerEvent("startJob", "Wędkarstwo", "PW");
                        player.SetSharedData("fisherman_rodType", 1);
                        return;
                    }


                    PlayerDataManager.NotifyPlayer(player, "Nie posiadasz wędki! Zakup ją u wędkarza!");

                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie odblokowałeś tej pracy!");
                }

            }
        }
        public static void BuyFishingRod(Player player)
        {
            if (player.GetSharedData<Int32>("waterpoints") >= 150)
            {
                if (player.GetSharedData<Int32>("money") >= 3000)
                {
                    player.TriggerEvent("fitItemInEquipment", 1000);
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie stać Cię na wędkę!");
                }
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie posiadasz wystarczająco PW: 150!");
            }
        }
        public static void ConfirmFishingRod(Player player)
        {
            PlayerDataManager.UpdatePlayersMoney(player, -3000);
        }
    }
}

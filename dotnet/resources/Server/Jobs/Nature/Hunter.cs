using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using ServerSide;

namespace ServerSide.Jobs
{
    public class Hunter
    {
        public List<Vector3> positions = new List<Vector3>()
        {
            new Vector3(-1580.8352f, 4679.4473f, 45.14447f),
            new Vector3(-1619.7543f, 4604.0776f, 41.948223f),
            new Vector3(-1693.181f, 4595.059f, 47.631176f),
            new Vector3(-1649.3699f, 4540.9985f, 40.550636f),
            new Vector3(-1611.3374f, 4553.2456f, 40.899166f),
            new Vector3(-1518.8733f, 4561.575f, 38.66934f),
            new Vector3(-1456.5646f, 4535.3135f, 57.391106f),
            new Vector3(-1380.8135f, 4555.4814f, 70.42914f),
            new Vector3(-1467.9082f, 4608.144f, 49.24176f),
            new Vector3(-1502.7765f, 4578.9604f, 34.671295f),
            new Vector3(-1532.5311f, 4664.451f, 35.404358f),
            new Vector3(-1505.4167f, 4723.0054f, 43.86909f),
            new Vector3(-1463.0026f, 4727.129f, 49.370384f),
            new Vector3(-1427.9056f, 4718.429f, 42.212803f),
            new Vector3(-1394.8989f, 4688.4375f, 63.778416f),
            new Vector3(-1390.8667f, 4632.1465f, 75.71304f),
            new Vector3(-1585.6437f, 4646.318f, 46.903564f),
            new Vector3(-1662.8322f, 4685.486f, 31.554386f),
            new Vector3(-1308.5585f, 4742.3096f, 89.68926f),
            new Vector3(-1270.0265f, 4687.861f, 84.734f),
            new Vector3(-1291.169f, 4639.797f, 106.294754f),
            new Vector3(-1337.2549f, 4635.641f, 119.26861f),
            new Vector3(-1334.1202f, 4693.583f, 67.92821f)
    };

        List<PedHash> animalsToHunt = new List<PedHash>()
        {
            PedHash.Deer, //25
            PedHash.Rabbit, //25
            PedHash.Boar, //20
            PedHash.Coyote, //15
            PedHash.MountainLion, //10
            PedHash.Orleans //5
        };

        int[] probabilities = new int[]
        {
            0,0,0,0,0,1,1,1,1,1,2,2,2,2,3,3,3,4,4,5
        };

        int[] luckyProbabilities = new int[]
        {
            0,1,2,3,4,5
        };

        PlayerDataManager playerDataManager = new PlayerDataManager();
        ColShape hunterColshape;
        Blip hunterBlip;
        Ped hunterPed;
        TextLabel pedText;
        public Hunter(Vector3 startPoint, Vector3 pedPoint, float pedHeading)
        {
            hunterColshape = NAPI.ColShape.CreateCylinderColShape(startPoint, 1.0f, 2.0f);
            hunterColshape.SetSharedData("type", "hunter");
            new CustomMarkers().CreateJobMarker(startPoint, "Myśliwy");
            hunterColshape = NAPI.ColShape.CreateCylinderColShape(pedPoint, 1.0f, 2.0f);
            hunterColshape.SetSharedData("type", "hunter-sell");
            hunterBlip = NAPI.Blip.CreateBlip(463, startPoint, 0.8f, 69, name: "Praca: Myśliwy", shortRange: true);
            hunterPed = NAPI.Ped.CreatePed((uint)PedHash.Hunter, pedPoint, pedHeading, frozen: true, invincible: true);
            pedText = NAPI.TextLabel.CreateTextLabel("Myśliwy", new Vector3(pedPoint.X, pedPoint.Y, pedPoint.Z + 1.3f), 10.0f, 0.6f, 4, new Color(255, 255, 255));
        }

        public void startJob(Player player)
        {
            if(player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                if (player.GetSharedData<bool>("jobBonus_117"))
                {
                    if (player.GetSharedData<bool>("jobBonus_118"))
                    {
                        player.GiveWeapon(WeaponHash.Sniperrifle, 9999);
                    }
                    else
                    {
                        player.GiveWeapon(WeaponHash.Pumpshotgun, 9999);
                    }
                    playerDataManager.NotifyPlayer(player, "Praca rozpoczęta!");
                    player.SetSharedData("job", "hunter");
                    GetRandomAnimalAndSendToPlayer(player);
                    player.TriggerEvent("startJob", "Myśliwy", "PN");
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie odblokowałeś tej pracy!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Masz inną pracę!");
            }
        }

        public void GetRandomAnimalAndSendToPlayer(Player player)
        {
            Random rnd = new Random();

            int luck = 0;
            if(player.GetSharedData<bool>("jobBonus_123"))
            {
                luck = 10;
            }
            else if (player.GetSharedData<bool>("jobBonus_122"))
            {
                luck = 14;
            }
            else if (player.GetSharedData<bool>("jobBonus_121"))
            {
                luck = 20;
            }

            bool betterProb = luck == 0 ? false : rnd.Next(0, luck) == 0;

            PedHash animal;

            if (betterProb)
            {
                animal = animalsToHunt[luckyProbabilities[rnd.Next(0, probabilities.Length)]];
            }
            else
            {
                animal = animalsToHunt[probabilities[rnd.Next(0, probabilities.Length)]];
            }

            rnd = new Random();
            Vector3 pos = positions[rnd.Next(0, positions.Count)];
            player.TriggerEvent("huntNewAnimal", new Vector3(pos.X, pos.Y, pos.Z + 2.0f), animal, hunterPed.Position);
        }
    }
}

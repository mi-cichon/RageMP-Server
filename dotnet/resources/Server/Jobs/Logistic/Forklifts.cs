using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public static class Forklifts
    {

        public static List<KeyValuePair<Vector3, float>> boxPositions = new List<KeyValuePair<Vector3, float>>()
        {
            new KeyValuePair<Vector3, float>(new Vector3(-520.160767, -2378.36084, 13.4060326), 45f),
            new KeyValuePair<Vector3, float>(new Vector3(-522.6573, -2376.038, 13.4060326), 50f),
            new KeyValuePair<Vector3, float>(new Vector3(-525.5191, -2373.67578, 13.4060326), 50f),
            new KeyValuePair<Vector3, float>(new Vector3(-607.9698, -2462.04272, 13.3965521), 50f),
            new KeyValuePair<Vector3, float>(new Vector3(-605.352539, -2464.14453, 13.3965521), 50f)
        };

        public static List<Vector3> dropPositions = new List<Vector3>()
        {
            new Vector3(-556.98444f, -2392.6128f, 13.716819f)
        };

        public static List<string> BoxNames = new List<string>()
        {
            "prop_boxpile_06a"
        };

        public static List<GTANetworkAPI.Object> Boxes = new List<GTANetworkAPI.Object>();
        public static Vector3 StartPosition;
        public static ColShape StartColshape;
        public static Blip ForkBlip;
        public static void InstantiateForklifts(Vector3 startPosition)
        {     
            StartPosition = startPosition;
            StartColshape = NAPI.ColShape.CreateCylinderColShape(startPosition, 1.0f, 2.0f);
            StartColshape.SetSharedData("type", "forklifts");
            ForkBlip = NAPI.Blip.CreateBlip(569, startPosition, 0.8f, 69, name: "Wózki widłowe", shortRange: true);
            CustomMarkers.CreateJobMarker(startPosition, "Praca: Wózki widłowe");
            NAPI.Ped.CreatePed((uint)PedHash.Business02AFM, new Vector3(-554.0335f, -2354.093f, 13.994385f), -18f, frozen: true, invincible: true);

            CreateBoxSpawnPoints();
        }

        public static void StartJob(Player player)
        {

            if (!player.GetSharedData<bool>("nodriving"))
            {
                if (player.GetSharedData<bool>("jobBonus_7"))
                {
                    if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
                    {
                        player.SetSharedData("job", "forklifts");
                        player.TriggerEvent("startJob", "Wózki widłowe", "PL");
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Masz inną pracę!");
                    }
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie odblokowałeś tej pracy!");
                }
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie możesz prowadzić pojazdów do " + player.GetSharedData<string>("nodrivingto") + "!");
            }
        }
        private static void CreateBoxSpawnPoints()
        {
            foreach(KeyValuePair<Vector3, float> pair in boxPositions)
            {
                Random rnd = new Random();

                GTANetworkAPI.Object box = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(BoxNames[rnd.Next(0, BoxNames.Count)]), pair.Key, new Vector3(0, 0, pair.Value));
                box.SetSharedData("boxID", box.Id);
                box.SetSharedData("posID", boxPositions.IndexOf(pair));
                Boxes.Add(box);
            }
        }

        public static void CreateNewBox(int id)
        {
            NAPI.Task.Run(() =>
            {
                KeyValuePair<Vector3, float> pair = boxPositions[id];
                Random rnd = new Random();
                GTANetworkAPI.Object box = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(BoxNames[rnd.Next(0, BoxNames.Count)]), pair.Key, new Vector3(0, 0, pair.Value));
                box.SetSharedData("boxID", box.Id);
                box.SetSharedData("posID", boxPositions.IndexOf(pair));
                Boxes.Add(box);
            }, delayTime: 10000);
        }
    }
}

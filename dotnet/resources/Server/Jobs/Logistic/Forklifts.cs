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
            new KeyValuePair<Vector3, float>(new Vector3(-529.8499, -2372.777, 13.1299629), 50f),
            new KeyValuePair<Vector3, float>(new Vector3(-527.1217, -2375.06616, 13.1299629), 50f),
            new KeyValuePair<Vector3, float>(new Vector3(-524.5464, -2377.44531, 13.1299629), 50f),
            new KeyValuePair<Vector3, float>(new Vector3(-609.570068, -2463.342, 13.1175718), 50f),
            new KeyValuePair<Vector3, float>(new Vector3(-612.2968, -2461.13257, 13.1175718), 50f)
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
            new InteriorTeleport(new Vector3(-557.37805f, -2349.0842f, 13.944196f), 54.442875f, new Vector3(-554.24475f, -2351.8567f, 13.71682f), -131.29285f, "Magazyn");
            
            StartPosition = startPosition;
            StartColshape = NAPI.ColShape.CreateCylinderColShape(startPosition, 1.0f, 2.0f);
            StartColshape.SetSharedData("type", "forklifts");
            ForkBlip = NAPI.Blip.CreateBlip(569, startPosition, 0.8f, 69, name: "Wózki widłowe", shortRange: true);
            CustomMarkers.CreateJobMarker(startPosition, "Praca: Wózki widłowe");
            NAPI.Ped.CreatePed((uint)PedHash.Business02AFM, new Vector3(-551.69934f, -2360.5007f, 13.71682f), 50.8f, frozen: true, invincible: true);

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

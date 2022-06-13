using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using ServerSide;
namespace ServerSide
{
    public static class Lawnmowing
    {
        public static ColShape lawnColshape;
        public static Marker lawnMarker;
        public static Blip lawnBlip;
        public static Vector3 startPoint;
        public static void InstantiateLawnmowing(Vector3 startPoint)
        {
            startPoint = new Vector3(startPoint.X, startPoint.Y, startPoint.Z);
            lawnColshape = NAPI.ColShape.CreateCylinderColShape(startPoint - new Vector3(0, 0, 1), 1.0f, 2.0f);
            lawnColshape.SetSharedData("type", "lawnmowing");
            lawnBlip = NAPI.Blip.CreateBlip(497, startPoint, 0.8f, 69, name: "Praca: Koszenie trawników", shortRange: true);
            CustomMarkers.CreateJobMarker(startPoint, "Koszenie trawników");
            foreach (Vector3 grassPosition in GrassObjects.objects)
            {
                grassObjects.Add(new Grass(grassPosition, GetRandomGrassModel(), grassObjects.Count));
            }
        }

        public static void startJob(Player player)
        {

            if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                Random rnd = new Random();
                player.SetSharedData("job", "lawnmowing");
                player.TriggerEvent("startJob", "Koszenie trawników", "PN");
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie ma wolnych miejsc na parkingu lub masz inną pracę!");
            }
        }

        public static int GetRandomGrassModel()
        {
            Random rnd = new Random();
            return grassModels[rnd.Next(0, grassModels.Count)];
        }

        public static List<Grass> grassObjects = new List<Grass>();

        public static List<int> grassModels = new List<int>()
        {
            //(int)NAPI.Util.GetHashKey("prop_veg_grass_01_a"),
            //(int)NAPI.Util.GetHashKey("prop_veg_grass_01_b"),
            //(int)NAPI.Util.GetHashKey("prop_veg_grass_01_c"),
            (int)NAPI.Util.GetHashKey("prop_veg_grass_01_d")
        };
    }

    public class Grass
    {
        public GTANetworkAPI.Object grassObj { get; set; }
<<<<<<< Updated upstream
        public GTANetworkAPI.ColShape grassShape { get; set; }
=======
>>>>>>> Stashed changes
        public DateTime? pickedUpTime { get; set; }
        public Vector3 position { get; }
        public int model { get; }
        public int id { get; }


        public Grass(Vector3 position, int model, int ID)
        {
            this.position = position;
            this.model = model;
            this.id = ID;

            Random rnd = new Random();
            double moveX = rnd.Next(-5, 6);
            double moveY = rnd.Next(-5, 6);

            this.position = new Vector3(this.position.X + (moveX * 0.1), this.position.Y + (moveY * 0.1), this.position.Z - 0.4);

            grassObj = NAPI.Object.CreateObject(model, this.position, new Vector3());
<<<<<<< Updated upstream
            
            grassShape = NAPI.ColShape.CreateCylinderColShape(this.position - new Vector3(0, 1.4, 0), 1.5f, 3.0f);
            grassShape.SetSharedData("type", "grass");
            grassShape.SetSharedData("grassId", ID);
            grassShape.SetSharedData("grassExists", true);
=======
>>>>>>> Stashed changes

            pickedUpTime = null;
        }

        public bool Destroy()
        {
            if (grassObj.Exists)
            {
                grassObj.Delete();
                pickedUpTime = DateTime.Now;
                grassShape.SetSharedData("grassExists", false);
                return true;
            }
            return false;
        }

        public void Create()
        {
            if(!grassObj.Exists && !grassShape.Exists)
            {
                pickedUpTime = null;
                grassObj = NAPI.Object.CreateObject(model, this.position, new Vector3());
                grassShape.SetSharedData("grassExists", true);
            }
        }
    }
}
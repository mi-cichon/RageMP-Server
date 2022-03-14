using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using ServerSide;
namespace ServerSide.Jobs
{
    public class Lawnmowing
    {
        PlayerDataManager playerDataManager = new PlayerDataManager();
        CustomMarkers customMarkers = new CustomMarkers();
        public ColShape lawnColshape;
        public Marker lawnMarker;
        public Blip lawnBlip;
        public Vector3 startPoint;
        public Lawnmowing(Vector3 startPoint)
        {
            this.startPoint = new Vector3(startPoint.X, startPoint.Y, startPoint.Z);
            lawnColshape = NAPI.ColShape.CreateCylinderColShape(startPoint - new Vector3(0,0,1), 1.0f, 2.0f);
            lawnColshape.SetSharedData("type", "lawnmowing");
            lawnBlip = NAPI.Blip.CreateBlip(497, startPoint, 0.8f, 69, name: "Praca: Koszenie trawników (Natura)", shortRange: true);
            customMarkers.CreateJobMarker(startPoint, "Koszenie trawników");
        }

        public void startJob(Player player)
        {
           
            if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                Random rnd = new Random();
                player.SetSharedData("job", "lawnmowing");
                player.TriggerEvent("startJob", "Koszenie trawników", "PN");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie ma wolnych miejsc na parkingu lub masz inną pracę!");
            }
        }
    }
}

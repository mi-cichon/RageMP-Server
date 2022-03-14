using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using System.Linq;

namespace ServerSide.Jobs
{
    public class Junkyard
    {
        PlayerDataManager playerDataManager = new PlayerDataManager();

        public ColShape junkyardColshape;
        public Ped junkyardPed;
        public Marker junkyardMarker;
        public Blip junkyardBlip;
        public Vector3 pedPosition;
        CustomMarkers customMarkers = new CustomMarkers();
        public Junkyard(Vector3 colShapeposition, Vector3 pedPosition, float pedHeading)
        {
            this.pedPosition = pedPosition;
            junkyardPed = NAPI.Ped.CreatePed((uint)PedHash.Beach01AMO, pedPosition, pedHeading, invincible: true, frozen: true);
            junkyardColshape = NAPI.ColShape.CreateCylinderColShape(colShapeposition, 1.0f, 3.0f);
            junkyardColshape.SetSharedData("type", "junkyard");
            customMarkers.CreateJobMarker(colShapeposition, "Złomowisko");
            junkyardBlip = NAPI.Blip.CreateBlip(728, colShapeposition, 0.8f, 69, name: "Praca: Złomowisko (Społeczna)", shortRange: true);
        }

        public void startJob(Player player)
        {
            if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                player.SetSharedData("job", "junkyard");
                player.TriggerEvent("startJob", "Złomowisko", "PS");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Masz inną pracę!");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using System.Linq;

namespace ServerSide.Jobs
{
    public static class Junkyard
    {

        public static ColShape junkyardColshape;
        public static Ped junkyardPed;
        public static Marker junkyardMarker;
        public static Blip junkyardBlip;
        public static Vector3 pedPosition;
        public static void InstantiateJunkyard(Vector3 colShapeposition, Vector3 pedPosition, float pedHeading)
        {
            pedPosition = pedPosition;
            junkyardPed = NAPI.Ped.CreatePed((uint)PedHash.Beach01AMO, pedPosition, pedHeading, invincible: true, frozen: true);
            junkyardColshape = NAPI.ColShape.CreateCylinderColShape(colShapeposition, 1.0f, 3.0f);
            junkyardColshape.SetSharedData("type", "junkyard");
            CustomMarkers.CreateJobMarker(colShapeposition, "Złomowisko");
            junkyardBlip = NAPI.Blip.CreateBlip(728, colShapeposition, 0.8f, 69, name: "Praca: Złomowisko (Społeczna)", shortRange: true);
        }

        public static void startJob(Player player)
        {
            if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                player.SetSharedData("job", "junkyard");
                player.TriggerEvent("startJob", "Złomowisko", "PS");
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Masz inną pracę!");
            }
        }
    }
}

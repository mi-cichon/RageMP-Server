using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class Diver
    {
        PlayerDataManager playerDataManager;
        public Diver(ref PlayerDataManager playerDataManager)
        {
            this.playerDataManager = playerDataManager;
            Vector3 startPosition = new Vector3(1339.3713f, 4226.024f, 33.91554f);
            ColShape startCol = NAPI.ColShape.CreateCylinderColShape(startPosition, 1.0f, 2.0f);
            startCol.SetSharedData("type", "diver");
            NAPI.Blip.CreateBlip(471, startPosition, 0.8f, 69, name: "Praca: Nurek", shortRange: true);
            new CustomMarkers().CreateJobMarker(startPosition, "Nurek");
        }

        public void StartJob(Player player)
        {
            if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                if (player.GetSharedData<bool>("jobBonus_60"))
                {
                    player.SetSharedData("job", "diver");
                    player.TriggerEvent("startJob", "Nurek", "PW");
                    playerDataManager.SetJobClothes(player, true, "diver");
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie odblokowałeś tej pracy!");
                }
            }
        }
    }
}

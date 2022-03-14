using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class DebrisCleaner
    {
        ColShape containerColshape;
        public DebrisCleaner(Vector3 containerPosition)
        {
            containerColshape = NAPI.ColShape.CreateCylinderColShape(containerPosition, 2.0f, 2.0f);
            containerColshape.SetSharedData("type", "debriscleaner");
            NAPI.Blip.CreateBlip(440, containerPosition, 0.8f, 69, name: "Praca: Zbieranie odpadów (Woda)", shortRange: true);
            NAPI.Ped.CreatePed((uint)PedHash.Tramp01, containerPosition, -65, frozen: true, invincible: true);
            NAPI.TextLabel.CreateTextLabel("Zbieracz", new Vector3(containerPosition.X, containerPosition.Y, containerPosition.Z + 1.3), 10.0f, 0.2f, 0, new Color(255, 255, 255));
        }

        public void startJob(Player player)
        {
            if(player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                player.SetSharedData("job", "debriscleaner");
                player.TriggerEvent("startJob", "Zbieranie odpadów", "PW");
            }
        }
    }
}

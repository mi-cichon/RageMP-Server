using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class Peds
    {
        public Peds()
        {
        }
        public void CreateDepartmentPed(Vector3 position, float heading, string text)
        {
            Ped ped = NAPI.Ped.CreatePed((uint)PedHash.Business02AFM, position, heading, frozen: true, invincible: true);
            TextLabel txt = NAPI.TextLabel.CreateTextLabel(text, position + new Vector3(0, 0, 1), 10.0f, 1.0f, 4, new Color(255, 255, 255));

            ped = NAPI.Ped.CreatePed((uint)PedHash.Business03AMY, new Vector3(-1568.3905f, -549.71234f, 114.57581f), 124.973976f, frozen: true, invincible: true);
            txt = NAPI.TextLabel.CreateTextLabel("Organizacje", new Vector3(-1568.3905f, -549.71234f, 114.57581f) + new Vector3(0, 0, 1), 10.0f, 1.0f, 4, new Color(255, 255, 255));
        }
    }

    
}

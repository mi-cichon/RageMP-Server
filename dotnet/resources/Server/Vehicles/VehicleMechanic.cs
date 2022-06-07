using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide
{
    public class VehicleMechanic
    {
        Vector3 pedPosition, stationPosition;
        float pedHeading;
        bool makeBlip;
        public ColShape stationColShape, pedColShape;
        private Ped ped = null;
        public Blip blip;
        private string mechType;
        
        private TextLabel mechanicText;
        public VehicleMechanic(Vector3 pedPosition, float pedHeading, Vector3 stationPosition, bool makeBlip, string mechType)
        {
            this.pedPosition = new Vector3(pedPosition.X, pedPosition.Y, pedPosition.Z);
            this.pedHeading = pedHeading;
            this.stationPosition = new Vector3(stationPosition.X, stationPosition.Y, stationPosition.Z - 0.5f);
            this.makeBlip = makeBlip;
            this.mechType = mechType;
            Instantiate();
        }

        private void Instantiate()
        {
            stationColShape = NAPI.ColShape.CreateCylinderColShape(stationPosition, 3.0f, 2.0f);
            stationColShape.SetSharedData("type", "mechstation");
            pedColShape = NAPI.ColShape.CreateCylinderColShape(pedPosition, 1.0f, 2.0f);
            pedColShape.SetSharedData("type", "mech");
            mechanicText = NAPI.TextLabel.CreateTextLabel("Mechanik", pedPosition + new Vector3(0f, 0f, 1f), 10f, 2.0f, 4, new Color(0, 255, 0));
            if (mechType.Equals("poor"))
            {
                ped = NAPI.Ped.CreatePed((uint)PedHash.Acult02AMO, pedPosition, pedHeading, invincible: true, frozen: true);
                ped.SetSharedData("type", "mech");
            }
            else if (mechType.Equals("rich"))
            {
                ped = NAPI.Ped.CreatePed((uint)PedHash.Xmech01SMY, pedPosition, pedHeading, invincible: true, frozen: true);
                ped.SetSharedData("type", "mech");
            }
            if (makeBlip)
            {
                blip = NAPI.Blip.CreateBlip(402, stationPosition, 1.2f, 75, name: "Naprawa pojazdów", shortRange: true);
            }
        }
    }
}

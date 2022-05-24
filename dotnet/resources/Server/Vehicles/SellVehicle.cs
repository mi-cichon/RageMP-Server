using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide
{
    public class SellVehicle
    {
        public SellVehicle()
        {
            Vector3 position = new Vector3(-1118.99f, -2014.1088f, 13.188314f);
            ColShape colshape = NAPI.ColShape.CreateCylinderColShape(position, 2.0f, 2.0f);
            colshape.SetSharedData("type", "sellveh");
            NAPI.Marker.CreateMarker(27, position - new Vector3(0, 0, 0.9), new Vector3(), new Vector3(), 3.0f, new Color(173, 216, 230));
            NAPI.TextLabel.CreateTextLabel("Skup pojazdów", position, 10.0f, 2.0f, 4, new Color(255, 255, 255));
            NAPI.Blip.CreateBlip(664, position, 0.8f, 75, name: "Skup pojazdów", shortRange: true);
        }
    }
}

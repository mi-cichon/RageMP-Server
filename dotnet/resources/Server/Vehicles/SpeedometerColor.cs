using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using System.Xml;
using System.IO;
using System.Linq;

namespace ServerSide
{
    public class SpeedometerColor
    {
        public ColShape colShape;
        public SpeedometerColor(Vector3 position)
        {
            colShape = NAPI.ColShape.CreateCylinderColShape(position, 4.0f, 5.0f);
            colShape.SetSharedData("type", "speedoColor");
            NAPI.Blip.CreateBlip(483, position, 0.7f, 75, name: "Kolor licznika", shortRange: true);
            NAPI.Marker.CreateMarker(27, position - new Vector3(0, 0, 0.9), new Vector3(), new Vector3(), 3.0f, new Color(173, 216, 230));
            NAPI.TextLabel.CreateTextLabel("Kolor licznika", position, 10.0f, 2.0f, 4, new Color(255, 255, 255));
        }
    }
}

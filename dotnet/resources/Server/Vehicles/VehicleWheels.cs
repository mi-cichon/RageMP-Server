using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using System.Xml;
using System.IO;
using System.Linq;

namespace ServerSide
{
    public class VehicleWheels
    {
        ColShape colShape;
        public VehicleWheels(Vector3 position)
        {
            colShape = NAPI.ColShape.CreateCylinderColShape(position - new Vector3(0, 0, 1), 2.0f, 2.0f);
            colShape.SetSharedData("type", "wheelstune");
            NAPI.Marker.CreateMarker(27, position - new Vector3(0, 0, 0.8), new Vector3(), new Vector3(), 2.0f, new Color(255, 0, 0));
            NAPI.Blip.CreateBlip(488, position, 0.7f, 75, name: "Zmiana felg", shortRange: true);
        }
    }
}

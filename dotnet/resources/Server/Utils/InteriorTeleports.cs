using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide
{
    public class InteriorTeleport
    {
        public ColShape insideColShape;
        public ColShape outsideColShape;
        public TextLabel outsideText;
        public TextLabel insideText;
        
        public Blip blip;
        public InteriorTeleport(Vector3 outsidePosition, float outsideHeading, Vector3 insidePosition, float insideHeading, string name)
        {
            insideColShape = NAPI.ColShape.CreateCylinderColShape(insidePosition, 1.0f, 3.0f);
            insideColShape.SetSharedData("type", "teleport");
            insideColShape.SetSharedData("heading", outsideHeading);
            insideColShape.SetSharedData("name", "Wyjście");
            insideColShape.SetSharedData("position", outsidePosition);

            outsideColShape = NAPI.ColShape.CreateCylinderColShape(outsidePosition, 1.0f, 3.0f);
            outsideColShape.SetSharedData("type", "teleport");
            outsideColShape.SetSharedData("heading", insideHeading);
            outsideColShape.SetSharedData("name", name);
            outsideColShape.SetSharedData("position", insidePosition);

            CustomMarkers.CreateSimpleMarker(insidePosition, "Wyjście");
            CustomMarkers.CreateSimpleMarker(outsidePosition, name);

            switch (name)
            {
                case "Urząd miasta":
                    blip = NAPI.Blip.CreateBlip(475, outsidePosition, 0.8f, 30, name: name, shortRange: true);
                    break;
            }
            
        }

    }

    public class LSPDTeleport
    {
        public ColShape insideColShape;
        public ColShape outsideColShape;
        public TextLabel outsideText;
        public TextLabel insideText;

        public Blip blip;
        public LSPDTeleport(Vector3 outsidePosition, float outsideHeading, Vector3 insidePosition, float insideHeading, string name)
        {
            insideColShape = NAPI.ColShape.CreateCylinderColShape(insidePosition, 1.0f, 3.0f);
            insideColShape.SetSharedData("type", "LSPD-teleport");
            insideColShape.SetSharedData("heading", outsideHeading);
            insideColShape.SetSharedData("name", "Wyjście");
            insideColShape.SetSharedData("position", outsidePosition);

            outsideColShape = NAPI.ColShape.CreateCylinderColShape(outsidePosition, 1.0f, 3.0f);
            outsideColShape.SetSharedData("type", "LSPD-teleport");
            outsideColShape.SetSharedData("heading", insideHeading);
            outsideColShape.SetSharedData("name", name);
            outsideColShape.SetSharedData("position", insidePosition);

            CustomMarkers.CreateLSPDMarker(insidePosition, "Wyjście");
            CustomMarkers.CreateLSPDMarker(outsidePosition, name);

            switch (name)
            {
                case "Urząd miasta":
                    blip = NAPI.Blip.CreateBlip(475, outsidePosition, 1.0f, 60, name: name, shortRange: true);
                    break;
            }

        }
    }
}

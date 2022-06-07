using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class ChangingRoom
    {
        Vector3 colShapePosition;
        float playerHeading;
        ColShape changingRoomColshape;
        Blip changingRoomBlip;

        public ChangingRoom(Vector3 colShapePos, float playerH)
        {
            colShapePosition = colShapePos;
            playerHeading = playerH;
            changingRoomColshape = NAPI.ColShape.CreateCylinderColShape(colShapePosition - new Vector3(0,0,1), 2.0f, 2.0f);
            changingRoomColshape.SetSharedData("type", "changingroom");
            changingRoomColshape.SetSharedData("playerhead", playerHeading);
            CustomMarkers.CreateSimpleMarker(colShapePosition, "Sklep z ubraniami");
            changingRoomBlip = NAPI.Blip.CreateBlip(73, colShapePosition, 0.8f, 30, name: "Sklep z ubraniami", shortRange: true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class PaintShop
    {
        public ColShape paintShopColshape;
        public Blip paintShopBlip;
        public Marker paintShopMarker;
        public PaintShop(Vector3 colShapePosition)
        {
            paintShopColshape = NAPI.ColShape.CreateCylinderColShape(colShapePosition, 4.0f, 5.0f);
            paintShopColshape.SetSharedData("type", "paintshop");
            paintShopBlip = NAPI.Blip.CreateBlip(72, colShapePosition, 0.7f, 75, name: "Lakiernia", shortRange: true);
            NAPI.Marker.CreateMarker(27, colShapePosition - new Vector3(0, 0, 0.9), new Vector3(), new Vector3(), 3.0f, new Color(173, 216, 230));
            NAPI.TextLabel.CreateTextLabel("Lakiernia", colShapePosition, 10.0f, 2.0f, 4, new Color(255, 255, 255));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide
{
    public class CarTrader
    {
        public CarTrader(Vector3 pedPosition, float pedHeading, Vector3 position)
        {
            ColShape cs = NAPI.ColShape.CreateCylinderColShape(position - new Vector3(0, 0, 1), 2.0f, 2.0f);
            Ped ped = NAPI.Ped.CreatePed((uint)PedHash.Business03AFY, pedPosition, pedHeading, invincible: true, frozen: true);
            TextLabel tl = NAPI.TextLabel.CreateTextLabel("Sprzedaż pojazdów", pedPosition + new Vector3(0, 0, 1), 10.0f, 1.0f, 4, new Color(255, 255, 255));
            cs.SetSharedData("type", "cartrader");
        }
    }
}

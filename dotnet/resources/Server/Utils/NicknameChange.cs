using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class NicknameChange
    {
        public NicknameChange()
        {
            ColShape col = NAPI.ColShape.CreateCylinderColShape(new Vector3(-1564.3102f, -561.14087f, 114.44851f), 1.2f, 2.0f);
            col.SetSharedData("type", "nickname");
        }
    }
}

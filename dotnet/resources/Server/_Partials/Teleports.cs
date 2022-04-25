using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        private void CreateTeleports()
        {
            InteriorTeleport departmentLS = new InteriorTeleport(new Vector3(-1581.3214f, -558.3011f, 34.9531f), 36f, new Vector3(-1560.9137f, -568.60657f, 114.57642), 40f, "Urząd miasta");
            InteriorTeleport salonPremium = new InteriorTeleport(new Vector3(-803.1636f, -223.88596f, 37.225594f), 120f, new Vector3(-787.7405f, -219.56136f, 58.519867f), 30f, "Salon premium");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public static class ItemShops
    {
        static List<PedHash> peds = new List<PedHash>()
        {
            PedHash.AgentCutscene,
            PedHash.ArmLieut01GMM,
            PedHash.Baygor,
            PedHash.ChiGoon02GMM,
            PedHash.Chimp,
            PedHash.DebraCutscene,
            PedHash.DeniseFriendCutscene,
            PedHash.Downtown01AFM,
            PedHash.Eastsa01AFM,
            PedHash.Eastsa02AFY,
            PedHash.Eastsa03AFY,
            PedHash.FatBla01AFM,
            PedHash.FemBarberSFM,
            PedHash.ForgeryFemale01,
            PedHash.PatriciaCutscene
        };

        public static void InitializeShops()
        {
            CreateShop(new ShopInfo(new Vector3(-46.86535f, -1759.0315f, 29.421005f), 46.031837f, new Vector3(-48.67781f, -1757.5618f, 29.421005f)));
            CreateShop(new ShopInfo(new Vector3(24.04396f, -1345.591f, 29.497028f), -93.76412f, new Vector3(26.101736f, -1345.6154f, 29.497025f)));
            CreateShop(new ShopInfo(new Vector3(1133.6372f, -983.1239f, 46.415813f), -87.80536f, new Vector3(1136.1764f, -982.8724f, 46.415813f)));
            CreateShop(new ShopInfo(new Vector3(1165.5026f, -323.52377f, 69.205055f), 93.01437f, new Vector3(1163.3219f, -324.06702f, 69.205055f)));
            CreateShop(new ShopInfo(new Vector3(372.61478f, 328.09473f, 103.566376f), -105.03993f, new Vector3(374.58936f, 327.70114f, 103.566376f)));
            CreateShop(new ShopInfo(new Vector3(2555.423f, 380.49866f, 108.62295f), -8.141312f, new Vector3(2555.5193f, 382.45355f, 108.62295f)));
            CreateShop(new ShopInfo(new Vector3(2676.33f, 3279.9678f, 55.241127f), -37.00693f, new Vector3(2677.2432f, 3281.5815f, 55.241127f)));
            CreateShop(new ShopInfo(new Vector3(1165.2759f, 2711.2712f, 38.157696f), 175.17651f, new Vector3(1165.2806f, 2708.8757f, 38.157696f)));
            CreateShop(new ShopInfo(new Vector3(549.60895f, 2669.7366f, 42.156494f), 96.79589f, new Vector3(547.71716f, 2669.41f, 42.156494f)));
            CreateShop(new ShopInfo(new Vector3(1392.8195f, 3606.6584f, 34.98093f), -168.23717f, new Vector3(1393.6595f, 3605.0076f, 34.98093f)));
            CreateShop(new ShopInfo(new Vector3(1958.8549f, 3741.361f, 32.343746f), -69.59676f, new Vector3(1960.6143f, 3742.2976f, 32.343746f)));
            CreateShop(new ShopInfo(new Vector3(1696.9124f, 4922.964f, 42.063667f), -44.833523f, new Vector3(1698.2455f, 4924.889f, 42.063667f)));
            CreateShop(new ShopInfo(new Vector3(1728.2334f, 6416.884f, 35.03723f), -118.67633f, new Vector3(1730.0128f, 6416.105f, 35.03722f)));
            CreateShop(new ShopInfo(new Vector3(-3243.9526f, 999.78925f, 12.830707f), -7.1730094f, new Vector3(-3243.9163f, 1001.61096f, 12.830705f)));
            CreateShop(new ShopInfo(new Vector3(-3040.403f, 583.6446f, 7.9089293f), 10.926718f, new Vector3(-3041.1243f, 585.3698f, 7.9089293f)));
            CreateShop(new ShopInfo(new Vector3(-2965.936f, 391.53418f, 15.043313f), 81.06821f, new Vector3(-2968.188f, 391.536f, 15.043313f)));
            CreateShop(new ShopInfo(new Vector3(-1819.0848f, 793.91614f, 138.07326f), 125.62713f, new Vector3(-1820.6827f, 792.3624f, 138.11862f)));
            CreateShop(new ShopInfo(new Vector3(-1486.33f, -377.17508f, 40.16339f), 128.61798f, new Vector3(-1487.8688f, -378.86945f, 40.16339f)));
            CreateShop(new ShopInfo(new Vector3(-1221.1693f, -908.4476f, 12.326356f), 31.312563f, new Vector3(-1222.4337f, -906.5728f, 12.326356f)));
            CreateShop(new ShopInfo(new Vector3(-705.4629f, -914.52295f, 19.215588f), 83.94446f, new Vector3(-707.6986f, -914.6637f, 19.215588f)));
        }

        public static void CreateShop(ShopInfo shopInfo)
        {
            Random rand = new Random();
            int ped = rand.Next(0, peds.Count);
            NAPI.Ped.CreatePed((uint)peds[ped], shopInfo.PedPos, shopInfo.PedHead, frozen: true, invincible: true);
            ColShape shape = NAPI.ColShape.CreateCylinderColShape(shopInfo.ColPos - new Vector3(0, 0, 1), 2.0f, 2.0f);
            shape.SetSharedData("type", "itemShop");
            NAPI.Blip.CreateBlip(52, shopInfo.PedPos, 0.6f, 30, name: "Sklep z przedmiotami", shortRange: true);
        }
    }

    public struct ShopInfo
    {
        public ShopInfo(Vector3 PedPos, float PedHead, Vector3 ColPos)
        {
            this.PedPos = PedPos;
            this.PedHead = PedHead;
            this.ColPos = ColPos;
        }
        public Vector3 PedPos { get; set; }
        public Vector3 ColPos { get; set; }
        public float PedHead { get; set; }
        
    }
}

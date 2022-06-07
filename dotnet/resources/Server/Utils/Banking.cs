using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public static class Banking
    {
        static List<Vector3> cashMachines = new List<Vector3>()
        {
            new Vector3(155.8989f, 6642.891f, 31.600456f),
            new Vector3(-95.55596f, 6457.184f, 31.46097f),
            new Vector3(-97.3411f, 6455.4355f, 31.467825f),
            new Vector3(-386.79504f, 6046.1006f, 31.501719f),
            new Vector3(-3241.2327f, 997.5441f, 12.550399f),
            new Vector3(-3043.9192f, 594.5837f, 7.7364764f),
            new Vector3(-2958.953f, 487.72107f, 15.463918f),
            new Vector3(-2956.804f, 487.63068f, 15.46392f),
            new Vector3(-2975.0583f, 380.175f, 14.999065f),
            new Vector3(-2072.3489f, -317.24933f, 13.31597f),
            new Vector3(-2295.4983f, 358.16235f, 174.60176f),
            new Vector3(-2294.7158f, 356.48965f, 174.60176f),
            new Vector3(-2293.9395f, 354.8125f, 174.60176f),
            new Vector3(-1827.2495f, 784.8734f, 138.3042f),
            new Vector3(-1315.778f, -834.78925f, 16.961721f),
            new Vector3(-1314.8075f, -835.9298f, 16.960205f),
            new Vector3(-1571.0524f, -547.36237f, 34.957466f),
            new Vector3(-1570.0756f, -546.6853f, 34.955902f),
            new Vector3(-1305.4044f, -706.39526f, 25.322428f),
            new Vector3(-1409.786f, -100.48356f, 52.384663f),
            new Vector3(-1410.3203f, -98.798225f, 52.428616f),
            new Vector3(-1205.765f, -324.83054f, 37.85896f),
            new Vector3(-1204.9984f, -326.3088f, 37.83871f),
            new Vector3(-866.66486f, -187.74205f, 37.84275f),
            new Vector3(-867.6196f, -186.12524f, 37.84232f),
            new Vector3(-721.0681f, -415.54468f, 34.981663f),
            new Vector3(-537.8214f, -854.4782f, 29.290934f),
            new Vector3(-660.64886f, -854.04724f, 24.485912f),
            new Vector3(-717.7031f, -915.6709f, 19.215588f),
            new Vector3(-821.6944f, -1081.9205f, 11.132433f),
            new Vector3(-1109.7357f, -1690.7672f, 4.3750095f),
            new Vector3(-165.06773f, 232.69064f, 94.92194f),
            new Vector3(-165.15602f, 234.77928f, 94.92193f),
            new Vector3(285.47873f, 143.38086f, 104.17248f),
            new Vector3(380.7503f, 323.38522f, 103.56639f),
            new Vector3(228.20967f, 338.40512f, 105.56525f),
            new Vector3(527.26697f, -160.726f, 57.08847f),
            new Vector3(89.63978f, 2.4434967f, 68.30544f),
            new Vector3(-57.7012f, -92.668564f, 57.780693f),
            new Vector3(1153.6488f, -326.6988f, 69.20514f),
            new Vector3(1166.9337f, -456.05185f, 66.8051f),
            new Vector3(1138.2137f, -468.96243f, 66.729904f),
            new Vector3(1077.7943f, -776.52936f, 58.242184f),
            new Vector3(-56.96678f, -1752.0995f, 29.421015f),
            new Vector3(288.81403f, -1282.2946f, 29.631695f),
            new Vector3(289.0749f, -1256.729f, 29.440718f),
            new Vector3(33.16713f, -1348.2609f, 29.497023f),
            new Vector3(130.09265f, -1292.7349f, 29.26953f),
            new Vector3(129.67316f, -1291.899f, 29.269531f),
            new Vector3(129.2238f, -1291.1097f, 29.269531f),
            new Vector3(-526.6308f, -1222.9875f, 18.455f),
            new Vector3(147.61287f, -1035.7853f, 29.34306f),
            new Vector3(145.90468f, -1035.1775f, 29.344927f),
            new Vector3(296.48608f, -894.174f, 29.23143f),
            new Vector3(295.76215f, -896.10394f, 29.21632f),
            new Vector3(119.029106f, -883.7283f, 31.123077f),
            new Vector3(112.656425f, -819.40295f, 31.337639f),
            new Vector3(114.44452f, -776.3772f, 31.417706f),
            new Vector3(111.24295f, -775.2693f, 31.438417f),
            new Vector3(-28.052662f, -724.5022f, 44.228615f),
            new Vector3(-30.244795f, -723.67664f, 44.228462f),
            new Vector3(-254.39772f, -692.43243f, 33.608604f),
            new Vector3(-256.23206f, -716.0145f, 33.524128f),
            new Vector3(-258.8381f, -723.3675f, 33.47095f),
            new Vector3(-301.67114f, -830.0155f, 32.417274f),
            new Vector3(-303.3183f, -829.7565f, 32.417274f),
            new Vector3(-203.79988f, -861.3666f, 30.267626f),
            new Vector3(5.270799f, -919.84296f, 29.559038f),
            new Vector3(24.47649f, -945.9506f, 29.357582f),
            new Vector3(2558.7458f, 350.939f, 108.621544f),
            new Vector3(2558.4995f, 389.4747f, 108.62293f),
            new Vector3(2564.501f, 2584.777f, 38.083107f),
            new Vector3(1172.5156f, 2702.5938f, 38.17475f),
            new Vector3(1171.6056f, 2702.5918f, 38.175423f),
            new Vector3(540.34283f, 2671.1216f, 42.15653f),
            new Vector3(-1091.4603f, 2708.6492f, 18.953505f),
            new Vector3(2683.0806f, 3286.558f, 55.24113f),
            new Vector3(1968.0938f, 3743.55f, 32.343742f),
            new Vector3(1822.6886f, 3683.058f, 34.276745f),
            new Vector3(1686.8358f, 4815.791f, 42.008617f),
            new Vector3(1703.0338f, 4933.5835f, 42.06368f),
            new Vector3(1735.2804f, 6410.491f, 35.037216f),
            new Vector3(1701.1918f, 6426.5293f, 32.764034f),
            new Vector3(174.08301f, 6637.8823f, 31.573069f),
            new Vector3(-981.9395f, -2642.1519f, 13.991373f),
            new Vector3(1039.7086f, -2111.9746f, 32.677387f),
            new Vector3(824.3546f, -1042.93f, 26.890291f),
            new Vector3(819.0355f, -2966.8237f, 6.02066f)

    };

        public static void InstantiateATMs()
        {
            foreach(Vector3 pos in cashMachines)
            {
                ColShape col = NAPI.ColShape.CreateCylinderColShape(pos, 0.3f, 2.0f);
                col.SetSharedData("type", "atm");
                NAPI.Blip.CreateBlip(108, pos, 0.4f, 43, name: "Bankomat", shortRange: true);
            }
        }
    }
}

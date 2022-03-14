using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;
using Object = GTANetworkAPI.Object;

namespace ServerSide
{
    public class Gardener
    {
        public List<GardenerOrder> Orders = new List<GardenerOrder>();
        public List<Ground> Grounds = new List<Ground>();
        PlayerDataManager playerDataManager;
        VehicleDataManager vehicleDataManager;
        CustomMarkers customMarkers = new CustomMarkers();
        int lastOrderId = 0;
        public Gardener(ref PlayerDataManager playerDataManager, ref VehicleDataManager vehicleDataManager)
        {
            this.playerDataManager = playerDataManager;
            this.vehicleDataManager = vehicleDataManager;
            NAPI.Ped.CreatePed((uint)PedHash.Farmer01AMM, new Vector3(-1275.7875f, -1139.7943f, 6.7924547f), 117.193184f, frozen: true, invincible: true);
            ColShape sellout = NAPI.ColShape.CreateCylinderColShape(new Vector3(-1275.7875f, -1139.7943f, 6.7924547f), 2.0f, 2.0f);
            sellout.SetSharedData("type", "gardener_sellout");
            ColShape shape = NAPI.ColShape.CreateCylinderColShape(new Vector3(1546.5374f, 2166.511f, 78.72393f), 1.0f, 2.0f);
            customMarkers.CreateJobMarker(new Vector3(1546.5374f, 2166.511f, 78.72393f), "Ogrodnik");
            NAPI.Blip.CreateBlip(761, new Vector3(1546.5374f, 2166.511f, 78.72393f), 0.8f, 69, name: "Praca: Ogrodnik (Natura)", shortRange: true);
            shape.SetSharedData("type", "gardener");

            for(int i = 0; i < 15; i++)
            {
                Orders.Add(new GardenerOrder(lastOrderId));
                lastOrderId++;
            }

            CreateGrounds();
            
        }


        public string GetOrders()
        {
            List<List<int>> orders = new List<List<int>>();
            foreach(GardenerOrder order in Orders)
            {
                orders.Add(new List<int>()
                {
                    order.Id, order.Plants[0], order.Plants[1], order.Plants[2], order.Plants[3], order.Plants[4]
                });
            }

            return JsonConvert.SerializeObject(orders);
        }

        public void NewOrder(int index)
        {
            Orders[index] = new GardenerOrder(lastOrderId);
            lastOrderId++;
        }

        public void StartJob(Player player)
        {
            if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                if (player.GetSharedData<Int32>("naturepoints") >= 150)
                {
                    if (!player.GetSharedData<bool>("nodriving"))
                    {
                        if (player.GetSharedData<bool>("licenceBp"))
                        {
                            player.SetSharedData("job", "gardener");
                            player.TriggerEvent("startJob", "Ogrodnik", "PN");
                            playerDataManager.NotifyPlayer(player, "Praca rozpoczęta!");
                            player.TriggerEvent("openGardenerOrdersBrowser", GetOrders());
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie masz prawa jazdy");
                        }
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie możesz prowadzić pojazdów do " + player.GetSharedData<string>("nodrivingto") + "!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie posiadasz wystarczająco PN: 150!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Masz inną pracę!");
            }
        }
        
        private void CreateGrounds()
        {
            Grounds.Add(new Ground("prop_plant_01b", Grounds.Count, 900, new List<Vector3>()
            {
                new Vector3(2619.1118f, 4831.3403f, 33.58665f),
                new Vector3(2622.851f, 4835.3315f, 33.244186f),
                new Vector3(2615.8652f, 4834.773f, 33.968f),
                new Vector3(2619.8716f, 4838.919f, 33.762226f),
                new Vector3(2612.6987f, 4838.3135f, 34.255714f),
                new Vector3(2616.7256f, 4842.407f, 34.178802f),
                new Vector3(2609.4216f, 4841.7295f, 34.530003f),
                new Vector3(2606.0881f, 4845.1934f, 34.80226f),
                new Vector3(2613.2773f, 4845.731f, 34.455578f),
                new Vector3(2602.8315f, 4848.5825f, 35.05518f),
                new Vector3(2608.4883f, 4849.614f, 34.788086f),
                new Vector3(2598.939f, 4852.6216f, 35.26634f),
                new Vector3(2605.815f, 4852.402f, 34.99214f),
                new Vector3(2595.6833f, 4855.9995f, 35.359283f),
                new Vector3(2602.652f, 4855.755f, 35.1805f),
                new Vector3(2591.867f, 4859.944f, 35.35988f),
                new Vector3(2599.522f, 4859.003f, 35.37049f),
                new Vector3(2587.972f, 4863.983f, 35.27462f),
                new Vector3(2596.1938f, 4862.3975f, 35.445538f),
                new Vector3(2584.0872f, 4868.007f, 35.467155f),
                new Vector3(2592.9807f, 4865.6724f, 35.411446f),
                new Vector3(2590.3428f, 4868.356f, 35.39549f),
                new Vector3(2580.5562f, 4871.652f, 35.673206f),
                new Vector3(2586.656f, 4871.936f, 35.50698f),
                new Vector3(2582.475f, 4875.7725f, 35.68925f),
                new Vector3(2630.65f, 4842.645f, 33.502f),
                new Vector3(2627.472f, 4846.0264f, 33.70449f),
                new Vector3(2623.675f, 4850.035f, 34.02029f),
                new Vector3(2620.361f, 4853.5244f, 34.256725f),
                new Vector3(2616.5168f, 4857.5815f, 34.51971f),
                new Vector3(2613.2021f, 4861.0776f, 34.74365f),
                new Vector3(2609.8706f, 4864.581f, 34.965263f),
                new Vector3(2606.028f, 4868.633f, 35.206284f),
                new Vector3(2602.172f, 4872.689f, 35.370502f),
                new Vector3(2598.9453f, 4876.275f, 35.503864f),
                new Vector3(2595.2559f, 4880.38f, 35.633324f),
                new Vector3(2592.0369f, 4883.9536f, 35.754604f),
                new Vector3(2588.4895f, 4887.8896f, 36.06175f),
                new Vector3(2584.7285f, 4892.0522f, 36.40194f),
                new Vector3(2580.989f, 4896.209f, 36.847816f),
                new Vector3(2577.7446f, 4899.8022f, 37.21574f),
                new Vector3(2574.6025f, 4903.1455f, 37.50663f),
                new Vector3(2576.6167f, 4909.433f, 37.627346f),
                new Vector3(2579.1829f, 4906.705f, 37.4312f),
                new Vector3(2582.4998f, 4903.1504f, 37.180813f),
                new Vector3(2585.8264f, 4899.749f, 36.914024f),
                new Vector3(2588.493f, 4896.9585f, 36.695232f),
                new Vector3(2625.741f, 4840.5103f, 33.371468f),
                new Vector3(2591.716f, 4893.584f, 36.418583f),
                new Vector3(2595.5347f, 4889.5957f, 36.090702f),
                new Vector3(2621.6223f, 4845.5576f, 34.021397f),
                new Vector3(2598.6675f, 4886.3115f, 35.859703f),
                new Vector3(2601.9753f, 4882.8604f, 35.642498f),
                new Vector3(2605.0989f, 4879.592f, 35.41837f),
                new Vector3(2608.8767f, 4875.6533f, 35.141567f),
                new Vector3(2612.0327f, 4872.3633f, 34.929203f),
                new Vector3(2615.8076f, 4868.4277f, 34.684437f),
                new Vector3(2619.208f, 4864.892f, 34.46103f),
                new Vector3(2622.4595f, 4861.497f, 34.25382f),
                new Vector3(2626.2393f, 4857.5684f, 34.03571f),
                new Vector3(2629.5117f, 4854.163f, 33.869793f),
                new Vector3(2633.3586f, 4850.1655f, 33.713337f),
                new Vector3(2636.499f, 4846.9023f, 33.593155f),
                new Vector3(2640.1511f, 4851.195f, 33.605846f),
                new Vector3(2636.8408f, 4854.7124f, 33.736046f),
                new Vector3(2633.2625f, 4858.4946f, 33.850964f),
                new Vector3(2630.0037f, 4861.959f, 33.94191f),
                new Vector3(2626.6555f, 4865.506f, 34.089245f),
                new Vector3(2623.3206f, 4869.0303f, 34.29691f),
                new Vector3(2620.1353f, 4872.402f, 34.522346f),
                new Vector3(2616.8057f, 4875.921f, 34.775837f),
                new Vector3(2613.5488f, 4879.461f, 35.03963f),
                new Vector3(2610.3687f, 4882.8594f, 35.33696f),
                new Vector3(2607.2153f, 4886.2324f, 35.661507f),
                new Vector3(2603.8972f, 4889.771f, 35.98399f),
                new Vector3(2600.946f, 4892.93f, 36.273315f),
                new Vector3(2597.618f, 4896.4766f, 36.58317f),
                new Vector3(2594.3806f, 4899.937f, 36.88451f),
                new Vector3(2591.0627f, 4903.4844f, 37.05144f),
                new Vector3(2587.7605f, 4907f, 37.20978f),
                new Vector3(2584.5964f, 4910.377f, 37.380806f),
                new Vector3(2581.3538f, 4913.829f, 37.574684f),
                new Vector3(2585.7974f, 4917.59f, 37.599094f),
                new Vector3(2589.002f, 4914.25f, 37.42641f),
                new Vector3(2592.31f, 4910.804f, 37.29955f),
                new Vector3(2595.4712f, 4907.5093f, 37.274754f),
                new Vector3(2599.0989f, 4903.724f, 37.11803f),
                new Vector3(2602.508f, 4900.178f, 36.794693f),
                new Vector3(2606.3567f, 4896.1665f, 36.422127f),
                new Vector3(2610.1775f, 4892.2007f, 36.030823f),
                new Vector3(2614.0315f, 4888.193f, 35.62861f),
                new Vector3(2617.8047f, 4884.274f, 35.218887f),
                new Vector3(2621.183f, 4880.7744f, 34.845257f),
                new Vector3(2624.4421f, 4877.3916f, 34.548115f),
                new Vector3(2627.6802f, 4874.033f, 34.29119f),
                new Vector3(2631.0195f, 4870.586f, 34.067867f),
                new Vector3(2634.2395f, 4867.2485f, 33.921734f),
                new Vector3(2628.7178f, 4837.143f, 33.072784f),
                new Vector3(2625.4482f, 4840.557f, 33.379444f),
                new Vector3(2621.6086f, 4844.582f, 33.99914f),
                new Vector3(2618.3718f, 4847.973f, 34.250103f),
                new Vector3(2615.0154f, 4851.479f, 34.49551f),
                new Vector3(2611.673f, 4854.8853f, 34.74691f),
                new Vector3(2607.87f, 4858.8125f, 35.008236f),
                new Vector3(2604.0562f, 4862.8203f, 35.26423f),
                new Vector3(2600.7146f, 4866.3413f, 35.42535f),
                new Vector3(2597.1838f, 4870.0747f, 35.456432f),
                new Vector3(2593.9775f, 4873.447f, 35.511436f),
                new Vector3(2590.8687f, 4876.7266f, 35.553528f),
                new Vector3(2587.1448f, 4880.6533f, 35.682022f),
                new Vector3(2583.853f, 4884.1265f, 36.02171f),
                new Vector3(2580.0137f, 4888.1743f, 36.422195f),
                new Vector3(2576.6577f, 4891.6997f, 36.798634f),
                new Vector3(2573.4355f, 4895.0923f, 37.171093f),
                new Vector3(2570.138f, 4898.557f, 37.53782f)
            }, ref playerDataManager));


            List<Vector3> high1 = new List<Vector3>()
            {
                new Vector3(1474.2958f, -2628.399f, 47.361423f),
                new Vector3(1477.4044f, -2638.211f, 44.932316f),
                new Vector3(1481.5675f, -2643.29f, 43.346733f),
                new Vector3(1483.0873f, -2649.4785f, 41.84267f),
                new Vector3(1484.8389f, -2654.977f, 40.49244f),
                new Vector3(1486.5925f, -2660.3345f, 39.18239f),
                new Vector3(1488.629f, -2666.5034f, 37.701183f),
                new Vector3(1486.5271f, -2671.347f, 36.968483f),
                new Vector3(1485.1335f, -2676.4927f, 36.37289f),
                new Vector3(1483.8846f, -2681.5635f, 35.779167f),
                new Vector3(1480.1045f, -2682.8591f, 36.624252f),
                new Vector3(1478.3737f, -2679.2644f, 37.61466f),
                new Vector3(1476.1075f, -2671.624f, 38.89642f),
                new Vector3(1474.0934f, -2664.2961f, 39.8724f),
                new Vector3(1468.1716f, -2658.651f, 41.273273f),
                new Vector3(1467.3544f, -2651.1802f, 42.941803f),
                new Vector3(1471.9177f, -2644.8665f, 43.868057f),
                new Vector3(1469.3239f, -2637.2656f, 45.91059f),
                new Vector3(1463.2874f, -2631.4185f, 47.92008f),
                new Vector3(1458.7622f, -2635.019f, 47.102768f),
                new Vector3(1458.2316f, -2640.8545f, 45.91506f),
                new Vector3(1458.4188f, -2646.9392f, 44.39479f),
                new Vector3(1458.6958f, -2653.3643f, 42.706207f),
                new Vector3(1458.6714f, -2659.261f, 41.520733f),
                new Vector3(1458.6565f, -2664.622f, 40.990643f),
                new Vector3(1458.6538f, -2669.9036f, 40.524464f),
                new Vector3(1458.6982f, -2675.4026f, 40.097073f),
                new Vector3(1459.4404f, -2684.4067f, 37.3428f),
                new Vector3(1462.5283f, -2694.785f, 35.96574f),
                new Vector3(1454.1895f, -2685.6958f, 35.469124f),
                new Vector3(1451.6439f, -2676.8374f, 38.912064f),
                new Vector3(1451.0114f, -2671.5188f, 40.08258f),
                new Vector3(1452.5791f, -2663.745f, 41.253925f),
                new Vector3(1452.3822f, -2659.6475f, 41.682392f),
                new Vector3(1452.1127f, -2651.98f, 43.18882f),
                new Vector3(1451.8961f, -2644.7117f, 45.26031f),
                new Vector3(1451.7039f, -2637.5962f, 46.765903f),
                new Vector3(1444.1166f, -2633.8887f, 47.387875f),
                new Vector3(1440.7714f, -2638.5452f, 46.609375f),
                new Vector3(1440.6898f, -2645.8044f, 44.818558f),
                new Vector3(1440.47f, -2651.3892f, 43.138332f),
                new Vector3(1440.0354f, -2657.8257f, 41.93188f),
                new Vector3(1440.1885f, -2663.6729f, 41.357063f),
                new Vector3(1441.9537f, -2672.005f, 40.111504f),
                new Vector3(1434.3861f, -2673.567f, 40.079872f),
                new Vector3(1430.6506f, -2671.9326f, 40.29323f),
                new Vector3(1427.993f, -2664.9543f, 41.093967f),
                new Vector3(1427.2565f, -2660.995f, 41.83676f),
                new Vector3(1426.5863f, -2657.1755f, 42.589615f),
                new Vector3(1425.3171f, -2649.687f, 44.05556f),
                new Vector3(1424.6912f, -2642.5405f, 45.105225f),
                new Vector3(1424.2268f, -2637.561f, 46.02538f),
                new Vector3(1421.8424f, -2630.0935f, 46.320988f),
                new Vector3(1418.3112f, -2625.4573f, 46.17858f),
                new Vector3(1414.2745f, -2628.7527f, 45.867725f),
                new Vector3(1412.6495f, -2636.7542f, 45.24439f),
                new Vector3(1412.9005f, -2642.6392f, 44.289967f),
                new Vector3(1414.0371f, -2648.0698f, 43.285362f),
                new Vector3(1415.403f, -2654.2874f, 41.713425f),
                new Vector3(1416.2336f, -2660.4126f, 40.09193f),
                new Vector3(1417.0759f, -2666.1355f, 39.01677f),
                new Vector3(1417.9542f, -2671.6353f, 38.33726f),
                new Vector3(1419.4879f, -2680.5903f, 37.829693f),
                new Vector3(1422.9379f, -2689.0488f, 37.36116f),
                new Vector3(1423.1725f, -2697.9827f, 35.801514f),
                new Vector3(1415.3992f, -2700.8745f, 34.69894f),
                new Vector3(1431.9109f, -2700.1575f, 34.868343f),
                new Vector3(1438.2236f, -2694.3499f, 34.07563f),
                new Vector3(1444.5042f, -2688.1309f, 33.623894f)
            };
            for (int i = 0; i < high1.Count; i++)
            {
                high1[i] = new Vector3(high1[i].X, high1[i].Y, high1[i].Z - 0.5);
            }
            Grounds.Add(new Ground("prop_plant_int_02b", Grounds.Count, 903, high1, ref playerDataManager));

            List<Vector3> high2 = new List<Vector3>()
            {
                new Vector3(-487.4333f, 1595.349f, 366.91946f),
                new Vector3(-490.1923f, 1588.3231f, 368.73474f),
                new Vector3(-492.10086f, 1581.0426f, 370.6775f),
                new Vector3(-492.86908f, 1576.7296f, 372.131f),
                new Vector3(-493.96075f, 1569.0576f, 374.86465f),
                new Vector3(-494.09366f, 1565.005f, 376.61923f),
                new Vector3(-493.99908f, 1561.0083f, 378.36154f),
                new Vector3(-494.0127f, 1561.1582f, 378.2944f),
                new Vector3(-494.23657f, 1556.1854f, 380.20694f),
                new Vector3(-493.47507f, 1549.1494f, 383.15082f),
                new Vector3(-490.50455f, 1542.4043f, 386.16928f),
                new Vector3(-488.04138f, 1535.1781f, 388.163f),
                new Vector3(-488.17676f, 1527.1316f, 388.76358f),
                new Vector3(-488.0649f, 1523.066f, 389.32883f),
                new Vector3(-483.6698f, 1516.2903f, 389.91278f),
                new Vector3(-486.89636f, 1508.3623f, 389.04166f),
                new Vector3(-474.81363f, 1513.0264f, 388.79245f),
                new Vector3(-472.3532f, 1519.6097f, 390.32703f),
                new Vector3(-471.20715f, 1523.2731f, 391.0047f),
                new Vector3(-469.8875f, 1527.2676f, 391.36646f),
                new Vector3(-468.2756f, 1532.0302f, 390.8546f),
                new Vector3(-473.67838f, 1536.7242f, 390.10162f),
                new Vector3(-474.7097f, 1543.7715f, 387.09454f),
                new Vector3(-475.8162f, 1548.9169f, 384.46042f),
                new Vector3(-476.64172f, 1552.9949f, 383.13184f),
                new Vector3(-477.88965f, 1559.3584f, 379.68375f),
                new Vector3(-479.13983f, 1565.7969f, 375.68588f),
                new Vector3(-479.8326f, 1572.4633f, 373.1664f),
                new Vector3(-479.00528f, 1579.3813f, 370.6376f),
                new Vector3(-477.42856f, 1586.1047f, 368.34152f),
                new Vector3(-477.0235f, 1592.3319f, 366.63794f),
                new Vector3(-477.34183f, 1598.4825f, 364.74634f),
                new Vector3(-477.21198f, 1603.8025f, 362.41925f),
                new Vector3(-474.93643f, 1609.9005f, 359.2578f),
                new Vector3(-473.69016f, 1615.108f, 356.88138f),
                new Vector3(-468.30188f, 1609.5525f, 358.19394f),
                new Vector3(-468.62457f, 1605.224f, 359.8888f),
                new Vector3(-469.02078f, 1601.549f, 361.3947f),
                new Vector3(-470.04657f, 1594.6326f, 363.94922f),
                new Vector3(-470.76727f, 1589.4545f, 365.52734f),
                new Vector3(-471.99622f, 1581.1779f, 368.27194f),
                new Vector3(-472.60568f, 1576.7495f, 369.82898f),
                new Vector3(-473.2397f, 1571.9438f, 371.60406f),
                new Vector3(-471.6162f, 1564.9739f, 374.13144f),
                new Vector3(-469.91992f, 1558.9413f, 377.32983f),
                new Vector3(-468.14142f, 1552.3329f, 381.02798f),
                new Vector3(-466.6276f, 1546.6884f, 384.49606f),
                new Vector3(-464.96225f, 1540.4829f, 387.54f),
                new Vector3(-460.0549f, 1533.8898f, 389.31326f),
                new Vector3(-453.47174f, 1531.3281f, 388.72595f),
                new Vector3(-454.6011f, 1525.2405f, 388.82806f),
                new Vector3(-448.78223f, 1525.281f, 388.0318f),
                new Vector3(-443.99664f, 1526.22f, 387.46942f),
                new Vector3(-439.15363f, 1527.1622f, 387.1137f),
                new Vector3(-431.95053f, 1528.4752f, 386.73212f),
                new Vector3(-432.33408f, 1534.2664f, 385.92117f),
                new Vector3(-440.46567f, 1539.8043f, 384.36053f),
                new Vector3(-442.21164f, 1544.9801f, 381.31552f),
                new Vector3(-443.98672f, 1550.2537f, 378.29926f),
                new Vector3(-446.6703f, 1555.1122f, 375.69952f),
                new Vector3(-449.59076f, 1559.6053f, 373.43192f),
                new Vector3(-453.81818f, 1565.1934f, 370.949f),
                new Vector3(-456.20285f, 1570.0902f, 368.48645f),
                new Vector3(-457.82513f, 1575.2101f, 366.32166f),
                new Vector3(-459.52997f, 1580.2977f, 364.1567f),
                new Vector3(-461.7134f, 1586.4473f, 362.57803f)
            };
            for (int i = 0; i < high2.Count; i++)
            {
                high2[i] = new Vector3(high2[i].X, high2[i].Y, high2[i].Z - 0.5);
            }
            Grounds.Add(new Ground("prop_plant_01a", Grounds.Count, 904, high2, ref playerDataManager));



            Grounds.Add(new Ground("prop_plant_fern_01a", Grounds.Count, 902, new List<Vector3>()
            {
                new Vector3(-1632.8395f, 4999.7935f, 44.054714f),
                new Vector3(-1638.3939f, 4996.961f, 42.414806f),
                new Vector3(-1644.2739f, 4994.3843f, 40.871677f),
                new Vector3(-1650.052f, 4992.1055f, 39.391426f),
                new Vector3(-1654.6696f, 4988.673f, 38.17628f),
                new Vector3(-1659.0663f, 4985.6113f, 37.230255f),
                new Vector3(-1664.4171f, 4983.456f, 36.18331f),
                new Vector3(-1669.7507f, 4982.76f, 35.160934f),
                new Vector3(-1669.4003f, 4991.054f, 34.828968f),
                new Vector3(-1661.9014f, 4994.519f, 36.299034f),
                new Vector3(-1654.513f, 4995.6655f, 37.971565f),
                new Vector3(-1648.1528f, 5000.944f, 39.070614f),
                new Vector3(-1639.9686f, 4998.902f, 41.596542f),
                new Vector3(-1633.7043f, 5004.1587f, 43.23751f),
                new Vector3(-1635.6022f, 5007.6763f, 42.23555f),
                new Vector3(-1637.1094f, 5011.949f, 41.47223f),
                new Vector3(-1643.7394f, 5010.3813f, 39.49649f),
                new Vector3(-1649.9602f, 5011.3394f, 38.024197f),
                new Vector3(-1659.3989f, 5012.6567f, 36.171623f),
                new Vector3(-1661.0328f, 5005.5425f, 35.72507f),
                new Vector3(-1671.0219f, 5002.636f, 33.35971f),
                new Vector3(-1674.0035f, 5008.056f, 32.165653f),
                new Vector3(-1672.7506f, 5015.5635f, 33.54721f),
                new Vector3(-1661.3495f, 5019.8955f, 36.175205f),
                new Vector3(-1654.1438f, 5019.6167f, 37.308075f),
                new Vector3(-1651.6498f, 5023.315f, 37.789623f),
                new Vector3(-1649.6841f, 5026.5654f, 38.25817f),
                new Vector3(-1642.4303f, 5028.0503f, 40.152485f),
                new Vector3(-1635.4268f, 5025.3745f, 42.110203f),
                new Vector3(-1637.8824f, 5034.235f, 41.839092f),
                new Vector3(-1636.8403f, 5041.956f, 41.57152f),
                new Vector3(-1636.4746f, 5047.987f, 40.239616f),
                new Vector3(-1641.4406f, 5054.242f, 39.3372f),
                new Vector3(-1649.4723f, 5053.2837f, 38.66914f),
                new Vector3(-1654.8877f, 5051.68f, 37.539474f),
                new Vector3(-1664.4335f, 5052.806f, 35.265255f),
                new Vector3(-1670.8113f, 5048.406f, 34.737057f),
                new Vector3(-1670.8008f, 5040.917f, 35.095642f),
                new Vector3(-1669.4471f, 5033.183f, 35.470127f),
                new Vector3(-1680.0692f, 5041.8994f, 34.215866f),
                new Vector3(-1685.4423f, 5048.864f, 33.310905f),
                new Vector3(-1690.0322f, 5045.6006f, 33.235363f),
                new Vector3(-1693.5946f, 5038.867f, 33.55063f),
                new Vector3(-1692.3424f, 5030.653f, 32.2264f),
                new Vector3(-1705.2712f, 5031.1113f, 29.812767f),
                new Vector3(-1712.7449f, 5038.658f, 29.335142f),
                new Vector3(-1719.1979f, 5045.455f, 28.629986f),
                new Vector3(-1722.6465f, 5052.8945f, 27.685629f),
                new Vector3(-1716.8428f, 5057.9707f, 28.114044f),
                new Vector3(-1728.1769f, 5041.301f, 26.397661f),
                new Vector3(-1728.9556f, 5032.256f, 25.356245f),
                new Vector3(-1731.6472f, 5028.544f, 24.973915f),
                new Vector3(-1722.1537f, 5022.7495f, 24.998915f),
                new Vector3(-1714.7089f, 5020.688f, 25.553583f),
                new Vector3(-1705.4781f, 5018.022f, 26.837837f)
            }, ref playerDataManager));

            Grounds.Add(new Ground("prop_plant_fern_01b", Grounds.Count, 901, new List<Vector3>()
            {
                new Vector3(-1002.97406f, 295.11816f, 68.1172f),
                new Vector3(-998.96906f, 295.3009f, 68.22273f),
                new Vector3(-991.34686f, 295.34262f, 68.41844f),
                new Vector3(-986.2344f, 295.11374f, 68.58291f),
                new Vector3(-978.0245f, 294.48508f, 68.88432f),
                new Vector3(-973.02234f, 293.96167f, 69.06241f),
                new Vector3(-965.4765f, 291.99918f, 69.35997f),
                new Vector3(-960.3512f, 290.74625f, 69.59852f),
                new Vector3(-957.8658f, 294.40305f, 69.895515f),
                new Vector3(-957.69305f, 302.32428f, 70.561356f),
                new Vector3(-962.95105f, 298.51096f, 69.837036f),
                new Vector3(-967.5222f, 298.76718f, 69.6079f),
                new Vector3(-972.6927f, 299.89667f, 69.400566f),
                new Vector3(-981.26624f, 298.8715f, 68.98729f),
                new Vector3(-985.6621f, 300.25043f, 68.88186f),
                new Vector3(-993.81824f, 300.2262f, 68.56819f),
                new Vector3(-998.8146f, 299.52585f, 68.3804f),
                new Vector3(-997.9729f, 308.53665f, 68.70473f),
                new Vector3(-1000.60114f, 312.7184f, 68.74608f),
                new Vector3(-1002.6344f, 316.39984f, 68.83483f),
                new Vector3(-1003.82715f, 323.9611f, 69.26429f),
                new Vector3(-996.97424f, 326.28046f, 69.785545f),
                new Vector3(-996.11975f, 318.00613f, 69.272026f),
                new Vector3(-988.22955f, 316.3772f, 69.61316f),
                new Vector3(-985.83813f, 320.24744f, 69.950134f),
                new Vector3(-986.9013f, 327.31158f, 70.30476f),
                new Vector3(-982.93066f, 331.16916f, 70.71618f),
                new Vector3(-976.77386f, 335.31372f, 71.21365f),
                new Vector3(-973.3868f, 338.0654f, 71.51339f),
                new Vector3(-970.88965f, 335.91467f, 71.42207f),
                new Vector3(-966.2052f, 332.89832f, 71.18436f),
                new Vector3(-963.4721f, 326.22318f, 71.09627f),
                new Vector3(-963.02325f, 321.73904f, 70.957436f),
                new Vector3(-961.535f, 317.06808f, 70.92246f),
                new Vector3(-960.01685f, 312.5426f, 70.90406f),
                new Vector3(-965.4161f, 310.2634f, 70.3928f),
                new Vector3(-970.761f, 309.27872f, 69.97297f),
                new Vector3(-971.18066f, 313.89944f, 70.123856f),
                new Vector3(-966.3157f, 316.26746f, 70.50894f),
                new Vector3(-971.0518f, 324.10107f, 70.61826f),
                new Vector3(-974.6353f, 320.76245f, 70.3831f),
                new Vector3(-979.56757f, 321.021f, 70.29261f),
                new Vector3(-984.0465f, 320.56174f, 70.07975f),
                new Vector3(-968.8552f, 300.53818f, 69.63193f)
            }, ref playerDataManager));
        }
    }


    public class Ground
    {
        public List<Vector3> PlantPlaces { get; set; }

        PlayerDataManager playerDataManager;
        public string PlantModel { get; set; }
        public Dictionary<Vector3, Plant> Plants = new Dictionary<Vector3, Plant>();
        int LastId = 0;
        public int PlantType { get; set; }
        public int GroundId { get; set; }

        public Ground(string model, int id, int plantType, List<Vector3> places, ref PlayerDataManager playerDataManager)
        {
            for(int i = 0; i < places.Count; i++)
            {
                places[i] = new Vector3(places[i].X, places[i].Y, places[i].Z - 1);
            }
            PlantType = plantType;
            GroundId = id;
            PlantModel = model;
            PlantPlaces = places;
            Instantiate();
            this.playerDataManager = playerDataManager;
        }

        private void Instantiate()
        {
            Random rnd = new Random();
            for(int i = 0; i < PlantPlaces.Count/2; i++)
            {

                Vector3 pos = PlantPlaces[rnd.Next(0, PlantPlaces.Count)];

                while(Plants.ContainsKey(pos))
                {
                    pos = PlantPlaces[rnd.Next(0, PlantPlaces.Count)];
                }

                Plants.Add(pos, new Plant(pos, PlantModel, GroundId, LastId, PlantType));
                LastId++;
            }
        }

        private void NewPlant()
        {
            Random rnd = new Random();
            Vector3 pos = PlantPlaces[rnd.Next(0, PlantPlaces.Count)];

            while (Plants.ContainsKey(pos))
            {
                pos = PlantPlaces[rnd.Next(0, PlantPlaces.Count)];
            }

            Plants.Add(pos, new Plant(pos, PlantModel, GroundId, LastId, PlantType));
            LastId++;
        }

        public void PickPlantUp(Player player, KeyValuePair<Vector3, Plant> p)
        {
            if (Plants.ContainsKey(p.Key) && Plants[p.Key] == p.Value)
            {
                Plant plant = Plants[p.Key];
                if (!plant.PickingUp)
                {
                    plant.PickingUp = true;
                    player.TriggerEvent("gardener_pickupAnimate", plant.Type);
                    NAPI.Task.Run(() =>
                    {
                        if (player.Exists)
                        {
                            player.TriggerEvent("checkIfItemFits", plant.Type, "plant");
                            plant.Remove();
                            Plants.Remove(p.Key);
                            NewPlant();
                        }
                        else
                        {
                            plant.PickingUp = false;
                        }
                    }, (plant.Type == 903 || plant.Type == 904) ? 5000 : 3000);
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Roślina jest już zbierana przez kogoś innego!");
                }
            }
            
        }
    }

    public class Plant
    {
        public Vector3 Position { get; set; }
        public string Model { get; set; }
        public ColShape Colshape { get; set; }
        public Object PlantObject { get; set; }
        public bool PickingUp { get; set; }
        public int Id { get; set; }
        
        public int Type { get; set; }
        public Plant(Vector3 position, string model, int groundId, int plantId, int type)
        {
            Type = type;
            Id = plantId;
            PickingUp = false;
            Position = position;
            Model = model;
            PlantObject = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(model), position, new Vector3());
            Colshape = NAPI.ColShape.CreateCylinderColShape(position, 0.8f, 2.0f);
            Colshape.SetSharedData("type", "gardener_plant");
            Colshape.SetSharedData("plantId", Id);
            Colshape.SetSharedData("groundId", groundId);
        }

        public void Remove()
        {
            if(PlantObject.Exists)
            {
                PlantObject.Delete();
            }
            if(Colshape.Exists)
            {
                Colshape.Delete();
            }
        }
    }

    public class GardenerOrder
    {
        public int[] Plants = new int[] { 0, 0, 0, 0, 0 };
        public int Id { get; set; }

        public GardenerOrder(int id)
        {
            GenerateOrder();
            Id = id;
        }

        private void GenerateOrder()
        {
            Random rnd = new Random();
            int orderWeight = 0;
            int orderMaxWeight = rnd.Next(60, 96);
            while(orderWeight < orderMaxWeight)
            {
                int item = rnd.Next(1, 6);
                switch(item)
                {
                    case 1:
                        orderWeight++;
                        Plants[0]++;
                        break;
                    case 2:
                        orderWeight++;
                        Plants[1]++;
                        break;
                    case 3:
                        orderWeight++;
                        Plants[2]++;
                        break;
                    case 4:
                        orderWeight += 2;
                        Plants[3]++;
                        break;
                    case 5:
                        orderWeight += 2;
                        Plants[4]++;
                        break;
                }
            }
        }
    }
}

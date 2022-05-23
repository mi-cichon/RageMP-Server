using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using GTANetworkAPI;

namespace ServerSide
{
    public class Refinery
    {
        public List<OilPump> oilPumps = new List<OilPump>();
        List<Vector3> smallPumps = new List<Vector3>() 
        {
            new Vector3(1876.1083f, -1036.4635f, 79.453316f),
            new Vector3(1869.3633f, -1122.388f, 86.265205f),
            new Vector3(1833.6986f, -1174.2456f, 91.45703f),
            new Vector3(1770.2701f, -1323.0969f, 95.07098f),
            new Vector3(1659.3352f, -1525.3187f, 112.68451f),
            new Vector3(1459.2634f, -1669.206f, 66.24122f),
            new Vector3(1454.1842f, -1706.5806f, 67.45594f),
            new Vector3(1502.215f, -1747.0444f, 78.98162f),
            new Vector3(1580.4843f, -1854.3445f, 94.16772f),
            new Vector3(1693.91f, -1923.3009f, 115.36639f),
            new Vector3(1427.7397f, -2100.6194f, 55.459118f),
            new Vector3(1435.8785f, -2303.6926f, 67.03722f),
            new Vector3(1370.5785f, -2280.2378f, 61.488792f),
            new Vector3(1130.704f, -2457.7275f, 31.327618f),
            new Vector3(1265.7849f, -2343.2583f, 50.842274f),
            new Vector3(1292.6829f, -1966.1907f, 43.60764f),
            new Vector3(1255.8015f, -1907.7957f, 38.498386f),
            new Vector3(1227.3159f, -1888.6815f, 38.495846f),
            new Vector3(1340.1947f, -1865.7709f, 57.090088f),
        };
        List<Vector3> bigPumps = new List<Vector3>()
        {
            new Vector3(1883.5123f, -1027.9612f, 78.78056f),
            new Vector3(1362.6289f, -1883.235f, 56.730183f),
            new Vector3(1215.9015f, -1871.783f, 38.49177f),
            new Vector3(1228.6844f, -1868.572f, 38.495804f),
            new Vector3(1342.0835f, -1852.6484f, 57.12203f),
            new Vector3(1262.482f, -1952.8942f, 43.263973f),
            new Vector3(1256.1099f, -1939.6157f, 43.2639f),
            new Vector3(1208.3168f, -2215.245f, 41.423317f),
            new Vector3(1193.9226f, -2198.115f, 41.423325f),
            new Vector3(1169.3217f, -2128.1318f, 43.25021f),
            new Vector3(1517.9792f, -2539.1814f, 56.90238f),
            new Vector3(1506.9315f, -2538.0647f, 55.791298f),
            new Vector3(1215.4717f, -2459.8494f, 44.481594f),
            new Vector3(1205.4285f, -2443.5676f, 44.481594f),
            new Vector3(1414.781f, -2302.841f, 66.56705f),
            new Vector3(1363.8367f, -2202.372f, 60.207203f),
            new Vector3(1371.9437f, -2204.6492f, 60.46684f),
            new Vector3(1438.7919f, -2267.6653f, 66.430885f),
            new Vector3(1525.2156f, -2065.866f, 77.28148f),
            new Vector3(1520.9663f, -2178.7432f, 77.65018f),
            new Vector3(1436.4609f, -2080.3284f, 54.51442f),
            new Vector3(1676.8634f, -1854.3104f, 108.378815f),
            new Vector3(1663.1727f, -1838.825f, 109.412926f),
            new Vector3(1573.7708f, -1763.1418f, 88.25973f),
            new Vector3(1566.3181f, -1856.696f, 92.441055f),
            new Vector3(1714.6597f, -1682.0493f, 112.56814f),
            new Vector3(1564.1475f, -1597.6931f, 90.712494f),
            new Vector3(1481.8195f, -1598.6877f, 72.13866f),
            new Vector3(1472.0084f, -1606.4293f, 70.87708f),
            new Vector3(1787.6278f, -1345.3794f, 99.33922f),
            new Vector3(1691.9392f, -1439.3933f, 112.44483f),
            new Vector3(1687.305f, -1450.0676f, 112.06876f),
            new Vector3(1833.6198f, -1188.4087f, 92.0349f),
            new Vector3(1509.6155f, -1720.7355f, 78.87619f),
            new Vector3(646.2772f, 3011.4744f, 43.289883f),
            new Vector3(601.8901f, 3024.4104f, 41.998592f),
            new Vector3(497.3609f, 2953.8308f, 42.381535f),
            new Vector3(544.4821f, 2882.9194f, 42.941418f),
            new Vector3(616.3299f, 2854.271f, 39.900692f),
            new Vector3(587.194f, 2926.5662f, 40.87846f),
            new Vector3(647.38135f, 2928.0122f, 41.995712f),
            new Vector3(700.7452f, 2883.4585f, 50.28966f)
        };
        PlayerDataManager playerDataManager;
        public Refinery(ref PlayerDataManager playerDataManager)
        {
            this.playerDataManager = playerDataManager;
            Vector3 startPos = new Vector3(2747.9456f, 1332.1823f, 25.136633f);
            ColShape start = NAPI.ColShape.CreateCylinderColShape(startPos, 1.0f, 2.0f);
            start.SetSharedData("type", "refinery");
            new CustomMarkers().CreateJobMarker(startPos, "Rafineria");
            NAPI.Blip.CreateBlip(750, startPos, 0.8f, 69, name: "Praca: Rafineria", shortRange: true);
            foreach(Vector3 smallPump in smallPumps)
            {
                oilPumps.Add(new OilPump(smallPump, 1200));
            }
            foreach (Vector3 bigPump in bigPumps)
            {
                oilPumps.Add(new OilPump(bigPump, 2000));
            }
            System.Timers.Timer refillPumps = new System.Timers.Timer(90000);
            refillPumps.Elapsed += RefillPumps;
            refillPumps.Enabled = true;
        }

        public void StartJob(Player player)
        {
            if (!player.GetSharedData<bool>("nodriving"))
            {
                if (player.GetSharedData<bool>("jobBonus_12"))
                {
                    if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
                    {
                        player.SetSharedData("job", "refinery");
                        player.TriggerEvent("refinery_openHUDBrowser");
                        player.TriggerEvent("startJob", "Rafineria", "PS");
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Masz inną pracę!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie odblokowałeś tej pracy!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie możesz prowadzić pojazdów do " + player.GetSharedData<string>("nodrivingto") + "!");
            }
        }

        public void RefillPumps(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (OilPump oilPump in oilPumps)
                {
                    if (oilPump.OilAmount < oilPump.MaxCapacity)
                    {
                        oilPump.OilAmount += oilPump.MaxCapacity * 0.1f;
                        if (oilPump.OilAmount > oilPump.MaxCapacity)
                            oilPump.OilAmount = oilPump.MaxCapacity;
                        oilPump.Colshape.SetSharedData("oilAmount", oilPump.OilAmount);
                        oilPump.Label.Text = "Zapełenienie: " + Convert.ToInt32(oilPump.OilAmount / oilPump.MaxCapacity * 100).ToString() + "%";
                    }
                }
            });
        }
    }

    public class OilPump
    {
        public Vector3 Position { get; set; }
        public float MaxCapacity { get; set; }
        public float OilAmount { get; set; }
        public ColShape Colshape { get; set; }
        public TextLabel Label { get; set; }

        public OilPump(Vector3 Position, float MaxCapacity)
        {
            Colshape = NAPI.ColShape.CreateCylinderColShape(Position - new Vector3(0,0,2), 8.0f, 5.0f);
            Colshape.SetSharedData("type", "refineryPump");
            Colshape.SetSharedData("oilAmount", MaxCapacity);
            Colshape.SetSharedData("maxCapacity", MaxCapacity);
            this.MaxCapacity = MaxCapacity;
            OilAmount = MaxCapacity;
            Label = NAPI.TextLabel.CreateTextLabel("Zapełenienie: " + Convert.ToInt32(OilAmount / MaxCapacity * 100).ToString() + "%", Position + new Vector3(0, 0, 2.5), 10.0f, 1.0f, 0, new Color(255, 253, 141, 255), entitySeethrough: true);
        }

        public void UpdateOilAmount(float amount)
        {
            OilAmount = amount;
            Colshape.SetSharedData("oilAmount", OilAmount);
            Label.Text = "Zapełenienie: " + Convert.ToInt32(OilAmount / MaxCapacity * 100).ToString() + "%";
        }
    }
}

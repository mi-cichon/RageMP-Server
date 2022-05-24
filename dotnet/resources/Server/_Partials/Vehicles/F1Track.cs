using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        public void CreateF1Track()
        {

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("ch_prop_track_paddock_01"), new Vector3(1278.25700000, -3360.33800000, 3.93215000), new Vector3(0, 0, 0)));

            objList.Add(NAPI.Object.CreateObject(-608407618, new Vector3(1304.71300000, -3385.68800000, 4.68288200), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("ch_prop_track_pit_garage_01a"), new Vector3(1304.98500000, -3422.50800000, 4.67336100), new Vector3(0, 0, 180)));

            objList.Add(NAPI.Object.CreateObject(1299320654, new Vector3(1392.39800000, -3397.19000000, 4.67336100), new Vector3(0, 0, -90.00001)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1496.37500000, -3385.68800000, 4.68288200), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("ch_prop_track_bend_bar_lc"), new Vector3(1679.69500000, -3398.57400000, 4.66664700), new Vector3(0, 0, 90.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1602.32100000, -3385.68800000, 4.68288200), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_L_b"), new Vector3(1705.42700000, -3447.77600000, 4.66676900), new Vector3(0, 0, 269.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(1755.29300000, -3455.45700000, 4.68312600), new Vector3(0, 0, -7.1046765E-05)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(1794.15800000, -3441.93300000, 4.68312600), new Vector3(0, 0, -179.99992)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(1839.67100000, -3440.55700000, 4.68312600), new Vector3(0, 0, 149.99992)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(1879.73800000, -3462.12900000, 4.68361500), new Vector3(0, 0, 119.99991)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(1903.68100000, -3500.85000000, 4.68385900), new Vector3(0, 0, 89.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1908.87600000, -3684.86000000, 4.68300400), new Vector3(0, 0, -90.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1908.87600000, -3579.02100000, 4.68300400), new Vector3(0, 0, -90.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("ch_prop_track_bend_bar_lc"), new Vector3(1921.75200000, -3762.41500000, 4.63820500), new Vector3(0, 0, 269.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_L_b"), new Vector3(1970.95800000, -3788.21800000, 4.66921000), new Vector3(0, 0, 90.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1983.88100000, -3865.77000000, 4.68300400), new Vector3(0, 0, -90.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_L_b"), new Vector3(1970.95800000, -3942.76600000, 4.66921000), new Vector3(0, 0, -7.563043E-05)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1893.39200000, -3955.65800000, 4.68288200), new Vector3(0, 0, -7.906817E-05)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_L"), new Vector3(1816.93900000, -3941.22000000, 4.64821400), new Vector3(0, 0, 270.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1800.65500000, -3863.60200000, 4.68202800), new Vector3(0, 0, 95.0001)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(1790.69500000, -3784.97900000, 4.68288200), new Vector3(0, -0, 94.99992)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1767.28300000, -3712.77700000, 4.68202800), new Vector3(0, 0, 110.00008)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_M"), new Vector3(1739.21600000, -3648.23200000, 4.68385900), new Vector3(0, -0, 109.99993)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_M"), new Vector3(1718.53500000, -3636.62000000, 4.68239400), new Vector3(0, 0, 290.00008)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_M"), new Vector3(1692.02600000, -3556.55100000, 4.68239400), new Vector3(0, -0, 94.99993)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(1703.14200000, -3599.18200000, 4.71718400), new Vector3(0, 0, 275.0001)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_M"), new Vector3(1667.50200000, -3540.40600000, 4.69533300), new Vector3(0, -0, 140.00006)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1604.07200000, -3540.56200000, 4.68300400), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1498.05900000, -3540.56200000, 4.68300400), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1392.21500000, -3540.56200000, 4.68300400), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1286.31500000, -3540.56200000, 4.68300400), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1180.45600000, -3540.56200000, 4.68300400), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1158.26300000, -3540.55900000, 4.67812200), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1113.01300000, -3385.67900000, 4.69606600), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(1033.79300000, -3388.70900000, 4.69814100), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(986.18320000, -3401.83900000, 4.69814100), new Vector3(-0, 0, 194.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(943.64320000, -3426.85900000, 4.69814100), new Vector3(-0, 0, 209.99992)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_L"), new Vector3(915.10310000, -3468.23900000, 4.69814100), new Vector3(-0, 0, 224.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(942.10310000, -3511.40900000, 4.69814100), new Vector3(0, 0, 314.999935)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(985.03310000, -3533.41900000, 4.69814100), new Vector3(0, 0, 329.999939)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1052.55300000, -3538.72900000, 4.67812200), new Vector3(0, 0, -2.00006)));

        }
    }
}

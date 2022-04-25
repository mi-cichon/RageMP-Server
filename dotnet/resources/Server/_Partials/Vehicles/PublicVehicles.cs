using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {


        private void RespawnPublicVehicles(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (PublicVehicleSpawn pvs in publicVehicleSpawns)
                {
                    if (pvs.veh == null && !NAPI.Pools.GetAllVehicles().Any(vehicle => pvs.colshape.IsPointWithin(vehicle.Position)))
                    {
                        pvs.CreatePublicVehicle();
                    }
                }
                foreach (JobVehicleSpawn jobVehicleSpawn in jobVehicleSpawns)
                {
                    if (jobVehicleSpawn.Veh == null && !NAPI.Pools.GetAllVehicles().Any(vehicle => jobVehicleSpawn.Col.IsPointWithin(vehicle.Position)))
                    {
                        jobVehicleSpawn.CreateNewVehicle(true);
                    }
                }
            });
        }

        public void CreatePublicVehicles()
        {
            //sandy spawn
            // new Vector3(103.76306f, -1083.1083f, 29.19238f); heading: -25.003464f  -  szkuteratuti
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-358.64642f, -126.77186f, 38.695824f), 68.55481f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-802.21204f, -227.84772f, 37.193943f), 118.34483f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-1576.1237f, -551.59076f, 34.953655f), 31.633419f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-1623.031f, -876.25134f, 9.440182f), -44.95958f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-51.619274f, -1081.5751f, 26.90126f), 71.67744f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(146.28804f, -2507.457f, 5.9999943f), -72.78241f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(9.326692f, -2757.4258f, 6.004301f), 0.8770372f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(1302.9294f, -3343.4783f, 5.5801206f), -90.95663f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-1368.5206f, 46.290375f, 53.905636f), 89.611f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-1684.6394f, 58.149143f, 64.02879f), 136.98888f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(1897.2589f, 3710.4368f, 32.755775f), 118.230255f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(1659.4358f, 3814.761f, 34.869144f), -52.362194f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(2548.382f, 4664.3765f, 34.07682f), 16.364094f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-121.30436f, 6392.6216f, 31.48985f), 43.430614f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-81.15758f, 6472.5234f, 31.479202f), -141.37119f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-11.886782f, 6303.1455f, 31.376215f), 21.04515f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(115.53413f, -1087.2607f, 29.214767f), 25.903833f, this));

            //szpitale

            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-451.15082f, -351.43372f, 34.50172f), 82.60945f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(296.77106f, -601.909f, 43.30345f), 115.56599f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(1146.9432f, -1524.7622f, 34.843403f), -19.130648f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(1837.8899f, 3667.2068f, 33.67998f), -149.71346f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-242.66228f, 6325.4795f, 32.426174f), -101.287056f, this));

        }
    }
}

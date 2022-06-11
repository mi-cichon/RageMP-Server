using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        public void CreateJobVehicles()
        {
            //LAWETY
            jobVehicleSpawns.Add(new JobVehicleSpawn("towtruck", new Vector3(575.0778f, -3037.5444f, 6.0692883f), 0f, VehicleHash.Flatbed, 42, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "towtruck")));
            jobVehicleSpawns.Add(new JobVehicleSpawn("towtruck", new Vector3(588.34125f, -3037.7256f, 6.0692873f), 0f, VehicleHash.Flatbed, 42, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "towtruck")));
            jobVehicleSpawns.Add(new JobVehicleSpawn("towtruck", new Vector3(581.9523f, -3037.773f, 6.069285f), 0f, VehicleHash.Flatbed, 42, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "towtruck")));

            //KOSIARKI
            jobVehicleSpawns.Add(new JobVehicleSpawn("lawnmowing", new Vector3(-1357.8574f, 140.44446f, 56.252647f), -85.41613f, VehicleHash.Mower, 53, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "lawnmowing")));
            jobVehicleSpawns.Add(new JobVehicleSpawn("lawnmowing", new Vector3(-1357.5975f, 136.84306f, 56.25227f), -85.41613f, VehicleHash.Mower, 53, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "lawnmowing")));

            //MYŚLIWY
            jobVehicleSpawns.Add(new JobVehicleSpawn("hunter", new Vector3(-1491.6906f, 4973.8906f, 63.787167f), 86.14462f, VehicleHash.Blazer, 53, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "hunter")));
            jobVehicleSpawns.Add(new JobVehicleSpawn("hunter", new Vector3(-1491.5835f, 4976.113f, 63.65564f), 86.14462f, VehicleHash.Blazer, 53, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "hunter")));

            //OGRODNIK
            jobVehicleSpawns.Add(new JobVehicleSpawn("gardener", new Vector3(1542.5646f, 2175.6204f, 78.80925f), 90f, VehicleHash.Kalahari, 53, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "gardener")));
            jobVehicleSpawns.Add(new JobVehicleSpawn("gardener", new Vector3(1542.5675f, 2179.2874f, 78.810936f), 90f, VehicleHash.Kalahari, 53, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "gardener")));

            //WÓZKI WIDŁOWE
            jobVehicleSpawns.Add(new JobVehicleSpawn("forklifts", new Vector3(-555.99615f, -2362.6638f, 13.993408f), -130.63968f, VehicleHash.Forklift, 42, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "forklifts")));
            jobVehicleSpawns.Add(new JobVehicleSpawn("forklifts", new Vector3(-553.5449f, -2359.4949f, 13.993403f), -130.63968f, VehicleHash.Forklift, 42, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "forklifts")));

            //RAFINERIA
            jobVehicleSpawns.Add(new JobVehicleSpawn("refinery", new Vector3(2762.1292f, 1340.3951f, 25.473862f), -0.9825836f, (VehicleHash)NAPI.Util.GetHashKey("oiltanker"), 42, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "refinery"), true));
            jobVehicleSpawns.Add(new JobVehicleSpawn("refinery", new Vector3(2757.0752f, 1340.4435f, 25.473667f), -0.9825836f, (VehicleHash)NAPI.Util.GetHashKey("oiltanker"), 42, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "refinery"), true));
            jobVehicleSpawns.Add(new JobVehicleSpawn("refinery", new Vector3(2752.2551f, 1340.5282f, 25.473862f), -0.9825836f, (VehicleHash)NAPI.Util.GetHashKey("oiltanker"), 42, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "refinery"), true));

            //NUREK
            jobVehicleSpawns.Add(new JobVehicleSpawn("diver", new Vector3(1339.6202f, 4231.784f, 29.943439f), -100f, VehicleHash.Seashark, 42, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "diver")));
            jobVehicleSpawns.Add(new JobVehicleSpawn("diver", new Vector3(1341.9657f, 4229.7056f, 29.943439f), -10f, VehicleHash.Seashark, 42, "BASICRPG", jobVehicles.Find(veh => veh.JobType == "diver")));
        }
    }
}

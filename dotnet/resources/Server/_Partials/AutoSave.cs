using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("saveData_saveJobData")]
        public void SaveData_saveJobData(Player player, string data)
        {
            autoSave.SavePlayersJobData(player, data);
        }

        [RemoteEvent("saveData_giveJobVeh")]
        public void SaveData_giveJobVeh(Player player, string jobType, Vector3 vehPos, string trunk, float oiltank)
        {
            player.Position = vehPos;
            JobVehicle jobVeh = jobVehicles.Find(veh => veh.JobType == jobType);
            Vehicle veh = jobVeh.CreateVehicle(vehPos, trunk);
            if(oiltank != 0.0f)
            {
                veh.SetSharedData("oiltank", oiltank);
            }
            NAPI.Task.Run(() =>
            {
                if(player.Exists && veh.Exists)
                {
                    player.SetIntoVehicle(veh.Handle, 0);
                }
            }, 1000);

        }
    }
}

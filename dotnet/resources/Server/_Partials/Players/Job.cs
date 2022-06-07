using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {

        [RemoteEvent("endJob")]
        public void EndJob(Player player)
        {
            player.SetSharedData("job", "");
            PlayerDataManager.NotifyPlayer(player, "Praca zakończona");
            player.SetSharedData("handObj", "");
            player.TriggerEvent("stopJob");
            player.RemoveAllWeapons();
            if (player.HasSharedData("jobveh") && player.GetSharedData<int>("jobveh") != -1111)
            {
                var veh = VehicleDataManager.GetVehicleByRemoteId(Convert.ToUInt16(player.GetSharedData<Int32>("jobveh")));
                if (veh != null && veh.Exists)
                    veh.Delete();
                player.SetSharedData("jobveh", -1111);

            }
            player.SetSharedData("jobveh", -1111);
            player.TriggerEvent("closeGardenerHUDBrowser");
            PlayerDataManager.SetJobClothes(player, false, "");
            AutoSave.SavePlayersJobData(player, "");
        }

    }
}

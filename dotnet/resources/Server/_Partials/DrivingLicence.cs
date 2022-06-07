using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("licenceCheckMoney")]
        public void LicenceCheckMoney(Player player, int money)
        {
            if (PlayerDataManager.UpdatePlayersMoney(player, -1 * money))
            {
                player.TriggerEvent("startLicenceTest");
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie stać Cię na rozpoczęcie tego egzaminu!");
            }
        }

        [RemoteEvent("startTestLicence")]
        public void StartTestLicence(Player player)
        {
            drivingLicences.StartLicenceB(player);
        }

        [RemoteEvent("licenceBpassed")]
        public void LicenceBPassed(Player player)
        {
            player.SetSharedData("job", "");
            PlayerDataManager.NotifyPlayer(player, "Udało Ci się ukończyć egzamin praktyczny Kat. B z wynikiem pozytywnym!");
            player.Position = drivingLicences.endPosition;
            if (player.HasSharedData("jobveh") && player.GetSharedData<int>("jobveh") != -1111)
            {
                var veh = VehicleDataManager.GetVehicleByRemoteId(Convert.ToUInt16(player.GetSharedData<Int32>("jobveh")));
                if (veh != null && veh.Exists)
                    veh.Delete();
                player.SetSharedData("jobveh", -1111);
            }
            player.SetSharedData("licenceBp", true);
            PlayerDataManager.SavePlayerDataToDB(player, "licenceBp");
        }

        [RemoteEvent("licenceBfailed")]
        public void LicenceBFailed(Player player)
        {
            player.SetSharedData("job", "");
            player.Position = drivingLicences.endPosition;
            if (player.HasSharedData("jobveh") && player.GetSharedData<int>("jobveh") != -1111)
            {
                var veh = VehicleDataManager.GetVehicleByRemoteId(Convert.ToUInt16(player.GetSharedData<Int32>("jobveh")));
                if (veh != null && veh.Exists)
                    veh.Delete();
                player.SetSharedData("jobveh", -1111);

            }
            if (drivingLicences.currentPlayerPassing == player)
            {
                drivingLicences.currentPlayerPassing = null;
            }
        }

        [RemoteEvent("playerLeftPassingArea")]
        public void PlayerLeftPassingArea(Player player)
        {
            if (drivingLicences.currentPlayerPassing == player)
            {
                drivingLicences.currentPlayerPassing = null;
            }
        }

        [RemoteEvent("licenceCompleted")]
        public void LicenceCompleted(Player player)
        {
            player.SetSharedData("licenceBt", true);
            PlayerDataManager.SavePlayerDataToDB(player, "licenceBt");
        }
    }
}

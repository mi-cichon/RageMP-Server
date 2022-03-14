using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class DrivingLicences
    {
        CustomMarkers customMarkers = new CustomMarkers();
        PlayerDataManager playerDataManager = new PlayerDataManager();
        public Vector3 endPosition = new Vector3();
        public Player currentPlayerPassing = null;
        public DrivingLicences()
        {

        }

        public void CreateLicenceB(Vector3 colshapePosT, Vector3 colshapePosP, Vector3 endpos)
        {
            ColShape csT = NAPI.ColShape.CreateCylinderColShape(colshapePosT - new Vector3(0, 0, 1), 0.5f, 2.0f);
            csT.SetSharedData("type", "licenceb");
            NAPI.Blip.CreateBlip(536, colshapePosT, 0.5f, 30, name: "Prawo jazdy - Kat. B", shortRange: true);
            customMarkers.CreateSimpleMarker(colshapePosT, "Egzamin teoretyczny");

            ColShape csP = NAPI.ColShape.CreateCylinderColShape(colshapePosP - new Vector3(0, 0, 1), 0.5f, 2.0f);
            csP.SetSharedData("type", "licencebp");
            customMarkers.CreateSimpleMarker(colshapePosP, "Egzamin praktyczny");

            endPosition = endpos;
        }

        public void StartLicenceB(Player player)
        {
            if (player.GetSharedData<string>("job") == "")
            {
                if (player.GetSharedData<bool>("licenceBt") && !player.GetSharedData<bool>("nodriving"))
                {
                    if(currentPlayerPassing != null && currentPlayerPassing.Exists){
                        playerDataManager.NotifyPlayer(player, "Plac jest w tej chwili zajęty. Wróć za chwilę!");
                    }
                    else
                    {
                        if(playerDataManager.UpdatePlayersMoney(player, -140))
                        {
                            currentPlayerPassing = player;
                            Vehicle lVeh = NAPI.Vehicle.CreateVehicle(VehicleHash.Blista, new Vector3(1014.363, -2324.1343, 30.00485), -95f, 44, 44, " KAT. B");
                            lVeh.SetSharedData("licence", true);
                            lVeh.SetSharedData("type", "jobveh");
                            lVeh.SetSharedData("petrol", 25);
                            lVeh.SetSharedData("name", "Blista");
                            lVeh.SetSharedData("petroltank", 35);
                            lVeh.SetSharedData("combustion", 7);
                            lVeh.SetSharedData("speed", 160);
                            lVeh.SetSharedData("power", 0);
                            player.SetIntoVehicle(lVeh.Handle, 0);
                            player.SetSharedData("jobveh", lVeh.Id);
                            player.SetSharedData("job", "licenceB");
                            lVeh.Rotation = new Vector3(0, 0, -95f);
                            lVeh.SetSharedData("collision", false);
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie posiadasz wystarczająco gotówki ($140)");
                        }
                    }
                    
                }
                else
                {
                    if(!player.GetSharedData<bool>("licenceBt"))
                        playerDataManager.NotifyPlayer(player, "Nie zdałeś egzaminu teoretycznego!");
                    if(player.GetSharedData<bool>("nodriving")){
                        playerDataManager.NotifyPlayer(player, "Nie możesz prowadzić pojazdów!");
                    }
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Najpierw zakończ pracę!");
            }
            
        }

    }
}

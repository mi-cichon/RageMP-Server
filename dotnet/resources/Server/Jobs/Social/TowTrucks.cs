using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;

namespace ServerSide.Jobs
{
    public class TowTrucks
    {
        Vector3 startPosition;
        ColShape startColshape;
        Blip startBlip;
        public Vector3 basePosition;
        Dictionary<Vector3, float> startPositions = new Dictionary<Vector3, float>();
        PlayerDataManager playerDataManager = new PlayerDataManager();
        CustomMarkers customMarkers = new CustomMarkers();
        VehicleDataManager vehicleDataManager = new VehicleDataManager();
        public TowTrucks(Vector3 startPosition, Vector3 basePosition)
        {
            this.startPosition = startPosition;
            this.basePosition = basePosition;
            Instantiate();
        }

        private void Instantiate()
        {
            startColshape = NAPI.ColShape.CreateCylinderColShape(startPosition, 1.0f, 3.0f);
            startColshape.SetSharedData("type", "towtruck");
            customMarkers.CreateJobMarker(startPosition, "Lawety");
            startBlip = NAPI.Blip.CreateBlip(67, startPosition, 0.8f, 69, name: "Praca: Lawety", shortRange: true);
        }

        public void addSpawningPosition(Vector3 spawningPosition, float heading)
        {
            startPositions.Add(spawningPosition, heading);
        }

        public void startJob(Player player)
        {
            if(player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                if (!player.GetSharedData<bool>("nodriving"))
                {
                    if (player.GetSharedData<bool>("licenceBp"))
                    {
                        if (player.GetSharedData<bool>("jobBonus_21"))
                        {
                            player.SetSharedData("job", "towtruck");
                            Vehicle veh = null;
                            if (player.GetSharedData<bool>("jobBonus_27"))
                            {
                                veh = vehicleDataManager.GetRandomVehicleToTow();
                            }
                            player.TriggerEvent("setVehicleToTow", veh);
                            player.TriggerEvent("startJob", "Lawety", "PS");
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie odblokowałeś tej pracy!");
                        }
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
                playerDataManager.NotifyPlayer(player, "Masz już inną pracę!");
            }
        }
    }
}

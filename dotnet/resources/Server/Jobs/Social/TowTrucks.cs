using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;

namespace ServerSide.Jobs
{
    public static class TowTrucks
    {
        static Vector3 startPosition;
        static ColShape startColshape;
        static Blip startBlip;
        public static Vector3 basePosition;
        static Dictionary<Vector3, float> startPositions = new Dictionary<Vector3, float>();
        public static void InstantiateTowTrucks(Vector3 startPosition, Vector3 basePosition)
        {
            startPosition = startPosition;
            basePosition = basePosition;
            startColshape = NAPI.ColShape.CreateCylinderColShape(startPosition, 1.0f, 3.0f);
            startColshape.SetSharedData("type", "towtruck");
            CustomMarkers.CreateJobMarker(startPosition, "Lawety");
            startBlip = NAPI.Blip.CreateBlip(67, startPosition, 0.8f, 69, name: "Praca: Lawety", shortRange: true);
        }

        public static void AddSpawningPosition(Vector3 spawningPosition, float heading)
        {
            startPositions.Add(spawningPosition, heading);
        }

        public static void StartJob(Player player)
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
                                veh = VehicleDataManager.GetRandomVehicleToTow();
                            }
                            player.TriggerEvent("setVehicleToTow", veh);
                            player.TriggerEvent("startJob", "Lawety", "PS");
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Nie odblokowałeś tej pracy!");
                        }
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Nie masz prawa jazdy");
                    }

                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie możesz prowadzić pojazdów do " + player.GetSharedData<string>("nodrivingto") + "!");
                }
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Masz już inną pracę!");
            }
        }
    }
}

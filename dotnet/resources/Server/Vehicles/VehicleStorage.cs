using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class VehicleStorage
    {
        private List<Vector3> spawningPoints = new List<Vector3>();
        private List<float> spawningPointsRotation = new List<float>();
        private VehicleDataManager vehicleDataManager = new VehicleDataManager();
        private Vector3 storagePosition;
        private Vector3 storageTakeout;
        private float storageRadius;
        private PlayerDataManager playerDataManager = new PlayerDataManager();
        public int storageId;
        public GTANetworkAPI.ColShape takeoutColShape;
        public GTANetworkAPI.ColShape storageColShape;
        CustomMarkers customMarkers = new CustomMarkers();
        public VehicleStorage(int storageId, Vector3 storagePosition, float storageRadius, Vector3 storageTakeout)
        {
            this.storageId = storageId;
            this.storagePosition = storagePosition;
            this.storageRadius = storageRadius;
            this.storageTakeout = storageTakeout;
            this.storagePosition.Z -= 1.5f;
            CreateColShapes();
        }

        public void AddSpawningPoint(Vector3 spawningPoint, float rotation)
        {
            spawningPoints.Add(spawningPoint);
            spawningPointsRotation.Add(rotation);
        }

        public void CreateColShapes()
        {
            takeoutColShape = NAPI.ColShape.CreateCylinderColShape(storageTakeout + new Vector3(0f, 0f, 0.3f), 1.0f, 2.0f);
            takeoutColShape.SetSharedData("type", "storage");
            takeoutColShape.SetSharedData("storageid", storageId);
            storageColShape = NAPI.ColShape.CreateCylinderColShape(storagePosition, storageRadius, 2.0f);
            storageColShape.SetSharedData("type", "storagein");
            customMarkers.CreateSimpleMarker(storageTakeout, "Odbierz pojazd");
            NAPI.Blip.CreateBlip(267, storageTakeout, 0.8f, 30, name: "Przechowalnia pojazdów", drawDistance: 300f, shortRange: true);
            // NAPI.Marker.CreateMarker(1, new Vector3(storagePosition.X, storagePosition.Y, storagePosition.Z - storageRadius*1.25), new Vector3(), new Vector3(), storageRadius * 1.6f, new Color(255, 255, 255));
        }

        public Vehicle SpawnCar(Player player, string vehiclesId)
        {
            Vehicle veh = null;
            Vector3 position = GetAvailableSpawnPoint();
            if (position != new Vector3())
            {
                veh = vehicleDataManager.CreatePersonalVehicle(Convert.ToInt32(vehiclesId), position, spawningPointsRotation[spawningPoints.IndexOf(position)], false);
                vehicleDataManager.UpdateVehicleSpawned(veh, true);
                veh.SetSharedData("storageTime", DateTime.Now.ToLongTimeString());
                playerDataManager.NotifyPlayer(player, "Pojazd wyciągnięty!");
                //RemoveSpawned(position, veh, player);
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Wszystkie miejsca są zajęte! Poczekaj chwilę!");
            }
            return veh;
        }

        private Vector3 GetAvailableSpawnPoint()
        {
            Vector3 v = new Vector3();
            foreach (Vector3 sp in spawningPoints)
            {
                if (!isAnyPersonalVehicleNearPoint(sp))
                {
                    return sp;
                }
            }
            return v;
        }

        //private void RemoveSpawned(Vector3 position, GTANetworkAPI.Vehicle vehicle, Player player)
        //{
        //    NAPI.Task.Run(() =>
        //    {
        //        if (vehicle != null && vehicle.Exists)
        //            if (vehicle.Position.DistanceTo(position) < 3.0f)
        //            {
        //                if (player.Exists)
        //                {
        //                    playerDataManager.NotifyPlayer(player, "Twój pojazd stał za długo na podjeździe i został przeniesiony do przechowalni!");
        //                }
        //                vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
        //                vehicle.Delete();
        //            }
        //    }, delayTime: 30000);
        //}

        private bool isAnyPersonalVehicleNearPoint(Vector3 point)
        {
            foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
            {
                if (veh.HasSharedData("type"))
                {
                    if (veh.GetSharedData<string>("type").Equals("personal") && veh.Position.DistanceTo(point) < 2.0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

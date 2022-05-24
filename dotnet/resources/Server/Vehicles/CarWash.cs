using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class CarWash
    {
        VehicleDataManager vehicleDataManager = new VehicleDataManager();
        public ColShape shape;
        Marker marker;
        Vector3 position, particlePosition;
        TextLabel label;
        public CarWash(Vector3 position, Vector3 particlePosition)
        {
            this.position = position;
            this.particlePosition = particlePosition;
            shape = NAPI.ColShape.CreateCylinderColShape(position - new Vector3(0,0,1), 2.0f, 2.0f);
            shape.SetSharedData("type", "carwash");
            marker = NAPI.Marker.CreateMarker(27, position - new Vector3(0, 0, 0.9), new Vector3(), new Vector3(), 3.0f, new Color(173, 216, 230));
            label = NAPI.TextLabel.CreateTextLabel("Myjnia samochodowa", position, 10.0f, 2.0f, 4, new Color(255, 255, 255));
            NAPI.Blip.CreateBlip(100, position, 0.8f, 75, name: "Myjnia samochodowa", shortRange: true);
        }

        public void WashCar(Player player, Vehicle vehicle)
        {
            vehicle.SetSharedData("wash", true);
            vehicle.SetSharedData("veh_brake", true);
            //anim = NAPI.Particle.CreateLoopedParticleEffectOnPosition("core", "veh_downwash", position - new Vector3(0, 0, 0.8), new Vector3(), 1.0f);
            player.TriggerEvent("stopParticles");
            player.TriggerEvent("startParticles", particlePosition);
            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run(() =>
            {
                NAPI.Task.Run(() =>
                {
                    if(vehicle.Exists)
                    {
                        player.TriggerEvent("stopParticles");
                        vehicle.SetSharedData("wash", false);
                        vehicle.SetSharedData("veh_brake", false);
                        vehicleDataManager.UpdateVehiclesWashTime(vehicle, DateTime.Now.AddDays(1).ToString());
                        vehicleDataManager.UpdateVehiclesDirtLevel(vehicle, 0);
                    }
                    //anim.Delete();
                }, delayTime: 10000);
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide
{
    public class PublicVehicleSpawn
    {
        private Vector3 position;
        private Blip blip;
        public ColShape colshape;
        private Marker marker;
        private float rotation;
        public Vehicle veh;
        MainClass mc;
        PlayerDataManager playerDataManager = new PlayerDataManager();
        VehicleDataManager vehicleDataManager = new VehicleDataManager();
        public DateTime leaveTime;

        public PublicVehicleSpawn(Vector3 position, float rotation, MainClass mc)
        {
            this.position = new Vector3(position.X, position.Y, position.Z - 1.0f);
            this.rotation = rotation;
            this.mc = mc;
            Instantiate();
        }
        private void Instantiate()
        {
            blip = NAPI.Blip.CreateBlip(661, new Vector3(position.X, position.Y, position.Z - 100), 0.8f, 39, name: "Pojazd publiczny", shortRange: true);
            colshape = NAPI.ColShape.CreateCylinderColShape(position, 1.0f, 2.0f);
            marker = NAPI.Marker.CreateMarker(27, position + new Vector3(0f,0f,0.1f), new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255));
            NAPI.TextLabel.CreateTextLabel("Pojazd publiczny", position + new Vector3(0f, 0f, 1.3f), 10f, 2.0f, 4, new Color(255, 255, 255));
            CreatePublicVehicle();
            
        }
        public void CreatePublicVehicle()
        {
            Random rnd = new Random();
            veh = NAPI.Vehicle.CreateVehicle(VehicleHash.Faggio, position - new Vector3(0,0,0.2f), rotation, rnd.Next(0, 160), 112, numberPlate: "BASICRPG");
            veh.SetSharedData("veh_brake", true);
            veh.SetSharedData("publicposition", position - new Vector3(0, 0, 0.2f));
            veh.SetSharedData("type", "public");
            veh.SetSharedData("speed", 65);
            veh.SetSharedData("Skuter publiczny", 65);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class JobVehicleSpawn
    {
        public Vehicle Veh { get; set; }
        public ColShape Col { get; set; }
        public Dictionary<string, System.Object> Data { get; set; }
        public string Type { get; set; }
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }
        public int Color { get; set; }
        public string Plate { get; set; }
        public VehicleHash Model { get; set; }

        public JobVehicleSpawn(string type, Vector3 position, float rotation, VehicleHash model, int color, string plate, Dictionary<string, System.Object> data)
        {
            Type = type;
            Position = new Vector3(position.X, position.Y, position.Z - 1);
            Rotation = rotation;
            Model = model;
            Color = color;
            Plate = plate;
            Data = data;

            Col = NAPI.ColShape.CreateCylinderColShape(Position, 1.2f, 4.0f);
            Col.SetSharedData("type", "jobVehSpawn");
            new CustomMarkers().CreateVehicleSpawnMarker(position);

            Veh = NAPI.Vehicle.CreateVehicle(model, Position, rotation, color, 0, numberPlate: plate);
            Veh.SetSharedData("spawnPos", Position);
            Veh.SetSharedData("type", "jobveh");
            Veh.SetSharedData("jobtype", type);
            Veh.SetSharedData("player", false);
            foreach(KeyValuePair<string, System.Object> obj in data)
            {
                Veh.SetSharedData(obj.Key, obj.Value);
            }
        }

        public void CreateNewVehicle()
        {
            Veh = NAPI.Vehicle.CreateVehicle(Model, Position, Rotation, Color, Color, numberPlate: Plate);
            Veh.SetSharedData("spawnPos", Position);
            Veh.SetSharedData("type", "jobveh");
            Veh.SetSharedData("jobtype", Type);
            Veh.SetSharedData("player", false);
            foreach (KeyValuePair<string, System.Object> obj in Data)
            {
                Veh.SetSharedData(obj.Key, obj.Value);
            }
        }
    }
}

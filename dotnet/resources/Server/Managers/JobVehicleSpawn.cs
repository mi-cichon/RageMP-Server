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
        public JobVehicle Data { get; set; }
        public string Type { get; set; }
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }
        public int Color { get; set; }
        public string Plate { get; set; }
        public VehicleHash Model { get; set; }
        public int[] Trunk { get; set; }
        public JobVehicleSpawn(string type, Vector3 position, float rotation, VehicleHash model, int color, string plate, JobVehicle jobVehData, bool oiltanker = false)
        {
            Type = type;
            Position = new Vector3(position.X, position.Y, position.Z - 1);
            Rotation = rotation;
            Model = model;
            Color = color;
            Plate = plate;
            Data = jobVehData;

            Col = NAPI.ColShape.CreateCylinderColShape(Position, 1.2f, 4.0f);
            Col.SetSharedData("type", "jobVehSpawn");
            new CustomMarkers().CreateVehicleSpawnMarker(position);

            CreateNewVehicle(oiltanker);
        }

        public void CreateNewVehicle(bool oiltanker = false)
        {
            Veh = NAPI.Vehicle.CreateVehicle(Model, Position, Rotation, Color, Color, numberPlate: Plate);
            Veh.SetSharedData("spawnPos", Position);
            Veh.SetSharedData("type", "jobveh");
            Veh.SetSharedData("jobtype", Type);
            Veh.SetSharedData("player", false);
            
            if(Data.Petrol != 0)
            {
                Veh.SetSharedData("petrol", Data.Petrol);
            }
            if (Data.Tank != 0)
            {
                Veh.SetSharedData("petroltank", Data.Tank);
            }
            if (Data.Combustion != 0)
            {
                Veh.SetSharedData("combustion", Data.Combustion);
            }
            if (Data.Trunk != "")
            {
                Veh.SetSharedData("trunk", Data.Trunk);
            }
            if (Data.Damage != "")
            {
                Veh.SetSharedData("damage", Data.Damage);
            }

            Veh.SetSharedData("offroad", Data.Offroad);
            Veh.SetSharedData("name", Data.Name);
            Veh.SetSharedData("speed", Data.MaxSpeed);
            Veh.SetSharedData("power", Data.Power);
            Veh.SetSharedData("veh_brake", Data.Brake);

            if(oiltanker)
            {
                Veh.SetSharedData("oiltank", 0.0f);
            }
        }
    }
}

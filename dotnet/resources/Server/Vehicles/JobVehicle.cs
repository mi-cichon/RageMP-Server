using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class JobVehicle
    {
        public VehicleHash Model { get; set; }
        public string JobType { get; set; }
        public int Petrol { get; set; }
        public int Tank { get; set; }
        public int Combustion { get; set; }
        public bool Offroad { get; set; }
        public string Name { get; set; }
        public int MaxSpeed { get; set; }
        public int Power { get; set; }
        public bool Brake { get; set; }
        public string Trunk { get; set; }
        public string Damage { get; set; }
        public string Plate { get; set; }
        public int Color { get; set; }

        public JobVehicle(VehicleHash model, string jobType, int petrol, int tank, int combustion, bool offroad, string name, int maxspeed, int power, bool brake, string trunk, string damage, string plate, int color)
        {
            Model = model;
            JobType = jobType;
            Petrol = petrol;
            Tank = tank;
            Combustion = combustion;
            Offroad = offroad;
            Name = name;
            MaxSpeed = maxspeed;
            Power = power;
            Brake = brake;
            Trunk = trunk;
            Damage = damage;
            Plate = plate;
            Color = color;
        }

        public Vehicle CreateVehicle(Vector3 Position, string trunk)
        {
            if(trunk != "")
            {
                Trunk = trunk;
            }

            Vehicle Veh = NAPI.Vehicle.CreateVehicle(Model, Position, new Vector3(), Color, Color, numberPlate: Plate);
            Veh.SetSharedData("spawnPos", Position);
            Veh.SetSharedData("type", "jobveh");
            Veh.SetSharedData("jobtype", JobType);
            Veh.SetSharedData("player", false);

            if (Petrol != 0)
            {
                Veh.SetSharedData("petrol", Petrol);
            }
            if (Tank != 0)
            {
                Veh.SetSharedData("petroltank", Tank);
            }
            if (Combustion != 0)
            {
                Veh.SetSharedData("combustion", Combustion);
            }
            if (Trunk != "")
            {
                Veh.SetSharedData("trunk", Trunk);
            }
            if (Damage != "")
            {
                Veh.SetSharedData("damage", Damage);
            }

            Veh.SetSharedData("offroad", Offroad);
            Veh.SetSharedData("name", Name);
            Veh.SetSharedData("speed", MaxSpeed);
            Veh.SetSharedData("power", Power);
            Veh.SetSharedData("veh_brake", Brake);

            if(JobType == "refinery")
            {
                Veh.SetSharedData("oiltank", 0.0f);
            }
            return Veh;
        }
    }
}

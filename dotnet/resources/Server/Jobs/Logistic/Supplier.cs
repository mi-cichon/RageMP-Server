using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;

namespace ServerSide.Jobs
{
    public static class Supplier
    {
        static Dictionary<Player, Vehicle> trailers = new Dictionary<Player, Vehicle>();
        public static List<Vector3> spawnPositions = new List<Vector3>()
        {
            new Vector3(78.964554f, 6338.7256f, 31.225761f),
            new Vector3(83.72409f, 6329.917f, 31.227898f),
            new Vector3(86.26046f, 6324.3315f, 31.236996f)
    };

        public static Dictionary<Vector3, float> trailerPositions = new Dictionary<Vector3, float>()
        {
            [new Vector3(-2959.1506f, 60.139057f, 11.608494f)] = 65.62127f,
            [new Vector3(-3184.9214f, 1091.1962f, 20.845089f)] = -25.375887f,
            [new Vector3(-2523.7595f, 2343.0278f, 33.049828f)] = -148.55608f,
            [new Vector3(-2530.657f, 2342.0596f, 33.059837f)] = -146.53354f,
            [new Vector3(-1897.0753f, 1993.8054f, 141.95158f)] = 8.503107f,
            [new Vector3(-555.96375f, 301.76285f, 84.92008f)] = -93.317215f,
            [new Vector3(368.40552f, -75.21446f, 68.9222f)] = -109.49181f,
            [new Vector3(777.21735f, 218.86203f, 86.7906f)] = -119.53002f,
            [new Vector3(781.71216f, 224.64572f, 86.65199f)] = -121.145775f,
            [new Vector3(744.5335f, 134.01292f, 81.72799f)] = -121.00253f,
            [new Vector3(742.06396f, 126.696365f, 81.42685f)] = -122.68131f,
            [new Vector3(952.4513f, -124.55317f, 75.92671f)] = -144.64278f,
            [new Vector3(977.36053f, -142.92206f, 75.863815f)] = 49.354984f
        };

        public static Dictionary<Vector3, float> basePositions = new Dictionary<Vector3, float>()
        {
            [new Vector3(498.6088f, -1972.8278f, 24.907175f)] = 121.329956f,
            [new Vector3(509.23904f, -2002.9613f, 24.753252f)] = 123.008705f,
            [new Vector3(538.9685f, -1984.3318f, 24.751003f)] = -59.95388f,
            [new Vector3(285.08334f, 2827.1672f, 43.41496f)] = -60.274734f,
            [new Vector3(315.96237f, 2829.2346f, 43.43604f)] = 9.574545f,
            [new Vector3(327.8657f, 2872.6472f, 43.45387f)] = 168.1005f,
            [new Vector3(2784.611f, 1619.7511f, 24.500694f)] = -0.41784364f,
            [new Vector3(2670.1423f, 1427.633f, 24.500786f)] = -3.3341978f,
            [new Vector3(2777.5845f, 1395.8726f, 24.452044f)] = -0.65276116f
        };

        public static List<VehicleHash> availableTrailers = new List<VehicleHash>() 
        {
            // VehicleHash.Trailers,
            // VehicleHash.Trailers2,
            // VehicleHash.Trailers3,
            // VehicleHash.Trailers4,
            VehicleHash.Trailerlogs,
            VehicleHash.Tanker
            // VehicleHash.Tvtrailer
        };

        static Vector3 startPosition;
        public static ColShape supplyColshape;
        public static Blip supplyBlip;
        public static void InstantiateSupplier(Vector3 startPos)
        {
            startPosition = startPos;
            supplyColshape = NAPI.ColShape.CreateCylinderColShape(startPosition, 1.0f, 2.0f);
            supplyColshape.SetSharedData("type", "supplier");
            supplyBlip = NAPI.Blip.CreateBlip(477, startPosition, 1.0f, 44, name: "Dostawca", shortRange: true);
            CustomMarkers.CreateJobMarker(startPosition, "Dostawca");
        }

        public static void startJob(Player player)
        {
            KeyValuePair<Vector3, float> spawnPoint = GetAvailableSpawnPoint(spawnPositions);

            if (!player.GetSharedData<bool>("nodriving"))
            {
                if (player.GetSharedData<Int32>("logisticpoints") >= 750)
                {
                    if(player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
                    {
                        if(spawnPoint.Key != new Vector3())
                        {
                            CreateJobVehicle(player, spawnPoint.Key, spawnPoint.Value);
                            player.SetSharedData("job", "supplier");
                            player.TriggerEvent("startJob", "Dostawca", "PL");
                            CreateTrailer(player);
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Nie ma wolnych miejsc na parkingu!");
                        }                    
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Masz inną pracę!");
                    }
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie posiadasz wystarczająco LP: 250!");
                }
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie możesz prowadzić pojazdów do " + player.GetSharedData<string>("nodrivingto") + "!");
            }
        }

        public static void CreateJobVehicle(Player player, Vector3 position, float rotation)
        {
            Random rnd = new Random();
            Vehicle jobVeh = NAPI.Vehicle.CreateVehicle(VehicleHash.Packer, position, rotation, rnd.Next(0, 160), 112, numberPlate: "DOSTAWCA");
            jobVeh.Rotation = new Vector3(0f, 0f, -65f);
            jobVeh.SetSharedData("type", "jobveh");
            jobVeh.SetSharedData("invincible", true);
            player.SetSharedData("jobveh", jobVeh.Id);
            player.SetIntoVehicle(jobVeh, 0);
        }

        public static void CreateTrailer(Player player)
        {
            Random rnd = new Random();
            KeyValuePair<Vector3, float> position;
            bool done = false;
            do
            {
                position = trailerPositions.ElementAt(rnd.Next(0, trailerPositions.Count));
                done = !isAnyVehicleNearPoint(position.Key);
            } while (!done);
            Vehicle trailer = NAPI.Vehicle.CreateVehicle(availableTrailers[rnd.Next(0, availableTrailers.Count)], position.Key, position.Value, 41, 41, numberPlate: "Dostawca");
            trailer.SetSharedData("type", "trailer");
            trailer.Rotation = new Vector3(0, 0, position.Value);
            if(trailers.ContainsKey(player))
            {
                trailers[player] = trailer;
            }
            else
            {
                trailers.Add(player, trailer);
            }
            KeyValuePair<Vector3, float> basePosition = basePositions.ElementAt(rnd.Next(0, basePositions.Count));
            player.TriggerEvent("markNewTrailer", trailer, trailer.Position, basePosition.Key);
        }

        private static KeyValuePair<Vector3, float> GetAvailableSpawnPoint(object type)
        {
            Vector3 pos = new Vector3();
            float rot = 0;
            if(type is Dictionary<Vector3, float>)
            {
                foreach(KeyValuePair<Vector3, float> pair in type as Dictionary<Vector3, float>)
                {
                    if (!isAnyVehicleNearPoint(pair.Key))
                    {
                        return pair;
                    }
                }
            }
            else if(type is List<Vector3>)
            {
                foreach (Vector3 p in type as List<Vector3>)
                {
                    if (!isAnyVehicleNearPoint(p))
                    {
                        pos = p;
                        rot = 110f;
                        break;
                    }
                }
            }

            return new KeyValuePair<Vector3, float>(pos, rot);
        }
        private static bool isAnyVehicleNearPoint(Vector3 point)
        {
            foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
            {
                if (veh.Position.DistanceTo(point) < 3.0f)
                {
                    return true;
                }
            }
            return false;
        }

    }
}

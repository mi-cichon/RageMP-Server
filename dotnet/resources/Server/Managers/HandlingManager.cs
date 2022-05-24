using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    public class HandlingManager
    {
        public HandlingManager() { }


        public bool IsCarOffroadType(Vehicle vehicle)
        {
            if (vehicle.HasSharedData("wheels"))
            {
                int[] wheels = JsonConvert.DeserializeObject<int[]>(vehicle.GetSharedData<string>("wheels"));
                if (wheels[0] == 4 && wheels[1] != -1)
                {
                    return true;
                }
                if(StockOffroadVehicles.Contains(vehicle.Model) && wheels[1] == -1)
                {
                    return true;
                }
                if (OffroadMotorcycles.Contains(vehicle.Model))
                {
                    return true;
                }
            }
            return false;
        }




        readonly List<ulong> StockOffroadVehicles = new List<ulong>()
        {
            2166734073,
            3025077634,
            298565713,
            67753863,
            408825843,
            2538945576,
            740289177,
            3932816511,
            2945871676,
            4240635011,
            3105951696,
            989381445,
            4173521127,
            2633113103,
            2762269779,
            101905590,
            1126868326,
            3057713523,
            3631668194,
            4192631813,
            2484160806,
            2230595153,
            2815302597,
            3449006043,
            3087195462,
            2166734073,
            3624880708
        };
        readonly List<ulong> OffroadMotorcycles = new List<ulong>()
        {
            86520421,
            1753414259,
            788045382,
            390201602,
            2771538552,
            1086534307  
        };
    }
}

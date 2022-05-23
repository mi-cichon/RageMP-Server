using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("carMugshot_create")]
        public void CarMugshot_create(Player player, string part1, string part2, string part3, string part4)
        {
            List<string> base64 = JsonConvert.DeserializeObject<List<string>>(part1 + part2 + part3 + part4);
            for(int i=0; i < base64.Count; i++)
            {
                base64[i] = base64[i].Replace("data:image/png;base64,", "");
            }
            playerDataManager.CreateCarMugshot(base64);
        }
    }
}

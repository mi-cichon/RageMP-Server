using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class Warehouse
    {
        PlayerDataManager playerDataManager = new PlayerDataManager();
        CustomMarkers customMarkers = new CustomMarkers();
        public List<GTANetworkAPI.Object> boxes = new List<GTANetworkAPI.Object>();
        public List<Vector3> boxPositions = new List<Vector3>()
        {
            new Vector3(-81.73724, 6499.287, 31.24549),
            new Vector3(-82.11272, 6498.912, 31.24549),
            new Vector3(-82.49572, 6498.529, 31.24549),
            new Vector3(-82.87618, 6498.148, 31.24549),
            new Vector3(-83.25713, 6497.767, 31.24549),

            new Vector3(-81.73724, 6499.287, 31.5718),
            new Vector3(-82.11272, 6498.912, 31.5718),
            new Vector3(-82.49572, 6498.529, 31.5718),
            new Vector3(-82.87618, 6498.148, 31.5718),
            new Vector3(-83.25713, 6497.767, 31.5718),

            new Vector3(-81.73724, 6499.287, 31.88133),
            new Vector3(-82.11272, 6498.912, 31.88133),
            new Vector3(-82.49572, 6498.529, 31.88133),
            new Vector3(-82.87618, 6498.148, 31.88133),
            new Vector3(-83.25713, 6497.767, 31.88133),


            new Vector3(-84.45409, 6496.223, 31.24549),
            new Vector3(-84.5462, 6495.699, 31.24549),
            new Vector3(-84.64043, 6495.166, 31.24549),
            new Vector3(-84.73373, 6494.636, 31.24549),
            new Vector3(-84.82726, 6494.105, 31.24549),

            new Vector3(-84.45409, 6496.223, 31.5718),
            new Vector3(-84.5462, 6495.699, 31.5718),
            new Vector3(-84.64043, 6495.166, 31.5718),
            new Vector3(-84.73373, 6494.636, 31.5718),
            new Vector3(-84.82726, 6494.105, 31.5718),

            new Vector3(-84.45409, 6496.223, 31.88133),
            new Vector3(-84.5462, 6495.699, 31.88133),
            new Vector3(-84.64043, 6495.166, 31.88133),
            new Vector3(-84.73373, 6494.636, 31.88133),
            new Vector3(-84.82726, 6494.105, 31.88133)
        };

        Vector3 firstRotation = new Vector3(0, 0, 45);
        Vector3 secondRotation = new Vector3(0, 0, 80);

        public Warehouse(Vector3 startPosition)
        {
            NAPI.Blip.CreateBlip(478, startPosition, 0.8f, 69, name: "Praca: Magazynier", shortRange: true);
            ColShape warehouseColshape = NAPI.ColShape.CreateCylinderColShape(startPosition, 1.0f, 2.0f);
            warehouseColshape.SetSharedData("type", "warehouse");
            customMarkers.CreateJobMarker(startPosition, "Magazynier");
        }

        public void startJob(Player player)
        {
            if (player.GetSharedData<string>("job") == "" && !(player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty")))
            {
                player.SetSharedData("job", "warehouse");
                player.TriggerEvent("startJob", "Magazynier", "PL");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Masz inną pracę!");
            }
        }

        public void CreateBox()
        {
            if(boxes.Count == 29)
            {
                List<Player> players = new List<Player>();
                foreach(Player player in NAPI.Pools.GetAllPlayers())
                {
                    if(player.GetSharedData<string>("job") == "warehouse")
                    {
                        players.Add(player);
                    }
                }
                int payment = 100 / players.Count;
                foreach(Player player in players)
                {
                    playerDataManager.NotifyPlayer(player, $"Otrzymujesz {payment}$ za pracę zespołową!");
                    playerDataManager.UpdatePlayersMoney(player, payment);
                    player.TriggerEvent("updateJobVars", payment, 0, 0);
                }

                foreach(GTANetworkAPI.Object box in boxes)
                {
                    box.Delete();
                }
                boxes = new List<GTANetworkAPI.Object>();
            }
            else
            {
                boxes.Add(NAPI.Object.CreateObject(1302435108, boxPositions[boxes.Count], boxes.Count < 15 ? firstRotation : secondRotation));
            }
        }
    }
}

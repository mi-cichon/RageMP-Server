using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("openClothes")]
        public void OpenClothes(Player player, float heading)
        {
            player.Dimension = (uint)1000 + player.Id;
            player.Heading = heading;
            player.SetSharedData("gui", true);
            player.TriggerEvent("client:clothes.show");
        }

        [RemoteEvent("previewClothes")]
        public void PreviewClothes(Player player, int type, int cloth, int variant)
        {
            player.SetClothes(type, cloth, variant);
        }

        [RemoteEvent("saveClothes")]
        public void SaveClothes(Player player)
        {
            if (playerDataManager.UpdatePlayersMoney(player, -1000))
            {
                playerDataManager.UpdatePlayersClothes(player);
                playerDataManager.NotifyPlayer(player, "Ubrania zostały pomyślnie zmienione!");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie stać Cię na to!");
                playerDataManager.LoadPlayersClothes(player);
            }
            player.Dimension = 0;
            player.SetSharedData("gui", false);

        }

        [RemoteEvent("loadClothes")]
        public void LoadClothes(Player player)
        {
            playerDataManager.LoadPlayersClothes(player);
        }

        [RemoteEvent("loadPlayersClothes")]
        public void LoadPlayersClothes(Player player, Player player2)
        {
            playerDataManager.LoadPlayersClothes(player2);
        }
        public void CreateChangingRooms()
        {
            //ls
            changingRooms.Add(new ChangingRoom(new Vector3(71.18146f, -1399.4702f, 29.376146f), -52.9638f));

            //sandy
            changingRooms.Add(new ChangingRoom(new Vector3(1190.1489f, 2714.4536f, 38.222637f), -139.84138f));

            //paleto
            changingRooms.Add(new ChangingRoom(new Vector3(12.768764f, 6513.704f, 31.87785f), 66.228615f));
        }
    }
}

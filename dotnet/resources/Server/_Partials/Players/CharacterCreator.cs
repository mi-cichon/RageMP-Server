using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {

        [RemoteEvent("createCharacter")]
        public void CreateCharacter(Player player, string charStr)
        {
            charStr = charStr.Remove(0, 1);
            charStr = charStr.Remove(charStr.Length - 1, 1);
            charStr.Replace(@"\", "");
            PlayerDataManager.UpdatePlayersCharacter(player, charStr);
            player.Dimension = 0;
            SpawnSelected(player, "ls");
            player.SetSharedData("gui", false);
            PlayerDataManager.SetPlayersClothes(player);
        }

    }
}

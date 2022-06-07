using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {

        [RemoteEvent("changeNickname")]
        public void ChangeNickname(Player player, string nickname)
        {
            if (PlayerDataManager.UpdatePlayersNickname(player, nickname))
            {
                player.TriggerEvent("closeNicknameBrowser");
                PlayerDataManager.NotifyPlayer(player, "Pomyślnie zmieniono nick!");
            }
            else
            {
                player.TriggerEvent("callNicknameError", "Nick nie spełnia wymagań, jest zajęty, lub nie stać Cię na jego zmianę!");
            }
        }

        [RemoteEvent("openNicknameBrowser")]
        public void OpenNicknameBrowser(Player player)
        {
            player.TriggerEvent("openNicknameBrowser", player.GetSharedData<string>("username") == player.SocialClubName);
        }

    }
}

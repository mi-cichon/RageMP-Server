using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("messenger_requestConversationsData")]
        public void Messenger_requestConversationsData(Player player)
        {
            string conversations = PlayerDataManager.GetPlayersConversations(player);
            player.TriggerEvent("messenger_receiveConversationsData", conversations);
        }

        [RemoteEvent("messenger_requestMessageData")]
        public void Messenger_requestMessageData(Player player, string playerId)
        {
            string messages = PlayerDataManager.GetPlayersMessages(player, playerId);
            player.TriggerEvent("messenger_receiveMessageData", messages);
        }

        [RemoteEvent("messenger_sendMessage")]
        public void Messenger_sendMessage(Player player, string playerId, string message)
        {
            PlayerDataManager.SendMessageToPlayer(player, playerId, message);
            string messages = PlayerDataManager.GetPlayersMessages(player, playerId);
            player.TriggerEvent("messenger_receiveMessageData", messages);
        }

        [RemoteEvent("messenger_checkNewMessages")]
        public void Messenger_checkNewMessages(Player player)
        {
            string messagesId = PlayerDataManager.HasPlayerNewMessages(player);
            if (messagesId != "")
            {
                player.TriggerEvent("newMessageSound", messagesId);
            }
            player.TriggerEvent("messages_setNewMessages", messagesId == "" ? 0 : JsonConvert.DeserializeObject<List<int>>(messagesId).Count);
        }

        [RemoteEvent("messenger_searchForPlayers")]
        public void Messenger_searchForPlayers(Player player, string keyword)
        {
            string players = PlayerDataManager.SearchForPlayers(player, keyword);
            if(players != "")
            {
                player.TriggerEvent("messenger_receiveSearchedPlayers", players);
            }
        }
    }
}

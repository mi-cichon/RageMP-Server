using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {

        [RemoteEvent("setPassengersWaypoint")]
        public void SetPlayersWaypoint(Player player, Vehicle vehicle, float x, float y, float z)
        {
            foreach (Player pl in vehicle.Occupants)
            {
                if (pl != player)
                {
                    pl.TriggerEvent("setWaypoint", x, y);
                    playerDataManager.NotifyPlayer(pl, player.GetSharedData<string>("username") + " ustawił Ci waypoint!");
                }
            }
        }


        [RemoteEvent("sendInfoMessage")]
        public void SendInfoMessage(Player player, string message)
        {
            playerDataManager.SendInfoToPlayer(player, message);
        }

        [RemoteEvent("setGui")]
        public void SetGui(Player player, bool state)
        {
            player.SetSharedData("gui", state);
        }

        //SPAWN SELECTION

        [RemoteEvent("spawnSelected")]
        public void SpawnSelected(Player player, string spawn)
        {
            Vector3 paleto = new Vector3(-122.91341f, 6389.752f, 32.17763f);
            Vector3 sandy = new Vector3(1894.2115f, 3715.0637f, 32.762226f);
            Vector3 ls = new Vector3(109.02803f, -1088.5417f, 29.30091f);

            float paletoHeading = 42f;
            float lsHeading = -20f;
            float sandyHeading = 125f;
            switch (spawn)
            {
                case "paleto":
                    player.Position = paleto;
                    player.Heading = paletoHeading;
                    break;
                case "ls":
                    player.Position = ls;
                    player.Heading = lsHeading;
                    break;
                case "sandy":
                    player.Position = sandy;
                    player.Heading = sandyHeading;
                    break;
                case "last":
                    player.Position = player.GetSharedData<Vector3>("lastpos");
                    player.Heading = 0f;
                    break;
                case "house":
                    player.Position = player.GetSharedData<Vector3>("housepos");
                    player.Heading = 0f;
                    break;
            }
            playerDataManager.SetPlayersConnectValues(player, currentWeather);
            progressManager.SetPlayersJobBonuses(player);
            if (spawn == "last")
            {
                autoSave.LoadPlayersJob(player);
            }
            else
            {
                autoSave.RemovePlayersJobData(player);
            }
        }

        //VOICE CHAT
        [RemoteEvent("add_voice_listener")]
        public void AddVoiceListener(Player player, Player target)
        {
            player.EnableVoiceTo(target);
        }

        [RemoteEvent("remove_voice_listener")]
        public void RemoveVoiceListener(Player player, Player target)
        {
            player.DisableVoiceTo(target);
        }

        [RemoteEvent("setVoiceChat")]
        public void setVoiceChat(Player player, bool state)
        {
            player.SetSharedData("voicechat", state);
        }


        [RemoteEvent("setPlayerTexting")]
        public void SetPlayerTexting(Player player, bool state)
        {
            player.SetSharedData("texting", state);
        }

        [RemoteEvent("playerCommandHandler")]
        public void PlayerCommandHandler(Player player, string command, string args)
        {
            commands.ExecuteCommand(player, command, args);
        }

        //DOOR MANAGER

        [RemoteEvent("door_switch")]
        public void Door_Switch(Player player, int doorId, bool notify)
        {
            foreach (Door door in doorManager.Doors)
            {
                if (door.Id == doorId)
                {
                    door.Locked = !door.Locked;
                    if (notify)
                    {
                        playerDataManager.NotifyPlayer(player, door.Locked ? "Zamknąłeś drzwi!" : "Otworzyłeś drzwi!");
                    }
                    NAPI.ClientEvent.TriggerClientEventForAll("door_state", door.Hash, door.Position.X, door.Position.Y, door.Position.Z, door.Locked);
                    break;
                }
            }
        }

        [RemoteEvent("setPlayersDimension")]
        public void SetPlayersDimension(Player player, int dimension)
        {
            player.Dimension = Convert.ToUInt32(dimension);
        }


        [RemoteEvent("setPlayersControlsBlocked")]
        public void SetPlayersControlsBlocked(Player player, bool value)
        {
            player.SetSharedData("controlsblocked", value);
        }

        [RemoteEvent("playerInfoHandler")]
        public void PlayerInfoHandler(Player player, string message)
        {
            playerDataManager.SendMessageToPlayer(player, message);
        }

        [RemoteEvent("playerMessageHandler")]
        public void PlayerMessageHandler(Player player, string message)
        {
            if (!player.GetSharedData<bool>("muted"))
                playerDataManager.SendMessageToNearPlayers(player, message, 50);
            else
                playerDataManager.NotifyPlayer(player, "Jesteś wyciszony do " + player.GetSharedData<string>("mutedto"));
        }
    }
}

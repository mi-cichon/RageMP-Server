using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft;

namespace ServerSide
{
    public class Karting
    {
        PlayerDataManager playerDataManager = new PlayerDataManager();
        List<Player> queue = new List<Player>();
        Blip kartingBlip;
        ColShape kartingShape;
        Vector3 position, scorePosition, startPosition, cpPosition;
        List<KeyValuePair<string, string>> times = new List<KeyValuePair<string, string>>();
        List<GTANetworkAPI.TextLabel> labels = new List<TextLabel>();
        float startRotation;
        CustomMarkers customMarkers = new CustomMarkers();
        public Karting(Vector3 position, Vector3 scorePosition, Vector3 startPosition, float startRotation, Vector3 cpPosition)
        {
            this.position = new Vector3(position.X, position.Y, position.Z - 1.0f);
            this.scorePosition = scorePosition;
            this.startPosition = startPosition;
            this.cpPosition = cpPosition;
            this.startRotation = startRotation;

            kartingBlip = NAPI.Blip.CreateBlip(611, this.position, 1.0f, 4, name: "Karting League", shortRange: true);
            kartingShape = NAPI.ColShape.CreateCylinderColShape(this.position, 2.0f, 2.0f);
            kartingShape.SetSharedData("type", "karting");
            InitializeTimes(true);
            customMarkers.CreateSimpleMarker(position, "Karting - rozpocznij");

        }

        public void SignForARace(Player player)
        {
            if(player.Exists)
            {
                queue.Add(player);
            }
        }

        public void ManageQueue()
        {
            if(queue.Count > 0 && queue[0].Exists)
            {
                if (Vector3.Distance(position, queue[0].Position) < 10.0f)
                {
                    StartRace(queue[0]);
                    queue.RemoveAt(0);
                }
                else
                {
                    queue.RemoveAt(0);
                }
            }
        }

        public void StartRace(Player player)
        {
            playerDataManager.NotifyPlayer(player, "Rozpoczęto wyścig!");
            Vehicle kart = NAPI.Vehicle.CreateVehicle(2802050217, startPosition, startRotation, 55, 131);
            kart.SetSharedData("type", "race");
            kart.SetSharedData("collision", false);
            NAPI.Player.SetPlayerIntoVehicle(player, kart.Handle, 0);
            kart.Rotation = new Vector3(0, 0, startRotation);
            player.TriggerEvent("startKartingRace", startPosition, cpPosition);
        }

        public void StopRace(Player player)
        {
            player.Position = position;
        }

        public void InitializeTimes(bool createList = false)
        {
            times = playerDataManager.GetRacingTimes("karting");

            if(createList)
            {
                CreateRacingList();
            }
        }

        public void CheckPosition(Player player, float time)
        {
            if(times.Count == 0)
            {
                times.Add(new KeyValuePair<string, string>(player.SocialClubId.ToString(), time.ToString()));
                playerDataManager.UpdateRacingTimes("karting", times);
                RefreshRacingList();
            }
            else
            {
                foreach(KeyValuePair<string, string> timepair in times)
                {
                    if(timepair.Key == player.SocialClubId.ToString() && float.Parse(timepair.Value) > time)
                    {
                        times.Remove(timepair);
                        CheckPosition(player, time);
                        break;
                    }
                    else
                    {
                        if(timepair.Key == player.SocialClubId.ToString() && float.Parse(timepair.Value) <= time)
                        {
                            return;
                        }
                    }
                }
                foreach(KeyValuePair<string, string> timepair in times)
                {
                    if(float.Parse(timepair.Value) > time)
                    {
                        times.Insert(times.IndexOf(timepair), new KeyValuePair<string, string>(player.SocialClubId.ToString(), time.ToString()));
                        playerDataManager.UpdateRacingTimes("karting", times);
                        RefreshRacingList();
                        break;
                    }
                    else if(times.IndexOf(timepair) == times.Count-1 && times.Count < 10)
                    {
                        times.Add(new KeyValuePair<string, string>(player.SocialClubId.ToString(), time.ToString()));
                        playerDataManager.UpdateRacingTimes("karting", times);
                        RefreshRacingList();
                    }
                }
            }
        }
        public void CreateRacingList()
        {
            labels.Add(NAPI.TextLabel.CreateTextLabel($"Karting - TOP 10", scorePosition + new Vector3(0, 0, 3.0), 8.0f, 2.0f, 4, new Color(255, 255, 255)));
            foreach (KeyValuePair<string, string> time in times)
            {
                labels.Add(NAPI.TextLabel.CreateTextLabel($"{times.IndexOf(time) + 1}: {playerDataManager.GetPlayerNameById(time.Key)} - {time.Value}s", scorePosition + new Vector3(0,0, 3-(0.2 * (times.IndexOf(time) + 1))), 8.0f, 2.0f, 4, new Color(255, 255, 255)));
            }
        }

        public void RefreshRacingList()
        {
            foreach (TextLabel label in labels)
            {
                label.Delete();
            }
            labels = new List<TextLabel>();
            CreateRacingList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;
using ServerSide.Jobs;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("startHunter")]
        public void StartHunter(Player player)
        {
            Hunter.startJob(player);
        }

        [RemoteEvent("removeHunterVehicle")]
        public void RemoveHunterVeh(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                vehicle.Delete();
            }
            if (player != null && player.Exists)
                player.RemoveAllWeapons();
        }

        [RemoteEvent("endHunterJob")]
        public void EndHunterJob(Player player)
        {
            if (player != null)
            {
                player.SetSharedData("job", "");
                if (player.HasSharedData("jobveh"))
                {
                    var veh = VehicleDataManager.GetVehicleByRemoteId(Convert.ToUInt16(player.GetSharedData<Int32>("jobveh")));
                    if (veh != null && veh.Exists)
                        veh.Delete();
                    player.SetSharedData("jobveh", -1111);
                }

            }
        }
        [RemoteEvent("animalHunted")]
        public void AnimalHunted(Player player, string pedstr)
        {
            PayoutManager.HunterPaymentAnimal(player, pedstr);
            Hunter.GetRandomAnimalAndSendToPlayer(player);
        }

        [RemoteEvent("takeHunterPelt")]
        public void TakeHunterPelt(Player player, string pedstr)
        {
            switch (pedstr)
            {
                case "1682622302":
                    player.TriggerEvent("fitItemInEquipment", 8);
                    break;
                case "3462393972":
                    player.TriggerEvent("fitItemInEquipment", 7);
                    break;
                case "3753204865":
                    player.TriggerEvent("fitItemInEquipment", 5);
                    break;
                case "1641334641":
                    player.TriggerEvent("fitItemInEquipment", 10);
                    break;
                case "307287994":
                    player.TriggerEvent("fitItemInEquipment", 9);
                    break;
                case "3630914197":
                    player.TriggerEvent("fitItemInEquipment", 6);
                    break;
            }
        }
        [RemoteEvent("sellPelts")]
        public void SellPelts(Player player)
        {
            List<string> pelts = new List<string>();
            string eq = player.GetSharedData<string>("equipment");
            List<Dictionary<string, string>> items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(eq);
            foreach (Dictionary<string, string> item in items)
            {
                switch (Int32.Parse(item["typeID"]))
                {
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        pelts.Add(item["typeID"]);
                        player.TriggerEvent("removeEqItem", Int32.Parse(item["id"]));
                        break;
                }
            }
            NAPI.Task.Run(() =>
            {
                if (player.Exists)
                {
                    player.TriggerEvent("refreshEquipment", player.GetSharedData<string>("equipment"));
                }
            }, 1000);
            if (pelts.Count == 0)
            {
                PlayerDataManager.NotifyPlayer(player, "Nie masz żadnych skór do sprzedania!");
            }
            else
            {
                PayoutManager.HunterPaymentPelts(player, pelts);
            }
        }
    }
}

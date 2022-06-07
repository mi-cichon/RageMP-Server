using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("updateEquipment")]
        public void UpdateEquipment(Player player, string equipmentString)
        {
            PlayerDataManager.UpdatePlayersEquipment(player, equipmentString);
        }

        [RemoteEvent("useItem")]
        public void UseItem(Player player, int typeId, int itemId)
        {
            switch (typeId)
            {
                case 0:
                case 1:
                    PlayerDataManager.NotifyPlayer(player, "Zostałeś uleczony!");
                    player.TriggerEvent("removeEqItem", itemId);
                    int newHealt = Math.Clamp(player.Health + 50, 50, 100);
                    player.Health = newHealt;
                    break;
                case 2:
                    PlayerDataManager.NotifyPlayer(player, "Zostałeś uleczony!");
                    player.TriggerEvent("removeEqItem", itemId);
                    int newH = Math.Clamp(player.Health + 25, 25, 100);
                    player.Health = newH;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    PlayerDataManager.NotifyPlayer(player, "Skóry możesz sprzedać u myśliwego!");
                    break;
                case 11:
                    if (player.Vehicle == null)
                    {
                        Vehicle closest = VehicleDataManager.GetClosestVehicle(player);
                        if (closest != null && closest.Exists)
                        {
                            if (closest.HasSharedData("petrol"))
                            {
                                float petrol = closest.GetSharedData<float>("petrol");
                                petrol += 10;
                                if (petrol > closest.GetSharedData<Int32>("petroltank"))
                                {
                                    petrol = closest.GetSharedData<Int32>("petroltank");
                                }
                                closest.SetSharedData("petrol", petrol);
                                player.TriggerEvent("removeEqItem", itemId);
                                PlayerDataManager.NotifyPlayer(player, "Pojazd pomyślnie zatankowany!");
                            }
                            else
                            {
                                PlayerDataManager.NotifyPlayer(player, "Tego pojazdu nie można zatankować!");
                            }
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Nie stoisz w pobliżu żadnego pojazdu!");
                        }
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Nie możesz być w pojeździe aby tego użyć!");
                    }
                    break;
                case 12:
                    player.TriggerEvent("openTrollBrowser", "XD");
                    player.TriggerEvent("removeEqItem", itemId);
                    break;
            }
        }

        [RemoteEvent("willItemFit")]
        public void WillItemFit(Player player, bool state, int itemId, string type)
        {
            if (type == "plant")
            {
                if (state)
                {
                    switch (itemId)
                    {
                        case 900:
                            PlayerDataManager.NotifyPlayer(player, "Podniosłeś fioletowego bratka!");
                            break;
                        case 901:
                            PlayerDataManager.NotifyPlayer(player, "Podniosłeś różowego bratka!");
                            break;
                        case 902:
                            PlayerDataManager.NotifyPlayer(player, "Podniosłeś żółtego bratka!");
                            break;
                        case 903:
                            PlayerDataManager.NotifyPlayer(player, "Podniosłeś figowca!");
                            break;
                        case 904:
                            PlayerDataManager.NotifyPlayer(player, "Podniosłeś dracenę!");
                            break;
                    }
                    player.TriggerEvent("fitItemInEquipment", itemId);
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Roślina nie zmieściła się do Twojego ekwipunku!");
                }
            }
            else if (type == "shop")
            {
                if (state)
                {
                    int cost = 0;
                    switch (itemId)
                    {
                        case 0:
                        case 1:
                            cost = 20;
                            break;
                        case 2:
                            cost = 5;
                            break;
                        case 11:
                            cost = 250;
                            break;
                    }
                    if (PlayerDataManager.UpdatePlayersMoney(player, -1 * cost))
                    {
                        player.TriggerEvent("addItemToEquipment", itemId);
                        PlayerDataManager.NotifyPlayer(player, "Pomyślnie zakupiono przedmiot!");
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Nie stać Cię na ten przedmiot!");
                    }
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie zmieścisz tego przedmiotu do ekwipunku!");
                }
            }
            else if(type == "rod")
            {
                int cost = 0;
                switch (itemId)
                {
                    case 997:
                        cost = 1000;
                        break;
                    case 998:
                        cost = 3000;
                        break;
                    case 999:
                        cost = 8000;
                        break;
                    case 1000:
                        cost = 15000;
                        break;
                }
                if (PlayerDataManager.UpdatePlayersMoney(player, -1 * cost))
                {
                    player.TriggerEvent("addItemToEquipment", itemId);
                    PlayerDataManager.NotifyPlayer(player, "Pomyślnie zakupiono przedmiot!");
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie stać Cię na ten przedmiot!");
                }
            }
            else
            {
                if (state)
                {
                    DroppedItemsManager.ConfirmPickingUp(player, UInt64.Parse(type));
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie zmieścisz tego przedmiotu do ekwipunku!");
                }
            }

        }

        [RemoteEvent("pickItemUp")]
        public void PickItemUp(Player player, string type)
        {
            DroppedItemsManager.PickItemUp(player, UInt64.Parse(type));
        }


        [RemoteEvent("dropItem")]
        public void DropItem(Player player, int typeId, string itemName)
        {
            if (typeId == 1000 && !PlayerDataManager.HasItem(player, 1000))
            {
                EndJob(player);
            }
            DroppedItemsManager.AddItem(player.Position - new Vector3(0, 0, 1), typeId, itemName);
        }

        [RemoteEvent("updateEquipments")]
        public void UpdateEquipments(Player player, string equipment1, string equipment2, string eqId, Vehicle vehicle = null)
        {
            PlayerDataManager.UpdatePlayersEquipment(player, equipment1);
            if (eqId.Contains('v'))
            {
                eqId = eqId.Remove(0, 1);
                int id;
                if (Int32.TryParse(eqId, out id))
                {
                    Vehicle veh = VehicleDataManager.GetVehicleById(id.ToString());
                    if (veh != null)
                    {
                        VehicleDataManager.UpdateVehiclesTrunk(veh, equipment2);
                    }
                }
            }
            else if (eqId.Contains('h'))
            {
                eqId = eqId.Remove(0, 1);
                int id;
                if (Int32.TryParse(eqId, out id))
                {
                    foreach (House house in Houses.houses)
                    {
                        if (house.id == id)
                        {
                            house.UpdateStorage(equipment2);
                            break;
                        }
                    }
                }
            }
            else if (eqId == "gardener" && vehicle != null && vehicle.Exists)
            {
                vehicle.SetSharedData("trunk", equipment2);
            }
        }
        [RemoteEvent("doesItemFit")]
        public void DoesItemFit(Player player, bool state, int itemId)
        {
            if (state)
            {
                switch (itemId)
                {
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        player.TriggerEvent("peltTaken", state);
                        break;
                    case 1000:
                        FisherMan.ConfirmFishingRod(player);
                        break;
                }
            }
            else
            {
                switch (itemId)
                {
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        PlayerDataManager.NotifyPlayer(player, "Nie zmieścisz tej skóry do ekwipunku!");
                        break;
                    case 1000:
                        PlayerDataManager.NotifyPlayer(player, "Nie zmieścisz wędki do ekwipunku!");
                        break;
                }
            }
        }
    }
}

using GTANetworkAPI;
using Newtonsoft.Json;
using ServerSide;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    public partial class MainClass
    {


        List<TuneBusiness> tuneBusinesses = new List<TuneBusiness>()
            {
            //    new TuneBusiness(1, new Vector3(-2149.3696f, -386.9429f, 14.115926f), new Vector3(-2137.951f, -381.41757f, 14.174999f), new Vector3(-2138.4968f, -388.5696f, 14.115928f), new Vector3(-2142.7688f, -379.78958f, 14.175088f), new Vector3(-2149.2578f, -379.46912f, 14.115923f), new Vector3(-2142.151f, -382.7191f, 14.115902f)),
            //    new TuneBusiness(2, new Vector3(298.72745f, -697.9284f, 29.81644f), new Vector3(293.53754f, -686.1317f, 29.87553f), new Vector3(300.83325f, -686.9971f, 29.816448f), new Vector3(291.95392f, -690.9179f, 29.875538f), new Vector3(291.2251f, -697.3684f, 29.816442f), new Vector3(295.93387f, -690.6306f, 29.81649f)),
            //    new TuneBusiness(3, new Vector3(430.7077f, 2605.0051f, 45.047375f), new Vector3(423.46164f, 2594.4587f, 45.093903f), new Vector3(420.38037f, 2600.8064f, 45.04738f), new Vector3(428.37003f, 2595.4326f, 45.11502f), new Vector3(434.2113f, 2598.2717f, 45.04734f), new Vector3(426.36078f, 2597.379f, 45.047413f))
                new TuneBusiness(1, new Vector3(464.8677f, -1706.1548f, 29.657595f), new Vector3(462.66492f, -1701.8595f, 29.703436f), new Vector3(466.24677f, -1696.6865f, 29.658342f), new Vector3(468.99323f, -1700.6045f, 29.658342f), new Vector3(465.45526f, -1701.1407f, 29.658432f)),
                new TuneBusiness(2, new Vector3(117.79005f, -125.78291f, 55.146957f), new Vector3(121.63005f, -132.44383f, 55.20675f), new Vector3(118.90395f, -131.58363f, 55.146877f), new Vector3(116.59098f, -133.95071f, 55.147007f), new Vector3(120.18769f, -131.04904f, 55.14683f))
            };




        [RemoteEvent("buyTuneBusinessHUD")]
        public void BuyTuneBusinessHUD(Player player, int businessId)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    if (tuneBusiness.Owner == player.SocialClubId)
                    {
                        player.TriggerEvent("openManageTuneBusinessBrowser", businessId);
                    }
                    else if (tuneBusiness.Owner != 0)
                    {
                        playerDataManager.NotifyPlayer(player, "Ten warsztat nie należy do Ciebie!");
                    }
                    else
                    {
                        player.TriggerEvent("openBuyTunePanelBrowser", businessId);
                    }
                    break;
                }
            }
        }

        [RemoteEvent("buyTuneBusiness")]
        public void BuyTuneBusiness(Player player, int businessId)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    if (tuneBusiness.Owner != 0)
                    {
                        playerDataManager.NotifyPlayer(player, "Ten warsztat został już zakupiony!");
                        player.TriggerEvent("closeBuyTunePanelBrowser");
                    }
                    else
                    {
                        if (playerDataManager.UpdatePlayersMoney(player, -250000))
                        {
                            tuneBusiness.SetNewOwner(player.SocialClubId);
                            playerDataManager.NotifyPlayer(player, "Jesteś nowym właścicielem warsztatu!");
                            player.TriggerEvent("closeBuyTunePanelBrowser");
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie stać Cię na to!");
                            player.TriggerEvent("closeBuyTunePanelBrowser");
                        }
                    }
                    break;
                }
            }
        }

        [RemoteEvent("requestAvailableWheels")]
        public void RequestAvailableWheels(Player player)
        {
            player.TriggerEvent("sendAvailableWheels", vehicleDataManager.GetAllAvailableWheels());
        }

        [RemoteEvent("createWheelsOrder")]
        public void CreateWheelsOrder(Player player, int businessId, int currentType, int currentId, int currentAmount, int currentPrice)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    if (playerDataManager.UpdatePlayersMoney(player, -1 * currentAmount * currentPrice))
                    {
                        for (int i = 0; i < currentAmount; i++)
                        {
                            tuneBusiness.WheelOrders.Add(new KeyValuePair<int[], DateTime>(new int[] { currentType, currentId, currentPrice }, DateTime.Now.AddDays(1)));
                        }
                        tuneBusiness.SaveBusinessToDB();
                        playerDataManager.NotifyPlayer(player, "Poprawnie złożono zamówienie!");
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie stać Cię na to zamówienie!");
                    }
                    break;
                }
            }
        }
        [RemoteEvent("requestOwnedWheels")]
        public void RequestOwnedWheels(Player player, int businessId)
        {
            List<string[]> ownedWheels = new List<string[]>();
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    foreach (int[] wheel in tuneBusiness.AvailableWheels)
                    {
                        ownedWheels.Add(new string[]
                        {
                            vehicleDataManager.GetWheelNameById(wheel[0], wheel[1]),
                            wheel[0].ToString(),
                            wheel[1].ToString(),
                            wheel[2].ToString()
                        });
                    }
                    break;
                }
            }
            player.TriggerEvent("sendOwnedWheels", JsonConvert.SerializeObject(ownedWheels));
        }

        [RemoteEvent("requestShipmentWheels")]
        public void RequestShipmentWheels(Player player, int businessId)
        {
            List<string[]> shipmentWheels = new List<string[]>();
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    foreach (KeyValuePair<int[], DateTime> order in tuneBusiness.WheelOrders)
                    {
                        shipmentWheels.Add(new string[]
                        {
                            vehicleDataManager.GetWheelNameById(order.Key[0], order.Key[1]),
                            order.Key[0].ToString(),
                            order.Key[1].ToString(),
                            order.Value.ToString()
                        });
                    }
                    break;
                }
            }
            player.TriggerEvent("sendShipmentWheels", JsonConvert.SerializeObject(shipmentWheels));
        }

        [RemoteEvent("requestManageData")]
        public void RequestManageData(Player player, int businessId)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    player.TriggerEvent("sendManageData", tuneBusiness.PaidTo.ToString(), "1500");
                    break;
                }
            }
        }

        [RemoteEvent("extendTuneBusinessTime")]
        public void ExtendTuneTime(Player player, int businessId, int days)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    if (tuneBusiness.PaidTo.AddDays(days) > DateTime.Now.AddDays(2))
                    {
                        playerDataManager.NotifyPlayer(player, "Działalność można opłacić na maksymalnie 2 dni do przodu!");
                    }
                    else
                    {
                        if (playerDataManager.UpdatePlayersMoney(player, -1 * days * 1500))
                        {
                            tuneBusiness.PaidTo = tuneBusiness.PaidTo.AddDays(days);
                            tuneBusiness.SaveBusinessToDB();
                            player.TriggerEvent("sendManageData", tuneBusiness.PaidTo.ToString(), "1500");
                            playerDataManager.NotifyPlayer(player, "Pomyślnie dokonano opłaty!");
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie stać Cię na to!");
                        }
                    }
                    break;
                }
            }
        }
        [RemoteEvent("switchBusinessJob")]
        public void SwitchBusinessJob(Player player, int businessId)
        {
            if (player.HasSharedData("job") && player.GetSharedData<string>("job") != "business-tune" && player.GetSharedData<string>("job") != "")
            {
                playerDataManager.NotifyPlayer(player, "Musisz zakończyć pracę przed wejściem na stanowisko!");
            }
            else
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Id == businessId)
                    {
                        if (player.HasSharedData("job") && player.GetSharedData<string>("job") == "business-tune")
                        {
                            tuneBusiness.SetOwnerWorking(false);
                            player.SetSharedData("job", "");
                            player.SetSharedData("business-id", 0);
                            player.TriggerEvent("closeManageTuneBusinessBrowser");
                        }
                        else
                        {
                            tuneBusiness.SetOwnerWorking(true);
                            player.SetSharedData("job", "business-tune");
                            player.SetSharedData("business-id", businessId);
                            player.TriggerEvent("closeManageTuneBusinessBrowser");
                        }
                        break;
                    }
                }
            }

        }

        [RemoteEvent("openBusinessWheelsStation")]
        public void OpenBusinessWheelsStation(Player player, int businessId)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    if (tuneBusiness.StationVeh != null && tuneBusiness.VehColshape.Exists && tuneBusiness.VehColshape.IsPointWithin(tuneBusiness.StationVeh.Position))
                    {
                        List<string[]> availableWheels = new List<string[]>();
                        for (int i = 0; i < tuneBusiness.AvailableWheels.Count; i++)
                        {
                            var wheel = tuneBusiness.AvailableWheels[i];
                            availableWheels.Add(new string[]
                            {
                                i.ToString(),
                                vehicleDataManager.GetWheelNameById(wheel[0], wheel[1]),
                                wheel[0].ToString(),
                                wheel[1].ToString(),
                                wheel[2].ToString()
                            });
                        }
                        player.TriggerEvent("openWheelsTuneBrowser", businessId, JsonConvert.SerializeObject(availableWheels), vehicleDataManager.GetVehiclesWheels(tuneBusiness.StationVeh), tuneBusiness.StationVeh);
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Na stanowisku nie ma żadnego pojazdu");
                    }
                    break;
                }
            }
        }
        [RemoteEvent("previewWheelTune")]
        public void PreviewWheelTune(Player player, Vehicle vehicle, int type, int id, int sport, int businessId)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    if (tuneBusiness.StationVeh != null && tuneBusiness.StationVeh == vehicle)
                    {
                        NAPI.Vehicle.SetVehicleWheelType(vehicle.Handle, type);
                        vehicle.SetMod(23, id);
                        if (NAPI.Vehicle.GetVehicleClass((VehicleHash)vehicle.Model) == 8)
                        {
                            vehicle.SetMod(24, id);
                        }

                        NAPI.Vehicle.SetVehicleCustomTires(vehicle.Handle, sport == 1 ? true : false);
                        NAPI.Vehicle.SetVehicleWheelColor(vehicle.Handle, 156);
                    }
                    else
                    {
                        player.TriggerEvent("closeWheelsTuneBrowser");
                        playerDataManager.NotifyPlayer(player, "Wystąpił błąd");
                    }
                    break;
                }
            }
        }

        [RemoteEvent("sendWheelTuneOffer")]
        public void SendWheelTuneOffer(Player player, Vehicle vehicle, int businessId, string installType, string name, int type, int id, int price, int partId)
        {
            if (vehicle != null && vehicle.Exists)
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Id == businessId && tuneBusiness.StationVeh == vehicle && tuneBusiness.VehColshape.IsPointWithin(vehicle.Position) && vehicle.Occupants.Count > 0)
                    {
                        if (vehicle.HasSharedData("owner") && vehicle.GetSharedData<Int64>("owner").ToString() == vehicle.Occupants[0].GetSharedData<string>("socialclub"))
                        {
                            Player driver = (Player)vehicle.Occupants[0];
                            if (driver.HasSharedData("tuneOffer") && driver.GetSharedData<bool>("tuneOffer"))
                            {
                                playerDataManager.NotifyPlayer(player, "Złożyłeś już ofertę temu graczowi!");
                            }
                            else
                            {
                                switch (installType)
                                {
                                    case "install":
                                        driver.TriggerEvent("openConfirmWheelsTunePanel", businessId, type, id, name, price, true, partId);
                                        playerDataManager.NotifyPlayer(player, "Pomyślnie wysłano ofertę!");
                                        break;
                                    case "remove":
                                        driver.TriggerEvent("openConfirmWheelsTunePanel", businessId, type, id, name, price, false, partId);
                                        playerDataManager.NotifyPlayer(player, "Pomyślnie   wysłano ofertę!");
                                        break;
                                }
                            }

                        }
                        else
                        {
                            Player driver = (Player)vehicle.Occupants[0];
                            player.TriggerEvent("closeWheelsTuneBrowser");
                            playerDataManager.NotifyPlayer(player, "Części może montować tylko właściciel pojazdu!");
                            playerDataManager.NotifyPlayer(driver, "Części może montować tylko właściciel pojazdu!");
                        }
                        break;
                    }
                }
            }
        }
        [RemoteEvent("declineWheelsTuneOffer")]
        public void DeclineWheelsTuneOffer(Player player, int businessId, int wheelType, int wheelId, int wheelPrice, bool state)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    foreach (Player owner in NAPI.Pools.GetAllPlayers())
                    {
                        if (tuneBusiness.Owner == owner.SocialClubId)
                        {
                            player.SetSharedData("tuneOffer", false);
                            playerDataManager.NotifyPlayer(owner, "Gracz odrzucił ofertę montażu felg!");
                            owner.TriggerEvent("closeWheelsTuneBrowser");
                            break;
                        }
                    }
                    break;
                }
            }
        }
        [RemoteEvent("acceptWheelsTuneOffer")]
        public void AcceptWheelsTuneOffer(Player player, int businessId, int wheelType, int wheelId, int wheelPrice, bool state, int partId)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    foreach (Player owner in NAPI.Pools.GetAllPlayers())
                    {
                        if (tuneBusiness.Owner == owner.SocialClubId)
                        {
                            if (player.Vehicle != null && player.Vehicle.Exists)
                            {
                                if (state)
                                {
                                    if (playerDataManager.UpdatePlayersMoney(player, -1 * wheelPrice))
                                    {
                                        playerDataManager.UpdatePlayersMoney(owner, wheelPrice);
                                        vehicleDataManager.UpdateVehiclesWheels(player.Vehicle, JsonConvert.SerializeObject(new int[] { wheelType, wheelId, 0 }));
                                        playerDataManager.NotifyPlayer(player, "Pomyślnie zamontowano felgi!");
                                        playerDataManager.NotifyPlayer(owner, "Gracz zaakcpetował ofertę!");
                                        owner.TriggerEvent("closeWheelsTuneBrowser");
                                        tuneBusiness.AvailableWheels.RemoveAt(partId);
                                        tuneBusiness.SaveBusinessToDB();

                                    }
                                    else
                                    {
                                        playerDataManager.NotifyPlayer(player, "Nie stać cię na to!");
                                        playerDataManager.NotifyPlayer(owner, "Gracz odrzucił ofertę montażu felg!");
                                        owner.TriggerEvent("closeWheelsTuneBrowser");
                                    }
                                }
                                else
                                {
                                    if (playerDataManager.UpdatePlayersMoney(owner, -1 * wheelPrice))
                                    {
                                        playerDataManager.UpdatePlayersMoney(player, wheelPrice);
                                        vehicleDataManager.UpdateVehiclesWheels(player.Vehicle, JsonConvert.SerializeObject(new int[] { 0, -1, 0 }));
                                        playerDataManager.NotifyPlayer(player, "Pomyślnie zdemontowano felgi!");
                                        playerDataManager.NotifyPlayer(owner, "Gracz zaakcpetował ofertę!");
                                        tuneBusiness.AvailableWheels.Add(new int[] { wheelType, wheelId, vehicleDataManager.GetWheelPriceById(wheelType, wheelId) });
                                        tuneBusiness.SaveBusinessToDB();
                                        owner.TriggerEvent("closeWheelsTuneBrowser");
                                    }
                                    else
                                    {
                                        playerDataManager.NotifyPlayer(owner, "Nie masz tyle pieniędzy!");
                                        playerDataManager.NotifyPlayer(player, "Tunera nie stać na to!");
                                        owner.TriggerEvent("closeWheelsTuneBrowser");
                                    }
                                }
                                player.SetSharedData("tuneOffer", false);
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(owner, "Nie odnaleziono pojazdu!");
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }





        //tuner
        [RemoteEvent("openBusinessMechStation")]
        public void OpenBusinessMechStation(Player player, int businessId)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    if (tuneBusiness.StationVeh != null && tuneBusiness.StationVeh.Exists && tuneBusiness.VehColshape.IsPointWithin(tuneBusiness.StationVeh.Position))
                    {
                        player.TriggerEvent("openMechTuneBrowser", businessId, vehicleDataManager.GetVehiclesMechTune(tuneBusiness.StationVeh), tuneBusiness.StationVeh);
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Na stanowisku nie ma żadnego pojazdu");
                    }
                    break;
                }
            }
        }
        [RemoteEvent("sendMechTuneOffer")]
        public void SendMechTuneOffer(Player player, Vehicle vehicle, int businessId, string installType, int tuneId, string tuneName, int fullPrice, int offer)
        {
            if (vehicle != null && vehicle.Exists)
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Id == businessId)
                    {
                        if (tuneBusiness.StationVeh == vehicle)
                        {
                            if (tuneBusiness.VehColshape.IsPointWithin(vehicle.Position))
                            {
                                if (vehicle.Occupants.Count > 0)
                                {
                                    if (vehicle.HasSharedData("owner") && vehicle.GetSharedData<Int64>("owner").ToString() == vehicle.Occupants[0].GetSharedData<string>("socialclub"))
                                    {
                                        Player driver = (Player)vehicle.Occupants[0];
                                        if (driver.HasSharedData("tuneOffer") && driver.GetSharedData<bool>("tuneOffer"))
                                        {
                                            playerDataManager.NotifyPlayer(player, "Złożyłeś już ofertę temu graczowi!");
                                        }
                                        else
                                        {
                                            driver.TriggerEvent("openConfirmTunePanel", businessId, installType == "install" ? true : false, tuneName, fullPrice, tuneId, offer);
                                            playerDataManager.NotifyPlayer(player, "Oferta złożona!");
                                        }

                                    }
                                    else
                                    {
                                        Player driver = (Player)vehicle.Occupants[0];
                                        player.TriggerEvent("closeWheelsTuneBrowser");
                                        playerDataManager.NotifyPlayer(player, "Części może montować tylko właściciel pojazdu!");
                                        playerDataManager.NotifyPlayer(driver, "Części może montować tylko właściciel pojazdu!");
                                    }
                                }
                                else
                                {
                                    player.TriggerEvent("closeMechTuneBrowser");
                                    playerDataManager.NotifyPlayer(player, "Nie ma pasażerów!");
                                }
                            }
                            else
                            {
                                player.TriggerEvent("closeMechTuneBrowser");
                                playerDataManager.NotifyPlayer(player, "Pojazd nie jest w colshape");
                            }
                        }
                        else
                        {
                            player.TriggerEvent("closeMechTuneBrowser");
                            playerDataManager.NotifyPlayer(player, "Nie znaleziono pojazdu");
                        }
                        break;
                    }
                }
            }
        }
        [RemoteEvent("declineMechTuneOffer")]
        public void DeclineMechTuneOffer(Player player, int businessId)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    foreach (Player owner in NAPI.Pools.GetAllPlayers())
                    {
                        if (tuneBusiness.Owner == owner.SocialClubId)
                        {
                            player.SetSharedData("tuneOffer", false);
                            playerDataManager.NotifyPlayer(owner, "Gracz odrzucił ofertę montażu tuningu!");
                            owner.TriggerEvent("closeMechTuneBrowser");
                            break;
                        }
                    }
                    break;
                }
            }
        }
        [RemoteEvent("acceptMechTuneOffer")]
        public void AcceptMechTuneOffer(Player player, int businessId, string state, string name, int price, int partId, int offer)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    foreach (Player owner in NAPI.Pools.GetAllPlayers())
                    {
                        if (tuneBusiness.Owner == owner.SocialClubId)
                        {
                            if (player.Vehicle != null && player.Vehicle.Exists)
                            {
                                if (state == "1")
                                {
                                    if (playerDataManager.UpdatePlayersMoney(player, -1 * price))
                                    {
                                        playerDataManager.UpdatePlayersMoney(owner, offer);
                                        List<int> tuneList = JsonConvert.DeserializeObject<List<int>>(player.Vehicle.GetSharedData<string>("mechtune"));
                                        tuneList[partId] = 1;
                                        vehicleDataManager.UpdateVehiclesMechTune(player.Vehicle, JsonConvert.SerializeObject(tuneList));
                                        vehicleDataManager.RefreshVehiclesTune(player.Vehicle);
                                        playerDataManager.NotifyPlayer(player, $"Pomyślnie zamontowano {name}!");
                                        playerDataManager.NotifyPlayer(owner, "Gracz zaakcpetował ofertę!");
                                        owner.TriggerEvent("closeMechTuneBrowser");
                                    }
                                    else
                                    {
                                        playerDataManager.NotifyPlayer(player, "Nie stać cię na to!");
                                        playerDataManager.NotifyPlayer(owner, "Gracz odrzucił ofertę montażu tuningu!");
                                        owner.TriggerEvent("closeMechTuneBrowser");
                                    }
                                }
                                else
                                {
                                    if (playerDataManager.UpdatePlayersMoney(player, price))
                                    {
                                        playerDataManager.UpdatePlayersMoney(owner, offer);
                                        List<int> tuneList = JsonConvert.DeserializeObject<List<int>>(player.Vehicle.GetSharedData<string>("mechtune"));
                                        tuneList[partId] = 0;
                                        vehicleDataManager.UpdateVehiclesMechTune(player.Vehicle, JsonConvert.SerializeObject(tuneList));
                                        vehicleDataManager.RefreshVehiclesTune(player.Vehicle);
                                        playerDataManager.NotifyPlayer(player, $"Pomyślnie zdemontowano {name}!");
                                        playerDataManager.NotifyPlayer(owner, "Gracz zaakcpetował ofertę!");
                                        owner.TriggerEvent("closeMechTuneBrowser");
                                    }
                                    else
                                    {
                                        owner.TriggerEvent("closeMechTuneBrowser");
                                    }
                                }
                                player.SetSharedData("tuneOffer", false);
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(owner, "Nie odnaleziono pojazdu!");
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }


        //wheels
        [RemoteEvent("openWheelsTune")]
        public void OpenWheelsTune(Player player)
        {
            if (player.Vehicle != null && player.Vehicle.Exists && player.Vehicle.HasSharedData("owner") && player.Vehicle.GetSharedData<Int64>("owner").ToString() == player.GetSharedData<string>("socialclub"))
            {
                player.TriggerEvent("openWheelsTuneBrowser", vehicleDataManager.GetAvailableWheelsForVehicle(player.Vehicle), vehicleDataManager.GetVehiclesWheels(player.Vehicle), player.Vehicle);

            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie jesteś właścicielem pojazdu!");
            }
        }

        [RemoteEvent("removeCurrentWheels")]
        public void RemoveCurrentWheels(Player player, Vehicle vehicle, int price)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (playerDataManager.UpdatePlayersMoney(player, price))
                {
                    vehicleDataManager.UpdateVehiclesWheels(vehicle, "[0, -1, 0]");
                    vehicleDataManager.SetVehiclesWheels(vehicle);

                    player.TriggerEvent("refreshWheelsTuneBrowser", vehicleDataManager.GetAvailableWheelsForVehicle(player.Vehicle), vehicleDataManager.GetVehiclesWheels(player.Vehicle), player.Vehicle);
                    playerDataManager.NotifyPlayer(player, "Pomyślnie zdemontowano felgi!");
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Transakcja nie powiodła się!");
                }
            }
        }



        [RemoteEvent("applyWheelTune")]
        public void ApplyWheelTune(Player player, Vehicle vehicle, int type, int id, int sport, int price)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (playerDataManager.UpdatePlayersMoney(player, -price))
                {
                    int[] wheels = new int[]
                    {
                        type, id, sport
                    };
                    vehicleDataManager.UpdateVehiclesWheels(vehicle, JsonConvert.SerializeObject(wheels));
                    vehicleDataManager.SetVehiclesWheels(vehicle);
                    player.TriggerEvent("closeWheelsTuneBrowser");
                    playerDataManager.NotifyPlayer(player, "Pomyślnie zamontowano część!");
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie stać Cię na to!");
                }
            }
        }

        [RemoteEvent("closeBusiness")]
        public void CloseBusiness(Player player, int businessId)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Owner == player.SocialClubId && tuneBusiness.Id == businessId)
                {
                    tuneBusiness.ResetOwner();
                    playerDataManager.NotifyPlayer(player, "Pomyślnie zrezygnowano z biznesu!");
                }
            }
        }





        //visu
        [RemoteEvent("openVisuTune")]
        public void OpenVisuTune(Player player)
        {
            if (player.Vehicle != null && player.Vehicle.Exists && player.Vehicle.HasSharedData("owner") && player.Vehicle.GetSharedData<Int64>("owner").ToString() == player.GetSharedData<string>("socialclub"))
            {
                if (!vehicleDataManager.IsVehicleDamaged(player.Vehicle))
                {
                    player.TriggerEvent("openVisuTuneBrowser", vehicleDataManager.GetVehicleAvailableTune(player.Vehicle), vehicleDataManager.GetVehiclesCurrentTune(player.Vehicle), player.Vehicle);
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Pojazd jest uszkodzony!");
                }

            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie jesteś właścicielem pojazdu!");
            }
        }
        [RemoteEvent("previewVisuTune")]
        public void PreviewVisuTune(Player player, Vehicle vehicle, int modtype, int mod)
        {
            if (vehicle != null && vehicle.Exists)
            {
                vehicle.SetMod(modtype, mod);
            }
        }

        [RemoteEvent("applyVisuTune")]
        public void ApplyVisuTune(Player player, Vehicle vehicle, int modtype, int mod, int price)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (playerDataManager.UpdatePlayersMoney(player, -price))
                {
                    Dictionary<int, int> tune = JsonConvert.DeserializeObject<Dictionary<int, int>>(vehicle.GetSharedData<string>("tune"));
                    tune.Add(modtype, mod);
                    vehicleDataManager.UpdateVehiclesTune(vehicle, JsonConvert.SerializeObject(tune));
                    player.TriggerEvent("removeVisuType", modtype);
                    playerDataManager.NotifyPlayer(player, "Pomyślnie zamontowano część!");
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie stać Cię na to!");
                }
                vehicle.SetMod(modtype, mod);
            }
        }

        [RemoteEvent("removeVisuTune")]
        public void RemoveVisuTune(Player player, Vehicle vehicle, int modtype, int mod, int price)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (playerDataManager.UpdatePlayersMoney(player, price))
                {
                    Dictionary<int, int> tune = JsonConvert.DeserializeObject<Dictionary<int, int>>(vehicle.GetSharedData<string>("tune"));
                    tune.Remove(modtype);
                    playerDataManager.NotifyPlayer(player, "Pomyślnie zdemontowano część!");
                    vehicleDataManager.UpdateVehiclesTune(vehicle, JsonConvert.SerializeObject(tune));
                    NAPI.Task.Run(() =>
                    {
                        vehicleDataManager.RefreshVehiclesTune(vehicle);
                    }, delayTime: 2000);
                    player.TriggerEvent("refreshVisuTuneBrowser", vehicleDataManager.GetVehicleAvailableTune(player.Vehicle), vehicleDataManager.GetVehiclesCurrentTune(player.Vehicle), player.Vehicle);
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Transakcja nie powiodła się!");
                }
                vehicle.SetMod(modtype, mod);
            }
        }



    }
}

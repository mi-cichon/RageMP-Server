using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {
        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Player player, Player killer, uint reason)
        {
            if (player.HasSharedData("handObj") && player.GetSharedData<string>("handObj") != "")
            {
                player.SetSharedData("handObj", "");
            }
            player.SetSharedData("deathpos", player.Position);
            player.TriggerEvent("closeSpeedometerBrowser");
            if (player.HasSharedData("job") && player.GetSharedData<string>("job") == "business-tune" && player.GetSharedData<int>("business-id") != 0)
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Owner == player.SocialClubId)
                    {
                        tuneBusiness.SetOwnerWorking(false);
                        break;
                    }
                }
            }
            if (player.GetSharedData<string>("job") != "")
            {
                if (player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty"))
                {
                    LSPD.SwitchPlayersDuty(player, true);
                }
                else
                {
                    EndJob(player);
                }
            }
            PlayerDataManager.SetJobClothes(player, false, "");
        }

        [ServerEvent(Event.VehicleDeath)]
        public void OnVehicleDeath(Vehicle vehicle)
        {
            if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "personal")
            {
                VehicleDataManager.UpdateVehiclesDamage(vehicle, VehicleDataManager.wreckedDamage);
                VehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                vehicle.Delete();
            }
        }

        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player)
        {
            LogManager.CreatePlayersDirectories(player.SocialClubId.ToString());
            try
            {
                //if (PlayerDataManager.IsWhiteListed(player))
                //{
                Console.WriteLine(player.Address + " " + player.SocialClubName);
                PlayerDataManager.SetPlayerDataFromDB(player);
                player.SetSharedData("bonustime", AutoSave.GetPlayersBonusData(player));
                PlayerDataManager.setUsersPenalties(player);

                bool house = Houses.SetPlayersHouse(player);
                LSPD.IsPlayerInGroup(player);
                NAPI.Entity.SetEntityTransparency(player.Handle, 0);
                player.Dimension = 1;
                player.Position = new Vector3(116.44823f, -921.2917f, 29.941246f);
                player.TriggerEvent("setHouseBlipsInvisible");
                if (player.HasSharedData("banned") && player.GetSharedData<bool>("banned"))
                {
                    player.Kick("Jesteś zbanowany!");
                }
                if (player.GetSharedData<string>("character") == "")
                {
                    player.Dimension = (uint)1000 + player.Id;
                    player.Position = new Vector3(-1562.3833f, -564.9895f, 114.575905f);
                    player.Rotation = new Vector3(0, 0, 100);
                    player.Heading = 100;
                    player.TriggerEvent("client:creator.show");
                    player.SetSharedData("gui", true);
                }
                else
                {
                    player.TriggerEvent("createCharacter");
                    PlayerDataManager.SetPlayersClothes(player);
                    if (!LSPD.IsPlayerArrested(player))
                    {
                        player.TriggerEvent("openSpawnSelection", house);
                        player.SetSharedData("gui", true);
                    }
                    else
                    {
                        PlayerDataManager.SetPlayersConnectValues(player, currentWeather);
                        LSPD.ReturnPlayerIntoArrest(player);
                        PlayerDataManager.NotifyPlayer(player, "Jesteś aresztowany! Pozostało " + player.GetSharedData<int>("arrested_time").ToString() + " minut aresztu!");
                    }
                }
                DoorManager.SetDoorsForPlayer(player);
                OrgManager.SetPlayersOrg(player);

                //}
                //else
                //{
                //    LogManager.LogLoginInfo(player.SocialClubId.ToString(), $"Próba dołączenia bez WL, Nick: {player.SocialClubName}, IP: {player.Address}");
                //    player.Kick("WL");
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seatId)
        {
            player.SetSharedData("vehSeat", (int)(byte)seatId);
            if (!AntiCheat.ShouldPlayerEnterThisVehicle(player, vehicle))
            {
                player.TriggerEvent("openTrollBrowser", "rick");
                player.WarpOutOfVehicle();
                return;
            }
            if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "jobveh" && !(vehicle.HasSharedData("player") && vehicle.GetSharedData<bool>("player")))
            {
                vehicle.SetSharedData("player", true);
                player.SetSharedData("jobveh", vehicle.Id);
                vehicle.SetSharedData("veh_brake", false);
                foreach (JobVehicleSpawn vehSpawn in jobVehicleSpawns)
                {
                    if (vehSpawn.Veh == vehicle)
                    {
                        vehSpawn.Veh = null;
                        break;
                    }
                }
            }
            if (player.HasSharedData("username") && seatId == 0)
                vehicle.SetSharedData("admin_lastDriver", player.GetSharedData<string>("username"));
            if (vehicle.HasSharedData("storageTime"))
            {
                vehicle.SetSharedData("storageTime", "");
            }
            if (vehicle.HasSharedData("mech") && vehicle.GetSharedData<bool>("mech"))
            {
                player.WarpOutOfVehicle();
            }
            else if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "public")
            {
                vehicle.SetSharedData("publicTime", "");
                foreach (PublicVehicleSpawn pvs in publicVehicleSpawns)
                {
                    if (pvs.veh != null && pvs.veh == vehicle)
                    {
                        pvs.veh = null;
                    }
                }
            }
            else if (vehicle.HasSharedData("market") && vehicle.GetSharedData<bool>("market"))
            {
                carMarket.RemoveVehicleFromMarket(vehicle);
            }
            if (vehicle.HasSharedData("drivers") && seatId == 0)
            {
                VehicleDataManager.SaveVehiclesDriver(vehicle, player);
            }
            player.SetSharedData("seatbelt", false);
        }
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnect(Player player, DisconnectionType type, string reason)
        {
            LogManager.LogLoginInfo(player.SocialClubId.ToString(), $"Opuszczono serwer z IP: {player.Address}");
            if (player.HasSharedData("handObj") && player.GetSharedData<string>("handObj") != "")
            {
                player.SetSharedData("handObj", "");
            }
            if (player.HasSharedData("jobveh") && player.GetSharedData<int>("jobveh") != -1111)
            {
                var veh = VehicleDataManager.GetVehicleByRemoteId(Convert.ToUInt16(player.GetSharedData<Int32>("jobveh")));
                if (veh != null && veh.Exists)
                    veh.Delete();
            }
            if (drivingLicences.currentPlayerPassing == player)
            {
                drivingLicences.currentPlayerPassing = null;
            }
            if (player.HasSharedData("job") && player.GetSharedData<string>("job") == "business-tune" && player.GetSharedData<int>("business-id") != 0)
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Owner == player.SocialClubId)
                    {
                        tuneBusiness.SetOwnerWorking(false);
                        break;
                    }
                }
            }
        }
        [ServerEvent(Event.PlayerSpawn)]
        public void OnPlayerSpawn(Player player)
        {
            if (player.HasSharedData("arrested") && player.GetSharedData<bool>("arrested"))
            {
                LSPD.SpawnPlayerInArrest(player);
            }
            if (player.HasSharedData("handCuffed") && player.GetSharedData<bool>("handCuffed") && player.GetSharedData<Player>("cuffedBy").Exists)
            {
                player.Position = player.GetSharedData<Player>("cuffedBy").Position;
            }
            else
            {
                PlayerDataManager.SpawnPlayerAtClosestHospital(player);
            }
            player.Dimension = 0;
        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void OnPlayerExitVehicle(Player player, Vehicle vehicle)
        {
            int seat = 0;
            if (player.HasSharedData("vehSeat"))
            {
                seat = player.GetSharedData<int>("vehSeat");
            }
            if (seat == 0)
            {
                if (vehicle.HasSharedData("owner"))
                {
                    VehicleDataManager.UpdateVehiclesUsedTime(vehicle);
                }
                if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "LSPD")
                {
                    if (LSPD.StorageCenter.IsPointWithin(vehicle.Position))
                    {
                        if (!VehicleDataManager.IsVehicleDamaged(vehicle))
                        {
                            if (vehicle.HasSharedData("petroltank") && vehicle.GetSharedData<float>("petrol") / vehicle.GetSharedData<int>("petroltank") >= 0.9f)
                            {
                                LSPD.SetVehicleSpawned(vehicle.GetSharedData<int>("id"), false);
                                PlayerDataManager.NotifyPlayer(player, $"Pojazd został przeniesiony na parking!");
                                vehicle.Delete();
                            }
                            else
                            {
                                PlayerDataManager.NotifyPlayer(player, "Pojazd musi być zatankowany w przynajmniej 90%!");
                            }
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Pojazd jest za bardzo uszkodzony!");
                        }
                    }
                }
                else
                {
                    foreach (VehicleStorage vs in vehicleStorages)
                    {
                        if (vehicle != null && vehicle.Exists && vs.storageColShape.IsPointWithin(vehicle.Position))
                        {
                            if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "personal")
                            {
                                VehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                                PlayerDataManager.NotifyPlayer(player, $"Pojazd o ID: {vehicle.GetSharedData<Int32>("id").ToString()} został przeniesiony do przechowalni!");
                                vehicle.Delete();
                                break;
                            }
                        }
                    }
                }

            }

            if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "public")
            {
                vehicle.SetSharedData("publicTime", DateTime.Now.ToLongTimeString());
                PlayerDataManager.NotifyPlayer(player, "Masz minutę na powrót do pojazdu!");
            }
        }

        [ServerEvent(Event.PlayerEnterColshape)]
        public void OnPlayerEnterColshape(ColShape shape, Player player)
        {
            if (player.Vehicle != null && player.Vehicle.HasSharedData("petrol") && player.VehicleSeat == 0 && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "stationveh")
            {
                foreach (PetrolStation petrolStation in petrolStations)
                {
                    if (petrolStation.vehicleColShape == shape)
                    {
                        if (petrolStation.currentVehicle != null && petrolStation.currentVehicle.Exists && petrolStation.vehicleColShape.IsPointWithin(petrolStation.currentVehicle.Position))
                        {
                            PlayerDataManager.NotifyPlayer(player, "To stanowisko jest zajęte!");
                        }
                        else
                        {
                            if (petrolStation.currentVehicle != null && petrolStation.currentVehicle.Exists)
                            {
                                petrolStation.currentVehicle.SetSharedData("petrol_onStation", false);
                            }
                            petrolStation.currentVehicle = player.Vehicle;
                            player.Vehicle.SetSharedData("petrol_onStation", true);
                            PlayerDataManager.NotifyPlayer(player, "Udaj się do dystrybutora aby rozpocząć proces tankowania!");
                        }
                        break;
                    }
                }
            }
            if (shape.HasSharedData("pair"))
            {
                PlayerDataManager.NotifyPlayer(player, shape.GetSharedData<string>("name"));
            }
            // --------------Colshape przechowalni-----------------
            if (player.Vehicle != null && player.Vehicle.Exists && player.GetSharedData<int>("vehSeat") == 0)
            {
                if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "storagein")
                {
                    PlayerDataManager.NotifyPlayer(player, "Zaparkuj tu pojazd aby schować go do przechowalni!");
                }
                if (player.Vehicle.GetSharedData<string>("type") == "LSPD" && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "lspd_storageCenter")
                {
                    PlayerDataManager.NotifyPlayer(player, "Zaparkuj tu pojazd aby schować go na parking policyjny!");
                }
            }
            if (player.Vehicle != null && player.Vehicle.Exists && player.Vehicle.HasSharedData("type") && player.Vehicle.GetSharedData<string>("type") == "personal" && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "business-veh")
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Id == shape.GetSharedData<int>("business-id"))
                    {
                        tuneBusiness.StationVeh = player.Vehicle;
                        break;
                    }
                }
            }
            if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "lspd_mainGate" && player.Vehicle != null && player.GetSharedData<int>("vehSeat") == 0 && player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty"))
            {
                LSPD.SwitchMainGate(true);
            }
            else if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "lspd_backGate" && player.Vehicle != null && player.GetSharedData<int>("vehSeat") == 0 && player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty"))
            {
                LSPD.SwitchBackGate(true);
            }
        }

        [ServerEvent(Event.PlayerExitColshape)]
        public void OnPlayerExitColShape(ColShape shape, Player player)
        {
            if(shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "speedoColor" && player.Vehicle != null && player.Vehicle.HasSharedData("speedometer") && player.VehicleSeat == 0)
            {
                player.TriggerEvent("speedometer_setColor", player.Vehicle.GetSharedData<string>("speedometer"));
            }
            if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "stationveh" && player.Vehicle != null && player.Vehicle.HasSharedData("petrol_onStation") && player.VehicleSeat == 0)
            {
                player.Vehicle.SetSharedData("petrol_onStation", false);
                player.Vehicle.SetSharedData("petrol_refueling", false);
                foreach (PetrolStation petrolStation in petrolStations)
                {
                    if (petrolStation.vehicleColShape == shape && petrolStation.currentVehicle == player.Vehicle)
                    {
                        petrolStation.currentVehicle = null;
                        break;
                    }
                }
            }
            if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "fishingspot" && player.GetSharedData<string>("job") == "fisherman")
            {
                EndJob(player);
            }
            if (player.Vehicle != null && player.Vehicle.Exists && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "visutune")
            {
                VehicleDataManager.RefreshVehiclesTune(player.Vehicle);
            }
            else if (player.Vehicle != null && player.Vehicle.Exists && player.Vehicle.HasSharedData("type") && player.Vehicle.GetSharedData<string>("type") == "public")
            {
                foreach (PublicVehicleSpawn pvs in publicVehicleSpawns)
                {
                    if (pvs.veh == player.Vehicle)
                    {
                        pvs.veh = null;
                        pvs.leaveTime = DateTime.Now;
                        break;
                    }
                }
            }
            if (player.Vehicle != null && player.Vehicle.Exists && player.Vehicle.HasSharedData("type") && player.Vehicle.GetSharedData<string>("type") == "personal" && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "business-veh")
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Id == shape.GetSharedData<int>("business-id"))
                    {
                        tuneBusiness.StationVeh = null;
                        VehicleDataManager.SetVehiclesWheels(player.Vehicle);
                        break;
                    }
                }
            }
            if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "business-tune-center" && player.HasSharedData("job") && player.GetSharedData<string>("job") == "business-tune" && player.GetSharedData<int>("business-id") == shape.GetSharedData<int>("business-id"))
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Owner == player.SocialClubId)
                    {
                        tuneBusiness.SetOwnerWorking(false);
                        player.SetSharedData("job", "");
                        break;
                    }
                }
            }
            if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "lspd_mainGate" && player.Vehicle != null && player.GetSharedData<int>("vehSeat") == 0 && player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty"))
            {
                LSPD.SwitchMainGate(false);
            }
            else if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "lspd_backGate" && player.Vehicle != null && player.GetSharedData<int>("vehSeat") == 0 && player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty"))
            {
                LSPD.SwitchBackGate(false);
            }
        }
        [ServerEvent(Event.VehicleTrailerChange)]
        public void OnVehicleTrailerChange(Vehicle vehicle, Vehicle trailer)
        {
            Console.WriteLine("nowa naczepa");
        }
    }
}

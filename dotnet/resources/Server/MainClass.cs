using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using System.Diagnostics;
using System.Timers;
using System.Threading.Tasks;
using System.IO;
using ServerSide.Jobs;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;

namespace ServerSide
{
    public class MainClass : Script
    {
        private LogManager logManager = new LogManager();
        private PlayerDataManager playerDataManager = new PlayerDataManager();
        private VehicleDataManager vehicleDataManager = new VehicleDataManager();
        private DroppedItemsManager droppedItemsManager = new DroppedItemsManager();
        private PayoutManager payoutManager = new PayoutManager();
        private Peds peds = new Peds();
        public List<CarDealer> carDealers = new List<CarDealer>();
        public List<VehicleStorage> vehicleStorages = new List<VehicleStorage>();
        public List<PublicVehicleSpawn> publicVehicleSpawns = new List<PublicVehicleSpawn>();
        public List<VehicleMechanic> vehicleMechanics = new List<VehicleMechanic>();
        public List<ChangingRoom> changingRooms = new List<ChangingRoom>();
        public List<CarWash> carWashes = new List<CarWash>();
        public List<JobVehicleSpawn> jobVehicleSpawns = new List<JobVehicleSpawn>();
        public CollectibleManager collectibleManager = new CollectibleManager();
        public List<PetrolStation> petrolStations = new List<PetrolStation>();
        public Refinery refinery;
        public CommandsManager commands;
        private Vector3 defaultSpawn = new Vector3(1894.2115f, 3715.0637f, 32.762226f);
        private TowTrucks towTruck;
        private Junkyard junkyard;
        private Hunter hunter;
        private Lawnmowing lawnmowing;
        private SellVehicle sellVehicle = new SellVehicle();
        //private Supplier supplier = new Supplier(new Vector3(-17.305935f, 6303.6406f, 31.37465f));
        public NicknameChange nicknameChange = new NicknameChange();
        public CarTrader carTrader = new CarTrader(new Vector3(-1565.0809f, -554.6135f, 114.57642), 123f, new Vector3(-1567.3447f, -555.9636f, 114.44851f));
        internal static CultureInfo CustomCulture = new CultureInfo("pl-PL");
        public Houses houses;
        private DebrisCleaner debrisCleaner;
        private Warehouse warehouse;
        private Forklifts forklifts;
        private DoorManager doorManager = new DoorManager();
        private AntiCheat antiCheat = new AntiCheat();
        private DataManager dataManager = new DataManager();
        LSPD lspd;
        //VehicleWheels vehicleWheels = new VehicleWheels(new Vector3(446.45886f, -1777.846f, 29.069387f));
        List<TuneBusiness> tuneBusinesses;
        private FisherMan fisherMan = new FisherMan();
        private DrivingLicences drivingLicences = new DrivingLicences();
        public List<Report> reportsList = new List<Report>();
        public OrgManager orgManager;
        //CarMarket carMarket;
        //Karting karting = new Karting(new Vector3(-1616.593f, -881.3603f, 9.443093f), new Vector3(-1618.2606f, -876.4926f, 9.508349f), new Vector3(-1624.71f, -884.8388f, 9.170503f), 45.0f, new Vector3(-1656.3325f, -889.42786f, 8.739723f));
        List<GTANetworkAPI.Object> objList = new List<GTANetworkAPI.Object>();
        string currentWeather = "EXTRASUNNY";
        int secondsPassed = 0;
        public Vehicle ShowVeh = null;
        private Diver diver;
        Gardener gardener;

        //-------------SERVER EVENTY-------------

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {

            CultureInfo ci = new CultureInfo("pl-PL");
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            CustomCulture.NumberFormat.NumberDecimalSeparator = ",";
            Thread.CurrentThread.CurrentCulture = CustomCulture;
            Thread.CurrentThread.CurrentUICulture = CustomCulture;
            CultureInfo.DefaultThreadCurrentCulture = CustomCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CustomCulture;
            Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ",";
            CultureInfo.DefaultThreadCurrentCulture.NumberFormat.NumberDecimalSeparator = ",";
            CultureInfo.DefaultThreadCurrentUICulture.NumberFormat.NumberDecimalSeparator = ",";
            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Util.ConsoleOutput("Server started...");
            NAPI.Server.SetDefaultSpawnLocation(defaultSpawn, heading: 125f);
            NAPI.Server.SetGlobalServerChat(false);
            peds.CreateDepartmentPed(new Vector3(-1561.8259f, -559.97095f, 114.57642), 110f, "Zmiana nicku");
            orgManager = new OrgManager();
            CreatePublicVehicles();
            new Banking();
            new ItemShops();
            CreatePetrolStations();
            CreateVehicleMechanics();
            CreateChangingRooms();
            CreateVehicleStorages();
            CreatePaintShops();
            CreateTeleports();
            CreateCarWashes();
            CreateCarDealers();
            CreateJobVehicles();
            payoutManager.SetNewBonus();

            tuneBusinesses = new List<TuneBusiness>()
            {
                new TuneBusiness(1, new Vector3(-2149.3696f, -386.9429f, 14.115926f), new Vector3(-2137.951f, -381.41757f, 14.174999f), new Vector3(-2138.4968f, -388.5696f, 14.115928f), new Vector3(-2142.7688f, -379.78958f, 14.175088f), new Vector3(-2149.2578f, -379.46912f, 14.115923f), new Vector3(-2142.151f, -382.7191f, 14.115902f)),
                new TuneBusiness(2, new Vector3(298.72745f, -697.9284f, 29.81644f), new Vector3(293.53754f, -686.1317f, 29.87553f), new Vector3(300.83325f, -686.9971f, 29.816448f), new Vector3(291.95392f, -690.9179f, 29.875538f), new Vector3(291.2251f, -697.3684f, 29.816442f), new Vector3(295.93387f, -690.6306f, 29.81649f)),
                new TuneBusiness(3, new Vector3(430.7077f, 2605.0051f, 45.047375f), new Vector3(423.46164f, 2594.4587f, 45.093903f), new Vector3(420.38037f, 2600.8064f, 45.04738f), new Vector3(428.37003f, 2595.4326f, 45.11502f), new Vector3(434.2113f, 2598.2717f, 45.04734f), new Vector3(426.36078f, 2597.379f, 45.047413f))
            };



            diver = new Diver(ref playerDataManager);
            new VehicleVisu(new Vector3(-211.95407f, -1324.0376f, 30.890387f));
            forklifts = new Forklifts(new Vector3(-553.21564f, -2359.1633f, 13.716812f));
            warehouse = new Warehouse(new Vector3(-87.66176f, 6494.595f, 32.100685f));
            debrisCleaner = new DebrisCleaner(new Vector3(-1705.2916f, -994.86456f, 6.161489f));
            lawnmowing = new Lawnmowing(new Vector3(-1348.476f, 142.67131f, 56.437782f));
            hunter = new Hunter(new Vector3(-1490.5903f, 4981.5283f, 63.345905f), new Vector3(-1493.011f, 4971.482f, 63.92059f), 91.5f);
            junkyard = new Junkyard(new Vector3(2403.5667f, 3127.8855f, 48.15293f), new Vector3(2400.5923f, 3125.3843f, 48.153015f), -163.50455f);
            towTruck = new TowTrucks(new Vector3(593.4641f, -3043.515f, 6.1697326f), new Vector3(561.56323f, -3037.9924f, 6.091237f));
            gardener = new Gardener(ref playerDataManager, ref vehicleDataManager);
            refinery = new Refinery(ref playerDataManager);
            lspd = new LSPD(ref playerDataManager, ref vehicleDataManager);
            payoutManager.bonusTime = DateTime.Now.AddHours(2);
            towTruck.addSpawningPosition(new Vector3(516.8819f, -3054.159f, 6.0696325f), 1);
            towTruck.addSpawningPosition(new Vector3(509.95694f, -3054.1301f, 6.0696325f), 1);
            //carMarket = new CarMarket(new Vector3(-1680.5989f, 62.772358f, 64.01972f), ref orgManager);
            //carMarket.CreateMarketVehicles();
            houses = new Houses();
            NAPI.World.RequestIpl("vw_casino_main");
            NAPI.World.RequestIpl("vw_casino_garage");
            NAPI.World.RequestIpl("vw_casino_carpark");
            NAPI.World.RequestIpl("vw_casino_penthouse");
            drivingLicences.CreateLicenceB(new Vector3(1001.5448f, -2317.2195f, 30.962214f), new Vector3(1003.244f, -2317.394f, 30.962214f), new Vector3(1007.4232f, -2318.8247f, 30.963396f));
            //Dictionary<string, float> damage = new Dictionary<string, float>()
            //{
            //};
            //Console.WriteLine(JsonSerializer.Serialize(damage));

            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += ValuesToDB;
            timer.Enabled = true;

            System.Timers.Timer messages = new System.Timers.Timer(1800000);
            messages.Elapsed += InfoMessages;
            messages.Enabled = true;

            System.Timers.Timer refreshHousesAndPenalties = new System.Timers.Timer(60000);
            refreshHousesAndPenalties.Elapsed += RefreshHousesAndPenalties;
            refreshHousesAndPenalties.Enabled = true;

            // System.Timers.Timer manageQueues = new System.Timers.Timer(5000);
            // manageQueues.Elapsed += ManageQueues;
            // manageQueues.Enabled = true;

            System.Timers.Timer updateWashTimesAndStorage = new System.Timers.Timer(60000);
            updateWashTimesAndStorage.Elapsed += UpdateCarWashAndStorage;
            updateWashTimesAndStorage.Enabled = true;

            System.Timers.Timer timeHandler = new System.Timers.Timer(1000);
            timeHandler.Elapsed += TimeHandler;
            timeHandler.Enabled = true;

            System.Timers.Timer weatherHandler = new System.Timers.Timer(300000);
            weatherHandler.Elapsed += WeatherHandler;
            weatherHandler.Enabled = true;

            System.Timers.Timer carDealerRoll = new System.Timers.Timer(60000);
            carDealerRoll.Elapsed += CarDealerRoll;
            carDealerRoll.Enabled = true;

            System.Timers.Timer setNewBonus = new System.Timers.Timer(7200000);
            setNewBonus.Elapsed += SetNewBonus;
            setNewBonus.Enabled = true;

            System.Timers.Timer updateArrestTimes = new System.Timers.Timer(60000);
            updateArrestTimes.Elapsed += UpdateArrestTimes;
            updateArrestTimes.Enabled = true;

            System.Timers.Timer respawnPublicVehicles = new System.Timers.Timer(60000);
            respawnPublicVehicles.Elapsed += RespawnPublicVehicles;
            respawnPublicVehicles.Enabled = true;

            CreateF1Track();

            commands = new CommandsManager(houses, carDealers, reportsList, playerDataManager, ref orgManager, ref tuneBusinesses, ref lspd, ref payoutManager);

            NAPI.Task.Run(() =>
            {
                vehicleDataManager.LoadPersonalVehiclesFromDB(ref orgManager);
            }, delayTime: 5000);

            Thread awaitInput = new Thread(new ThreadStart(AwaitInput));
            awaitInput.Start();
        }

        public void AwaitInput()
        {
            while (true)
            {
                string text = Console.ReadLine();
                if(text.Length > 0)
                {
                    if (text[0] != '/')
                    {
                        playerDataManager.SendRemoteMessageToAllPlayers(text);
                    }
                    else
                    {
                        commands.ExecuteConsoleCommand(text);
                    }
                }
            }
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

        [RemoteEvent("setPlayersControlsBlocked")]
        public void SetPlayersControlsBlocked(Player player, bool value)
        {
            player.SetSharedData("controlsblocked", value);
        }

        [RemoteEvent("setVehiclesLights")]
        public void setVehiclesLights(Player player, Vehicle vehicle)
        {
            if (!vehicle.HasSharedData("veh_lights"))
            {
                vehicle.SetSharedData("veh_lights", true);
                playerDataManager.NotifyPlayer(player, "Swiatła będą zawsze włączone!");
            }
            else
            {
                if (vehicle.GetSharedData<bool>("veh_lights"))
                {
                    vehicle.SetSharedData("veh_lights", false);
                    playerDataManager.NotifyPlayer(player, "Swiatła będą zawsze wyłączone!");
                }
                else
                {
                    vehicle.SetSharedData("veh_lights", true);
                    playerDataManager.NotifyPlayer(player, "Swiatła będą zawsze włączone!");
                }
            }
        }

        //ADMIN CONTEXT

        [RemoteEvent("admin_deleteCar")]
        public void AdminDeleteCar(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") != "dealer")
                {
                    if (vehicle.GetSharedData<string>("type") == "personal")
                    {
                        vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                        playerDataManager.NotifyPlayer(player, $"Pojazd o ID: {vehicle.GetSharedData<Int32>("id").ToString()} został przeniesiony do przechowalni!");
                        vehicle.Delete();
                    }
                    else if (vehicle.GetSharedData<string>("type") == "lspd")
                    {
                        lspd.SetVehicleSpawned(vehicle.GetSharedData<int>("id"), false);
                        playerDataManager.NotifyPlayer(player, $"Pojazd został usunięty!");
                        vehicle.Delete();
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, $"Pojazd został usunięty!");
                        vehicle.Delete();
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie można usunąć pojazdu z komisu!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }

        [RemoteEvent("admin_bringCar")]
        public void AdminBringCar(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") != "dealer")
                {
                    if (vehicle.GetSharedData<string>("type") == "personal")
                    {
                        vehicle.SetSharedData("lastpos", player.Position);
                        vehicle.Position = player.Position;
                        if (vehicle.GetSharedData<bool>("veh_brake"))
                        {
                            vehicle.SetSharedData("veh_brake", false);
                            NAPI.Task.Run(() =>
                            {
                                vehicle.SetSharedData("veh_brake", true);
                                vehicleDataManager.UpdateVehiclesLastPos(vehicle);
                            }, 5000);
                        }
                        else
                        {
                            vehicleDataManager.UpdateVehiclesLastPos(vehicle);
                        }
                    }
                    else
                    {
                        vehicle.Position = player.Position;
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie można przenieść pojazdu z komisu!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }

        [RemoteEvent("admin_lastDriver")]
        public void AdminLastDriver(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("admin_lastDriver"))
                {
                    playerDataManager.SendInfoToPlayer(player, "Ostatni kierowca pojazdu: " + vehicle.GetSharedData<string>("admin_lastDriver"));
                }
                else if (vehicle.HasSharedData("drivers"))
                {
                    playerDataManager.SendInfoToPlayer(player, "Ostatni kierowcy pojazdu: " + vehicle.GetSharedData<string>("drivers"));
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Pojazd nie miał kierowcy!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }

        [RemoteEvent("admin_carOwner")]
        public void AdminCarOwner(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("id"))
                {
                    string vehowner = vehicleDataManager.GetVehiclesOwnerName(vehicle.GetSharedData<int>("id").ToString());
                    if (vehowner != "")
                    {
                        playerDataManager.SendInfoToPlayer(player, "Pojazd należy do " + vehowner);
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie odnaleziono właściciela!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Pojazd nie ma właściciela!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }

        [RemoteEvent("admin_flipCar")]
        public void AdminFlipCar(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                Vector3 rotation = vehicle.Rotation;
                vehicle.Position = new Vector3(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z + 1);
                vehicle.Rotation = new Vector3(0, 0, rotation.Z);
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }

        [RemoteEvent("admin_fixCar")]
        public void AdminFixCar(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("owner"))
                    vehicleDataManager.RepairVehicle(vehicle);
                else
                {
                    vehicle.Repair();
                }

            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }

        //SETTINGS

        [RemoteEvent("savePlayerSettings")]
        public void SavePlayerSettings(Player player, string set)
        {
            playerDataManager.UpdatePlayersSettings(player, set);
            Settings settings = JsonConvert.DeserializeObject<Settings>(player.GetSharedData<string>("settings"));
            player.TriggerEvent("settings_setSpeedometerScale", settings.SpeedometerSize);
            player.TriggerEvent("settings_setHUDScales", settings.HudSize, settings.ChatSize);
        }

        [RemoteEvent("resetPlayerSettings")]
        public void ResetPlayerSettings(Player player)
        {
            playerDataManager.SetPlayersSettings(player);
            Settings settings = JsonConvert.DeserializeObject<Settings>(player.GetSharedData<string>("settings"));
            player.TriggerEvent("settings_setSpeedometerScale", settings.SpeedometerSize);
            player.TriggerEvent("settings_setHUDScales", settings.HudSize, settings.ChatSize);
        }

        //LSPD
        [RemoteEvent("lspd_StartDuty")]
        public void LSPDStartDuty(Player player)
        {
            lspd.SwitchPlayersDuty(player, true);
        }
        [RemoteEvent("lspd_createBarrier")]
        public void LSPDCreateBarrier(Player player, Vector3 position, Vector3 rotation)
        {
            lspd.CreateBarrier(player.GetSharedData<string>("username"), position, rotation);
        }
        [RemoteEvent("lspd_removeClosestBarrier")]
        public void LSPDRemoveClosestBarrier(Player player)
        {
            if (player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty") && player.Vehicle == null)
            {
                if (lspd.Barriers.Count > 0)
                {
                    KeyValuePair<GTANetworkAPI.Object, string> closestBarrier = new KeyValuePair<GTANetworkAPI.Object, string>();

                    foreach (KeyValuePair<GTANetworkAPI.Object, string> obj in lspd.Barriers)
                    {
                        if (closestBarrier.Equals(new KeyValuePair<GTANetworkAPI.Object, string>()) && obj.Key.Exists && obj.Key.Position.DistanceTo(player.Position) < 2)
                        {
                            closestBarrier = obj;
                        }
                        else if (closestBarrier.Key != null && closestBarrier.Key.Exists && obj.Key != null && obj.Key.Exists && player.Position.DistanceTo(obj.Key.Position) < player.Position.DistanceTo(closestBarrier.Key.Position))
                        {
                            closestBarrier = obj;
                        }
                    }
                    if (!closestBarrier.Equals(new KeyValuePair<GTANetworkAPI.Object, string>()))
                    {
                        string name = lspd.RemoveBarrier(closestBarrier);
                        playerDataManager.SendInfoToPlayer(player, "Usunięto barierkę stworzoną przez: " + name);
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "W pobliżu nie ma żadnej barierki!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "W pobliżu nie ma żadnej barierki!");
                }
            }
        }
        [RemoteEvent("lspd_cuffClosestPlayer")]
        public void LSPDCuffClosestPlayer(Player player)
        {
            Player p = playerDataManager.GetClosestCivilianToCuff(player.Position, 1);
            if (p != null)
            {
                if (p.HasSharedData("handCuffed") && p.GetSharedData<bool>("handCuffed") && p.GetSharedData<Player>("cuffedBy") == player)
                {
                    p.TriggerEvent("handCuff", player, false);
                    p.SetSharedData("handCuffed", false);
                    player.SetSharedData("cuffedPlayer", 0);
                    playerDataManager.NotifyPlayer(player, "Rozkuto  " + p.GetSharedData<string>("username"));
                    playerDataManager.NotifyPlayer(p, "Zostałeś rozkuty");
                }
                else if (!(p.HasSharedData("handCuffed") && p.GetSharedData<bool>("handCuffed")))
                {
                    if (!(player.HasSharedData("cuffedPlayer") && player.GetSharedData<Player>("cuffedPlayer") != null && player.GetSharedData<Player>("cuffedPlayer").Exists))
                    {
                        p.TriggerEvent("handCuff", player, true);
                        playerDataManager.NotifyPlayer(p, "Zostałeś zakuty przez " + player.GetSharedData<string>("username"));
                        playerDataManager.NotifyPlayer(player, "Zakuto " + p.GetSharedData<string>("username"));
                        p.SetSharedData("cuffedBy", player.Handle);
                        p.SetSharedData("handCuffed", true);
                        player.SetSharedData("cuffedPlayer", p.Handle);
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Zakuć możesz tylko jednego gracza!");
                    }
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono gracza lub jest on za daleko!");
            }
        }

        [RemoteEvent("setCuffedPlayerIntoVeh")]
        public void SetCuffedPlayerIntoVeh(Player player, Vehicle vehicle, Player cuffed, int seat)
        {
            if (vehicle != null && vehicle.Exists && cuffed != null && cuffed.Exists)
            {
                cuffed.SetIntoVehicle(vehicle.Handle, seat);
            }
        }

        [RemoteEvent("warpCuffedPlayerOutOfVeh")]
        public void WarpCuffedPlayerOutOfVeh(Player player, Player cuffed)
        {
            if (cuffed != null && cuffed.Exists && cuffed.Vehicle != null)
            {
                cuffed.WarpOutOfVehicle();
                cuffed.TriggerEvent("setCuffed", player);
                NAPI.ClientEvent.TriggerClientEventForAll("setPlayerCuffed", cuffed, player);
            }
        }

        [RemoteEvent("lspd_openStorage")]
        public void LSPDOpenStorage(Player player)
        {
            if (player.HasSharedData("lspd_power") && player.GetSharedData<int>("lspd_power") > 0)
            {
                string stor = lspd.GetAvailableVehicles(player.GetSharedData<int>("lspd_power"));
                if (stor.Length > 0)
                {
                    player.TriggerEvent("openLspdStorageBrowser", stor);
                }

            }
        }
        [RemoteEvent("lspd_CreateVehicle")]
        public void LSPDCreateVehicle(Player player, int id)
        {
            if (lspd.SpawnVehicle(id))
            {
                playerDataManager.NotifyPlayer(player, "Pomyślnie wyjęto pojazd!");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie udało się wyjąć pojazdu!");
            }
        }
        //Przelewy
        [RemoteEvent("transferMoneyToPlayer")]
        public void TransferMoneyToPlayer(Player player, Player target, int money, string title)
        {
            if (target != null && target.Exists)
            {
                if (target.Position.DistanceTo(player.Position) <= 10)
                {
                    if (playerDataManager.UpdatePlayersMoney(player, -1 * money))
                    {
                        playerDataManager.UpdatePlayersMoney(target, money);
                        playerDataManager.SendInfoToPlayer(target, "Otrzymano przelew od " + player.GetSharedData<string>("username") + " o kwocie $" + money.ToString() + ". Tytuł: " + title + ".");
                        playerDataManager.SaveTransferToDB(player, target, money, title);
                        player.TriggerEvent("closeMoneyTransferBrowser");
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Błąd przelewu: nie masz tyle gotówki!");
                        player.TriggerEvent("closeMoneyTransferBrowser");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Błąd przelewu: gracz się oddalił!");
                    player.TriggerEvent("closeMoneyTransferBrowser");
                }

            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Błąd przelewu: gracz opuścił serwer!");
                player.TriggerEvent("closeMoneyTransferBrowser");
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

        //KARTING
        // [RemoteEvent("signPlayerForARace")]
        // public void SignPlayerForARace(Player player, string race)
        // {
        //     switch (race)
        //     {
        //         case "karting":
        //             karting.SignForARace(player);
        //             break;
        //     }
        // }
        // [RemoteEvent("deleteKart")]
        // public void deleteKart(Player player, Vehicle kart)
        // {
        //     if (kart != null && kart.Exists)
        //     {
        //         karting.StopRace(player);
        //         kart.Delete();
        //     }
        // }
        // [RemoteEvent("endKartingRace")]
        // public void endKartingRace(Player player, float time)
        // {
        //     karting.CheckPosition(player, time);
        //     playerDataManager.SendInfoToPlayer(player, $"Udało ci się zakończyć wyścig z czasem {time.ToString()} sekund!");
        // }
        [RemoteEvent("playerCommandHandler")]
        public void PlayerCommandHandler(Player player, string command, string args)
        {
            commands.ExecuteCommand(player, command, args);
        }
        //Item SHOPS
        [RemoteEvent("itemShopBuyItem")]
        public void ItemShopBuyItem(Player player, int itemId)
        {
            player.TriggerEvent("checkIfItemFits", itemId, "shop");
        }


        [RemoteEvent("removeDrowningVehicle")]
        public void RemoveDrowningVehicle(Player player, Vehicle vehicle)
        {
            if(vehicle != null && vehicle.Exists)
            {
                vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                vehicleDataManager.UpdateVehiclesDamage(vehicle, vehicleDataManager.wreckedDamage);
                vehicle.Delete();
            }
            if(player != null && player.Exists)
            {
                playerDataManager.NotifyPlayer(player, "Twój pojazd został zatopiony, możesz go odebrać w przechowalni!");
            }
        }

        //REFINERY
        [RemoteEvent("refinery_startJob")]
        public void Refinery_StartJob(Player player)
        {
            refinery.StartJob(player);
        }

        [RemoteEvent("refinery_selectJobType")]
        public void Refinery_SelectJobType(Player player, int jobLevel)
        {
            int stationsCount = jobLevel == 1 ? 3 : jobLevel == 2 ? 5 : jobLevel == 3 ? 7 : 0;
            Dictionary<int, float> stationIndexes = new Dictionary<int, float>();
            List<Vector3> stations = new List<Vector3>();
            for(int i = 0; i < stationsCount; i++)
            {
                Random rnd = new Random();
                Vector3 petrolStation = petrolStations[rnd.Next(0, petrolStations.Count)].vehiclePosition;
                while(stations.Contains(petrolStation))
                {
                    petrolStation = petrolStations[rnd.Next(0, petrolStations.Count)].vehiclePosition;
                }
                stations.Add(petrolStation);
            }
            Dictionary<Vector3, float> stationValues = new Dictionary<Vector3, float>();
            foreach(Vector3 stationPos in stations)
            {
                Random rnd = new Random();
                var value = (float)Math.Round((rnd.NextDouble() * (1500 - 1000) + 1000));
                stationValues.Add(stationPos, value);
                stationIndexes.Add(petrolStations.IndexOf(petrolStations.Find(ps => ps.vehiclePosition == stationPos)), value);
            }
            player.TriggerEvent("refinery_setNewStations", JsonConvert.SerializeObject(stationValues), jobLevel, JsonConvert.SerializeObject(stationIndexes));
        }

        [RemoteEvent("refinery_openPumpingPanel")]
        public void Refinery_OpenPumpingPanel(Player player, Vehicle vehicle)
        {
            if(vehicle != null && vehicle.Exists && vehicle.HasSharedData("oiltank"))
            {
                if(vehicle.GetSharedData<float>("oiltank") < 5000)
                {
                    foreach (OilPump oilPump in refinery.oilPumps)
                    {
                        if (oilPump.Colshape.IsPointWithin(player.Position))
                        {
                            if (oilPump.OilAmount > 0)
                            {
                                player.TriggerEvent("refinery_openBrowser", vehicle.GetSharedData<float>("oiltank"), vehicle, refinery.oilPumps.IndexOf(oilPump));
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Ten szyb jest pusty!");
                            }
                            return;
                        }
                    }
                    playerDataManager.NotifyPlayer(player, "Nie jesteś w pobliżu żadnego szybu naftowego!");
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Pojazd jest pełny!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Wystąpił błąd!");
            }
        }

        [RemoteEvent("refinery_addOil")]
        public void Refinery_AddOil(Player player, Vehicle vehicle, int pumpIndex, float amount)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("oiltank"))
            {
                if (vehicle.GetSharedData<float>("oiltank") < 5000)
                {
                    var oilPump = refinery.oilPumps[pumpIndex];
                    if (oilPump.OilAmount > 0)
                    {
                        var oil = vehicle.GetSharedData<float>("oiltank");
                        if(oilPump.OilAmount - amount <= 0)
                        {
                            amount = oilPump.OilAmount;
                            oilPump.UpdateOilAmount(0);
                        }
                        else
                        {
                            oilPump.UpdateOilAmount(oilPump.OilAmount - amount);
                        }
                        if (oil + amount >= 5000)
                        {
                            oil = 5000.0f;
                        }
                        else
                        {
                            oil += amount;
                        }
                        vehicle.SetSharedData("oiltank", oil);
                        player.TriggerEvent("refinery_refreshTank", oil);
                        player.TriggerEvent("refinery_refreshStationValues");
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Ten szyb jest pusty!");
                        player.TriggerEvent("refinery_closeBrowser");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Pojazd jest pełny!");
                    player.TriggerEvent("refinery_closeBrowser");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Wystąpił błąd!");
                player.TriggerEvent("refinery_closeBrowser");
            }
        }
        [RemoteEvent("refinery_updateTruckOil")]
        public void Refinery_UpdateTruckOil(Player player, Vehicle vehicle, float amount)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("oiltank"))
            {
                vehicle.SetSharedData("oiltank", amount);
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Wystąpił błąd!");
                player.TriggerEvent("refinery_closeBrowser");
            }
        }

        [RemoteEvent("refinery_payment")]
        public void Refinery_Payment(Player player, int liters, int type, string currentOrder)
        {
            payoutManager.RefineryPayment(player, liters, type);
            dataManager.UpdatePetrolStationValues(currentOrder);
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

        //Banking
        [RemoteEvent("depositBankMoney")]
        public void DepositBankMoney(Player player, int money)
        {
            if (playerDataManager.DepositMoney(player, money))
            {
                player.TriggerEvent("setBankingVars", player.GetSharedData<int>("bank"));
                playerDataManager.NotifyPlayer(player, "Pomyślnie wpłacono gotówkę!");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie posiadasz takiej ilości gotówki!");
            }
        }


        [RemoteEvent("withdrawBankMoney")]
        public void WithdrawBankMoney(Player player, int money)
        {
            if (playerDataManager.WithdrawMoney(player, money))
            {
                player.TriggerEvent("setBankingVars", player.GetSharedData<int>("bank"));
                playerDataManager.NotifyPlayer(player, "Pomyślnie wypłacono gotówkę!");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie masz tyle gotówki na koncie!");
            }
        }

        //driving licence
        [RemoteEvent("licenceCheckMoney")]
        public void LicenceCheckMoney(Player player, int money)
        {
            if (playerDataManager.UpdatePlayersMoney(player, -1 * money))
            {
                player.TriggerEvent("startLicenceTest");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie stać Cię na rozpoczęcie tego egzaminu!");
            }
        }

        [RemoteEvent("startTestLicence")]
        public void StartTestLicence(Player player)
        {
            drivingLicences.StartLicenceB(player);
        }

        [RemoteEvent("licenceBpassed")]
        public void LicenceBPassed(Player player)
        {
            player.SetSharedData("job", "");
            playerDataManager.NotifyPlayer(player, "Udało Ci się ukończyć egzamin praktyczny Kat. B z wynikiem pozytywnym!");
            player.Position = drivingLicences.endPosition;
            if (player.HasSharedData("jobveh") && player.GetSharedData<int>("jobveh") != -1111)
            {
                var veh = vehicleDataManager.GetVehicleByRemoteId(Convert.ToUInt16(player.GetSharedData<Int32>("jobveh")));
                if (veh != null && veh.Exists)
                    veh.Delete();
                player.SetSharedData("jobveh", -1111);
            }
            player.SetSharedData("licenceBp", true);
            playerDataManager.SavePlayerDataToDB(player, "licenceBp");
        }

        [RemoteEvent("licenceBfailed")]
        public void LicenceBFailed(Player player)
        {
            player.SetSharedData("job", "");
            player.Position = drivingLicences.endPosition;
            if (player.HasSharedData("jobveh") && player.GetSharedData<int>("jobveh") != -1111)
            {
                var veh = vehicleDataManager.GetVehicleByRemoteId(Convert.ToUInt16(player.GetSharedData<Int32>("jobveh")));
                if (veh != null && veh.Exists)
                    veh.Delete();
                player.SetSharedData("jobveh", -1111);

            }
            if (drivingLicences.currentPlayerPassing == player)
            {
                drivingLicences.currentPlayerPassing = null;
            }
        }

        [RemoteEvent("playerLeftPassingArea")]
        public void PlayerLeftPassingArea(Player player)
        {
            if (drivingLicences.currentPlayerPassing == player)
            {
                drivingLicences.currentPlayerPassing = null;
            }
        }

        [RemoteEvent("licenceCompleted")]
        public void LicenceCompleted(Player player)
        {
            player.SetSharedData("licenceBt", true);
            playerDataManager.SavePlayerDataToDB(player, "licenceBt");
        }


        [RemoteEvent("createShowroomVehicle")]
        public void CreateShowroomVehicle(Player player, string model)
        {
            if (ShowVeh != null && ShowVeh.Exists)
            {
                ShowVeh.Delete();
            }
            ShowVeh = NAPI.Vehicle.CreateVehicle(uint.Parse(model), new Vector3(227.8945f, -1000.1026f, -98.99988f), -42f, 131, 131);
            ShowVeh.Rotation = new Vector3(0, 0, -42f);
        }

        [RemoteEvent("saveShowroomVehicle")]
        public void SaveShowroomVehicle(Player player)
        {
            if (ShowVeh != null && ShowVeh.Exists)
            {
                File.AppendAllText(@"notes.txt", ShowVeh.Model.ToString() + "," + Environment.NewLine, new UTF8Encoding(false, true));
                playerDataManager.NotifyPlayer(player, "Notatka zapisana w pliku!");
            }
        }
        //BIZNES - TUNE

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
                        if (playerDataManager.UpdatePlayersMoney(player, -2000000))
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
                    if (tuneBusiness.WheelStationVeh != null && tuneBusiness.WheelStationVeh.Exists && tuneBusiness.WheelsShape.IsPointWithin(tuneBusiness.WheelStationVeh.Position))
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
                        player.TriggerEvent("openWheelsTuneBrowser", businessId, JsonConvert.SerializeObject(availableWheels), vehicleDataManager.GetVehiclesWheels(tuneBusiness.WheelStationVeh), tuneBusiness.WheelStationVeh);
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
                    if (tuneBusiness.WheelStationVeh != null && tuneBusiness.WheelStationVeh == vehicle)
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
                    if (tuneBusiness.Id == businessId && tuneBusiness.WheelStationVeh == vehicle && tuneBusiness.WheelsShape.IsPointWithin(vehicle.Position) && vehicle.Occupants.Count > 0)
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

        //biznes tune - mechaniczny
        [RemoteEvent("openBusinessMechStation")]
        public void OpenBusinessMechStation(Player player, int businessId)
        {
            foreach (TuneBusiness tuneBusiness in tuneBusinesses)
            {
                if (tuneBusiness.Id == businessId)
                {
                    if (tuneBusiness.MechStationVeh != null && tuneBusiness.MechStationVeh.Exists && tuneBusiness.MechColshape.IsPointWithin(tuneBusiness.MechStationVeh.Position))
                    {
                        player.TriggerEvent("openMechTuneBrowser", businessId, vehicleDataManager.GetVehiclesMechTune(tuneBusiness.MechStationVeh), tuneBusiness.MechStationVeh);
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
                        if (tuneBusiness.MechStationVeh == vehicle)
                        {
                            if (tuneBusiness.MechColshape.IsPointWithin(vehicle.Position))
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

        //item sync
        [RemoteEvent("putItemInHand")]
        public void PutItemInHand(Player player, string item)
        {
            if (player.HasSharedData("handObj") && player.GetSharedData<string>("handObj") != "")
            {
                player.SetSharedData("handObj", "");
            }
            else
            {
                player.SetSharedData("handObj", item);
            }
        }
        [RemoteEvent("removeItemFromHand")]
        public void RemoveItemFromHand(Player player)
        {
            if (player.HasSharedData("handObj") && player.GetSharedData<string>("handObj") != "")
            {
                player.SetSharedData("handObj", "");
            }
        }

        //cool clothes
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

        //character creator
        [RemoteEvent("createCharacter")]
        public void CreateCharacter(Player player, string charStr)
        {
            charStr = charStr.Remove(0, 1);
            charStr = charStr.Remove(charStr.Length - 1, 1);
            charStr.Replace(@"\", "");
            playerDataManager.UpdatePlayersCharacter(player, charStr);
            player.Dimension = 0;
            SpawnSelected(player, "ls");
            player.SetSharedData("gui", false);
            playerDataManager.SetPlayersClothes(player);
        }




        //report
        [RemoteEvent("markReportAsSolved")]
        public void MarkReportAsSolved(Player player, string reportId)
        {
            bool solved = false;
            foreach (Report report in reportsList)
            {
                if (report.id.ToString().Equals(reportId))
                {
                    if (report.informer.Exists)
                    {
                        playerDataManager.SendInfoToPlayer(report.informer, $"Twój raport o ID {report.id.ToString()} został przyjęty przez {player.GetSharedData<string>("username")}");
                    }
                    solved = true;
                    playerDataManager.SendInfoToPlayer(player, $"Raport przyjęty: {report.id.ToString()}: {report.informerName} => {report.reportedName}, powód: {report.description}");
                    reportsList.Remove(report);
                    break;
                }
            }
            if (!solved)
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono raportu!");
        }

        [RemoteEvent("removeReport")]
        public void RemoveReport(Player player, string reportId)
        {
            bool removed = false;
            foreach (Report report in reportsList)
            {
                if (report.id.ToString().Equals(reportId))
                {
                    removed = true;
                    playerDataManager.SendInfoToPlayer(player, $"Raport usunięty: {report.id.ToString()}: {report.informerName} => {report.reportedName}, powód: {report.description}");
                    reportsList.Remove(report);
                    break;
                }
            }
            if (!removed)
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono raportu!");
        }

        [RemoteEvent("requireReports")]
        public void RequireReports(Player player)
        {
            List<List<string>> reports = new List<List<string>>();
            foreach (Report report in reportsList)
            {
                reports.Add(new List<string>()
                {
                    report.id.ToString(),
                    report.informerName,
                    report.reportedName,
                    report.description,
                    report.time.ToString()
                });
            }
            string reportString = JsonConvert.SerializeObject(reports);
            player.TriggerEvent("insertReportsToBrowser", reportString);

        }
        //felgi
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

        //tune
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

        [RemoteEvent("endJob")]
        public void EndJob(Player player)
        {
            player.SetSharedData("job", "");
            playerDataManager.NotifyPlayer(player, "Praca zakończona");
            player.SetSharedData("handObj", "");
            player.TriggerEvent("stopJob");
            player.RemoveAllWeapons();
            if (player.HasSharedData("jobveh") && player.GetSharedData<int>("jobveh") != -1111)
            {
                var veh = vehicleDataManager.GetVehicleByRemoteId(Convert.ToUInt16(player.GetSharedData<Int32>("jobveh")));
                if (veh != null && veh.Exists)
                    veh.Delete();
                player.SetSharedData("jobveh", -1111);

            }
            player.SetSharedData("jobveh", -1111);
            player.TriggerEvent("closeGardenerHUDBrowser");
            playerDataManager.SetJobClothes(player, false, "");
        }
        [RemoteEvent("tpWPVeh")]
        public void TpWPVeh(Player player, Vector3 point)
        {
            if (player.Vehicle != null)
            {
                player.Vehicle.Position = point;
            }
        }

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
            if(player.GetSharedData<string>("job") != "")
            {
                if (player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty"))
                {
                    lspd.SwitchPlayersDuty(player, true);
                }
                else
                {
                    EndJob(player);
                }
            }
            playerDataManager.SetJobClothes(player, false, "");
        }

        [ServerEvent(Event.VehicleDeath)]
        public void OnVehicleDeath(Vehicle vehicle)
        {
            // if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "personal")
            // {
            //     vehicleDataManager.UpdateVehiclesDamage(vehicle, vehicleDataManager.wreckedDamage);
            //     vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
            //     vehicle.Delete();
            // }
        }

        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player)
        {
            logManager.CreatePlayersDirectories(player.SocialClubId.ToString());
            try
            {
                //if (playerDataManager.IsWhiteListed(player))
                //{
                Console.WriteLine(player.Address + " " + player.SocialClubName);
                playerDataManager.SetPlayerDataFromDB(player);
                playerDataManager.setUsersPenalties(player);
                bool house = houses.SetPlayersHouse(player);
                lspd.IsPlayerInGroup(player);
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
                    playerDataManager.SetPlayersClothes(player);
                    if (!lspd.IsPlayerArrested(player))
                    {
                        player.TriggerEvent("openSpawnSelection", house);
                        player.SetSharedData("gui", true);
                    }
                    else
                    {
                        playerDataManager.SetPlayersConnectValues(player, currentWeather);
                        lspd.ReturnPlayerIntoArrest(player);
                        playerDataManager.NotifyPlayer(player, "Jesteś aresztowany! Pozostało " + player.GetSharedData<int>("arrested_time").ToString() + " minut aresztu!");
                    }
                }
                doorManager.SetDoorsForPlayer(player);
                orgManager.SetPlayersOrg(player);

                //}
                //else
                //{
                //    logManager.LogLoginInfo(player.SocialClubId.ToString(), $"Próba dołączenia bez WL, Nick: {player.SocialClubName}, IP: {player.Address}");
                //    player.Kick("WL");
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        [RemoteEvent("sendInfoMessage")]
        public void SendInfoMessage(Player player, string message)
        {
            playerDataManager.SendInfoToPlayer(player, message);
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seatId)
        {
            player.SetSharedData("vehSeat", (int)(byte)seatId);
            if (!antiCheat.ShouldPlayerEnterThisVehicle(player, vehicle))
            {
                player.TriggerEvent("openTrollBrowser", "rick");
                player.WarpOutOfVehicle();
                return;
            }
            if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "jobveh" && !( vehicle.HasSharedData("player") && vehicle.GetSharedData<bool>("player")))
            {
                vehicle.SetSharedData("player", true);
                player.SetSharedData("jobveh", vehicle.Id);
                vehicle.SetSharedData("veh_brake", false);
                foreach(JobVehicleSpawn vehSpawn in jobVehicleSpawns)
                {
                    if(vehSpawn.Veh == vehicle)
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
                //carMarket.RemoveVehicleFromMarket(vehicle);
            }
            if (vehicle.HasSharedData("drivers") && seatId == 0)
            {
                vehicleDataManager.SaveVehiclesDriver(vehicle, player);
            }
            player.SetSharedData("seatbelt", false);
        }
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnect(Player player, DisconnectionType type, string reason)
        {
            logManager.LogLoginInfo(player.SocialClubId.ToString(), $"Opuszczono serwer z IP: {player.Address}");
            if (player.HasSharedData("handObj") && player.GetSharedData<string>("handObj") != "")
            {
                player.SetSharedData("handObj", "");
            }
            if (player.HasSharedData("jobveh") && player.GetSharedData<int>("jobveh") != -1111)
            {
                var veh = vehicleDataManager.GetVehicleByRemoteId(Convert.ToUInt16(player.GetSharedData<Int32>("jobveh")));
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
                lspd.SpawnPlayerInArrest(player);
            }
            if (player.HasSharedData("handCuffed") && player.GetSharedData<bool>("handCuffed") && player.GetSharedData<Player>("cuffedBy").Exists)
            {
                player.Position = player.GetSharedData<Player>("cuffedBy").Position;
            }
            else
            {
                playerDataManager.SpawnPlayerAtClosestHospital(player);
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
                    vehicleDataManager.UpdateVehiclesUsedTime(vehicle);
                }
                if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "lspd")
                {
                    if (lspd.StorageCenter.IsPointWithin(vehicle.Position))
                    {
                        if (!vehicleDataManager.IsVehicleDamaged(vehicle))
                        {
                            if (vehicle.HasSharedData("petroltank") && vehicle.GetSharedData<float>("petrol") / vehicle.GetSharedData<int>("petroltank") >= 0.9f)
                            {
                                lspd.SetVehicleSpawned(vehicle.GetSharedData<int>("id"), false);
                                playerDataManager.NotifyPlayer(player, $"Pojazd został przeniesiony na parking!");
                                vehicle.Delete();
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Pojazd musi być zatankowany w przynajmniej 90%!");
                            }
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Pojazd jest za bardzo uszkodzony!");
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
                                vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                                playerDataManager.NotifyPlayer(player, $"Pojazd o ID: {vehicle.GetSharedData<Int32>("id").ToString()} został przeniesiony do przechowalni!");
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
                playerDataManager.NotifyPlayer(player, "Masz minutę na powrót do pojazdu!");
            }
        }

        [ServerEvent(Event.PlayerEnterColshape)]
        public void OnPlayerEnterColshape(ColShape shape, Player player)
        {
            if(player.Vehicle != null && player.Vehicle.HasSharedData("petrol") && player.VehicleSeat == 0 && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "stationveh")
            {
                foreach(PetrolStation petrolStation in petrolStations)
                {
                    if(petrolStation.vehicleColShape == shape)
                    {
                        if(petrolStation.currentVehicle != null && petrolStation.currentVehicle.Exists && petrolStation.vehicleColShape.IsPointWithin(petrolStation.currentVehicle.Position))
                        {
                            playerDataManager.NotifyPlayer(player, "To stanowisko jest zajęte!");
                        }
                        else
                        {
                            if(petrolStation.currentVehicle != null && petrolStation.currentVehicle.Exists)
                            {
                                petrolStation.currentVehicle.SetSharedData("petrol_onStation", false);
                            }
                            petrolStation.currentVehicle = player.Vehicle;
                            player.Vehicle.SetSharedData("petrol_onStation", true);
                            playerDataManager.NotifyPlayer(player, "Udaj się do dystrybutora aby rozpocząć proces tankowania!");
                        }
                        break;
                    }
                }
            }
            if (shape.HasSharedData("pair"))
            {
                playerDataManager.NotifyPlayer(player, shape.GetSharedData<string>("name"));
            }
            // --------------Colshape przechowalni-----------------
            if (player.Vehicle != null && player.Vehicle.Exists && player.GetSharedData<int>("vehSeat") == 0)
            {
                if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "storagein")
                {
                    playerDataManager.NotifyPlayer(player, "Zaparkuj tu pojazd aby schować go do przechowalni!");
                }
                if (player.Vehicle.GetSharedData<string>("type") == "lspd" && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "lspd_storageCenter")
                {
                    playerDataManager.NotifyPlayer(player, "Zaparkuj tu pojazd aby schować go na parking policyjny!");
                }
            }
            if (player.Vehicle != null && player.Vehicle.Exists && player.Vehicle.HasSharedData("type") && player.Vehicle.GetSharedData<string>("type") == "personal" && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "business-wheels")
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Id == shape.GetSharedData<int>("business-id"))
                    {
                        tuneBusiness.WheelStationVeh = player.Vehicle;
                        break;
                    }
                }
            }
            if (player.Vehicle != null && player.Vehicle.Exists && player.Vehicle.HasSharedData("type") && player.Vehicle.GetSharedData<string>("type") == "personal" && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "business-mech")
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Id == shape.GetSharedData<int>("business-id"))
                    {
                        tuneBusiness.MechStationVeh = player.Vehicle;
                        break;
                    }
                }
            }
            if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "lspd_mainGate" && player.Vehicle != null && player.GetSharedData<int>("vehSeat") == 0 && player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty"))
            {
                lspd.SwitchMainGate(true);
            }
            else if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "lspd_backGate" && player.Vehicle != null && player.GetSharedData<int>("vehSeat") == 0 && player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty"))
            {
                lspd.SwitchBackGate(true);
            }
        }

        [ServerEvent(Event.PlayerExitColshape)]
        public void OnPlayerExitColShape(ColShape shape, Player player)
        {
            if(shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "stationveh" && player.Vehicle != null && player.Vehicle.HasSharedData("petrol_onStation") && player.VehicleSeat == 0)
            {
                player.Vehicle.SetSharedData("petrol_onStation", false);
                player.Vehicle.SetSharedData("petrol_refueling", false);
                foreach (PetrolStation petrolStation in petrolStations)
                {
                    if(petrolStation.vehicleColShape == shape && petrolStation.currentVehicle == player.Vehicle)
                    {
                        petrolStation.currentVehicle = null;
                        break;
                    }
                }
            }
            if(shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "fishingspot" && player.GetSharedData<string>("job") == "fisherman")
            {
                EndJob(player);
            }
            if (player.Vehicle != null && player.Vehicle.Exists && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "visutune")
            {
                vehicleDataManager.RefreshVehiclesTune(player.Vehicle);
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
            if (player.Vehicle != null && player.Vehicle.Exists && player.Vehicle.HasSharedData("type") && player.Vehicle.GetSharedData<string>("type") == "personal" && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "business-wheels")
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Id == shape.GetSharedData<int>("business-id"))
                    {
                        tuneBusiness.WheelStationVeh = null;
                        vehicleDataManager.SetVehiclesWheels(player.Vehicle);
                        break;
                    }
                }
            }
            if (player.Vehicle != null && player.Vehicle.Exists && player.Vehicle.HasSharedData("type") && player.Vehicle.GetSharedData<string>("type") == "personal" && shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "business-mech")
            {
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Id == shape.GetSharedData<int>("business-id"))
                    {
                        tuneBusiness.MechStationVeh = null;
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
                lspd.SwitchMainGate(false);
            }
            else if (shape.HasSharedData("type") && shape.GetSharedData<string>("type") == "lspd_backGate" && player.Vehicle != null && player.GetSharedData<int>("vehSeat") == 0 && player.HasSharedData("lspd_duty") && player.GetSharedData<bool>("lspd_duty"))
            {
                lspd.SwitchBackGate(false);
            }
        }

        [RemoteEvent("freezePublicVehicle")]
        public void FreezePublicVehicle(Player player, Vehicle vehicle, bool value)
        {
            vehicle.SetSharedData("veh_brake", value);

        }

        //waypointy
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


        //USUWANIE POJAZDU
        [RemoteEvent("removeIndicatedVehicle")]
        public void RemovePointedVehicle(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") != "dealer" && vehicle.GetSharedData<string>("type") != "public")
            {
                if (vehicle.GetSharedData<string>("type") == "personal")
                {
                    vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                    playerDataManager.NotifyPlayer(player, $"Pojazd o ID: {vehicle.GetSharedData<Int32>("id").ToString()} został przeniesiony do przechowalni!");
                    vehicle.Delete();
                }
                else if (vehicle.GetSharedData<string>("type") == "lspd")
                {
                    lspd.SetVehicleSpawned(vehicle.GetSharedData<int>("id"), false);
                    playerDataManager.NotifyPlayer(player, $"Pojazd został usunięty!");
                    vehicle.Delete();
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, $"Pojazd został usunięty!");
                    vehicle.Delete();
                }
            }
        }
        //SPALANIE

        [RemoteEvent("savePetrolLevel")]
        public void SavePetrolLevel(Player player, Vehicle vehicle, float petrol)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("petrol"))
            {
                if (vehicle.HasSharedData("owner"))
                    vehicleDataManager.UpdateVehiclesPetrol(vehicle, petrol);
                vehicle.SetSharedData("petrol", petrol);
            }
        }

        [RemoteEvent("petrol_checkStation")]
        public void Petrol_CheckStation(Player player, ColShape distributor)
        {
            foreach(PetrolStation petrolStation in petrolStations)
            {
                if(petrolStation.distributorColShape == distributor)
                {
                    if(petrolStation.currentVehicle != null)
                    {
                        if(!(petrolStation.currentVehicle.Exists && petrolStation.currentVehicle.HasSharedData("petrol_refueling") && petrolStation.currentVehicle.GetSharedData<bool>("petrol_refueling")))
                        {
                            if (petrolStation.currentVehicle.Exists)
                            {
                                petrolStation.currentVehicle.SetSharedData("petrol_refueling", true);
                                player.TriggerEvent("petrol_startRefueling", petrolStation.currentVehicle, dataManager.GetPetrolPriceAtIndex(petrolStations.IndexOf(petrolStation)));
                            }
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie można rozpocząć tankowania!");
                        }
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Na stanowisku nie ma odpowedniego pojazdu!");
                    }
                    break;
                }
            }
        }

        [RemoteEvent("petrol_cancelRefueling")]
        public void Petrol_CancelRefueling(Player player, Vehicle vehicle)
        {
            if(vehicle != null && vehicle.Exists)
            {
                vehicle.SetSharedData("petrol_refueling", false);
            }
        }

        [RemoteEvent("petrol_stopRefueling")]
        public void Petrol_StopRefueling(Player player, ColShape shape, Vehicle vehicle, int cost, float petrol)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if(playerDataManager.UpdatePlayersMoney(player, -1 * cost))
                {
                    vehicleDataManager.UpdateVehiclesPetrol(vehicle, Math.Clamp(vehicle.GetSharedData<float>("petrol") + petrol, 0, vehicle.GetSharedData<int>("petroltank")));
                    playerDataManager.NotifyPlayer(player, "Pomyślnie zatankowano pojazd!");
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie stać Cię na to!");
                }
                vehicle.SetSharedData("petrol_refueling", false);
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }

        //Giełda pojazdów
        //[RemoteEvent("applyCarToMarket")]
        //public void ApplyCarToMarket(Player player, Vehicle vehicle, int price, string description)
        //{
        //    if (vehicle != null && vehicle.Exists)
        //    {
        //        if (!vehicleDataManager.IsVehicleDamaged(vehicle))
        //        {
        //            if (carMarket.AddVehicleToMarket(vehicle, price, description, player.GetSharedData<string>("username")))
        //            {
        //                playerDataManager.NotifyPlayer(player, "Pomyślnie wystawiono pojazd na giełdę!");
        //            }
        //            else
        //            {
        //                playerDataManager.NotifyPlayer(player, "Na giełdzie nie ma wolnych miejsc!");
        //            }
        //        }
        //        else
        //        {
        //            playerDataManager.NotifyPlayer(player, "Pojazd jest uszkodzony!");
        //        }

        //    }
        //}

        //Handel pojazdami

        [RemoteEvent("openCarTraderBrowser")]
        public void OpenCarTraderBrowser(Player player)
        {
            string cars = vehicleDataManager.GetPlayersVehicles(player, false, null);
            Dictionary<int, string> players = new Dictionary<int, string>();
            foreach (Player p in NAPI.Pools.GetAllPlayers())
            {
                if (player.Position.DistanceTo(p.Position) < 10.0f && p != player)
                {
                    players.Add(p.GetSharedData<Int32>("id"), p.GetSharedData<string>("username"));
                }
            }
            string playerstr = "";
            if (players.Count != 0)
            {
                playerstr = JsonConvert.SerializeObject(players);
            }
            player.TriggerEvent("openCarTraderBrowser", playerstr, cars);
        }

        [RemoteEvent("sendTrade")]
        public void SendTrade(Player player, int playerId, int carId, int price)
        {
            if (vehicleDataManager.GetVehiclesOwner(carId.ToString()) == player.GetSharedData<string>("socialclub"))
            {
                Player p = playerDataManager.GetPlayerByRemoteId(playerId.ToString());
                if (p != null && p.Position.DistanceTo(player.Position) < 10.0f)
                {
                    if ((p.HasSharedData("tradeOffer") && !p.GetSharedData<bool>("tradeOffer")) || !p.HasSharedData("tradeOffer"))
                    {
                        p.SetSharedData("tradeOffer", true);
                        string[] vehicleData = vehicleDataManager.GatherVehiclesInfo(carId);
                        p.TriggerEvent("carTrade_openBrowser", player, carId, price, vehicleData[0], vehicleData[1], vehicleData[2]);
                        playerDataManager.NotifyPlayer(player, "Oferta wysłana!");
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Gracz otrzymał już inną ofertę!");
                    }

                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Gracz oddalił się od punktu sprzedaży pojazdów!");
                }


            }
        }

        [RemoteEvent("carTrade_confirmTrade")]
        public void CarTrade_ConfirmTrade(Player player, Player seller, int carId, int price)
        {
            if (seller != null && seller.Exists && vehicleDataManager.GetVehiclesOwner(carId.ToString()) == seller.GetSharedData<string>("socialclub"))
            {
                if (player.Position.DistanceTo(seller.Position) < 10.0f)
                {
                    if (playerDataManager.HasPlayerFreeSlot(player))
                    {
                        if (player != null && seller != null && player.Exists && seller.Exists && playerDataManager.UpdatePlayersMoney(player, -1 * price))
                        {
                            playerDataManager.UpdatePlayersMoney(seller, price);
                            vehicleDataManager.UpdateVehiclesDBOwner(carId, player.GetSharedData<string>("socialclub"));
                            Vehicle veh = vehicleDataManager.GetVehicleById(carId.ToString());
                            if (veh != null)
                            {
                                veh.SetSharedData("owner", player.SocialClubId);
                            }
                            playerDataManager.NotifyPlayer(player, "Pomyślnie zakupiono pojazd!");
                            playerDataManager.NotifyPlayer(seller, "Pomyślnie sprzedano pojazd!");
                            player.SetSharedData("tradeOffer", false);
                            logManager.LogVehicleTrade(player.SocialClubId.ToString(), $"Zakupiono pojazd o ID {carId.ToString()} za {price.ToString()}$ od {seller.SocialClubId.ToString()}");
                            logManager.LogVehicleTrade(seller.SocialClubId.ToString(), $"Sprzedano pojazd o ID {carId.ToString()} za {price.ToString()}$ graczowi {player.SocialClubId.ToString()}");
                            foreach (Organization org in orgManager.orgs)
                            {
                                if (org.vehicles.Contains(carId) || org.vehicleRequests.Contains(carId))
                                {
                                    if (org.vehicles.Contains(carId))
                                    {
                                        org.vehicles.Remove(carId);
                                    }
                                    if (org.vehicleRequests.Contains(carId))
                                    {
                                        org.vehicleRequests.Remove(carId);
                                    }
                                    org.SaveOrgToDataBase();
                                    break;
                                }
                            }
                            if (veh != null)
                            {
                                veh.SetSharedData("orgId", 0);
                            }
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie stać cię na to!");
                        }
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie masz wolnych slotów na pojazdy!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Gracz się oddalił!");
                }
            }
        }
        [RemoteEvent("setTradeOffer")]
        public void SetTradeOffer(Player player, bool state)
        {
            player.SetSharedData("tradeOffer", state);
        }
        //ZMIANA NICKU
        [RemoteEvent("changeNickname")]
        public void ChangeNickname(Player player, string nickname)
        {
            if (playerDataManager.UpdatePlayersNickname(player, nickname))
            {
                player.TriggerEvent("closeNicknameBrowser");
                playerDataManager.NotifyPlayer(player, "Pomyślnie zmieniono nick!");
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

        //Skills

        

        //EQUIPMENT
        [RemoteEvent("updateEquipment")]
        public void UpdateEquipment(Player player, string equipmentString)
        {
            playerDataManager.UpdatePlayersEquipment(player, equipmentString);
        }

        [RemoteEvent("useItem")]
        public void UseItem(Player player, int typeId, int itemId)
        {
            switch (typeId)
            {
                case 0:
                case 1:
                    playerDataManager.NotifyPlayer(player, "Zostałeś uleczony!");
                    player.TriggerEvent("removeEqItem", itemId);
                    int newHealt = Math.Clamp(player.Health + 50, 50, 100);
                    player.Health = newHealt;
                    break;
                case 2:
                    playerDataManager.NotifyPlayer(player, "Zostałeś uleczony!");
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
                    playerDataManager.NotifyPlayer(player, "Skóry możesz sprzedać u myśliwego!");
                    break;
                case 11:
                    if(player.Vehicle == null)
                    {
                        Vehicle closest = vehicleDataManager.GetClosestVehicle(player);
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
                                playerDataManager.NotifyPlayer(player, "Pojazd pomyślnie zatankowany!");
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Tego pojazdu nie można zatankować!");
                            }
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie stoisz w pobliżu żadnego pojazdu!");
                        }
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie możesz być w pojeździe aby tego użyć!");
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
                            playerDataManager.NotifyPlayer(player, "Podniosłeś fioletowego bratka!");
                            break;
                        case 901:
                            playerDataManager.NotifyPlayer(player, "Podniosłeś różowego bratka!");
                            break;
                        case 902:
                            playerDataManager.NotifyPlayer(player, "Podniosłeś żółtego bratka!");
                            break;
                        case 903:
                            playerDataManager.NotifyPlayer(player, "Podniosłeś figowca!");
                            break;
                        case 904:
                            playerDataManager.NotifyPlayer(player, "Podniosłeś dracenę!");
                            break;
                    }
                    player.TriggerEvent("fitItemInEquipment", itemId);
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Roślina nie zmieściła się do Twojego ekwipunku!");
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
                    if (playerDataManager.UpdatePlayersMoney(player, -1 * cost))
                    {
                        player.TriggerEvent("addItemToEquipment", itemId);
                        playerDataManager.NotifyPlayer(player, "Pomyślnie zakupiono przedmiot!");
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie stać Cię na ten przedmiot!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie zmieścisz tego przedmiotu do ekwipunku!");
                }
            }
            else
            {
                if (state)
                {
                    droppedItemsManager.ConfirmPickingUp(player, UInt64.Parse(type));
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie zmieścisz tego przedmiotu do ekwipunku!");
                }
            }

        }

        [RemoteEvent("pickItemUp")]
        public void PickItemUp(Player player, string type)
        {
            droppedItemsManager.PickItemUp(player, UInt64.Parse(type));
        }


        [RemoteEvent("dropItem")]
        public void DropItem(Player player, int typeId, string itemName)
        {
            if (typeId == 1000 && !playerDataManager.HasItem(player, 1000))
            {
                EndJob(player);
            }
            droppedItemsManager.AddItem(player.Position - new Vector3(0, 0, 1), typeId, itemName);
        }

        [RemoteEvent("updateEquipments")]
        public void UpdateEquipments(Player player, string equipment1, string equipment2, string eqId, Vehicle vehicle = null)
        {
            playerDataManager.UpdatePlayersEquipment(player, equipment1);
            if (eqId.Contains('v'))
            {
                eqId = eqId.Remove(0, 1);
                int id;
                if (Int32.TryParse(eqId, out id))
                {
                    Vehicle veh = vehicleDataManager.GetVehicleById(id.ToString());
                    if (veh != null)
                    {
                        vehicleDataManager.UpdateVehiclesTrunk(veh, equipment2);
                    }
                }
            }
            else if (eqId.Contains('h'))
            {
                eqId = eqId.Remove(0, 1);
                int id;
                if (Int32.TryParse(eqId, out id))
                {
                    foreach (House house in houses.houses)
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



        //Collectibles
        [RemoteEvent("collectiblePickedUp")]
        public void CollectiblePickedUp(Player player, int id)
        {
            if (!collectibleManager.IsCollectiblePickedUp(player, id))
            {
                System.Object[] vals = collectibleManager.RemovePlayersCollectible(player, id);
                Dictionary<int, bool> collectible = (Dictionary<int, bool>)vals[0];
                int found = (int)vals[1];
                playerDataManager.UpdatePlayersCollectibles(player, JsonConvert.SerializeObject(collectible));
                playerDataManager.NotifyPlayer(player, $"Znalazłeś jajo {found} z {collectibleManager.collectibleCount}!");
                playerDataManager.UpdatePlayersMoney(player, 100);
                playerDataManager.UpdatePlayersExp(player, 100);
            }
            else
            {
                Console.WriteLine("podwójna znajdźka: " + player.SocialClubId.ToString() + " " + id.ToString());
            }
        }
        //LAKIERNIA
        [RemoteEvent("showVehicleColor")]
        public void ShowVehicleColor(Player player, Vehicle vehicle, string color1string, string color2string, string defaultMods)
        {
            if (color1string != "" && color2string != "" && defaultMods == "")
            {
                int[] color1 = vehicleDataManager.JsonToColor(color1string);
                int colorMod1 = vehicleDataManager.JsonToColorMod(color1string);
                int[] color2 = vehicleDataManager.JsonToColor(color2string);
                int colorMod2 = vehicleDataManager.JsonToColorMod(color2string);
                NAPI.Vehicle.SetVehicleCustomPrimaryColor(vehicle.Handle, color1[0], color1[1], color1[2]);
                NAPI.Vehicle.SetVehicleCustomSecondaryColor(vehicle.Handle, color2[0], color2[1], color2[2]);
                vehicle.SetSharedData("color1mod", colorMod1);
                vehicle.SetSharedData("color2mod", colorMod2);
            }
            else
            {
                try
                {
                    int[] color1 = vehicleDataManager.JsonToColor(vehicle.GetSharedData<string>("color1"));
                    int[] color2 = vehicleDataManager.JsonToColor(vehicle.GetSharedData<string>("color2"));
                    NAPI.Vehicle.SetVehicleCustomPrimaryColor(vehicle.Handle, color1[0], color1[1], color1[2]);
                    NAPI.Vehicle.SetVehicleCustomSecondaryColor(vehicle.Handle, color2[0], color2[1], color2[2]);
                    int[] colorMods = System.Text.Json.JsonSerializer.Deserialize<int[]>(defaultMods);
                    vehicle.SetSharedData("color1mod", colorMods[0]);
                    vehicle.SetSharedData("color2mod", colorMods[1]);

                }
                catch { }
            }
        }

        [RemoteEvent("changeVehicleColor")]
        public void ChangeVehicleColor(Player player, Vehicle vehicle, string color1string, string color2string)
        {
            float price = 3000.0f;
            int[] color1 = vehicleDataManager.JsonToColor(color1string);
            int colorMod1 = vehicleDataManager.JsonToColorMod(color1string);
            int[] color2 = vehicleDataManager.JsonToColor(color2string);
            int colorMod2 = vehicleDataManager.JsonToColorMod(color2string);
            if (playerDataManager.UpdatePlayersMoney(player, -1 * Convert.ToInt32(price)))
            {
                vehicleDataManager.UpdateVehiclesColor1(vehicle, color1[0], color1[1], color1[2], colorMod1);
                vehicleDataManager.UpdateVehiclesColor2(vehicle, color2[0], color2[1], color2[2], colorMod2);
                playerDataManager.NotifyPlayer(player, "Kolor pomyślnie zmieniony!");
                player.TriggerEvent("closePaintShopBrowser");
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie stać cię na to!");
            }
        }


        //CHANGING ROOM
        [RemoteEvent("disablePlayerControls")]
        public void DisablePlayerControls(Player player, bool value)
        {
            player.SetSharedData("disablecontrols", value);
        }

        [RemoteEvent("showSkin")]
        public void showPlayerSkin(Player player, string idSkina)
        {
            int d;
            if (int.TryParse(idSkina, out d))
            {
                string pedstring = playerDataManager.GetPedHashById(idSkina);
                player.SetSkin(NAPI.Util.GetHashKey(pedstring));
            }
            else
            {
                player.SetSkin(Convert.ToUInt32(player.GetSharedData<Int64>("ped")));
            }
        }

        [RemoteEvent("changeSkin")]
        public void changePlayersSkin(Player player, string idSkina)
        {
            string pedstring = playerDataManager.GetPedHashById(idSkina);
            playerDataManager.UpdatePlayersPed(player, NAPI.Util.GetHashKey(pedstring));
        }

        [RemoteEvent("setGui")]
        public void SetGui(Player player, bool state)
        {
            player.SetSharedData("gui", state);
        }
        //Info Panel
        [RemoteEvent("mainPanel_requestData")]
        public void MainPanel_requestData(Player player)
        {
            string vehiclesData = vehicleDataManager.GetPlayersVehicles(player);
            string playersData = playerDataManager.GetPlayersInfo(player);
            string settingsData = player.GetSharedData<string>("settings");
            string skillsData = playerDataManager.GetPlayersSkills(player);
            player.TriggerEvent("mainPanel_setData", playersData, skillsData, vehiclesData, settingsData, (playerDataManager.time.Hour.ToString().Length == 1 ? ("0" + playerDataManager.time.Hour.ToString()) : playerDataManager.time.Hour.ToString()) + ":" + (playerDataManager.time.Minute.ToString().Length == 1 ? ("0" + playerDataManager.time.Minute.ToString()) : playerDataManager.time.Minute.ToString()));
        }

        [RemoteEvent("mainPanel_requestVehicleData")]
        public void MainPanel_requestVehicleData(Player player, int id)
        {
            string[] vehicleData = vehicleDataManager.GatherVehiclesInfo(id);
            player.TriggerEvent("mainPanel_setVehicleData", vehicleData[0], vehicleData[1], vehicleData[2]);
        }

        [RemoteEvent("mainPanel_addSkillPoint")]
        public void MainPanel_AddSkillPoint(Player player, int skill)
        {
            playerDataManager.UpgradePlayersSkill(player, skill);
            int[] skills = new int[5];
            for (int i = 0; i < 5; i++)
            {
                skills[i] = player.GetSharedData<Int32>("skill-" + i.ToString());
            }
            player.TriggerEvent("mainPanel_setSkillsToUpgrade", player.GetSharedData<Int32>("skillpoints"), JsonConvert.SerializeObject(skills));
        }

        //SKUP POJAZDÓW
        [RemoteEvent("sellVeh_open")]
        public void SellVeh_Open(Player player)
        {
            //if(player.GetSharedData<int>("power") >= 2)
            //{
                if(player.Vehicle != null)
                {
                    DateTime last = DateTime.Parse(player.GetSharedData<string>("vehsold"));
                    if (last.AddHours(12) <= DateTime.Now)
                    {
                        if(player.Vehicle.HasSharedData("owner") && player.Vehicle.GetSharedData<Int64>("owner").ToString() == player.SocialClubId.ToString())
                        {
                            int price = vehicleDataManager.GetVehiclesSellPrice(player.Vehicle);
                            player.TriggerEvent("sellVeh_openBrowser", player.Vehicle, player.Vehicle.GetSharedData<string>("name"), price.ToString(), player.Vehicle.GetSharedData<float>("veh_trip").ToString());
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie jesteś właścicielem tego pojazdu!");
                        }
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie możesz jeszcze sprzedać pojazdu, będzie to możliwe " + last.AddHours(12).ToString() + ".");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Aby z tego korzystać musisz być w pojeździe!");
                }
            //}
            //else
            //{
            //    playerDataManager.NotifyPlayer(player, "Ta funkcja chwilowo jest dostępna tylko dla testerów!");
            //}
            
        }

        [RemoteEvent("sellVeh_sellVehicle")]
        public void SellVeh_SellVehicle(Player player, Vehicle vehicle, int price)
        {
            if(vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("owner") && vehicle.GetSharedData<Int64>("owner").ToString() == player.SocialClubId.ToString())
                {
                    vehicleDataManager.UpdateVehiclesOwner(vehicle, 0);
                    vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                    int carId = vehicle.GetSharedData<int>("id");
                    vehicle.Delete();
                    foreach (Organization org in orgManager.orgs)
                    {
                        if (org.vehicles.Contains(carId) || org.vehicleRequests.Contains(carId))
                        {
                            if (org.vehicles.Contains(carId))
                            {
                                org.vehicles.Remove(carId);
                            }
                            if (org.vehicleRequests.Contains(carId))
                            {
                                org.vehicleRequests.Remove(carId);
                            }
                            org.SaveOrgToDataBase();
                            break;
                        }
                    }
                    if (playerDataManager.UpdatePlayersMoney(player, price))
                    {
                        playerDataManager.NotifyPlayer(player, "Pomyślnie sprzedano pojazd! Kolejna sprzedaż będzie możliwa " + DateTime.Now.AddHours(12).ToString() + ".");
                        playerDataManager.UpdateVehicleSold(player, DateTime.Now.ToString());
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Wystąpił błąd!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Nie jesteś właścicielem tego pojazdu!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }


        //handling

        [RemoteEvent("saveVehiclesSettings")]
        public void SaveVehiclesSettings(Player player, string settings)
        {
            File.AppendAllText(@"handlings.txt", settings + Environment.NewLine, new UTF8Encoding(false, true));
            playerDataManager.NotifyPlayer(player, "Notatka zapisana w pliku!");
        }

        //DAMAGE
        [RemoteEvent("updateVehicleDamage")]
        public void UpdateVehicleDamage(Player player, Vehicle vehicle, string damageString)
        {
            vehicleDataManager.UpdateVehiclesDamage(vehicle, damageString);
        }

        //ORGANIZACJE
        [RemoteEvent("openOrgBrowser")]
        public void OpenOrgBrowser(Player player)
        {
            if (!player.HasSharedData("orgId") || (player.HasSharedData("orgId") && player.GetSharedData<Int32>("orgId") == 0))
            {
                List<string[]> orgs = new List<string[]>();
                foreach (Organization org in orgManager.orgs)
                {
                    orgs.Add(new string[]
                    {
                        org.name,
                        org.tag,
                        (org.members.Count).ToString(),
                        org.id.ToString()
                    });
                }

                player.TriggerEvent("openOrgBrowser", JsonConvert.SerializeObject(orgs));
            }
            else if (player.HasSharedData("orgOwner") && player.GetSharedData<bool>("orgOwner"))
            {
                foreach (Organization org in orgManager.orgs)
                {
                    if (org.owner == player.SocialClubId)
                    {
                        List<List<string[]>> data = org.GetOrgData(player.SocialClubId);

                        player.TriggerEvent("openManageOrgBrowser", JsonConvert.SerializeObject(data[0]), JsonConvert.SerializeObject(data[1]), JsonConvert.SerializeObject(data[2]), JsonConvert.SerializeObject(data[3]), JsonConvert.SerializeObject(data[4]));
                        break;
                    }
                }
            }
            else if (player.HasSharedData("orgId") && player.GetSharedData<Int32>("orgId") != 0 && (!player.HasSharedData("orgOwner") || (player.HasSharedData("orgOwner") && !player.GetSharedData<bool>("orgOwner"))))
            {
                foreach (Organization org in orgManager.orgs)
                {
                    if (org.id == player.GetSharedData<int>("orgId"))
                    {
                        List<List<string[]>> data = org.GetOrgDataForMember(player.SocialClubId);

                        player.TriggerEvent("openMemberOrgBrowser", JsonConvert.SerializeObject(data[0]), JsonConvert.SerializeObject(data[1]), JsonConvert.SerializeObject(data[2]), JsonConvert.SerializeObject(data[3]));
                        break;
                    }
                }
            }
        }

        [RemoteEvent("sendOrgRequest")]
        public void SendOrgRequest(Player player, int orgId)
        {
            if (!player.HasSharedData("orgId") || (player.HasSharedData("orgId") && player.GetSharedData<Int32>("orgId") == 0))
            {
                if (player.GetSharedData<Int32>("money") >= 500)
                {
                    if (orgManager.AddRequestToOrg(player.SocialClubId, orgId))
                    {
                        playerDataManager.NotifyPlayer(player, "Pomyślnie złożono podanie do organizacji!");
                        playerDataManager.UpdatePlayersMoney(player, -500);
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Złożyłeś już podanie do tej organizacji!");
                    }
                }
                else
                {
                    playerDataManager.NotifyPlayer(player, "Aby złożyć podanie do organizacji potrzebujesz $500!");
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Należysz już do organizacji!");
            }

        }
        [RemoteEvent("createOrg")]
        public void CreateOrg(Player player, string name, string tag)
        {
            if (player.GetSharedData<Int32>("money") >= 35000)
            {
                int org = orgManager.CreateOrg(player, name, tag);
                if (org == 0)
                {
                    player.TriggerEvent("closeOrgBrowser");
                    playerDataManager.NotifyPlayer(player, "Organizacja została pomyślnie utworzona!");
                    playerDataManager.UpdatePlayersMoney(player, -35000);
                }
                else if (org == 1)
                {
                    player.TriggerEvent("orgBrowserError", "Istnieje już organizacja z taką nazwą!");
                }
                else if (org == 2)
                {
                    player.TriggerEvent("orgBrowserError", "Istnieje już organizacja z takim tagiem!");
                }
            }
            else
            {
                player.TriggerEvent("orgBrowserError", "Potrzebujesz $35000!");
            }
        }

        [RemoteEvent("answerMemberRequest")]
        public void AnswerMemberRequest(Player player, string id, bool state)
        {
            ulong ID;
            if (ulong.TryParse(id, out ID))
            {
                foreach (Organization org in orgManager.orgs)
                {
                    if (org.owner == player.SocialClubId)
                    {
                        if (state)
                        {
                            if (orgManager.AcceptRequest(ID, org.id))
                            {
                                playerDataManager.NotifyPlayer(player, "Pomyślnie dodano członka do organizacji!");
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Ten członek dołączył już do innej organizacji!");
                            }
                        }
                        else
                        {
                            if (orgManager.RemoveRequestFromOrg(ID, org.id))
                            {
                                playerDataManager.NotifyPlayer(player, "Pomyślnie odrzucono podanie!");
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Nie odnaleziono podania!");
                            }
                        }
                        List<List<string[]>> data = org.GetOrgData(player.SocialClubId);
                        player.TriggerEvent("refreshManageOrgData", JsonConvert.SerializeObject(data[0]), JsonConvert.SerializeObject(data[1]), JsonConvert.SerializeObject(data[2]), JsonConvert.SerializeObject(data[3]), JsonConvert.SerializeObject(data[4]));
                        break;
                    }
                }
            }
        }

        [RemoteEvent("answerVehicleRequest")]
        public void AnswerVehicleRequest(Player player, string id, bool state)
        {
            int ID;
            if (int.TryParse(id, out ID))
            {
                foreach (Organization org in orgManager.orgs)
                {
                    if (org.owner == player.SocialClubId)
                    {
                        if (state)
                        {
                            if (orgManager.AcceptVehicleRequest(ID, org.id))
                            {
                                playerDataManager.NotifyPlayer(player, "Pomyślnie dodano pojazd do organizacji!");
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
                            }
                        }
                        else
                        {
                            if (orgManager.RemoveVehicleRequest(ID, org.id))
                            {
                                playerDataManager.NotifyPlayer(player, "Pomyślnie odrzucono pojazd!");
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
                            }
                        }
                        List<List<string[]>> data = org.GetOrgData(player.SocialClubId);
                        player.TriggerEvent("refreshManageOrgData", JsonConvert.SerializeObject(data[0]), JsonConvert.SerializeObject(data[1]), JsonConvert.SerializeObject(data[2]), JsonConvert.SerializeObject(data[3]), JsonConvert.SerializeObject(data[4]));
                        break;
                    }
                }
            }
        }

        [RemoteEvent("removeOrg")]
        public void RemoveOrg(Player player)
        {
            foreach (Organization org in orgManager.orgs)
            {
                if (org.owner == player.SocialClubId)
                {
                    orgManager.DeleteOrg(player.SocialClubId, org.id);
                    player.TriggerEvent("closeManageOrgBrowser");
                    break;
                }
            }
        }

        [RemoteEvent("removeSharedVehicle")]
        public void RemoveSharedVehicle(Player player, string id, string type)
        {
            int ID;
            if (int.TryParse(id, out ID))
            {
                foreach (Organization org in orgManager.orgs)
                {
                    if (org.id == player.GetSharedData<int>("orgId"))
                    {
                        if (orgManager.RemoveVehicleFromOrg(ID, org.id))
                        {
                            playerDataManager.NotifyPlayer(player, "Pomyślnie usunięto pojazd z organizacji!");
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
                        }
                        if (type == "manage")
                        {
                            List<List<string[]>> data = org.GetOrgData(player.SocialClubId);
                            player.TriggerEvent("refreshManageOrgData", JsonConvert.SerializeObject(data[0]), JsonConvert.SerializeObject(data[1]), JsonConvert.SerializeObject(data[2]), JsonConvert.SerializeObject(data[3]), JsonConvert.SerializeObject(data[4]));
                        }
                        else
                        {
                            List<List<string[]>> data = org.GetOrgDataForMember(player.SocialClubId);
                            player.TriggerEvent("refreshMemberOrgData", JsonConvert.SerializeObject(data[0]), JsonConvert.SerializeObject(data[1]), JsonConvert.SerializeObject(data[2]), JsonConvert.SerializeObject(data[3]));
                        }
                        break;
                    }
                }
            }

        }

        [RemoteEvent("shareVehicle")]
        public void ShareVehicle(Player player, string id, string type)
        {
            int ID;
            if (int.TryParse(id, out ID))
            {
                foreach (Organization org in orgManager.orgs)
                {
                    if (org.id == player.GetSharedData<int>("orgId"))
                    {
                        if (orgManager.SendVehicleRequest(player.SocialClubId, ID, org.id))
                        {
                            playerDataManager.NotifyPlayer(player, "Pomyślnie zaproponowano dodanie pojazdu!");
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
                        }
                        if (type == "manage")
                        {
                            List<List<string[]>> data = org.GetOrgData(player.SocialClubId);
                            player.TriggerEvent("refreshManageOrgData", JsonConvert.SerializeObject(data[0]), JsonConvert.SerializeObject(data[1]), JsonConvert.SerializeObject(data[2]), JsonConvert.SerializeObject(data[3]), JsonConvert.SerializeObject(data[4]));
                        }
                        else
                        {
                            List<List<string[]>> data = org.GetOrgDataForMember(player.SocialClubId);
                            player.TriggerEvent("refreshMemberOrgData", JsonConvert.SerializeObject(data[0]), JsonConvert.SerializeObject(data[1]), JsonConvert.SerializeObject(data[2]), JsonConvert.SerializeObject(data[3]));
                        }
                        break;
                    }
                }
            }
        }

        [RemoteEvent("leaveOrg")]
        public void LeaveOrg(Player player)
        {
            foreach (Organization org in orgManager.orgs)
            {
                if (org.id == player.GetSharedData<int>("orgId"))
                {
                    orgManager.RemoveMemberFromOrg(player.SocialClubId, org.id);
                    player.TriggerEvent("closeMemberOrgBrowser");
                    playerDataManager.NotifyPlayer(player, "Pomyślnie opuszczono organizację!");
                    break;
                }
            }
        }

        [RemoteEvent("kickMemberFromOrg")]
        public void KickPlayerFromOrg(Player player, string playerId)
        {
            ulong ID;
            if (ulong.TryParse(playerId, out ID))
            {
                foreach (Organization org in orgManager.orgs)
                {
                    if (org.members.Contains(ID))
                    {
                        orgManager.RemoveMemberFromOrg(ID, org.id);
                        List<List<string[]>> data = org.GetOrgData(player.SocialClubId);
                        playerDataManager.NotifyPlayer(player, "Pomyślnie wyrzucono członka!");
                        player.TriggerEvent("refreshManageOrgData", JsonConvert.SerializeObject(data[0]), JsonConvert.SerializeObject(data[1]), JsonConvert.SerializeObject(data[2]), JsonConvert.SerializeObject(data[3]), JsonConvert.SerializeObject(data[4]));
                        break;
                    }
                }
            }
        }

        [RemoteEvent("setVehicleHorn")]
        public void SetVehicleHorn(Player player, Vehicle vehicle, bool state)
        {
            if (vehicle != null && vehicle.Exists)
                vehicle.SetSharedData("horn", state);
        }

        //manage org


        ////NOWA PRZECHOWALNIA

        [RemoteEvent("spawnSelectedVehicle")]
        public void spawnSelectedVehicle(Player player, string vehicleId, string storageId)
        {
            Vehicle veh = null;
            foreach (VehicleStorage vs in vehicleStorages)
            {
                if (vs.storageId == Convert.ToInt32(storageId))
                {
                    veh = vs.SpawnCar(player, vehicleId);
                    break;
                }
            }
            if (veh != null)
            {
                orgManager.SetVehiclesOrg(veh);
            }
        }

        [RemoteEvent("requestStorageData")]
        public void RequestStorageData(Player player)
        {
            List<int> orgIds = null;
            if (player.HasSharedData("orgId") && player.GetSharedData<Int32>("orgId") != 0)
            {
                foreach (Organization org in orgManager.orgs)
                {
                    if (org.id == player.GetSharedData<Int32>("orgId"))
                    {
                        orgIds = org.vehicles;
                        break;
                    }
                }
            }
            string vehicles = vehicleDataManager.GetPlayersVehicles(player, true, orgIds);
            player.TriggerEvent("insertStorageVehicles", vehicles);
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
        }

        //DIRT

        [RemoteEvent("updateVehiclesDirtLevel")]
        public void updateVehiclesDirtLevel(Player player, Vehicle vehicle, float dirtLevel)
        {
            if (vehicle != null && vehicle.Exists)
                vehicleDataManager.UpdateVehiclesDirtLevel(vehicle, dirtLevel);
        }

        //MECHANIK

        [RemoteEvent("openMechHUD")]
        public void openMechHUD(Player player, ColShape colshape)
        {
            Vehicle stationVehicle = null;
            string error = "";
            foreach (VehicleMechanic vm in vehicleMechanics)
            {
                if (vm.pedColShape == colshape)
                {
                    foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
                    {
                        if (vm.stationColShape.IsPointWithin(veh.Position))
                        {
                            stationVehicle = veh;
                            break;
                        }
                    }
                    if (stationVehicle != null)
                    {
                        if(stationVehicle.HasSharedData("mech") && stationVehicle.GetSharedData<bool>("mech"))
                        {
                            playerDataManager.NotifyPlayer(player, "Pojazd jest już w trakcie naprawy!");
                        }
                        else
                        {
                            player.TriggerEvent("openMechHUD", stationVehicle, vehicleDataManager.GetVehicleModelPrice(stationVehicle), vm.stationColShape);
                        }
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Na stanowisku nie ma żadnego pojazdu!");
                    }
                    break;
                }
            }
        }

        [RemoteEvent("mech_confirmRepair")]
        public void Mech_ConfirmRepair(Player player, Vehicle vehicle, int price, int time, string partsToRepair, ColShape mechColshape)
        {
            if (vehicle != null && vehicle.Exists)
            {
                foreach(VehicleMechanic vehMech in vehicleMechanics)
                {
                    if(vehMech.stationColShape == mechColshape)
                    {
                        if(!vehMech.stationColShape.IsPointWithin(vehicle.Position))
                        {
                            playerDataManager.NotifyPlayer(player, "Pojazd nie znajduje się na stanowisku!");
                            return;
                        }
                    }
                }
                if (vehicle.HasSharedData("mech") && vehicle.GetSharedData<bool>("mech"))
                {
                    playerDataManager.NotifyPlayer(player, "Pojazd jest już w trakcie naprawy!");
                }
                else
                {
                    if (playerDataManager.UpdatePlayersMoney(player, -1 * price))
                    {
                        playerDataManager.NotifyPlayer(player, "Naprawa rozpoczęta!");
                        vehicleDataManager.setRepairingInterval(vehicle, time, player, partsToRepair);
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Nie stać cię na naprawę tego pojazdu!");
                    }
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }

        }

        //DOMKI
        [RemoteEvent("openHouseStorage")]
        public void OpenHouseStorage(Player player, int houseId)
        {
            foreach (House house in houses.houses)
            {
                if (house.id == houseId)
                {
                    player.TriggerEvent("openSecondEquipmentBrowser", player.GetSharedData<string>("equipment"), house.storage, "h" + house.id, JsonConvert.SerializeObject(house.storageSize));
                    break;
                }
            }
        }
        [RemoteEvent("enterHouse")]
        public void EnterHouse(Player player, ColShape house)
        {
            player.Position = house.GetSharedData<Vector3>("interior");
            player.Dimension = (uint)(house.GetSharedData<Int32>("id") + 500);
        }

        [RemoteEvent("leaveHouse")]
        public void LeaveHouse(Player player, ColShape house)
        {
            player.Position = house.GetSharedData<Vector3>("housepos");
            player.Dimension = 0;
        }

        [RemoteEvent("confirmHouseBuy")]
        public void ConfirmHouseBuy(Player player, int houseId, int time)
        {
            foreach (House house in houses.houses)
            {
                if (house.houseColShape.GetSharedData<Int32>("id") == houseId)
                {
                    if (house.owner != "")
                    {
                        playerDataManager.NotifyPlayer(player, "Dom został już wynajęty przez kogoś innego!");
                        player.TriggerEvent("closeHousePanelBrowser");
                    }
                    else
                    {
                        if (player.GetSharedData<Int32>("houseid") != -1)
                        {
                            playerDataManager.NotifyPlayer(player, "Jesteś już w posiadaniu innego domku!");
                            player.TriggerEvent("closeHousePanelBrowser");
                        }
                        else
                        {
                            int price = house.houseColShape.GetSharedData<Int32>("price");
                            if (playerDataManager.UpdatePlayersMoney(player, -1 * price * time))
                            {
                                house.setOwner(player, DateTime.Now.AddDays(time).ToString());
                                player.TriggerEvent("closeHousePanelBrowser");
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Nie stać cię na ten domek!");
                                player.TriggerEvent("closeHousePanelBrowser");
                            }
                        }
                    }
                }
            }
        }

        [RemoteEvent("confirmHouseExtend")]
        public void ConfirmHouseExtend(Player player, int houseId, int time)
        {
            foreach (House house in houses.houses)
            {
                if (house.houseColShape.GetSharedData<Int32>("id") == houseId)
                {
                    if (house.owner != player.GetSharedData<string>("socialclub"))
                    {
                        playerDataManager.NotifyPlayer(player, "Dom został już wynajęty przez kogoś innego!");
                        player.TriggerEvent("closeHousePanelBrowser");
                    }
                    else
                    {
                        int price = house.houseColShape.GetSharedData<Int32>("price");
                        DateTime datetime = DateTime.Parse(house.houseColShape.GetSharedData<string>("time"));
                        if (DateTime.Compare(datetime.AddDays(time), DateTime.Now.AddDays(14)) > 0)
                        {
                            player.TriggerEvent("callHousePanelError", "Dom można wynająć na maksymalnie 14 dni naprzód");
                        }
                        else
                        {
                            if (playerDataManager.UpdatePlayersMoney(player, -1 * price * time))
                            {
                                house.extendTime(datetime.AddDays(time).ToString());
                                player.TriggerEvent("updateHousePanelTime", datetime.AddDays(time).ToString());
                                playerDataManager.NotifyPlayer(player, "Przedłużono wynajem domu do: " + datetime.AddDays(time).ToString());
                            }
                            else
                            {
                                playerDataManager.NotifyPlayer(player, "Nie stać cię na ten domek!");
                                player.TriggerEvent("closeHousePanelBrowser");
                            }
                        }


                    }
                }
            }
        }

        [RemoteEvent("giveHouseUp")]
        public void GiveUpHouse(Player player, int houseId)
        {
            foreach (House house in houses.houses)
            {
                if (house.houseColShape.GetSharedData<Int32>("id") == houseId)
                {
                    if (house.owner != player.SocialClubId.ToString())
                    {
                        playerDataManager.NotifyPlayer(player, "Nie jesteś właścicielem domu!");
                        player.TriggerEvent("closeHousePanelBrowser");
                    }
                    else
                    {
                        house.clearOwner();
                        playerDataManager.NotifyPlayer(player, "Nie jesteś już właścicielem tego domu!");
                        player.TriggerEvent("closeHousePanelBrowser");
                    }
                }
            }
        }

        ////---------------Zarządzanie autem------------------

        [RemoteEvent("trip_update")]
        public void Trip_Update(Player player, Vehicle vehicle, float dist)
        {
            if(vehicle != null && vehicle.Exists)
            {
                vehicleDataManager.UpdateVehiclesTrip(vehicle, dist);
            }
        }

        [RemoteEvent("vehc_switchLights")]
        public void VehC_SwitchLights(Player player, bool state)
        {
            if (player.Vehicle != null)
            {
                player.Vehicle.SetSharedData("veh_lights", state);
            }
        }

        [RemoteEvent("vehc_switchEngine")]
        public void VehC_SwitchEngine(Player player, bool state)
        {
            if (player.Vehicle != null)
            {
                player.Vehicle.SetSharedData("veh_engine", state);
            }
        }

        [RemoteEvent("vehc_switchParkingbrake")]
        public void VehC_SwitchParkingbrake(Player player, bool state)
        {
            if (player.Vehicle != null)
            {
                vehicleDataManager.UpdateVehiclesBrake(player.Vehicle, state);
            }
        }

        [RemoteEvent("vehc_switchLocks")]
        public void VehC_SwitchLocks(Player player, bool state)
        {
            if (player.Vehicle != null)
            {
                player.Vehicle.SetSharedData("veh_locked", state);
            }
        }

        [RemoteEvent("vehc_kickPassengers")]
        public void VehC_KickPassengers(Player player)
        {
            if (player.Vehicle != null)
            {
                foreach (Player passenger in player.Vehicle.Occupants)
                {
                    if (passenger != player)
                    {
                        passenger.WarpOutOfVehicle();
                        playerDataManager.NotifyPlayer(passenger, "Zostałeś wyrzucony z pojazdu!");
                    }
                }
            }
        }





        ////--------------KOMIS/ZAKUP POJAZDU---------------


        [RemoteEvent("openCarDealer")]
        public void OpenCarDealer(Player player)
        {
            foreach (CarDealer cd in carDealers)
            {
                if (cd.colShape.IsPointWithin(player.Position))
                {
                    if (cd.vehicle != null && cd.vehicle.Exists)
                    {
                        int[] t = new int[] { 1, 1 };
                        if (cd.vehicle.HasSharedData("trunksize"))
                            t = JsonConvert.DeserializeObject<int[]>(cd.vehicle.GetSharedData<string>("trunksize"));
                        int trunk = t[0] * t[1];
                        string[] data = new string[]
                        {
                            cd.customVehicle.name,
                            cd.customVehicle.price.ToString(),
                            cd.customVehicle.combustion.ToString(),
                            cd.customVehicle.tank.ToString(),
                            trunk.ToString(),
                            cd.vehicle.GetSharedData<float>("veh_trip").ToString()
                        };

                        player.TriggerEvent("openVehBuyBrowser", JsonConvert.SerializeObject(data), cd.vehicle);
                    }
                    break;
                }

            }
        }



        [RemoteEvent("confirmVehicleBuy")]
        public void ConfirmVehicleBuy(Player player, Vehicle vehicle)
        {
            foreach (CarDealer cd in carDealers)
            {
                if (cd.vehicle == vehicle)
                {
                    if (playerDataManager.HasPlayerFreeSlot(player))
                    {
                        if (playerDataManager.UpdatePlayersMoney(player, -1 * cd.customVehicle.price))
                        {
                            cd.vehicle = null;
                            vehicleDataManager.CreatePersonalVehicleFromDealer(player, vehicle);
                            cd.SpawnNew(false, false);
                            if (cd.type == "hyper" || cd.type == "classic" || cd.type == "suv")
                            {
                                vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                                vehicle.Delete();
                                playerDataManager.NotifyPlayer(player, "Pomyślnie zakupiono pojazd. Czeka on na Ciebie w przechowalni!");
                            }
                            playerDataManager.NotifyPlayer(player, "Pomyślnie zakupiono pojazd!");
                            player.TriggerEvent("closeVehBuyBrowser");
                            break;
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Niestety, nie stać Cię na ten pojazd!");
                            break;
                        }
                    }
                    else
                    {
                        playerDataManager.NotifyPlayer(player, "Niestety, nie masz wolnych slotów na pojazdy!");
                        break;
                    }
                }
            }
        }



        //carwash
        private void UpdateCarWashAndStorage(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                {
                    if (vehicle.HasSharedData("washtime") && vehicle.GetSharedData<string>("washtime") != "" && DateTime.Compare(DateTime.Parse(vehicle.GetSharedData<string>("washtime")), DateTime.Now) < 0)
                    {
                        vehicleDataManager.UpdateVehiclesWashTime(vehicle, "");
                    }
                    if (vehicle.HasSharedData("storageTime") && vehicle.GetSharedData<string>("storageTime") != "")
                    {
                        DateTime time = DateTime.Parse(vehicle.GetSharedData<string>("storageTime"));
                        if (time.AddMinutes(1) <= DateTime.Now)
                        {
                            if (vehicle != null && vehicle.Exists)
                            {
                                vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                                vehicle.Delete();
                            }
                        }

                    }
                    if (vehicle.HasSharedData("publicTime") && vehicle.GetSharedData<string>("publicTime") != "")
                    {
                        DateTime time = DateTime.Parse(vehicle.GetSharedData<string>("publicTime"));
                        if (time.AddMinutes(1) <= DateTime.Now)
                        {
                            if (vehicle != null && vehicle.Exists)
                            {
                                vehicle.Delete();
                            }
                        }
                    }
                }
            });
        }

        [RemoteEvent("startCarWash")]
        public void StartCarWash(Player player, Vehicle vehicle, ColShape colshape)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("washtime") && player.VehicleSeat == 0)
            {
                foreach (CarWash carWash in carWashes)
                {
                    if (carWash.shape == colshape)
                    {
                        if (playerDataManager.UpdatePlayersMoney(player, -100))
                        {
                            carWash.WashCar(player, vehicle);
                            break;
                        }
                        else
                        {
                            playerDataManager.NotifyPlayer(player, "Nie stać cię na to!");
                            break;
                        }

                    }
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Pojazd nie należy do Ciebie!");
            }
        }

        [RemoteEvent("freezeJobVeh")]
        public void FreezeJobVeh(Player player, bool state, Vehicle vehicle)
        {
            if(vehicle != null && vehicle.Exists)
            {
                vehicle.SetSharedData("veh_brake", state);
            }
        }

        //TOWTRUCKS

        [RemoteEvent("createTowObject")]
        public void CreateTowObject(Player player, Vehicle towtruck, string type)
        {
            switch (type)
            {
                case "car":
                    towtruck.SetSharedData("towingobj", "imp_prop_covered_vehicle_03a");
                    break;
                default:
                    towtruck.SetSharedData("towingobj", type);
                    break;
            }
        }
        [RemoteEvent("removeTowedVehicle")]
        public void RemoveTowedVehicle(Player player, Vehicle vehicle)
        {
            if (vehicle != null)
            {
                if (vehicle.HasSharedData("market") && vehicle.GetSharedData<bool>("market"))
                {
                    //carMarket.RemoveVehicleFromMarket(vehicle);
                }
                vehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                vehicle.Delete();
            }
        }
        [RemoteEvent("markVehicleAsNotTowed")]
        public void MarkVehicleAsNotTowed(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
                vehicle.SetSharedData("towed", false);
        }

        [RemoteEvent("vehicleTowed")]
        public void VehicleTowed(Player player, string type, float distance, float dmg)
        {
            payoutManager.TowtruckPayment(player, type, distance, dmg);
            player.Vehicle.SetSharedData("towingobj", "");
        }

        [RemoteEvent("startTowTrucks")]
        public void StartTowTrucks(Player player)
        {
            towTruck.startJob(player);
        }

        [RemoteEvent("getVehicleToTow")]
        public void GetVehicleToTow(Player player)
        {
            Vehicle veh = vehicleDataManager.GetRandomVehicleToTow();
            player.TriggerEvent("setVehicleToTow", veh);
        }

        //DIVER

        [RemoteEvent("diver_startJob")]
        public void Diver_StartJob(Player player)
        {
            diver.StartJob(player);
        }
        [RemoteEvent("diver_payment")]
        public void Diver_Payment(Player player, int priceMult)
        {
            payoutManager.DiverPayment(player, priceMult);
        }

        //DEBRISCLEANER

        [RemoteEvent("startDebrisCleaner")]
        public void StartDebrisCleaner(Player player)
        {
            debrisCleaner.startJob(player);
        }

        [RemoteEvent("debrisCleanerReward")]
        public void DebrisCleanerReward(Player player, int weight)
        {
            payoutManager.DebrisCleanerPayment(player, weight);
        }

        //FISHERMAN

        [RemoteEvent("buyFishingRod")]
        public void BuyFishingRod(Player player)
        {
            fisherMan.BuyFishingRod(player);
        }

        [RemoteEvent("startFishing")]
        public void StartFishing(Player player)
        {
            fisherMan.StartJob(player);
        }

        [RemoteEvent("fishingDone")]
        public void FishingDone(Player player, int size, int type)
        {
            fisherMan.Done(player, size, type);
        }

        [RemoteEvent("sellFishes")]
        public void SellFishes(Player player)
        {
            List<string> fishes = new List<string>();
            string eq = player.GetSharedData<string>("equipment");
            List<Dictionary<string, string>> items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(eq);
            foreach (Dictionary<string, string> item in items)
            {
                switch (Int32.Parse(item["typeID"]))
                {
                    case 1001:
                    case 1002:
                    case 1003:
                    case 1004:
                    case 1005:
                    case 1006:
                    case 1007:
                    case 1008:
                    case 1009:
                    case 1010:
                    case 1011:
                    case 1012:
                    case 1013:
                    case 1014:
                    case 1015:
                    case 1016:
                    case 1017:
                    case 1018:
                    case 1019:
                    case 1020:
                    case 1021:
                    case 1022:
                        fishes.Add(item["typeID"]);
                        player.TriggerEvent("removeEqItem", Int32.Parse(item["id"]));
                        break;
                }
            }
            if (fishes.Count == 0)
            {
                playerDataManager.NotifyPlayer(player, "Nie masz żadnych ryb do sprzedania!");
            }
            else
            {
                payoutManager.FisherSold(player, fishes);
                NAPI.Task.Run(() =>
                {
                    if (player != null && player.Exists)
                        player.TriggerEvent("saveEquipment");
                }, 500);
            }
        }

        [RemoteEvent("sellJunk")]
        public void SellJunk(Player player)
        {
            List<string> junks = new List<string>();
            string eq = player.GetSharedData<string>("equipment");
            if (eq != "[]")
            {
                List<Dictionary<string, string>> items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(eq);
                foreach (Dictionary<string, string> item in items)
                {
                    switch (Int32.Parse(item["typeID"]))
                    {
                        case 1050:
                        case 1051:
                        case 1052:
                        case 1053:
                        case 1054:
                        case 1055:
                        case 1056:
                        case 1057:
                        case 1058:
                            junks.Add(item["typeID"]);
                            player.TriggerEvent("removeEqItem", Int32.Parse(item["id"]));
                            break;
                    }
                }
                if (junks.Count == 0)
                {
                    playerDataManager.NotifyPlayer(player, "Nie masz żadnych przedmiotów do sprzedania!");
                }
                else
                {
                    payoutManager.FisherJunkSold(player, junks);
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie masz żadnych przedmiotów do sprzedania!");
            }

        }

        //WAREHOUSE

        [RemoteEvent("startWarehouse")]
        public void StartWareHouse(Player player)
        {
            warehouse.startJob(player);
        }

        [RemoteEvent("warehouseBoxDelievered")]
        public void WarehouseBoxDelievered(Player player)
        {
            payoutManager.WarehousePayment(player);
            warehouse.CreateBox();
        }

        //JUNKYARD
        [RemoteEvent("startJunkyard")]
        public void StartJunkyard(Player player)
        {
            junkyard.startJob(player);
        }

        [RemoteEvent("junkDelievered")]
        public void JunkDelievered(Player player)
        {
            payoutManager.JunkyardPayment(player);
        }


        //GARDENER

        [RemoteEvent("gardener_pickupPlant")]
        public void Gardener_PickupPlant(Player player, int plant_id, int groundId)
        {
            foreach (Ground ground in gardener.Grounds)
            {
                if (ground.GroundId == groundId)
                {
                    foreach (KeyValuePair<Vector3, Plant> pair in ground.Plants)
                    {
                        if (pair.Value.Id == plant_id)
                        {
                            ground.PickPlantUp(player, pair);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        [RemoteEvent("gardener_startJob")]
        public void Gardener_StartJob(Player player)
        {
            gardener.StartJob(player);
        }

        [RemoteEvent("gardener_selectOrder")]
        public void Gardener_SelectOrder(Player player, int orderId)
        {
            foreach (GardenerOrder order in gardener.Orders)
            {
                if (order.Id == orderId)
                {
                    player.TriggerEvent("closeGardenerOrdersBrowser");
                    string o = JsonConvert.SerializeObject(order.Plants);
                    player.TriggerEvent("gardener_setNewOrder", o);
                    gardener.NewOrder(gardener.Orders.IndexOf(order));
                    return;
                }
            }
            playerDataManager.NotifyPlayer(player, "To zlecenie jest już nieaktualne!");
        }

        [RemoteEvent("gardener_refreshOrders")]
        public void Gardener_RefreshOrders(Player player)
        {
            player.TriggerEvent("gardener_insertData", gardener.GetOrders());
        }

        [RemoteEvent("gardener_cancelOrder")]
        public void Gardener_CancelOrder(Player player)
        {
            EndJob(player);
        }

        [RemoteEvent("gardener_sellPlants")]
        public void Gardener_SellPlants(Player player, string baseOrder, string orderState)
        {
            int[] order = JsonConvert.DeserializeObject<int[]>(orderState);
            List<EqItem> equipment = JsonConvert.DeserializeObject<List<EqItem>>(player.GetSharedData<string>("equipment"));
            List<int> itemsToRemove = new List<int>();
            int exp = 0;
            foreach (EqItem item in equipment)
            {
                switch (item.TypeID)
                {
                    case 900:
                        if (order[0] > 0)
                        {
                            order[0]--;
                            itemsToRemove.Add(item.Id);
                            exp += 3;
                        }
                        break;
                    case 901:
                        if (order[1] > 0)
                        {
                            order[1]--;
                            itemsToRemove.Add(item.Id);
                            exp += 3;
                        }
                        break;
                    case 902:
                        if (order[2] > 0)
                        {
                            order[2]--;
                            itemsToRemove.Add(item.Id);
                            exp += 3;
                        }
                        break;
                    case 903:
                        if (order[3] > 0)
                        {
                            order[3]--;
                            itemsToRemove.Add(item.Id);
                            exp += 5;
                        }
                        break;
                    case 904:
                        if (order[4] > 0)
                        {
                            order[4]--;
                            itemsToRemove.Add(item.Id);
                            exp += 5;
                        }
                        break;
                }
            }

            if (itemsToRemove.Count > 0)
            {
                itemsToRemove.ForEach(item =>
                {
                    player.TriggerEvent("removeEqItem", item);
                });
                payoutManager.GardenerPlantsSold(player, exp);
                playerDataManager.NotifyPlayer(player, "Pomyślnie oddano rośliny!");
                player.TriggerEvent("gardener_updateOrder", JsonConvert.SerializeObject(order));
                NAPI.Task.Run(() =>
                {
                    if (player != null && player.Exists)
                        player.TriggerEvent("saveEquipment");
                }, 500);


                if (order.All(o => o == 0))
                {
                    playerDataManager.NotifyPlayer(player, "Pomyślnie skompletowano zamówienie!");
                    payoutManager.GardenerOrderCompleted(player, baseOrder);
                    player.TriggerEvent("closeGardenerHUDBrowser");
                    player.TriggerEvent("openGardenerOrdersBrowser", gardener.GetOrders());
                }
            }
            else
            {
                playerDataManager.NotifyPlayer(player, "Nie posiadasz żadnych roślin do oddania!");
            }


        }

        //HUNTER

        [RemoteEvent("startHunter")]
        public void StartHunter(Player player)
        {
            hunter.startJob(player);
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
                    var veh = vehicleDataManager.GetVehicleByRemoteId(Convert.ToUInt16(player.GetSharedData<Int32>("jobveh")));
                    if (veh != null && veh.Exists)
                        veh.Delete();
                    player.SetSharedData("jobveh", -1111);
                }

            }
        }
        [RemoteEvent("animalHunted")]
        public void AnimalHunted(Player player, string pedstr)
        {
            payoutManager.HunterPaymentAnimal(player, pedstr);
            hunter.GetRandomAnimalAndSendToPlayer(player);
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
                playerDataManager.NotifyPlayer(player, "Nie masz żadnych skór do sprzedania!");
            }
            else
            {
                payoutManager.HunterPaymentPelts(player, pelts);
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
                        fisherMan.ConfirmFishingRod(player);
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
                        playerDataManager.NotifyPlayer(player, "Nie zmieścisz tej skóry do ekwipunku!");
                        break;
                    case 1000:
                        playerDataManager.NotifyPlayer(player, "Nie zmieścisz wędki do ekwipunku!");
                        break;
                }
            }
        }


        [RemoteEvent("startLawnmowing")]
        public void StartLawnmowing(Player player)
        {
            lawnmowing.startJob(player);
        }

        [RemoteEvent("freezeLawnmower")]
        public void FreezeLawnmower(Player player, Vehicle vehicle, bool val)
        {
            vehicle.SetSharedData("veh_brake", val);
        }

        [RemoteEvent("lawnmowingReward")]
        public void LawnmowingReward(Player player)
        {
            payoutManager.LawnmowingPayment(player);
        }

        //forklifts
        [RemoteEvent("startForklifts")]
        public void StartForklifts(Player player)
        {
            forklifts.StartJob(player);
        }

        [RemoteEvent("forkliftBoxPickedUp")]
        public void ForkliftBoxPickedUp(Player player, GTANetworkAPI.Object obj)
        {
            if (obj.HasSharedData("posID"))
            {
                forklifts.CreateNewBox(obj.GetSharedData<Int32>("posID"));
            }
            obj.Delete();
        }

        [RemoteEvent("forkliftsBoxDropped")]
        public void ForkliftsBoxDropped(Player player)
        {
            payoutManager.ForkliftsPayment(player);
        }

        [RemoteEvent("savePos")]
        public void SavePos(Player player, Vector3 position)
        {
            string pos = $"new Vector3({position.X.ToString().Replace(',', '.')}f, {position.Y.ToString().Replace(',', '.')}f, {position.Z.ToString().Replace(',', '.')}f);";
            File.AppendAllText(@"positions.txt", pos + Environment.NewLine, new UTF8Encoding(false, true));
        }
        //SUPPLIER
        [RemoteEvent("startSupplier")]
        public void StartSupplier(Player player)
        {
            //supplier.startJob(player);
        }
        [RemoteEvent("supplyPoint")]
        public void SupplyPoint(Player player, bool state, Vehicle t)
        {
            int color = 120;
            if (state)
            {
                color = 158;
            }
            NAPI.Vehicle.SetVehiclePrimaryColor(t.Handle, color);

        }
        [RemoteEvent("deleteSupplyCar")]
        public void RemoveSupplyCar(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                vehicle.Delete();
            }
        }

        [RemoteEvent("supplyDone")]
        public void SupplyDone(Player player, int supplies, float dmg)
        {
            payoutManager.SupplierPayment(player, supplies, dmg);
            //logManager.LogJobTransaction(player.SocialClubId.ToString(), $"Dostawca: +{reward.ToString()}$, +1PP ({player.GetSharedData<Int32>("money")},{player.GetSharedData<Int32>("pp")})");
        }
        ////-------------STANDARDOWE METODY---------------

        private void ValuesToDB(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (Player player in NAPI.Pools.GetAllPlayers())
                {
                    if (player.HasSharedData("spawned"))
                    {
                        if (player.Dimension == 0 && !(player.HasSharedData("spec") && player.GetSharedData<bool>("spec")))
                        {
                            playerDataManager.UpdatePlayersLastPos(player);
                        }
                        player.TriggerEvent("updatePlayerBlips", NAPI.Pools.GetAllBlips().ToArray());
                        player.TriggerEvent("getVehicleDamage");
                        player.TriggerEvent("updateVehicleBlips");
                    }
                }
                foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                {
                    if (vehicle.GetSharedData<string>("type") == "personal" && vehicle.Occupants.Count > 0)
                    {
                        (vehicle.Occupants[0] as Player).TriggerEvent("updateDirtLevel");
                    }
                    if ((vehicle.GetSharedData<string>("type") == "personal" && vehicle.HasSharedData("veh_brake") && !vehicle.GetSharedData<bool>("veh_brake")) || (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") == "lspd"))
                    {
                        vehicleDataManager.UpdateVehiclesLastPos(vehicle);
                    }
                }
            });
        }

        private void UpdateArrestTimes(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (Player player in NAPI.Pools.GetAllPlayers())
                {
                    if (player.HasSharedData("arrested") && player.GetSharedData<bool>("arrested"))
                    {
                        foreach (Arrest arrest in lspd.Arrests)
                        {
                            bool found = false;
                            foreach (KeyValuePair<ulong, int> inmate in arrest.Inmates)
                            {
                                if (inmate.Key == player.SocialClubId)
                                {
                                    int time = inmate.Value;
                                    time -= 1;
                                    arrest.Inmates[arrest.Inmates.IndexOf(inmate)] = new KeyValuePair<ulong, int>(player.SocialClubId, time);
                                    if (time == 0)
                                    {
                                        lspd.RemovePlayerFromArrest(player);
                                    }
                                    else if (time % 5 == 0)
                                    {
                                        playerDataManager.SendInfoToPlayer(player, "Pozostało " + time.ToString() + " minut aresztu!");
                                    }

                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                                break;
                        }
                    }
                }
            });
        }

        [RemoteEvent("setPlayersDimension")]
        public void SetPlayersDimension(Player player, int dimension)
        {
            player.Dimension = Convert.ToUInt32(dimension);
        }
        private void CreateCarDealers()
        {
            carDealers.Add(new CarDealer(new Vector3(-47.854286f, -1102.2466f, 26.422354f), 69.04183f, "sport", false, ref vehicleDataManager, new int[] {0, 200}));
            carDealers.Add(new CarDealer(new Vector3(-44.30936f, -1094.234f, 26.422354f), 118.58548f, "sport", true, ref vehicleDataManager, new int[] { 0, 200 }));
            carDealers.Add(new CarDealer(new Vector3(-48.185146f, -1092.8115f, 26.422354f), 118.58548f, "sport", false, ref vehicleDataManager, new int[] { 0, 200 }));

            carDealers.Add(new CarDealer(new Vector3(2553.4504f, 4672.2476f, 33.93297f), 15f, "offroad", true, ref vehicleDataManager, new int[] { 1000, 10000 }));

            carDealers.Add(new CarDealer(new Vector3(-240.60123f, 6199.235f, 31.489218f), 144f, "regular", false, ref vehicleDataManager, new int[] { 15000, 35000 }));
            carDealers.Add(new CarDealer(new Vector3(-243.02003f, 6201.7427f, 31.489218f), 144f, "regular", true, ref vehicleDataManager, new int[] { 15000, 35000 }));
            carDealers.Add(new CarDealer(new Vector3(-245.30743f, 6203.9624f, 31.489218f), 144f, "regular", false, ref vehicleDataManager, new int[] { 15000, 35000 }));

            carDealers.Add(new CarDealer(new Vector3(1616.0236f, 3788.4014f, 34.725067f), -140f, "junk", false, ref vehicleDataManager, new int[] { 50000, 75000 }));
            carDealers.Add(new CarDealer(new Vector3(1617.5266f, 3791.165f, 34.73369f), -140f, "junk", true, ref vehicleDataManager, new int[] { 50000, 75000 }));

            carDealers.Add(new CarDealer(new Vector3(-806.7707f, -210.8126f, 58.910355f), -150f, "hyper", false, ref vehicleDataManager, new int[] { 0, 100 }));
            carDealers.Add(new CarDealer(new Vector3(-812.215f, -201.61041f, 58.88827f), -150f, "hyper", false, ref vehicleDataManager, new int[] { 0, 100 }));

            carDealers.Add(new CarDealer(new Vector3(1252.9817f, -1141.6354f, 38.757774f), 75.5f, "bike", false, ref vehicleDataManager, new int[] { 1000, 10000 }));
            carDealers.Add(new CarDealer(new Vector3(1251.4142f, -1137.7074f, 38.75776f), 75.5f, "bike", true, ref vehicleDataManager, new int[] { 1000, 10000 }));
            carDealers.Add(new CarDealer(new Vector3(1250.1792f, -1133.866f, 38.757716f), 75.5f, "bike", false, ref vehicleDataManager, new int[] { 1000, 10000 }));

            carDealers.Add(new CarDealer(new Vector3(-786.7027f, -202.84512f, 58.89831f), 30f, "suv", false, ref vehicleDataManager, new int[] { 0, 200 }));
            carDealers.Add(new CarDealer(new Vector3(-792.28204f, -193.24438f, 58.8983f), 30f, "suv", false, ref vehicleDataManager, new int[] { 0, 200 }));
            carDealers.Add(new CarDealer(new Vector3(-803.0028f, -194.12485f, 58.910297f), 120f, "classic", true, ref vehicleDataManager, new int[] { 20000, 50000 }));

            carDealers.Add(new CarDealer(new Vector3(1010.34125f, -1869.964f, 30.88981f), -8f, "regular2", false, ref vehicleDataManager, new int[] { 10000, 40000 }));
            carDealers.Add(new CarDealer(new Vector3(1005.42834f, -1869.6227f, 30.88981f), -3, "regular2", true, ref vehicleDataManager, new int[] { 10000, 40000 }));
            carDealers.Add(new CarDealer(new Vector3(1000.40985f, -1868.8093f, 30.88981f), -3f, "regular2", false, ref vehicleDataManager, new int[] { 10000, 40000 }));


        }

        private void CreateTeleports()
        {
            InteriorTeleport departmentLS = new InteriorTeleport(new Vector3(-1581.3214f, -558.3011f, 34.9531f), 36f, new Vector3(-1560.9137f, -568.60657f, 114.57642), 40f, "Urząd miasta");
            InteriorTeleport salonPremium = new InteriorTeleport(new Vector3(-803.1636f, -223.88596f, 37.225594f), 120f, new Vector3(-787.7405f, -219.56136f, 58.519867f), 30f, "Salon premium");
        }
        private void CreateVehicleStorages()
        {
            VehicleStorage vs = new VehicleStorage(1, new Vector3(1879.1293f, 3760.452f, 33.062183f), 7.0f, new Vector3(1872.2902f, 3757.814f, 33.06228f));
            vs.AddSpawningPoint(new Vector3(1882.7478f, 3760.1377f, 33.06218f), -150f);
            vs.AddSpawningPoint(new Vector3(1877.7385f, 3757.1594f, 33.06218f), -150f);
            vehicleStorages.Add(vs);
            VehicleStorage vsls = new VehicleStorage(2, new Vector3(137.94495f, -1083.2755f, 29.19444f), 7.0f, new Vector3(137.32211f, -1085.9031f, 29.19237f));
            vsls.AddSpawningPoint(new Vector3(132.21432f, -1081.7181f, 29.226038f), 2f);
            vsls.AddSpawningPoint(new Vector3(136.09067f, -1082.0425f, 29.222183f), 2f);
            vsls.AddSpawningPoint(new Vector3(139.804f, -1082.0377f, 29.226038f), 2f);
            vsls.AddSpawningPoint(new Vector3(143.53342f, -1082.0083f, 29.226038f), 2f);
            vehicleStorages.Add(vsls);
            VehicleStorage vspaleto = new VehicleStorage(3, new Vector3(-72.57493f, 6425.9814f, 31.439983f), 5.0f, new Vector3(-70.828255f, 6424.371f, 31.439854f));
            vspaleto.AddSpawningPoint(new Vector3(-69.98739f, 6427.8022f, 31.4392f), 40f);
            vspaleto.AddSpawningPoint(new Vector3(-74.2419f, 6423.5034f, 31.490437f), 40f);
            vehicleStorages.Add(vspaleto);
        }
        private void CreateVehicleMechanics()
        {
            VehicleMechanic vm = new VehicleMechanic(new Vector3(1566.1263f, 3792.468f, 34.645317f), 100f, new Vector3(1560.1971f, 3793.4932f, 34.716663f), true, "poor");
            vehicleMechanics.Add(vm);
            VehicleMechanic vmls1 = new VehicleMechanic(new Vector3(-219.53825f, -1173.263f, 23.615189f), 45f, new Vector3(-223.8401f, -1173.0829f, 23.260616f), false, "rich");
            vehicleMechanics.Add(vmls1);
            VehicleMechanic vmls2 = new VehicleMechanic(new Vector3(-227.4387f, -1173.3578f, 23.615189f), 45f, new Vector3(-231.78595f, -1172.9685f, 23.258411f), true, "rich");
            vehicleMechanics.Add(vmls2);
            VehicleMechanic vmls3 = new VehicleMechanic(new Vector3(-235.41434f, -1173.3915f, 23.615189f), 45f, new Vector3(-239.52097f, -1172.9838f, 23.267845f), false, "rich");
            vehicleMechanics.Add(vmls3);

            VehicleMechanic vmpaleto1 = new VehicleMechanic(new Vector3(-285.28976f, 6049.638f, 31.694714f), 75f, new Vector3(-288.6118f, 6046.74f, 31.7553f), true, "rich");
            vehicleMechanics.Add(vmpaleto1);

            VehicleMechanic vmpaleto2 = new VehicleMechanic(new Vector3(-290.86105f, 6044.035f, 31.694653f), 75f, new Vector3(-294.0558f, 6041.2373f, 31.754717f), false, "rich");
            vehicleMechanics.Add(vmpaleto2);
        }
        public void CreatePublicVehicles()
        {
            //sandy spawn
            // new Vector3(103.76306f, -1083.1083f, 29.19238f); heading: -25.003464f  -  szkuteratuti
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-358.64642f, -126.77186f, 38.695824f), 68.55481f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-802.21204f, -227.84772f, 37.193943f), 118.34483f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-1576.1237f, -551.59076f, 34.953655f), 31.633419f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-1623.031f, -876.25134f, 9.440182f), -44.95958f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-51.619274f, -1081.5751f, 26.90126f), 71.67744f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(146.28804f, -2507.457f, 5.9999943f), -72.78241f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(9.326692f, -2757.4258f, 6.004301f), 0.8770372f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(1302.9294f, -3343.4783f, 5.5801206f), -90.95663f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-1368.5206f, 46.290375f, 53.905636f), 89.611f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-1684.6394f, 58.149143f, 64.02879f), 136.98888f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(1897.2589f, 3710.4368f, 32.755775f), 118.230255f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(1659.4358f, 3814.761f, 34.869144f), -52.362194f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(2548.382f, 4664.3765f, 34.07682f), 16.364094f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-121.30436f, 6392.6216f, 31.48985f), 43.430614f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-81.15758f, 6472.5234f, 31.479202f), -141.37119f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-11.886782f, 6303.1455f, 31.376215f), 21.04515f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(115.53413f, -1087.2607f, 29.214767f), 25.903833f, this));

            //szpitale

            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-451.15082f, -351.43372f, 34.50172f), 82.60945f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(296.77106f, -601.909f, 43.30345f), 115.56599f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(1146.9432f, -1524.7622f, 34.843403f), -19.130648f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(1837.8899f, 3667.2068f, 33.67998f), -149.71346f, this));
            publicVehicleSpawns.Add(new PublicVehicleSpawn(new Vector3(-242.66228f, 6325.4795f, 32.426174f), -101.287056f, this));

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

        public void CreatePetrolStations()
        {
            petrolStations.Add(new PetrolStation(new Vector3(-719.70325f, -932.6565f, 19.01702f), new Vector3(-723.2033f, -932.51117f, 19.213932f)));

            petrolStations.Add(new PetrolStation(new Vector3(-728.3354f, -938.9094f, 19.017021f), new Vector3(-731.628f, -939.21454f, 19.131214f)));

            petrolStations.Add(new PetrolStation(new Vector3(1178.0122f, -335.59198f, 69.178444f), new Vector3(1177.5814f, -331.61685f, 69.316574f)));

            petrolStations.Add(new PetrolStation(new Vector3(-73.753136f, -1756.4204f, 29.546978f), new Vector3(-77.032196f, -1755.2241f, 29.80032f)));

            petrolStations.Add(new PetrolStation(new Vector3(2584.7908f, 358.39944f, 108.457344f), new Vector3(2581.459f, 358.8888f, 108.647804f)));

            petrolStations.Add(new PetrolStation(new Vector3(2007.8373f, 3772.059f, 32.18083f), new Vector3(2006.5984f, 3774.334f, 32.403954f)));

            petrolStations.Add(new PetrolStation(new Vector3(176.05913f, 6604.464f, 31.848372f), new Vector3(178.64561f, 6604.723f, 32.047367f)));

            petrolStations.Add(new PetrolStation(new Vector3(-2092.7256f, -326.8467f, 13.027078f), new Vector3(-2089.2368f, -327.29593f, 13.168626f)));

            petrolStations.Add(new PetrolStation(new Vector3(-2558.0168f, 2330.2908f, 33.06005f), new Vector3(-2558.3157f, 2333.3333f, 33.256554f)));

            petrolStations.Add(new PetrolStation(new Vector3(624.70514f, 264.5712f, 103.089424f), new Vector3(628.99805f, 263.81094f, 103.25263f)));
            petrolStations.Add(new PetrolStation(new Vector3(616.5143f, 273.59412f, 103.089386f), new Vector3(613.2378f, 274.02643f, 103.08946f)));
            petrolStations.Add(new PetrolStation(new Vector3(-1795.0027f, 804.60046f, 138.51329f), new Vector3(-1791.4583f, 805.88306f, 138.69159f)));
            petrolStations.Add(new PetrolStation(new Vector3(-1804.0342f, 801.4774f, 138.51334f), new Vector3(-1808.2684f, 800.4213f, 138.67442f)));
            petrolStations.Add(new PetrolStation(new Vector3(-2100.418f, -312.10123f, 13.027773f), new Vector3(-2103.9517f, -311.07608f, 13.167596f)));
            petrolStations.Add(new PetrolStation(new Vector3(-1427.516f, -276.1158f, 46.207676f), new Vector3(-1428.5182f, -278.56113f, 46.371094f)));
            petrolStations.Add(new PetrolStation(new Vector3(-1440.4117f, -272.64352f, 46.207653f), new Vector3(-1444.1167f, -273.56512f, 46.370132f)));
            petrolStations.Add(new PetrolStation(new Vector3(-529.0941f, -1209.0199f, 18.184855f), new Vector3(-532.08014f, -1211.9678f, 18.328247f)));
            petrolStations.Add(new PetrolStation(new Vector3(-67.4831f, -1765.2819f, 29.252151f), new Vector3(-64.16005f, -1767.7323f, 29.251905f)));
            petrolStations.Add(new PetrolStation(new Vector3(269.05417f, -1263.1492f, 29.142893f), new Vector3(273.33795f, -1261.3694f, 29.291903f)));
            petrolStations.Add(new PetrolStation(new Vector3(260.76926f, -1260.4154f, 29.142912f), new Vector3(257.16565f, -1261.1693f, 29.28791f)));
            petrolStations.Add(new PetrolStation(new Vector3(177.59439f, -1560.0685f, 29.251604f), new Vector3(176.6006f, -1556.4692f, 29.318207f)));
            petrolStations.Add(new PetrolStation(new Vector3(814.9064f, -1027.8191f, 26.250578f), new Vector3(811.3122f, -1026.2401f, 26.399002f)));
            petrolStations.Add(new PetrolStation(new Vector3(823.22314f, -1027.2488f, 26.24993f), new Vector3(819.5721f, -1026.2556f, 26.396868f)));
            petrolStations.Add(new PetrolStation(new Vector3(1182.8838f, -325.65872f, 69.17435f), new Vector3(1183.2421f, -321.69476f, 69.35189f)));
            petrolStations.Add(new PetrolStation(new Vector3(1210.7844f, 2661.6572f, 37.809975f), new Vector3(1209.0221f, 2659.937f, 37.89978f)));
            petrolStations.Add(new PetrolStation(new Vector3(1782.6595f, 3329.2073f, 41.252148f), new Vector3(1785.4874f, 3329.485f, 41.407673f)));
            petrolStations.Add(new PetrolStation(new Vector3(2001.8948f, 3776.4958f, 32.180775f), new Vector3(2003.5157f, 3774.2593f, 32.403915f)));
            petrolStations.Add(new PetrolStation(new Vector3(51.208614f, 2781.6694f, 57.884014f), new Vector3(49.40787f, 2780.1584f, 58.042217f)));
            petrolStations.Add(new PetrolStation(new Vector3(2677.362f, 3266.931f, 55.240562f), new Vector3(2680.1035f, 3266.9175f, 55.38626f)));
            petrolStations.Add(new PetrolStation(new Vector3(1706.3865f, 6418.156f, 32.637672f), new Vector3(1706.1519f, 6415.268f, 32.763397f)));
            petrolStations.Add(new PetrolStation(new Vector3(1697.3052f, 6414.5747f, 32.71776f), new Vector3(1697.4539f, 6417.2554f, 32.647694f)));
            petrolStations.Add(new PetrolStation(new Vector3(1210.1422f, -1401.1631f, 35.224182f), new Vector3(1207.4503f, -1398.6072f, 35.373466f)));
        }

        public void CreateCarWashes()
        {
            carWashes.Add(new CarWash(new Vector3(-699.83606f, -932.6352f, 19.013899f), new Vector3(-699.97925f, -931.32666f, 21.0139f)));

            carWashes.Add(new CarWash(new Vector3(136.39879f, 6650.799f, 31.89376f), new Vector3(137.90192f, 6649.2163f, 33.893246f)));
            carWashes.Add(new CarWash(new Vector3(19.957819f, -1391.9983f, 29.324995f), new Vector3(21.814413f, -1392.196f, 31.328192f)));
            carWashes.Add(new CarWash(new Vector3(1362.914f, 3592.054f, 34.919933f), new Vector3(1364.7023f, 3592.6375f, 36.910748f)));
        }


        public void CreatePaintShops()
        {
            PaintShop psls = new PaintShop(new Vector3(-327.23065f, -144.6532f, 39.059948f));
        }
        public void InfoMessages(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                playerDataManager.SendInfoMessageToAllPlayers("Serwer jest w trakcie przygotowań. Wszystkie postępy zostaną usunięte tuż przed startem. Wszelkie błędy prosimy zgłaszać na Discordzie:  https://discord.gg/Yr6JeUtEM7");
            });
        }
        public string[] GetArgs(string arg)
        {
            if (arg == null || arg == "")
                return null;
            if (!arg.Contains(" "))
            {
                return new string[] { arg };
            }
            if (arg.Contains(" "))
                return arg.Split(" ");
            return null;
        }

        public void RefreshHousesAndPenalties(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (House house in houses.houses)
                {
                    if (house.owner != "")
                    {
                        string time = house.houseColShape.GetSharedData<string>("time");
                        if (DateTime.Compare(DateTime.Now, DateTime.Parse(time)) > 0)
                        {
                            house.clearOwner();
                        }
                    }
                }
                foreach (Player player in NAPI.Pools.GetAllPlayers())
                {
                    if (player.Exists)
                    {
                        player.SetSharedData("ping", player.Ping);
                        if (player.HasSharedData("playtime"))
                        {
                            player.SetSharedData("playtime", player.GetSharedData<Int32>("playtime") + 1);
                            playerDataManager.UpdatePlayersPlaytime(player, player.GetSharedData<Int32>("playtime"));
                        }

                        if (playerDataManager.isPlayersPenaltyExpired(player, "muted"))
                        {
                            player.SetSharedData("muted", false);
                            playerDataManager.SendInfoToPlayer(player, "Twoja kara wyciszenia wygasła!");
                        }
                        if (playerDataManager.isPlayersPenaltyExpired(player, "nodriving"))
                        {
                            player.SetSharedData("nodriving", false);
                            playerDataManager.SendInfoToPlayer(player, "Twoje prawo jazdy odzyskało ważność!");
                        }
                    }
                }
                foreach (TuneBusiness tuneBusiness in tuneBusinesses)
                {
                    if (tuneBusiness.Owner != 0 && tuneBusiness.PaidTo <= DateTime.Now)
                    {
                        tuneBusiness.ResetOwner();
                    }
                    else if (tuneBusiness.WheelOrders != null && tuneBusiness.WheelOrders.Count > 0)
                    {
                        List<KeyValuePair<int[], DateTime>> idsToRemove = new List<KeyValuePair<int[], DateTime>>();
                        foreach (KeyValuePair<int[], DateTime> order in tuneBusiness.WheelOrders)
                        {
                            if (order.Value <= DateTime.Now)
                            {
                                idsToRemove.Add(order);
                            }
                        }
                        if (idsToRemove.Count > 0)
                        {
                            foreach (KeyValuePair<int[], DateTime> id in idsToRemove)
                            {
                                tuneBusiness.AvailableWheels.Add(id.Key);
                                tuneBusiness.WheelOrders.Remove(id);
                            }
                            tuneBusiness.SaveBusinessToDB();
                        }
                    }
                }
            });
        }

        public void ManageQueues(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                //karting.ManageQueue();
            });
        }

        public void TimeHandler(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                secondsPassed++;
                if (secondsPassed == 8)
                {
                    secondsPassed = 0;
                    foreach (Player player in NAPI.Pools.GetAllPlayers())
                    {
                        player.TriggerEvent("setTime", (playerDataManager.time.Hour.ToString().Length == 1 ? ("0" + playerDataManager.time.Hour.ToString()) : playerDataManager.time.Hour.ToString()) + ":" + (playerDataManager.time.Minute.ToString().Length == 1 ? ("0" + playerDataManager.time.Minute.ToString()) : playerDataManager.time.Minute.ToString()));
                    }
                }
                playerDataManager.time = playerDataManager.time.AddSeconds(8);
                NAPI.World.SetTime(playerDataManager.time.Hour, playerDataManager.time.Minute, playerDataManager.time.Second);

            });
        }

        public void CarDealerRoll(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (CarDealer cD in carDealers)
                {
                    if (DateTime.Compare(DateTime.Now, cD.rollTime) >= 0)
                    {
                        cD.SpawnNew(true, false);
                    }
                }

            });
        }

        public void SetNewBonus(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                string[] bonus = payoutManager.SetNewBonus();
                string time = DateTime.Now.AddHours(2).ToString();
                payoutManager.bonusTime = DateTime.Now.AddHours(2);
                playerDataManager.SendInfoMessageToAllPlayers($"Wylosowano nowy bonus {float.Parse(bonus[1]) * 100}% na: {bonus[0]} do: {time}");
            });
        }

        public void WeatherHandler(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                Random rnd = new Random();
                int chance = rnd.Next(0, 30);
                string weather = currentWeather;
                if (chance == 0)
                {
                    weather = "THUNDER";
                }
                else if (chance == 1)
                {
                    weather = "RAIN";
                }
                else if (chance == 2 || chance == 3 || chance == 4)
                {
                    weather = "CLOUDS";
                }
                else if (chance == 5 || chance == 6)
                {
                    weather = "FOGGY";
                }
                else
                {
                    weather = "EXTRASUNNY";
                }
                if (currentWeather != weather)
                {
                    currentWeather = weather;
                    foreach (Player player in NAPI.Pools.GetAllPlayers())
                    {
                        player.TriggerEvent("setWeather", currentWeather, true);
                    }
                }
            });
        }

        //RESPAWN PUBLIC VEHICLES
        private void RespawnPublicVehicles(System.Object source, ElapsedEventArgs e)
        {
            NAPI.Task.Run(() =>
            {
                foreach (PublicVehicleSpawn pvs in publicVehicleSpawns)
                {
                    if (pvs.veh == null && !NAPI.Pools.GetAllVehicles().Any(vehicle => pvs.colshape.IsPointWithin(vehicle.Position)))
                    {
                        pvs.CreatePublicVehicle();
                    }
                }
                foreach(JobVehicleSpawn jobVehicleSpawn in jobVehicleSpawns)
                {
                    if(jobVehicleSpawn.Veh == null && !NAPI.Pools.GetAllVehicles().Any(vehicle => jobVehicleSpawn.Col.IsPointWithin(vehicle.Position)))
                    {
                        jobVehicleSpawn.CreateNewVehicle();
                    }
                }
            });
        }



        public void CreateJobVehicles()
        {
            //LAWETY
            jobVehicleSpawns.Add(new JobVehicleSpawn("towtruck", new Vector3(575.0778f, -3037.5444f, 6.0692883f), 0f, VehicleHash.Flatbed, 42, "BASICRPG", new Dictionary<string, object>()
            {
                ["petrol"] = 25,
                ["petroltank"] = 35,
                ["combustion"] = 9,
                ["offroad"] = true,
                ["name"] = "Laweta",
                ["speed"] = 90,
                ["power"] = 0,
                ["veh_brake"] = true,
                ["damage"] = vehicleDataManager.defaultDamage
            }));
            jobVehicleSpawns.Add(new JobVehicleSpawn("towtruck", new Vector3(588.34125f, -3037.7256f, 6.0692873f), 0f, VehicleHash.Flatbed, 42, "BASICRPG", new Dictionary<string, object>()
            {
                ["petrol"] = 25,
                ["petroltank"] = 35,
                ["combustion"] = 9,
                ["offroad"] = true,
                ["name"] = "Laweta",
                ["speed"] = 90,
                ["power"] = 0,
                ["veh_brake"] = true,
                ["damage"] = vehicleDataManager.defaultDamage
            }));
            jobVehicleSpawns.Add(new JobVehicleSpawn("towtruck", new Vector3(581.9523f, -3037.773f, 6.069285f), 0f, VehicleHash.Flatbed, 42, "BASICRPG", new Dictionary<string, object>()
            {
                ["petrol"] = 25,
                ["petroltank"] = 35,
                ["combustion"] = 9,
                ["offroad"] = true,
                ["name"] = "Laweta",
                ["speed"] = 90,
                ["power"] = 0,
                ["veh_brake"] = true,
                ["damage"] = vehicleDataManager.defaultDamage
            }));

            //KOSIARKI
            jobVehicleSpawns.Add(new JobVehicleSpawn("lawnmowing", new Vector3(-1357.8574f, 140.44446f, 56.252647f), -85.41613f, VehicleHash.Mower, 53, "BASICRPG", new Dictionary<string, object>()
            {
                ["offroad"] = true,
                ["name"] = "Kosiarka",
                ["speed"] = 30,
                ["power"] = 0,
                ["veh_brake"] = true
            }));
            jobVehicleSpawns.Add(new JobVehicleSpawn("lawnmowing", new Vector3(-1357.5975f, 136.84306f, 56.25227f), -85.41613f, VehicleHash.Mower, 53, "BASICRPG", new Dictionary<string, object>()
            {
                ["offroad"] = true,
                ["name"] = "Kosiarka",
                ["speed"] = 30,
                ["power"] = 0,
                ["veh_brake"] = true
            }));

            //MYŚLIWY
            jobVehicleSpawns.Add(new JobVehicleSpawn("hunter", new Vector3(-1491.6906f, 4973.8906f, 63.787167f), 86.14462f, VehicleHash.Blazer, 53, "BASICRPG", new Dictionary<string, object>()
            {
                ["offroad"] = true,
                ["name"] = "Blazer",
                ["speed"] = 70,
                ["power"] = 0,
                ["veh_brake"] = true
            }));
            jobVehicleSpawns.Add(new JobVehicleSpawn("hunter", new Vector3(-1491.5835f, 4976.113f, 63.65564f), 86.14462f, VehicleHash.Blazer, 53, "BASICRPG", new Dictionary<string, object>()
            {
                ["offroad"] = true,
                ["name"] = "Blazer",
                ["speed"] = 70,
                ["power"] = 0,
                ["veh_brake"] = true
            }));

            //OGRODNIK
            jobVehicleSpawns.Add(new JobVehicleSpawn("gardener", new Vector3(1542.5646f, 2175.6204f, 78.80925f), 90f, VehicleHash.Kalahari, 53, "BASICRPG", new Dictionary<string, object>()
            {
                ["offroad"] = true,
                ["name"] = "Kalahari",
                ["speed"] = 155,
                ["power"] = 18,
                ["veh_brake"] = true,
                ["petrol"] = 25,
                ["combustion"] = 8,
                ["petroltank"] = 35,
                ["trunk"] = "[]",
                ["damage"] = vehicleDataManager.defaultDamage
            }));
            jobVehicleSpawns.Add(new JobVehicleSpawn("gardener", new Vector3(1542.5675f, 2179.2874f, 78.810936f), 90f, VehicleHash.Kalahari, 53, "BASICRPG", new Dictionary<string, object>()
            {
                ["offroad"] = true,
                ["name"] = "Kalahari",
                ["speed"] = 155,
                ["power"] = 18,
                ["veh_brake"] = true,
                ["petrol"] = 25,
                ["combustion"] = 8,
                ["petroltank"] = 35,
                ["trunk"] = "[]",
                ["damage"] = vehicleDataManager.defaultDamage
            }));

            //WÓZKI WIDŁOWE
            jobVehicleSpawns.Add(new JobVehicleSpawn("forklifts", new Vector3(-555.56934f, -2362.5415f, 13.716819f), 141.7f, VehicleHash.Forklift, 42, "BASICRPG", new Dictionary<string, object>()
            {
                ["offroad"] = true,
                ["name"] = "Wózek widłowy",
                ["speed"] = 25,
                ["power"] = 0,
                ["veh_brake"] = true
            }));
            jobVehicleSpawns.Add(new JobVehicleSpawn("forklifts", new Vector3(-558.0603f, -2360.3997f, 13.716819f), 141.7f, VehicleHash.Forklift, 42, "BASICRPG", new Dictionary<string, object>()
            {
                ["offroad"] = true,
                ["name"] = "Wózek widłowy",
                ["speed"] = 25,
                ["power"] = 0,
                ["veh_brake"] = true
            }));

            //RAFINERIA
            jobVehicleSpawns.Add(new JobVehicleSpawn("refinery", new Vector3(2770.2668f, 1398.3207f, 24.529041f), 87f, (VehicleHash)NAPI.Util.GetHashKey("oiltanker"), 42, "BASICRPG", new Dictionary<string, object>()
            {
                ["offroad"] = true,
                ["name"] = "Cysterna",
                ["speed"] = 115,
                ["power"] = 0,
                ["veh_brake"] = true,
                ["damage"] = vehicleDataManager.defaultDamage,
                ["petrol"] = 25,
                ["combustion"] = 11,
                ["petroltank"] = 35,
                ["oiltank"] = 0.0f
            }));
            jobVehicleSpawns.Add(new JobVehicleSpawn("refinery", new Vector3(2771.487f, 1404.5233f, 24.533354f), 87f, (VehicleHash)NAPI.Util.GetHashKey("oiltanker"), 42, "BASICRPG", new Dictionary<string, object>()
            {
                ["offroad"] = true,
                ["name"] = "Cysterna",
                ["speed"] = 115,
                ["power"] = 0,
                ["veh_brake"] = true,
                ["damage"] = vehicleDataManager.defaultDamage,
                ["petrol"] = 25,
                ["combustion"] = 11,
                ["petroltank"] = 35,
                ["oiltank"] = 0.0f
            }));

            jobVehicleSpawns.Add(new JobVehicleSpawn("diver", new Vector3(1335.4865f, 4230.0303f, 30.685176f), -10f, VehicleHash.Seashark, 42, "BASICRPG", new Dictionary<string, object>()
            {
                ["name"] = "Seashark",
                ["speed"] = 85,
                ["power"] = 0,
                ["veh_brake"] = true
            }));
            jobVehicleSpawns.Add(new JobVehicleSpawn("diver", new Vector3(1327.3524f, 4231.6084f, 30.685176f), -10f, VehicleHash.Seashark, 42, "BASICRPG", new Dictionary<string, object>()
            {
                ["name"] = "Seashark",
                ["speed"] = 85,
                ["power"] = 0,
                ["veh_brake"] = true
            }));
        }

        public void CreateF1Track()
        {
            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("ch_prop_track_paddock_01"), new Vector3(1278.25700000, -3360.33800000, 3.93215000), new Vector3(0, 0, 0)));

            objList.Add(NAPI.Object.CreateObject(-608407618, new Vector3(1304.71300000, -3385.68800000, 4.68288200), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("ch_prop_track_pit_garage_01a"), new Vector3(1304.98500000, -3422.50800000, 4.67336100), new Vector3(0, 0, 180)));

            objList.Add(NAPI.Object.CreateObject(1299320654, new Vector3(1392.39800000, -3397.19000000, 4.67336100), new Vector3(0, 0, -90.00001)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1496.37500000, -3385.68800000, 4.68288200), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("ch_prop_track_bend_bar_lc"), new Vector3(1679.69500000, -3398.57400000, 4.66664700), new Vector3(0, 0, 90.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1602.32100000, -3385.68800000, 4.68288200), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_L_b"), new Vector3(1705.42700000, -3447.77600000, 4.66676900), new Vector3(0, 0, 269.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(1755.29300000, -3455.45700000, 4.68312600), new Vector3(0, 0, -7.1046765E-05)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(1794.15800000, -3441.93300000, 4.68312600), new Vector3(0, 0, -179.99992)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(1839.67100000, -3440.55700000, 4.68312600), new Vector3(0, 0, 149.99992)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(1879.73800000, -3462.12900000, 4.68361500), new Vector3(0, 0, 119.99991)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(1903.68100000, -3500.85000000, 4.68385900), new Vector3(0, 0, 89.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1908.87600000, -3684.86000000, 4.68300400), new Vector3(0, 0, -90.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1908.87600000, -3579.02100000, 4.68300400), new Vector3(0, 0, -90.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("ch_prop_track_bend_bar_lc"), new Vector3(1921.75200000, -3762.41500000, 4.63820500), new Vector3(0, 0, 269.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_L_b"), new Vector3(1970.95800000, -3788.21800000, 4.66921000), new Vector3(0, 0, 90.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1983.88100000, -3865.77000000, 4.68300400), new Vector3(0, 0, -90.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_L_b"), new Vector3(1970.95800000, -3942.76600000, 4.66921000), new Vector3(0, 0, -7.563043E-05)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1893.39200000, -3955.65800000, 4.68288200), new Vector3(0, 0, -7.906817E-05)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_L"), new Vector3(1816.93900000, -3941.22000000, 4.64821400), new Vector3(0, 0, 270.00007)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1800.65500000, -3863.60200000, 4.68202800), new Vector3(0, 0, 95.0001)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(1790.69500000, -3784.97900000, 4.68288200), new Vector3(0, -0, 94.99992)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1767.28300000, -3712.77700000, 4.68202800), new Vector3(0, 0, 110.00008)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_M"), new Vector3(1739.21600000, -3648.23200000, 4.68385900), new Vector3(0, -0, 109.99993)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_M"), new Vector3(1718.53500000, -3636.62000000, 4.68239400), new Vector3(0, 0, 290.00008)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_M"), new Vector3(1692.02600000, -3556.55100000, 4.68239400), new Vector3(0, -0, 94.99993)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(1703.14200000, -3599.18200000, 4.71718400), new Vector3(0, 0, 275.0001)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_M"), new Vector3(1667.50200000, -3540.40600000, 4.69533300), new Vector3(0, -0, 140.00006)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1604.07200000, -3540.56200000, 4.68300400), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1498.05900000, -3540.56200000, 4.68300400), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1392.21500000, -3540.56200000, 4.68300400), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1286.31500000, -3540.56200000, 4.68300400), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1180.45600000, -3540.56200000, 4.68300400), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1158.26300000, -3540.55900000, 4.67812200), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1113.01300000, -3385.67900000, 4.69606600), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(1033.79300000, -3388.70900000, 4.69814100), new Vector3(-0, 0, 179.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(986.18320000, -3401.83900000, 4.69814100), new Vector3(-0, 0, 194.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(943.64320000, -3426.85900000, 4.69814100), new Vector3(-0, 0, 209.99992)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_Bar_L"), new Vector3(915.10310000, -3468.23900000, 4.69814100), new Vector3(-0, 0, 224.99994)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_15D_Bar"), new Vector3(942.10310000, -3511.40900000, 4.69814100), new Vector3(0, 0, 314.999935)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Bend_30D_Bar"), new Vector3(985.03310000, -3533.41900000, 4.69814100), new Vector3(0, 0, 329.999939)));

            objList.Add(NAPI.Object.CreateObject(NAPI.Util.GetHashKey("stt_Prop_Track_Straight_LM_Bar"), new Vector3(1052.55300000, -3538.72900000, 4.67812200), new Vector3(0, 0, -2.00006)));









        }

        public Vector3 QtoVector(Quaternion q)
        {
            float x = q.X, y = q.Y, z = q.Z, w = q.W;
            Vector3 v = new Vector3(0, 0, 1);
            v.X = 2 * (x * z - w * y);
            v.Y = 2 * (y * z + w * x);
            v.Z = 1 - 2 * (x * x + y * y);
            Console.WriteLine(v.ToString());
            return v;
        }

        public Vector3 toEuler(Quaternion q)
        {
            Vector3 retVal = new Vector3();

            // roll (x-axis rotation)
            double sinr_cosp = 2.0 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1.0 - 2.0 * (q.X * q.X + q.Y * q.Y);
            retVal.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = 2.0 * (q.W * q.Y - q.Z * q.X);

            if (Math.Abs(sinp) >= 1)
            {
                retVal.Y = 90.0f; // use 90 degrees if out of range
            }
            else
            {
                retVal.Y = (float)Math.Asin(sinp);
            }


            // yaw (z-axis rotation)
            double siny_cosp = 2.0 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1.0 - 2.0 * (q.Y * q.Y + q.Z * q.Z);
            retVal.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            // Rad to Deg
            retVal.X *= (float)(180.0f / Math.PI);
            retVal.Y *= (float)(180.0f / Math.PI);
            retVal.Z *= (float)(180.0f / Math.PI);

            return retVal;
        }
    }
}

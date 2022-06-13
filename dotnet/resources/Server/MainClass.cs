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
    public partial class MainClass : Script
    {
        private Vector3 defaultSpawn = new Vector3(1894.2115f, 3715.0637f, 32.762226f);


        public List<ChangingRoom> changingRooms = new List<ChangingRoom>();
        public List<CarDealer> carDealers = new List<CarDealer>();
        public List<VehicleStorage> vehicleStorages = new List<VehicleStorage>();
        public List<PublicVehicleSpawn> publicVehicleSpawns = new List<PublicVehicleSpawn>();
        public List<VehicleMechanic> vehicleMechanics = new List<VehicleMechanic>();
        public List<CarWash> carWashes = new List<CarWash>();
        public List<JobVehicleSpawn> jobVehicleSpawns = new List<JobVehicleSpawn>();
        public List<PetrolStation> petrolStations = new List<PetrolStation>();
        private SellVehicle sellVehicle = new SellVehicle();
        public CarTrader carTrader = new CarTrader(new Vector3(-1565.0809f, -554.6135f, 114.57642), 123f, new Vector3(-1567.3447f, -555.9636f, 114.44851f));
        private DrivingLicences drivingLicences = new DrivingLicences();
        private SpeedometerColor speedometerColor = new SpeedometerColor(new Vector3(387.58972f, 3587.3884f, 33.29227f));
        private CarMarket carMarket;

        // Supplier supplier;

        public List<Report> reportsList = new List<Report>();

        List<GTANetworkAPI.Object> objList = new List<GTANetworkAPI.Object>();

        private string currentWeather = "EXTRASUNNY";

        private int secondsPassed = 0;

        public Vehicle ShowVeh = null;

        internal static CultureInfo CustomCulture = new CultureInfo("pl-PL");

        List<JobVehicle> jobVehicles;



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
            NAPI.Server.SetDefaultSpawnLocation(defaultSpawn, heading: 125f);
            NAPI.Server.SetGlobalServerChat(false);


            //INSTANTIATE THINGS
            PlayerDataManager.SetPaths();
            LSPD.InstantiateLSPD();
            DataManager.InstantiatePetrolStationsData();
            DoorManager.InstantiateMissionRowLSPD();
            OrgManager.InstantiateOrgs();
            Banking.InstantiateATMs();
            Houses.InstantiateHouses();
            NicknameChange.InstantiateNicknameChange();


            Peds.CreateDepartmentPed(new Vector3(-1561.8259f, -559.97095f, 114.57642), 110f, "Zmiana nicku");

            //SET JOB VEHICLES SETTINGS
            jobVehicles = new List<JobVehicle>()
            {
                new JobVehicle(VehicleHash.Flatbed, "towtruck", 25, 35, 9, true, "Laweta", 90, 0, true, "", VehicleDataManager.defaultDamage, "BASICRPG", 42),
                new JobVehicle(VehicleHash.Mower, "lawnmowing", 0, 0, 0, true, "Kosiarka", 30, 50, true, "", "", "BASICRPG", 53),
                new JobVehicle(VehicleHash.Blazer, "hunter", 0, 0, 0, true, "Blazer", 70, 0, true, "", "", "BASICRPG", 53),
                new JobVehicle(VehicleHash.Kalahari, "gardener", 25, 35, 8, true, "Kalahari", 155, 18, true, "[]", VehicleDataManager.defaultDamage, "BASICRPG", 53),
                new JobVehicle(VehicleHash.Forklift, "forklifts", 0, 0, 0, true, "Wózek widłowy", 25, 0, true, "", "", "BASICRPG", 42),
                new JobVehicle((VehicleHash)NAPI.Util.GetHashKey("oiltanker"), "refinery", 25, 35, 11, true, "Cysterna", 115, 0, true, "", VehicleDataManager.defaultDamage, "BASICRPG", 42),
                new JobVehicle(VehicleHash.Blazer, "diver", 0, 0, 0, true, "Seashark", 85, 0, true, "", "", "BASICRPG", 42)
            };

            //INSTANTIATE UTILS

            CreatePublicVehicles();

            CreatePetrolStations();

            CreateVehicleMechanics();

            CreateChangingRooms();

            CreateVehicleStorages();

            CreatePaintShops();

            CreateTeleports();

            CreateCarWashes();

            CreateCarDealers();

            CreateJobVehicles();

            CreateF1Track();

            Wheels.InitiateWheels();

            new VehicleVisu(new Vector3(-211.95407f, -1324.0376f, 30.890387f));

            //SET NEW BONUS

            PayoutManager.SetNewBonus();


            //INSTANTIATE JOBS

            Diver.InstantiateDiver();

            Forklifts.InstantiateForklifts(new Vector3(-553.48566f, -2352.2178f, 13.994338f));
            FisherMan.InstantiateFisherMan();
            Warehouse.InstantiateWarehouse(new Vector3(-87.66176f, 6494.595f, 32.100685f));
            DebrisCleaner.InstantiateDebrisCleaner(new Vector3(-1705.2916f, -994.86456f, 6.161489f));
            Lawnmowing.InstantiateLawnmowing(new Vector3(-1348.476f, 142.67131f, 56.437782f));
            Hunter.InstantiateHunter(new Vector3(-1490.5903f, 4981.5283f, 63.345905f), new Vector3(-1493.011f, 4971.482f, 63.92059f), 91.5f);
            TowTrucks.InstantiateTowTrucks(new Vector3(593.4641f, -3043.515f, 6.1697326f), new Vector3(561.56323f, -3037.9924f, 6.091237f));
            Gardener.InstantiateGardener();
            Refinery.InstantiateRefinery();
            
            PayoutManager.bonusTime = DateTime.Now.AddHours(1);
            
            TowTrucks.AddSpawningPosition(new Vector3(516.8819f, -3054.159f, 6.0696325f), 1);
            
            TowTrucks.AddSpawningPosition(new Vector3(509.95694f, -3054.1301f, 6.0696325f), 1);

             carMarket = new CarMarket(new Vector3(-112.2122f, -2015.1759f, 18.016949f));

            //supplier = new Supplier(new Vector3(-17.289665f, 6303.619f, 31.374657f));


            //INSTANTIATE IPL'S

            NAPI.World.RequestIpl("vw_casino_main");
            NAPI.World.RequestIpl("vw_casino_garage");
            NAPI.World.RequestIpl("vw_casino_carpark");
            NAPI.World.RequestIpl("vw_casino_penthouse");
            drivingLicences.CreateLicenceB(new Vector3(1001.5448f, -2317.2195f, 30.962214f), new Vector3(1003.244f, -2317.394f, 30.962214f), new Vector3(1007.4232f, -2318.8247f, 30.963396f));



            //TIMERS

            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += ValuesToDB;
            timer.Enabled = true;

            System.Timers.Timer messages = new System.Timers.Timer(1800000);
            messages.Elapsed += InfoMessages;
            messages.Enabled = true;

            System.Timers.Timer globalTimer = new System.Timers.Timer(60000);
            globalTimer.Elapsed += GlobalTimer;
            globalTimer.Enabled = true;

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

            System.Timers.Timer setNewBonus = new System.Timers.Timer(3600000);
            setNewBonus.Elapsed += SetNewBonus;
            setNewBonus.Enabled = true;

            System.Timers.Timer updateArrestTimes = new System.Timers.Timer(60000);
            updateArrestTimes.Elapsed += UpdateArrestTimes;
            updateArrestTimes.Enabled = true;

            System.Timers.Timer respawnPublicVehicles = new System.Timers.Timer(60000);
            respawnPublicVehicles.Elapsed += RespawnPublicVehicles;
            respawnPublicVehicles.Enabled = true;

            System.Timers.Timer savePlayersJobData = new System.Timers.Timer(5000);
            savePlayersJobData.Elapsed += SavePlayersJobData;
            savePlayersJobData.Enabled = true;

            System.Timers.Timer updateGrass = new System.Timers.Timer(20000);
            updateGrass.Elapsed += UpdateGrass;
            updateGrass.Enabled = true;

            //INSTANTIATE A COMMAND MANAGER
            CommandsManager.InstantiateCommandsManager(carDealers, reportsList, ref tuneBusinesses);


            //LOAD VEHICLES FROM DB (5S DELAY)
            NAPI.Task.Run(() =>
            {
                VehicleDataManager.LoadPersonalVehiclesFromDB();
                carMarket.CreateMarketVehicles();
            }, delayTime: 5000);

            Thread awaitInput = new Thread(new ThreadStart(AwaitInput));
            awaitInput.Start();
        }

        //remote commands
        public void AwaitInput()
        {
            while (true)
            {
                string text = Console.ReadLine();
                if(text.Length > 0)
                {
                    if (text[0] != '/')
                    {
                        PlayerDataManager.SendRemoteMessageToAllPlayers(text);
                    }
                    else
                    {
                        CommandsManager.ExecuteConsoleCommand(text);
                    }
                }
            }
        }
    }
}

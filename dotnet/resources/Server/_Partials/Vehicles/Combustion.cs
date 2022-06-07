using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {

        [RemoteEvent("savePetrolLevel")]
        public void SavePetrolLevel(Player player, Vehicle vehicle, float petrol)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("petrol"))
            {
                if (vehicle.HasSharedData("owner"))
                    VehicleDataManager.UpdateVehiclesPetrol(vehicle, petrol);
                vehicle.SetSharedData("petrol", petrol);
            }
        }

        [RemoteEvent("petrol_checkStation")]
        public void Petrol_CheckStation(Player player, ColShape distributor)
        {
            foreach (PetrolStation petrolStation in petrolStations)
            {
                if (petrolStation.distributorColShape == distributor)
                {
                    if (petrolStation.currentVehicle != null)
                    {
                        if (!(petrolStation.currentVehicle.Exists && petrolStation.currentVehicle.HasSharedData("petrol_refueling") && petrolStation.currentVehicle.GetSharedData<bool>("petrol_refueling")))
                        {
                            if (petrolStation.currentVehicle.Exists)
                            {
                                petrolStation.currentVehicle.SetSharedData("petrol_refueling", true);
                                player.TriggerEvent("petrol_startRefueling", petrolStation.currentVehicle, DataManager.GetPetrolPriceAtIndex(petrolStations.IndexOf(petrolStation)));
                            }
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Nie można rozpocząć tankowania!");
                        }
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Na stanowisku nie ma odpowedniego pojazdu!");
                    }
                    break;
                }
            }
        }

        [RemoteEvent("petrol_cancelRefueling")]
        public void Petrol_CancelRefueling(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                vehicle.SetSharedData("petrol_refueling", false);
            }
        }

        [RemoteEvent("petrol_stopRefueling")]
        public void Petrol_StopRefueling(Player player, ColShape shape, Vehicle vehicle, int cost, float petrol)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (PlayerDataManager.UpdatePlayersMoney(player, -1 * cost))
                {
                    VehicleDataManager.UpdateVehiclesPetrol(vehicle, Math.Clamp(vehicle.GetSharedData<float>("petrol") + petrol, 0, vehicle.GetSharedData<int>("petroltank")));
                    PlayerDataManager.NotifyPlayer(player, "Pomyślnie zatankowano pojazd!");
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie stać Cię na to!");
                }
                vehicle.SetSharedData("petrol_refueling", false);
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
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
    }
}

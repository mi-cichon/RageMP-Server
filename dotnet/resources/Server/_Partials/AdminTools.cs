using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace ServerSide
{
    partial class MainClass
    {
        //showroom
        [RemoteEvent("createShowroomVehicle")]
        public void CreateShowroomVehicle(Player player, string model)
        {
            if (ShowVeh != null && ShowVeh.Exists)
            {
                ShowVeh.Delete();
            }
            ShowVeh = NAPI.Vehicle.CreateVehicle(uint.Parse(model), new Vector3(-105.15191f, -941.76086f, 463.79953f), -120.86137f, 14, 14);
            ShowVeh.SetSharedData("washtime", DateTime.Now.AddYears(1).ToString());
            ShowVeh.Rotation = new Vector3(0, 0, -42f);
        }

        [RemoteEvent("saveShowroomVehicle")]
        public void SaveShowroomVehicle(Player player)
        {
            if (ShowVeh != null && ShowVeh.Exists)
            {
                File.AppendAllText(@"notes.txt", ShowVeh.Model.ToString() + "," + Environment.NewLine, new UTF8Encoding(false, true));
                PlayerDataManager.NotifyPlayer(player, "Notatka zapisana w pliku!");
            }
        }

        //admin commands - context menu

        [RemoteEvent("admin_deleteCar")]
        public void AdminDeleteCar(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") != "dealer")
                {
                    if(vehicle.HasSharedData("market") && vehicle.GetSharedData<bool>("market"))
                    {
                        PlayerDataManager.NotifyPlayer(player, $"Pojazd jest na giełdzie!");
                        return;
                    }
                    if (vehicle.GetSharedData<string>("type") == "personal")
                    {
                        VehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                        PlayerDataManager.NotifyPlayer(player, $"Pojazd o ID: {vehicle.GetSharedData<Int32>("id").ToString()} został przeniesiony do przechowalni!");
                        vehicle.Delete();
                    }
                    else if (vehicle.GetSharedData<string>("type") == "LSPD")
                    {
                        LSPD.SetVehicleSpawned(vehicle.GetSharedData<int>("id"), false);
                        PlayerDataManager.NotifyPlayer(player, $"Pojazd został usunięty!");
                        vehicle.Delete();
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, $"Pojazd został usunięty!");
                        vehicle.Delete();
                    }
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie można usunąć pojazdu z komisu!");
                }
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
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
                        if (vehicle.HasSharedData("market") && vehicle.GetSharedData<bool>("market"))
                        {
                            PlayerDataManager.NotifyPlayer(player, $"Pojazd jest na giełdzie!");
                            return;
                        }
                        vehicle.SetSharedData("lastpos", player.Position);
                        vehicle.Position = player.Position;
                        if (vehicle.GetSharedData<bool>("veh_brake"))
                        {
                            vehicle.SetSharedData("veh_brake", false);
                            NAPI.Task.Run(() =>
                            {
                                vehicle.SetSharedData("veh_brake", true);
                                VehicleDataManager.UpdateVehiclesLastPos(vehicle);
                            }, 5000);
                        }
                        else
                        {
                            VehicleDataManager.UpdateVehiclesLastPos(vehicle);
                        }
                    }
                    else
                    {
                        vehicle.Position = player.Position;
                    }
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Nie można przenieść pojazdu z komisu!");
                }
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }

        [RemoteEvent("admin_lastDriver")]
        public void AdminLastDriver(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("admin_lastDriver"))
                {
                    PlayerDataManager.SendInfoToPlayer(player, "Ostatni kierowca pojazdu: " + vehicle.GetSharedData<string>("admin_lastDriver"));
                }
                else if (vehicle.HasSharedData("drivers"))
                {
                    PlayerDataManager.SendInfoToPlayer(player, "Ostatni kierowcy pojazdu: " + vehicle.GetSharedData<string>("drivers"));
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Pojazd nie miał kierowcy!");
                }
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }

        [RemoteEvent("admin_carOwner")]
        public void AdminCarOwner(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("id"))
                {
                    string vehowner = VehicleDataManager.GetVehiclesOwnerName(vehicle.GetSharedData<int>("id").ToString());
                    if (vehowner != "")
                    {
                        PlayerDataManager.SendInfoToPlayer(player, "Pojazd należy do " + vehowner);
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono właściciela!");
                    }
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Pojazd nie ma właściciela!");
                }
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
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
                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
        }

        [RemoteEvent("admin_fixCar")]
        public void AdminFixCar(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists)
            {
                if (vehicle.HasSharedData("owner"))
                    VehicleDataManager.RepairVehicle(vehicle);
                else
                {
                    vehicle.Repair();
                }

            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
            }
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
                        PlayerDataManager.SendInfoToPlayer(report.informer, $"Twój raport o ID {report.id.ToString()} został przyjęty przez {player.GetSharedData<string>("username")}");
                    }
                    solved = true;
                    PlayerDataManager.SendInfoToPlayer(player, $"Raport przyjęty: {report.id.ToString()}: {report.informerName} => {report.reportedName}, powód: {report.description}");
                    reportsList.Remove(report);
                    break;
                }
            }
            if (!solved)
                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono raportu!");
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
                    PlayerDataManager.SendInfoToPlayer(player, $"Raport usunięty: {report.id.ToString()}: {report.informerName} => {report.reportedName}, powód: {report.description}");
                    reportsList.Remove(report);
                    break;
                }
            }
            if (!removed)
                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono raportu!");
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

        //tp to waypoint with vehicle
        [RemoteEvent("tpWPVeh")]
        public void TpWPVeh(Player player, Vector3 point)
        {
            if (player.Vehicle != null)
            {
                player.Vehicle.Position = point;
            }
        }

        //USUWANIE POJAZDU
        [RemoteEvent("removeIndicatedVehicle")]
        public void RemovePointedVehicle(Player player, Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("type") && vehicle.GetSharedData<string>("type") != "dealer" && vehicle.GetSharedData<string>("type") != "public")
            {
                if (vehicle.HasSharedData("market") && vehicle.GetSharedData<bool>("market"))
                {
                    PlayerDataManager.NotifyPlayer(player, $"Pojazd jest na giełdzie!");
                    return;
                }
                if (vehicle.GetSharedData<string>("type") == "personal")
                {
                    VehicleDataManager.UpdateVehicleSpawned(vehicle, false);
                    PlayerDataManager.NotifyPlayer(player, $"Pojazd o ID: {vehicle.GetSharedData<Int32>("id").ToString()} został przeniesiony do przechowalni!");
                    vehicle.Delete();
                }
                else if (vehicle.GetSharedData<string>("type") == "LSPD")
                {
                    LSPD.SetVehicleSpawned(vehicle.GetSharedData<int>("id"), false);
                    PlayerDataManager.NotifyPlayer(player, $"Pojazd został usunięty!");
                    vehicle.Delete();
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, $"Pojazd został usunięty!");
                    vehicle.Delete();
                }
            }
        }

        [RemoteEvent("savePos")]
        public void SavePos(Player player, Vector3 position)
        {
            string pos = $"new mp.Vector3({position.X.ToString().Replace(',', '.')}, {position.Y.ToString().Replace(',', '.')}, {position.Z.ToString().Replace(',', '.')}),";
            File.AppendAllText(@"positions.txt", pos + Environment.NewLine, new UTF8Encoding(false, true));
            PlayerDataManager.NotifyPlayer(player, "Zapisano pozycję!");
        }

        [RemoteEvent("appendGrassPos")]
        public void AppendGrassPos(Player player, Vector3 pos)
        {
            string position = $"new mp.Vector3({pos.X.ToString().Replace(',', '.')}, {pos.Y.ToString().Replace(',', '.')}, {pos.Z.ToString().Replace(',', '.')}),";
            File.AppendAllText(@"grassPositions.txt", position + Environment.NewLine, new UTF8Encoding(false, true));
        }
    }
}

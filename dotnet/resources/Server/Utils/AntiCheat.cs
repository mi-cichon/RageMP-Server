using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class AntiCheat
    {
        public bool ShouldPlayerEnterThisVehicle(Player player, Vehicle vehicle)
        {
            if (player.GetSharedData<int>("vehSeat") == 0)
            {
                if (!(player.HasSharedData("handCuffed") && player.GetSharedData<bool>("handCuffed")))
                {
                    if (player.HasSharedData("job") && player.GetSharedData<string>("job") != "")
                    {
                        if (player.HasSharedData("jobveh") && player.GetSharedData<int>("jobveh") == vehicle.Id)
                        {
                            return true;
                        }
                        else if (player.GetSharedData<int>("jobveh") == -1111 && vehicle.HasSharedData("jobtype") && vehicle.GetSharedData<string>("jobtype") == player.GetSharedData<string>("job") && !vehicle.GetSharedData<bool>("player"))
                        {
                            return true;
                        }
                        else if (player.GetSharedData<string>("job") == "lspd" && vehicle.GetSharedData<string>("type") == "lspd")
                        {
                            if (player.HasSharedData("licenceBp") && player.GetSharedData<bool>("licenceBp"))
                            {
                                if (!player.GetSharedData<bool>("nodriving"))
                                    return true;
                                else
                                    return false;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (vehicle.GetSharedData<string>("type") == "public")
                    {
                        return true;
                    }
                    if (vehicle.GetSharedData<string>("type") == "furka")
                    {
                        return true;
                    }
                    if (vehicle.GetSharedData<string>("type") == "personal" && (vehicle.GetSharedData<Int64>("owner").ToString() == player.GetSharedData<string>("socialclub") || vehicle.GetSharedData<int>("id") == player.GetSharedData<int>("carkeys") || (vehicle.HasSharedData("orgId") && player.HasSharedData("orgId") && vehicle.GetSharedData<int>("orgId") == player.GetSharedData<int>("orgId"))) && !(vehicle.HasSharedData("mech") && vehicle.GetSharedData<bool>("mech")))
                    {
                        if (player.HasSharedData("licenceBp") && player.GetSharedData<bool>("licenceBp"))
                        {
                            if (!player.GetSharedData<bool>("nodriving"))
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }
    }
}

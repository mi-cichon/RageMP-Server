using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;
using MySqlConnector;
using Newtonsoft.Json;
using Server.Database;

namespace ServerSide
{

    public static class OrgManager
    {

        public static List<Organization> orgs = new List<Organization>();

        public static void InstantiateOrgs()
        {
            using var context = new ServerDB();
            var organizations = context.Orgs.ToList();

            foreach(var org in organizations)
            {
                Organization o = new Organization(org.Id, org.Name, org.Tag, UInt64.Parse(org.Owner), JsonConvert.DeserializeObject<List<ulong>>(org.Members), JsonConvert.DeserializeObject<List<ulong>>(org.Requests), JsonConvert.DeserializeObject<List<int>>(org.Vehicles), JsonConvert.DeserializeObject<List<int>>(org.VehicleRequests));
                orgs.Add(o);
            }

            ColShape col = NAPI.ColShape.CreateCylinderColShape(new Vector3(-1570.2968f, -551.0935f, 114.57582f), 1.2f, 2.0f);
            col.SetSharedData("type", "org");
        }

        public static void SetPlayersOrg(Player player)
        {
            foreach (Organization org in orgs)
            {
                if (org.owner == player.SocialClubId)
                {
                    player.SetSharedData("orgId", org.id);
                    player.SetSharedData("orgOwner", true);
                    player.SetSharedData("orgTag", org.tag);
                    player.SetSharedData("orgName", org.name);
                }
                else if (org.members.Contains(player.SocialClubId))
                {
                    player.SetSharedData("orgId", org.id);
                    player.SetSharedData("orgTag", org.tag);
                    player.SetSharedData("orgName", org.name);
                    player.SetSharedData("orgOwner", false);
                    break;
                }

            }
        }

        public static void SetVehiclesOrg(Vehicle vehicle)
        {
            foreach (Organization org in orgs)
            {
                if (org.vehicles.Contains(vehicle.GetSharedData<Int32>("id")))
                {
                    vehicle.SetSharedData("orgId", org.id);
                    break;
                }
            }
        }

        public static int CreateOrg(Player owner, string name, string tag)
        {
            using var context = new ServerDB();
            var sameNameOrgs = context.Orgs.Where(o => o.Name.ToLower() == name.ToLower()).ToList();
            if(sameNameOrgs.Count > 0)
            {
                return 1;
            }
            else
            {
                Organization org = new Organization(name, tag, owner.SocialClubId);
                org.SaveOrgToDataBase();
                orgs.Add(org);
                owner.SetSharedData("orgName", name);
                owner.SetSharedData("orgOwner", true);
                owner.SetSharedData("orgTag", org.tag);
                NAPI.Task.Run(() =>{ owner.SetSharedData("orgId", org.id); }, 2000);
                return 0;
            }
        }

        public static bool DeleteOrg(ulong player, int orgId)
        {
            foreach (Organization org in orgs)
            {
                if (org.id == orgId && org.owner == player)
                {
                    org.Delete();
                    orgs.Remove(org);
                    return true;
                }
            }
            return false;
        }

        public static bool AddMemberToOrg(ulong playerId, int orgId)
        {
            foreach (Organization org in orgs)
            {
                if (org.id == orgId)
                {
                    if (org.members.Contains(playerId))
                    {
                        return false;
                    }
                    else
                    {
                        org.members.Add(playerId);
                        org.SaveOrgToDataBase();
                        foreach (Player player in NAPI.Pools.GetAllPlayers())
                        {
                            if (player.SocialClubId == playerId)
                            {
                                player.SetSharedData("orgId", org.id);
                                player.SetSharedData("orgName", org.name);
                                player.SetSharedData("orgOwner", false);
                                player.SetSharedData("orgTag", org.tag);
                                break;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool RemoveMemberFromOrg(ulong playerId, int orgId)
        {
            foreach (Organization org in orgs)
            {
                if (org.id == orgId)
                {
                    if (!org.members.Contains(playerId))
                    {
                        return false;
                    }
                    else
                    {
                        org.members.Remove(playerId);
                        org.SaveOrgToDataBase();
                        foreach (Player player in NAPI.Pools.GetAllPlayers())
                        {
                            if (player.SocialClubId == playerId)
                            {
                                player.SetSharedData("orgId", 0);
                                player.SetSharedData("orgName", "");
                                player.SetSharedData("orgOwner", false);
                                player.SetSharedData("orgTag", "");
                                break;
                            }
                        }
                        foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                        {
                            if (vehicle.HasSharedData("owner") && (uint)vehicle.GetSharedData<Int64>("owner") == playerId && vehicle.HasSharedData("orgId") && vehicle.GetSharedData<Int32>("orgId") == orgId)
                            {
                                vehicle.SetSharedData("orgId", 0);
                                break;
                            }
                        }
                        foreach (int vehId in PlayerDataManager.GetPlayersVehiclesById(playerId))
                        {
                            if (org.vehicleRequests.Contains(vehId))
                            {
                                org.vehicleRequests.Remove(vehId);
                                org.SaveOrgToDataBase();
                            }
                            if (org.vehicles.Contains(vehId))
                            {
                                org.vehicles.Remove(vehId);
                                org.SaveOrgToDataBase();
                                break;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AddRequestToOrg(ulong playerId, int orgId)
        {
            foreach (Organization org in orgs)
            {
                if (org.id == orgId)
                {
                    if (org.requests.Contains(playerId))
                    {
                        return false;
                    }
                    else
                    {
                        org.requests.Add(playerId);
                        org.SaveOrgToDataBase();
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool RemoveRequestFromOrg(ulong playerId, int orgId)
        {
            foreach (Organization org in orgs)
            {
                if (org.id == orgId)
                {
                    if (!org.requests.Contains(playerId))
                    {
                        return false;
                    }
                    else
                    {
                        org.requests.Remove(playerId);
                        org.SaveOrgToDataBase();
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AcceptRequest(ulong playerId, int orgId)
        {
            foreach (Organization org in orgs)
            {
                if (org.id == orgId)
                {
                    if (!org.requests.Contains(playerId))
                    {
                        return false;
                    }
                    else
                    {
                        foreach (Organization organization in orgs)
                        {
                            if (organization != org && organization.members.Contains(playerId))
                            {
                                org.requests.Remove(playerId);
                                org.SaveOrgToDataBase();
                                return false;
                            }
                        }
                        org.requests.Remove(playerId);
                        AddMemberToOrg(playerId, orgId);
                        org.SaveOrgToDataBase();
                        foreach (Player player in NAPI.Pools.GetAllPlayers())
                        {
                            if (player.SocialClubId == playerId)
                            {
                                player.SetSharedData("orgId", org.id);
                                player.SetSharedData("orgName", org.name);
                                player.SetSharedData("orgOwner", false);
                                player.SetSharedData("orgTag", org.tag);
                                break;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool RemoveVehicleFromOrg(int vehId, int orgId)
        {
            foreach (Organization org in orgs)
            {
                if (org.id == orgId)
                {
                    if (!org.vehicles.Contains(vehId) && !org.vehicleRequests.Contains(vehId))
                    {
                        return false;
                    }
                    else
                    {
                        if (org.vehicles.Contains(vehId))
                            org.vehicles.Remove(vehId);
                        if (org.vehicleRequests.Contains(vehId))
                            org.vehicleRequests.Remove(vehId);
                        org.SaveOrgToDataBase();

                        foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                        {
                            if (vehicle.HasSharedData("orgId") && vehicle.GetSharedData<Int32>("orgId") == orgId)
                            {
                                vehicle.SetSharedData("orgId", 0);
                                break;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool SendVehicleRequest(ulong playerId, int vehId, int orgId)
        {
            foreach (Organization org in orgs)
            {

                if (org.id == orgId)
                {
                    if (org.vehicleRequests.Contains(vehId) || org.vehicles.Contains(vehId))
                    {
                        return false;
                    }
                    else
                    {
                        if (org.members.Contains(playerId) || org.owner == playerId)
                        {
                            org.vehicleRequests.Add(vehId);
                            org.SaveOrgToDataBase();
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                }
            }
            return false;
        }

        public static bool RemoveVehicleRequest(int vehId, int orgId)
        {
            foreach (Organization org in orgs)
            {
                if (org.id == orgId)
                {
                    if (!org.vehicleRequests.Contains(vehId))
                    {
                        return false;
                    }
                    else
                    {
                        org.vehicleRequests.Remove(vehId);
                        org.SaveOrgToDataBase();
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AcceptVehicleRequest(int vehId, int orgId)
        {
            foreach (Organization org in orgs)
            {
                if (org.id == orgId)
                {
                    if (!org.vehicleRequests.Contains(vehId))
                    {
                        return false;
                    }
                    else
                    {
                        org.vehicleRequests.Remove(vehId);
                        org.vehicles.Add(vehId);
                        org.SaveOrgToDataBase();
                        foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                        {
                            if (vehicle != null && vehicle.Exists && vehicle.HasSharedData("id") && vehicle.GetSharedData<int>("id") == vehId)
                            {
                                vehicle.SetSharedData("orgId", orgId);
                                break;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    }




    public class Organization
    {
        public int id { get; set; }
        public string name { get; set; }
        public string tag { get; set; }
        public ulong owner { get; set; }

        public List<int> vehicleRequests = new List<int>();

        public List<int> vehicles = new List<int>();

        public List<ulong> members = new List<ulong>();

        public List<ulong> requests = new List<ulong>();

        public Organization(int id, string name, string tag, ulong owner, List<ulong> members, List<ulong> requests, List<Int32> vehicles, List<Int32> vehicleRequests)
        {
            this.id = id;
            this.name = name;
            this.tag = tag;
            this.owner = owner;
            this.members = members;
            this.requests = requests;
            this.vehicles = vehicles;
            this.vehicleRequests = vehicleRequests;
        }
        public Organization(string name, string tag, ulong owner)
        {
            this.id = 0;
            this.name = name;
            this.tag = tag;
            this.owner = owner;
            this.members.Add(owner);
        }

        public void SaveOrgToDataBase()
        {
            using var context = new ServerDB();
            if (id == 0)
            {
                context.Orgs.Add(new Server.Models.Org
                {
                    Name = name,
                    Tag = tag,
                    Owner = owner.ToString(),
                    Members = "[]",
                    Requests = "[]",
                    Vehicles = "[]",
                    VehicleRequests = "[]"
                });

                context.SaveChanges();

                id = context.Orgs.ToList().Last().Id;
            }
            else
            {
                var org = context.Orgs.Where(x => x.Id == id).FirstOrDefault();

                org.Name = name;
                org.Tag = tag;
                org.Owner = owner.ToString();
                org.Members = JsonConvert.SerializeObject(members);
                org.Requests = JsonConvert.SerializeObject(requests);
                org.Vehicles = JsonConvert.SerializeObject(vehicles);
                org.VehicleRequests = JsonConvert.SerializeObject(vehicleRequests);

                context.SaveChanges();
            }
        }

        public void Delete()
        {
            using var context = new ServerDB();
            context.Orgs.Remove(context.Orgs.Where(x => x.Id == id).FirstOrDefault());
            context.SaveChanges();
            foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
            {
                if (vehicle.HasSharedData("orgId") && vehicles.Contains(vehicle.GetSharedData<Int32>("id")))
                {
                    vehicle.SetSharedData("orgId", 0);
                }
            }
            foreach (Player player in NAPI.Pools.GetAllPlayers())
            {
                if (player.HasSharedData("orgId") && (members.Contains(player.SocialClubId) || owner == player.SocialClubId))
                {
                    player.SetSharedData("orgId", 0);
                    player.SetSharedData("orgName", "");
                    player.SetSharedData("orgOwner", false);
                    player.SetSharedData("orgTag", "");
                }
            }
        }

        public List<List<string[]>> GetOrgData(ulong memberId)
        {
            List<List<string[]>> data = new List<List<string[]>>();
            List<string[]> members = new List<string[]>();
            List<string[]> vehicles = new List<string[]>();
            List<string[]> membersRequests = new List<string[]>();
            List<string[]> vehiclesRequests = new List<string[]>();
            List<string[]> vehiclesToShare = new List<string[]>();
            using var context = new ServerDB();
            if (this.members.Count > 0)
            {
                var mmbs = context.Users.ToList().Where(x => (this.members.Contains(ulong.Parse(x.Login)) || this.owner.ToString() == x.Login) && x.Login != memberId.ToString()).ToList();
                foreach(var member in mmbs)
                {
                    members.Add(new string[]
                    {
                        member.Id.ToString(),
                        member.Username
                    });
                }
            }

            if (this.vehicles.Count > 0)
            {
                var orgVehicles = (from veh in context.Set<Server.Models.Vehicle>()
                                   join user in context.Set<Server.Models.User>()
                                   on veh.Owner equals user.Login
                                   where this.vehicles.Contains(veh.Id)
                                   select new { veh, user }).ToList();

                foreach(var vehicle in orgVehicles)
                {
                    vehicles.Add(new string[]
                    {
                        vehicle.veh.Id.ToString(),
                        vehicle.veh.Name,
                        vehicle.user.Username
                    });
                }
            }

            if (this.vehicleRequests.Count > 0)
            {
                var vehRequests = (from veh in context.Set<Server.Models.Vehicle>()
                                   join user in context.Set<Server.Models.User>()
                                   on veh.Owner equals user.Login
                                   where this.vehicleRequests.Contains(veh.Id)
                                   select new { veh, user }).ToList();

                foreach (var vehicle in vehRequests)
                {
                    vehiclesRequests.Add(new string[]
                    {
                        vehicle.veh.Id.ToString(),
                        vehicle.veh.Name,
                        vehicle.user.Username
                    });
                }
            }

            if (this.requests.Count > 0)
            {
                var memberRequests = context.Users.ToList().Where(x => this.requests.Contains(ulong.Parse(x.Login))).ToList();
                foreach (var member in memberRequests)
                {
                    membersRequests.Add(new string[]
                    {
                        member.Id.ToString(),
                        member.Username,
                        member.Login
                    });
                }
            }

            var vehsToShare = (from veh in context.Set<Server.Models.Vehicle>()
                               join user in context.Set<Server.Models.User>()
                               on veh.Owner equals user.Login
                               where !this.vehicleRequests.Contains(veh.Id) && !this.vehicles.Contains(veh.Id) && user.Login == memberId.ToString()
                               select new { veh, user }).ToList();

            foreach (var vehicle in vehsToShare)
            {
                vehiclesToShare.Add(new string[]
                    {
                        vehicle.veh.Id.ToString(),
                        vehicle.veh.Name
                    });
            }

            data.Add(members);
            data.Add(vehicles);
            data.Add(membersRequests);
            data.Add(vehiclesRequests);
            data.Add(vehiclesToShare);
            return data;
        }

        public List<List<string[]>> GetOrgDataForMember(ulong memberId)
        {
            List<List<string[]>> data = new List<List<string[]>>();
            List<string[]> members = new List<string[]>();
            List<string[]> vehicles = new List<string[]>();
            List<string[]> vehiclesToShare = new List<string[]>();
            List<string[]> sharedVehicles = new List<string[]>();

            using var context = new ServerDB();

            if (this.members.Count > 0)
            {
                var mmbs = context.Users.ToList().Where(x => (this.members.Contains(ulong.Parse(x.Login)) || this.owner.ToString() == x.Login) && x.Login != memberId.ToString()).ToList();
                foreach (var member in mmbs)
                {
                    members.Add(new string[]
                    {
                        member.Id.ToString(),
                        member.Username
                    });
                }
            }

            if (this.vehicles.Count > 0)
            {
                var orgVehicles = (from veh in context.Set<Server.Models.Vehicle>()
                                   join user in context.Set<Server.Models.User>()
                                   on veh.Owner equals user.Login
                                   where this.vehicles.Contains(veh.Id)
                                   select new { veh, user }).ToList();

                foreach (var vehicle in orgVehicles)
                {
                    vehicles.Add(new string[]
                    {
                        vehicle.veh.Id.ToString(),
                        vehicle.veh.Name,
                        vehicle.user.Username
                    });
                }
            }

            var vehsToShare = (from veh in context.Set<Server.Models.Vehicle>()
                               join user in context.Set<Server.Models.User>()
                               on veh.Owner equals user.Login
                               where !this.vehicleRequests.Contains(veh.Id) && !this.vehicles.Contains(veh.Id) && user.Login == memberId.ToString()
                               select new { veh, user }).ToList();

            foreach (var vehicle in vehsToShare)
            {
                vehiclesToShare.Add(new string[]
                    {
                        vehicle.veh.Id.ToString(),
                        vehicle.veh.Name
                    });
            }

            var sharedVehs = (from veh in context.Set<Server.Models.Vehicle>()
                               join user in context.Set<Server.Models.User>()
                               on veh.Owner equals user.Login
                               where (this.vehicleRequests.Contains(veh.Id) || this.vehicles.Contains(veh.Id)) && user.Login == memberId.ToString()
                               select new { veh, user }).ToList();

            foreach (var vehicle in sharedVehs)
            {
                sharedVehicles.Add(new string[]
                    {
                        vehicle.veh.Id.ToString(),
                        vehicle.veh.Name
                    });
            }

            data.Add(members);
            data.Add(vehicles);
            data.Add(vehiclesToShare);
            data.Add(sharedVehicles);
            return data;
        }
    }
}

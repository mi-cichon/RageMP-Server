using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using MySqlConnector;
using Newtonsoft.Json;

namespace ServerSide
{

    public class OrgManager
    {

        public List<Organization> orgs = new List<Organization>();
        PlayerDataManager playerDataManager = new PlayerDataManager();
        public OrgManager()
        {
            LoadOrgsFromDatabase();
            ColShape col = NAPI.ColShape.CreateCylinderColShape(new Vector3(-1570.2968f, -551.0935f, 114.57582f), 1.2f, 2.0f);
            col.SetSharedData("type", "org");
        }

        public void LoadOrgsFromDatabase()
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM orgs";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Organization org = new Organization(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), UInt64.Parse(reader.GetString(3)), JsonConvert.DeserializeObject<List<ulong>>(reader.GetString(4)), JsonConvert.DeserializeObject<List<ulong>>(reader.GetString(5)), JsonConvert.DeserializeObject<List<int>>(reader.GetString(6)), JsonConvert.DeserializeObject<List<int>>(reader.GetString(7)));
                    orgs.Add(org);
                }
            }
            dataBase.connection.Close();
        }

        public void SetPlayersOrg(Player player)
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

        public void SetVehiclesOrg(Vehicle vehicle)
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

        public int CreateOrg(Player owner, string name, string tag)
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"SELECT * FROM orgs WHERE LOWER(name) = '{name.ToLower()}'";
            var result = dataBase.command.ExecuteScalar();
            if (result != null)
            {
                dataBase.connection.Close();
                return 1;
            }
            else
            {
                dataBase.command.CommandText = $"SELECT * FROM orgs WHERE LOWER(tag) = '{tag.ToLower()}'";
                var res = dataBase.command.ExecuteScalar();
                if (res != null)
                {
                    dataBase.connection.Close();
                    return 2;
                }
                else
                {
                    Organization org = new Organization(name, tag, owner.SocialClubId);
                    org.SaveOrgToDataBase();
                    orgs.Add(org);
                    owner.SetSharedData("orgName", name);
                    owner.SetSharedData("orgOwner", true);
                    owner.SetSharedData("orgTag", org.tag);
                    NAPI.Task.Run(() =>
                    {
                        owner.SetSharedData("orgId", org.id);
                    }, 2000);
                    dataBase.connection.Close();
                    return 0;
                }
            }
        }

        public bool DeleteOrg(ulong player, int orgId)
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

        public bool AddMemberToOrg(ulong playerId, int orgId)
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

        public bool RemoveMemberFromOrg(ulong playerId, int orgId)
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
                        foreach (int vehId in playerDataManager.GetPlayersVehiclesById(playerId))
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

        public bool AddRequestToOrg(ulong playerId, int orgId)
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

        public bool RemoveRequestFromOrg(ulong playerId, int orgId)
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

        public bool AcceptRequest(ulong playerId, int orgId)
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

        public bool RemoveVehicleFromOrg(int vehId, int orgId)
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

        public bool SendVehicleRequest(ulong playerId, int vehId, int orgId)
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

        public bool RemoveVehicleRequest(int vehId, int orgId)
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

        public bool AcceptVehicleRequest(int vehId, int orgId)
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
            DBConnection dataBase = new DBConnection();
            if (id == 0)
            {
                dataBase.command.CommandText = $"INSERT INTO orgs (name, tag, owner, members, requests, vehicles, vehiclerequests) values ('{name}','{tag}','{owner}','[]','[]','[]','[]'); SELECT LAST_INSERT_ID();";
                id = Convert.ToInt32(dataBase.command.ExecuteScalar());
            }
            else
            {
                dataBase.command.CommandText = $"UPDATE orgs SET name = '{name}', tag = '{tag}', owner = '{owner}', members = '{JsonConvert.SerializeObject(members)}', requests = '{JsonConvert.SerializeObject(requests)}', vehicles = '{JsonConvert.SerializeObject(vehicles)}', vehiclerequests = '{JsonConvert.SerializeObject(vehicleRequests)}' WHERE id = {id}";
                dataBase.command.ExecuteNonQuery();
            }
            dataBase.connection.Close();
        }

        public void Delete()
        {
            DBConnection dataBase = new DBConnection();
            dataBase.command.CommandText = $"DELETE FROM orgs WHERE id = {id}";
            dataBase.command.ExecuteNonQuery();
            dataBase.connection.Close();
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

            DBConnection dataBase = new DBConnection();

            if (this.members.Count > 0)
            {
                string mm = "";
                foreach (ulong member in this.members)
                {
                    mm += $"login = '{member}'";
                    if (this.members.IndexOf(member) != this.members.Count - 1)
                    {
                        mm += " OR ";
                    }
                }
                dataBase.command.CommandText = "SELECT id, username FROM users WHERE " + mm + ";";
                using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        members.Add(new string[]
                        {
                            reader.GetInt32(0).ToString(),
                            reader.GetString(1),
                        });
                    }
                }
            }

            if (this.vehicles.Count > 0)
            {
                string mm = "";
                foreach (int vehicle in this.vehicles)
                {
                    mm += $"vehicles.id = {vehicle}";
                    if (this.vehicles.IndexOf(vehicle) != this.vehicles.Count - 1)
                    {
                        mm += " OR ";
                    }
                }
                dataBase.command.CommandText = "SELECT vehicles.id, vehicles.name, users.username FROM vehicles LEFT JOIN users ON vehicles.owner = users.login WHERE " + mm + ";";
                using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vehicles.Add(new string[]
                        {
                            reader.GetInt32(0).ToString(),
                            reader.GetString(1),
                            reader.GetString(2)
                        });
                    }
                }
            }

            if (this.vehicleRequests.Count > 0)
            {
                string mm = "";
                foreach (int vehicle in this.vehicleRequests)
                {
                    mm += $"vehicles.id = {vehicle}";
                    if (this.vehicleRequests.IndexOf(vehicle) != this.vehicleRequests.Count - 1)
                    {
                        mm += " OR ";
                    }
                }
                dataBase.command.CommandText = "SELECT vehicles.id, vehicles.name, users.username FROM vehicles LEFT JOIN users ON vehicles.owner = users.login WHERE " + mm + ";";
                using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vehiclesRequests.Add(new string[]
                        {
                            reader.GetInt32(0).ToString(),
                            reader.GetString(1),
                            reader.GetString(2)
                        });
                    }
                }
            }

            if (this.requests.Count > 0)
            {
                string mm = "";
                foreach (ulong member in this.requests)
                {
                    mm += $"login = '{member}'";
                    if (this.requests.IndexOf(member) != this.requests.Count - 1)
                    {
                        mm += " OR ";
                    }
                }
                dataBase.command.CommandText = "SELECT id, username, login FROM users WHERE " + mm + ";";
                using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        membersRequests.Add(new string[]
                        {
                            reader.GetInt32(0).ToString(),
                            reader.GetString(1),
                            reader.GetString(2)
                        });
                    }
                }
            }

            string m = "";
            if (this.vehicleRequests.Count > 0)
            {
                m = " AND ";
            }
            foreach (int vehicle in this.vehicleRequests)
            {
                m += $" vehicles.id NOT LIKE {vehicle}";
                if (this.vehicleRequests.IndexOf(vehicle) != this.vehicleRequests.Count - 1)
                {
                    m += " AND ";
                }
            }
            if (m == "" && this.vehicles.Count > 0)
            {
                m += " AND";
            }
            else if (this.vehicles.Count > 0)
            {
                m += " AND";
            }
            foreach (int vehicle in this.vehicles)
            {
                m += $" vehicles.id NOT LIKE {vehicle}";
                if (this.vehicles.IndexOf(vehicle) != this.vehicles.Count - 1)
                {
                    m += " AND ";
                }
            }
            dataBase.command.CommandText = $"SELECT vehicles.id, vehicles.name FROM vehicles LEFT JOIN users ON vehicles.owner = users.login WHERE users.login = '{memberId}'" + m + ";";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    vehiclesToShare.Add(new string[]
                    {
                        reader.GetInt32(0).ToString(),
                        reader.GetString(1)
                    });
                }
            }

            data.Add(members);
            data.Add(vehicles);
            data.Add(membersRequests);
            data.Add(vehiclesRequests);
            data.Add(vehiclesToShare);
            dataBase.connection.Close();
            return data;
        }

        public List<List<string[]>> GetOrgDataForMember(ulong memberId)
        {
            List<List<string[]>> data = new List<List<string[]>>();
            List<string[]> members = new List<string[]>();
            List<string[]> vehicles = new List<string[]>();
            List<string[]> vehiclesToShare = new List<string[]>();
            List<string[]> sharedVehicles = new List<string[]>();

            DBConnection dataBase = new DBConnection();

            if (this.members.Count > 0)
            {
                string mm = "";
                foreach (ulong member in this.members)
                {
                    mm += $" login = '{member}' ";
                    if (this.members.IndexOf(member) != this.members.Count - 1)
                    {
                        mm += " OR ";
                    }
                }
                dataBase.command.CommandText = "SELECT id, username FROM users WHERE " + mm + ";";
                using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        members.Add(new string[]
                        {
                            reader.GetInt32(0).ToString(),
                            reader.GetString(1),
                        });
                    }
                }
            }

            if (this.vehicles.Count > 0)
            {
                string mm = "";
                foreach (int vehicle in this.vehicles)
                {
                    mm += $" vehicles.id = {vehicle}";
                    if (this.vehicles.IndexOf(vehicle) != this.vehicles.Count - 1)
                    {
                        mm += " OR ";
                    }
                }
                dataBase.command.CommandText = "SELECT vehicles.id, vehicles.name, users.username FROM vehicles LEFT JOIN users ON vehicles.owner = users.login WHERE " + mm + ";";
                using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vehicles.Add(new string[]
                        {
                            reader.GetInt32(0).ToString(),
                            reader.GetString(1),
                            reader.GetString(2)
                        });
                    }
                }
            }

            string m = "";
            if (this.vehicleRequests.Count > 0)
            {
                m = " AND ";
            }
            foreach (int vehicle in this.vehicleRequests)
            {
                m += $" vehicles.id NOT LIKE {vehicle}";
                if (this.vehicleRequests.IndexOf(vehicle) != this.vehicleRequests.Count - 1)
                {
                    m += " AND ";
                }
            }
            if (m == "" && this.vehicles.Count > 0)
            {
                m += " AND";
            }
            else if (this.vehicles.Count > 0)
            {
                m += " AND";
            }
            foreach (int vehicle in this.vehicles)
            {
                m += $" vehicles.id NOT LIKE {vehicle}";
                if (this.vehicles.IndexOf(vehicle) != this.vehicles.Count - 1)
                {
                    m += " AND ";
                }
            }
            dataBase.command.CommandText = $"SELECT vehicles.id, vehicles.name FROM vehicles LEFT JOIN users ON vehicles.owner = users.login WHERE users.login = '{memberId}'" + m + ";";
            using (MySqlDataReader reader = dataBase.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    vehiclesToShare.Add(new string[]
                    {
                        reader.GetInt32(0).ToString(),
                        reader.GetString(1)
                    });
                }
            }

            m = "";
            if (this.vehicleRequests.Count > 0)
            {
                m = " AND (";
            }
            foreach (int vehicle in this.vehicleRequests)
            {
                m += $" vehicles.id = {vehicle}";
                if (this.vehicleRequests.IndexOf(vehicle) != this.vehicleRequests.Count - 1)
                {
                    m += " OR ";
                }
            }
            if (m == "" && this.vehicles.Count > 0)
            {
                m += " AND(";
            }
            else if (this.vehicles.Count > 0)
            {
                m += " OR";
            }
            foreach (int vehicle in this.vehicles)
            {
                m += $" vehicles.id = {vehicle}";
                if (this.vehicles.IndexOf(vehicle) != this.vehicles.Count - 1)
                {
                    m += " OR ";
                }
            }
            if (m != "")
            {
                dataBase.command.CommandText = $"SELECT vehicles.id, vehicles.name FROM vehicles LEFT JOIN users ON vehicles.owner = users.login WHERE users.login = '{memberId}'" + m + (m.Contains('(') ? ")" : "") + ";";
                using (MySqlDataReader reader = dataBase.command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sharedVehicles.Add(new string[]
                        {
                            reader.GetInt32(0).ToString(),
                            reader.GetString(1)
                        });
                    }
                }
            }

            data.Add(members);
            data.Add(vehicles);
            data.Add(vehiclesToShare);
            data.Add(sharedVehicles);
            dataBase.connection.Close();
            return data;
        }
    }
}

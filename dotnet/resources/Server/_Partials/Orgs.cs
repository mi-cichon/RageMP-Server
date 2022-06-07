using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {
        [RemoteEvent("openOrgBrowser")]
        public void OpenOrgBrowser(Player player)
        {
            if (!player.HasSharedData("orgId") || (player.HasSharedData("orgId") && player.GetSharedData<Int32>("orgId") == 0))
            {
                List<string[]> orgs = new List<string[]>();
                foreach (Organization org in OrgManager.orgs)
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
                foreach (Organization org in OrgManager.orgs)
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
                foreach (Organization org in OrgManager.orgs)
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
                    if (OrgManager.AddRequestToOrg(player.SocialClubId, orgId))
                    {
                        PlayerDataManager.NotifyPlayer(player, "Pomyślnie złożono podanie do organizacji!");
                        PlayerDataManager.UpdatePlayersMoney(player, -500);
                    }
                    else
                    {
                        PlayerDataManager.NotifyPlayer(player, "Złożyłeś już podanie do tej organizacji!");
                    }
                }
                else
                {
                    PlayerDataManager.NotifyPlayer(player, "Aby złożyć podanie do organizacji potrzebujesz $500!");
                }
            }
            else
            {
                PlayerDataManager.NotifyPlayer(player, "Należysz już do organizacji!");
            }

        }
        [RemoteEvent("createOrg")]
        public void CreateOrg(Player player, string name, string tag)
        {
            if (player.GetSharedData<Int32>("money") >= 35000)
            {
                int org = OrgManager.CreateOrg(player, name, tag);
                if (org == 0)
                {
                    player.TriggerEvent("closeOrgBrowser");
                    PlayerDataManager.NotifyPlayer(player, "Organizacja została pomyślnie utworzona!");
                    PlayerDataManager.UpdatePlayersMoney(player, -35000);
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
                foreach (Organization org in OrgManager.orgs)
                {
                    if (org.owner == player.SocialClubId)
                    {
                        if (state)
                        {
                            if (OrgManager.AcceptRequest(ID, org.id))
                            {
                                PlayerDataManager.NotifyPlayer(player, "Pomyślnie dodano członka do organizacji!");
                            }
                            else
                            {
                                PlayerDataManager.NotifyPlayer(player, "Ten członek dołączył już do innej organizacji!");
                            }
                        }
                        else
                        {
                            if (OrgManager.RemoveRequestFromOrg(ID, org.id))
                            {
                                PlayerDataManager.NotifyPlayer(player, "Pomyślnie odrzucono podanie!");
                            }
                            else
                            {
                                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono podania!");
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
                foreach (Organization org in OrgManager.orgs)
                {
                    if (org.owner == player.SocialClubId)
                    {
                        if (state)
                        {
                            if (OrgManager.AcceptVehicleRequest(ID, org.id))
                            {
                                PlayerDataManager.NotifyPlayer(player, "Pomyślnie dodano pojazd do organizacji!");
                            }
                            else
                            {
                                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
                            }
                        }
                        else
                        {
                            if (OrgManager.RemoveVehicleRequest(ID, org.id))
                            {
                                PlayerDataManager.NotifyPlayer(player, "Pomyślnie odrzucono pojazd!");
                            }
                            else
                            {
                                PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
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
            foreach (Organization org in OrgManager.orgs)
            {
                if (org.owner == player.SocialClubId)
                {
                    OrgManager.DeleteOrg(player.SocialClubId, org.id);
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
                foreach (Organization org in OrgManager.orgs)
                {
                    if (org.id == player.GetSharedData<int>("orgId"))
                    {
                        if (OrgManager.RemoveVehicleFromOrg(ID, org.id))
                        {
                            PlayerDataManager.NotifyPlayer(player, "Pomyślnie usunięto pojazd z organizacji!");
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
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
                foreach (Organization org in OrgManager.orgs)
                {
                    if (org.id == player.GetSharedData<int>("orgId"))
                    {
                        if (OrgManager.SendVehicleRequest(player.SocialClubId, ID, org.id))
                        {
                            PlayerDataManager.NotifyPlayer(player, "Pomyślnie zaproponowano dodanie pojazdu!");
                        }
                        else
                        {
                            PlayerDataManager.NotifyPlayer(player, "Nie odnaleziono pojazdu!");
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
            foreach (Organization org in OrgManager.orgs)
            {
                if (org.id == player.GetSharedData<int>("orgId"))
                {
                    OrgManager.RemoveMemberFromOrg(player.SocialClubId, org.id);
                    player.TriggerEvent("closeMemberOrgBrowser");
                    PlayerDataManager.NotifyPlayer(player, "Pomyślnie opuszczono organizację!");
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
                foreach (Organization org in OrgManager.orgs)
                {
                    if (org.members.Contains(ID))
                    {
                        OrgManager.RemoveMemberFromOrg(ID, org.id);
                        List<List<string[]>> data = org.GetOrgData(player.SocialClubId);
                        PlayerDataManager.NotifyPlayer(player, "Pomyślnie wyrzucono członka!");
                        player.TriggerEvent("refreshManageOrgData", JsonConvert.SerializeObject(data[0]), JsonConvert.SerializeObject(data[1]), JsonConvert.SerializeObject(data[2]), JsonConvert.SerializeObject(data[3]), JsonConvert.SerializeObject(data[4]));
                        break;
                    }
                }
            }
        }
    }
}

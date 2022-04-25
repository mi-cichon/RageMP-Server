using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    partial class MainClass
    {
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
    }
}

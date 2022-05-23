using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using System.IO;

namespace ServerSide
{
    public class LogManager
    {
        public LogManager()
        {

        }

        public void CreatePlayersDirectories(string playerId)
        {
            Directory.CreateDirectory(@$"logs/players/{playerId}");
            Directory.CreateDirectory(@$"logs/players/{playerId}/chat/local");
            Directory.CreateDirectory(@$"logs/players/{playerId}/chat/global");
            Directory.CreateDirectory(@$"logs/players/{playerId}/chat/pm");
            Directory.CreateDirectory(@$"logs/players/{playerId}/transactions");
            Directory.CreateDirectory(@$"logs/players/{playerId}/login");
            Directory.CreateDirectory(@$"logs/players/{playerId}/login");
            Directory.CreateDirectory(@$"logs/players/{playerId}/penalties");
        }
        public void LogGlobalChatToServer(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/chat/global.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }
        public void LogLocalChat(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/chat/local/local.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public void LogGlobalChat(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/chat/global/global.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public void LogPrivateChat(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/chat/pm/pm.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public void LogJobTransaction(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/transactions/jobs.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public void LogVehicleTrade(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/transactions/vehicletrades.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public void LogLoginInfo(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/login/logins.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }
        public void SaveLoginInfo(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/login/logins.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public void LogPenalty(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/penalties/penalties.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }
    }
}

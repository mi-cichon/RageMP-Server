using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using System.IO;

namespace ServerSide
{
    public static class LogManager
    {
        public static void CreatePlayersDirectories(string playerId)
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
        public static void LogGlobalChatToServer(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/chat/global.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }
        public static void LogLocalChat(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/chat/local/local.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public static void LogGlobalChat(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/chat/global/global.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public static void LogPrivateChat(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/chat/pm/pm.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public static void LogJobTransaction(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/transactions/jobs.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public static void LogVehicleTrade(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/transactions/vehicletrades.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public static void LogLoginInfo(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/login/logins.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }
        public static void SaveLoginInfo(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/login/logins.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }

        public static void LogPenalty(string playerId, string logText)
        {
            //string log = $"[{DateTime.Now}] {logText}";
            //File.AppendAllText(@$"logs/players/{playerId}/penalties/penalties.txt", log + Environment.NewLine, new UTF8Encoding(false, true));
        }
    }
}

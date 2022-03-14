using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class Report
    {
        public Player informer, reported;
        public string description;
        public DateTime time;
        public uint id;
        public string reportedName, informerName;
        public Report(Player informer, Player reported, string description, DateTime time)
        {
            this.reported = reported;
            this.informer = informer;
            this.description = description;
            this.time = time;
            this.reportedName = reported.GetSharedData<string>("username");
            this.informerName = informer.GetSharedData<string>("username");
            id = NAPI.Util.GetHashKey(time.ToString() + time.Millisecond.ToString());
        }
    }
}
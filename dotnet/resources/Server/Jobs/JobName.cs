using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    public class JobName
    {
        public string JobType { get; set; }
        public string Name { get; set; }
        public string PointsName { get; set; }

        public JobName(string jobType = "", string name = "", string pointsName = "")
        {
            JobType = jobType;
            Name = name;
            PointsName = pointsName;
        }
    }
}

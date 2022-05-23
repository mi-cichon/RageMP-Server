using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    partial class MainClass
    {
        public Vector3 QtoVector(Quaternion q)
        {
            float x = q.X, y = q.Y, z = q.Z, w = q.W;
            Vector3 v = new Vector3(0, 0, 1);
            v.X = 2 * (x * z - w * y);
            v.Y = 2 * (y * z + w * x);
            v.Z = 1 - 2 * (x * x + y * y);
            Console.WriteLine(v.ToString());
            return v;
        }

        public Vector3 toEuler(Quaternion q)
        {
            Vector3 retVal = new Vector3();

            // roll (x-axis rotation)
            double sinr_cosp = 2.0 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1.0 - 2.0 * (q.X * q.X + q.Y * q.Y);
            retVal.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = 2.0 * (q.W * q.Y - q.Z * q.X);

            if (Math.Abs(sinp) >= 1)
            {
                retVal.Y = 90.0f; // use 90 degrees if out of range
            }
            else
            {
                retVal.Y = (float)Math.Asin(sinp);
            }


            // yaw (z-axis rotation)
            double siny_cosp = 2.0 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1.0 - 2.0 * (q.Y * q.Y + q.Z * q.Z);
            retVal.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            // Rad to Deg
            retVal.X *= (float)(180.0f / Math.PI);
            retVal.Y *= (float)(180.0f / Math.PI);
            retVal.Z *= (float)(180.0f / Math.PI);

            return retVal;
        }
        public string[] GetArgs(string arg)
        {
            if (arg == null || arg == "")
                return null;
            if (!arg.Contains(" "))
            {
                return new string[] { arg };
            }
            if (arg.Contains(" "))
                return arg.Split(" ");
            return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkXR
{
    public class AndroidUtils
    {
        public static string GetLocalMacAddress()
        {
            //return NibiruTaskApi.GetMacAddress();
            string macddr = null;
            return macddr ?? "02:00:00:00:00:00";
        }
        public static string GetDeviceName()
        {
            string deviceName = "";
            return deviceName ?? "";
        }

        public static long GetMemTotalMb()
        {
            return 0;
        }

        public static long GetMemUsed()
        {
            return 0;
        }

        public static long GetDiskTotal()
        {
            return 0;
        }

        public static long GetDiskUsed()
        {
            return 0;
        }

        public static long GetSdMemMbTotal()
        {
            return 0;
        }

        public static long GetSdMemUsed()
        {
            return 0;
        }
    }
}


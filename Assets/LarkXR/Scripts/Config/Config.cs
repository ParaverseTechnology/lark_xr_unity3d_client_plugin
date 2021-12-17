using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkXR
{
    public class Config
    {
        public delegate void ServerAddressChange(string ip, int port);

        public static ServerAddressChange serverAddressChange;
        #region ip setup.
        // key-setup.
        public const string KEY_IP = "cloudIpAddress";
        public const string KEY_CLOUDLARK_PORT = "cloudlarkPort";
        // default value for ip.
        public const string DEFAULT_IP = "";
        public const int DEFAULT_LARK_PORT = 8181;

        // cached storge for session id.
        private static string cachedIp = "";
        private static int cachedLarkPort = 0;

        public static void ClearServerAddr()
        {
            PlayerPrefs.SetString(KEY_IP, DEFAULT_IP);
            PlayerPrefs.SetInt(KEY_CLOUDLARK_PORT, DEFAULT_LARK_PORT);
            PlayerPrefs.Save();

            cachedIp = DEFAULT_IP;
            cachedLarkPort = DEFAULT_LARK_PORT;
        }

        public static bool IsServerAddressEmpty()
        {
            Debug.Log("===================IsServerAddressEmpty ip:" + GetServerIp() + ";larkport:" + GetLarkPort());
            return GetServerIp().Equals("") || GetLarkPort() == 0;
        }
        public static void SetServerIp(string ip)
        {
            PlayerPrefs.SetString(KEY_IP, ip);
            PlayerPrefs.Save();
            cachedIp = ip;
            serverAddressChange?.Invoke(cachedIp, cachedLarkPort);
        }

        public static void SetCloudLarkPort(int cloudlarkPort)
        {
            PlayerPrefs.SetInt(KEY_CLOUDLARK_PORT, cloudlarkPort);
            PlayerPrefs.Save();
            cachedLarkPort = cloudlarkPort;
            serverAddressChange?.Invoke(cachedIp, cachedLarkPort);
        }
        public static string GetServerIp()
        {
            if (cachedIp.Equals(""))
            {
                cachedIp = PlayerPrefs.GetString(KEY_IP, DEFAULT_IP);
            }
            return cachedIp;
        }
        public static int GetLarkPort()
        {
            if (cachedLarkPort == 0)
            {
                cachedLarkPort = PlayerPrefs.GetInt(KEY_CLOUDLARK_PORT, DEFAULT_LARK_PORT);
            }
            return cachedLarkPort;
        }
        #endregion

        #region user setting.
        public const string KEY_TRACKED_POSITION = "trackedPosition";
        public const string KEY_CODERATE = "codeRate";
        public const string KEY_EXTRA_HEIGHT = "extraHeight";
        public const string KEY_QUICK_SETUP_LEVEL = "quickSetupLevel";

        public const string KEY_RENDER_WIDTH = "renderWidth";
        public const string KEY_RENDER_HEIGHT = "renderHeight";

        public const int DEFAULT_VALUE_TRACKED_POSITION = 0;
        public const float DEFAULT_EXTRA_HEIGHT = 1.5f;
        public const int DEFAULT_CODE_RATE = 20 * 1000;
        // QuickConfigLevel_Fast
        public const int DEFAULT_QUICK_SETUP_LEVEL = 2;
        // render
        public const int DEFAULT_RENDER_WIDTH = 1920;
        public const int DEFAULT_RENDER_HEIGHT = 1080;

        private static float cachedExtraHeight = -1.0f;
        private static int cachedCodeRate = -1;

        public static float GetExtraHeight()
        {
            if (cachedExtraHeight == -1) {
                cachedExtraHeight = PlayerPrefs.GetFloat(KEY_EXTRA_HEIGHT, DEFAULT_EXTRA_HEIGHT);
            }
            return cachedExtraHeight;
        }

        public static void SetExtraHeight(float height)
        {
            cachedExtraHeight = height;
            PlayerPrefs.SetFloat(KEY_EXTRA_HEIGHT, height);
            PlayerPrefs.Save();
        }

        public static bool GetTrackedPosition()
        {
            return PlayerPrefs.GetInt(KEY_TRACKED_POSITION, DEFAULT_VALUE_TRACKED_POSITION) == 1;
        }
        public static void SetTrackedPosition(bool tracked)
        {
            PlayerPrefs.SetInt(KEY_TRACKED_POSITION, tracked ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static void SetCodeRate(int codeRate)
        {
            cachedCodeRate = codeRate;
            PlayerPrefs.SetInt(KEY_CODERATE, codeRate);
            PlayerPrefs.Save();
        }
        public static int GetCodeRate()
        {
            if(cachedCodeRate == -1)
            {
                cachedCodeRate = PlayerPrefs.GetInt(KEY_CODERATE, DEFAULT_CODE_RATE);
            }
            return cachedCodeRate;
        }
        public static void SetQuickSetupLevel(int level)
        {
            PlayerPrefs.SetInt(KEY_QUICK_SETUP_LEVEL, level);
            PlayerPrefs.Save();
        }
        public static int GetQuickSetupLevel()
        {
            return PlayerPrefs.GetInt(KEY_QUICK_SETUP_LEVEL, DEFAULT_QUICK_SETUP_LEVEL);
        }
        public static void SetRenderWidth(int renderWidth)
        {
            PlayerPrefs.SetInt(KEY_RENDER_WIDTH, renderWidth);
            PlayerPrefs.Save();
        }
        public static int GetRenderWidth()
        {
            return PlayerPrefs.GetInt(KEY_RENDER_WIDTH, DEFAULT_RENDER_WIDTH);
        }
        public static void SetRenderHeight(int renderHeight)
        {
            PlayerPrefs.SetInt(KEY_RENDER_HEIGHT, renderHeight);
            PlayerPrefs.Save();
        }
        public static int GetRenderHeight()
        {
            return PlayerPrefs.GetInt(KEY_RENDER_HEIGHT, DEFAULT_RENDER_HEIGHT);
        }
        #endregion
    }
}
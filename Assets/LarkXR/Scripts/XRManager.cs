using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace LarkXR {
    public class XRManager : MonoBehaviour
    {
        const string OBJ_NAME = "XRManager";

        private static XRManager xrManager = null;
        public static XRManager Instance
        {
            get
            {
                if (xrManager == null)
                {
                    xrManager = FindObjectOfType<XRManager>();
                }
                if (xrManager == null)
                {
                    var go = new GameObject(OBJ_NAME);
                    xrManager = go.AddComponent<XRManager>();
                    go.transform.localPosition = Vector3.zero;
                }
                return xrManager;
            }
        }

        public XRApi XRApi
        {
            get;
            private set;
        }

        public TaskManager TaskManager
        {
            get;
            private set;
        }

        public RenderManger RenderManger
        {
            get;
            private set;
        }

        public bool IsConnect
        {
            get;
            private set;
        } = false;

        public bool AutoStartTask = false;

        //public VrApplication VrApplication { get; private set; }

        private void Awake()
        {
            XRApi = GetComponent<XRApi>();

            if (XRApi == null)
            {
                XRApi = gameObject.AddComponent<XRApi>();
            }
            Debug.Assert(XRApi != null);

            TaskManager = GetComponent<TaskManager>();
            if (TaskManager == null)
            {
                TaskManager = gameObject.AddComponent<TaskManager>();
            }
            Debug.Assert(TaskManager != null);

            RenderManger = GetComponent<RenderManger>();
            if (RenderManger == null)
            {
                RenderManger = gameObject.AddComponent<RenderManger>();
            }
            Debug.Assert(RenderManger != null);

            // Config.ClearServerAddr();
            // Debug.Log("xr api system info inited " + XRApi.SystemInited());

            // 连接测试服务器
            // Config.SetServerIp("192.168.31.120");
            // Config.SetCloudLarkPort(8585);
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
            XRApi.InitSystemInfo();
            XRApi.InitContext();
            // flip h when android.
            XRApi.SetFlipDraw(true);
#endif

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || PLATFORM_STANDALONE_WIN
            // WIN32 not support fov rending yet.
            XRApi.SetEnableFoveatedRendering(false);
#endif

            XRApi.SetServerAddr(Config.GetServerIp(), Config.GetLarkPort());

/*            string timestamp = ApiUtils.CurrentTimeMillis().ToString();
            string signature = ApiUtils.GetSignature("1", "2", timestamp);
            Debug.Log("test signature " + signature + " timestamp " + timestamp);
            ApiBase<object>.SetCertificate("", "");

            Debug.Log("test path " + Application.dataPath + " " + System.Environment.CurrentDirectory);*/
        }

        // Start is called before the first frame update
        void Start()
        {
            if (AutoStartTask) { 
                TaskManager.StartTask();
            }
            // RegisterCallbacks after native inited.
            this.XRApi.RegisterCallbacks();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) {
                XRApi.OnPause();
            } else
            {
                XRApi.OnResume();
            }
        }

        private void OnApplicationQuit()
        {
            XRApi.ClearCallbacks();
            if (XRApi.IsConnected()) {
                XRApi.Close();
            }
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
            XRApi.ReleaseContext();
            XRApi.ReleaseSystemInfo();
#endif
            XRApi.OnDestory();
        }

        // Update is called once per frame
        void Update()
        {
        }
        
        // 使用 appliid 进入应用
        public void OnEnterAppli(string appliId)
        {
            Debug.Log("================enerappli:" + appliId);
            XRApi.EnterAppli(appliId);
            
            // 拼接 json 字段进入应用
            //string json = System.String.Format("{{\"appliId\": \"{0}\", \"extraParam.Test1\": \"{1}\", \"extraParam.Test2\": \"{2}\", \"extraParam.Test3\": \"{3}\"}}",
            //    appliId, "Test1", "Test2", "Test3");
            // Debug.Log("================enerappli with json:" + json);
            // XRApi.EnterAppliWithJsonString(json);
        }

        // 拼接 json 字段进入应用, 必须字段为 appliId，其他字段参考
        //  https://www.pingxingyun.com/online/api3_2.html?id=532 1.2.2 进入应用接口
        // string json = System.String.Format("{{\"appliId\": \"{0}\", \"extraParam.Test1\": \"{1}\", \"extraParam.Test2\": \"{2}\", \"extraParam.Test3\": \"{3}\"}}",
        //            appliId, "Test1", "Test2", "Test3");
        // Debug.Log("================enerappli with json:" + json);
        public void OnEnterAppliWithJsonStr(string jsonstr)
        {
            XRApi.EnterAppliWithJsonString(jsonstr);
        }

        public void OnClose()
        {
            Debug.Log("================OnClose");
            XRApi.Close();
        }

        // 从当前的根目录下载入 appkey 和 appsecret
        // demo 中使用。可根据实际业务需求载入,不使用该方法
        // certificate_appkey.txt
        // certificate_appsecret.txt
        public void LoadCertificateFromFile()
        {
            string appKeyPath = System.Environment.CurrentDirectory + "/certificate_appkey.txt";
            string appKey = "";

            string appSecretPath = System.Environment.CurrentDirectory + "/certificate_appsecret.txt";
            string appSecret = "";

            if (File.Exists(appKeyPath)) {
                string[] lines = File.ReadAllLines(appKeyPath);
                if (lines != null && lines.Length > 0) {
                    appKey = lines[0];
                }
            } else
            {
                File.CreateText(appKeyPath).Close();
            }

            if (File.Exists(appSecretPath)) {
                string[] lines = File.ReadAllLines(appSecretPath);
                if (lines != null && lines.Length > 0) {
                    appSecret = lines[0];
                }
            } else
            {
                File.CreateText(appSecretPath).Close();
            }

            ApiBase<object>.SetCertificate(appKey, appSecret);
        }
    }
}
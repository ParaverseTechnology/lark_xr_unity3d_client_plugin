using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LarkXR {
    public class UIManager : MonoBehaviour
    {
        public enum MsgLevel
        {
            INFO = 0,
            WARING,
            ERROR
        }

        public InputField serverIpInput;
        public InputField serverPortInput;
        public Text statusText;
        public Slider extraHeight;

        public GameObject homePage;
        public GameObject loadingPage;

        // quick setup
        public Button quickSetupFast;
        public Button quickSetupNormal;
        public Button quickSetupExtreme;
        //
        public Slider codeRateSlider;
        public Text codeRateSliderLable;
        // res setup
        public Button resSetup1920x1080;
        public Button resSetup2k;
        public Button resSetup4k;

        private AppListContariner appListContariner;
        private float toastTime = -1;

        // Start is called before the first frame update
        void Start()
        {

            Debug.Assert(serverIpInput != null);
            Debug.Assert(serverPortInput != null);
            Debug.Assert(homePage != null);
            Debug.Assert(loadingPage != null);
            Debug.Assert(extraHeight != null);

            Debug.Assert(quickSetupFast != null);
            Debug.Assert(quickSetupNormal != null);
            Debug.Assert(quickSetupExtreme != null);

            Debug.Assert(codeRateSlider != null);
            Debug.Assert(codeRateSliderLable != null);

            Debug.Assert(resSetup1920x1080 != null);
            Debug.Assert(resSetup2k != null);
            Debug.Assert(resSetup4k != null);

            loadingPage.SetActive(false);

            Debug.Log("server ip:" + Config.GetServerIp());
            Debug.Log("server port:" + Config.GetLarkPort());
            serverIpInput.text = Config.GetServerIp();
            serverPortInput.text = Config.GetLarkPort() + "";
            serverIpInput.onEndEdit.AddListener(OnInputServerIp);
            serverPortInput.onEndEdit.AddListener(OnInputServerPort);


            extraHeight.value = Config.GetExtraHeight();
            extraHeight.onValueChanged.AddListener(onExtraHeighChange);

            // native callback
            XRManager.Instance.XRApi.onConnected += OnConnected;
            XRManager.Instance.XRApi.onClose += OnClose;
            XRManager.Instance.XRApi.onInfo += OnInfo;
            XRManager.Instance.XRApi.onError += OnError;

            XRManager.Instance.TaskManager.onApplistSuccess += OnApplistSuccess;
            XRManager.Instance.TaskManager.onApplistFailed += OnApplistFailed;
            XRManager.Instance.RenderManger.onTexture2D += OnStartCloudRender;
            XRManager.Instance.RenderManger.onTexture2DStereo += OnStartCloudRenderStereo;
            XRManager.Instance.RenderManger.onClose += OnStopCloudRender;
            XRManager.Instance.RenderManger.onConnected += OnConnect;
            appListContariner = GetComponentInChildren<AppListContariner>();

            quickSetupFast.onClick.AddListener(OnQuickSetupFast);
            quickSetupNormal.onClick.AddListener(OnQuickSetupNormal);
            quickSetupExtreme.onClick.AddListener(OnQuickSetupExtreme);
            OnQuickSetupChange((XRApi.QuickConfigLevel)Config.GetQuickSetupLevel());

            codeRateSlider.onValueChanged.AddListener(OnChangeCodeRate);
            codeRateSlider.value = Config.GetCodeRate() / 1000;

            codeRateSliderLable.text = "码率 " + codeRateSlider.value + " M";

            //
            int renderWidth = Config.GetRenderWidth();
            int renderHeight = Config.GetRenderHeight();
            OnResSetup(renderWidth, renderHeight);

            resSetup1920x1080.onClick.AddListener(OnResSetup1920x1080);
            resSetup2k.onClick.AddListener(OnResSetup2k);
            resSetup4k.onClick.AddListener(OnResSetup4k);
        }

        // Update is called once per frame
        void Update()
        {
            // clear toast after 2s.
            if (toastTime != -1 && Time.time - toastTime > 3)
            {
                toastTime = -1;
                ClearToast();
            }
/*            if (XRApi.GetLastError() != 0) {
                ToastError("出现错误 code: " + XRApi.GetLastError());
                XRApi.ClearError();
            }*/
        }


        #region
        public void OnStartCloudRenderStereo(Texture2D textureLeft, Texture2D textureRight)
        {
            Debug.Log("OnStartCloudRenderStereo");
            gameObject.SetActive(false);
        }
        public void OnStartCloudRender(Texture2D texture)
        {
            Debug.Log("OnStartCloudRender");
            gameObject.SetActive(false);
        }        
        public void OnStartCloudRender(RenderTexture texture)
        {
            Debug.Log("OnStartCloudRender");
            gameObject.SetActive(false);
        }
        public void OnConnect()
        {
            Debug.Log("ui manager on connect");
            loadingPage.SetActive(true);
            homePage.SetActive(false);
        }
        public void OnStopCloudRender()
        {
            Debug.Log("OnStopCloudRender");
            gameObject.SetActive(true);
            homePage.SetActive(true);
            loadingPage.SetActive(false);
        }
        #endregion

        #region button callback
        public void OnClose()
        {
            XRManager.Instance.OnClose();
        }
        #endregion

        #region task manager callabck
        public void OnApplistSuccess(GetAppliList.Page startAppInfo)
        {
            // Debug.Log("OnApplistSuccess:" + startAppInfo.Count + ";");
            appListContariner?.SetData(startAppInfo);
        }
        public void OnApplistFailed(string msg)
        {
            Debug.Log("On applist failed:" + msg + ";");
            ToastError("获取应用列表失败:" + msg + ";");
            appListContariner?.ClearList();
        }
        #endregion

        #region input callback
        public void OnInputServerIp(string txt)
        {
            Debug.Log("set server ip:" + serverIpInput.text);
            Config.SetServerIp(serverIpInput.text);
            appListContariner?.ClearList();
            // update android server address;
            XRApi.SetServerAddr(Config.GetServerIp(), Config.GetLarkPort());
        }
        public void OnInputServerPort(string txt)
        {
            Debug.Log("set server port:" + serverPortInput.text);
            Config.SetCloudLarkPort(System.Int32.Parse(serverPortInput.text));
            appListContariner?.ClearList();
            // update android server address;
            XRApi.SetServerAddr(Config.GetServerIp(), Config.GetLarkPort());
        }
        public void onExtraHeighChange(float value)
        {
            LarkXR.Config.SetExtraHeight(value);
            Debug.Log("onExtraHeighChange " + value);
        }
        #endregion

        #region toast
        public void ToastInfo(string msg)
        {
            Toast(MsgLevel.INFO, msg);
        }
        public void ToastWaring(string msg)
        {
            Toast(MsgLevel.WARING, msg);
        }
        public void ToastError(string msg)
        {
            Toast(MsgLevel.ERROR, msg);
        }
        public void Toast(MsgLevel level, string msg)
        {
            string txt = "[" + GetStatuPrexByLevel(level) + "]: " + msg;
            statusText.text = txt;
            toastTime = Time.time;
        }
        void ClearToast()
        {
            Debug.Assert(statusText != null);
            statusText.text = "";
        }
        string GetStatuPrexByLevel(MsgLevel level)
        {
            switch (level)
            {
                case MsgLevel.INFO: return "信息";
                case MsgLevel.WARING: return "警告";
                case MsgLevel.ERROR: return "错误";
                default: return "信息";
            }
        }
        #endregion

        #region network setup
        void OnQuickSetupManu()
        {
            Debug.Log("OnQuickSetupManu");
            OnQuickSetupChange(XRApi.QuickConfigLevel.QuickConfigLevel_Manual);
        }
        void OnQuickSetupFast()
        {
            Debug.Log("OnQuickSetupFast");
            OnQuickSetupChange(XRApi.QuickConfigLevel.QuickConfigLevel_Fast);
        }
        void OnQuickSetupNormal()
        {
            Debug.Log("OnQuickSetupNormal");
            OnQuickSetupChange(XRApi.QuickConfigLevel.QuickConfigLevel_Normal);
        }
        void OnQuickSetupExtreme()
        {
            Debug.Log("OnQuickSetupExtreme");
            OnQuickSetupChange(XRApi.QuickConfigLevel.QuickConfigLevel_Extreme);
        }
        void OnQuickSetupChange(XRApi.QuickConfigLevel level)
        {
            quickSetupFast.interactable = level != XRApi.QuickConfigLevel.QuickConfigLevel_Fast;
            quickSetupNormal.interactable = level != XRApi.QuickConfigLevel.QuickConfigLevel_Normal;
            quickSetupExtreme.interactable = level != XRApi.QuickConfigLevel.QuickConfigLevel_Extreme;
            XRApi.QuickConfigWithDefaulSetup(level);
            Config.SetQuickSetupLevel((int)level);
            codeRateSlider.value = XRApi.GetBitrateKbps() / 1000;
        }
        public void OnChangeCodeRate(float value) {
            Debug.Log("OnChangeCodeRate " + value);

            XRApi.SetupBitrateKbps((int)(value * 1000));
            codeRateSliderLable.text = "码率 " + value + " M";
        }

        public void OnResSetup1920x1080()
        {
            OnResSetup(1920, 1080);
        }

        public void OnResSetup2k()
        {
            OnResSetup(2560, 1440);
        }

        public void OnResSetup4k()
        {
            OnResSetup(3860, 1080);
        }

        public void OnResSetup(int renderWidth, int renderHeight)
        {
            Debug.Log("OnResSetup " + renderWidth + " " + renderHeight);

            resSetup1920x1080.interactable = renderWidth != 1920;
            resSetup2k.interactable = renderWidth != 2560;
            resSetup4k.interactable = renderWidth != 3860;

            XRApi.RenderInfo renderInfo = XRApi.GetRenderInfo();
            renderInfo.renderWidth = renderWidth;
            renderInfo.renderHeight = renderHeight;
            XRApi.SetRenderInfo(renderInfo);

            Config.SetRenderWidth(renderWidth);
            Config.SetRenderHeight(renderHeight);
        }
        #endregion

        #region xrapi callback
        public void OnConnected()
        {
            Debug.Log("OnConnected");
        }
        public void OnClose(int code)
        {
            Debug.Log("OnClose" + code);
        }
        public void OnInfo(int code, string msg)
        {
            Debug.Log("OnInfo" + code + " msg : " + msg);
            ToastInfo("[INFO]" + msg);
        }
        public void OnError(int code, string msg)
        {
            Debug.LogWarning("OnError" + code + " msg : " + msg);

            ToastError("出现错误 code: " + code + " msg: " + msg);
        }

        #endregion
    }
}

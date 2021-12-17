using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LarkXR
{
    public class AppListItem : MonoBehaviour
    {

        public delegate void OnEnterApp(string appliId);

        public Text nameText;
        public Text numberText;
        public Button enterButton;
        public OnEnterApp onEnterApp;
        private GetAppliList.StartAppInfo startAppInfo;

        // Use this for initialization
        void Start()
        {
            enterButton.onClick.AddListener(OnEnter);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetData(GetAppliList.StartAppInfo startAppInfo)
        {
            if (startAppInfo == null) return;
            if (startAppInfo.Equals(this.startAppInfo)) return;
            nameText.text = startAppInfo.appliName;
            numberText.text = startAppInfo.runCnt + "/" + startAppInfo.instanceMax;

            this.startAppInfo = startAppInfo;
        }

        void OnEnter()
        {
            if (startAppInfo == null) return;
            onEnterApp?.Invoke(startAppInfo.appliId);
        }
    }
}
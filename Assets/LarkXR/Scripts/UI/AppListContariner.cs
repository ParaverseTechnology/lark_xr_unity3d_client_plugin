using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LarkXR{
    public class AppListContariner : MonoBehaviour {

        public Button nextButton;
        public Button preButton;

        enum  PageType {
            NONE,
            PRE,
            NEXT,
            BOTH
        }
        const int ITEM_LENGTH = 8;
        //XRManager vrManager;
        AppListItem[] items = new AppListItem[ITEM_LENGTH];
        int page = 0;
        int totalPage = 0;

        void Awake()
        {
            //vrManager = XRManager.VrManagerIns;
        }

        // Use this for initialization
        void Start () {
            AppListItem appListItem = GetComponentInChildren<AppListItem>();

            for (var i = 0; i < ITEM_LENGTH; i++) {
                AppListItem child = Instantiate(appListItem, transform);
                child.onEnterApp = OnEnterApp;
                items[i] = child;
            }
            appListItem.gameObject.SetActive(false);

            nextButton.onClick.AddListener(OnNextPage);
            preButton.onClick.AddListener(OnPrePage);

            ClearList();
        }

        // Update is called once per frame
        void Update () {
            //var data = vrManager.GetAppList();
            //if (data == null) {
            //    SetPageButton(PageType.NONE);
            //    ClearList();
            //    return;
            //}
            //if (data.Length != totalPage)
            //{
            //    totalPage = data.Length;
            //    SetPageButton();
            //}

            //for (var i = 0; i < ITEM_LENGTH; i++) {
            //    var child = items[i];
            //    if (page * ITEM_LENGTH + i < data.Length) {
            //        child.SetData(data[page * ITEM_LENGTH + i]);
            //        child.gameObject.SetActive(true);
            //    } else
            //    {
            //        child.gameObject.SetActive(false);
            //    }
            //}
        }

        public void SetData(List<GetAppliList.StartAppInfo> data)
        {
            if (data == null)
            {
                ClearList();
                return;
            }
            if (data.Count != totalPage)
            {
                totalPage = data.Count;
                SetPageButton();
            }

            for (var i = 0; i < ITEM_LENGTH; i++)
            {
                var child = items[i];
                if (page * ITEM_LENGTH + i < data.Count)
                {
                    child.SetData(data[page * ITEM_LENGTH + i]);
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public void ClearList()
        {
            for (var i = 0; i < ITEM_LENGTH; i++)
            {
                items[i].gameObject.SetActive(false);
            }
        }

        void SetPageButton(PageType type) {
            switch (type) {
                case PageType.NONE:
                    preButton.gameObject.SetActive(false);
                    nextButton.gameObject.SetActive(false);
                    break;
                case PageType.PRE:
                    preButton.gameObject.SetActive(true);
                    nextButton.gameObject.SetActive(false);
                    break;
                case PageType.NEXT:
                    preButton.gameObject.SetActive(false);
                    nextButton.gameObject.SetActive(true);
                    break;
                case PageType.BOTH:
                    preButton.gameObject.SetActive(true);
                    nextButton.gameObject.SetActive(true);
                    break;
            }
        }

        void OnNextPage()
        {
            if (page < totalPage)
            {
                page++;
                SetPageButton();
            }
        }

        void OnPrePage()
        {
            if (page > 0) {
                page--;
                SetPageButton();
            }
        }

        void SetPageButton()
        {
            // update page button
            if (totalPage < ITEM_LENGTH)
            {
                SetPageButton(PageType.NONE);
            }
            else if (page > 0 && (page + 1) * ITEM_LENGTH < totalPage)
            {
                SetPageButton(PageType.BOTH);
            }
            else if (page > 0 && (page + 1) * ITEM_LENGTH > totalPage)
            {
                SetPageButton(PageType.PRE);
            }
            else
            {
                SetPageButton(PageType.NEXT);
            }
        }

        void OnEnterApp(string appId)
        {
            Debug.Log("OnEnterApp:" + appId);
            if (!XRManager.Instance.IsConnect)
            {
                XRManager.Instance.OnEnterAppli(appId);
            } else
            {
                Debug.Log("already connected");
            }
        }
    }
}
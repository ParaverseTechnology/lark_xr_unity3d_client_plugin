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

        public void SetData(GetAppliList.Page data)
        {
            if (data == null)
            {
                ClearList();
                return;
            }

            SetPageButton(data);

            for (var i = 0; i < ITEM_LENGTH; i++)
            {
                var child = items[i];
                if (i < data.records.Count)
                {
                    child.SetData(data.records[i]);
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
            XRManager.Instance.TaskManager.NextPage();
        }

        void OnPrePage()
        {
            XRManager.Instance.TaskManager.PrePage();
        }

        void SetPageButton(GetAppliList.Page page)
        {
            // update page button
            if (!page.hasNextPage && !page.hasPreviousPage)
            {
                SetPageButton(PageType.NONE);
            }
            else if (page.hasNextPage && page.hasPreviousPage)
            {
                SetPageButton(PageType.BOTH);
            }
            else if (page.hasPreviousPage)
            {
                SetPageButton(PageType.PRE);
            }
            else if (page.hasNextPage)
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
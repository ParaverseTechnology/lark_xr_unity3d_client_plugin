using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkXR
{
    public class TaskManager : MonoBehaviour
    {
        // call backs
        public delegate void OnApplistSuccess(List<GetAppliList.StartAppInfo> startAppInfo);
        public delegate void OnApplistFailed(string msg);

        public delegate void OnReportResourceSuccess();
        public delegate void OnReportResourceFailed();

        public delegate void SampleRTCStatusUpdated(SampleRTCStatus status);

        public OnReportResourceSuccess onReportResourceSuccess;
        public OnReportResourceFailed onReportResourceFailed;

        public SampleRTCStatusUpdated onSampleRTCStatusUpdated;

        public OnApplistSuccess onApplistSuccess;
        public OnApplistFailed onApplistFailed;

        ReportResource reportResource;
        GetAppliList getAppliList;

        //AndroidGetAppList androidGetAppList;

        string selectCategory = "";
        int currentPage = 0;
        double currentPower = 1;
        bool taskStarted = false;

        int rtcsampleTest = 25;
        int reportResourceNumber = 0;

        // Start is called before the first frame update
        void Start()
        {
            getAppliList = new GetAppliList();
            reportResource = new ReportResource(AndroidUtils.GetLocalMacAddress(), AndroidUtils.GetDeviceName());
            // add systask eventlistner.
            // NibiruTask.NibiruTaskApi.addOnPowerChangeListener(OnPowerChanged);

            //androidGetAppList = GetComponent<AndroidGetAppList>();
            //androidGetAppList.onApplistSuccess += OnAndroidAppListSuccess;
            //androidGetAppList.onApplistFailed += OnAndroidAppListFailed;
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        public void SetCategory(string category)
        {
            selectCategory = category;
        }

        public void ClearRTCSampleTestNumber()
        {
            rtcsampleTest = 25;
        }

        public void SetPage(int page)
        {
            currentPage = page;
        }

        public void StopTask()
        {
            taskStarted = false;
            StopCoroutine("Task");
        }

        public void StartTask()
        {
            taskStarted = true;
            StartCoroutine(Task());
        }

        IEnumerator Task()
        {
            while (taskStarted)
            {
                yield return new WaitForSeconds(1F);
                yield return GetAppliListTask();

                //androidGetAppList.Send();
                //yield return getAppliList.Send();
                //Debug.Log("================task running1.");
                //yield return TestTask();
                //Debug.Log("================task running2.");
                //if (reportResourceNumber % 60 == 0)
                //{
                //    yield return ReportResourceTask();
                //    reportResourceNumber = 0;
                //}
                //reportResourceNumber++;
                //Debug.Log("================task running3.");
            }
        }

        IEnumerator GetAppliListTask()
        {
            if (getAppliList != null)
            {
                yield return getAppliList.Send();
                if (!getAppliList.IsError)
                {
                    // Debug.Log("applist success:" + getAppliList.List.Count);
                    onApplistSuccess?.Invoke(getAppliList.List);
                } else
                {
                    Debug.Log("get applist failed:" + getAppliList.Error);
                    onApplistFailed?.Invoke(getAppliList.Error);
                }
            }
        }

        IEnumerator ReportResourceTask()
        {
            if (reportResource != null)
            {
                yield return reportResource.Send(currentPower * 100);
                if (!reportResource.IsError)
                {
                    onReportResourceSuccess?.Invoke();
                }
                else
                {
                    onReportResourceFailed?.Invoke();
                }
            }
        }

        IEnumerator TestTask()
        {
            yield return new WaitForSeconds(5F);
            yield break;
        }

        void UpdateSampleRTCStatus()
        {
            if (onSampleRTCStatusUpdated != null)
            {
            }
        }

        void OnAndroidAppListSuccess(List<GetAppliList.StartAppInfo> startAppInfo)
        {
            if (onApplistSuccess != null)
                onApplistSuccess(startAppInfo);
        }

        void OnAndroidAppListFailed(string e)
        {
            if (onApplistFailed != null)
                onApplistFailed(e);
        }
    }
}
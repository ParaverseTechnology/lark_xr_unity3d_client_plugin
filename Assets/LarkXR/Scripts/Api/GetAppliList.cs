using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkXR
{
    public class GetAppliList : ApiBase<GetAppliList.RawPageInfo>
    {
        private const string METHOD_APPLIST = "/appli/getAppliList";

        public override bool IsError
        {
            get
            {
                return base.IsError || !IsResultSuccess;
            }
        }

        public override string Error
        {
            get
            {
                if (!base.Error.Equals(""))
                {
                    return base.Error;
                }
                else if (!IsResultSuccess)
                {
                    return Message;
                }
                else
                {
                    return "";
                }
            }
        }

        public bool IsResultSuccess { get; private set; } = false;

        public string Message { get; private set; } = "";

        public Page CurrentPage
        {
            get;
            private set;
        } = null;

        public List<StartAppInfo> List { get; private set; } = null;


        public int PageNum = 1;
        public int PageSize = 8;

        public GetAppliList()
        {
        }

        public IEnumerator Send()
        {
            HttpQueryParam param = new HttpQueryParam();
            // fixed vr
            param.Add("appliType", "VR");

            if (PageNum > 0)
            {
                param.Add("page", PageNum + "");
            }

            if (PageSize > 0)
            {
                param.Add("pageSize", PageSize + "");
            }


            if (!ApiBase<object>.IS_APP_KEY_EMPTY && ApiBase<object>.IS_APP_SECRET_EMPTY) {
                param.Add("appKey", ApiBase<object>.APP_KEY);
            } else if (!ApiBase<object>.IS_APP_KEY_EMPTY && !ApiBase<object>.IS_APP_SECRET_EMPTY)
            {
                string timestamp = ApiUtils.CurrentTimeMillis().ToString();
                string signature = ApiUtils.GetSignature(ApiBase<object>.APP_KEY, ApiBase<object>.APP_SECRET, timestamp);

                param.Add("appKey", ApiBase<object>.APP_KEY);
                param.Add("timestamp", timestamp);
                param.Add("signature", signature);
            }

            // Debug.Log("get applist: " + param.ToString());

            yield return GetText(METHOD_APPLIST, param.ToString());
        }

        protected override void OnApiResponseSuccess(ApiResponse<GetAppliList.RawPageInfo> response)
        {
            base.OnApiResponseSuccess(response);
            // Debug.Log("============ applist serrch result:" + response.code + "; list:" + response.result);
            if (IsCodeSuccess(response.code))
            {
                IsResultSuccess = true;
                Message = response.message;
                List = response.result.records;

                CurrentPage = new Page();
                CurrentPage.records = response.result.records;

                try
                {
                    CurrentPage.pageNum = Int32.Parse(response.result.current);
                    CurrentPage.size = Int32.Parse(response.result.size);
                    CurrentPage.total = Int32.Parse(response.result.total);
                    CurrentPage.pages = Int32.Parse(response.result.pages);
                } catch (FormatException e)
                {
                    Debug.LogError("parse page info failed " + e.Message);
                }

                CurrentPage.hasPreviousPage = CurrentPage.pageNum > 1;
                CurrentPage.hasNextPage = CurrentPage.pageNum < CurrentPage.pages;

                CurrentPage.prePage = CurrentPage.hasPreviousPage ? CurrentPage.pageNum  - 1 : CurrentPage.pageNum;
                CurrentPage.nextPage = CurrentPage.hasNextPage ? CurrentPage.pageNum + 1 : CurrentPage.pageNum;

                CurrentPage.firstPage = 1;
                CurrentPage.lastPage = CurrentPage.pages;

                // CurrentPage.pageNum = int Set response.result.current;


                // CurrentPage = response.result;

                // fill page info.
                // raw:
                // current: "1"
                // pages: "3"
                // size: "8"
                // total: "24"
                // Debug.Log("page info " + CurrentPage);
            }
            else
            {
                IsResultSuccess = false;
                Message = response.message;
                List = null;
                CurrentPage = null;
            }
        }

        protected override void OnFailed(string error)
        {
            base.OnFailed(error);
            IsResultSuccess = false;
            Message = "";
            List = null;
            CurrentPage = null;
        }

        // new api page info
        [System.Serializable]
        public class RawPageInfo
        {
            public string current;
            public string size;
            public string total;
            public string pages;
            public List<StartAppInfo> records;
        }

        [System.Serializable]
        public class Page
        {
            public int endRow;
            public int firstPage;
            public bool hasNextPage;
            public bool hasPreviousPage;
            public bool isFirstPage;
            public bool isLastPage;
            public int lastPage;
            public List<StartAppInfo> records;
            public int navigateFirstPage;
            public int navigateLastPage;
            public int navigatePages;
            public List<int> navigatepageNums;
            public int nextPage;
            public int pageNum;
            public int pageSize;
            public int pages;
            public int prePage;
            public int size;
            public int startRow;
            public int total;
        }
        [System.Serializable]
        public class StartAppInfo
        {
            public string appliId;
            public string appliName;
            public string appliPath;
            public string appliType;
            public long createDate;
            public string createUser;
            public string deleteFlag;
            public string exeFileName;
            public string hasExtraParam;
            public string initWinSize;
            public short instanceMax;
            public string param;
            public string resolutionRatio;
            public short runCnt;
            public string startFlag;
            public string syncStatus;
            public long updateDate;
            public string updateUser;
            public string zipFile;

            public override bool Equals(object obj)
            {
                return obj is StartAppInfo info &&
                       appliId == info.appliId &&
                       appliName == info.appliName &&
                       appliType == info.appliType &&
                       runCnt == info.runCnt &&
                       startFlag == info.startFlag &&
                       syncStatus == info.syncStatus;
            }

            public override int GetHashCode()
            {
                int hashCode = 2024296495;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(appliId);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(appliName);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(appliType);
                hashCode = hashCode * -1521134295 + runCnt.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(startFlag);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(syncStatus);
                return hashCode;
            }
        }
    }
}


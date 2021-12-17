using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkXR
{
    public class GetAppliList : ApiBase<GetAppliList.Page>
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

        public List<StartAppInfo> List { get; private set; } = null;

        public GetAppliList()
        {
        }

        public IEnumerator Send()
        {
            HttpQueryParam param = new HttpQueryParam();
            // fixed vr
            param.Add("appliType", "VR");
            yield return GetText(METHOD_APPLIST, param.ToString());
        }

        protected override void OnApiResponseSuccess(ApiResponse<GetAppliList.Page> response)
        {
            base.OnApiResponseSuccess(response);
            //Debug.Log("============ applist serrch result:" + response.code + "; list:" + response.result.Count);
            if (IsCodeSuccess(response.code))
            {
                IsResultSuccess = true;
                Message = response.message;
                List = response.result.records;
            }
            else
            {
                IsResultSuccess = false;
                Message = response.message;
                List = null;
            }
        }

        protected override void OnFailed(string error)
        {
            base.OnFailed(error);
            IsResultSuccess = false;
            Message = "";
            List = null;
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
            public string total;
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
        }
    }
}


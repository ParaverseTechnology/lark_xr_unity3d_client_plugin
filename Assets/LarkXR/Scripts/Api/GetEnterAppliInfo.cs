using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkXR {
    public class GetEnterAppliInfo : ApiBase<GetEnterAppliInfo.EnterAppliInfo>
    {
        private const string METHOD_ENTERAPPLIINFO = "/getEnterAppliInfo";
        private readonly string macAddress;
        public GetEnterAppliInfo(string macAddress)
        {
            this.macAddress = macAddress;
        }

        public IEnumerator Send(string appliId)
        {
            HttpQueryParam param = new HttpQueryParam();
            param.Add("appliId", appliId);
            param.Add("clientMac", macAddress);
            yield return GetText(METHOD_ENTERAPPLIINFO, param.ToString());

            if (IsHttpError || IsNetworkError || IsParseJsonError)
            {
                Debug.Log("============ enter appli error:" + Error);
            }
            else
            {
                Debug.Log("============ enter applisuccess:" + ApiResponse.code + "; list:" + ApiResponse.result);
            }
        }



        [System.Serializable]
        public class EnterAppliInfo
        {
            public string wsServer;
            public string appliId;
            public string preferPubOutIp;
            public string appServer;
            public string wsPort;
            public string width;
            public int appPort;
            public string wsPath;
            public string initWinSize;
            public string taskId;
            public string height;
            public string network;
            public string noOperationTimeout;
        }
    }

}

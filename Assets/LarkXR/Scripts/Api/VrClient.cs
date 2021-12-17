using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LarkXR
{
    public class VrClient : ApiBase<string>
    {
        private const string METHOD_ONLINE = "/client/online";
        private const string METHOD_OFFLINE = "/client/offline";

        private readonly string macAddress;
        public VrClient(string macAddress)
        {
            this.macAddress = macAddress;
        }

        public IEnumerator ClientOnline()
        {
            //HttpQueryParam httpQueryParam = new HttpQueryParam();
            //httpQueryParam.Add("clientMac", macAddress);

            List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
            if (!macAddress.Equals(""))
                iparams.Add(new MultipartFormDataSection("clientMac", macAddress));

            yield return PostText(METHOD_ONLINE, iparams);
        }

        public IEnumerator ClientOffline()
        {
            //HttpQueryParam httpQueryParam = new HttpQueryParam();
            //httpQueryParam.Add("clientMac", macAddress);

            List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
            if (!macAddress.Equals(""))
                iparams.Add(new MultipartFormDataSection("clientMac", macAddress));

            yield return PostText(METHOD_OFFLINE, iparams);
        }

        protected override void OnApiResponseSuccess(ApiResponse<string> response)
        {
            base.OnApiResponseSuccess(response);
            Debug.Log("===========================VrClient success.code:" + response.code + ";message:" + response.message + ";mac address:" + macAddress);
        }

        protected override void OnFailed(string error)
        {
            base.OnFailed(error);
            //Debug.Log("===========================VrClient failed:" + error);
        }
    }
}


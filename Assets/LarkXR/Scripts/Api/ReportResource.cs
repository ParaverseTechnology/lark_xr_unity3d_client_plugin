using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LarkXR {
    public class ReportResource : ApiBase<string>
    {
        private const string METHOD_REPORTRESOURCE = "/client/reportResource";

        public override bool IsError
        {
            get
            {
                return base.IsError || IsResultError;
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
                else if (IsResultError)
                {
                    return message;
                }
                else
                {
                    return "";
                }
            }
        }

        public bool IsResultError
        {
            get
            {
                return !resultSuccess;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
        }

        bool resultSuccess = false;
        string message = "";

        private readonly string macAddress;
        private readonly string deviceName;
        // cache data.
        private readonly string memTotal;
        private readonly string memUsed;
        private readonly string diskTotal;
        private readonly string diskUsed;
        private readonly string sdTotal;
        private readonly string sdUsed;
        public ReportResource(string macAddress, string deviceName)
        {
            this.macAddress = macAddress;
            this.deviceName = deviceName;
            // TODO
            // update normal.
            this.memTotal = AndroidUtils.GetMemTotalMb().ToString();
            this.memUsed = AndroidUtils.GetMemUsed().ToString();
            this.diskTotal = AndroidUtils.GetDiskTotal().ToString();
            this.diskUsed = AndroidUtils.GetDiskUsed().ToString();
            this.sdTotal = AndroidUtils.GetSdMemMbTotal().ToString();
            this.sdUsed = AndroidUtils.GetSdMemUsed().ToString();
        }

        // power for head. 0 - 100 %;
        public IEnumerator Send(double power)
        {
            //HttpQueryParam param = new HttpQueryParam();
            //param.Add("clientMac", macAddress);
            //param.Add("clientBrand", deviceName);
            //param.Add("memTotal", AndroidUtils.GetMemTotalMb().ToString());
            //param.Add("memUsed", AndroidUtils.GetMemUsed().ToString());
            //param.Add("diskTotal", AndroidUtils.GetDiskTotal().ToString());
            //param.Add("diskUsed", AndroidUtils.GetDiskUsed().ToString());
            //param.Add("sdTotal", AndroidUtils.GetSdMemMbTotal().ToString());
            //param.Add("sdUsed", AndroidUtils.GetSdMemUsed().ToString());
            // TODO add battery
            // param.Add("batteryHead", "");
            // param.Add("batteryHandLeft", "");
            // param.Add("batteryHandRight", "");
            List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
            if (!macAddress.Equals(""))
            {
                /// TODO add default mac address.
                iparams.Add(new MultipartFormDataSection("clientMac", macAddress));
            }
            if (!deviceName.Equals(""))
            {
                iparams.Add(new MultipartFormDataSection("clientBrand", deviceName));
            }
            if (!memTotal.Equals(""))
            {
                iparams.Add(new MultipartFormDataSection("memTotal", memTotal));
            }
            if (!memUsed.Equals(""))
            {
                iparams.Add(new MultipartFormDataSection("memUsed", memUsed));
            }
            if (!diskTotal.Equals(""))
            {
                iparams.Add(new MultipartFormDataSection("diskTotal", diskTotal));
            }
            if (!diskUsed.Equals(""))
            {
                iparams.Add(new MultipartFormDataSection("diskUsed", diskUsed));
            }
            if (!sdTotal.Equals(""))
            {
                iparams.Add(new MultipartFormDataSection("sdTotal", sdTotal));
            }
            if (!sdUsed.Equals(""))
            {
                iparams.Add(new MultipartFormDataSection("sdUsed", sdUsed));
            }
            iparams.Add(new MultipartFormDataSection("batteryHead", power.ToString()));

            yield return PostText(METHOD_REPORTRESOURCE, iparams);

            //if (IsHttpError || IsNetworkError || IsParseJsonError)
            //{
            //    Debug.Log("error:" + Error);
            //}
            //else
            //{
            //    Debug.Log("============ report resource success:" + ApiResponse.code + "; result:" + ApiResponse.result);
            //}
        }

        protected override void OnApiResponseSuccess(ApiResponse<string> response)
        {
            base.OnApiResponseSuccess(response);
            if (IsCodeSuccess(response.code))
            {
                resultSuccess = true;
                message = response.message;
            } else
            {
                resultSuccess = false;
                message = response.message;
            }
        }

        protected override void OnFailed(string error)
        {
            base.OnFailed(error);
            resultSuccess = false;
            message = "";
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LarkXR
{
    public class RenderManger : MonoBehaviour
    {
        public const int EYE_LEFT = 0;
        public const int EYE_RIGHT = 1;
        public const int EYE_BOTH = 2;

        // got render texture.
        public delegate void OnTexture2D(Texture2D texture);
        public delegate void OnTexture2DStrereo(Texture2D textureLeft, Texture2D textureRight);
        // connected.
        public delegate void OnConnected();
        // on media ready
        public delegate void OnMediaReady();
        // got new frame.update render.
        public delegate void OnTrackingFrame(XRApi.TrackingFrame trackingFrame);
        // close
        public delegate void OnClose();

        public OnTexture2D onTexture2D;
        public OnTexture2DStrereo onTexture2DStereo;
        public OnConnected onConnected;
        public OnTrackingFrame onTrackingFrame;
        public OnClose onClose;
        public OnMediaReady onMediaReady;

        public bool Connected { get; private set; } = false;
        public bool IsFrameInited { get; private set; } = false;

        // render texture
        private Texture2D textureLeft { get; set; }
        private Texture2D textureRight { get; set; }
        private Texture2D textureAll { get; set; }

        private System.IntPtr textureLeftId = System.IntPtr.Zero;
        private System.IntPtr textureRightId = System.IntPtr.Zero;
        private System.IntPtr textureAllId = System.IntPtr.Zero;

        // public texture change to Texture2D
        public Texture2D RTextureLeft { get { return textureLeft; } }
        public Texture2D RTextureRight { get { return textureRight;  } }
        public Texture2D RTextureAll { get { return textureAll;  } }

        public bool IsStereoTexture { get; private set; }

        public int WaitFrameTimeoutMilliSeconds = 33;

        public bool UseRenderQueue {
            set
            {
                XRApi.SetUseRenderQueue(value);
                useRenderQueue = value;
            }
            get { return useRenderQueue; }
        }
        private bool useRenderQueue = false;

        public void SetRenderQueueSize(int size)
        {
            XRApi.SetRenderQueueSize(size);
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (XRApi.IsConnected())
            {
                if (!Connected)
                {
                    Connected = true;
                    onConnected?.Invoke();
                    Debug.Log("Connnected");
                }
                UpdateTexture();
                // Update pose only for test.
                //UpdatePose();
                UpdateRender();
            }
            else
            {
                if (Connected)
                {
                    // disconnected
                    Connected = false;
                    IsFrameInited = false;

                    textureLeftId = System.IntPtr.Zero;
                    textureRightId = System.IntPtr.Zero;
                    textureAllId = System.IntPtr.Zero;

                    textureLeft = null;
                    textureRight = null;
                    textureAll = null;

                    onClose?.Invoke();
                    Debug.Log("Disconected");
                }
            }
        }

        // 
        void UpdateTexture()
        {
            // init texture.
            if (!IsFrameInited && XRApi.IsFrameInited())
            {
                XRApi.HwRenderTexture hwTexture = XRApi.GetHwRenerTexture();

                int width = hwTexture.width;
                int height = hwTexture.height;

                if (hwTexture.type == XRApi.HwRenderTextureType.larkxrHwRenderTextureType_D3D11_Multiview || 
                    hwTexture.type == XRApi.HwRenderTextureType.larkxrHwRenderTextureType_Android_Multiview)
                {
                    if (hwTexture.textureSlot1 == System.IntPtr.Zero)
                    {
                        Debug.Log("init texture failed. texture empty. slot " + hwTexture.textureSlot1);
                        return;
                    }
                    textureAllId = hwTexture.textureSlot1;

                    Debug.Log("init texture success. texture. slot " + textureAllId + " " + width + " " + height);

                    textureAll = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, textureAllId);

                    Debug.Log("init texture success. texture. texture " + textureAll + " " + width + " " + height);

                    onTexture2D?.Invoke(textureAll);
                    // on media ready callback.
                    onMediaReady?.Invoke();

                    IsStereoTexture = false;
                    IsFrameInited = true;
                }
                else if (hwTexture.type == XRApi.HwRenderTextureType.larkxrHwRenderTextureType_D3D11_Stereo ||
                         hwTexture.type == XRApi.HwRenderTextureType.larkxrHwRenderTextureType_Android_Stereo)
                {
                    if (hwTexture.textureSlot1 == System.IntPtr.Zero || hwTexture.textureSlot2 == System.IntPtr.Zero)
                    {
                        Debug.Log("init texture failed. texture empty. slot " + hwTexture.textureSlot1 + " " + hwTexture.textureSlot2);
                        return;
                    }

                    textureLeftId = hwTexture.textureSlot1;
                    textureRightId = hwTexture.textureSlot2;
                    textureLeft = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, textureLeftId);
                    textureRight = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, textureRightId);

                    onTexture2DStereo?.Invoke(textureLeft, textureRight);
                    // on media ready callback.
                    onMediaReady?.Invoke();

                    IsStereoTexture = true;
                    IsFrameInited = true;
                }
                else
                {
                    Debug.LogWarning("init texture failed. texture not support." + hwTexture.type);
                }
            }
        }

        // show how to update device pose to cloud. only for test.
        void UpdatePose()
        {
            // update pose.
            // test data.
            Vector3 testPosition = new Vector3(0, 0, 0);
            // test rotation data.
            Quaternion testRotation = new Quaternion(-0.275494039F, 0.103380702F, 0.0448519662F, 0.95467472F);
            XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_HMD, testPosition, testRotation);
            XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_Controller_Left, testPosition, testRotation);
            XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_Controller_Right, testPosition, testRotation);
            // update device input
            XRApi.ControllerInputState controllerInputState = new XRApi.ControllerInputState();
            XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Left, controllerInputState);
            XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Right, controllerInputState);
            // send deivce pair info to server.
            XRApi.SendDeivcePair();
        }

        void UpdateRender()
        {
            // render texture
            if (IsFrameInited)
            {
                bool hasNewFrame = false;
                hasNewFrame = XRApi.WaitFroNewFrame(WaitFrameTimeoutMilliSeconds);

                if (!hasNewFrame)
                {
                    Debug.Log("wait timeout");
                }


                XRApi.TrackingFrame trackingFrame = new XRApi.TrackingFrame();
                XRApi.HwRenderTexture hwRenderTexture = new XRApi.HwRenderTexture();

                if (useRenderQueue)
                {
                    XRApi.RenderQueue(ref hwRenderTexture, ref trackingFrame);
                }
                else {
                    trackingFrame = XRApi.Render();
                }

                // hasNewFrame = trackingFrame.avaliable;

                if (!trackingFrame.avaliable)
                {
                    // Debug.Log("frame un avaliable ");
                }

#if UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || PLATFORM_STANDALONE_WIN
                if (IsStereoTexture)
                {
                    textureLeft.UpdateExternalTexture(textureLeftId);
                    textureRight.UpdateExternalTexture(textureRightId);
                }
                else
                {
                    textureAll.UpdateExternalTexture(textureAllId);
                }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || PLATFORM_STANDALONE_WIN
                XRApi.IssuePluginEvent();
#endif
#endif
                if (useRenderQueue) { 
                    XRApi.RenderQueueRelease();
                }

                onTrackingFrame?.Invoke(trackingFrame);

                if (hasNewFrame)
                {
                    // 提交统计数据
                    XRApi.CollectorrSubmit(trackingFrame);
                }
            }
            else {
                if (useRenderQueue) {
                    XRApi.TrackingFrame trackingFrame = new XRApi.TrackingFrame();
                    XRApi.HwRenderTexture hwRenderTexture = new XRApi.HwRenderTexture();
                    XRApi.RenderQueue(ref hwRenderTexture, ref trackingFrame);
                    // init texture.
                    UpdateTexture();
                    XRApi.RenderQueueRelease();
                }
            }
        }
    }
}

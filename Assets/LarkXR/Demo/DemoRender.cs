using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LarkXR;
public class DemoRender : MonoBehaviour
{
    public float sensitivityMouse = 2f;
    public float sensitivetyKeyBoard = 0.1f;
    public float sensitivetyMouseWheel = 10f;

    public GameObject hmd;
    public GameObject controllerLeft;
    public GameObject controllerRight;
    public RawImage renderLeft;
    public RawImage renderRight;
    public RawImage renderAll;

    public GameObject debugUIGroup;
    public Button buttonClose;

    public StateButton buttonAX;
    public StateButton buttonBY;
    public StateButton buttonJoystick;
    public StateButton buttonGrip;
    public StateButton buttonTrigger;
    public Toggle toggleController;

    public bool UseMutiView = false;

    // 是否启用htc样式手柄测试
    // 默认模拟 oculus 系列手柄
    public bool UseHTCTypeController = false;

    // 左上角模拟操作哪个手柄
    private bool isLeftController = true;
    // 手柄按钮的状态,按下或者抬起
    private bool buttonStateAX = false;
    private bool buttonStateBY = false;
    private bool buttonStateJoyStick = false;
    private bool buttonStateGrip = false;
    private bool buttonStateMenu = false;
    private bool buttonStateTrigger = false;

    void Start()
    {
        Debug.Assert(hmd != null);
        Debug.Assert(controllerLeft != null);
        Debug.Assert(controllerRight != null);

        renderAll.enabled = false;
        Debug.Assert(renderAll != null);

        renderLeft.enabled = false;
        Debug.Assert(renderLeft != null);

        renderRight.enabled = false;
        Debug.Assert(renderRight != null);

        Debug.Assert(debugUIGroup != null);
        Debug.Assert(buttonClose != null);

        Debug.Assert(buttonAX != null);
        Debug.Assert(buttonBY != null);
        Debug.Assert(buttonJoystick != null);
        Debug.Assert(buttonTrigger != null);
        Debug.Assert(buttonGrip != null);
        Debug.Assert(toggleController != null);

        // 初始化 SDK ID 
        string sdkID = "Your SDK ID";
        if (!XRApi.InitSdkAuthorization(sdkID)) 
        {
            int errCode = XRApi.GetLastError();
            Debug.LogError("初始化云雀SDK ID 失败 code " + errCode);
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || PLATFORM_STANDALONE_WIN
        // 设置 log 文件
        // XRApi.EnableDebugMode(true, System.Environment.CurrentDirectory + "/test.log");
#elif UNITY_ANDROID
        // 启用 android log
        XRApi.EnableDebugMode(true, "");
#endif

        // 设置客户端凭证。
        // 对应后台接入管理中设置的访问凭证
        // ApiBase<object>.SetCertificate("YourAppKey", "YourAppSecret");

        // 从本地文件中读取客户端访问凭证。
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || PLATFORM_STANDALONE_WIN
        XRManager.Instance.LoadCertificateFromFile();
#endif

        XRManager.Instance.RenderManger.onClose += OnClose;
        XRManager.Instance.RenderManger.onConnected += OnConnect;
        XRManager.Instance.RenderManger.onTexture2DStereo += OnTexture2DStrereo;
        XRManager.Instance.RenderManger.onTexture2D += OnTexture2D;

        // auto start task
        XRManager.Instance.AutoStartTask = true;
        // XRManager.Instance.AutoStartTask = false;

        // test use render queue mode.
        XRManager.Instance.RenderManger.UseRenderQueue = true;
        // XRManager.Instance.RenderManger.UseRenderQueue = false;
        // set render queue size;
        XRManager.Instance.RenderManger.SetRenderQueueSize(2);

        // config render.
        XRApi.RenderInfo renderInfo = XRApi.GetDefaultRenderInfo();
        renderInfo.renderWidth = 1920;
        renderInfo.renderHeight = 1080;
        renderInfo.fps = 60;
        
        // renderInfo.ipd = 0;
        XRApi.SetRenderInfo(renderInfo);

        // XRApi.SetupBitrateKbps(20 * 1000);
        // XRApi.SetStreamType(XRApi.StreamType.larkStreamType_UDP);
        XRApi.SetStreamType(XRApi.StreamType.larkStreamType_KCP);
        // XRApi.SetResolutionScale(0.5f);

        // 设置头盔类型为，主要会影响云端手柄的默认展示
        var headSetControllerDesc = XRApi.GetDefaultHeadSetControllerDesc();
        if (UseHTCTypeController)
        {
            headSetControllerDesc.type = XRApi.HeadSetType.larkHeadSetType_HTC;
        } else
        {
            // 设置为 oculus 系列类型
            headSetControllerDesc.type = XRApi.HeadSetType.larkHeadSetType_OCULUS;
        }
        XRApi.SetHeadSetControllerDesc(headSetControllerDesc);

        // 是否输出左右眼在同一张纹理上面
        XRApi.SetUseMultiview(UseMutiView);

        // 强制指定 TCP 模式，测试使用
        XRApi.QuickConfigLevel level = XRApi.QuickConfigLevel.QuickConfigLevel_Manual;
        XRApi.QuickConfigWithDefaulSetup(level);
        Config.SetQuickSetupLevel((int)level);
        // XRApi.SetStreamType(XRApi.StreamType.larkStreamType_TCP);

        // test frame rate.
        Application.targetFrameRate = 60;

        // 手机常亮
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // debug ui
        debugUIGroup.SetActive(false);
        buttonClose.gameObject.SetActive(false);
        buttonClose.onClick.AddListener(Close);

        // XRApi.SendText("your data");
    }

    /*    private void OnGUI()
        {
            if (!XRApi.IsConnected())
            {
                return;
            }

            GUI.Box(new Rect(10, 10, 260, 150), "Input Test");
            if (GUI.Button(new Rect(20, 40, 50, 20), "关闭")) {
                // 关闭当前连接
                XRManager.Instance.OnClose();
                Debug.Log("关闭当前连接");
            }

            buttonAX = GUI.RepeatButton(new Rect(20, 70, 50, 20), "X/A");
            buttonBY = GUI.RepeatButton(new Rect(80, 70, 50, 20), "Y/B");
            buttonJoyStick = GUI.RepeatButton(new Rect(140, 70, 100, 20), "Joystick");
            buttonGrip = GUI.RepeatButton(new Rect(20, 100, 50, 20), "grip");
            buttonMenu = GUI.RepeatButton(new Rect(80, 100, 50, 20), "menu");
            buttonTrigger = GUI.RepeatButton(new Rect(140, 100, 100, 20), "trigger");

            // 是否是左手柄
            isLeftController = GUI.Toggle(new Rect(20, 130, 380, 20), isLeftController, "左手柄");
        }*/

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        if (XRApi.IsConnected())
        {
            UpdateCloudPose();

            // Debug.Log("AX " + buttonAX.Pressed);
            buttonStateAX = buttonAX.Pressed;
            buttonStateBY = buttonBY.Pressed;
            buttonStateJoyStick = buttonJoystick.Pressed;
            buttonStateGrip = buttonGrip.Pressed;
            buttonStateTrigger = buttonTrigger.Pressed;
            isLeftController = toggleController.isOn;
        }
    }
    private void UpdateCamera()
    {
        // 滚轮实现镜头缩进和拉远
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            // Camera.main.fieldOfView = Camera.main.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * sensitivetyMouseWheel;
        }
        // 按着鼠标左键实现视角转动
        if (Input.GetMouseButton(0))
        {
            hmd.transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityMouse, 0);
        }
        // 键盘按钮←/a和→/d实现视角水平移动，键盘按钮↑/w和↓/s实现视角水平旋转
        if (Input.GetAxis("Horizontal") != 0)
        {
            hmd.transform.Translate(Input.GetAxis("Horizontal") * sensitivetyKeyBoard, 0, 0);
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            hmd.transform.Translate(0, 0, Input.GetAxis("Vertical") * sensitivetyKeyBoard);
        }
/*        if (Input.GetAxis("UpDown") != 0)
        {
            hmd.transform.Translate(0, Input.GetAxis("UpDown") * sensitivetyKeyBoard, 0);
        }*/
    }
    #region cloud
    private void UpdateCloudPose()
    {
        if (hmd == null)
        {
            return;
        }

        // unity坐标系转换为 openvr坐标系
        OpenVrPose openVrPose = new OpenVrPose(hmd.transform);
        // 模拟房间高度设置
        openVrPose.Position.y += LarkXR.Config.GetExtraHeight();
        // 设置头盔位置和旋转
        XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_HMD, openVrPose.Position, openVrPose.Rotation);

        // 左手手柄
        // 设置手柄的姿态
        XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_Controller_Left, controllerLeft.transform);

        if (UseHTCTypeController) {
            // HTC 类型手柄状态
            XRApi.ControllerInputStateNative controllerInputStateNativeLeft = new XRApi.ControllerInputStateNative();
            controllerInputStateNativeLeft.deviceType = XRApi.DeviceType.Device_Type_Controller_Left;
            controllerInputStateNativeLeft.isConnected = true;
            controllerInputStateNativeLeft.triggerValue = 0.0f;
            controllerInputStateNativeLeft.gripValue = 0.0f;
            controllerInputStateNativeLeft.batteryPercentRemaining = 0;
            if (isLeftController)
            {
                if (buttonStateJoyStick)
                {
                    // controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Trackpad_Click);
                    // controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Trackpad_Touch);
                    // controllerInputStateNativeLeft.touchPadAxis = new Vector2(0.5f, 0.5f);

                    // controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Trackpad_X);
                    // controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Trackpad_Y);

                    // TODO why?
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Joystick_Click);
                }
                if (buttonStateTrigger)
                {
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Trigger_Click);
                    // 按下时triggervalue设置为1
                    controllerInputStateNativeLeft.triggerValue = 1.0f;
                }
                if (buttonStateMenu) {
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_System_Click);
                }
                if (buttonStateGrip)
                {
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Grip_Click);
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Grip_Touch);
                    // 按下时triggervalue设置为1
                    controllerInputStateNativeLeft.gripValue = 1.0f;
                }
            }
            XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Left, controllerInputStateNativeLeft);
        }
        else
        {
            // 设置手柄按钮的状态
            XRApi.ControllerInputStateNative controllerInputStateNativeLeft = new XRApi.ControllerInputStateNative();
            controllerInputStateNativeLeft.deviceType = XRApi.DeviceType.Device_Type_Controller_Left;
            controllerInputStateNativeLeft.isConnected = true;
            controllerInputStateNativeLeft.triggerValue = 0.0f;
            controllerInputStateNativeLeft.gripValue = 0.0f;
            controllerInputStateNativeLeft.batteryPercentRemaining = 0;
            if (isLeftController)
            {
                if (buttonStateAX)
                {
                    // quest 类型手柄左手 A 键
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_A_Click);
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_A_Touch);
                }
                if (buttonStateBY)
                {
                    // quest 类型手柄左手 B 键
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_B_Click);
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_B_Touch);
                }
                if (buttonStateJoyStick)
                {
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Joystick_Click);
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Joystick_Touch);
                    controllerInputStateNativeLeft.touchPadAxis.y = 1.0f;
                    controllerInputStateNativeLeft.touchPadAxis.x = 0.0f;
                }
                if (buttonStateMenu)
                {
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Application_Menu_Click);
                }
                if (buttonStateTrigger)
                {
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Trigger_Click);
                    // 按下时triggervalue设置为1
                    controllerInputStateNativeLeft.triggerValue = 1.0f;
                }
                if (buttonStateGrip)
                {
                    controllerInputStateNativeLeft.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Grip_Click);
                    // 按下时triggervalue设置为1
                    controllerInputStateNativeLeft.gripValue = 1.0f;
                }
            }
            XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Left, controllerInputStateNativeLeft);
        }


        // 右手手柄
        // 设置手柄的姿态
        XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_Controller_Right, controllerRight.transform);

        if (UseHTCTypeController) {
            // HTC 类型手柄状态
            // 设置手柄按钮的状态
            XRApi.ControllerInputStateNative controllerInputStateNativeRight = new XRApi.ControllerInputStateNative();
            controllerInputStateNativeRight.deviceType = XRApi.DeviceType.Device_Type_Controller_Right;
            controllerInputStateNativeRight.isConnected = true;
            controllerInputStateNativeRight.triggerValue = 0.0f;
            controllerInputStateNativeRight.gripValue = 0.0f;
            controllerInputStateNativeRight.batteryPercentRemaining = 0;
            if (!isLeftController)
            {
                if (buttonStateJoyStick)
                {
                    // controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Trackpad_Click);
                    // controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Trackpad_Touch);

                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Joystick_Click);
                }
                if (buttonStateTrigger)
                {
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Trigger_Click);
                    // 按下时triggervalue设置为1
                    controllerInputStateNativeRight.triggerValue = 1.0f;
                }
                if (buttonStateMenu)
                {
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Application_Menu_Click);
                }
                if (buttonStateGrip)
                {
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Grip_Click);
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Grip_Touch);
                    // 按下时triggervalue设置为1
                    controllerInputStateNativeRight.gripValue = 1.0f;
                }
            }
            XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Right, controllerInputStateNativeRight);
        }
        else
        {
            // 设置手柄按钮的状态
            XRApi.ControllerInputStateNative controllerInputStateNativeRight = new XRApi.ControllerInputStateNative();
            controllerInputStateNativeRight.deviceType = XRApi.DeviceType.Device_Type_Controller_Right;
            controllerInputStateNativeRight.isConnected = true;
            controllerInputStateNativeRight.triggerValue = 0.0f;
            controllerInputStateNativeRight.gripValue = 0.0f;
            controllerInputStateNativeRight.batteryPercentRemaining = 0;
            if (!isLeftController)
            {
                if (buttonStateAX)
                {
                    // quest 类型手柄右手 X 键
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_X_Click);
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_X_Touch);
                }
                if (buttonStateBY)
                {
                    // quest 类型手柄右手 Y 键
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Y_Click);
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Y_Touch);
                }
                if (buttonStateJoyStick)
                {
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Joystick_Click);
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Joystick_Touch);
                    controllerInputStateNativeRight.touchPadAxis.y = 1.0f;
                    controllerInputStateNativeRight.touchPadAxis.x = 0.0f;
                }
                if (buttonStateMenu)
                {
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Application_Menu_Click);
                }
                if (buttonStateTrigger)
                {
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Trigger_Click);
                    // 按下时triggervalue设置为1
                    controllerInputStateNativeRight.triggerValue = 1.0f;
                }
                if (buttonStateGrip)
                {
                    controllerInputStateNativeRight.AddButtonState(XRApi.InputButtonFlag.larkxr_Input_Grip_Click);
                    // 按下时triggervalue设置为1
                    controllerInputStateNativeRight.gripValue = 1.0f;
                }
            }
            XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Right, controllerInputStateNativeRight);
        }

        // send deivce pair info to server.
        XRApi.SendDeivcePair();
    }
    private void OnTexture2D(Texture2D texture)
    {
        Debug.Log("===============TestRender OnTexture2D");
        renderAll.enabled = true;
        renderAll.texture = texture;

        // Graphics.Blit(texture, (RenderTexture)null);

        /*        Mesh mesh = GetComponent<MeshFilter>().mesh;
                Vector3[] vertices = mesh.vertices;
                Vector2[] uvs = new Vector2[vertices.Length];

                // front u v
                //  0,0--------0.5,0--------1,0
                //   |           |           |
                //   |           |           |
                //  0,1,-------0.5,1--------1,1
                uvs[0] = new Vector2(0.0f, 0.0f);
                uvs[1] = new Vector2(1.0f, 0.0f);
                uvs[2] = new Vector2(0.0f, 1.0f);
                uvs[3] = new Vector2(1.0f, 1.0f);

                // Back
                uvs[6] = new Vector2(0.0f, 0.0f);
                uvs[7] = new Vector2(1.0f, 0.0f);
                uvs[10] = new Vector2(0.0f, 1.0f);
                uvs[11] = new Vector2(1.0f, 1.0f);
                mesh.uv = uvs;*/
    }

    private void OnTexture2DStrereo(Texture2D textureLeft, Texture2D textureRight)
    {
        Debug.Log("===============TestRender OnTexture2DStrereo");
        renderLeft.enabled = true;
        renderLeft.texture = textureLeft;

        renderRight.enabled = true;
        renderRight.texture = textureRight;
        // new Rect(0.5f, 0, 0.5f, 1);
        // render.uvRect = new Rect(0, 0, 0.5f, 1);
    }

    private void OnClose()
    {
        renderAll.enabled = false;
        renderLeft.enabled = false;
        renderRight.enabled = false;
        hmd.transform.position = new Vector3(0, 0, 0);
        hmd.transform.rotation = new Quaternion(0, 0, 0, 1);

        buttonClose.gameObject.SetActive(false);
        debugUIGroup.SetActive(false);
    }

    private void OnConnect()
    {
        hmd.transform.position = new Vector3(0, 0, 0);
        hmd.transform.rotation = new Quaternion(0, 0, 0, 1);

        buttonClose.gameObject.SetActive(true);
        debugUIGroup.SetActive(true);
    }
    #endregion
    public void Close()
    {
        XRManager.Instance.OnClose();
    }
}

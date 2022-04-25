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
    public RawImage renderLeft;
    public RawImage renderRight;
    public RawImage renderAll;

    public bool UseMutiView = false;

    private Button closeButton;
    void Start()
    {
        Debug.Assert(hmd != null);

        renderAll.enabled = false;
        Debug.Assert(renderAll != null);

        renderLeft.enabled = false;
        Debug.Assert(renderLeft != null);

        renderRight.enabled = false;
        Debug.Assert(renderRight != null);

        closeButton = GetComponentInChildren<Button>();
        Debug.Assert(closeButton != null);
        closeButton.gameObject.SetActive(false);

        // 初始化 SDK ID 
        string sdkID = "初始化 SDK ID ";

        if (!XRApi.InitSdkAuthorization(sdkID)) {
            int errCode = XRApi.GetLastError();
            Debug.LogError("初始化云雀SDK ID 失败 code " + errCode);
        }

        XRManager.Instance.RenderManger.onClose += OnClose;
        XRManager.Instance.RenderManger.onConnected += OnConnect;
        XRManager.Instance.RenderManger.onTexture2DStereo += OnTexture2DStrereo;
        XRManager.Instance.RenderManger.onTexture2D += OnTexture2D;

        // config render.
        XRApi.RenderInfo renderInfo = XRApi.GetDefaultRenderInfo();
        renderInfo.renderWidth = 1920;
        renderInfo.renderHeight = 1080;
        renderInfo.fps = 60;
        // renderInfo.ipd = 0;
        XRApi.SetRenderInfo(renderInfo);
        XRApi.SetupBitrateKbps(50 * 1000);

        // 是否输出左右眼在同一张纹理上面
        XRApi.SetUseMultiview(UseMutiView);

        // test frame rate.
        Application.targetFrameRate = 60;

        // 手机常亮
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        if (XRApi.IsConnected())
        {
            UpdateCloudPose();
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
        OpenVrPose openVrPose = new OpenVrPose(hmd.transform);
        openVrPose.Position.y += LarkXR.Config.GetExtraHeight();
        XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_HMD, openVrPose.Position, openVrPose.Rotation);
        //XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_HMD, mainCamera.transform.position, mainCamera.transform.rotation);
        //XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_Controller_Left, testPosition, testRotation);
        //XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_Controller_Right, testPosition, testRotation);
        // update device input
        //XRApi.ControllerInputState controllerInputState = new XRApi.ControllerInputState();
        //XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Left, controllerInputState);
        //XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Right, controllerInputState);
        // send deivce pair info to server.
        XRApi.SendDeivcePair();
    }
    private void OnTexture2D(Texture2D texture)
    {
        Debug.Log("===============TestRender OnTexture2D");
        renderAll.enabled = true;
        renderAll.texture = texture;
        closeButton.gameObject.SetActive(true);


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
        closeButton.gameObject.SetActive(true);
    }

    private void OnClose()
    {
        renderAll.enabled = false;
        renderLeft.enabled = false;
        renderRight.enabled = false;
        closeButton.gameObject.SetActive(false);
        hmd.transform.position = new Vector3(0, 0, 0);
        hmd.transform.rotation = new Quaternion(0, 0, 0, 1);
    }

    private void OnConnect()
    {
        closeButton.gameObject.SetActive(true);
        hmd.transform.position = new Vector3(0, 0, 0);
        hmd.transform.rotation = new Quaternion(0, 0, 0, 1);
    }
    #endregion
}

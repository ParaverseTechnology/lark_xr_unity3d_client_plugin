using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LarkXR;
using zSpace.Core.Interop;
using zSpace.Core;
using System;
using zSpace.Core.Sdk;
using System.Runtime.InteropServices;
using UnityEditor;
using zSpace.Core.Extensions;
using zSpace.Core.Input;

public class ZspaceDemo : MonoBehaviour
{
    public float sensitivityMouse = 2f;
    public float sensitivetyKeyBoard = 0.1f;
    public float sensitivetyMouseWheel = 10f;

    public Button closeButton;

    public RawImage leftImage;
    public RawImage rightImage;

    ZStylus stylus;

    void Start()
    {
        Debug.Assert(leftImage != null);
        Debug.Assert(rightImage != null);

        leftImage.enabled = false;
        rightImage.enabled = false;

        Debug.Assert(closeButton != null);

        closeButton.gameObject.SetActive(false);

        // 初始化 SDK ID 
        string sdkID = "初始化 SDK ID";
        if (!XRApi.InitSdkAuthorization(sdkID))
        {
            int errCode = XRApi.GetLastError();
            Debug.LogError("初始化云雀SDK ID 失败 code " + errCode);
        }

        // 设置头盔类型
        XRApi.HeadSetControllerDesc headSetControllerDesc = XRApi.GetDefaultHeadSetControllerDesc();
        headSetControllerDesc.type = XRApi.HeadSetType.larkHeadSetType_HTC;
        XRApi.SetHeadSetControllerDesc(headSetControllerDesc);

        XRManager.Instance.RenderManger.onClose += OnClose;
        XRManager.Instance.RenderManger.onConnected += OnConnect;
        XRManager.Instance.RenderManger.onTexture2DStereo += OnRenderTextureStereo;
        XRManager.Instance.RenderManger.onTexture2D += OnTexture2D;

        /*        ZDisplay display = ZProvider.Context.DisplayManager.GetDisplay(ZDisplayType.zSpace);

                if (display == null)
                {
                    Debug.LogError(
                        "Trying to get zSpace system serial number for " +
                        "licensing purposes, but no zSpace display could be " +
                        "found.  Error details:");
                    return;
                }

                string zSpaceSystemSerialNumber =
                display.GetAttribute(ZDisplayAttribute.SerialNumber);

                Debug.Log("Serial Number: " + zSpaceSystemSerialNumber);*/


        // 是否输出左右眼在同一张纹理上面
        XRApi.SetUseMultiview(false);

        stylus = GameObject.FindObjectOfType<ZStylus>();
        Debug.Assert(stylus != null);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();

        if (XRApi.IsConnected())
        {
            UpdateCloudPose();

            if (Input.GetKeyUp(KeyCode.Escape)) {
                XRManager.Instance.OnClose();
            }
        }
    }

    private void OnDestroy()
    {
        // Shut down and destroy the XR Overlay.
        ZPlugin.DestroyXROverlay();
    }

    private void OnGUI()
    {
/*        GUI.Box(new Rect(10, 10, 600, 90), "Debug Info");
        GUI.Label(new Rect(25, 35, 300, 30), "IsXROverlayActive " + ZPlugin.IsXROverlayActive());
        GUI.Label(new Rect(25, 55, 300, 30), "IsXROverlayEnabled " + ZPlugin.IsXROverlayEnabled());
        GUI.Label(new Rect(25, 75, 300, 30), "ZProvider.IsInitialized " + ZProvider.IsInitialized);
        GUI.Label(new Rect(300, 35, 300, 30), "createOverlayresult " + createOverlayresult);
        GUI.Label(new Rect(300, 55, 300, 30), "hwnd " + hwnd);
        GUI.Label(new Rect(300, 75, 300, 30), "winRect " + winRect.ToString());*/
    }

    private void UpdateCamera()
    {
        // 滚轮实现镜头缩进和拉远
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.main.fieldOfView = Camera.main.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * sensitivetyMouseWheel;
        }
        // 按着鼠标左键实现视角转动
        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityMouse, 0);
        }
        // 键盘按钮←/a和→/d实现视角水平移动，键盘按钮↑/w和↓/s实现视角水平旋转
        if (Input.GetAxis("Horizontal") != 0)
        {
            Camera.main.transform.Translate(Input.GetAxis("Horizontal") * sensitivetyKeyBoard, 0, 0);
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            Camera.main.transform.Translate(0, Input.GetAxis("Vertical") * sensitivetyKeyBoard, 0);
        }
    }
    #region cloud
    private void UpdateCloudPose()
    {
        if (Camera.main == null) {
            return;
        }
        OpenVrPose openVrPose = new OpenVrPose(Camera.main.transform);
        openVrPose.Position.y += LarkXR.Config.GetExtraHeight();

        XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_HMD, openVrPose.Position, openVrPose.Rotation);

        XRApi.ControllerInputState controllerInputState = new XRApi.ControllerInputState();
        if (stylus != null) {
            Vector3 position = stylus.transform.localPosition;
            Quaternion rotation = stylus.transform.localRotation;
            Quaternion rotation2 = Quaternion.Euler(45, 0, 0);
            rotation *= rotation2;

            OpenVrPose stylusPose = new OpenVrPose(position, Matrix4x4.Rotate(rotation));
            stylusPose.Position.y += LarkXR.Config.GetExtraHeight();
            XRApi.UpdateDevicePose(XRApi.DeviceType.Device_Type_Controller_Right, stylusPose.Position, stylusPose.Rotation);
            
            controllerInputState.deviceType = XRApi.DeviceType.Device_Type_Controller_Right;
            controllerInputState.isConnected = true;
            controllerInputState.triggerPressed = stylus.GetButton(0);
            controllerInputState.gripPressed = stylus.GetButton(1);
            controllerInputState.touchPadPressed = stylus.GetButton(2);
            XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Right, controllerInputState);
        } else
        {
            controllerInputState.deviceType = XRApi.DeviceType.Device_Type_Controller_Right;
            controllerInputState.isConnected = false;
            XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Right, controllerInputState);
        }

        controllerInputState.deviceType = XRApi.DeviceType.Device_Type_Controller_Left;
        controllerInputState.isConnected = false;
        XRApi.UpdateControllerInput(XRApi.ControllerType.Controller_Left, controllerInputState);

        // send deivce pair info to server.
        XRApi.SendDeivcePair();
    }
    
    private void OnRenderTextureStereo(Texture2D textureLeft, Texture2D textureRight)
    {
        Debug.Log("===============TestRender OnRenderTextureStereo");
        leftImage.texture = textureLeft;
        rightImage.texture = textureRight;

        leftImage.enabled = true;
        rightImage.enabled = true;

        closeButton.gameObject.SetActive(true);
    }

    private void OnTexture2D(Texture2D texture)
    {
        Debug.Log("===============TestRender OnTexture2D");
        leftImage.texture = texture;
        rightImage.texture = texture;

        leftImage.enabled = true;
        rightImage.enabled = true;

        closeButton.gameObject.SetActive(true);
    }

    private void OnConnect()
    {
        closeButton.gameObject.SetActive(true);
    }
    private void OnClose()
    {
        leftImage.enabled = false;
        rightImage.enabled = false;

        closeButton.gameObject.SetActive(false);
    }
    #endregion
}

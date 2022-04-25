using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestApi : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var color = LarkXR.XRApi.GetDefaultColorCorrention();
        Debug.Log("Defautl color correction " + color.ToString());

        color.brightness = 0;
        color.contrast = 0;
        color.enableColorCorrection = false;
        color.gamma = 0;
        color.saturation = 0;
        color.sharpening = 0;

        LarkXR.XRApi.SetColorCorrention(color);
        color = LarkXR.XRApi.GetColorCorrention();
        Debug.Log("set color correction to " + color.ToString());
        color = LarkXR.XRApi.GetDefaultColorCorrention();
        LarkXR.XRApi.SetColorCorrention(color);

        var headset = LarkXR.XRApi.GetDefaultHeadSetControllerDesc();
        Debug.Log("default headset " + headset.ToString());

        headset.type = LarkXR.XRApi.HeadSetType.larkHeadSetType_NOLO_Sonic_1;
        headset.forece3dof = false;
        headset.hapticsIntensity = 0;

        LarkXR.XRApi.SetHeadSetControllerDesc(headset);
        headset = LarkXR.XRApi.GetHeadSetControllerDesc();
        Debug.Log("set headset to " + headset.ToString());
        headset = LarkXR.XRApi.GetDefaultHeadSetControllerDesc();
        LarkXR.XRApi.SetHeadSetControllerDesc(headset);

        var fov = LarkXR.XRApi.GetDefaultFoveatedRendering();
        Debug.Log("default fov " + fov.ToString());

        fov.enableFoveateRendering = false;
        fov.foveationShape = 0;
        fov.foveationStrength = 0;
        fov.foveationVerticalOffset = 0;

        LarkXR.XRApi.SetFoveatedRendering(fov);
        fov = LarkXR.XRApi.GetFoveatedRendering();
        Debug.Log("set fov to " + fov.ToString());
        fov = LarkXR.XRApi.GetDefaultFoveatedRendering();
        LarkXR.XRApi.SetFoveatedRendering(fov);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

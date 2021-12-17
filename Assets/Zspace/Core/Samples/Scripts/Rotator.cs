////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace zSpace.Core.Samples
{
    // Specifying the Script Execution Order is recommended when rotating 
    // transforms containing a ZFrame or ZCameraRig.
    [DefaultExecutionOrder(ZCameraRig.ScriptPriority - 1)]
    public class Rotator : MonoBehaviour
    {
        public Vector3 DegreesPerSecond;
        
        void Update()
        {
            Vector3 deltaAngles = this.DegreesPerSecond * Time.unscaledDeltaTime;
            Vector3 eulerAngles = this.transform.localEulerAngles;

            this.transform.localRotation = 
                Quaternion.Euler(eulerAngles) *
                Quaternion.Euler(deltaAngles);
        }
    }
}

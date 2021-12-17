////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Extensions;

namespace zSpace.Core
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(ScriptPriority)]
    public class ZCameraRig : MonoBehaviour
    {
        public const int ScriptPriority = ZProvider.ScriptPriority + 10;

        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The viewing frame to attach the camera rig to. When attached,
        /// the camera rig's transform is controlled by the viewing frame and 
        /// any attempts to directly modify it will be overridden.
        /// </summary>
        [Tooltip(
            "The viewing frame to attach the camera rig to. When attached, " +
            "the camera rig's transform is controlled by the viewing frame " +
            "and any attempts to directly modify it will be overridden.")]
        public ZFrame Frame = null;

        /// <summary>
        /// The scale of the user's current view. At a value of 1, one 
        /// Unity world unit is equal to one meter.
        /// </summary>
        [Range(0.1f, 1000.0f)]
        [Tooltip(
            "The scale of the user's current view. At a value of 1, one " +
            "Unity world unit is equal to one meter.")]
        public float ViewerScale = 1.0f;

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void Update()
        {
            if (this.Frame != null)
            {
                this.transform.position = this.Frame.WorldPosition;
                this.transform.rotation = this.Frame.WorldRotation;

                this.ViewerScale = this.Frame.ViewerScale;
            }

            this.transform.SetUniformScale(
                this.ViewerScale * ZProvider.DisplayScaleFactor);
        }
    }
}

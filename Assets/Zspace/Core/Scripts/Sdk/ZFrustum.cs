////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;

using UnityEngine;

using zSpace.Core.Extensions;
using zSpace.Core.Interop;

namespace zSpace.Core.Sdk
{
    public class ZFrustum : ZNativeResource
    {
        public ZFrustum(ZViewport viewport, IntPtr nativePtr)
            : base(nativePtr)
        {
            this._viewport = viewport;

            this.CameraOffset = DefaultCameraOffset;
            this.DisplayEulerAngles = ZDisplay.DefaultEulerAngles;
            this.PortalModeFlags = ZPortalMode.None;
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Static Members
        ////////////////////////////////////////////////////////////////////////

        public static readonly float DefaultIpd = 0.06f;

        public static readonly float DefaultNearClip = 0.03f;

        public static readonly float DefaultFarClip = 100.0f;

        public static readonly Vector3 DefaultCameraOffset = 
            Vector3.back * 0.25f;

        public static readonly float CoupledZoneDepth = -0.13f;

        public static readonly float UncoupledZoneDepth = 0.3f;

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets/sets the physical separation, or inter-pupillary distance, 
        /// between the eyes in meters. (Default: 0.06)
        /// </summary>
        public float Ipd
        {
            get
            {
                return this.GetAttributeFloat(ZFrustumAttribute.Ipd);
            }
            set
            {
                this.SetAttribute(ZFrustumAttribute.Ipd, value);
            }
        }

        /// <summary>
        /// Gets/sets the near clipping plane for the frustum in meters. 
        /// (Default: 0.03)
        /// </summary>
        public float NearClip
        {
            get
            {
                return this.GetAttributeFloat(ZFrustumAttribute.NearClip);
            }
            set
            {
                this.SetAttribute(ZFrustumAttribute.NearClip, value);
            }
        }

        /// <summary>
        /// Gets/sets the far clipping plane for the frustum in meters. 
        /// (Default: 1000)
        /// </summary>
        public float FarClip
        {
            get
            {
                return this.GetAttributeFloat(ZFrustumAttribute.FarClip);
            }
            set
            {
                this.SetAttribute(ZFrustumAttribute.FarClip, value);
            }
        }

        /// <summary>
        /// Gets/sets the  display's desired euler angles in degrees about the
        /// X, Y, and Z axes. These angles are only used when PortalMode.Angle 
        /// is not enabled on the frustum. (Default: [x=90, y=0, z=0])
        /// </summary>
        public Vector3 DisplayEulerAngles
        {
            get
            {
                float x = this.GetAttributeFloat(
                    ZFrustumAttribute.DisplayAngleX);

                float y = this.GetAttributeFloat(
                    ZFrustumAttribute.DisplayAngleY);

                float z = this.GetAttributeFloat(
                    ZFrustumAttribute.DisplayAngleZ);

                return new Vector3(x, y, z);
            }
            set
            {
                this.SetAttribute(ZFrustumAttribute.DisplayAngleX, value.x);
                this.SetAttribute(ZFrustumAttribute.DisplayAngleY, value.y);
                this.SetAttribute(ZFrustumAttribute.DisplayAngleZ, value.z);
            }
        }

        /// <summary>
        /// Gets/sets the offset of the camera in meters relative to the 
        /// center of the application window's viewport. This offset is 
        /// used when computing the transformation from viewport to camera 
        /// space. (Default: [x=0, y=0, z=-0.25])
        /// </summary>
        public Vector3 CameraOffset
        {
            get
            {
                ZVector3 cameraOffset;
                ZPlugin.LogOnError(ZPlugin.GetFrustumCameraOffset(
                    this._nativePtr, out cameraOffset),
                    "GetFrustumCameraOffset");

                return cameraOffset.ToVector3();
            }
            set
            {
                ZPlugin.LogOnError(ZPlugin.SetFrustumCameraOffset(
                    this._nativePtr, value.ToZVector3()),
                    "SetFrustumCameraOffset");
            }
        }

        /// <summary>
        /// Gets/sets the flags leveraged to enable features such as 
        /// angle-awareness.
        /// </summary>
        /// 
        /// <remarks>
        /// In portal mode, the scene is fixed relative to the physical 
        /// world, not the viewport. Refer to ZPortalMode for details on 
        /// portal modes available.
        /// </remarks>
        public ZPortalMode PortalModeFlags
        {
            get
            {
                ZPortalMode portalModeFlags = ZPortalMode.None;
                ZPlugin.LogOnError(ZPlugin.GetFrustumPortalMode(
                    this._nativePtr, out portalModeFlags),
                    "GetFrustumPortalMode");

                return portalModeFlags;
            }
            private set
            {
                ZPlugin.LogOnError(
                    ZPlugin.SetFrustumPortalMode(this._nativePtr, value),
                    "SetFrustumPortalMode");
            }
        }

        /// <summary>
        /// Gets/sets the frustum's current head pose.
        /// </summary>
        public Pose HeadPose
        {
            get
            {
                ZPose headPose;
                ZPlugin.LogOnError(
                    ZPlugin.GetFrustumHeadPose(this._nativePtr, out headPose),
                    "GetFrustumHeadPose");

                return headPose.ToPose();
            }
            set
            {
                ZPlugin.LogOnError(ZPlugin.SetFrustumHeadPose(
                    this._nativePtr, value.ToZPose()), "SetFrustumHeadPose");
            }
        }

        /// <summary>
        /// Computes the default head pose for most optimal 
        /// viewing perspective.
        /// </summary>
        public Pose DefaultHeadPose
        {
            get
            {
                Matrix4x4 displayToTrackerMatrix =
                    this._viewport.GetCoordinateSpaceTransform(
                        ZCoordinateSpace.Display,
                        ZCoordinateSpace.Tracker);

                // Create the pose in display space.
                Pose pose = new Pose(
                    Vector3.back * this.CameraOffset.magnitude, 
                    Quaternion.identity);

                // Transform the pose to tracker space before
                // it is returned.
                return pose.GetTransformedBy(displayToTrackerMatrix);
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the view matrix for the specified eye.
        /// </summary>
        /// 
        /// <remarks>
        /// The view matrix is right-handed because Unity cameras expect
        /// view matrices to be right-handed.
        /// </remarks>
        /// 
        /// <param name="eye">
        /// The eye (left, right, or center) to retrieve the view matrix for.
        /// </param>
        /// 
        /// <returns>
        /// The view matrix for the specified eye.
        /// </returns>
        public Matrix4x4 GetViewMatrix(ZEye eye)
        {
            ZMatrix4 viewMatrix;
            ZPlugin.LogOnError(ZPlugin.GetFrustumViewMatrix(
                this._nativePtr, eye, out viewMatrix), "GetFrustumViewMatrix");

            return viewMatrix.ToMatrix4x4(false);
        }

        /// <summary>
        /// Gets a scaled view matrix for the specified eye.
        /// </summary>
        /// 
        /// <remarks>
        /// The view matrix is right-handed because Unity cameras expect
        /// view matrices to be right-handed.
        /// </remarks>
        /// 
        /// <param name="eye">
        /// The eye (left, right, or center) to retrieve the view matrix for.
        /// </param>
        /// <param name="scale">
        /// The scale to apply to the resultant view matrix.
        /// </param>
        /// 
        /// <returns>
        /// The scaled view matrix for the specified eye.
        /// </returns>
        public Matrix4x4 GetViewMatrix(ZEye eye, Vector3 scale)
        {
            Matrix4x4 viewMatrix =
                Matrix4x4.Scale(scale) * this.GetViewMatrix(eye);

            return viewMatrix.ToPoseMatrix();
        }

        /// <summary>
        /// Gets the projection matrix for the specified eye.
        /// </summary>
        /// 
        /// <remarks>
        /// The projection matrix is right-handed because Unity cameras 
        /// expect projection matrices to be right-handed.
        /// </remarks>
        /// 
        /// <param name="eye">
        /// The eye (left, right, or center) to retrieve the projection 
        /// matrix for.
        /// </param>
        /// 
        /// <returns>
        /// The projection matrix for the specified eye.
        /// </returns>
        public Matrix4x4 GetProjectionMatrix(ZEye eye)
        {
            ZMatrix4 projectionMatrix;
            ZPlugin.LogOnError(ZPlugin.GetFrustumProjectionMatrix(
                this._nativePtr, eye, out projectionMatrix),
                "GetFrustumProjectionMatrix");

            return projectionMatrix.ToMatrix4x4(false);
        }

        /// <summary>
        /// Gets the frustum planes (left, right, top, bottom, near, and far)
        /// for the specified eye.
        /// </summary>
        /// 
        /// <param name="eye">
        /// The eye (left, right, or center) to retrieve the frustum 
        /// planes for.
        /// </param>
        /// 
        /// <returns>
        /// The frustum planes for the specified eye.
        /// </returns>
        public FrustumPlanes GetPlanes(ZEye eye)
        {
            ZFrustumBounds bounds;
            ZPlugin.LogOnError(
                ZPlugin.GetFrustumBounds(this._nativePtr, eye, out bounds),
                "GetFrustumBounds");

            return bounds.ToFrustumPlanes();
        }

        /// <summary>
        /// Get the position of the specified eye in the specified 
        /// coordinate space.
        /// </summary>
        /// 
        /// <param name="eye">
        /// The eye (left, right, or center) to retrieve the position for.
        /// </param>
        /// <param name="coordinateSpace">
        /// The coordinate space (Tracker, Display, Viewport, or Camera)
        /// to retrieve the eye position in.
        /// </param>
        /// 
        /// <returns>
        /// The position of the specified eye in the specified 
        /// coordinate space.
        /// </returns>
        public Vector3 GetEyePosition(
            ZEye eye, ZCoordinateSpace coordinateSpace)
        {
            ZVector3 eyePosition;
            ZPlugin.LogOnError(ZPlugin.GetFrustumEyePosition(
                this._nativePtr, eye, coordinateSpace, out eyePosition),
                "GetFrustumEyePosition");

            return eyePosition.ToVector3();
        }

        /// <summary>
        /// Gets a float value for the specified frustum attribute.
        /// </summary>
        /// 
        /// <param name="attribute">
        /// The frustum attribute to retrieve the float value for.
        /// </param>
        /// 
        /// <returns>
        /// The float value for the specified frustum attribute.
        /// </returns>
        public float GetAttributeFloat(ZFrustumAttribute attribute)
        {
            float value = 0;
            ZPlugin.LogOnError(ZPlugin.GetFrustumAttributeF32(
                this._nativePtr, attribute, out value),
                "GetFrustumAttributeF32");

            return value;
        }

        /// <summary>
        /// Gets a boolean value for the specified frustum attribute.
        /// </summary>
        /// 
        /// <param name="attribute">
        /// The frustum attribute to retrieve the boolean value for.
        /// </param>
        /// 
        /// <returns>
        /// The boolean value for the specified frustum attribute.
        /// </returns>
        public bool GetAttributeBool(ZFrustumAttribute attribute)
        {
            bool value = false;
            ZPlugin.LogOnError(ZPlugin.GetFrustumAttributeB(
                this._nativePtr, attribute, out value),"GetFrustumAttributeB");

            return value;
        }

        /// <summary>
        /// Sets the float value for the specified frustum attribute.
        /// </summary>
        /// 
        /// <param name="attribute">
        /// The frustum attribute to update.
        /// </param>
        /// <param name="value">
        /// The float value to update the frustum attribute with.
        /// </param>
        public void SetAttribute(ZFrustumAttribute attribute, float value)
        {
            ZPlugin.LogOnError(ZPlugin.SetFrustumAttributeF32(
                this._nativePtr, attribute, value), "SetFrustumAttributeF32");
        }

        /// <summary>
        /// Sets the boolean value for the specified frustum attribute.
        /// </summary>
        /// 
        /// <param name="attribute">
        /// The frustum attribute to update.
        /// </param>
        /// <param name="value">
        /// The boolean value to update the frustum attribute with.
        /// </param>
        public void SetAttribute(ZFrustumAttribute attribute, bool value)
        {
            ZPlugin.LogOnError(ZPlugin.SetFrustumAttributeB(
                this._nativePtr, attribute, value), "SetFrustumAttributeB");
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private ZViewport _viewport = null;
    }
}

////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

using System;

using UnityEditor;
using UnityEngine;

using zSpace.Core.Extensions;
using zSpace.Core.Interop;
using zSpace.Core.Sdk;

namespace zSpace.Core
{
    public sealed partial class ZCamera
    {
        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void DestroyOverlayResources()
        {
            if (this._leftTexture != null)
            {
                Destroy(this._leftTexture);
                this._leftTexture = null;
            }

            if (this._rightTexture != null)
            {
                Destroy(this._rightTexture);
                this._rightTexture = null;
            }

            this._leftTexturePtr = IntPtr.Zero;
            this._rightTexturePtr = IntPtr.Zero;
        }

        private void UpdateOverlay()
        {
            // If this is not the main camera, early out since
            // overlay rendering can only support one camera.
            if (!this.CompareTag("MainCamera"))
            {
                return;
            }

            // If the XR Overlay is enabled, render to it.
            if (ZPlugin.IsXROverlayActive() && ZPlugin.IsXROverlayEnabled())
            {
                if (LarkXR.XRApi.IsConnected() && 
                    LarkXR.XRManager.Instance.RenderManger.RTextureLeft != null &&
                    LarkXR.XRManager.Instance.RenderManger.RTextureRight != null)
                {
                    this._leftTexturePtr = LarkXR.XRManager.Instance.RenderManger.RTextureLeft.GetNativeTexturePtr();
                    this._rightTexturePtr = LarkXR.XRManager.Instance.RenderManger.RTextureRight.GetNativeTexturePtr();
                } else
                {
                    this.RefreshOverlayTextures();

                    this.RenderOverlayTextures();
                }

                // Set the left and right textures for the XR Overlay.
                ZPlugin.SetXROverlayTextures(
                    this._leftTexturePtr, this._rightTexturePtr);

                // Issue plugin event to queue up left and right textures
                // to be copied and rendered by the XR Overlay.
                ZPlugin.IssueEvent(ZPluginEvent.QueueXROverlayFrame);
            }
        }

        private void RefreshOverlayTextures()
        {
            // Refresh the left render texture.
            this.RefreshOverlayTexture(
                ZProvider.WindowSizePixels,
                ref this._leftTexture,
                ref this._leftTexturePtr);

            // Refresh the right render texture.
            this.RefreshOverlayTexture(
                ZProvider.WindowSizePixels,
                ref this._rightTexture,
                ref this._rightTexturePtr);
        }

        private void RefreshOverlayTexture(
            Vector2Int size, 
            ref RenderTexture renderTexture,
            ref IntPtr renderTexturePtr)
        {
            // Check to see if the render texture should be refreshed.
            if (renderTexture == null ||
                renderTexture.width != size.x ||
                renderTexture.height != size.y)
            {
                // If a texture of a different size is already created,
                // destroy it before attempting to create a new texture.
                if (renderTexture != null)
                {
                    DestroyImmediate(renderTexture);
                }

                // Create a new texture.
                renderTexture = new RenderTexture(size.x, size.y, 24);
                renderTexture.format = RenderTextureFormat.ARGB32;
                renderTexture.filterMode = FilterMode.Bilinear;
                renderTexture.Create();

                // Cache the render texture's native pointer.
                renderTexturePtr = renderTexture.GetNativeTexturePtr();
            }
        }

        private void RenderOverlayTextures()
        {
            // Ensure that the target render textures are valid.
            if (this._leftTexture == null || this._rightTexture == null)
            {
                return;
            }

            // Determine whether to swap the eyes.
            bool swapEyes = EditorPrefs.GetBool(EnableEyeSwapMenuItem);

            RenderTexture leftTexture = 
                swapEyes ? this._rightTexture : this._leftTexture;

            RenderTexture rightTexture =
                swapEyes ? this._leftTexture : this._rightTexture;

            // Render the scene for each eye.
            switch (this.StereoRenderMode)
            {
                case RenderMode.SingleCamera:
                {
                    this.Camera.enabled = false;

                    this.Camera.Render(
                        leftTexture,
                        Camera.StereoscopicEye.Left,
                        this.GetPose(ZEye.Left));

                    this.Camera.Render(
                        rightTexture,
                        Camera.StereoscopicEye.Right,
                        this.GetPose(ZEye.Right));
                }
                break;

                case RenderMode.MultiCamera:
                {
                    this._leftCamera.enabled = false;
                    this._leftCamera.Render(leftTexture);

                    this._rightCamera.enabled = false;
                    this._rightCamera.Render(rightTexture);
                }
                break;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private const string EnableEyeSwapMenuItem = "zSpace/Enable Eye Swap";

        private RenderTexture _leftTexture = null;
        private RenderTexture _rightTexture = null;

        private IntPtr _leftTexturePtr = IntPtr.Zero;
        private IntPtr _rightTexturePtr = IntPtr.Zero;
    }
}

#endif // UNITY_EDITOR

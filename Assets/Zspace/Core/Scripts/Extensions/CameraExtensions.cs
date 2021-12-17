////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace zSpace.Core.Extensions
{
    public static class CameraExtensions
    {
        /// <summary>
        /// Makes this camera's settings match the other camera.
        /// </summary>
        /// 
        /// <remarks>
        /// This will copy all camera variables (field of view, clear flags,
        /// culling mask, etc.) from the other camera.
        /// 
        /// Additionally it will copy the other camera's stereo projection
        /// matrix to this camera's mono/stereo projection matrices and set
        /// the stereo target eye based on the specified eye.
        /// </remarks>
        /// 
        /// <param name="other">
        /// The camera to copy settings from.
        /// </param>
        /// <param name="eye">
        /// The stereo target eye to copy the projection matrix from.
        /// </param>
        public static void CopyFrom(
            this Camera c, Camera other, Camera.StereoscopicEye eye)
        {
            c.CopyFrom(other);
            c.projectionMatrix = c.GetStereoProjectionMatrix(eye);

            switch (eye)
            {
                case Camera.StereoscopicEye.Left:
                    c.stereoTargetEye = StereoTargetEyeMask.Left;
                    break;

                case Camera.StereoscopicEye.Right:
                    c.stereoTargetEye = StereoTargetEyeMask.Right;
                    break;
            }
        }

        /// <summary>
        /// Renders the camera to the specified target texture.
        /// </summary>
        /// 
        /// <param name="targetTexture">
        /// The target texture to render to.
        /// </param>
        public static void Render(
            this Camera c, RenderTexture targetTexture)
        {
            RenderTexture originalTargetTexture = c.targetTexture;
            {
                c.targetTexture = targetTexture;
                c.Render();
            }
            c.targetTexture = originalTargetTexture;
        }

        /// <summary>
        /// Renders the camera to the specified target texture.
        /// </summary>
        /// 
        /// <remarks>
        /// The specified target eye will determine which projection matrix
        /// to use when rendering. For example, if the eye is set to 
        /// Camera.StereoscopicEye.Left, the camera will use its left eye
        /// stereo projection matrix.
        /// </remarks>
        /// 
        /// <param name="targetTexture">
        /// The target texture to render to.
        /// </param>
        /// <param name="eye">
        /// The target eye to render the perspective from.
        /// </param>
        public static void Render(
            this Camera c, 
            RenderTexture targetTexture,
            Camera.StereoscopicEye eye)
        {
            Matrix4x4 originalProjectionMatrix = c.projectionMatrix;
            {
                c.projectionMatrix = c.GetStereoProjectionMatrix(eye);
                c.Render(targetTexture);
            }
            c.projectionMatrix = originalProjectionMatrix;
        }

        /// <summary>
        /// Renders the camera to the specified target texture.
        /// </summary>
        /// 
        /// <remarks>
        /// The specified target eye will determine which projection matrix
        /// to use when rendering. For example, if the eye is set to 
        /// Camera.StereoscopicEye.Left, the camera will use its left eye
        /// stereo projection matrix.
        /// 
        /// Additionally, the specified pose corresponds to the desired world 
        /// pose to render the camera perspective from.
        /// </remarks>
        /// 
        /// <param name="targetTexture">
        /// The target texture to render to.
        /// </param>
        /// <param name="eye">
        /// The target eye to render the perspective from.
        /// </param>
        /// <param name="pose">
        /// The world pose to render the perspective from.
        /// </param>
        public static void Render(
            this Camera c,
            RenderTexture targetTexture,
            Camera.StereoscopicEye eye,
            Pose pose)
        {
            Pose originalPose = c.transform.ToPose();
            {
                c.transform.SetPose(pose);
                c.Render(targetTexture, eye);
            }
            c.transform.SetPose(originalPose);
        }
    }
}

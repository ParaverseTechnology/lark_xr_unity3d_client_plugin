////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace zSpace.Core.Extensions
{
    public static class TransformExtensions
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Extension Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Applies a uniform scale to the transform's x, y, and z
        /// local scale components.
        /// </summary>
        /// 
        /// <param name="scale">
        /// The uniform scale factor to apply to the transform's local scale.
        /// </param>
        public static void SetUniformScale(this Transform t, float scale)
        {
            t.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// Sets the world position and rotation of this transform based
        /// on the position and rotation of the specified world space pose.
        /// </summary>
        /// 
        /// <param name="pose">
        /// The world space pose to update the transform's world position
        /// and rotation to.
        /// </param>
        /// <param name="resetScale">
        /// Flag specifying whether to reset the transform's scale to (1, 1, 1).
        /// </param>
        public static void SetPose(
            this Transform t, Pose pose, bool resetScale = false)
        {
            t.position = pose.position;
            t.rotation = pose.rotation;

            if (resetScale)
            {
                t.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Sets the local position and rotation of this transform based
        /// on the position and rotation of the specified local space pose.
        /// </summary>
        /// 
        /// <param name="localPose">
        /// The local space pose to update the transform's local position
        /// and rotation to.
        /// </param>
        /// <param name="resetScale">
        /// Flag specifying whether to reset the transform's scale to (1, 1, 1).
        /// </param>
        public static void SetLocalPose(
            this Transform t, Pose localPose, bool resetScale = false)
        {
            t.localPosition = localPose.position;
            t.localRotation = localPose.rotation;

            if (resetScale)
            {
                t.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Converts the transform to a Unity Pose based on its current
        /// world space position and rotation.
        /// </summary>
        /// 
        /// <returns>
        /// A Unity Pose based on the transform's world space 
        /// position and rotation.
        /// </returns>
        public static Pose ToPose(this Transform t)
        {
            return new Pose(t.position, t.rotation);
        }

        /// <summary>
        /// Converts the transform to a Unity Pose based on its current
        /// local space position and rotation.
        /// </summary>
        /// 
        /// <returns>
        /// A Unity Pose based on the transform's local space 
        /// position and rotation.
        /// </returns>
        public static Pose ToLocalPose(this Transform t)
        {
            return new Pose(t.localPosition, t.localRotation);
        }

        /// <summary>
        /// Converts the transform to a Unity Ray based on its current
        /// position and forward vector.
        /// </summary>
        /// 
        /// <returns>
        /// A Unity Ray based on the transform's position and forward vector.
        /// </returns>
        public static Ray ToRay(this Transform t)
        {
            return new Ray(t.position, t.forward);
        }
    }
}


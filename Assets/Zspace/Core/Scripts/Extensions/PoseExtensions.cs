////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Interop;

namespace zSpace.Core.Extensions
{
    public static class PoseExtensions
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Static Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Performs linear interpolation between two Poses.
        /// </summary>
        /// 
        /// <param name="from">
        /// The Pose start point to interpolate from.
        /// </param>
        /// <param name="to">
        /// The Pose end point to interpolate to.
        /// </param>
        /// <param name="t">
        /// Normalized time specified between 0 and 1 (inclusive).
        /// </param>
        /// 
        /// <returns>
        /// The interpolated Pose value.
        /// </returns>
        public static Pose Lerp(Pose from, Pose to, float t)
        {
            Vector3 position = Vector3.Lerp(from.position, to.position, t);
            Quaternion rotation = Quaternion.Lerp(from.rotation, to.rotation, t);

            return new Pose(position, rotation);
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Extension Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Performs linear interpolation between the current
        /// and specified Poses.
        /// </summary>
        /// 
        /// <param name="to">
        /// The Pose end point to interpolate to.
        /// </param>
        /// <param name="t">
        /// Normalized time specified between 0 and 1 (inclusive).
        /// </param>
        /// 
        /// <returns>
        /// The interpolated Pose value.
        /// </returns>
        public static Pose LerpTo(this Pose from, Pose to, float t)
        {
            return Lerp(from, to, t);
        }

        /// <summary>
        /// Returns a new Pose that is the result of transforming
        /// the current pose by the specified transformation matrix.
        /// </summary>
        /// 
        /// <param name="matrix">
        /// The Matrix4x4 to transform the current Pose by.
        /// </param>
        /// 
        /// <returns>
        /// Pose equal to the original Pose transformed by the specified
        /// transformation matrix.
        /// </returns>
        public static Pose GetTransformedBy(this Pose pose, Matrix4x4 matrix)
        {
            return new Pose(
                matrix.MultiplyPoint(pose.position),
                matrix.rotation * pose.rotation);
        }

        /// <summary>
        /// Converts Unity's Pose data structure to the zSpace SDK's 
        /// ZPose data structure.
        /// </summary>
        /// 
        /// <remarks>
        /// The ZMatrix4 belonging to the ZPose is right-handed.
        /// </remarks>
        /// 
        /// <returns>
        /// ZPose initialized based on the current state of the Pose.
        /// </returns>
        public static ZPose ToZPose(this Pose p)
        {
            Matrix4x4 m = p.ToMatrix4x4();

            return new ZPose(m.ToZMatrix4(), 0.0f);
        }

        /// <summary>
        /// Converts the Pose to a Matrix4x4 (left-handed).
        /// </summary>
        ///
        /// <returns>
        /// Matrix4x4 initialized based on the current state of the Pose.
        /// </returns>
        public static Matrix4x4 ToMatrix4x4(this Pose p)
        {
            return Matrix4x4.TRS(p.position, p.rotation, Vector3.one);
        }
    }
}


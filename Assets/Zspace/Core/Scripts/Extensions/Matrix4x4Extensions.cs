////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Interop;

namespace zSpace.Core.Extensions
{
    public static class Matrix4x4Extensions
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Static Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Performs linear interpolation between two Matrix4x4s.
        /// </summary>
        /// 
        /// <param name="from">
        /// The Matrix4x4 start point to interpolate from.
        /// </param>
        /// <param name="to">
        /// The Matrix4x4 end point to interpolate to.
        /// </param>
        /// <param name="t">
        /// Normalized time specified between 0 and 1 (inclusive).
        /// </param>
        /// 
        /// <returns>
        /// The interpolated Matrix4x4 value.
        /// </returns>
        public static Matrix4x4 Lerp(Matrix4x4 from, Matrix4x4 to, float t)
        {
            Vector3 position = 
                Vector3.Lerp(from.GetColumn(3), to.GetColumn(3), t);

            Quaternion rotation = 
                Quaternion.Lerp(from.rotation, to.rotation, t);

            return Matrix4x4.TRS(position, rotation, Vector3.one);
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Extension Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Performs linear interpolation between the current
        /// and specified Matrix4x4s.
        /// </summary>
        /// 
        /// <param name="to">
        /// The Matrix4x4 end point to interpolate to.
        /// </param>
        /// <param name="t">
        /// Normalized time specified between 0 and 1 (inclusive).
        /// </param>
        /// 
        /// <returns>
        /// The interpolated Matrix4x4 value.
        /// </returns>
        public static Matrix4x4 LerpTo(this Matrix4x4 from, Matrix4x4 to, float t)
        {
            return Lerp(from, to, t);
        }

        /// <summary>
        /// Converts Unity's Matrix4x4 data structure to the zSpace SDK's
        /// ZMatrix4 data structure.
        /// </summary>
        /// 
        /// <remarks>
        /// Unity's Matrix4x4 struct is usually left-handed (minus view 
        /// matrices, projection matrices, etc.) and the ZMatrix4 struct is 
        /// right-handed. For convenience, there is a flip handedness 
        /// parameter that defaults to true in order to facilitate seamless 
        /// conversions between Unity's and zSpace's 4x4 matrix data 
        /// structures.
        /// </remarks>
        /// 
        /// <param name="flipHandedness">
        /// Flips the handedness of the resultant ZMatrix4 from left to right
        /// (or right to left) depending on the current handedness of the 
        /// Matrix4x4.
        /// </param>
        /// 
        /// <returns>
        /// ZMatrix4 initialized based on the current state of the Matrix4x4.
        /// </returns>
        public static ZMatrix4 ToZMatrix4(
            this Matrix4x4 m, bool flipHandedness = true)
        {
            if (flipHandedness)
            {
                m = m.FlipHandedness();
            }

            return new ZMatrix4(m);
        }

        /// <summary>
        /// Returns a Unity Matrix4x4 of the opposite handedness (e.g. if 
        /// current handedness is left-handed, then the resultant Matrix4x4 
        /// will be right-handed).
        /// </summary>
        /// 
        /// <returns>
        /// Matrix4x4 of the opposite handedness.
        /// </returns>
        public static Matrix4x4 FlipHandedness(this Matrix4x4 m)
        {
            return FlipHandednessMatrix * m * FlipHandednessMatrix;
        }

        /// <summary>
        /// Returns a Unity Matrix4x4 with its original position and rotation,
        /// but no scale.
        /// </summary>
        /// 
        /// <remarks>
        /// More specifically, in order to remove scale, the scale component is 
        /// set to (1, 1, 1).
        /// </remarks>
        /// 
        /// <returns>
        /// Matrix4x4 with its original position and rotation, but no scale.
        /// </returns>
        public static Matrix4x4 ToPoseMatrix(this Matrix4x4 m)
        {
            return Matrix4x4.TRS(m.GetColumn(3), m.rotation, Vector3.one);
        }

        /// <summary>
        /// Converts the Matrix4x4 to a Unity Pose based on its current
        /// position and rotation.
        /// </summary>
        /// 
        /// <returns>
        /// A Unity Pose based on the Matrix4x4's position and rotation.
        /// </returns>
        public static Pose ToPose(this Matrix4x4 m)
        {
            return new Pose(m.GetColumn(3), m.rotation);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Static Members
        ////////////////////////////////////////////////////////////////////////

        private static readonly Matrix4x4 FlipHandednessMatrix = 
            Matrix4x4.Scale(new Vector4(1.0f, 1.0f, -1.0f));
    }
}

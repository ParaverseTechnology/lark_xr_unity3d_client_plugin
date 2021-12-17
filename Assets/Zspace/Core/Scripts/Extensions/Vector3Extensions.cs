////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Interop;

namespace zSpace.Core.Extensions
{
    public static class Vector3Extensions
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Static Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Computes a point on a quadratic bezier curve defined by the 
        /// specified control points.
        /// </summary>
        /// 
        /// <param name="p0">
        /// First control point defining the quadratic bezier curve.
        /// </param>
        /// <param name="p1">
        /// Second control point defining the quadratic bezier curve.
        /// </param>
        /// <param name="p2">
        /// Third control point defining the quadratic bezier curve.
        /// </param>
        /// <param name="t">
        /// The value between 0 and 1 (inclusive) defining where along the 
        /// bezier curve to compute the point. A value of 0 corresponds to the
        /// beginning of the curve. A value of 1 corresponds to the end of the
        /// curve.
        /// </param>
        /// 
        /// <returns>
        /// The point on the bezier curve.
        /// </returns>
        public static Vector3 ComputePointOnBezierCurve(
            Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            Vector3 point =
                (p0 * Mathf.Pow(1 - t, 2)) +
                (p1 * 2 * (1 - t) * t) +
                (p2 * Mathf.Pow(t, 2));

            return point;
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Extension Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Converts Unity's Vector3 data structure to the zSpace SDK's
        /// ZVector3 data structure.
        /// </summary>
        /// 
        /// <remarks>
        /// Unity's Vector3 struct is usually left-handed and the ZVector3 
        /// struct is right-handed. For convenience, there is a flip 
        /// handedness parameter that defaults to true in order to facilitate 
        /// seamless conversions between Unity's and zSpace's Vector3 data 
        /// structures.
        /// </remarks>
        /// 
        /// <param name="flipHandedness">
        /// Flips the handedness of the resultant ZVector3 from left to right
        /// (or right to left) depending on the current handedness of the 
        /// Vector3.
        /// </param>
        /// 
        /// <returns>
        /// ZVector3 initialized based on the current state of the Vector3.
        /// </returns>
        public static ZVector3 ToZVector3(
            this Vector3 v, bool flipHandedness = true)
        {
            return new ZVector3(v.x, v.y, flipHandedness ? -v.z : v.z);
        }
    }
}

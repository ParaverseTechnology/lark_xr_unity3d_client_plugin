////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace zSpace.Core.Extensions
{
    public static class LineRendererExtensions
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Extension Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Update the positions of the line renderer to comform to 
        /// a quadratic bezier curve based on the specified control points.
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
        public static void SetBezierCurve(
            this LineRenderer l, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            l.SetBezierCurve(0, l.positionCount, p0, p1, p2);
        }

        /// <summary>
        /// Update the positions of the line renderer (defined by the specified 
        /// start index) to comform to a quadratic bezier curve based on the 
        /// specified control points.
        /// </summary>
        /// 
        /// <param name="startIndex">
        /// The index of the first position to update.
        /// </param>
        /// <param name="p0">
        /// First control point defining the quadratic bezier curve.
        /// </param>
        /// <param name="p1">
        /// Second control point defining the quadratic bezier curve.
        /// </param>
        /// <param name="p2">
        /// Third control point defining the quadratic bezier curve.
        /// </param>
        public static void SetBezierCurve(this LineRenderer l,
            int startIndex, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            l.SetBezierCurve(startIndex, l.positionCount - startIndex, p0, p1, p2);
        }

        /// <summary>
        /// Update the positions of the line renderer (defined by the specified 
        /// start index and length) to comform to a quadratic bezier curve 
        /// based on the specified control points.
        /// </summary>
        /// 
        /// <param name="startIndex">
        /// The index of the first position to update.
        /// </param>
        /// <param name="length">
        /// The number of positions to update.
        /// </param>
        /// <param name="p0">
        /// First control point defining the quadratic bezier curve.
        /// </param>
        /// <param name="p1">
        /// Second control point defining the quadratic bezier curve.
        /// </param>
        /// <param name="p2">
        /// Third control point defining the quadratic bezier curve.
        /// </param>
        public static void SetBezierCurve(this LineRenderer l,
            int startIndex, int length, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float t = 0;
            float step = 1 / (float)(length - 1);

            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Vector3 position =
                    Vector3Extensions.ComputePointOnBezierCurve(p0, p1, p2, t);

                l.SetPosition(i, position);

                t += step;
            }
        }
    }
}

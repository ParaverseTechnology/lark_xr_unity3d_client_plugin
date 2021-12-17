////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace zSpace.Core.Extensions
{
    public static class RayExtensions
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Extension Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Converts a Ray to a Pose based on its origin and direction
        /// vectors.
        /// </summary>
        /// 
        /// <returns>
        /// Pose initialized based on the origin and direction of the Ray.
        /// </returns>
        public static Pose ToPose(this Ray ray, Vector3 up)
        {
            return new Pose(
                ray.origin, Quaternion.LookRotation(ray.direction, up));
        }
    }
}

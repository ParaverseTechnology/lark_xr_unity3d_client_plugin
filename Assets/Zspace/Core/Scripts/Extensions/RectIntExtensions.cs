////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace zSpace.Core.Extensions
{
    public static class RectIntExtensions
    {
        /// <summary>
        /// Checks whether the other rectangle overlaps this one.
        /// </summary>
        /// 
        /// <param name="other">
        /// The other rectangle to test overlapping with.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if the other rectangle overlaps this one.
        /// </returns>
        public static bool Overlaps(this RectInt r, RectInt other)
        {
            if (other.xMax > r.xMin && 
                other.xMin < r.xMax && 
                other.yMax > r.yMin)
            {
                return other.yMin < r.yMax;
            }

            return false;
        }
    }
}

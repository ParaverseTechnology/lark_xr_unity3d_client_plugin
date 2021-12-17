////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Interop;

namespace zSpace.Core.Extensions
{
    public static class FrustumPlanesExtensions
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Extension Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Converts Unity's FrustumPlanes data structure to the zSpace 
        /// SDK's ZFrustumBounds data structure.
        /// </summary>
        /// 
        /// <returns>
        /// ZFrustumBounds initialized based on the current state of 
        /// the FrustumPlanes.
        /// </returns>
        public static ZFrustumBounds ToZFrustumBounds(this FrustumPlanes f)
        {
            return new ZFrustumBounds(
                f.left, f.right, f.bottom, f.top, f.zNear, f.zFar);
        }
    }
}


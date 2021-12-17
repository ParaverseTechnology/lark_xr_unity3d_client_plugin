////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;

using UnityEngine;

namespace zSpace.Core.Interop
{
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct ZFrustumBounds
    {
        [FieldOffset(0)]
        public float left;

        [FieldOffset(4)]
        public float right;

        [FieldOffset(8)]
        public float bottom;

        [FieldOffset(12)]
        public float top;

        [FieldOffset(16)]
        public float nearClip;

        [FieldOffset(20)]
        public float farClip;

        public ZFrustumBounds(
            float left,
            float right,
            float bottom,
            float top,
            float nearClip,
            float farClip)
        {
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.top = top;
            this.nearClip = nearClip;
            this.farClip = farClip;
        }

        /// <summary>
        /// Converts the ZFrustumBounds struct to Unity's corresponding
        /// FrustumPlanes struct.
        /// </summary>
        /// 
        /// <returns>
        /// FrustumPlanes initialized based on the current state of the
        /// ZFrustumBounds.
        /// </returns>
        public FrustumPlanes ToFrustumPlanes()
        {
            FrustumPlanes frustumPlanes = new FrustumPlanes();
            frustumPlanes.left = this.left;
            frustumPlanes.right = this.right;
            frustumPlanes.bottom = this.bottom;
            frustumPlanes.top = this.top;
            frustumPlanes.zNear = this.nearClip;
            frustumPlanes.zFar = this.farClip;

            return frustumPlanes;
        }
    }
}

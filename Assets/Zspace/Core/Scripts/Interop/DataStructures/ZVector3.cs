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
    public struct ZVector3
    {
        [FieldOffset(0)]
        public float x;

        [FieldOffset(4)]
        public float y;

        [FieldOffset(8)]
        public float z;

        public ZVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public ZVector3(Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        /// <summary>
        /// Converts the ZVector3 struct to Unity's corresponding
        /// Vector3 struct.
        /// </summary>
        /// 
        /// <param name="flipHandedness">
        /// Flag to specify whether to flip the resultant Vector3's
        /// handedness (e.g. from left to right or vice-versa).
        /// </param>
        /// 
        /// <returns>
        /// Vector3 initialized based on the current state of the
        /// ZVector3.
        /// </returns>
        public Vector3 ToVector3(bool flipHandedness = true)
        {
            return new Vector3(
                this.x, this.y, flipHandedness ? -this.z : this.z);
        }
    }
}

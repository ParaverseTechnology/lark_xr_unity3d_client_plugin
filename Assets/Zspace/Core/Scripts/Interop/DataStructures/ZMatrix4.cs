////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;

using UnityEngine;

using zSpace.Core.Extensions;

namespace zSpace.Core.Interop
{
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct ZMatrix4
    {
        [FieldOffset(0)]
        public float m00;

        [FieldOffset(4)]
        public float m10;

        [FieldOffset(8)]
        public float m20;

        [FieldOffset(12)]
        public float m30;

        [FieldOffset(16)]
        public float m01;

        [FieldOffset(20)]
        public float m11;

        [FieldOffset(24)]
        public float m21;

        [FieldOffset(28)]
        public float m31;

        [FieldOffset(32)]
        public float m02;

        [FieldOffset(36)]
        public float m12;

        [FieldOffset(40)]
        public float m22;

        [FieldOffset(44)]
        public float m32;

        [FieldOffset(48)]
        public float m03;

        [FieldOffset(52)]
        public float m13;

        [FieldOffset(56)]
        public float m23;

        [FieldOffset(60)]
        public float m33;

        public ZMatrix4(Matrix4x4 m)
        {
            this.m00 = m[0, 0];
            this.m01 = m[0, 1];
            this.m02 = m[0, 2];
            this.m03 = m[0, 3];

            this.m10 = m[1, 0];
            this.m11 = m[1, 1];
            this.m12 = m[1, 2];
            this.m13 = m[1, 3];

            this.m20 = m[2, 0];
            this.m21 = m[2, 1];
            this.m22 = m[2, 2];
            this.m23 = m[2, 3];

            this.m30 = m[3, 0];
            this.m31 = m[3, 1];
            this.m32 = m[3, 2];
            this.m33 = m[3, 3];
        }

        /// <summary>
        /// Converts the ZMatrix4 struct to Unity's corresponding
        /// Matrix4x4 struct.
        /// </summary>
        /// 
        /// <param name="flipHandedness">
        /// Flag to specify whether to flip the resultant Matrix4x4's
        /// handedness (e.g. from left to right or vice-versa).
        /// </param>
        /// 
        /// <returns>
        /// Matrix4x4 initialized based on the current state of the
        /// ZMatrix4.
        /// </returns>
        public Matrix4x4 ToMatrix4x4(bool flipHandedness = true)
        {
            Matrix4x4 m = new Matrix4x4()
            {
                m00 = this.m00,
                m01 = this.m01,
                m02 = this.m02,
                m03 = this.m03,

                m10 = this.m10,
                m11 = this.m11,
                m12 = this.m12,
                m13 = this.m13,

                m20 = this.m20,
                m21 = this.m21,
                m22 = this.m22,
                m23 = this.m23,

                m30 = this.m30,
                m31 = this.m31,
                m32 = this.m32,
                m33 = this.m33,
            };

            if (flipHandedness)
            {
                m = m.FlipHandedness();
            }

            return m;
        }
    }
}

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
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct ZPose
    {
        public double timestamp;
        public ZMatrix4 matrix;

        public ZPose(ZMatrix4 matrix, double timestamp)
        {
            this.matrix = matrix;
            this.timestamp = timestamp;
        }

        /// <summary>
        /// Converts the ZPose struct to Unity's corresponding Pose struct.
        /// </summary>
        /// 
        /// <returns>
        /// Pose initialized based on the current state of the ZPose.
        /// </returns>
        public Pose ToPose()
        {
            Matrix4x4 matrix = this.matrix.ToMatrix4x4();

            return matrix.ToPose();
        }
    }
}

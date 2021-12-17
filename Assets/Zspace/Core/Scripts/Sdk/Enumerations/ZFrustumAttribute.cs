////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

namespace zSpace.Core.Sdk
{
    public enum ZFrustumAttribute
    {
        /// <summary>
        /// The physical separation, or inter-pupillary distance, between 
        /// the eyes in meters. An IPD of 0 will effectively disable stereo 
        /// since the eyes are assumed to be at the same location. 
        /// (Default: 0.06)
        /// </summary>
        Ipd = 0,

        /// <summary>
        /// Adjusts the scale of the frustum. Use larger values for scenes 
        /// with large models and smallers values for smaller models. The 
        /// default value of 1.0 denotes that all content will be displayed 
        /// at real-world scale in meters. (Default: 1)
        /// </summary>
        ViewerScale = 1,

        /// <summary>
        /// Uniform scale factor applied to the frustum's incoming head pose. 
        /// (Default: 1)
        /// </summary>
        HeadScale = 3,

        /// <summary>
        /// Near clipping plane for the frustum in meters. (Default: 0.03)
        /// </summary>
        NearClip = 4,

        /// <summary>
        /// Far clipping plane for the frustum in meters. (Default: 1000)
        /// </summary>
        FarClip = 5,

        /// <summary>
        /// Distance between the bridge of the glasses and the bridge of the 
        /// nose in meters. (Default: 0.01)
        /// </summary>
        GlassesOffset = 6,

        /// <summary>
        /// Maximum pixel disparity for crossed images (negative parallax) in 
        /// the coupled zone. The coupled zone refers to the area where our 
        /// eyes can both comfortably converge and focus on an object. 
        /// (Default: -100) 
        /// </summary>
        CCLimit = 7,

        /// <summary>
        /// Maximum pixel disparity for uncrossed images (positive parallax) 
        /// in the coupled zone. (Default: 100)
        /// </summary>
        UCLimit = 8,

        /// <summary>
        /// Maximum pixel disparity for crossed images (negative parallax) in 
        /// the uncoupled zone. (Default: -200)
        /// </summary>
        CULimit = 9,

        /// <summary>
        /// Maximum pixel disparity for uncrossed images (positive parallax) 
        /// in the uncoupled zone. (Default: 250)
        /// </summary>
        UULimit = 10,

        /// <summary>
        /// Maximum depth in meters for negative parallax in the coupled zone. 
        /// (Default: 0.13)
        /// </summary>
        CCDepth = 11,

        /// <summary>
        /// Maximum depth in meters for positive parallax in the coupled zone. 
        /// (Default: -0.30)
        /// </summary>
        UCDepth = 12,

        /// <summary>
        /// Display angle in degrees about the X axis. Is only used when 
        /// PortalMode.Angle is not enabled on the frustum. (Default: 90.0)
        /// </summary>
        DisplayAngleX = 13,

        /// <summary>
        /// Display angle in degrees about the Y axis. Is only used when 
        /// PortalMode.Angle is not enabled on the frustum. (Default: 0.0)
        /// </summary>
        DisplayAngleY = 14,

        /// <summary>
        /// Display angle in degrees about the Z axis. Is only used when 
        /// PortalMode.Angle is not enabled on the frustum. (Default: 0.0)
        /// </summary>
        DisplayAngleZ = 15,
    }
}


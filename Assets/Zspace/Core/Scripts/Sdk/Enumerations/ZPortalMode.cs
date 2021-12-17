////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;

namespace zSpace.Core.Sdk
{
    /// <summary>
    /// Defines options for positioning the scene relative to the physical 
    /// display or relative to the viewport.
    /// </summary>
    [Flags]
    public enum ZPortalMode
    {
        /// <summary>
        /// The scene is positioned relative to the viewport.
        /// </summary>
        None = 0,

        /// <summary>
        /// The scene's orientation is fixed relative to the physical desktop.
        /// </summary>
        Angle = 1,

        /// <summary>
        /// The scene's position is fixed relative to the display center.
        /// </summary>
        Position = 2,

        /// <summary>
        /// All portal modes except "none" are enabled.
        /// </summary>
        All = ~0,
    }
}

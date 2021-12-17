////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;

using zSpace.Core.Interop;

namespace zSpace.Core.Sdk
{
    public class ZDisplayManager : ZNativeResourceCache<ZDisplay>
    {
        public ZDisplayManager(ZContext context)
        {
            this._context = context;
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Refreshes the internal cache of information corresponding to
        /// all active displays.
        /// </summary>
        /// 
        /// <remarks>
        /// This method is expensive performance-wise and should be called
        /// sparingly (if at all).
        /// </remarks>
        public void RefreshDisplays()
        {
            this.ClearCache();

            ZPlugin.LogOnError(
                ZPlugin.RefreshDisplays(this._context.NativePtr),
                "RefreshDisplays");
        }

        /// <summary>
        /// Gets the number of displays that are currently active.
        /// </summary>
        /// 
        /// <returns>
        /// The number of displays that are currently active.
        /// </returns>
        public int GetNumDisplays()
        {
            int numDisplays = 0;
            ZPlugin.LogOnError(
                ZPlugin.GetNumDisplays(this._context.NativePtr, out numDisplays),
                "GetNumDisplays");

            return numDisplays;
        }

        /// <summary>
        /// Gets the number of displays of a specified type that are 
        /// currently active.
        /// </summary>
        /// 
        /// <param name="displayType">
        /// The display type.
        /// </param>
        /// 
        /// <returns>
        /// The number of displays of a specified type that are currently 
        /// active.
        /// </returns>
        public int GetNumDisplays(ZDisplayType displayType)
        {
            int numDisplays = 0;
            ZPlugin.LogOnError(ZPlugin.GetNumDisplaysByType(
                this._context.NativePtr, displayType, out numDisplays),
                "GetNumDisplaysByType");

            return numDisplays;
        }

        /// <summary>
        /// Gets a display based on a specified index.
        /// </summary>
        /// 
        /// <param name="index">
        /// The index to retrieve the display for.
        /// </param>
        /// 
        /// <returns>
        /// The display at the specified index.
        /// </returns>
        public ZDisplay GetDisplay(int index)
        {
            IntPtr displayNativePtr = IntPtr.Zero;
            ZPlugin.LogOnError(ZPlugin.GetDisplayByIndex(
                this._context.NativePtr, index, out displayNativePtr),
                "GetDisplayByIndex");

            return this.GetOrCreateCachedResource(displayNativePtr);
        }

        /// <summary>
        /// Gets a display that contains the specified (x, y) virtual desktop 
        /// position in pixels.
        /// </summary>
        /// 
        /// <param name="x">
        /// The virtual desktop x-position in pixels.
        /// </param>
        /// <param name="y">
        /// The virtual desktop y-position in pixels.
        /// </param>
        /// 
        /// <returns>
        /// The display that contains the specified position.
        /// </returns>
        public ZDisplay GetDisplay(int x, int y)
        {
            IntPtr displayNativePtr = IntPtr.Zero;
            ZPlugin.LogOnError(ZPlugin.GetDisplay(
                this._context.NativePtr, x, y, out displayNativePtr),
                "GetDisplay");

            return this.GetOrCreateCachedResource(displayNativePtr);
        }

        /// <summary>
        /// Gets a display of a specified type at a specified index.
        /// </summary>
        /// 
        /// <param name="displayType">
        /// The type of display.
        /// </param>
        /// <param name="index">
        /// The index of the display.
        /// </param>
        /// 
        /// <returns>
        /// The display of a specified type at a specified index.
        /// </returns>
        public ZDisplay GetDisplay(ZDisplayType displayType, int index = 0)
        {
            IntPtr displayNativePtr = IntPtr.Zero;
            ZPlugin.LogOnError(ZPlugin.GetDisplayByType(
                this._context.NativePtr, displayType, index,
                out displayNativePtr),
                "GetDisplayByType");

            return this.GetOrCreateCachedResource(displayNativePtr);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private ZDisplay GetOrCreateCachedResource(IntPtr displayNativePtr)
        {
            return this.GetOrCreateCachedResource(
                displayNativePtr, d => new ZDisplay(d));
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private ZContext _context = null;
    }
}

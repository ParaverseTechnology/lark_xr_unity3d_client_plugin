////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;

using zSpace.Core.Interop;

namespace zSpace.Core.Sdk
{
    public class ZTargetManager : ZNativeResourceCache<ZTarget>
    {
        public ZTargetManager(ZContext context)
        {
            this._context = context;
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The default head target (zSpace glasses).
        /// </summary>
        public ZTarget HeadTarget => this.GetTarget(ZTargetType.Head);

        /// <summary>
        /// The default stylus target.
        /// </summary>
        public ZTarget StylusTarget => this.GetTarget(ZTargetType.Primary);

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the number of trackable targets of a specified type that 
        /// are currently supported.
        /// </summary>
        /// 
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// 
        /// <returns>
        /// The number of supported trackable targets of a specified type.
        /// </returns>
        public int GetNumTargets(ZTargetType targetType)
        {
            int numTargets = 0;
            ZPlugin.LogOnError(ZPlugin.GetNumTargetsByType(
                this._context.NativePtr, targetType, out numTargets),
                "GetNumTargetsByType");

            return numTargets;
        }

        /// <summary>
        /// Gets a reference to a trackable target of a specified type at a
        /// specified index.
        /// </summary>
        /// 
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="index">
        /// The index to retrieve the target at.
        /// </param>
        /// 
        /// <returns>
        /// A reference to the trackable target if found. Null otherwise.
        /// </returns>
        public ZTarget GetTarget(ZTargetType targetType, int index = 0)
        {
            IntPtr targetNativePtr = IntPtr.Zero;
            ZPlugin.LogOnError(ZPlugin.GetTargetByType(
                this._context.NativePtr, targetType, index,
                out targetNativePtr), "GetTargetByType");

            return this.GetOrCreateCachedResource(targetNativePtr);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private ZTarget GetOrCreateCachedResource(IntPtr targetNativePtr)
        {
            return this.GetOrCreateCachedResource(
                targetNativePtr, t => new ZTarget(t));
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private ZContext _context = null;
    }
}

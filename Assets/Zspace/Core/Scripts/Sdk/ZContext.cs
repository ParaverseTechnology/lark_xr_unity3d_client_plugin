////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;

using UnityEngine;

using zSpace.Core.Interop;

namespace zSpace.Core.Sdk
{
    public class ZContext : ZNativeResource
    {
        /// <summary>
        /// The ZContext constructor.
        /// </summary>
        /// 
        /// <remarks>
        /// Will throw an exception if the zSpace SDK failed to initialize.
        /// </remarks>
        public ZContext()
        {
            ZPlugin.ThrowOnError(ZPlugin.Initialize(out this._nativePtr));

            this.DisplayManager = new ZDisplayManager(this);
            this.TargetManager = new ZTargetManager(this);
            this.MouseEmulator = new ZMouseEmulator(this);
            this.MouseEmulator.Target = this.TargetManager.StylusTarget;
        }

        ~ZContext()
        {
            this.Dispose(false);
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The manager responsible for managing display information
        /// corresponding to all active displays.
        /// </summary>
        public ZDisplayManager DisplayManager { get; } = null;

        /// <summary>
        /// The manager responsible for managing tracking information
        /// corresponding to all active, trackable targets.
        /// </summary>
        /// 
        /// <remarks>
        /// Currently the zSpace glasses and stylus are the only supported
        /// trackable targets.
        /// </remarks>
        public ZTargetManager TargetManager { get; } = null;

        /// <summary>
        /// The mouse emulator which provides support for allowing any
        /// 6-DOF trackable target to emulate the system-level mouse.
        /// </summary>
        public ZMouseEmulator MouseEmulator { get; } = null;

        /// <summary>
        /// The version of zSpace SDK runtime that is currently installed
        /// on the user's machine.
        /// </summary>
        public Version RuntimeVersion
        {
            get
            {
                int major = 0;
                int minor = 0;
                int patch = 0;
                ZPlugin.LogOnError(ZPlugin.GetRuntimeVersion(
                    this._nativePtr, out major, out minor, out patch),
                    "GetRuntimeVersion");

                return new Version(major, minor, patch);
            }
        }

        /// <summary>
        /// Specifies whether tracking is enabled.
        /// </summary>
        /// 
        /// <remarks>
        /// This property acts as a global flag to enable or disable 
        /// updates for all tracking related information.
        /// </remarks>
        public bool IsTrackingEnabled
        {
            get
            {
                bool isEnabled = false;
                ZPlugin.LogOnError(
                    ZPlugin.IsTrackingEnabled(this._nativePtr, out isEnabled),
                    "IsTrackingEnabled");

                return isEnabled;
            }
            set
            {
                ZPlugin.LogOnError(
                    ZPlugin.SetTrackingEnabled(this._nativePtr, value),
                    "SetTrackingEnabled");
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Updates the internal state of the context.
        /// </summary>
        /// 
        /// <remarks>
        /// In general, this method should only be called once per frame.
        /// 
        /// The update is responsible for capturing the latest tracking 
        /// information, forwarding the latest head pose information to all 
        /// active frustums, etc.
        /// </remarks>
        public void Update()
        {
            ZPlugin.LogOnError(ZPlugin.Update(this._nativePtr), "Update");
        }

        /// <summary>
        /// Creates an instance of the ZViewport class at the specified
        /// virtual desktop position.
        /// </summary>
        /// 
        /// <param name="position">
        /// The (x, y) virtual desktop position in pixels corresponding
        /// to the viewport's top-left corner.
        /// </param>
        /// 
        /// <returns>
        /// An instance of the ZViewport class.
        /// </returns>
        public ZViewport CreateViewport(Vector2Int position)
        {
            // Create the viewport.
            IntPtr viewportNativePtr;
            ZPlugin.LogOnError(
                ZPlugin.CreateViewport(this._nativePtr, out viewportNativePtr),
                "CreateViewport");

            ZViewport viewport = new ZViewport(viewportNativePtr);
            viewport.Position = position;

            // Update the context to ensure the appropriate display
            // angle has been passed to the viewport's frustum.
            this.Update();

            // Initialize the frustum.
            ZFrustum frustum = viewport.Frustum;
            frustum.HeadPose = frustum.DefaultHeadPose;

            return viewport;
        }

        ////////////////////////////////////////////////////////////////////////
        // Protected Methods
        ////////////////////////////////////////////////////////////////////////

        protected override void Dispose(bool disposing)
        {
            if (this._isDisposed)
            {
                return;
            }

            this._isDisposed = true;

            // Free managed objects.
            if (disposing)
            {
                this.DisplayManager.ClearCache();
                this.TargetManager.ClearCache();
            }

            // Free unmanaged objects.
            ZPlugin.LogOnError(ZPlugin.ShutDown(this._nativePtr), "ShutDown");

            // Call to base class implementation.
            base.Dispose(disposing);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private bool _isDisposed = false;
    }
}

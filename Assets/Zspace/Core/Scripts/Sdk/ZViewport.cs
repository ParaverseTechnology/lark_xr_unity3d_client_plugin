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
    public class ZViewport : ZNativeResource
    {
        public ZViewport(IntPtr nativePtr)
            : base(nativePtr)
        {
            // Retrieve and cache the frustum handle.
            IntPtr frustumNativePtr = IntPtr.Zero;
            ZPlugin.LogOnError(
                ZPlugin.GetFrustum(nativePtr, out frustumNativePtr),
                "GetFrustum");

            this.Frustum = new ZFrustum(this, frustumNativePtr);
        }

        ~ZViewport()
        {
            this.Dispose(false);
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The viewport's associated stereo frustum.
        /// </summary>
        public ZFrustum Frustum { get; private set; } = null;

        /// <summary>
        /// The current (x, y) virtual desktop position in pixels of the 
        /// viewport's top-left corner.
        /// </summary>
        public Vector2Int Position
        {
            get
            {
                int x = 0;
                int y = 0;
                ZPlugin.LogOnError(
                    ZPlugin.GetViewportPosition(this._nativePtr, out x, out y),
                    "GetViewportPosition");

                return new Vector2Int(x, y);
            }
            set
            {
                ZPlugin.LogOnError(ZPlugin.SetViewportPosition(
                    this._nativePtr, value.x, value.y), "SetViewportPosition");
            }
        }

        /// <summary>
        /// The current size in pixels of the viewport.
        /// </summary>
        public Vector2Int Size
        {
            get
            {
                int width = 0;
                int height = 0;
                ZPlugin.LogOnError(ZPlugin.GetViewportSize(
                    this._nativePtr, out width, out height), "GetViewportSize");

                return new Vector2Int(width, height);
            }
            set
            {
                ZPlugin.LogOnError(
                    ZPlugin.SetViewportSize(this._nativePtr, value.x, value.y),
                    "SetViewportSize");
            }
        }

        /// <summary>
        /// The current position and size in pixels of the viewport.
        /// </summary>
        public RectInt Rect
        {
            get
            {
                return new RectInt(this.Position, this.Size);
            }
            set
            {
                ZPlugin.LogOnError(ZPlugin.SetViewportPosition(
                    this._nativePtr, value.x, value.y),
                    "SetViewportPosition");

                ZPlugin.LogOnError(ZPlugin.SetViewportSize(
                    this._nativePtr, value.width, value.height),
                    "SetViewportSize");
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the transformation matrix that represents the mapping
        /// between two specified coordinate spaces.
        /// </summary>
        /// 
        /// <param name="from">
        /// The coordinate space to map from.
        /// </param>
        /// <param name="to">
        /// The coordinate space to map to.
        /// </param>
        /// 
        /// <returns>
        /// The coordinate space transformation matrix.
        /// </returns>
        public Matrix4x4 GetCoordinateSpaceTransform(
            ZCoordinateSpace from, ZCoordinateSpace to)
        {
            if (from == to)
            {
                return Matrix4x4.identity;
            }

            ZMatrix4 matrix;
            ZPlugin.LogOnError(ZPlugin.GetCoordinateSpaceTransform(
                this._nativePtr, from, to, out matrix),
                "GetCoordinateSpaceTransform");

            return matrix.ToMatrix4x4();
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
                this.Frustum = null;
            }

            ZPlugin.LogOnError(ZPlugin.DestroyViewport(this._nativePtr),
                "DestroyViewport");

            base.Dispose(disposing);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private bool _isDisposed = false;
    }
}

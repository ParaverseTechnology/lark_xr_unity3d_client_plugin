////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

using UnityEngine;

using zSpace.Core.Extensions;
using zSpace.Core.Interop;

namespace zSpace.Core.Sdk
{
    public class ZDisplay : ZNativeResource
    {
        public ZDisplay(IntPtr nativePtr)
            : base(nativePtr)
        {
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Types
        ////////////////////////////////////////////////////////////////////////

        public enum Profile
        {
            Custom = 0,
            Size15InchAspect16x9 = 1,
            Size24InchAspect16x9 = 2,
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Static Members
        ////////////////////////////////////////////////////////////////////////

        public static readonly Vector3 DefaultEulerAngles =
            new Vector3(90, 0, 0);

        public static readonly Profile ReferenceProfile =
            Profile.Size24InchAspect16x9;

        public static readonly Vector2 MinimumSize =
            new Vector2(0.01f, 0.01f);

        ////////////////////////////////////////////////////////////////////////
        // Public Static Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the display size in meters based on either the specified 
        /// display profile.
        /// </summary>
        /// 
        /// <param name="profile">
        /// The display profile to retrieve the size for.
        /// </param>
        /// 
        /// <returns>
        /// The size in meters.
        /// </returns>
        public static Vector2 GetSize(Profile profile)
        {
            switch (profile)
            {
                // Laptop (z400)
                case Profile.Size15InchAspect16x9:
                    return new Vector2(0.344f, 0.193f);

                // AIO (z200, z300)
                case Profile.Size24InchAspect16x9:
                    return new Vector2(0.521f, 0.293f);

                default:
                    return new Vector2(0.521f, 0.293f);
            }
        }

        /// <summary>
        /// Gets the native display resolution in pixels based on the
        /// specified display profile.
        /// </summary>
        /// 
        /// <param name="profile">
        /// The display profile to retrieve the native resolution for.
        /// </param>
        /// 
        /// <returns>
        /// The native resolution in pixels.
        /// </returns>
        public static Vector2Int GetNativeResolution(Profile profile)
        {
            switch (profile)
            {
                // Laptop (z400)
                case Profile.Size15InchAspect16x9:
                    return new Vector2Int(1920, 1080);

                // AIO (z200, z300)
                case Profile.Size24InchAspect16x9:
                    return new Vector2Int(1920, 1080);

                default:
                    return new Vector2Int(1920, 1080);
            }
        }

        /// <summary>
        /// Gets the display meters per pixel conversion factor based on
        /// the specified display profile.
        /// </summary>
        /// 
        /// <param name="profile">
        /// The display profile to retrieve the meters per pixel conversion 
        /// factor for.
        /// </param>
        /// 
        /// <returns>
        /// The meters per pixel conversion factor.
        /// </returns>
        public static Vector2 GetMetersPerPixel(Profile profile)
        {
            Vector2 size = GetSize(profile);
            Vector2Int nativeResolution = GetNativeResolution(profile);

            return new Vector2(
                size.x / nativeResolution.x,
                size.y / nativeResolution.y);
        }

        /// <summary>
        /// Gets the scale of a display based on its current size
        /// relative to the specified reference size.
        /// </summary>
        /// 
        /// <param name="referenceSize">
        /// The reference display size in meters.
        /// </param>
        /// <param name="currentSize">
        /// The current display size in meters.
        /// </param>
        /// 
        /// <returns>
        /// The scale of the current display relative to the 
        /// specified reference display.
        /// </returns>
        public static Vector2 GetScale(
            Vector2 referenceSize, Vector2 currentSize)
        {
            return new Vector2(
                referenceSize.x / currentSize.x,
                referenceSize.y / currentSize.y);
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The display's system-level number.
        /// </summary>
        public int Number
        {
            get
            {
                int number = 0;
                ZPlugin.LogOnError(
                    ZPlugin.GetDisplayNumber(this._nativePtr, out number),
                    "GetDisplayNumber");

                return number;
            }
        }

        /// <summary>
        /// The display's adapter index.
        /// </summary>
        public int AdapterIndex
        {
            get
            {
                int adapterIndex = 0;
                ZPlugin.LogOnError(ZPlugin.GetDisplayAdapterIndex(
                    this._nativePtr, out adapterIndex),
                    "GetDisplayAdapterIndex");

                return adapterIndex;
            }
        }

        /// <summary>
        /// The display's size in meters.
        /// </summary>
        public Vector2 Size
        {
            get
            {
                float width = 0;
                float height = 0;
                ZPlugin.LogOnError(ZPlugin.GetDisplaySize(
                    this._nativePtr, out width, out height),
                    "GetDisplaySize");

                return new Vector2(width, height);
            }
        }

        /// <summary>
        /// The display's (x, y) virtual desktop position in pixels.
        /// </summary>
        public Vector2Int Position
        {
            get
            {
                int x = 0;
                int y = 0;
                ZPlugin.LogOnError(
                    ZPlugin.GetDisplayPosition(this._nativePtr, out x, out y),
                    "GetDisplayPosition");

                return new Vector2Int(x, y);
            }
        }

        /// <summary>
        /// The display's native resolution in pixels.
        /// </summary>
        public Vector2Int NativeResolution
        {
            get
            {
                int x = 0;
                int y = 0;
                ZPlugin.LogOnError(ZPlugin.GetDisplayNativeResolution(
                    this._nativePtr, out x, out y), "GetDisplayNativeResolution");

                return new Vector2Int(x, y);
            }
        }

        /// <summary>
        /// The display's current rotation angles.
        /// </summary>
        public Vector3 EulerAngles
        {
            get
            {
                float x = 0;
                float y = 0;
                float z = 0;
                ZPlugin.LogOnError(ZPlugin.GetDisplayAngle(
                    this._nativePtr, out x, out y, out z), "GetDisplayAngle");

                return new Vector3(x, y, z);
            }
        }

        /// <summary>
        /// The display's vertical refresh rate in hertz.
        /// </summary>
        public float VerticalRefreshRate
        {
            get
            {
                float refreshRate = 0;
                ZPlugin.LogOnError(ZPlugin.GetDisplayVerticalRefreshRate(
                    this._nativePtr, out refreshRate),
                    "GetDisplayVerticalRefreshRate");

                return refreshRate;
            }
        }

        /// <summary>
        /// The (x, y) meters per pixel conversion factor based on the 
        /// display's size and native resolution.
        /// </summary>
        public Vector2 MetersPerPixel
        {
            get
            {
                Vector2 size = this.Size;
                Vector2Int nativeResolution = this.NativeResolution;

                return new Vector2(
                    size.x / nativeResolution.x,
                    size.y / nativeResolution.y);
            }
        }

        /// <summary>
        /// The (x, y) pixels per meter conversion factor based on the 
        /// display's size and native resolution.
        /// </summary>
        public Vector2 PixelsPerMeter
        {
            get
            {
                Vector2 size = this.Size;
                Vector2Int nativeResolution = this.NativeResolution;

                return new Vector2(
                    nativeResolution.x / size.x,
                    nativeResolution.y / size.y);
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Performs a raycast against the display using a ray generated
        /// from the specified tracker space pose.
        /// </summary>
        /// 
        /// <param name="pose">
        /// The tracker space pose to base the ray on.
        /// </param>
        /// 
        /// <returns>
        /// The result of the raycast.
        /// </returns>
        public ZDisplayIntersectionInfo Raycast(Pose pose)
        {
            ZDisplayIntersectionInfo intersectionInfo;

            ZPlugin.LogOnError(ZPlugin.IntersectDisplay(
                this._nativePtr, pose.ToZPose(), out intersectionInfo),
                "IntersectDisplay");

            return intersectionInfo;
        }

        /// <summary>
        /// Gets the string value of the specified display attribute.
        /// </summary>
        /// 
        /// <param name="attribute">
        /// The attribute to retrieve the string value for.
        /// </param>
        /// 
        /// <returns>
        /// The string value of the specified display attribute.
        /// </returns>
        public string GetAttribute(ZDisplayAttribute attribute)
        {
            // Get the string attribute size.
            int size = 0;
            ZPlugin.LogOnError(ZPlugin.GetDisplayAttributeStrSize(
                this._nativePtr, attribute, out size),
                "GetDisplayAttributeStrSize");

            // Get the string attribute value.
            StringBuilder buffer = new StringBuilder(size);
            ZPlugin.LogOnError(ZPlugin.GetDisplayAttributeStr(
                this._nativePtr, attribute, buffer, size),
                "GetDisplayAttributeStr");

            return buffer.ToString();
        }
    }
}

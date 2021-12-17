////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Text;

using UnityEngine;

using zSpace.Core.Sdk;

namespace zSpace.Core.Interop
{
    public static class ZPlugin
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        public static void ThrowOnError(ZPluginError pluginError)
        {
            if (pluginError != ZPluginError.Ok)
            {
                throw new Exception();
            }
        }

        public static void LogOnError(ZPluginError pluginError, string functionName)
        {
#if ZCORE_LOGGING_ENABLED
            if (pluginError != ZPluginError.Ok)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("ZPlugin.{0} returned ZPluginError: {1}",
                    functionName, pluginError);
#else
                Debug.LogErrorFormat("ZPlugin.{0} returned ZPluginError: {1}\n\n{2}",
                    functionName, pluginError, new System.Diagnostics.StackTrace());
#endif
            }
#endif
        }

        public static void IssueEvent(ZPluginEvent pluginEvent)
        {
            IntPtr renderEventFunc = GetRenderEventFunc();
            if (renderEventFunc != IntPtr.Zero)
            {
                GL.IssuePluginEvent(renderEventFunc, (int)pluginEvent);
            }
            else
            {
                Debug.LogError(
                    "Invalid render event function pointer. " +
                    $"Failed to issue plugin event: {pluginEvent}");
            }
        }

        public static void InitializeLogging()
        {
            SetLogger(Marshal.GetFunctionPointerForDelegate(s_loggerCallback));
        }

        public static void ShutDownLogging()
        {
#if UNITY_EDITOR_OSX
            SetLogger(IntPtr.Zero);
#endif
        }

        ////////////////////////////////////////////////////////////////////////
        // DLL Import Declarations
        ////////////////////////////////////////////////////////////////////////

        [DllImport(
            DllName,
            EntryPoint = "GetRenderEventFunc",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetRenderEventFunc();

        [DllImport(
            DllName,
            EntryPoint = "zcuSetLogger",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern void SetLogger(
            IntPtr logger);

        [DllImport(
            DllName,
            EntryPoint = "zcuGetPluginVersion",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern void GetPluginVersion(
            out int major,
            out int minor,
            out int patch);

        [DllImport(
            DllName,
            EntryPoint = "zcuGetWindowPosition",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern void GetWindowPosition(
            out int x,
            out int y);

        /// <summary>
        /// Loads the zSpace SDK runtime library and detects connected 
        /// peripherals. For the detected peripherals, <see cref="Initialize"/> 
        /// creates instances of all underlying Displays, TrackerDevices, 
        /// and TrackerTargets.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK. 
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuInitialize", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError Initialize(out IntPtr context);

        /// <summary>
        /// Updates the underlying state of the SDK. This includes 
        /// polling and caching tracker target pose information. 
        /// Additionally, any stereo frustums will receive the latest 
        /// pose information from the default head target. Call this 
        /// once per frame.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuUpdate", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError Update(IntPtr context);

        /// <summary>
        /// Frees allocated memory and unloads plugins. Call this 
        /// on application shutdown.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuShutDown", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError ShutDown(IntPtr context);

        /// <summary>
        /// Gets the version of the runtime that has been loaded by 
        /// <see cref="Initialize"/>. The version is in the following 
        /// format: major.minor.patch
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK. 
        /// </param>
        /// <param name="major">
        /// The major component of the revision number.
        /// </param>
        /// <param name="minor">
        /// The minor component of the revision number.
        /// </param>
        /// <param name="patch">
        /// The patch component of the revision number.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetRuntimeVersion", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetRuntimeVersion(
            IntPtr context, 
            out int major, 
            out int minor, 
            out int patch);

        /// <summary>
        /// Sets whether or not tracking for all devices is enabled.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        /// <param name="isEnabled">
        /// True to enable tracking, false to disable it.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuSetTrackingEnabled", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetTrackingEnabled(
            IntPtr context, 
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isEnabled);

        /// <summary>
        /// Checks whether tracking for all devices is enabled.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        /// <param name="isEnabled">
        /// True if enabled, false otherwise.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuIsTrackingEnabled", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsTrackingEnabled(
            IntPtr context, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isEnabled);

        /// <summary>
        /// Refreshes all of the underlying display information.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        /// 
        /// <remarks>
        /// This invalidates any cached display data that has been returned 
        /// in prior queries. You must query for these display handles again.
        /// </remarks>
        [DllImport(
            DllName, 
            EntryPoint = "zcuRefreshDisplays", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError RefreshDisplays(IntPtr context);

        /// <summary>
        /// Gets the number of connected displays.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        /// <param name="numDisplays">
        /// The number of connected displays.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetNumDisplays", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetNumDisplays(
            IntPtr context, 
            out int numDisplays);

        /// <summary>
        /// Gets the number of connected displays based on a specified type.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        /// <param name="displayType">
        /// The type of display to query.
        /// </param>
        /// <param name="numDisplays">
        /// The number of displays of the specified type.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetNumDisplaysByType", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetNumDisplaysByType(
            IntPtr context, 
            ZDisplayType displayType, 
            out int numDisplays);

        /// <summary>
        /// Gets the display handle based on the specified 
        /// (<paramref name="x"/>, <paramref name="y"/>) pixel 
        /// coordinates on the virtual desktop.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK. 
        /// </param>
        /// <param name="x">
        /// The x pixel coordinate on the virtual desktop. 
        /// </param>
        /// <param name="y">
        /// The y pixel coordinate on the virtual desktop.
        /// </param>
        /// <param name="displayHandle">
        /// The handle for the display at the specified pixel location. 
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplay", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplay(
            IntPtr context, 
            int x, 
            int y, 
            out IntPtr displayHandle);

        /// <summary>
        /// Gets the display handle at a specified index.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        /// <param name="index">
        /// The index of the display to query.
        /// </param>
        /// <param name="displayHandle">
        /// The handle for the display at the specified index.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayByIndex", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayByIndex(
            IntPtr context, 
            int index, 
            out IntPtr displayHandle);

        /// <summary>
        /// Gets the display handle for a specified type. Note 
        /// that in this case, the index is per type. Thus if 
        /// you have only one device of a given type, the index is 0.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        /// <param name="displayType">
        /// The display type to query.
        /// </param>
        /// <param name="index">
        /// The index for the specified device of this type.
        /// </param>
        /// <param name="displayHandle">
        /// The handle for the display of the specified type.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayByType", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayByType(
            IntPtr context, 
            ZDisplayType displayType, 
            int index, 
            out IntPtr displayHandle);

        /// <summary>
        /// Gets the display's type.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="displayType">
        /// The display's type.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayType", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayType(
            IntPtr displayHandle, 
            out ZDisplayType displayType);

        /// <summary>
        /// Gets the display's number. The display number refers 
        /// to the number shown for the display when you set screen 
        /// resolution in the Windows Control Panel.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display. 
        /// </param>
        /// <param name="displayNumber">
        /// The display's number.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayNumber", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayNumber(
            IntPtr displayHandle, 
            out int displayNumber);

        /// <summary>
        /// Gets the index of the GPU that is connected to the 
        /// specified display.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="adapterIndex">
        /// The index of the display's GPU. 
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayAdapterIndex", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayAdapterIndex(
            IntPtr displayHandle, 
            out int adapterIndex);

        /// <summary>
        /// Gets the index of the monitor attached to the display's GPU.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="monitorIndex">
        /// The index for the attached monitor.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayMonitorIndex", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayMonitorIndex(
            IntPtr displayHandle, 
            out int monitorIndex);

        /// <summary>
        /// Gets the string value of the specified attribute for 
        /// the display. See <see cref="ZDisplayAttribute"/> for a 
        /// list of the available attributes.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display. 
        /// </param>
        /// <param name="attribute">
        /// The attribute to query. 
        /// </param>
        /// <param name="buffer">
        /// The user allocated character buffer to hold the attribute's 
        /// string value.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the user allocated buffer.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayAttributeStr", 
            CallingConvention = CallingConvention.StdCall, 
            CharSet = CharSet.Ansi)]
        internal static extern ZPluginError GetDisplayAttributeStr(
            IntPtr displayHandle, 
            ZDisplayAttribute attribute, 
            [param: MarshalAs(UnmanagedType.LPStr), Out()]
            StringBuilder buffer, 
            int bufferSize);

        /// <summary>
        /// Gets the size of the specified attribute's value in bytes.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="attribute">
        /// The attribute to query.
        /// </param>
        /// <param name="size">
        /// The size of the attribute's value.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayAttributeStrSize", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayAttributeStrSize(
            IntPtr displayHandle, 
            ZDisplayAttribute attribute, 
            out int size);

        /// <summary>
        /// Gets the display's size in meters.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="width">
        /// The display's width in meters.
        /// </param>
        /// <param name="height">
        /// The display's height in meters.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplaySize", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplaySize(
            IntPtr displayHandle, 
            out float width, 
            out float height);

        /// <summary>
        /// Gets the (x, y) pixel location of the specified 
        /// display on the virtual desktop (top-left corner).
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="x">
        /// The x pixel location.
        /// </param>
        /// <param name="y">
        /// The y pixel location.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayPosition", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayPosition(
            IntPtr displayHandle, 
            out int x, 
            out int y);

        /// <summary>
        /// Gets the display's preferred native resolution in pixels.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="x">
        /// The width in pixels.
        /// </param>
        /// <param name="y">
        /// The height in pixels.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayNativeResolution", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayNativeResolution(
            IntPtr displayHandle, 
            out int x, 
            out int y);

        /// <summary>
        /// Gets the display's angles about each axis in degrees.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="x">
        /// The angle of the display about the x axis.
        /// </param>
        /// <param name="y">
        /// The angle of the display about the y axis.
        /// </param>
        /// <param name="z">
        /// The angle of the display about the z axis.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayAngle", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayAngle(
            IntPtr displayHandle, 
            out float x, 
            out float y, 
            out float z);

        /// <summary>
        /// Gets the display's vertical refresh rate.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="refreshRate">
        /// The vertical refresh rate.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetDisplayVerticalRefreshRate", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetDisplayVerticalRefreshRate(
            IntPtr displayHandle, 
            out float refreshRate);

        /// <summary>
        /// Checks if the specified display is connected via 
        /// the USB port. Currently this only applies to zSpace 
        /// displays.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="isHardwarePresent">
        /// True if connected, false otherwise.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuIsDisplayHardwarePresent", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsDisplayHardwarePresent(
            IntPtr displayHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isHardwarePresent);

        /// <summary>
        /// Performs a raycast against the display. The incoming 
        /// pose is assumed to transform the direction of the 
        /// negative Z vector, which is then used for the 
        /// intersection test.
        /// </summary>
        /// 
        /// <param name="displayHandle">
        /// A handle to the display.
        /// </param>
        /// <param name="pose">
        /// A <see cref="ZCTrackerPose"/> in tracker space.
        /// </param>
        /// <param name="intersectionInfo">
        /// Struct containing information about the intersection 
        /// (i.e. hit, screen position, etc.)
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuIntersectDisplay", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IntersectDisplay(
            IntPtr displayHandle, 
            [param: MarshalAs(UnmanagedType.LPStruct)]
            ZPose pose, 
            out ZDisplayIntersectionInfo intersectionInfo);

        /// <summary>
        /// Creates a stereo buffer for left/right frame detection.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        /// <param name="renderer">
        /// The application's graphics API.
        /// </param>
        /// <param name="reserved">
        /// A reserved argument depending on the <paramref name="renderer"/>.
        /// </param>
        /// <param name="bufferHandle">
        /// A handle for the buffer.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuCreateStereoBuffer", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError CreateStereoBuffer(
            IntPtr context, 
            int renderer, 
            IntPtr reserved, 
            out IntPtr bufferHandle);

        /// <summary>
        /// Destroys the stereo buffer.
        /// </summary>
        /// 
        /// <param name="bufferHandle">
        /// A handle to the buffer.
        /// </param>
        /// 
        /// <remarks>
        /// Calling <see cref="ShutDown(IntPtr)"/> will free any stereo 
        /// buffers that have not been explicitly destroyed by calling 
        /// zcDestroyStereoBuffer().
        /// </remarks>
        [DllImport(
            DllName, 
            EntryPoint = "zcuDestroyStereoBuffer", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError DestroyStereoBuffer(
            IntPtr bufferHandle);

        [DllImport(
            DllName, 
            EntryPoint = "zcuBeginStereoBufferPatternDetection", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError BeginStereoBufferPatternDetection(
            IntPtr bufferHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isPatternDetectionEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuEndStereoBufferPatternDetection", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError EndStereoBufferPatternDetection(
            IntPtr bufferHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isPatternDetected);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsStereoBufferPatternDetected", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsStereoBufferPatternDetected(
            IntPtr bufferHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isPatternDetected);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsStereoBufferSyncRequested", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsStereoBufferSyncRequested(
            IntPtr bufferHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isSyncRequested);

        /// <summary>
        /// Creates a stereo viewport.
        /// </summary>
        /// 
        /// <param name="context">
        /// A handle to the internal state of the zSpace SDK.
        /// </param>
        /// <param name="viewportHandle">
        /// A handle to the viewport.
        /// </param>
        /// 
        /// <remarks>
        /// The stereo viewport is abstract and not an actual window 
        /// that is created and registered through the OS. It manages a 
        /// stereo frustum, which is responsible for various stereoscopic 
        /// 3D calculations such as calculating the view and projection 
        /// matrices for each eye.
        /// </remarks>
        [DllImport(
            DllName, 
            EntryPoint = "zcuCreateViewport", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError CreateViewport(
            IntPtr context, 
            out IntPtr viewportHandle);

        /// <summary>
        /// Destroys a stereo viewport.
        /// </summary>
        /// 
        /// <param name="viewportHandle">
        /// A handle to the viewport.
        /// </param>
        /// 
        /// <remarks>
        /// Calling <see cref="ShutDown(IntPtr)"/> will free any 
        /// viewports that have not been explicitly destroyed by 
        /// calling zcDestroyViewport().
        /// </remarks>
        [DllImport(
            DllName, 
            EntryPoint = "zcuDestroyViewport", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError DestroyViewport(
            IntPtr viewportHandle);

        /// <summary>
        /// Sets the viewport's absolute virtual desktop coordinates 
        /// (top-left corner).
        /// </summary>
        /// 
        /// <param name="viewportHandle">
        /// A handle to the viewport.
        /// </param>
        /// <param name="x">
        /// The x coordinate for the upper left corner of the viewport.
        /// </param>
        /// <param name="y">
        /// The y coordinate for the upper left corner of the viewport.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuSetViewportPosition", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetViewportPosition(
            IntPtr viewportHandle, 
            int x, 
            int y);

        /// <summary>
        /// Gets the viewport's absolute virtual desktop coordinates 
        /// (top-left corner).
        /// </summary>
        /// 
        /// <param name="viewportHandle">
        /// A handle to the viewport.
        /// </param>
        /// <param name="x">
        /// The x coordinate for the upper left corner of the viewport.
        /// </param>
        /// <param name="y">
        /// The y coordinate for the upper left corner of the viewport.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetViewportPosition", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetViewportPosition(
            IntPtr viewportHandle, 
            out int x, 
            out int y);

        /// <summary>
        /// Sets the viewport's size in pixels.
        /// </summary>
        /// 
        /// <param name="viewportHandle">
        /// A handle to the viewport.
        /// </param>
        /// <param name="width">
        /// The width of the viewport in pixels.
        /// </param>
        /// <param name="height">
        /// The height of the viewport in pixels.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuSetViewportSize", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetViewportSize(
            IntPtr viewportHandle, 
            int width, 
            int height);

        /// <summary>
        /// Gets the viewport's size in pixels.
        /// </summary>
        /// 
        /// <param name="viewportHandle">
        /// A handle to the viewport.
        /// </param>
        /// <param name="width">
        /// The width of the viewport in pixels.
        /// </param>
        /// <param name="height">
        /// The height of the viewport in pixels.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetViewportSize", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetViewportSize(
            IntPtr viewportHandle, 
            out int width, 
            out int height);

        /// <summary>
        /// Gets the coordinate space transformation from 
        /// space <paramref name="a"/> to <paramref name="b"/>.
        /// </summary>
        /// 
        /// <param name="viewportHandle">
        /// A handle to the viewport.
        /// </param>
        /// <param name="a">
        /// The source coordinate space.
        /// </param>
        /// <param name="b">
        /// The destination coordinate space.
        /// </param>
        /// <param name="transform">
        /// The transformation matrix in order to transform from space a to b.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetCoordinateSpaceTransform", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetCoordinateSpaceTransform(
            IntPtr viewportHandle, 
            ZCoordinateSpace a, 
            ZCoordinateSpace b, 
            out ZMatrix4 transform);

        /// <summary>
        /// Transforms a 4x4 transformation matrix from 
        /// space <paramref name="a"/> to <paramref name="b"/>.
        /// </summary>
        /// 
        /// <param name="viewportHandle">
        /// A handle to the viewport.
        /// </param>
        /// <param name="a">
        /// The source coordinate space.
        /// </param>
        /// <param name="b">
        /// The destination coordinate space.
        /// </param>
        /// <param name="matrix">
        /// The input matrix to be transformed.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuTransformMatrix", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError TransformMatrix(
            IntPtr viewportHandle, 
            ZCoordinateSpace a, 
            ZCoordinateSpace b, 
            ref ZMatrix4 matrix);

        /// <summary>
        /// Gets the frustum owned by a specified viewport.
        /// </summary>
        /// 
        /// <param name="viewportHandle">
        /// A handle to the viewport.
        /// </param>
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetFrustum", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetFrustum(
            IntPtr viewportHandle, 
            out IntPtr frustumHandle);

        /// <summary>
        /// Sets the specified frustum attribute's value.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="attribute">
        /// The attribute to be modified.
        /// </param>
        /// <param name="value">
        /// The desired value to be applied to the attribute.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuSetFrustumAttributeF32", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetFrustumAttributeF32(
            IntPtr frustumHandle, 
            ZFrustumAttribute attribute, 
            float value);

        /// <summary>
        /// Gets the specified frustum attribute's value.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="attribute">
        /// The attribute to be queried.
        /// </param>
        /// <param name="value">
        /// The specified attribute's current value.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetFrustumAttributeF32", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetFrustumAttributeF32(
            IntPtr frustumHandle, 
            ZFrustumAttribute attribute, 
            out float value);

        /// <summary>
        /// Sets the specified frustum attribute's value.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="attribute">
        /// The attribute to be modified.
        /// </param>
        /// <param name="value">
        /// The desired value to be applied to the attribute.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuSetFrustumAttributeB", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetFrustumAttributeB(
            IntPtr frustumHandle, 
            ZFrustumAttribute attribute, 
            [param: MarshalAs(UnmanagedType.Bool)]
            bool value);

        /// <summary>
        /// Gets the specified frustum attribute's value.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="attribute">
        /// The attribute to be queried.
        /// </param>
        /// <param name="value">
        /// The specified attribute's current value.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetFrustumAttributeB", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetFrustumAttributeB(
            IntPtr frustumHandle, 
            ZFrustumAttribute attribute, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool value);

        /// <summary>
        /// Sets the frustum's portal mode. In portal mode, 
        /// the scene is fixed relative to the physical world, 
        /// not the viewport. Refer to <see cref="ZPortalMode"/> 
        /// for details on portal modes.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="portalModeFlags">
        /// A bitmask for the portal mode flags.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuSetFrustumPortalMode", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetFrustumPortalMode(
            IntPtr frustumHandle, 
            ZPortalMode portalModeFlags);

        /// <summary>
        /// Gets the frustum's setting for portal mode. In portal mode, 
        /// the scene is fixed relative to the physical world, not the 
        /// viewport.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="portalModeFlags">
        /// A bitmask representing the current portal mode settings.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetFrustumPortalMode", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetFrustumPortalMode(
            IntPtr frustumHandle, 
            out ZPortalMode portalModeFlags);

        /// <summary>
        /// Sets the frustum's camera offset.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum
        /// </param>
        /// <param name="cameraOffset">
        /// The desired camera offset in meters
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuSetFrustumCameraOffset", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetFrustumCameraOffset(
            IntPtr frustumHandle, 
            [param: MarshalAs(UnmanagedType.LPStruct)]
            ZVector3 cameraOffset);

        /// <summary>
        /// Gets the frustum's camera offset.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="cameraOffset">
        /// The current camera offset in meters.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetFrustumCameraOffset", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetFrustumCameraOffset(
            IntPtr frustumHandle, 
            out ZVector3 cameraOffset);

        /// <summary>
        /// Sets the frustum's head pose in tracker space.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="headPose">
        /// The desired head pose.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuSetFrustumHeadPose", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetFrustumHeadPose(
            IntPtr frustumHandle, 
            [param: MarshalAs(UnmanagedType.LPStruct)]
            ZPose headPose);

        /// <summary>
        /// Gets the frustum's current head pose in tracker space.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="headPose">
        /// The current head pose.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetFrustumHeadPose", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetFrustumHeadPose(IntPtr frustumHandle, out ZPose headPose);

        /// <summary>
        /// Gets the frustum's view matrix for a specified eye. 
        /// The view matrix is calculated from the head pose + eye offset.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="eye">
        /// The eye to query.
        /// </param>
        /// <param name="viewMatrix">
        /// The view matrix for the specified eye.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetFrustumViewMatrix", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetFrustumViewMatrix(
            IntPtr frustumHandle, 
            ZEye eye, 
            out ZMatrix4 viewMatrix);

        /// <summary>
        /// Gets the frustum's projection matrix for a specified eye.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="eye">
        /// The eye to query.
        /// </param>
        /// <param name="projectionMatrix">
        /// The projection matrix for the specified eye.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetFrustumProjectionMatrix", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetFrustumProjectionMatrix(
            IntPtr frustumHandle, 
            ZEye eye, 
            out ZMatrix4 projectionMatrix);

        /// <summary>
        /// Gets the frustum's boundaries for the specified eye.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="eye">
        /// The eye to query.
        /// </param>
        /// <param name="bounds">
        /// The boundaries for the specified eye.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetFrustumBounds", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetFrustumBounds(
            IntPtr frustumHandle, 
            ZEye eye, 
            out ZFrustumBounds bounds);

        /// <summary>
        /// Gets the specified eye's position in the specified 
        /// coordinate space.
        /// </summary>
        /// 
        /// <param name="frustumHandle">
        /// A handle to the frustum.
        /// </param>
        /// <param name="eye">
        /// The eye to query.
        /// </param>
        /// <param name="coordinateSpace">
        /// The coordinate space in which to return the eye position.
        /// </param>
        /// <param name="eyePosition">
        /// The eye's position.
        /// </param>
        [DllImport(
            DllName, 
            EntryPoint = "zcuGetFrustumEyePosition", 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetFrustumEyePosition(
            IntPtr frustumHandle, 
            ZEye eye, 
            ZCoordinateSpace coordinateSpace, 
            out ZVector3 eyePosition);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetNumTrackerDevices",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetNumTrackerDevices(
            IntPtr context, 
            out int numDevices);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetTrackerDevice",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetTrackerDevice(
            IntPtr context, 
            int index, 
            out IntPtr deviceHandle);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetTrackerDeviceByName",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern ZPluginError GetTrackerDeviceByName(
            IntPtr context, 
            [param: MarshalAs(UnmanagedType.LPStr)]
            string deviceName, 
            out IntPtr deviceHandle);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetTrackerDeviceEnabled",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetTrackerDeviceEnabled(
            IntPtr deviceHandle, 
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsTrackerDeviceEnabled",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsTrackerDeviceEnabled(
            IntPtr deviceHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isEnabled);

        [DllImport(
            DllName,
            EntryPoint = "zcuGetTrackerDeviceName",
            CallingConvention = CallingConvention.StdCall, 
            CharSet = CharSet.Ansi)]
        internal static extern ZPluginError GetTrackerDeviceName(
            IntPtr deviceHandle, 
            [param: MarshalAs(UnmanagedType.LPStr), Out()]
            StringBuilder buffer, int bufferSize);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetTrackerDeviceNameSize",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetTrackerDeviceNameSize(
            IntPtr deviceHandle, 
            out int size);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetNumTargets",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetNumTargets(
            IntPtr deviceHandle, 
            out int numTargets);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetNumTargetsByType",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetNumTargetsByType(
            IntPtr context, 
            ZTargetType targetType, 
            out int numTargets);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetTarget",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetTarget(
            IntPtr deviceHandle, 
            int index, 
            out IntPtr targetHandle);

        [DllImport(
            DllName,
            EntryPoint = "zcuGetTargetByName",
            CallingConvention = CallingConvention.StdCall, 
            CharSet = CharSet.Ansi)]
        internal static extern ZPluginError GetTargetByName(
            IntPtr deviceHandle, 
            [param: MarshalAs(UnmanagedType.LPStr)]
            string targetName, out IntPtr targetHandle);

        [DllImport(
            DllName,
            EntryPoint = "zcuGetTargetByType",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetTargetByType(
            IntPtr context, 
            ZTargetType targetType, 
            int index, 
            out IntPtr targetHandle);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetTargetName",
            CallingConvention = CallingConvention.StdCall, 
            CharSet = CharSet.Ansi)]
        internal static extern ZPluginError GetTargetName(
            IntPtr targetHandle, 
            [param: MarshalAs(UnmanagedType.LPStr), Out()]
            StringBuilder buffer, int bufferSize);

        [DllImport(
            DllName,
            EntryPoint = "zcuGetTargetNameSize",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetTargetNameSize(
            IntPtr targetHandle, 
            out int size);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetTargetEnabled",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetTargetEnabled(
            IntPtr targetHandle, 
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsTargetEnabled",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsTargetEnabled(
            IntPtr targetHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsTargetVisible",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsTargetVisible(
            IntPtr targetHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetTargetMoveEventThresholds",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetTargetMoveEventThresholds(
            IntPtr targetHandle, 
            float time, 
            float distance, 
            float angle);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetTargetMoveEventThresholds",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetTargetMoveEventThresholds(
            IntPtr targetHandle, 
            out float time, 
            out float distance, 
            out float angle);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetTargetPose",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetTargetPose(
            IntPtr targetHandle, 
            out ZPose pose);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetNumTargetButtons",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetNumTargetButtons(
            IntPtr targetHandle, 
            out int numButtons);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsTargetButtonPressed",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsTargetButtonPressed(
            IntPtr targetHandle, 
            int buttonId, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isButtonPressed);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetTargetLedEnabled",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetTargetLedEnabled(
            IntPtr targetHandle, 
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isLedEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsTargetLedEnabled",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsTargetLedEnabled(
            IntPtr targetHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isLedEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetTargetLedColor",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetTargetLedColor(
            IntPtr targetHandle, 
            float r, 
            float g, 
            float b);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetTargetLedColor",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetTargetLedColor(
            IntPtr targetHandle, 
            out float r, 
            out float g, 
            out float b);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetTargetVibrationEnabled",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetTargetVibrationEnabled(
            IntPtr targetHandle, 
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isVibrationEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsTargetVibrationEnabled",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsTargetVibrationEnabled(
            IntPtr targetHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isVibrationEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsTargetVibrating",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsTargetVibrating(
            IntPtr targetHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isVibrating);

        [DllImport(
            DllName, 
            EntryPoint = "zcuStartTargetVibration",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError StartTargetVibration(
            IntPtr targetHandle, 
            float onPeriod, 
            float offPeriod, 
            int numTimes, 
            float intensity);

        [DllImport(
            DllName, 
            EntryPoint = "zcuStopTargetVibration",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError StopTargetVibration(
            IntPtr targetHandle);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsTargetTapPressed",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsTargetTapPressed(
            IntPtr targetHandle, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isTapPressed);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetTargetTapThreshold",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetTargetTapThreshold(
            IntPtr targetHandle, 
            float seconds);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetTargetTapThreshold",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetTargetTapThreshold(
            IntPtr targetHandle, 
            out float seconds);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetMouseEmulationEnabled",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetMouseEmulationEnabled(
            IntPtr context, 
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuIsMouseEmulationEnabled",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError IsMouseEmulationEnabled(
            IntPtr context, 
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isEnabled);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetMouseEmulationTarget",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetMouseEmulationTarget(
            IntPtr context, 
            IntPtr targetHandle);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetMouseEmulationTarget",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetMouseEmulationTarget(
            IntPtr context,
            out IntPtr targetHandle);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetMouseEmulationMaxDistance",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetMouseEmulationMaxDistance(
            IntPtr context, 
            float maxDistance);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetMouseEmulationMaxDistance",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetMouseEmulationMaxDistance(
            IntPtr context, 
            out float maxDistance);

        [DllImport(
            DllName, 
            EntryPoint = "zcuSetMouseEmulationButtonMapping",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError SetMouseEmulationButtonMapping(
            IntPtr context, 
            int buttonId, 
            ZMouseButton mouseButton);

        [DllImport(
            DllName, 
            EntryPoint = "zcuGetMouseEmulationButtonMapping",
            CallingConvention = CallingConvention.StdCall)]
        internal static extern ZPluginError GetMouseEmulationButtonMapping(
            IntPtr context, 
            int buttonId, 
            out ZMouseButton mouseButton);

        [DllImport(
            DllName,
            EntryPoint = "zcuCreateXROverlay",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool CreateXROverlay();

        [DllImport(
            DllName,
            EntryPoint = "zcuDestroyXROverlay",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool DestroyXROverlay();

        [DllImport(
            DllName,
            EntryPoint = "zcuSetXROverlayParentWindowHandle",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void SetXROverlayParentWindowHandle(
            IntPtr hWnd);

        [DllImport(
            DllName,
            EntryPoint = "zcuSetXROverlayOnDestroyCallback",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void SetXROverlayOnDestroyCallback(
            IntPtr callback);

        [DllImport(
            DllName,
            EntryPoint = "zcuIsXROverlayActive",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool IsXROverlayActive();

        [DllImport(
            DllName,
            EntryPoint = "zcuIsXROverlayEnabled",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool IsXROverlayEnabled();

        [DllImport(
            DllName,
            EntryPoint = "zcuSetXROverlayEnabled",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void SetXROverlayEnabled(
            bool isEnabled);

        [DllImport(
            DllName,
            EntryPoint = "zcuSetXROverlayDimensions",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void SetXROverlayDimensions(
            int x,
            int y,
            int width,
            int height);

        [DllImport(
            DllName,
            EntryPoint = "zcuSetXROverlayPosition",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void SetXROverlayPosition(
            int x, 
            int y);

        [DllImport(
            DllName,
            EntryPoint = "zcuSetXROverlaySize",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void SetXROverlaySize(
            int width, 
            int height);

        [DllImport(
            DllName,
            EntryPoint = "zcuSetXROverlayTextures",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void SetXROverlayTextures(
            IntPtr leftTexturePtr, 
            IntPtr rightTexturePtr);

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        [AOT.MonoPInvokeCallback(typeof(LoggerCallbackDelegate))]
        private static void LoggerCallback(int severity, string message)
        {
            message = $"[{DllName}.dll] {message}";

            switch (severity)
            {
                case 0:
                    Debug.Log(message);
                    break;
                case 1:
                    Debug.LogWarning(message);
                    break;
                case 2:
                    Debug.LogError(message);
                    break;
                default:
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Types
        ////////////////////////////////////////////////////////////////////////

        private delegate void LoggerCallbackDelegate(
            int level,
            string message);

        ////////////////////////////////////////////////////////////////////////
        // Private Constants
        ////////////////////////////////////////////////////////////////////////

        private const string DllName = "zCoreUnity";

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private static LoggerCallbackDelegate s_loggerCallback =
            new LoggerCallbackDelegate(LoggerCallback);
    }
}
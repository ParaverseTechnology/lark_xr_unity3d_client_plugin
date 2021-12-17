////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Sdk;
using zSpace.Core.Utility;

namespace zSpace.Core
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(ScriptPriority)]
    [DisallowMultipleComponent]
    public sealed partial class ZProvider : ZSingleton<ZProvider>
    {
        public const int ScriptPriority = -1000;

        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        [Header("Screen Metrics")]

        [SerializeField]
        [Tooltip(
            "The profile of the reference display the application is being " +
            "designed for.")]
        private ZDisplay.Profile _displayReferenceProfile = 
            ZDisplay.ReferenceProfile;

        [SerializeField]
        [Tooltip("The display reference size in meters.")]
        private Vector2 _displayReferenceSize =
            ZDisplay.GetSize(ZDisplay.ReferenceProfile);

        [SerializeField]
        [Tooltip("The display reference resolution in pixels.")]
        private Vector2Int _displayReferenceResolution =
            ZDisplay.GetNativeResolution(ZDisplay.ReferenceProfile);

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        protected override void Awake()
        {
            base.Awake();

            // Perform an update to initialize state.
            this.Update();
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();

            State.ShutDown();
        }

        private void Update()
        {
            if (IsInitialized)
            {
                RectInt windowRect = ZApplicationWindow.Rect;

                // Update the viewport's position and size based on the 
                // current position and size of the application window.
                Viewport.Rect = windowRect;

                // Get the current display based on the center position
                // of the application window.
                if (!windowRect.Equals(this._previousWindowRect))
                {
                    CurrentDisplay = Context.DisplayManager.GetDisplay(
                        (int)windowRect.center.x, (int)windowRect.center.y);
                }

                // Update the SDK's context.
                Context.Update();

                this._previousWindowRect = windowRect;
            }

            this.UpdateScreenMetrics();
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Static Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Checks whether the zSpace provider has been properly initialized.
        /// </summary>
        /// 
        /// <remarks>
        /// In the scenario that the application is running on a non-zSpace
        /// device, is running on a system that doesn't have the zSpace System
        /// Software installed, etc., IsInitialized will be set to false.
        /// 
        /// Please make sure to check this before attempting to retrieve
        /// the zSpace Context, HeadTarget, StylusTarget, and/or Viewport.
        /// </remarks>
        public static bool IsInitialized => State.Instance.IsInitialized;

        /// <summary>
        /// Gets a reference to the zSpace SDK's primary context.
        /// </summary>
        /// 
        /// <remarks>
        /// The primary context will persist for the lifetime of the
        /// application.
        /// 
        /// If ZProvider.IsInitialized is false, this property will be 
        /// set to null.
        /// </remarks>
        public static ZContext Context => State.Instance.Context;

        /// <summary>
        /// Gets a reference to the default head target (glasses).
        /// </summary>
        /// 
        /// <remarks>
        /// If ZProvider.IsInitialized is false, this property will be 
        /// set to null.
        /// </remarks>
        public static ZTarget HeadTarget => Context?.TargetManager.HeadTarget;

        /// <summary>
        /// Gets a reference to the default stylus target.
        /// </summary>
        /// 
        /// <remarks>
        /// If ZProvider.IsInitialized is false, this property will be 
        /// set to null.
        /// </remarks>
        public static ZTarget StylusTarget => 
            Context?.TargetManager.StylusTarget;

        /// <summary>
        /// Gets a reference to the primary viewport.
        /// </summary>
        /// 
        /// <remarks>
        /// The viewport is responsible for managing information about the 
        /// application window's position and size.
        /// 
        /// Additionally, it manages the application's stereo frustum, which
        /// is responsible for computing the perspectives for the left and
        /// right eyes.
        /// 
        /// If ZProvider.IsInitialized is false, this property will be 
        /// set to null.
        /// </remarks>
        public static ZViewport Viewport => State.Instance.Viewport;

        /// <summary>
        /// Gets the display that the application window is currently on.
        /// </summary>
        /// 
        /// <remarks>
        /// The center of the application window's viewport is used to
        /// determine which display it's currently on.
        /// 
        /// If ZProvider.IsInitialized is false, this property will be 
        /// set to null.
        /// </remarks>
        public static ZDisplay CurrentDisplay { get; private set; } = null;

        /// <summary>
        /// The display reference size in meters.
        /// </summary>
        /// 
        /// <remarks>
        /// This is leveraged in use cases such as computing display scale
        /// factor.
        /// </remarks>
        public static Vector2 DisplayReferenceSize { get; private set; } =
            ZDisplay.GetSize(ZDisplay.ReferenceProfile);

        /// <summary>
        /// The display reference resolution in pixels.
        /// </summary>
        public static Vector2Int DisplayReferenceResolution { get; private set; } =
            ZDisplay.GetNativeResolution(ZDisplay.ReferenceProfile);

        /// <summary>
        /// The current display size in meters.
        /// </summary>
        /// 
        /// <remarks>
        /// If ZProvider.IsInitialized is set to false, the DisplaySize will
        /// be set to the DisplayReferenceSize.
        /// </remarks>
        public static Vector2 DisplaySize { get; private set; } =
            ZDisplay.GetSize(ZDisplay.ReferenceProfile);

        /// <summary>
        /// The current display resolution in pixels.
        /// </summary>
        /// 
        /// <remarks>
        /// If ZProvider.IsInitialized is set to false, the DisplayResolution
        /// will be set to the DisplayReferenceResolution.
        /// </remarks>
        public static Vector2Int DisplayResolution { get; private set; } =
            ZDisplay.GetNativeResolution(ZDisplay.ReferenceProfile);

        /// <summary>
        /// The meters per pixel conversion factor computed from the current
        /// DisplaySize and DisplayResolution.
        /// </summary>
        public static Vector2 DisplayMetersPerPixel { get; set; } =
            ZDisplay.GetMetersPerPixel(ZDisplay.ReferenceProfile);

        /// <summary>
        /// The scale of the current display based on its size relative to the 
        /// DisplayReferenceSize.
        /// </summary>
        public static Vector2 DisplayScale { get; private set; } = Vector2.one;

        /// <summary>
        /// The scale factor of the current display based on the DisplayScale.
        /// </summary>
        /// 
        /// <remarks>
        /// The current and only scale mode that is supported is "fit inside".
        /// </remarks>
        public static float DisplayScaleFactor { get; set; } = 1;

        /// <summary>
        /// The size of the application window in meters.
        /// </summary>
        /// 
        /// <remarks>
        /// If ZProvider.IsInitialized is set to false, the window size (and
        /// aspect ratio) is locked to the display reference size.
        /// </remarks>
        public static Vector2 WindowSize { get; private set; } =
            ZDisplay.GetSize(ZDisplay.ReferenceProfile);

        /// <summary>
        /// The size of the application window in pixels.
        /// </summary>
        /// 
        /// <remarks>
        /// If ZProvider.IsInitialized is set to false, the window size in 
        /// pixels is locked to the display reference resolution.
        /// </remarks>
        public static Vector2Int WindowSizePixels { get; private set; } =
            ZDisplay.GetNativeResolution(ZDisplay.ReferenceProfile);

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void UpdateScreenMetrics()
        {
            // If the display reference profile is not custom, lock
            // the display reference size to the appropriate value.
            if (this._displayReferenceProfile != ZDisplay.Profile.Custom)
            {
                this._displayReferenceSize = ZDisplay.GetSize(
                    this._displayReferenceProfile);

                this._displayReferenceResolution = ZDisplay.GetNativeResolution(
                    this._displayReferenceProfile);
            }

            this._displayReferenceSize = Vector2.Max(
                ZDisplay.MinimumSize, this._displayReferenceSize);

            this._displayReferenceResolution = Vector2Int.Max(
                Vector2Int.one, this._displayReferenceResolution);

            // Update current display information.
            DisplayReferenceSize = this._displayReferenceSize;
            DisplayReferenceResolution = this._displayReferenceResolution;
            DisplaySize = CurrentDisplay?.Size ?? this._displayReferenceSize;
            DisplayResolution = CurrentDisplay?.NativeResolution ??
                this._displayReferenceResolution;

            DisplayMetersPerPixel = new Vector2(
                DisplaySize.x / DisplayResolution.x,
                DisplaySize.y / DisplayResolution.y);

            DisplayScale = ZDisplay.GetScale(DisplayReferenceSize, DisplaySize);
            DisplayScaleFactor = Mathf.Min(DisplayScale.x, DisplayScale.y);
            
            // Update current window information.
            if (IsInitialized)
            {
                WindowSizePixels = ZApplicationWindow.Size;
                WindowSize = WindowSizePixels * DisplayMetersPerPixel;
            }
            else
            {
                WindowSizePixels = DisplayResolution;
                WindowSize = DisplaySize;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private RectInt _previousWindowRect;
    }
}

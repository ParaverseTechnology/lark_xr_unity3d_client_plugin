////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;

using UnityEditor;
using UnityEngine;

using zSpace.Core.Extensions;
using zSpace.Core.Interop;

namespace zSpace.Core
{
    [InitializeOnLoad]
    public static class ZOverlay
    {
        static ZOverlay()
        {
            // Register EditorApplication event handlers.
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Static Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Enables the zSpace XR Overlay.
        /// </summary>
        public static void Enable()
        {
            EditorWindow gameViewWindow = 
                EditorWindowExtensions.GetGameViewWindow();

            // Update the Game View window's title text as an indicator
            // that the zSpace XR Overlay is enabled.
            gameViewWindow.titleContent.text = "Game zSpace";

            // Parent the GameView window to the XR Overlay.
            // NOTE: A side effect of calling GetWindowHandle() will
            //       place the associated Editor Window in focus.
            ZPlugin.SetXROverlayParentWindowHandle(
                gameViewWindow.GetWindowHandle());

            // Register XR Overlay callbacks.
            ZPlugin.SetXROverlayOnDestroyCallback(
                Marshal.GetFunctionPointerForDelegate(s_onDestroyedCallback));

            // Initialize the XR Overlay's position and size.
            RectInt rect = gameViewWindow.GetClientRect();

            ZPlugin.SetXROverlayDimensions(
                rect.x, rect.y, rect.width, rect.height);

            // Create the XR Overlay.
            ZPlugin.CreateXROverlay();
            ZPlugin.SetXROverlayEnabled(true);

            // Force the application to run in the background while the XR
            // Overlay is enabled. This is necessary since upon creation, the 
            // XR Overlay steals focus from the GameView window and causes 
            // the application to pause.
            Application.runInBackground = true;
        }

        /// <summary>
        /// Disables the zSpace XR Overlay.
        /// </summary>
        public static void Disable()
        {
            // Restore the Game View window's title text.
            EditorWindow gameViewWindow =
                EditorWindowExtensions.GetGameViewWindow();

            gameViewWindow.titleContent.text = s_gameViewName;

            // Restore whether the application was original set to run in
            // the background.
            Application.runInBackground = s_runInBackground;

            // Shut down and destroy the XR Overlay.
            ZPlugin.DestroyXROverlay();
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Static Methods
        ////////////////////////////////////////////////////////////////////////

        private static void OnUpdate()
        {
            if (!Application.isPlaying ||
                !EditorPrefs.GetBool(ZMenu.EnableXROverlayMenuItem))
            {
                return;
            }

            // Check if there is a pending recreate request and if
            // so, recreate the XR Overlay.
            if (s_recreateRequest)
            {
                Enable();

                s_recreateRequest = false;
            }

            // If the XR Overlay is active, update it.
            if (ZPlugin.IsXROverlayActive())
            {
                EditorWindow gameViewWindow =
                    EditorWindowExtensions.GetGameViewWindow();

                // Update the XR Overlay's position and size.
                RectInt rect = gameViewWindow.GetClientRect();

                ZPlugin.SetXROverlayDimensions(
                    rect.x, rect.y, rect.width, rect.height);

                // Update whether the XR Overlay is enabled based on whether
                // it is currently overlapped.
                bool isGameViewFocused = gameViewWindow.IsFocused();
                bool isGameViewOverlapped = gameViewWindow.IsOverlappedBy(
                    EditorWindow.focusedWindow);

                if (isGameViewFocused)
                {
                    ZPlugin.SetXROverlayEnabled(true);
                }
                else if (isGameViewOverlapped)
                {
                    ZPlugin.SetXROverlayEnabled(false);
                }
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Check if the XR Overlay is enabled.
            if (!EditorPrefs.GetBool(ZMenu.EnableXROverlayMenuItem))
            {
                return;
            }

            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    CacheSettings();
                    Enable();
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    Disable();
                    break;
            }
        }

        private static void CacheSettings()
        {
            EditorWindow gameViewWindow =
                EditorWindowExtensions.GetGameViewWindow();

            s_gameViewName = gameViewWindow.titleContent.text;

            s_runInBackground = Application.runInBackground;
        }

        private static void OnDestroyed()
        {
            // If the XR Overlay was forcibly destroyed (e.g. parent window
            // was closed, etc.), queue up a request for the XR Overlay to
            // be recreated in the next update.
            s_recreateRequest = true;
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Static Members
        ////////////////////////////////////////////////////////////////////////

        private delegate void CallbackDelegate();

        private static CallbackDelegate s_onDestroyedCallback = 
            new CallbackDelegate(OnDestroyed);

        private static string s_gameViewName = "Game";
        private static bool s_recreateRequest = false;
        private static bool s_runInBackground = false;
    }
}

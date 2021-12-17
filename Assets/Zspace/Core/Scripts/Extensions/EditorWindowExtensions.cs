////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

using System;
using System.Reflection;

using UnityEditor;
using UnityEngine;

using zSpace.Core.Interop;

namespace zSpace.Core.Extensions
{
    public static class EditorWindowExtensions
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Static Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Checks whether the GameView editor window is open.
        /// </summary>
        /// 
        /// <returns>
        /// True if the GameView editor window is open. False otherwise.
        /// </returns>
        public static bool IsGameViewWindowOpen()
        {
            // Lazy initialization of the GameView editor window type.
            if (s_gameViewType == null)
            {
                Assembly assembly = typeof(EditorWindow).Assembly;
                s_gameViewType = assembly.GetType("UnityEditor.GameView");
            }

            EditorWindow[] gameViewWindows = 
                (EditorWindow[])Resources.FindObjectsOfTypeAll(s_gameViewType);

            return (gameViewWindows.Length > 0);
        }

        /// <summary>
        /// Gets the first available instance of the GameView editor window.
        /// </summary>
        /// 
        /// <remarks>
        /// If the GameView editor window is closed, invoking this method 
        /// will open it.
        /// </remarks>
        /// 
        /// <returns>
        /// A reference to the GameView editor window.
        /// </returns>
        public static EditorWindow GetGameViewWindow()
        {
            if (s_gameViewWindow != null)
            {
                return s_gameViewWindow;
            }

            // Lazy initialization of the GameView editor window type.
            if (s_gameViewType == null)
            {
                Assembly assembly = typeof(EditorWindow).Assembly;
                s_gameViewType = assembly.GetType("UnityEditor.GameView");
            }

            // Grab the GameView editor window.
            s_gameViewWindow = EditorWindow.GetWindow(
                s_gameViewType, false, null, false);

            return s_gameViewWindow;
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Extension Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Checks whether the editor window is docked.
        /// </summary>
        ///
        /// <returns>
        /// True if the editor window is docked. False otherwise.
        /// </returns>
        public static bool IsDocked(this EditorWindow window)
        {
            // Lazy initialization of EditorWindow docked method info.
            if (s_dockedMethodInfo == null)
            {
                BindingFlags bindingFlags = 
                    BindingFlags.Public | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Instance | 
                    BindingFlags.Static;

                PropertyInfo dockedPropertyInfo =
                    typeof(EditorWindow).GetProperty("docked", bindingFlags);

                s_dockedMethodInfo = dockedPropertyInfo.GetGetMethod(true);
            }

            // Invocation of EditorWindow docked property.
            return (bool)s_dockedMethodInfo.Invoke(window, null);
        }

        /// <summary>
        /// Checks whether the editor window has keyboard focus.
        /// </summary>
        /// 
        /// <returns>
        /// True if the editor window has keyboard focus. False otherwise.
        /// </returns>
        public static bool IsFocused(this EditorWindow window)
        {
            return (window == EditorWindow.focusedWindow);
        }

        /// <summary>
        /// Checks whether the editor window is overlapped by another
        /// editor window.
        /// </summary>
        /// 
        /// <param name="other">
        /// The other editor window to be used in the overlap check.
        /// </param>
        /// 
        /// <returns>
        /// True if the editor window is overlapped. False otherwise.
        /// </returns>
        public static bool IsOverlappedBy(
            this EditorWindow window, EditorWindow other)
        {
            if (other == null || window == other)
            {
                return false;
            }

            if (!window.IsDocked() || window.IsFocused())
            {
                return false;
            }

            return other.GetRect().Overlaps(window.GetRect());
        }

        /// <summary>
        /// Gets the editor window's associated native window handle.
        /// </summary>
        /// 
        /// <remarks>
        /// A side effect of the current implementation causes the 
        /// associated editor window to gain keyboard focus.
        /// </remarks>
        /// 
        /// <returns>
        /// The editor window's associated native window handle.
        /// </returns>
        public static IntPtr GetWindowHandle(this EditorWindow window)
        {
#if UNITY_EDITOR_WIN
            // Focus the window.
            window.Focus();

            // Return the window handle of the focused window.
            return Win32.GetFocus();
#else
            return IntPtr.Zero;
#endif
        }

        /// <summary>
        /// Gets the editor window's virtual desktop position and size 
        /// in pixels.
        /// </summary>
        /// 
        /// <remarks>
        /// This method accounts for whether the editor window is docked
        /// or undocked and adjusts the position and size accordingly.
        /// </remarks>
        /// 
        /// <returns>
        /// The editor window's position and size in pixels.
        /// </returns>
        public static RectInt GetRect(this EditorWindow window)
        {
            return new RectInt(window.GetPosition(), window.GetSize());
        }

        /// <summary>
        /// Gets the editor window client area's position and size in pixels.
        /// </summary>
        /// 
        /// <remarks>
        /// The position is relative to the top-left virtual desktop position
        /// of its corresponding window.
        /// 
        /// This method accounts for whether the editor window is docked
        /// or undocked and adjusts the position and size accordingly.
        /// </remarks>
        /// 
        /// <returns>
        /// The editor window client area's position and size in pixels.
        /// </returns>
        public static RectInt GetClientRect(this EditorWindow window)
        {
            Vector2Int size = window.GetSize();

            // HACK: Subtract 1 pixel from the height for more precise 
            //       alignment and sizing (features such as the XR Overlay
            //       depend on this).
            return new RectInt(
                window.GetBorderWidth(),
                window.GetTabHeight(),
                size.x,
                size.y - 1); 
        }

        /// <summary>
        /// Gets the editor window's virtual desktop position in pixels.
        /// </summary>
        /// 
        /// <remarks>
        /// This method accounts for whether the editor window is docked
        /// or undocked and adjusts the position accordingly.
        /// </remarks>
        /// 
        /// <returns>
        /// The editor window's virtual desktop position in pixels.
        /// </returns>
        public static Vector2Int GetPosition(this EditorWindow window)
        {
            return new Vector2Int(
                (int)window.position.x + window.GetBorderWidth(),
                (int)window.position.y + window.GetTabHeight());
        }

        /// <summary>
        /// Gets the editor window's size in pixels.
        /// </summary>
        /// 
        /// <remarks>
        /// This method accounts for whether the editor window is docked
        /// or undocked and adjusts the size accordingly.
        /// </remarks>
        /// 
        /// <returns>
        /// The editor window's size in pixels.
        /// </returns>
        public static Vector2Int GetSize(this EditorWindow window)
        {
            return new Vector2Int(
                (int)window.position.width,
                (int)window.position.height - ExcessHeight);
        }

        /// <summary>
        /// Gets the width of the editor window's side border in pixels.
        /// </summary>
        /// 
        /// <remarks>
        /// This method accounts for whether the editor window is docked
        /// or undocked and adjusts the border width accordingly.
        /// </remarks>
        /// 
        /// <returns>
        /// The width of the editor window's side border in pixels.
        /// </returns>
        public static int GetBorderWidth(this EditorWindow window)
        {
            int borderWidth = window.IsDocked() ?
                BorderWidthDocked : BorderWidthUndocked;

            return borderWidth;
        }

        /// <summary>
        /// Gets the height of the editor window's top tab in pixels.
        /// </summary>
        /// 
        /// <remarks>
        /// This method accounts for whether the editor window is docked
        /// or undocked and adjusts the tab height accordingly.
        /// </remarks>
        /// 
        /// <returns>
        /// The height of the editor window's top tab in pixels.
        /// </returns>
        public static int GetTabHeight(this EditorWindow window)
        {
            int tabHeight = window.IsDocked() ? 
                TabHeightDocked : TabHeightUndocked;

            return tabHeight;
        }

        ////////////////////////////////////////////////////////////////
        // Private Constants
        ////////////////////////////////////////////////////////////////

        // NOTE: The following constants are subject to change depending on
        //       the Unity version.
        private const int BorderWidthDocked = 2;
        private const int BorderWidthUndocked = 0;

        private const int TabHeightDocked = 36;
        private const int TabHeightUndocked = 40;

        private const int ExcessHeight = 17;

        ////////////////////////////////////////////////////////////////
        // Private Static Members
        ////////////////////////////////////////////////////////////////

        private static Type s_gameViewType = null;
        private static EditorWindow s_gameViewWindow = null;
        private static MethodInfo s_dockedMethodInfo = null;
    }
}

#endif // UNITY_EDITOR

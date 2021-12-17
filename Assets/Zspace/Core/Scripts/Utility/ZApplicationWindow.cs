////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using zSpace.Core.Extensions;
using zSpace.Core.Interop;

namespace zSpace.Core.Utility
{
    public static class ZApplicationWindow
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Static Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The virtual desktop position and size in pixels of the 
        /// application window.
        /// </summary>
        /// 
        /// <remarks>
        /// When running from the Unity Editor, the position and size 
        /// correspond to the Game View window.
        /// </remarks>
        public static RectInt Rect
        {
            get
            {
#if UNITY_EDITOR
                // Grab the position and size of the GameView window
                // when running from the editor.
                EditorWindow gameViewWindow = 
                    EditorWindowExtensions.GetGameViewWindow();

                return gameViewWindow.GetRect();
#elif UNITY_STANDALONE_WIN
                // Grab the position and size of the standalone player's
                // window when running a standalone build.
                int x = 0;
                int y = 0;
                ZPlugin.GetWindowPosition(out x, out y);

                return new RectInt(x, y, Screen.width, Screen.height);
#else
                return new RectInt(0, 0, Screen.width, Screen.height);
#endif
            }
        }

        /// <summary>
        /// The size in pixels of the application window.
        /// </summary>
        /// 
        /// <remarks>
        /// When running from the Unity Editor, the size corresponds 
        /// to the Game View window.
        /// </remarks>
        public static Vector2Int Size
        {
            get
            {
#if UNITY_EDITOR
                return Rect.size;
#else
                return new Vector2Int(Screen.width, Screen.height);
#endif
            }
        }
    }
}

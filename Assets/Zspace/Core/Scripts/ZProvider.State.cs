////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;

using UnityEngine;

using zSpace.Core.Interop;
using zSpace.Core.Sdk;

namespace zSpace.Core
{
    public sealed partial class ZProvider
    {
        private sealed class State : IDisposable
        {
            private State()
            {
                try
                {
                    // Initialize logging for the plugin.
                    ZPlugin.InitializeLogging();

                    // Initialize the zSpace context.
                    this.Context = new ZContext();

                    // Attempt to retrieve the zSpace display.
                    ZDisplay display = this.Context.DisplayManager.GetDisplay(
                        ZDisplayType.zSpace);

                    // Create and initialize the primary viewport.
                    this.Viewport = this.Context.CreateViewport(
                        (display != null) ? display.Position : Vector2Int.zero);

                    this.IsInitialized = true;
                }
                catch
                {
                    if (Application.isPlaying)
                    {
                        Debug.LogWarning(
                            "Failed to properly initialize the zSpace " +
                            "Provider. Reverting to mock tracker-less, " +
                            "monoscopic 3D.");
                    }
                    
                    this.Dispose();
                }
            }

            ~State()
            {
                this.Dispose();
            }

            ////////////////////////////////////////////////////////////////////
            // Public Static Methods
            ////////////////////////////////////////////////////////////////////

            /// <summary>
            /// A reference to the zSpace Provider's persistent state.
            /// </summary>
            public static State Instance
            {
                get
                {
                    if (s_instance == null)
                    {
                        s_instance = new State();
                    }

                    return s_instance;
                }
            }

            /// <summary>
            /// Shut down and clean up the zSpace Provider's persistent state. 
            /// This includes shutting down the state's SDK context.
            /// </summary>
            public static void ShutDown()
            {
                if (s_instance != null)
                {
                    s_instance.Dispose();
                    s_instance = null;
                }
            }

            ////////////////////////////////////////////////////////////////////
            // Public Properties
            ////////////////////////////////////////////////////////////////////

            /// <summary>
            /// Gets whether the zSpace Provider's persistent state (e.g. SDK 
            /// context) has been properly initialized.
            /// </summary>
            public bool IsInitialized { get; private set; } = false;

            /// <summary>
            /// The zSpace SDK context.
            /// </summary>
            public ZContext Context { get; private set; } = null;

            /// <summary>
            /// The primary viewport for managing the application window's
            /// position and size as well as its corresponding stereo frustum.
            /// </summary>
            public ZViewport Viewport { get; private set; } = null;

            ////////////////////////////////////////////////////////////////////
            // Public Methods
            ////////////////////////////////////////////////////////////////////

            public void Dispose()
            {
                this.Viewport?.Dispose();
                this.Context?.Dispose();

                this.Viewport = null;
                this.Context = null;

                this.IsInitialized = false;

                ZPlugin.ShutDownLogging();
            }

            ////////////////////////////////////////////////////////////////////
            // Private Members
            ////////////////////////////////////////////////////////////////////

            private static State s_instance = null;
        }
    }
}

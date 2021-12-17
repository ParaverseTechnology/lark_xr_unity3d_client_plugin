////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System.Linq;

using UnityEditor;
using UnityEngine;

namespace zSpace.Core
{
    [CustomEditor(typeof(ZCamera))]
    public class ZCameraEditor : Editor
    {
        ////////////////////////////////////////////////////////////////////////
        // Editor Callbacks
        ////////////////////////////////////////////////////////////////////////

        public override void OnInspectorGUI()
        {
            this.InitializeGUIStyles();

            this.CheckIsMainCamera();

            this.CheckIsStereoSdkEnabled();

            DrawDefaultInspector();
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void InitializeGUIStyles()
        {
            if (this._helpBoxStyle == null)
            {
                this._helpBoxStyle = GUI.skin.GetStyle("HelpBox");
                this._helpBoxStyle.richText = true;
            }
        }

        private void CheckIsMainCamera()
        {
            ZCamera camera = this.target as ZCamera;

            // Check whether this is the main camera.
            if (!camera.CompareTag("MainCamera"))
            {
                EditorGUILayout.HelpBox(
                    "<b>EDITOR:</b> This camera will not render to the " +
                    "XR Overlay. To enable XR Overlay rendering, please " +
                    "set this camera's associated tag to <color=#add8e6ff>" +
                    "MainCamera</color>.",
                    MessageType.Info);

                EditorGUILayout.Space();
            }
        }

        private void CheckIsStereoSdkEnabled()
        {
            // Retrieve the list of currently enabled virtual reality SDKs.
            string[] vrSdks = PlayerSettings.GetVirtualRealitySDKs(
                BuildTargetGroup.Standalone);

            // Check if the Desktop Stereo virtual reality SDK is enabled.
            if (!PlayerSettings.virtualRealitySupported ||
                !vrSdks.Contains(DesktopStereoVirtualRealitySdk))
            {
                EditorGUILayout.HelpBox(
                    "<b>STANDALONE PLAYER:</b> This camera will not perform " +
                    "stereoscopic 3D rendering when running the Windows " +
                    "Standalone Player. To enable stereoscopic 3D " +
                    "rendering, please perform the following steps:\n\n" +
                    "<b>1.</b> Go to Edit > Project Settings... > Player > " +
                    "XR Settings\n\n" +
                    "<b>2.</b> Enable Virtual Reality Supported\n\n" +
                    "<b>3.</b> Add <color=#add8e6ff>Stereo Display (non " +
                    "head-mounted)</color> to the list of Virtual Reality " +
                    "SDKs",
                    MessageType.Warning);

                EditorGUILayout.Space();
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private const string DesktopStereoVirtualRealitySdk = "stereo";

        private GUIStyle _helpBoxStyle = null;
    }
}

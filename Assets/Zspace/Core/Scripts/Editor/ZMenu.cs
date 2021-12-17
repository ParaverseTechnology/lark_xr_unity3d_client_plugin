////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

using zSpace.Core.Input;

namespace zSpace.Core
{
    public class ZMenu : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Constants
        ////////////////////////////////////////////////////////////////////////

        public const string CreateAllMenuItem =
            "GameObject/zSpace/All";

        public const string CreateProviderMenuItem =
            "GameObject/zSpace/Provider";

        public const string CreateFrameMenuItem =
            "GameObject/zSpace/Frame";

        public const string CreateCameraRigMenuItem =
            "GameObject/zSpace/Camera Rig";

        public const string CreateStylusMenuItem =
            "GameObject/zSpace/Stylus";

        public const string CreateMouseMenuItem =
            "GameObject/zSpace/Mouse";

        public const string CreateCanvasMenuItem =
            "GameObject/zSpace/Canvas";

        public const string CreateEventSystemMenuItem =
            "GameObject/zSpace/Event System";

        public const string EnableXROverlayMenuItem =
            "zSpace/Enable XR Overlay";

        public const string EnableEyeSwapMenuItem =
            "zSpace/Enable Eye Swap";

        ////////////////////////////////////////////////////////////////////////
        // Menu Item Static Methods
        ////////////////////////////////////////////////////////////////////////

        [MenuItem(CreateAllMenuItem, false, BasePriority)]
        static void CreateAll()
        {
            if (ValidateCreateProvider())
            {
                CreateProvider();
            }

            if (FindObjectOfType<ZFrame>() == null)
            {
                CreateFrame();
            }

            if (FindObjectOfType<ZCameraRig>() == null)
            {
                CreateCameraRig();
            }

            if (ValidateCreateStylus())
            {
                CreateStylus();
            }

            if (ValidateCreateMouse())
            {
                CreateMouse();
            }

            if (ValidateCreateEventSystem())
            {
                CreateEventSystem();
            }
        }

        [MenuItem(CreateProviderMenuItem, false, ProviderPriority)]
        static void CreateProvider()
        {
            CreateGameObject<ZProvider>("ZProvider");
        }

        [MenuItem(CreateProviderMenuItem, true)]
        static bool ValidateCreateProvider()
        {
            return (FindObjectOfType<ZProvider>() == null);
        }

        [MenuItem(CreateFrameMenuItem, false, ProviderPriority + 1)]
        static void CreateFrame()
        {
            CreateGameObjectFromPrefab<ZFrame>("ZFrame");
        }

        [MenuItem(CreateCameraRigMenuItem, false, ProviderPriority + 2)]
        static void CreateCameraRig()
        {
            // Create the camera rig.
            ZCameraRig cameraRig = 
                CreateGameObjectFromPrefab<ZCameraRig>("ZCameraRig");

            cameraRig.Frame = FindComponent<ZFrame>();

            // If a main camera has not been assigned, set the newly
            // created camera as the main camera.
            if (Camera.main == null)
            {
                cameraRig.GetComponentInChildren<ZCamera>().tag = "MainCamera";
            }
        }

        [MenuItem(CreateStylusMenuItem, false, InputPriority)]
        static void CreateStylus()
        {
            ZStylus stylus = CreateGameObjectFromPrefab<ZStylus>("ZStylus");
            stylus.EventCamera = FindComponent<ZCamera>();
        }

        [MenuItem(CreateStylusMenuItem, true)]
        static bool ValidateCreateStylus()
        {
            return (FindObjectOfType<ZStylus>() == null);
        }

        [MenuItem(CreateMouseMenuItem, false, InputPriority + 1)]
        static void CreateMouse()
        {
            ZMouse mouse = CreateGameObjectFromPrefab<ZMouse>("ZMouse");
            mouse.EventCamera = FindComponent<ZCamera>();
        }

        [MenuItem(CreateMouseMenuItem, true)]
        static bool ValidateCreateMouse()
        {
            return (FindObjectOfType<ZMouse>() == null);
        }

        [MenuItem(CreateCanvasMenuItem, false, UIPriority)]
        static void CreateCanvas()
        {
            // Create the canvas.
            Canvas canvas = CreateGameObjectFromPrefab<Canvas>(
                "ZCanvas", true, Selection.activeTransform);

            canvas.worldCamera = FindComponent<Camera>();

            // Create the event system if it doesn't already exist in 
            // the scene.
            if (ValidateCreateEventSystem())
            {
                CreateGameObjectFromPrefab<EventSystem>("ZEventSystem", false);
            }
        }

        [MenuItem(CreateEventSystemMenuItem, false, EventSystemPriority)]
        static void CreateEventSystem()
        {
            CreateGameObjectFromPrefab<EventSystem>("ZEventSystem");
        }

        [MenuItem(CreateEventSystemMenuItem, true)]
        static bool ValidateCreateEventSystem()
        {
            return (FindObjectOfType<EventSystem>() == null);
        }

        [MenuItem(EnableXROverlayMenuItem)]
        static void EnableXROverlay()
        {
#if UNITY_EDITOR_WIN
            bool isEnabled = !EditorPrefs.GetBool(EnableXROverlayMenuItem);
#else
            bool isEnabled = false;
#endif

            EditorPrefs.SetBool(EnableXROverlayMenuItem, isEnabled);
            Menu.SetChecked(EnableXROverlayMenuItem, isEnabled);

#if UNITY_EDITOR_WIN
            // Enable/disable the XR Overlay.
            if (Application.isPlaying)
            {
                if (isEnabled)
                {
                    ZOverlay.Enable();
                }
                else
                {
                    ZOverlay.Disable();
                }
            }
#endif
        }

        [MenuItem(EnableXROverlayMenuItem, true)]
        static bool ValidateEnableXROverlay()
        {
#if UNITY_EDITOR_WIN
            bool isValid = 
                ZProvider.IsInitialized &&
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11;

            bool isEnabled = EditorPrefs.GetBool(EnableXROverlayMenuItem);

            Menu.SetChecked(EnableXROverlayMenuItem, isValid && isEnabled);

            return isValid;
#else
            return false;
#endif
        }

        [MenuItem(EnableEyeSwapMenuItem)]
        static void EnableEyeSwap()
        {
#if UNITY_EDITOR_WIN
            bool isEnabled = !EditorPrefs.GetBool(EnableEyeSwapMenuItem);
#else
            bool isEnabled = false;
#endif

            EditorPrefs.SetBool(EnableEyeSwapMenuItem, isEnabled);
            Menu.SetChecked(EnableEyeSwapMenuItem, isEnabled);
        }

        [MenuItem(EnableEyeSwapMenuItem, true)]
        static bool ValidateEnableEyeSwap()
        {
#if UNITY_EDITOR_WIN
            bool isValid = ZProvider.IsInitialized;

            bool isEnabled = EditorPrefs.GetBool(EnableEyeSwapMenuItem);

            Menu.SetChecked(EnableEyeSwapMenuItem, isValid && isEnabled);

            return isValid;
#else
            return false;
#endif
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Static Methods
        ////////////////////////////////////////////////////////////////////////

        private static T CreateGameObject<T>(
            string name, bool setSelected = true, Transform parent = null)
            where T : Component
        {
            // Create the game object.
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent);
            gameObject.transform.SetAsLastSibling();

            // Register this operation with the Unity Editor's undo stack.
            Undo.RegisterCreatedObjectUndo(gameObject, $"Create {name}");

            // Determine whether to select the newly created Game Object
            // in the Unity Inspector window.
            if (setSelected)
            {
                Selection.activeGameObject = gameObject;
            }

            // Create the specified component.
            T component = gameObject.AddComponent<T>();

            return component;
        }

        private static T CreateGameObjectFromPrefab<T>(
            string name, bool setSelected = true, Transform parent = null)
            where T : Component
        {
            // Attempt to find a reference to the prefab asset.
            GameObject prefab = FindAsset<GameObject>(
                $"{name} t:prefab", PrefabAssetRelativePath);

            if (prefab == null)
            {
                Debug.LogError(
                    $"Failed to create instance of {name}. " +
                    "Prefab not found.");

                return null;
            }

            // Create an instance of the prefab.
            GameObject gameObject = Instantiate(prefab);
            gameObject.transform.SetParent(parent);
            gameObject.transform.SetAsLastSibling();
            gameObject.name = name;

            // Register the operation with the Unity Editor's undo stack.
            Undo.RegisterCreatedObjectUndo(gameObject, $"Create {name}");

            // Determine whether to select the newly created prefab instance
            // in the Unity Inspector window.
            if (setSelected)
            {
                Selection.activeGameObject = gameObject;
            }
            
            return gameObject.GetComponent<T>();
        }

        private static T FindComponent<T>() where T : Component
        {
            T component = null;

            // Check the current selection first.
            if (Selection.activeGameObject != null)
            {
                component = Selection.activeGameObject.GetComponent<T>();
            }
            
            // Otherwise search the entire scene for the first instance of T.
            if (component == null)
            {
                component = FindObjectOfType<T>();
            }

            return component;
        }

        private static T FindAsset<T>(string filter, string relativePath = null) 
            where T : Object
        {
            string[] guids = AssetDatabase.FindAssets(filter);

            for (int i = 0; i < guids.Length; ++i)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);

                if (string.IsNullOrEmpty(relativePath) || 
                    assetPath.Contains(relativePath))
                {
                    return (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                }
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Constants
        ////////////////////////////////////////////////////////////////////////

        private const string PrefabAssetRelativePath = "zSpace/Core/Prefabs";

        private const int BasePriority = 10;
        private const int ProviderPriority = 100;
        private const int InputPriority = 200;
        private const int UIPriority = 300;
        private const int EventSystemPriority = 400;
    }
}

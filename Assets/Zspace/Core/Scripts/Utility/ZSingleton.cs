////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace zSpace.Core.Utility
{
    [DisallowMultipleComponent]
    public class ZSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        protected virtual void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Debug.LogWarning(
                    $"Additional instance of {this.GetType()} found on " +
                    $"GameObject: {this.name}. Destroying this instance to " +
                    "ensure that only one instance is active.",
                    this);

                // Since an instance already exists, destroy this instance.
                if (Application.isEditor)
                {
                    DestroyImmediate(this.gameObject);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }

            if (s_instance == null)
            {
                s_instance = this as T;
            }
        }

        protected virtual void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Static Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a reference to the singleton instance.
        /// </summary>
        /// 
        /// <remarks>
        /// If a valid reference to the singleton instance has not been 
        /// cached, lazily search the Unity scene for a valid instance.
        /// </remarks>
        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<T>();
                }

                return s_instance;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Static Members
        ////////////////////////////////////////////////////////////////////////

        private static T s_instance = null;
    }
}

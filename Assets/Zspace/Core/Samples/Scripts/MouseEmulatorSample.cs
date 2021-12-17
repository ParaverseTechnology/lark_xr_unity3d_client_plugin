////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Sdk;

namespace zSpace.Core.Samples
{
    public class MouseEmulatorSample : MonoBehaviour
    {

        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        public float MaxDistance = 0.1f;
        public bool IsEnabled = true;

        ////////////////////////////////////////////////////////////////////////
        // Monobehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void Start()
        {
            if (ZProvider.IsInitialized)
            {
                this._mouseEmulator = ZProvider.Context.MouseEmulator;
                this.UpdateSettings();
            }
            else
            {
                Debug.LogWarning("ZProvider can not initialize.\n" + 
                    "Mouse emulation is unavailable.");
            }
        }

        private void OnValidate()
        {
            this.UpdateSettings();
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void UpdateSettings()
        {
            if (this._mouseEmulator != null)
            {
                this._mouseEmulator.IsEnabled = this.IsEnabled;
                this._mouseEmulator.MaxDistance = this.MaxDistance;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        ZMouseEmulator _mouseEmulator;
    }
}

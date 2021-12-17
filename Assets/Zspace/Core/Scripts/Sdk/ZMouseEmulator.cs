////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;

using zSpace.Core.Interop;

namespace zSpace.Core.Sdk
{
    public class ZMouseEmulator
    {
        public ZMouseEmulator(ZContext context)
        {
            this._context = context;

            this.IsEnabled = false;
            this.Target = null;

            this.SetButtonMapping(0, ZMouseButton.Left);
            this.SetButtonMapping(1, ZMouseButton.Right);
            this.SetButtonMapping(2, ZMouseButton.Center);
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets/sets whether mouse emulation is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                bool isEnabled = false;
                ZPlugin.LogOnError(ZPlugin.IsMouseEmulationEnabled(
                    this._context.NativePtr, out isEnabled),
                    "IsMouseEmulationEnabled");

                return isEnabled;
            }
            set
            {
                ZPlugin.LogOnError(ZPlugin.SetMouseEmulationEnabled(
                    this._context.NativePtr, value),
                    "SetMouseEmulationEnabled");
            }
        }

        /// <summary>
        /// Gets/sets the 6-DOF trackable target that is responsible for
        /// emulating the mouse.
        /// </summary>
        public ZTarget Target
        {
            get
            {
                return this._target;
            }
            set
            {
                this._target = value;

                if(this._target != null)
                {
                    ZPlugin.LogOnError(ZPlugin.SetMouseEmulationTarget(
                        this._context.NativePtr, this._target.NativePtr),
                        "SetMouseEmulationTarget");
                }
            }
        }

        /// <summary>
        /// Gets/sets the maximum distance from the display's screen in meters 
        /// that the 6-DOF trackable target can be positioned while still being 
        /// able to emulate the mouse.
        /// </summary>
        public float MaxDistance
        {
            get
            {
                float maxDistance = 0;
                ZPlugin.LogOnError(ZPlugin.GetMouseEmulationMaxDistance(
                    this._context.NativePtr, out maxDistance),
                    "GetMouseEmulationMaxDistance");

                return maxDistance;
            }
            set
            {
                ZPlugin.LogOnError(ZPlugin.SetMouseEmulationMaxDistance(
                    this._context.NativePtr, value),
                    "SetMouseEmulationMaxDistance");
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the associated mouse button that is mapped to the specified
        /// button id.
        /// </summary>
        /// 
        /// <param name="buttonId">
        /// The id of the button to retrieve the associated mouse button for.
        /// </param>
        /// 
        /// <returns>
        /// The mouse button mapped to the specified button id.
        /// </returns>
        public ZMouseButton GetButtonMapping(int buttonId)
        {
            ZMouseButton mouseButton = ZMouseButton.Unknown;
            ZPlugin.LogOnError(ZPlugin.GetMouseEmulationButtonMapping(
                this._context.NativePtr, buttonId, out mouseButton),
                "GetMouseEmulationButtonMapping");

            return mouseButton;
        }

        /// <summary>
        /// Maps the specified integer button id to the specified mouse button.
        /// </summary>
        /// 
        /// <param name="buttonId">
        /// The integer button id.
        /// </param>
        /// <param name="mouseButton">
        /// The mouse button to be mapped to.
        /// </param>
        public void SetButtonMapping(int buttonId, ZMouseButton mouseButton)
        {
            ZPlugin.LogOnError(ZPlugin.SetMouseEmulationButtonMapping(
                this._context.NativePtr, buttonId, mouseButton),
                "SetMouseEmulationButtonMapping");
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private ZContext _context = null;
        private ZTarget _target = null;
    }
}

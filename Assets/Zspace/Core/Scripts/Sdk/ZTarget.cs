////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

using UnityEngine;

using zSpace.Core.Interop;

namespace zSpace.Core.Sdk
{
    public class ZTarget : ZNativeResource
    {
        public ZTarget(IntPtr nativePtr)
            : base(nativePtr)
        {
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The name of the target.
        /// </summary>
        public string Name
        {
            get
            {
                // Get the string name size.
                int size = 0;
                ZPlugin.LogOnError(
                    ZPlugin.GetTargetNameSize(this._nativePtr, out size),
                    "GetTargetNameSize");

                // Get the string name value.
                StringBuilder buffer = new StringBuilder(size);
                ZPlugin.LogOnError(
                    ZPlugin.GetTargetName(this._nativePtr, buffer, size),
                    "GetTargetName");

                return buffer.ToString();
            }
        }

        /// <summary>
        /// The currently cached pose in tracker space.
        /// </summary>
        /// 
        /// <remarks>
        /// This pose will only be updated when the target's corresponding
        /// SDK context has been updated.
        /// </remarks>
        public Pose Pose
        {
            get
            {
                ZPose pose = default(ZPose);
                ZPlugin.LogOnError(
                    ZPlugin.GetTargetPose(this._nativePtr, out pose),
                    "GetTargetPose");

                return pose.ToPose();
            }
        }

        /// <summary>
        /// The visibility state of the target.
        /// </summary>
        /// 
        /// <remarks>
        /// The visibility state is updated internally based on whether
        /// the target is currently visible to the tracking cameras.
        /// </remarks>
        public bool IsVisible
        {
            get
            {
                bool isVisible = false;
                ZPlugin.LogOnError(
                    ZPlugin.IsTargetVisible(this._nativePtr, out isVisible),
                    "IsTargetVisible");

                return isVisible;
            }
        }

        /// <summary>
        /// Gets/sets whether the target is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                bool isEnabled = false;
                ZPlugin.LogOnError(
                    ZPlugin.IsTargetEnabled(this._nativePtr, out isEnabled),
                    "IsTargetEnabled");

                return isEnabled;
            }
            set
            {
                ZPlugin.LogOnError(
                    ZPlugin.SetTargetEnabled(this._nativePtr, value),
                    "SetTargetEnabled");
            }
        }

        /// <summary>
        /// Gets/sets whether the target's LED is enabled (if it exists).
        /// </summary>
        /// 
        /// <remarks>
        /// Currently only the stylus target has LED support.
        /// </remarks>
        public bool IsLedEnabled
        {
            get
            {
                bool isEnabled = false;
                ZPlugin.LogOnError(
                    ZPlugin.IsTargetLedEnabled(this._nativePtr, out isEnabled),
                    "IsTargetLedEnabled");

                return isEnabled;
            }
            set
            {
                ZPlugin.LogOnError(
                    ZPlugin.SetTargetLedEnabled(this._nativePtr, value),
                    "SetTargetLedEnabled");
            }
        }

        /// <summary>
        /// Gets/sets the target's LED color (if it exists).
        /// </summary>
        public Color LedColor
        {
            get
            {
                float r = 0;
                float g = 0;
                float b = 0;
                ZPlugin.LogOnError(ZPlugin.GetTargetLedColor(
                    this._nativePtr, out r, out g, out b), "GetTargetLedColor");

                return new Color(r, g, b);
            }
            set
            {
                ZPlugin.LogOnError(ZPlugin.SetTargetLedColor(
                    this._nativePtr, value.r, value.g, value.b),
                    "SetTargetLedColor");
            }
        }

        /// <summary>
        /// Gets/sets whether the target's vibration capabilities are enabled.
        /// </summary>
        public bool IsVibrationEnabled
        {
            get
            {
                bool isEnabled = false;
                ZPlugin.LogOnError(ZPlugin.IsTargetVibrationEnabled(
                    this._nativePtr, out isEnabled),
                    "IsTargetVibrationEnabled");

                return isEnabled;
            }
            set
            {
                ZPlugin.LogOnError(
                    ZPlugin.SetTargetVibrationEnabled(this._nativePtr, value),
                    "SetTargetVibrationEnabled");
            }
        }

        /// <summary>
        /// Checks whether the target is currently vibrating.
        /// </summary>
        public bool IsVibrating
        {
            get
            {
                bool isVibrating = false;
                ZPlugin.LogOnError(
                    ZPlugin.IsTargetVibrating(this._nativePtr, out isVibrating),
                    "IsTargetVibrating");

                return isVibrating;
            }
        }

        /// <summary>
        /// The number of buttons supported by the target.
        /// </summary>
        public int ButtonCount
        {
            get
            {
                int numButtons = 0;
                ZPlugin.LogOnError(ZPlugin.GetNumTargetButtons(
                    this._nativePtr, out numButtons), "GetNumTargetButtons");

                return numButtons;
            }
        }

        /// <summary>
        /// Gets whether the target is currently tapping against the surface 
        /// of the zSpace display device.
        /// </summary>
        public bool IsTapPressed
        {
            get
            {
                bool isTapPressed = false;
                ZPlugin.LogOnError(ZPlugin.IsTargetTapPressed(
                    this._nativePtr, out isTapPressed), "IsTargetTapPressed");

                return isTapPressed;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets whether the specified button is pressed.
        /// </summary>
        /// 
        /// <param name="id">
        /// The integer id of the specified button.
        /// </param>
        /// 
        /// <returns>
        /// True if the specified button is pressed. False otherwise.
        /// </returns>
        public bool IsButtonPressed(int id)
        {
            bool isPressed = false;
            ZPlugin.LogOnError(ZPlugin.IsTargetButtonPressed(
                this._nativePtr, id, out isPressed), "IsTargetButtonPressed");

            return isPressed;
        }

        /// <summary>
        /// Start a vibration based on a pattern specified by
        /// the on period, off period, repeat count, and intensity.
        /// </summary>
        /// 
        /// <param name="onPeriod">
        /// The time in seconds that the vibration will be active in
        /// a single cycle.
        /// </param>
        /// <param name="offPeriod">
        /// The time in seconds that the vibration will be inactive
        /// in a single cycle.
        /// </param>
        /// <param name="numTimes">
        /// The number of times to repeat the vibration cycle.
        /// </param>
        /// <param name="intensity">
        /// The intensity value between 0 and 1 (inclusive) of the vibration.
        /// The 0 value corresponds to no vibration and 1 corresponds to full
        /// vibration.
        /// </param>
        public void StartVibration(
            float onPeriod, float offPeriod, int numTimes, float intensity)
        {
            ZPlugin.LogOnError(ZPlugin.StartTargetVibration(
                this._nativePtr, onPeriod, offPeriod, numTimes, intensity),
                "StartTargetVibration");
        }

        /// <summary>
        /// Stops a currently active vibration.
        /// </summary>
        public void StopVibration()
        {
            ZPlugin.LogOnError(ZPlugin.StopTargetVibration(this._nativePtr),
                "StopTargetVibration");
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Sdk;

namespace zSpace.Core.Utility
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(ZCameraRig.ScriptPriority - 1)]
    public partial class ZDisplayAligner : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        public bool OverrideAngle = true;
        public bool ClampAngle = false;

        [SerializeField]
        [Range(0, 360)]
        private float _angle = 90.0f;
        [Range(0, 360)]
        public float MinAngle = 0.0f;
        [Range(0, 360)]
        public float MaxAngle = 90.0f;

        [Range(0, 1)]
        public float Pivot = 0.5f;

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void Update()
        {
            this.UpdateLocalRotation();
            this.UpdateLocalPosition();
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        public float Angle
        {
            get
            {
                if (this.ClampAngle && this.MinAngle <= this.MaxAngle)
                {
                    return Mathf.Clamp(
                        this._angle, this.MinAngle, this.MaxAngle);
                }

                return this._angle;
            }
            set
            {
                this._angle = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void UpdateLocalRotation()
        {
            Vector3 displayEulerAngles = Vector3.zero;

            if (!this.OverrideAngle)
            {
                displayEulerAngles =
                    ZProvider.CurrentDisplay?.EulerAngles ?? 
                    ZDisplay.DefaultEulerAngles;

                this.Angle = displayEulerAngles.x;
            }

            this.transform.localRotation = Quaternion.Euler(
                ZDisplay.DefaultEulerAngles - new Vector3(this.Angle, 0, 0));
        }

        private void UpdateLocalPosition()
        {
            Vector2 halfSize = ZProvider.WindowSize * 0.5f;

            float localPivotY = Mathf.Lerp(
                -halfSize.y, halfSize.y, this.Pivot);

            this._localPivot = new Vector3(0, localPivotY, 0);

            this.transform.localPosition =
                 this.transform.localRotation * -this._localPivot;
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private Vector3 _localPivot = Vector3.zero;
    }
}

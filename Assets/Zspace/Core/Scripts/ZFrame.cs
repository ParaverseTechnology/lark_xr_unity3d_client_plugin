////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Extensions;
using zSpace.Core.Utility;

namespace zSpace.Core
{
    [ExecuteInEditMode]
    public sealed partial class ZFrame : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The scale of the user's current view. At a value of 1, one 
        /// Unity world unit is equal to one meter.
        /// </summary>
        [Range(0.1f, 1000.0f)]
        [Tooltip(
            "The scale of the user's current view. At a value of 1, one " +
            "Unity world unit is equal to one meter.")]
        public float ViewerScale = 1.0f;

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void Awake()
        {
            this._displayAligner = 
                this.GetComponentInChildren<ZDisplayAligner>();
        }

        private void Update()
        {
            // Enforce uniform scale.
            this.transform.SetUniformScale(
                this.ViewerScale * ZProvider.DisplayScaleFactor);
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The world space position of the frame.
        /// </summary>
        /// 
        /// <remarks>
        /// This accounts for any impact the ZDisplayAligner might have
        /// on the frame's world space position.
        /// </remarks>
        public Vector3 WorldPosition =>
            this._displayAligner?.transform.position ??
            this.transform.position;

        /// <summary>
        /// The world space rotation of the frame.
        /// </summary>
        /// 
        /// <remarks>
        /// This accounts for any impact the ZDisplayAligner might have
        /// on the frame's world space rotation.
        /// </remarks>
        public Quaternion WorldRotation =>
            this._displayAligner?.transform.rotation ??
            this.transform.rotation;

        /// <summary>
        /// A reference to the frame's associated ZDisplayAligner.
        /// </summary>
        public ZDisplayAligner DisplayAligner => this._displayAligner;

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private ZDisplayAligner _displayAligner = null;
    }
}

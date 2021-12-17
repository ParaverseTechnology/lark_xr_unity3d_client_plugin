////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Extensions;
using zSpace.Core.Sdk;

namespace zSpace.Core.Input
{
    public class ZStylus : ZPointer
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        protected override void Awake()
        {
            base.Awake();

            if (ZProvider.IsInitialized)
            {
                this._target = ZProvider.StylusTarget;
                this._viewport = ZProvider.Viewport;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The unique id of the stylus pointer.
        /// </summary>
        public override int Id => 0;

        /// <summary>
        /// The current visibility state of the stylus.
        /// </summary>
        public override bool IsVisible => this._target?.IsVisible ?? false;

        /// <summary>
        /// The number of buttons supported by the stylus.
        /// </summary>
        public override int ButtonCount => this._target?.ButtonCount ?? 0;

        /// <summary>
        /// The current scroll delta of the stylus.
        /// </summary>
        /// 
        /// <remarks>
        /// Since the stylus has not scroll support, the current implementation
        /// is hard-coded to the zero vector.
        /// </remarks>
        public override Vector2 ScrollDelta => Vector2.zero;

        /// <summary>
        /// The pose of the stylus in tracker space.
        /// </summary>
        public Pose TrackerPose => this._target?.Pose ?? default(Pose);

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
        public override bool GetButton(int id)
        {
            return this._target?.IsButtonPressed(id) ?? false;
        }

        ////////////////////////////////////////////////////////////////////////
        // Protected Methods
        ////////////////////////////////////////////////////////////////////////

        protected override Pose ComputeWorldPose()
        {
            if (this._target == null || this._viewport == null)
            {
                return this.transform.ToPose();
            }

            Pose trackerPose = this._target.Pose;

            Matrix4x4 trackerToWorldMatrix =
                this.EventCamera.CameraToWorldMatrix *
                this._viewport.GetCoordinateSpaceTransform(
                    ZCoordinateSpace.Tracker, 
                    ZCoordinateSpace.Camera);

            return trackerPose.GetTransformedBy(trackerToWorldMatrix);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private ZTarget _target = null;
        private ZViewport _viewport = null;
    }
}

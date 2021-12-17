////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Extensions;

namespace zSpace.Core.Input
{
    public class ZMouse : ZPointer
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        protected override void OnEnable()
        {
            base.OnEnable();

            Cursor.visible = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Cursor.visible = true;
        }

        protected override void Start()
        {
            base.Start();

            Cursor.visible = false;
        }

        protected override void Update()
        {
            base.Update();

#if UNITY_EDITOR
            Cursor.visible = false;
#endif
        }

        private void OnApplicationPause(bool isPaused)
        {
            Cursor.visible = isPaused;
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The unique id of the mouse pointer.
        /// </summary>
        public override int Id => 1000;

        /// <summary>
        /// The current visibility state of the mouse.
        /// </summary>
        /// 
        /// <remarks>
        /// Since the mouse is not a 6-DOF trackable target and is present
        /// on all platforms we currently support (e.g. Windows), IsVisible
        /// is hard-coded to true.
        /// </remarks>
        public override bool IsVisible => true;

        /// <summary>
        /// The number of buttons supported by the mouse.
        /// </summary>
        public override int ButtonCount => 3;

        /// <summary>
        /// The current scroll delta for the mouse.
        /// </summary>
        /// 
        /// <remarks>
        /// The scroll delta for the mouse is only stored in Vector2.y
        /// (Vector2.x is ignored).
        /// </remarks>
        public override Vector2 ScrollDelta => 
            UnityEngine.Input.mouseScrollDelta;

        /// <summary>
        /// The pose of the pointer's current end point in world space.
        /// </summary>
        /// 
        /// <remarks>
        /// In this particular case, this will be the the mouse cursor's 
        /// world pose.
        /// </remarks>
        public override Pose EndPointWorldPose => new Pose(
            this.HitInfo.worldPosition,
            this.EventCamera?.ZeroParallaxPose.rotation ?? 
            this.transform.rotation);

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
            return UnityEngine.Input.GetMouseButton(id);
        }

        ////////////////////////////////////////////////////////////////////////
        // Protected Methods
        ////////////////////////////////////////////////////////////////////////

        protected override Pose ComputeWorldPose()
        {
            Ray mouseRay = this.EventCamera.Camera.ScreenPointToRay(
                UnityEngine.Input.mousePosition);

            return mouseRay.ToPose(this.EventCamera.transform.up);
        }
    }
}

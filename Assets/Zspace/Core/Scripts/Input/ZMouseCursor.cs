////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Extensions;

namespace zSpace.Core.Input
{
    public class ZMouseCursor : ZPointerVisualization
    {
        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The duration in seconds to flip the cursor.
        /// </summary>
        /// 
        /// <remarks>
        /// A value of 0 will cause the flip to occur instantaneously.
        /// </remarks>
        [Tooltip("The duration in seconds to flip the cursor.")]
        public float FlipDuration = 0.1f;

        /// <summary>
        /// The duration in seconds for the cursor to snap to objects.
        /// </summary>
        [Tooltip("The duration in seconds for the cursor to snap to objects.")]
        public float SnapDuration = 0.05f;

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void Awake()
        {
            this._spriteRenderer = 
                this.GetComponentInChildren<SpriteRenderer>();
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        public override void Process(ZPointer pointer, Vector3 worldScale)
        {
            base.Process(pointer, worldScale);

            // Update the mouse cursor's position and rotation.
            this.transform.SetPose(this.GetEndPointPose(pointer));

            // Update the mouse cursor's scale based on whether 
            // stereoscopic 3D rendering is currently active. When 
            // rendering in monoscopic 3D, scale the cursor based on its 
            // distance from the camera.
            float distanceScale = this.GetCameraDistanceScale(
                pointer.EventCamera, this.transform.position);

            this.transform.localScale = Vector3.Lerp(
                Vector3.one * distanceScale,
                worldScale,
                pointer.EventCamera.StereoWeight);

            // Update whether the mouse cursor should be flipped about its
            // horizontal and vertical axes based on the hit normal in
            // screen space. This will minimize the chance that the mouse 
            // cursor will be occluded by the object it is intersecting.
            if (!pointer.AnyButtonPressed)
            {
                float flipThreshold = 89;

                Vector3 screenNormal =
                    Quaternion.Inverse(this.transform.rotation) *
                    pointer.HitInfo.worldNormal;

                this._flipHorizontal =
                    (Vector3.Angle(screenNormal, Vector3.left) < flipThreshold);

                this._flipVertical =
                    (Vector3.Angle(screenNormal, Vector3.up) < flipThreshold);
            }

            // Update the mouse cursor's corresponding sprite.
            this.UpdateSprite();
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private Pose GetEndPointPose(ZPointer pointer)
        {
            if (pointer.AnyButtonPressed || pointer.MaxHitRadius == 0)
            {
                this._positionWeight = 1;
            }
            else if (pointer.HitInfo.gameObject != this._previousHitObject)
            {
                this._positionWeight = 0;
                this._startPosition = this.transform.position;
            }

            Pose pose = pointer.EndPointWorldPose;
            pose.position =
                Vector3.Lerp(this._startPosition, pose.position, this._positionWeight) +
                (pose.rotation * Vector3.back * 0.0001f);

            this._previousHitObject = pointer.HitInfo.gameObject;
            this._positionWeight += Time.unscaledDeltaTime / this.SnapDuration;
            this._positionWeight = Mathf.Clamp01(this._positionWeight);

            return pose;
        }

        public float GetCameraDistanceScale(ZCamera camera, Vector3 point)
        {
            Plane monoCameraPlane = new Plane(
                -camera.ZeroParallaxPlane.normal,
                camera.transform.position);

            float distanceToCameraPlane = 
                monoCameraPlane.GetDistanceToPoint(point);

            float distanceFromCameraToZeroParallax = 
                camera.CameraOffset.magnitude;

            return (distanceToCameraPlane / distanceFromCameraToZeroParallax);
        }

        private void UpdateSprite()
        {
            if (this._spriteRenderer == null)
            {
                return;
            }

            Transform spriteTransform = this._spriteRenderer.transform;

            // Update the sprite's local rotation.
            Quaternion targetRotation = Quaternion.Euler(
                this._flipVertical ? (this._flipHorizontal ? 180 : -180) : 0,
                this._flipHorizontal ? -180 : 0,
                0);

            if (this.FlipDuration == 0)
            {
                spriteTransform.localRotation = targetRotation;
            }
            else
            {
                spriteTransform.localRotation = Quaternion.RotateTowards(
                    spriteTransform.localRotation,
                    targetRotation,
                    180 / this.FlipDuration * Time.unscaledDeltaTime);
            }

            // Update the sprite's local scale.
            Vector3 metersPerPixel = ZProvider.DisplayMetersPerPixel;

            spriteTransform.localScale =
                Vector3.one * Mathf.Min(metersPerPixel.x, metersPerPixel.y);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private SpriteRenderer _spriteRenderer = null;

        private bool _flipHorizontal = false;
        private bool _flipVertical = false;

        private GameObject _previousHitObject = null;
        private float _positionWeight = 1;
        private Vector3 _startPosition = Vector3.zero;
    }
}

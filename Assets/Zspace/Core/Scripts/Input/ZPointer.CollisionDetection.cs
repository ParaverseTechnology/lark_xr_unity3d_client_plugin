////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;

using zSpace.Core.UI;

namespace zSpace.Core.Input
{
    public abstract partial class ZPointer
    {
        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private RaycastResult Raycast(Ray ray)
        {
            // Perform a raycast against all high priority objects.
            if (this.PriorityMask != 0)
            {
                RaycastResult highPriorityResult = this.Raycast(
                    ray, ~this.IgnoreMask & this.PriorityMask);

                if (highPriorityResult.gameObject != null)
                {
                    return highPriorityResult;
                }
            }
            
            // Perform a raycast against all low priority objects.
            RaycastResult lowPriorityResult = this.Raycast(
                ray, ~this.IgnoreMask & ~this.PriorityMask);

            return lowPriorityResult;
        }

        private RaycastResult Raycast(Ray ray, int layerMask)
        {
            // Initialize the raycast result.
            RaycastResult result = default(RaycastResult);
            result.distance = this.DefaultHitDistance;
            result.worldNormal = -ray.direction;
            result.worldPosition = ray.GetPoint(result.distance);
            result.screenPosition = this.EventCamera.Camera.WorldToScreenPoint(
                result.worldPosition);

            // Perform a physics raycast to determine if any physics objects 
            // are hit.
            RaycastHit physicsResult;
            if (Physics.Raycast(
                    ray, out physicsResult, result.distance, layerMask))
            {
                result.distance = physicsResult.distance;
                result.worldPosition = physicsResult.point;
                result.worldNormal = physicsResult.normal;
                result.gameObject = physicsResult.collider.gameObject;
                result.screenPosition =
                    this.EventCamera.Camera.WorldToScreenPoint(
                        physicsResult.point);
            }

            // Perform a graphics raycast to determine if any UI objects 
            // are hit.
            RaycastResult graphicResult;
            if (ZGraphicRaycaster.Raycast(
                    ray, out graphicResult, result.distance, layerMask))
            {
                result = graphicResult;
            }

            // Perform a physics spherecast if the hit radius is greater
            // than zero to determine if any physics objects are within the
            // hit radius.
            float radius = this.DefaultHitRadius;
            if (radius > 0)
            {
                // Increase the hit radius by 10% when an object is intersected
                // to eliminate instability issues in collision detection results
                // when the pointer is straddling intersection boundaries.
                if (this.HitInfo.gameObject != null)
                {
                    radius *= 1.1f;
                }

                if (this.SphereCast(
                        ray, radius, out physicsResult, result.distance, layerMask))
                {
                    if (result.gameObject == null ||
                        result.gameObject == physicsResult.collider.gameObject)
                    {
                        result.distance = physicsResult.distance;
                        result.worldPosition = physicsResult.point;
                        result.worldNormal = physicsResult.normal;
                        result.gameObject = physicsResult.collider.gameObject;
                        result.screenPosition =
                            this.EventCamera.Camera.WorldToScreenPoint(
                                physicsResult.point);
                    }
                }
            }

            // If neither the physics nor graphics raycast succeeded, 
            // perform a raycast against the default collision plane.
            if (result.gameObject == null &&
                this.DefaultCollisionPlane == CollisionPlane.Screen)
            {
                result = this.Raycast(
                    ray, this.EventCamera.ZeroParallaxPlane, false);
            }

            return result;
        }

        private RaycastResult Raycast(Ray ray, Plane plane, bool lockToPlane)
        {
            RaycastResult result = default(RaycastResult);

            float distance = 0.0f;
            if (plane.Raycast(ray, out distance))
            {
                distance = lockToPlane ?
                    distance : Mathf.Min(distance, this.DefaultHitDistance);

                result.worldNormal = plane.normal;
            }
            else
            {
                distance = lockToPlane ?
                    this._hitInfo.distance : this.DefaultHitDistance;

                result.worldNormal = -ray.direction.normalized;
            }

            result.distance = distance;
            result.worldPosition = ray.origin + (ray.direction * distance);
            result.screenPosition =
                this.EventCamera.Camera.WorldToScreenPoint(
                    result.worldPosition);

            return result;
        }

        private bool SphereCast(
            Ray ray, float radius, out RaycastHit hitInfo, float maxDistance, int layerMask)
        {
            hitInfo = default(RaycastHit);
            RaycastHit[] hitInfos = Physics.SphereCastAll(
                ray, radius, maxDistance, layerMask);

            bool success = false;
            float distance = float.MaxValue;

            // Prioritize spherecast results based on their distance from the
            // ray's line as opposed to the ray's origin.
            for (int i = 0; i < hitInfos.Length; ++i)
            {
                float distanceFromRay = Vector3.Cross(
                    ray.direction, hitInfos[i].point - ray.origin).magnitude;

                if (distanceFromRay < distance)
                {
                    hitInfo = hitInfos[i];
                    distance = distanceFromRay;
                    success = true;
                }
            }

            return success;
        }
    }
}

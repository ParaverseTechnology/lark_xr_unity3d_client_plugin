////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace zSpace.Core.Input
{
    public abstract partial class ZPointer
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void OnDrawGizmos()
        {
            // Update the hit info.
            if (!Application.isPlaying)
            {
                this._hitInfo.distance = this.DefaultHitDistance;

                this._hitInfo.worldPosition =
                    this.transform.position +
                    (this.transform.forward * this.DefaultHitDistance);

                this._hitInfo.worldNormal = -this.transform.forward;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (this.EventCamera != null)
            {
                Vector3 startPoint = this.transform.position;
                Vector3 hitPoint = this.HitInfo.worldPosition;
                Vector3 hitNormal = this.HitInfo.worldNormal;

                // Draw the hit distance.
                Handles.color = Color.white;
                Handles.DrawDottedLine(startPoint, hitPoint, 5);
                Handles.Label(
                    (startPoint + hitPoint) / 2,
                    string.Format("{0:0.000} m", this.HitInfo.distance));

                // Draw the hit normal.
                Handles.color = new Color32(0, 191, 255, 155);
                this.DrawNormal(hitPoint, hitNormal, this.transform.up);

                // Draw the hit radius information.
                float hitRadius = this.DefaultHitRadius;
                if (hitRadius > 0)
                {
                    Vector3 direction = this.transform.forward;
                    Vector3 projectedHitPoint =
                        Vector3.Project(hitPoint - startPoint, direction) +
                        startPoint;

                    Handles.color = Color.white;
                    this.DrawDiscs(startPoint, projectedHitPoint, hitRadius, 5);
     
                    Handles.DrawLine(startPoint, projectedHitPoint);
                    Handles.DrawDottedLine(hitPoint, projectedHitPoint, 5);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void DrawNormal(Vector3 position, Vector3 normal, Vector3 up)
        {
            Quaternion rotation = Quaternion.LookRotation(normal, up);
            float size = HandleUtility.GetHandleSize(position) * 0.75f;

            Handles.ArrowHandleCap(
                0, position, rotation, size, EventType.Repaint);
        }

        private void DrawDiscs(Vector3 a, Vector3 b, float radius, int count)
        {
            float t = 0;
            float step = 1 / (float)(count - 1);

            Color originalColor = Handles.color;
            Color startColor = Handles.color;
            startColor.a = 0.2f;

            Color endColor = Handles.color;
            endColor.a = 0.8f;

            for (int i = 0; i < count; ++i)
            {
                Handles.color = Color.Lerp(endColor, startColor, t);
                Handles.DrawWireDisc(Vector3.Lerp(b, a, t), b - a, radius);

                t += step;
            }

            Handles.color = originalColor;
        }
    }
}

#endif // UNITY_EDITOR

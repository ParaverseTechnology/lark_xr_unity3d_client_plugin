////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace zSpace.Core.Utility
{
    public partial class ZDisplayAligner
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void OnDrawGizmosSelected()
        {
            Handles.matrix = this.ComputePivotLocalToWorldMatrix();

            // Draw the ground plane.
            Handles.DrawSolidRectangleWithOutline(
                GroundPlaneVertices, RectFaceColor, RectOutlineColor);

            // Draw the pivot cross-hairs.
            this.DrawPivotCrossHairs(RectOutlineColor);

            // Draw the current display angle.
            this.DrawAngle(this.Angle, ArcFaceColor, ArcOutlineColor);

            // Draw the clamp region.
            if (this.ClampAngle)
            {
                Color clampColor = (this.MinAngle <= this.MaxAngle) ?
                    Color.white : Color.red;

                this.DrawAngleClamp(this.MinAngle, clampColor);
                this.DrawAngleClamp(this.MaxAngle, clampColor);
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void DrawPivotCrossHairs(Color color)
        {
            Vector3 windowSize = ZProvider.WindowSize;
            Vector3 windowHalfSize = windowSize * 0.5f;

            Gizmos.color = color;
            Gizmos.matrix = this.transform.localToWorldMatrix;

            // Draw horizontal line.
            Gizmos.DrawLine(
                new Vector3(-windowHalfSize.x, this._localPivot.y, 0),
                new Vector3(windowHalfSize.x, this._localPivot.y, 0));

            // Draw vertical line.
            Gizmos.DrawLine(
                new Vector3(this._localPivot.x, -windowHalfSize.y, 0),
                new Vector3(this._localPivot.x, windowHalfSize.y, 0));

            // Draw the window rectangle.
            Gizmos.DrawWireCube(
                Vector3.zero, new Vector3(windowSize.x, windowSize.y, 0));
        }

        private void DrawAngle(
            float angle, Color faceColor, Color outlineColor)
        {
            Vector3 labelPosition = this.ComputePositionOnArc(angle * 0.5f);
            Handles.Label(labelPosition * 1.25f, angle + "\u00B0");

            // Draw face.
            Handles.color = faceColor;
            Handles.DrawSolidArc(
                Vector3.zero, Vector3.left, Vector3.forward, angle, 1.0f);

            // Draw outline.
            Handles.color = outlineColor;
            Handles.DrawLine(Vector3.zero, Vector3.forward);
            Handles.DrawLine(Vector3.zero, this.ComputePositionOnArc(angle));
            Handles.DrawWireArc(
                Vector3.zero, Vector3.left, Vector3.forward, angle, 1.0f);
        }

        private void DrawAngleClamp(float angle, Color color)
        {
            Vector3 position = this.ComputePositionOnArc(angle);
            
            Handles.color = color;
            Handles.DrawLine(Vector3.zero, position);
            Handles.SphereHandleCap(
                0, position, Quaternion.identity, 0.05f, EventType.Repaint);
        }

        private Vector3 ComputePositionOnArc(float angle)
        {
            return Quaternion.Euler(-angle, 0, 0) * Vector3.forward;
        }

        private Matrix4x4 ComputePivotLocalToWorldMatrix()
        {
            Vector4 position =
                this.transform.position +
                (this.transform.rotation *
                    (this._localPivot * this.transform.lossyScale.y));

            Quaternion rotation =
                this.transform.parent?.rotation ?? Quaternion.identity;

            Vector3 scale =
                Vector3.one * HandleUtility.GetHandleSize(position) * 0.5f;

            return Matrix4x4.TRS(position, rotation, scale);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Static Members
        ////////////////////////////////////////////////////////////////////////

        private static readonly Color RectFaceColor =
            new Color32(255, 255, 255, 25);

        private static readonly Color RectOutlineColor =
            new Color32(255, 255, 255, 155);

        private static readonly Color ArcFaceColor =
            new Color32(0, 191, 255, 25);

        private static readonly Color ArcOutlineColor =
            new Color32(0, 191, 255, 155);

        private static readonly Vector3[] GroundPlaneVertices = 
            new Vector3[]
            {
                new Vector3(1, 0, 1),
                new Vector3(1, 0, -1),
                new Vector3(-1, 0, -1),
                new Vector3(-1, 0, 1),
            };
    }
}

#endif // UNITY_EDITOR

////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

using zSpace.Core.Extensions;
using zSpace.Core.Sdk;

namespace zSpace.Core
{
    public sealed partial class ZCamera
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void OnDrawGizmos()
        {
            // Compute the frustum corners for the positive, negative,
            // and zero parallax zones.
            this.GetFrustumCorners(
                ZFrustum.UncoupledZoneDepth, this._positiveParallaxCorners);

            this.GetFrustumCorners(
                ZFrustum.CoupledZoneDepth, this._negativeParallaxCorners);

            this.GetFrustumCorners(0, this._zeroParallaxCorners);

            // Draw the positive, negative, and zero parallax zones.
            Handles.matrix = this.transform.localToWorldMatrix.ToPoseMatrix();

            Handles.color = PositiveParallaxColor;
            this.DrawComfortZone(
                this._positiveParallaxCorners, this._zeroParallaxCorners);

            Handles.color = NegativeParallaxColor;
            this.DrawComfortZone(
                this._negativeParallaxCorners, this._zeroParallaxCorners);

            Handles.color = ZeroParallaxColor;
            this.DrawRectangle(this._zeroParallaxCorners);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw the positive, negative, and zero parallax zone labels.
            Handles.matrix = this.transform.localToWorldMatrix.ToPoseMatrix();

            this.DrawLabel(this._positiveParallaxCorners, "Positive Parallax");
            this.DrawLabel(this._negativeParallaxCorners, "Negative Parallax");
            this.DrawLabel(this._zeroParallaxCorners, "Zero Parallax");
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void GetFrustumCorners(float zOffset, Vector3[] corners)
        {
            float worldScale = this.WorldScale.z;
            float screenDistance = this.ZeroParallaxPlane.GetDistanceToPoint(
                this.transform.position);

            this.Camera.CalculateFrustumCorners(
                Viewport, 
                screenDistance + (zOffset * worldScale), 
                Camera.MonoOrStereoscopicEye.Mono, 
                corners);
        }

        private void DrawComfortZone(
            Vector3[] startCorners, Vector3[] endCorners)
        {
            this.DrawRectangle(startCorners);

            for (int i = 0; i < CornerCount; ++i)
            {
                Handles.DrawLine(startCorners[i], endCorners[i]);
            }
        }

        private void DrawRectangle(Vector3[] corners)
        {
            Handles.DrawSolidRectangleWithOutline(  
                corners, Color.clear, Color.white);
        }

        private void DrawLabel(Vector3[] corners, string label)
        {
            // Draw label at the top-middle of the rectangle.
            Handles.Label((corners[1] + corners[2]) / 2, label);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private static readonly Rect Viewport = new Rect(0, 0, 1, 1);

        private static readonly Color NegativeParallaxColor = 
            new Color32(30, 144, 255, 255);

        private static readonly Color PositiveParallaxColor = 
            new Color32(60, 179, 113, 255);

        private static readonly Color ZeroParallaxColor =
            new Color32(200, 125, 0, 255);

        private const int CornerCount = 4;

        private Vector3[] _positiveParallaxCorners = new Vector3[CornerCount];
        private Vector3[] _negativeParallaxCorners = new Vector3[CornerCount];
        private Vector3[] _zeroParallaxCorners = new Vector3[CornerCount]; 
    }
}

#endif // UNITY_EDITOR

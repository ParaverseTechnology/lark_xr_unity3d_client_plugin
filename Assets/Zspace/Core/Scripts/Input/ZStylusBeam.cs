////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using zSpace.Core.Extensions;

namespace zSpace.Core.Input
{
    public partial class ZStylusBeam : ZPointerVisualization
    {
        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// A ratio indciating how much of the beam will arc to its end point.
        /// </summary>
        [Tooltip("A ratio indciating how much of the beam will arc to its end point.")]
        [Range(0, 1)]
        public float CurveStartPivot = 0.35f;

        /// <summary>
        /// How quickly the beam will snap to its target end point.
        /// </summary>
        [Tooltip("How quickly the beam will snap to its target end point.")]
        [Range(0, 0.1f)]
        public float EndPointSmoothTime = 0.02f;

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void Awake()
        {
            this._lineRenderer = this.GetComponentInChildren<LineRenderer>();

            this._originalWidthMultiplier = this._lineRenderer.widthMultiplier;

            this._endPoint = this.transform.position;
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        public override void Process(ZPointer pointer, Vector3 worldScale)
        {
            base.Process(pointer, worldScale);

            this._lineRenderer.gameObject.SetActive(pointer.IsVisible);

            this._lineRenderer.widthMultiplier =
                this._originalWidthMultiplier * 
                Mathf.Min(worldScale.x, worldScale.y);

            this.UpdateLineRendererPositions(pointer);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void UpdateLineRendererPositions(ZPointer pointer)
        {
            // Update the end point.
            Vector3 hitPosition = pointer.HitInfo.worldPosition;

            if (pointer.AnyButtonPressed)
            {
                this._endPoint = hitPosition;
            }
            else
            {
                this._endPoint = Vector3.SmoothDamp(
                    this._endPoint, hitPosition, ref this._velocity, this.EndPointSmoothTime);
            }

            // Compute the control points for the line renderer's quadratic 
            // bezier curve. Additionally, transform the control points to be 
            // in the local space of the pointer since the current assumption 
            // is that the line renderer is a child of the pointer.
            Vector3 p0 = pointer.transform.position;
            Vector3 p2 = this._endPoint;
            Vector3 p1 = p0 + Vector3.Project(p2 - p0, pointer.transform.forward);

            Matrix4x4 worldToLocalMatrix = this.transform.worldToLocalMatrix;

            p0 = worldToLocalMatrix.MultiplyPoint(p0);
            p1 = worldToLocalMatrix.MultiplyPoint(p1);
            p2 = worldToLocalMatrix.MultiplyPoint(p2);

            this._lineRenderer.SetPosition(0, p0);
            this._lineRenderer.SetBezierCurve(
                1, Vector3.Lerp(p0, p1, this.CurveStartPivot), p1, p2);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private LineRenderer _lineRenderer = null;

        private float _originalWidthMultiplier = 1.0f;

        private Vector3 _endPoint = Vector3.zero;
        private Vector3 _velocity = Vector3.zero;
    }
}

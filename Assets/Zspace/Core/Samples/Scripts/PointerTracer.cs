////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using UnityEngine;

using zSpace.Core.Input;

namespace zSpace.Core.Samples
{
    public class PointerTracer : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The pointer to handle tracing for
        /// </summary>
        [Tooltip("The pointer to handle tracing for")]

        public ZPointer Pointer;

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        void Start() 
        {
            this._lineRenderer = gameObject.GetComponent<LineRenderer>();

            this.Pointer.OnObjectEntered.AddListener(this.HandleOnObjectEntered);
            this.Pointer.OnObjectExited.AddListener(this.HandleOnObjectExited);
            this.Pointer.OnButtonPressed.AddListener(this.HandleOnButtonPressed);
            this.Pointer.OnButtonReleased.AddListener(this.HandleOnButtonReleased);

            this._pointList = new List<Vector3>();
        }

        void Update() 
        {
            if (this._isDrawing)
            {
                this._accumulatedDelta += Time.deltaTime;
                if (this._accumulatedDelta > this._pointsPerSecond)
                {
                    this._pointList.Add(this.Pointer.EndPointWorldPose.position);
                    this._lineRenderer.positionCount = this._pointList.Count;
                    this._lineRenderer.SetPositions(this._pointList.ToArray());
                    this._accumulatedDelta = 0.0f;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void HandleOnObjectEntered(ZPointer p, GameObject objectEntered)
        {
            if (objectEntered.GetComponent<Draggable>() != null)
            {
                this._isDraggableIntersected = true;
            }
        }

        private void HandleOnObjectExited(ZPointer p, GameObject objectEntered)
        {
            this._isDraggableIntersected = false;
        }

        private void HandleOnButtonPressed(ZPointer p, int i)
        {
            if (!this._isDraggableIntersected && i == 0)
            {
                this._isDrawing = true;
                this._pointList.Clear();
                this._pointList.Add(this.Pointer.EndPointWorldPose.position);
                this._lineRenderer.positionCount = this._pointList.Count;
                this._lineRenderer.SetPositions(this._pointList.ToArray());
            }
        }

        private void HandleOnButtonReleased(ZPointer p, int i)
        {
            if (i == 0)
            {
                this._isDrawing = false;
                this._accumulatedDelta = 0.0f;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private LineRenderer _lineRenderer;
        private bool _isDraggableIntersected;
        private bool _isDrawing = false;
        private float _pointsPerSecond = 0.1f;
        private float _accumulatedDelta = 0.0f;
        private List<Vector3> _pointList;
    }
}

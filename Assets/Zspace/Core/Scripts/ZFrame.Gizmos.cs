////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace zSpace.Core
{
    public sealed partial class ZFrame
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void OnDrawGizmos()
        {
            Handles.matrix = this.DisplayLocalToWorldMatrix;

            Handles.color = DisplayColor;
            this.DrawRectangle(this.DisplaySize);
        }

        private void OnDrawGizmosSelected()
        {
            Handles.matrix = this.DisplayLocalToWorldMatrix;

            Handles.Label(this.DisplaySize * 0.5f, "Display");
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Properties
        ////////////////////////////////////////////////////////////////////////

        private Matrix4x4 DisplayLocalToWorldMatrix =>
            Matrix4x4.TRS(
                this.transform.position, 
                this.WorldRotation, 
                Vector3.one * this.ViewerScale) *
            Matrix4x4.Translate(
                new Vector3(0, (0.5f - this.DisplayPivot) * this.DisplaySize.y, 0));

        private Vector2 DisplaySize => ZProvider.DisplayReferenceSize;

        private float DisplayPivot => this._displayAligner?.Pivot ?? 0.5f;

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void DrawRectangle(Vector2 size)
        {
            Handles.DrawSolidRectangleWithOutline(
                new Rect(-size * 0.5f, size), Color.clear, Color.white);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Static Members
        ////////////////////////////////////////////////////////////////////////

        private static readonly Color DisplayColor =
            new Color32(169, 169, 169, 255);
    }
}

#endif // UNITY_EDITOR

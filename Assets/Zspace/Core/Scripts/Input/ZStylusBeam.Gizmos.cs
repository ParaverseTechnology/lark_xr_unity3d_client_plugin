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
    public partial class ZStylusBeam
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void OnDrawGizmosSelected()
        {
            if (this.gameObject != Selection.activeGameObject)
            {
                return;
            }

            if (this._pointer == null)
            {
                this._pointer = this.GetComponentInParent<ZPointer>();
            }

            if (this._pointer != null)
            {
                Vector3 p0 = this._pointer.transform.position;
                Vector3 p2 = this._pointer.HitInfo.worldPosition;
                Vector3 p1 = 
                    p0 + Vector3.Project(p2 - p0, this._pointer.transform.forward);

                Handles.color = Color.white;

                Handles.DrawDottedLine(p0, p1, 5);
                Handles.DrawDottedLine(p1, p2, 5);

                this.DrawPoint(Vector3.Lerp(p0, p1, this.CurveStartPivot), "p0");
                this.DrawPoint(p1, "p1");
                this.DrawPoint(p2, "p2");
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void DrawPoint(Vector3 position, string labelText)
        {
            Quaternion rotation = Quaternion.identity;
            float size = HandleUtility.GetHandleSize(position) * 0.1f;

            Handles.SphereHandleCap(0, position, rotation, size, EventType.Repaint);
            Handles.Label(position, labelText);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private ZPointer _pointer = null;
    }
}

#endif // UNITY_EDITOR

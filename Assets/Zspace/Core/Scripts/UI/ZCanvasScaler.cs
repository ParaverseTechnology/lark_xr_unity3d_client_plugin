////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace zSpace.Core.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Canvas))]
    public class ZCanvasScaler : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void Awake()
        {
            this._rectTransform = this.GetComponent<RectTransform>();
        }

        private void Update()
        {
            this.UpdateSize();
            this.UpdateScale();
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void UpdateSize()
        {
            this._rectTransform.sizeDelta = ZProvider.WindowSizePixels;
        }

        private void UpdateScale()
        {
            Vector2 metersPerPixel = ZProvider.DisplayMetersPerPixel;

            this._rectTransform.localScale = new Vector3(
                metersPerPixel.x,
                metersPerPixel.y,
                Mathf.Min(metersPerPixel.x, metersPerPixel.y));
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private RectTransform _rectTransform = null;
    }
}

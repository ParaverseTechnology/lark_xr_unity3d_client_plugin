////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;

namespace zSpace.Core.Samples
{
    public class HighlightOnHover : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
    {
        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The color to set an object's material to when hovered.
        /// </summary>
        [Tooltip("The color to set an object's material to when hovered.")]
        public Color HighlightColor;

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        void Start() 
        {
            this._mat = this.gameObject.GetComponent<MeshRenderer>().material;
            this._oldColor = this._mat.GetColor("_Color");
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        public void OnPointerEnter(PointerEventData evtData)
        {
            this._mat.SetColor("_Color", HighlightColor);
        }

        public void OnPointerExit(PointerEventData evtData)
        {
            this._mat.SetColor("_Color", _oldColor);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private Color _oldColor;
        private Material _mat;
    }
}

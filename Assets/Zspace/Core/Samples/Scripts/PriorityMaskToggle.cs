////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;

using zSpace.Core.Input;

namespace zSpace.Core.Samples
{
    public class PriorityMaskToggle : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void Start()
        {
            this._zStylus = GameObject.FindObjectOfType<ZStylus>();
            this._zMouse = GameObject.FindObjectOfType<ZMouse>();
            this._toggle = gameObject.GetComponent<Toggle>();
            this._toggle.onValueChanged.AddListener(this.HandleOnToggleValueChanged);
            this._priorityMask = (1 << LayerMask.NameToLayer("Water")) |
                (1 << LayerMask.NameToLayer("UI"));
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void HandleOnToggleValueChanged(bool b)
        {
            if (b)
            {
                this._zStylus.PriorityMask = this._priorityMask;
                this._zMouse.PriorityMask = this._priorityMask;
            }
            else
            {
                this._zStylus.PriorityMask = this._noneMask;
                this._zMouse.PriorityMask = this._noneMask;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private ZStylus _zStylus;
        private ZMouse _zMouse;
        private Toggle _toggle;
        private LayerMask _priorityMask;
        private LayerMask _noneMask = 0;
    }
}

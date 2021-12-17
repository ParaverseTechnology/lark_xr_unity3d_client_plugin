////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;

using zSpace.Core.Sdk;

namespace zSpace.Core.Samples
{
    public class StylusLEDFeedback : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
    {
        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        public Color HoverColor;

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void Start()
        {
            if (ZProvider.IsInitialized)
            {
                this._stylusTarget = ZProvider.StylusTarget;

                if (ZProvider.CurrentDisplay.Size !=
                    ZDisplay.GetSize(ZDisplay.Profile.Size24InchAspect16x9))
                {
                    Debug.LogWarning("AIO model hardware not detected.\n " +
                        "Stylus vibration and LED light feedback will not " +
                        "be experienced.");
                }
            }
            else
            {
                Debug.LogWarning("ZProvider can not initialize.\n Stylus" +
                    "LED light feedback will not be experienced.");

                Destroy(this);
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        public void OnPointerEnter(PointerEventData eventData)
        {
            this._stylusTarget.IsLedEnabled = true;
            this._stylusTarget.LedColor = HoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this._stylusTarget.IsLedEnabled = false;
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private ZTarget _stylusTarget;
    }
}

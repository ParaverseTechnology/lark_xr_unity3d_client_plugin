////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;

namespace zSpace.Core.Samples
{
    public class FramePickerMenu : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        void Start()
        {
            this._frames = GameObject.FindObjectsOfType<ZFrame>();
            this._cameraRig = GameObject.FindObjectOfType<ZCameraRig>();
            this._layoutGroup =
                this.gameObject.GetComponent<VerticalLayoutGroup>();
            this._baseButton = 
                this._layoutGroup.GetComponentInChildren<Button>().gameObject;

            for (int i = 0; i < this._frames.Length; i++)
            {
                GameObject button = GameObject.Instantiate(this._baseButton);
                button.name = this._frames[i].gameObject.name;
                button.transform.SetParent(this._layoutGroup.transform, false);
                button.GetComponentInChildren<Text>().text = button.name;

                int frameIndex = i;
                button.GetComponent<Button>().onClick.AddListener(
                    delegate{ this.SetFrame(frameIndex); });
            }
            Destroy(this._baseButton);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void SetFrame(int i)
        {
            this._cameraRig.Frame = this._frames[i];
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private ZFrame[] _frames;
        private ZCameraRig _cameraRig;
        private VerticalLayoutGroup _layoutGroup;
        private GameObject _baseButton;
    }
}

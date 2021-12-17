using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkXR {
    public class VkeyRow : MonoBehaviour {
        public string[] keys = { };
        public VkeyButton.OnVkeyClick onVkeyClick;

	    // Use this for initialization
	    void Start () {
        }
	
	    // Update is called once per frame
	    void Update () {
		
	    }

        public void FreshKeys()
        {
            VkeyButton btn = GetComponentInChildren<VkeyButton>();

            for (var i = 0; i < keys.Length; i++)
            {
                VkeyButton child = Instantiate(btn, transform);
                child.id = i;
                child.Text = keys[i];
                child.onVkeyClick += OnClick;
            }

            btn.gameObject.SetActive(false);
        }

        void OnClick(VkeyButton vkeyButton)
        {
            if (onVkeyClick != null)
            {
                onVkeyClick(vkeyButton);
            }
        }
    }
}
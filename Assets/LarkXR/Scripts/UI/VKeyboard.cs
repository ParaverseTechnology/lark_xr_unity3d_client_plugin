using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LarkXR {
    public class VKeyboard : MonoBehaviour {

        public delegate void OnKeyPress(VkeyButton.Type type, string key);

        public enum KeyType
        {
            NUM,
            NONE,
        }

        public KeyType keyType = KeyType.NUM;
        public OnKeyPress onKeyPress;

        private const string BTN_NAME_PREX = "";

        private readonly string[] nums = {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", "."
        };

        // TouchScreenKeyboard touchSceenkeyboard;

        // Use this for initialization
        void Start () {
            VkeyButton[] btns = GetComponentsInChildren<VkeyButton>();
            foreach (var i in btns) {
                if (i.type != VkeyButton.Type.VALUE)
                {
                    i.onVkeyClick += OnClick;
                }
            }

            // m_keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);
            VkeyRow vkeyRow = GetComponentInChildren<VkeyRow>();
            vkeyRow.keys = nums;
            vkeyRow.FreshKeys();
            vkeyRow.onVkeyClick += OnClick;

            // touchSceenkeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);
        }

        // Update is called once per frame
        void Update () {
            // disable androind keyboard.
            // m_keyboard.active = false;
            // touchSceenkeyboard.active = false;
        }

        void OnClick(VkeyButton vkeyButton)
        {
            Debug.Log("btn click" + vkeyButton.Text);
            if (onKeyPress != null)
            {
                onKeyPress(vkeyButton.type, vkeyButton.Text);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LarkXR {
    // virtual input textfield fro virtul input
    public class VInputField : MonoBehaviour
    {
        public delegate void OnInputStart();
        public delegate void OnInputEnd(string val);

        public string Value
        {
            get
            {
                return inputField == null ? "" : inputField.text;
            }
            set
            {
                if (inputField != null)
                    inputField.text = value;
                else
                    initText = value;
            }
        }
        // virtual keyboad
        public VKeyboard keyboard;
        public OnInputStart onInputStart;
        public OnInputEnd onInputEnd;

        InputField inputField;
        Button showKeyButton;

        // EventSystem currentEventSys;
        TouchScreenKeyboard touchSceenkeyboard;

        private bool isKeyboardshow = false;
        private string initText = "";

        // Use this for initialization
        void Start()
        {
            // currentEventSys = EventSystem.current;
            touchSceenkeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);
            touchSceenkeyboard.active = false;

            inputField = GetComponentInChildren<InputField>();
            showKeyButton = GetComponentInChildren<Button>();
            if (inputField == null) return;
            if (keyboard == null) return;

            inputField.shouldHideMobileInput = true;
            inputField.text = initText;

            keyboard.onKeyPress += OnVkeyPress;
            showKeyButton.onClick.AddListener(OnToggleVkeyboard);
        }

        // Update is called once per frame
        void Update()
        {
            touchSceenkeyboard.active = false;
            // check focused
            // if (inputField.isFocused && !isFocused)
            // {
            //    isFocused = true;
            //    keyboard.gameObject.SetActive(true);
            // } else if (!inputField.isFocused && isFocused)
            // {
            //    isFocused = false;
            //    keyboard.gameObject.SetActive(false);
            /// }
        }

        void OnVkeyPress(VkeyButton.Type type, string key)
        {
            Debug.Log("OnVkeyPress::" + type + ";" + key);
            if (isKeyboardshow)
            {
                switch (type)
                {
                    case VkeyButton.Type.VALUE:
                        inputField.text += key;
                        break;
                    case VkeyButton.Type.DELETE:
                        if (inputField.text.Length > 1)
                        {
                            inputField.text = inputField.text.Remove(inputField.text.Length - 1);
                        }
                        else
                        {
                            inputField.text = "";
                        }
                        break;
                    case VkeyButton.Type.ENTER:

                        OnToggleVkeyboard();
                        break;
                }
            }
        }
        void OnToggleVkeyboard()
        {
            Debug.Log("OnToggleVkeyboard:" + keyboard.gameObject.activeSelf);
            // currentEventSys.SetSelectedGameObject(null);
            isKeyboardshow = !keyboard.gameObject.activeSelf;
            if (isKeyboardshow && onInputStart != null)
            {
                onInputStart();
            }
            if (!isKeyboardshow && onInputEnd != null)
            {
                onInputEnd(inputField.text);
            }
            keyboard.gameObject.SetActive(isKeyboardshow);
        }
    }
}
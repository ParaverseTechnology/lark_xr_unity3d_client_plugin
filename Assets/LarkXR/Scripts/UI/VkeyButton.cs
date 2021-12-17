using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LarkXR
{
    public class VkeyButton : MonoBehaviour
    {

        public delegate void OnVkeyClick(VkeyButton vkeyButton);
        public enum Type
        {
            VALUE,
            DELETE,
            ENTER,
        }

        public int id = 0;
        public OnVkeyClick onVkeyClick;
        public Type type = Type.VALUE;

        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                Debug.Log("vkey text:" + text);
                if (button != null)
                {
                    button.GetComponentInChildren<Text>().text = value;
                }
            }
        }
        private Button button;

        void Awake()
        {
            button = GetComponentInChildren<Button>();
            button.onClick.AddListener(OnClick);
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
        void OnClick()
        {
            if (onVkeyClick != null)
            {
                onVkeyClick(this);
            }
        }
        public void AddEventListener(OnVkeyClick onVkeyClick)
        {
            this.onVkeyClick += onVkeyClick;
        }
    }
}
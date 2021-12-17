using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LarkXR {
    public class UIFPS : MonoBehaviour
    {
        private float fps = 60;
        private Text textField;
        void Awake()
        {
            textField = GetComponent<Text>();

            //// change keyboard postion or rotation
            // NibiruKeyBoard.Instance.keyBoardTransform.Rotate(new Vector3(30, 0, 0));
            // // show keyboard
            // NibiruKeyBoard.Instance.Show();
        }

        private int lastFPS = -1;
        void LateUpdate()
        {
            int fps = calculateFPS();
            if (fps != lastFPS)
            {
                string text = " FPS: " + fps + " fps";
                if (textField != null)
                {
                    textField.text = text;
                }
            }
        }

        private int calculateFPS()
        {
            float interp = Time.deltaTime / (0.5f + Time.deltaTime);
            float currentFPS = 1.0f / Time.deltaTime;
            fps = Mathf.Lerp(fps, currentFPS, interp);
            return Mathf.RoundToInt(fps);
        }
    }
}
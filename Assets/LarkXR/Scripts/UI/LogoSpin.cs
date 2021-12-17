using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkXR
{
    public class LogoSpin : MonoBehaviour
    {
        public const float speed = 100f;

        private bool isSpin = false;
        // Use this for initialization
        void Start()
        {
            StartSpin();
        }

        // Update is called once per frame
        void Update()
        {
            // Quaternion rotation = gameObject.transform.rotation;
            // Vector3 ang = rotation.eulerAngles;
            // ang.y += 1;
            // if (++ang.y == 360) ang.y = 0;
            // transform.eulerAngles = ang;

            // Debug.Log("****************** w:" + rotation.w + "x: " + rotation.x + ";y:" + rotation.y  + "; z:" + rotation.z); 
            // if (++rotation.y == 360) rotation.y = 0;
            // rotation.y = Mathf.PI;
            // gameObject.transform.rotation = rotation;
            if (isSpin)
                transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
        public void StartSpin()
        {
            isSpin = true;
        }
        public void StopSpin()
        {
            isSpin = false;
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
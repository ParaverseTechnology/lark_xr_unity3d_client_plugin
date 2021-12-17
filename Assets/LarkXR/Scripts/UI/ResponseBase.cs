using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LarkXR {
    public class ResponseBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Use this for initialization
        void Start()
        {
            Debug.Log("start");
        }

        // Update is called once per frame
        void Update()
        {
            // Debug.Log("update");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("pointer enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("pointer exit");
        }
    }
}
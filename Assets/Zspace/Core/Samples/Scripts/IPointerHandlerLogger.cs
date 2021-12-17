////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;

namespace zSpace.Core.Samples
{
    public class IPointerHandlerLogger : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler, IPointerClickHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer Entered");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Pointer Exited");
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer Down");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Pointer Up");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Pointer Click");
        }
    }
}

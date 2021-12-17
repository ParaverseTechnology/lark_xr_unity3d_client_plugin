////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace zSpace.Core.Input
{
    public class ZPointerInteractable : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Overrides the pointer's current drag policy for this interactable.
        /// </summary>
        /// 
        /// <param name="pointer">
        /// A reference to the pointer currently interacting with this 
        /// interactable.
        /// </param>
        /// 
        /// <returns>
        /// The interactable's drag policy.
        /// </returns>
        public virtual ZPointer.DragPolicy GetDragPolicy(ZPointer pointer)
        {
            if (this.GetComponent<RectTransform>() != null)
            {
                return pointer.UIDragPolicy;
            }
            else
            {
                return pointer.ObjectDragPolicy;
            }
        }

        /// <summary>
        /// Get the interactable's specified drag plane.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public virtual Plane GetDragPlane(ZPointer pointer)
        {
            if (pointer.DefaultCustomDragPlane != null)
            {
                return pointer.DefaultCustomDragPlane(pointer);
            }

            return default(Plane);
        }
    }
}

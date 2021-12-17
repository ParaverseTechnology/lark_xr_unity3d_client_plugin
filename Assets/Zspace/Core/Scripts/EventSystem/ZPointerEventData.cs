////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;

using zSpace.Core.Input;

namespace zSpace.Core.EventSystems
{
    public class ZPointerEventData : PointerEventData
    {
        public ZPointerEventData(EventSystem eventSystem)
            : base(eventSystem)
        {
        }

        /// <summary>
        /// A reference to the pointer responsible for dispatching
        /// the pointer event.
        /// </summary>
        public ZPointer Pointer { get; set; } = null;

        /// <summary>
        /// The event's associated pointer button id.
        /// </summary>
        public int ButtonId { get; set; } = -1;

        /// <summary>
        /// The pointer's 3D position delta since the last update.
        /// </summary>
        public Vector3 Delta3D { get; set; } = Vector3.zero;

        /// <summary>
        /// Is the pointer event in regards to a UI Object
        /// </summary>
        public bool IsUIObject { get; set; } = false;

        /// <summary>
        /// Checks whether the pointer is moving based on its 3D position delta.
        /// </summary>
        /// 
        /// <returns>
        /// True if the pointer is moving. False otherwise.
        /// </returns>
        public bool IsPointerMoving3D()
        {
            return (this.Delta3D.sqrMagnitude > 0);
        }
    }
}

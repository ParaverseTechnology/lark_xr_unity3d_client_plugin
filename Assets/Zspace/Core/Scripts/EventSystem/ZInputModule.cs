////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

using zSpace.Core.Input;

namespace zSpace.Core.EventSystems
{
    [DefaultExecutionOrder(ScriptPriority)]
    public class ZInputModule : StandaloneInputModule
    {
        public const int ScriptPriority = ZProvider.ScriptPriority + 40;

        ////////////////////////////////////////////////////////////////////////
        // BaseInputModule Overrides
        ////////////////////////////////////////////////////////////////////////

        public override bool IsModuleSupported()
        {
            return true;
        }

        public override void Process()
        {
            // Process keyboard events.
            bool sentEvent = this.SendUpdateEventToSelectedObject();

            if (this.eventSystem.sendNavigationEvents)
            {
                if (!sentEvent)
                {
                    sentEvent |= this.SendMoveEventToSelectedObject();
                }

                if (!sentEvent)
                {
                    this.SendSubmitEventToSelectedObject();
                }
            }

            // Fall back to the default logic for processing mouse events if 
            // no ZMouse pointer is active.
            if (!this.IsMousePointerActive())
            {
                this.ProcessMouseEvent();
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        private void Update()
        {
            // Process events for all currently active pointers.
            //
            // NOTE: This is being called in Update() instead of Process()
            //       since we need to ensure that ZCore, ZCamera(s), and
            //       ZPointer(s) are up-to-date (through an explicitly defined
            //       script execution order).
            this.ProcessPointers();
        }

        ////////////////////////////////////////////////////////////////////////
        // Protected Methods
        ////////////////////////////////////////////////////////////////////////

        protected void ProcessPointers()
        {
            IList<ZPointer> pointers = ZPointer.GetInstances();

            for (int i = 0; i < pointers.Count; ++i)
            {
                this.ProcessPointerEvent(pointers[i]);
            }
        }

        protected void ProcessPointerEvent(ZPointer pointer)
        {
            for (int i = 0; i < pointer.ButtonCount; ++i)
            {
                ZPointerEventData eventData = this.GetEventData(pointer, i);

                // Process button press/release events.
                if (pointer.GetButtonDown(i))
                {
                    this.ProcessButtonPress(eventData);
                }

                if (pointer.GetButtonUp(i))
                {
                    this.ProcessButtonRelease(eventData);
                }

                // Process move/scroll events only for the primary button.
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    this.ProcessMove(eventData);
                    this.ProcessScroll(eventData);
                }

                // Process drag events.
                this.ProcessDrag(eventData);
            }
        }

        protected ZPointerEventData GetEventData(ZPointer pointer, int buttonId)
        {
            int id = pointer.Id + buttonId;

            RaycastResult hitInfo = pointer.HitInfo;

            // Attempt to retrieve the pointer event data. If it doesn't exist,
            // create it.
            ZPointerEventData eventData = null;

            if (!this._eventDataCache.TryGetValue(id, out eventData))
            {
                eventData = new ZPointerEventData(this.eventSystem);
                eventData.position = hitInfo.screenPosition;

                this._eventDataCache.Add(id, eventData);
            }

            // Reset the pointer event data before populating it with latest 
            // information from the pointer.
            eventData.Reset();

            eventData.Pointer = pointer;
            eventData.ButtonId = buttonId;
            eventData.IsUIObject = 
                (hitInfo.gameObject?.GetComponent<RectTransform>() != null);
            eventData.Delta3D = 
                hitInfo.worldPosition - 
                eventData.pointerCurrentRaycast.worldPosition;

            eventData.button = pointer.GetButtonMapping(buttonId);
            eventData.delta = hitInfo.screenPosition - eventData.position;
            eventData.position = hitInfo.screenPosition;
            eventData.scrollDelta = pointer.ScrollDelta;
            eventData.pointerCurrentRaycast = hitInfo;

            return eventData;
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void ProcessButtonPress(ZPointerEventData eventData)
        {
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;

            eventData.eligibleForClick = true;
            eventData.delta = Vector2.zero;
            eventData.dragging = false;
            eventData.useDragThreshold = true;
            eventData.pressPosition = eventData.position;
            eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;

            this.DeselectIfSelectionChanged(hitObject, eventData);

            // Attempt to execute pointer down event.
            GameObject pressHandler = ExecuteEvents.ExecuteHierarchy(
                hitObject, eventData, ExecuteEvents.pointerDownHandler);

            // If a pointer down handler could not be found, attempt to
            // grab the hit object's pointer click handler as a fallback.
            if (pressHandler == null)
            {
                pressHandler = 
                    ExecuteEvents.GetEventHandler<IPointerClickHandler>(
                        hitObject);
            }

            // Determine the click count.
            float time = Time.unscaledTime;

            if (pressHandler == eventData.lastPress)
            {
                float timeSincePress = time - eventData.clickTime;

                eventData.clickCount = 
                    (timeSincePress < eventData.Pointer.ClickTimeThreshold) ? 
                        eventData.clickCount + 1 : 1;
            }
            else
            {
                eventData.clickCount = 1;
            }

            // Update the event data's press/click information.
            eventData.clickTime = time;
            eventData.rawPointerPress = hitObject;
            eventData.pointerPress = pressHandler;
            eventData.pointerDrag = 
                ExecuteEvents.GetEventHandler<IDragHandler>(hitObject);

            if (eventData.pointerDrag != null)
            {
                ExecuteEvents.Execute(
                    eventData.pointerDrag, 
                    eventData, 
                    ExecuteEvents.initializePotentialDrag);
            }
        }

        private void ProcessButtonRelease(ZPointerEventData eventData)
        {
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
            float timeSincePress = Time.unscaledTime - eventData.clickTime;

            // Execute pointer up event.
            ExecuteEvents.Execute(
                eventData.pointerPress, 
                eventData, 
                ExecuteEvents.pointerUpHandler);

            GameObject clickHandler =
                ExecuteEvents.GetEventHandler<IPointerClickHandler>(hitObject);

            // Execute pointer click and drop events.
            if (eventData.eligibleForClick && 
                (eventData.pointerPress == clickHandler ||
                 timeSincePress < eventData.Pointer.ClickTimeThreshold))
            {
                ExecuteEvents.Execute(
                    eventData.pointerPress,
                    eventData,
                    ExecuteEvents.pointerClickHandler);
            }
            else if (eventData.pointerDrag != null)
            {
                ExecuteEvents.ExecuteHierarchy(
                    hitObject, eventData, ExecuteEvents.dropHandler);
            }

            // Reset the event data's press/click information.
            eventData.eligibleForClick = false;
            eventData.pointerPress = null;
            eventData.rawPointerPress = null;

            // Execute end drag event.
            if (eventData.pointerDrag != null && eventData.dragging)
            {
                ExecuteEvents.Execute(
                    eventData.pointerDrag, 
                    eventData, 
                    ExecuteEvents.endDragHandler);
            }

            // Reset the event data's drag information.
            eventData.dragging = false;
            eventData.pointerDrag = null;

            // Redo pointer enter/exit to refresh state.
            if (hitObject != eventData.pointerEnter)
            {
                this.HandlePointerExitAndEnter(eventData, null);
                this.HandlePointerExitAndEnter(eventData, hitObject);
            }
        }

        // NOTE: The base ProcessDrag() implementation was originally
        //       being used until there was a need to determine if the input
        //       device's 3D position is changing as well as having the ability 
        //       to associate input scrolling with movement (e.g. scrolling 
        //       moves mouse input in the Z direction). So, reimplementing this 
        //       here to account for 3D movement and scrolling.
        private void ProcessDrag(ZPointerEventData eventData)
        {
            bool isPointerActive = 
                eventData.IsPointerMoving3D() || eventData.IsScrolling();

            bool shouldStartDrag =
                !eventData.IsUIObject ||
                !eventData.useDragThreshold ||
                this.ShouldStartDrag(
                    eventData.pressPosition,
                    eventData.position,
                    eventSystem.pixelDragThreshold);

            // Execute drag begin event.
            if (shouldStartDrag &&
                eventData.pointerDrag != null &&
                !eventData.dragging)
            {
                ExecuteEvents.Execute(
                    eventData.pointerDrag,
                    eventData,
                    ExecuteEvents.beginDragHandler);

                eventData.dragging = true;
            }

            // Execute drag event.
            if (eventData.dragging &&
                isPointerActive &&
                eventData.pointerDrag != null)
            {
                // Before performing a drag, cancel any pointer down state
                // and clear the current selection.
                if (eventData.pointerPress != eventData.pointerDrag)
                {
                    ExecuteEvents.Execute(
                        eventData.pointerPress,
                        eventData,
                        ExecuteEvents.pointerUpHandler);

                    eventData.eligibleForClick = false;
                    eventData.pointerPress = null;
                    eventData.rawPointerPress = null;
                }

                ExecuteEvents.Execute(
                    eventData.pointerDrag,
                    eventData,
                    ExecuteEvents.dragHandler);
            }
        }

        private void ProcessScroll(ZPointerEventData eventData)
        {
            if (!Mathf.Approximately(eventData.scrollDelta.sqrMagnitude, 0))
            {
                GameObject hitObject = 
                    eventData.pointerCurrentRaycast.gameObject;

                GameObject scrollHandler =
                    ExecuteEvents.GetEventHandler<IScrollHandler>(hitObject);

                ExecuteEvents.ExecuteHierarchy(
                    scrollHandler, eventData, ExecuteEvents.scrollHandler);
            }
        }

        private bool IsMousePointerActive()
        {
            IList<ZPointer> pointers = ZPointer.GetInstances();

            return pointers.Any(p => p is ZMouse);
        }

        private bool ShouldStartDrag(
            Vector2 pressPosition, Vector2 currentPosition, float threshold)
        {
            Vector2 deltaPosition = (pressPosition - currentPosition);

            return deltaPosition.sqrMagnitude >= (threshold * threshold);
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private Dictionary<int, ZPointerEventData> _eventDataCache =
            new Dictionary<int, ZPointerEventData>();
    }
}

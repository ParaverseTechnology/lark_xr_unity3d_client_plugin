////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using zSpace.Core.Extensions;

namespace zSpace.Core.Input
{
    [DefaultExecutionOrder(ScriptPriority)]
    public abstract partial class ZPointer : MonoBehaviour
    {
        public const int ScriptPriority = ZProvider.ScriptPriority + 30;

        ////////////////////////////////////////////////////////////////////////
        // Public Types
        ////////////////////////////////////////////////////////////////////////

        public enum CollisionPlane
        {
            None = 0,
            Screen = 1,
        }

        public enum DragPolicy
        {
            None = 0,
            LockHitPosition = 1,
            LockToSurfaceAlignedPlane = 2,
            LockToScreenAlignedPlane = 3,
            LockToCustomPlane = 4,
        }

        [Serializable]
        public class CollisionEvent : UnityEvent<ZPointer, GameObject>
        {
        }

        [Serializable]
        public class IntEvent : UnityEvent<ZPointer, int>
        {
        }

        ////////////////////////////////////////////////////////////////////////
        // Inspector Fields
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The camera that will be used when calculating the pointer's
        /// world space pose as well as to process pointer events.
        /// </summary>
        [Tooltip(
            "The camera that will be used when calculating the pointer's " +
            "world space pose as well as to process pointer events.")]
        public ZCamera EventCamera = null;

        /// <summary>
        /// The visualization to be processed by the pointer.
        /// </summary>
        [Tooltip("The visualization to be processed by the pointer.")]
        public ZPointerVisualization Visualization = null;

        [Header("Collision")]

        /// <summary>
        /// The maximum hit distance in meters.
        /// </summary>
        [Tooltip("The maximum hit distance in meters.")]
        public float MaxHitDistance = 1.0f;

        /// <summary>
        /// The maximum hit radius in meters.
        /// </summary>
        [Tooltip("The maximum hit radius in meters.")]
        [Range(0, 0.1f)]
        public float MaxHitRadius = 0.0f;

        /// <summary>
        /// The mask controlling which layers the pointer will ignore. All
        /// objects on the specified ignore layers will not receive any
        /// pointer events.
        /// </summary>
        [Tooltip(
            "The mask controlling which layers the pointer will ignore. " +
            "All objects on the specified ignore layers will not receive " +
            "any pointer events.")]
        public LayerMask IgnoreMask = 0;

        /// <summary>
        /// A mask specifying which objects take priority when snapping.
        /// </summary>
        [Tooltip("A mask specifying which objects take priority when snapping.")]
        public LayerMask PriorityMask = 0;

        /// <summary>
        /// Enable whether the pointer will attempt to intersect with a 
        /// collision plane (e.g. screen plane) if it is not intersecting 
        /// with UI or in-scene objects.
        /// </summary>
        /// 
        /// <remarks>
        /// This feature is useful for pointers such as the mouse in order
        /// ensure the mouse cursor is bound to the screen plane by default
        /// when it is not intersecting with UI or in-scene objects.
        /// </remarks>
        [Tooltip(
            "Enable whether the pointer will attempt to intersect with a " +
            "collision plane (e.g. screen plane) if it is not intersecting " +
            "with UI or in-scene objects.")]
        public CollisionPlane DefaultCollisionPlane = CollisionPlane.None;

        [Header("Drag")]

        /// <summary>
        /// The drag policy to be used when no object is intersected.
        /// </summary>
        [Tooltip("The drag policy to be used when no object is intersected.")]
        public DragPolicy DefaultDragPolicy = DragPolicy.None;

        /// <summary>
        /// The drag policy to be used by default for non-UI objects.
        /// </summary>
        [Tooltip("The drag policy to be used by default for non-UI objects.")]
        public DragPolicy ObjectDragPolicy = DragPolicy.LockHitPosition;

        /// <summary>
        /// The drag policy to be used by default for UI objects.
        /// </summary>
        [Tooltip("The drag policy to be used by default for UI objects.")]
        public DragPolicy UIDragPolicy = DragPolicy.LockToSurfaceAlignedPlane;

        /// <summary>
        /// The time threshold in seconds to differentiate between a click
        /// and drag. If the elapsed time between button press and release
        /// is less than the threshold, the action is interpreted as a 
        /// click. Otherwise it is interpreted as a drag.
        /// </summary>
        [Tooltip(
            "The time threshold in seconds to differentiate between a click " +
            "and drag. If the elapsed time between button press and release " +
            "is less than the threshold, the action is interpreted as a " +
            "click. Otherwise it is interpreted as a drag.")]
        public float ClickTimeThreshold = 0.3f;

        /// <summary>
        /// The conversion factor to convert scroll units to meters.
        /// </summary>
        [Tooltip("The conversion factor to convert scroll units to meters.")]
        public float ScrollMetersPerUnit = 0.01f;

        [Header("Events")]

        /// <summary>
        /// Event dispatched when the pointer enters an object.
        /// </summary>
        [Tooltip("Event dispatched when the pointer enters an object.")]
        public CollisionEvent OnObjectEntered = new CollisionEvent();

        /// <summary>
        /// Event dispatched when the pointer exits an object.
        /// </summary>
        [Tooltip("Event dispatched when the pointer exits an object.")]
        public CollisionEvent OnObjectExited = new CollisionEvent();

        /// <summary>
        /// Event dispatched when a pointer button becomes pressed.
        /// </summary>
        [Tooltip("Event dispatched when a pointer button becomes pressed.")]
        public IntEvent OnButtonPressed = new IntEvent();

        /// <summary>
        /// Event dispatched when a pointer button becomes released.
        /// </summary>
        [Tooltip("Event dispatched when a pointer button becomes released.")]
        public IntEvent OnButtonReleased = new IntEvent();

        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        protected virtual void Awake()
        {
            // Initialize hit info.
            this._hitInfo.distance = this.DefaultHitDistance;

            this._hitInfo.worldPosition =
                this.transform.position + 
                (this.transform.forward * this.DefaultHitDistance);

            this._hitInfo.worldNormal = -this.transform.forward;
        }

        protected virtual void Start()
        {
            if (this.EventCamera == null)
            {
                Debug.LogWarningFormat(
                    this,
                    "The <color=cyan>{0}</color> pointer will not be " +
                    "processed since no event camera is attached. " +
                    "Please make sure you have attached a valid event camera " +
                    "to enable pointer processing.",
                    this.name);
            }

            this._buttonState = new ButtonState[this.ButtonCount];
        }

        protected virtual void OnEnable()
        {
            if (!s_instances.Contains(this))
            {
                s_instances.Add(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (s_instances.Contains(this))
            {
                s_instances.Remove(this);
            }
        }

        protected virtual void Update()
        {
            this.Process();
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The unique id of the pointer.
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// The number of buttons supported by the pointer.
        /// </summary>
        public abstract int ButtonCount { get; }

        /// <summary>
        /// The current scroll delta of the pointer.
        /// </summary>
        public abstract Vector2 ScrollDelta { get; }

        /// <summary>
        /// The current visibility state of the pointer.
        /// </summary>
        public abstract bool IsVisible { get; }

        /// <summary>
        /// The pose of the pointer's current end point in world space.
        /// </summary>
        public virtual Pose EndPointWorldPose => new Pose(
            this.HitInfo.worldPosition, this.transform.rotation);

        /// <summary>
        /// The current hit information of the pointer.
        /// </summary>
        public RaycastResult HitInfo => this._hitInfo;

        /// <summary>
        /// The hit information corresponding to when any button is pressed
        /// to initiate a drag.
        /// </summary>
        public RaycastResult PressHitInfo => this._pressHitInfo;

        /// <summary>
        /// The world ray based on the pointer's current position and rotation.
        /// </summary>
        public Ray PointerRay => this.transform.ToRay();

        /// <summary>
        /// Checks whether any pointer button is currently pressed.
        /// </summary>
        public bool AnyButtonPressed => (this._dragButtonId != -1);

        /// <summary>
        /// A callback to override the default drag plane.
        /// </summary>
        public Func<ZPointer, Plane> DefaultCustomDragPlane { get; set; }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets all active pointer instances in the current scene.
        /// </summary>
        /// 
        /// <remarks>
        /// This is a convenience method for any logic that needs to 
        /// quickly iterate through all active pointers in the scene
        /// (e.g. ZInputModule).
        /// </remarks>
        public static IList<ZPointer> GetInstances()
        {
            return s_instances;
        }

        /// <summary>
        /// Gets whether the specified button is currently in a pressed state.
        /// </summary>
        /// 
        /// <param name="id">
        /// The id of the button.
        /// </param>
        /// 
        /// <returns>
        /// True if the specified button is pressed. False otherwise.
        /// </returns>
        public abstract bool GetButton(int id);

        /// <summary>
        /// Gets whether the specified button became pressed this frame.
        /// </summary>
        /// 
        /// <param name="id">
        /// The id of the button.
        /// </param>
        /// 
        /// <returns>
        /// True if the specified button became pressed. False otherwise.
        /// </returns>
        public bool GetButtonDown(int id)
        {
            return this._buttonState[id].BecamePressed;
        }

        /// <summary>
        /// Gets whether the specified button became released this frame.
        /// </summary>
        /// 
        /// <param name="id">
        /// The id of the button.
        /// </param>
        /// 
        /// <returns>
        /// True if the specified button became released. False otherwise.
        /// </returns>
        public bool GetButtonUp(int id)
        {
            return this._buttonState[id].BecameReleased;
        }

        /// <summary>
        /// Returns the appropriate Unity PointerEventData.InputButton based
        /// on a specified integer button id.
        /// </summary>
        /// 
        /// <param name="id">
        /// The integer button id to retrieve the InputButton for.
        /// </param>
        /// 
        /// <returns>
        /// The InputButton associated with the specified integer button id.
        /// </returns>
        public PointerEventData.InputButton GetButtonMapping(int id)
        {
            switch (id)
            {
                case 1:
                    return PointerEventData.InputButton.Right;
                    
                case 2:
                    return PointerEventData.InputButton.Middle;
                    
                case 0:
                default:
                    return PointerEventData.InputButton.Left;
            }
        }

        /// <summary>
        /// Allows a specified object to capture pointer events.
        /// </summary>
        /// 
        /// <remarks>
        /// To disable pointer event capture, call this method and pass
        /// in null for the capture object.
        /// </remarks>
        /// 
        /// <param name="captureObject">
        /// A reference to the GameObject responsible for capturing pointer
        /// events.
        /// </param>
        public void CapturePointer(GameObject captureObject)
        {
            this._captureObject = captureObject;
        }

        ////////////////////////////////////////////////////////////////////////
        // Protected Methods
        ////////////////////////////////////////////////////////////////////////

        protected abstract Pose ComputeWorldPose();

        ////////////////////////////////////////////////////////////////////////
        // Private Properties
        ////////////////////////////////////////////////////////////////////////

        private Matrix4x4 ScreenWorldPoseMatrix =>
            this.EventCamera.ZeroParallaxLocalToWorldMatrix.ToPoseMatrix();

        private Matrix4x4 DeltaScreenWorldPoseMatrix =>
            this.ScreenWorldPoseMatrix * 
            this._pressScreenWorldPoseMatrix.inverse;

        private float WorldScale => this.EventCamera?.WorldScale.z ?? 1;

        private float DefaultHitDistance =>
            this.MaxHitDistance * this.WorldScale;

        private float DefaultHitRadius =>
            this.MaxHitRadius * this.WorldScale;

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void Process()
        {
            if (this.EventCamera == null)
            {
                return;
            }

            // Process the pointer.
            if (this.IsVisible)
            {
                this.ProcessMove();
                this.ProcessButtonState();
                this.ProcessScroll();
                this.ProcessCollisions();

                this.SendEvents();
            }
            
            // Process the pointer's associated visualization.
            if (this.Visualization != null)
            {
                this.Visualization.Process(this, this.EventCamera.WorldScale);
            }
        }

        private void ProcessMove()
        {
            Pose worldPose = this.ComputeWorldPose();

            this.transform.SetPose(worldPose);
        }

        private void ProcessButtonState()
        {
            int buttonCount = this.ButtonCount;

            for (int i = 0; i < buttonCount; ++i)
            {
                bool isPressed = this.GetButton(i);
                bool wasPressed = this._buttonState[i].IsPressed;

                this._buttonState[i].BecamePressed = (isPressed && !wasPressed);
                this._buttonState[i].BecameReleased = (!isPressed && wasPressed);
                this._buttonState[i].IsPressed = isPressed;

                if (this._buttonState[i].BecamePressed)
                {
                    this.ProcessButtonPress(i);
                }

                if (this._buttonState[i].BecameReleased)
                {
                    this.ProcessButtonRelease(i);
                }
            }
        }

        private void ProcessButtonPress(int buttonId)
        {
            if (this._dragButtonId == -1)
            {
                this._dragButtonId = buttonId;
                this._dragScrollDistance = 0.0f;

                this._pressHitInfo = this.Raycast(this.PointerRay);
                this._pressObject = this._pressHitInfo.gameObject;
                this._pressInteractable = 
                    this._pressObject?.GetComponent<ZPointerInteractable>();

                this._pressScreenWorldPoseMatrix = this.ScreenWorldPoseMatrix;

                this._pressScreenWorldNormal = Vector3.Normalize(
                    this._pressScreenWorldPoseMatrix.MultiplyVector(
                        Vector3.back));

                this._pressLocalHitPosition =
                    this.transform.worldToLocalMatrix.MultiplyPoint(
                        this._pressHitInfo.worldPosition);

                this._pressLocalHitNormal = Vector3.Normalize(
                    this.transform.worldToLocalMatrix.MultiplyVector(
                        this._pressHitInfo.worldNormal));
                
                this._pressLocalHitDirection = Vector3.Normalize(
                    this.transform.worldToLocalMatrix.MultiplyVector(
                        this._pressHitInfo.worldPosition - 
                        this.transform.position));

                this._pressDragPolicy = this.GetDragPolicy(this._pressObject);
            }
        }

        private void ProcessButtonRelease(int buttonId)
        {
            if (this._dragButtonId == buttonId)
            {
                this._dragButtonId = -1;
            }
        }

        private void ProcessScroll()
        {
            if (this.AnyButtonPressed)
            {
                float scrollDelta = this.ScrollDelta.y;

                this._dragScrollDistance +=
                    scrollDelta * this.ScrollMetersPerUnit * this.WorldScale;

                if (this._dragScrollDistance != 0)
                {
                    // Clamp the scroll distance based the pointer's distance
                    // from the hit plane captured on button press.
                    Plane hitPlane = new Plane(
                        this._pressScreenWorldNormal, 
                        this._pressHitInfo.worldPosition);

                    hitPlane = this.DeltaScreenWorldPoseMatrix.TransformPlane(
                        hitPlane);

                    float minScrollDistance = -hitPlane.GetDistanceToPoint(
                        this.transform.position);

                    this._dragScrollDistance = Mathf.Max(
                        minScrollDistance + 0.01f, this._dragScrollDistance);
                }
            }
        }

        private void ProcessCollisions()
        {
            // Perform a physics and graphics raycast to determine if
            // the pointer is intersecting anything.
            RaycastResult hitInfo = this.Raycast(this.PointerRay);

            // If any key is currently being pressed, update the
            // resultant hit info in case the hit position is being 
            // constrained based on the current drag mode policy.
            if (this.AnyButtonPressed)
            {
                this.ProcessDrag(ref hitInfo);
            }

            // Check if the pointer was captured by an object. If so,
            // make sure all events are forwarded to the capture object.
            if (this._captureObject != null)
            {
                hitInfo.gameObject = this._captureObject;
            }

            // Update the cached entered object.
            this._enteredObject = null;

            if (hitInfo.gameObject != null &&
                hitInfo.gameObject != this._hitInfo.gameObject)
            {
                this._enteredObject = hitInfo.gameObject;
            }

            // Update the cached exited object.
            this._exitedObject = null;

            if (this._hitInfo.gameObject != null &&
                this._hitInfo.gameObject != hitInfo.gameObject)
            {
                this._exitedObject = this._hitInfo.gameObject;
            }

            this._hitInfo = hitInfo;
        }

        private void ProcessDrag(ref RaycastResult hitInfo)
        {
            // If the current hit info's object is not equal to
            // the current drag object, clear it so that objects
            // other than the current drag object won't receive
            // events.
            if (hitInfo.gameObject != this._pressHitInfo.gameObject)
            {
                hitInfo.gameObject = null;
            }

            // Update the hit info based on the current drag policy.
            switch (this._pressDragPolicy)
            {
                case DragPolicy.LockHitPosition:
                    this.ProcessDragLockHitPosition(ref hitInfo);
                    break;

                case DragPolicy.LockToSurfaceAlignedPlane:
                    this.ProcessDragLockToSurfacePlane(ref hitInfo);
                    break;

                case DragPolicy.LockToScreenAlignedPlane:
                    this.ProcessDragLockToScreenPlane(ref hitInfo);
                    break;

                case DragPolicy.LockToCustomPlane:
                    this.ProcessDragLockToCustomPlane(ref hitInfo);
                    break;
            }
        }

        private void ProcessDragLockHitPosition(ref RaycastResult hitInfo)
        {
            hitInfo.distance = this._pressHitInfo.distance;

            hitInfo.worldPosition =
                this.transform.localToWorldMatrix.MultiplyPoint(
                    this._pressLocalHitPosition);

            hitInfo.worldNormal = Vector3.Normalize(
                this.transform.localToWorldMatrix.MultiplyVector(
                    this._pressLocalHitNormal));

            hitInfo.screenPosition =
                this.EventCamera.Camera.WorldToScreenPoint(
                    hitInfo.worldPosition);
        }

        private void ProcessDragLockToSurfacePlane(ref RaycastResult hitInfo)
        {
            Vector3 normal = this._pressHitInfo.worldNormal;
            Vector3 position = this._pressHitInfo.worldPosition;
            Plane dragPlane = this.DeltaScreenWorldPoseMatrix.TransformPlane(
                new Plane(normal, position));

            this.ProcessDragLockToPlane(dragPlane, ref hitInfo);
        }

        private void ProcessDragLockToScreenPlane(ref RaycastResult hitInfo)
        {
            Vector3 normal = this._pressScreenWorldNormal;
            Vector3 scrollOffset = (-normal * this._dragScrollDistance);
            Vector3 position = this._pressHitInfo.worldPosition + scrollOffset;
            Plane dragPlane = this.DeltaScreenWorldPoseMatrix.TransformPlane(
                new Plane(normal, position));

            this.ProcessDragLockToPlane(dragPlane, ref hitInfo);
        }

        private void ProcessDragLockToCustomPlane(ref RaycastResult hitInfo)
        {
            Plane dragPlane = default(Plane);

            if (this._pressInteractable != null)
            {
                dragPlane = this._pressInteractable.GetDragPlane(this);
            }
            else if (this.DefaultCustomDragPlane != null)
            {
                dragPlane = this.DefaultCustomDragPlane(this);
            }

            this.ProcessDragLockToPlane(dragPlane, ref hitInfo);
        }

        private void ProcessDragLockToPlane(
            Plane plane, ref RaycastResult hitInfo)
        {
            // Compute the ray.
            Vector3 direction = this.transform.localToWorldMatrix.MultiplyVector(
                this._pressLocalHitDirection);

            Ray ray = new Ray(this.transform.position, direction);

            // Perform a raycast against the drag plane.
            RaycastResult result = this.Raycast(ray, plane, true);

            // Update the hit info.
            hitInfo.distance = result.distance;
            hitInfo.worldPosition = result.worldPosition;
            hitInfo.worldNormal = result.worldNormal;
            hitInfo.screenPosition = result.screenPosition;
        }

        private void SendEvents()
        {
            // Send collision events.
            if (this._exitedObject != null)
            {
                this.OnObjectExited.Invoke(this, this._exitedObject);
            }

            if (this._enteredObject != null)
            {
                this.OnObjectEntered.Invoke(this, this._enteredObject);
            }

            // Send button events.
            int buttonCount = this.ButtonCount;

            for (int i = 0; i < buttonCount; ++i)
            {
                if (this._buttonState[i].BecamePressed)
                {
                    this.OnButtonPressed.Invoke(this, i);
                }

                if (this._buttonState[i].BecameReleased)
                {
                    this.OnButtonReleased.Invoke(this, i);
                }
            }
        }

        private DragPolicy GetDragPolicy(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return this.DefaultDragPolicy;
            }

            ZPointerInteractable interactable = 
                gameObject.GetComponent<ZPointerInteractable>();

            if (interactable != null)
            {
                return interactable.GetDragPolicy(this);
            }
            else if (gameObject.GetComponent<RectTransform>() != null)
            {
                return this.UIDragPolicy;
            }
            else
            {
                return this.ObjectDragPolicy;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Types
        ////////////////////////////////////////////////////////////////////////

        private struct ButtonState
        {
            public bool IsPressed;
            public bool BecamePressed;
            public bool BecameReleased;
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private const int MaxButtonCount = 10;

        private static readonly List<ZPointer> s_instances = 
            new List<ZPointer>();

        private ButtonState[] _buttonState = new ButtonState[MaxButtonCount];

        private RaycastResult _hitInfo;
        private GameObject _captureObject = null;
        private GameObject _enteredObject = null;
        private GameObject _exitedObject = null;

        private int _dragButtonId = -1;
        private float _dragScrollDistance = 0.0f;

        private RaycastResult _pressHitInfo;
        private GameObject _pressObject = null;
        private ZPointerInteractable _pressInteractable = null;
        private Matrix4x4 _pressScreenWorldPoseMatrix;
        private Vector3 _pressScreenWorldNormal;
        private Vector3 _pressLocalHitPosition;
        private Vector3 _pressLocalHitNormal;
        private Vector3 _pressLocalHitDirection;
        private DragPolicy _pressDragPolicy = DragPolicy.None;
    }
}

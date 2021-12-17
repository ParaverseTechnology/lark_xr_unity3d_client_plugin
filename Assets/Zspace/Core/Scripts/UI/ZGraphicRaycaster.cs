////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using zSpace.Core.EventSystems;

namespace zSpace.Core.UI
{
    [RequireComponent(typeof(Canvas))]
    public class ZGraphicRaycaster : GraphicRaycaster
    {
        ////////////////////////////////////////////////////////////////////////
        // MonoBehaviour Callbacks
        ////////////////////////////////////////////////////////////////////////

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!s_instances.Contains(this))
            {
                s_instances.Add(this);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (s_instances.Contains(this))
            {
                s_instances.Remove(this);
            }
        }

        protected override void Start()
        {
            base.Start();

            this._sortBySortingLayer = SortingLayer.layers.Length > 1;

            if (this.Canvas.renderMode == RenderMode.WorldSpace &&
                this.Canvas.worldCamera == null)
            {
                Debug.LogWarning(
                    "No Event Camera found attached to associated world " +
                    "space canvas. Please make sure to assign an appropriate " +
                    "camera to your canvas to minimize performance impact " +
                    "and ensure raycasts are performed correctly.",
                    this);
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Properties
        ////////////////////////////////////////////////////////////////////////

        public Canvas Canvas
        {
            get
            {
                if (this._canvas == null)
                {
                    this._canvas = this.GetComponent<Canvas>();
                }

                return this._canvas;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a list of all enabled ZGraphicRaycaster instances in the scene.
        /// </summary>
        /// 
        /// <returns>
        /// The list of all enabled ZGraphicRaycasters instances in the scene.
        /// </returns>
        public static IList<ZGraphicRaycaster> GetRaycasters()
        {
            return s_instances;
        }

        /// <summary>
        /// Performs a raycast against all enabled ZGraphicRaycaster instances
        /// in the scene and reports the closest hit.
        /// </summary>
        /// 
        /// <param name="ray">
        /// The starting point and direction of the ray.
        /// </param>
        /// <param name="result">
        /// The raycast result corresponding to the closest hit.
        /// </param>
        /// <param name="maxDistance">
        /// The maximum distance that the hit result is allowed to be from
        /// the start of the ray.
        /// </param>
        /// <param name="layerMask">
        /// A layer mask that is used to selectively ignore graphics when 
        /// casting the ray.
        /// </param>
        /// 
        /// <returns>
        /// True if a graphic was hit. False otherwise.
        /// </returns>
        public static bool Raycast(
            Ray ray, out RaycastResult result, float maxDistance, int layerMask)
        {
            s_results.Clear();

            for (int i = 0; i < s_instances.Count; ++i)
            {
                s_instances[i].Raycast(ray, s_results, maxDistance, layerMask);
            }

            if (s_results.Count > 0)
            {
                result = s_results[0];
                return true;
            }

            result = default(RaycastResult);
            return false;
        }

        /// <summary>
        /// Performs a raycast against all enabled ZGraphicRaycaster instances
        /// in the scene and reports all hits.
        /// </summary>
        /// 
        /// <param name="ray">
        /// The starting point and direction of the ray.
        /// </param>
        /// <param name="resultAppendList">
        /// The raycast results corresponding to all hits.
        /// </param>
        /// <param name="maxDistance">
        /// The maximum distance that a hit result is allowed to be from
        /// the start of the ray.
        /// </param>
        /// <param name="layerMask">
        /// A layer mask that is used to selectively ignore graphics when 
        /// casting the ray.
        /// </param>
        /// 
        /// <returns>
        /// True if a graphic was hit. False otherwise.
        /// </returns>
        public static void RaycastAll(
            Ray ray,
            List<RaycastResult> resultAppendList,
            float maxDistance,
            int layerMask)
        {
            for (int i = 0; i < s_instances.Count; ++i)
            {
                s_instances[i].Raycast(
                    ray, resultAppendList, maxDistance, layerMask);
            }
        }

        public override void Raycast(
            PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            ZPointerEventData e = eventData as ZPointerEventData;

            if (e != null)
            {
                this.Raycast(
                    e.Pointer.PointerRay, 
                    resultAppendList, 
                    float.PositiveInfinity, 
                    ~0);
            }
            else
            {
                base.Raycast(eventData, resultAppendList);
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void Raycast(
            Ray ray,
            List<RaycastResult> resultAppendList,
            float maxDistance, 
            int layerMask)
        {
            // Potentially reduce the maximum hit distance based on whether
            // any 2D or 3D blocking objects have been intersected.
            float distance =
                this.eventCamera.farClipPlane - this.eventCamera.nearClipPlane;

            if (this.blockingObjects == BlockingObjects.ThreeD ||
                this.blockingObjects == BlockingObjects.All)
            {
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, distance, this.m_BlockingMask))
                {
                    maxDistance = Mathf.Min(hit.distance, maxDistance);
                }
            }

            if (this.blockingObjects == BlockingObjects.TwoD ||
                this.blockingObjects == BlockingObjects.All)
            {
                RaycastHit2D hit = Physics2D.GetRayIntersection(
                    ray, distance, this.m_BlockingMask);

                if (hit.collider != null)
                {
                    maxDistance = Mathf.Min(
                        hit.fraction * distance, maxDistance);
                }
            }

            // Retrieve the list of graphics associated with the canvas.
            IList<Graphic> graphics = 
                GraphicRegistry.GetGraphicsForCanvas(this.Canvas);

            // Iterate through each of graphics and perform hit tests.
            for (int i = 0; i < graphics.Count; ++i)
            {
                Graphic graphic = graphics[i];

                // Skip the graphic if it's not in the layer mask.
                if (((1 << graphic.gameObject.layer) & layerMask) == 0)
                {
                    continue;
                }

                // Perform a raycast against the graphic.
                RaycastResult result;
                if (this.Raycast(ray, graphic, out result, maxDistance))
                {
                    resultAppendList.Add(result);
                }
            }

            // Sort the results by depth.
            resultAppendList.Sort((x, y) => y.depth.CompareTo(x.depth));

            // Sort the results by sortingOrder.
            resultAppendList.Sort((x, y) =>
                y.sortingOrder.CompareTo(x.sortingOrder));

            // Sort the results by sortingLayer
            if (this._sortBySortingLayer)
            {
                resultAppendList.Sort(
                    (x, y) => y.sortingLayer.CompareTo(x.sortingLayer));
            }
        }

        private bool Raycast(
            Ray ray,
            Graphic graphic,
            out RaycastResult result,
            float maxDistance)
        {
            result = default(RaycastResult);

            // Skip graphics that aren't raycast targets.
            if (!graphic.raycastTarget)
            {
                return false;
            }

            // Skip graphics that aren't being drawn.
            if (graphic.depth == -1)
            {
                return false;
            }

            // Skip graphics that are reversed if the ignore reversed
            // graphics inspector field is enabled.
            if (this.ignoreReversedGraphics &&
                Vector3.Dot(ray.direction, -graphic.transform.forward) > 0)
            {
                return false;
            }

            // Create a plane based on the graphic's transform.
            Plane plane = new Plane(
                -graphic.transform.forward, graphic.transform.position);

            // Skip graphics that failed the plane intersection test.
            float distance = 0.0f;
            if (!plane.Raycast(ray, out distance))
            {
                return false;
            }

            // Skip graphics that are further away than the max distance.
            if (distance > maxDistance)
            {
                return false;
            }

            Vector3 worldPosition = 
                ray.origin + (ray.direction * distance);

            Vector3 screenPosition =
                this.eventCamera.WorldToScreenPoint(worldPosition);            

            // Skip graphics that have failed the bounds test.
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    graphic.rectTransform, screenPosition, this.eventCamera))
            {
                return false;
            }

            // Skip graphics that fail the raycast test.
            // NOTE: This is necessary to ensure that raycasts against
            //       masked out areas of the graphic are correctly ignored.
            if (!graphic.Raycast(screenPosition, this.eventCamera))
            {
                return false;
            }

            result.depth = graphic.depth;
            result.distance = distance;
            result.worldPosition = worldPosition;
            result.worldNormal = plane.normal;
            result.screenPosition = screenPosition;
            result.gameObject = graphic.gameObject;
            result.sortingLayer = graphic.canvas.cachedSortingLayerValue;
            result.sortingOrder = graphic.canvas.sortingOrder;
            result.module = this;

            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private bool _sortBySortingLayer = false;

        private static readonly List<ZGraphicRaycaster> s_instances =
            new List<ZGraphicRaycaster>();

        private static readonly List<RaycastResult> s_results =
            new List<RaycastResult>();

        private Canvas _canvas = null;
    }
}

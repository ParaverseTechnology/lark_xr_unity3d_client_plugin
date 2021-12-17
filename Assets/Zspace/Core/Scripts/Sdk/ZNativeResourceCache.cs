////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace zSpace.Core.Sdk
{
    public abstract class ZNativeResourceCache<T> where T : ZNativeResource
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Clears the internal cache of resources.
        /// </summary>
        public void ClearCache()
        {
            this._cache.Clear();
        }

        ////////////////////////////////////////////////////////////////////////
        // Protected Methods
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Adds a resource to the cache.
        /// </summary>
        /// 
        /// <param name="resource">
        /// The resource to be added to the cache.
        /// </param>
        protected void AddToCache(ZNativeResource resource)
        {
            this._cache.Add(resource.NativePtr, resource);
        }

        /// <summary>
        /// Removes a resource from the cache based on its specified
        /// native pointer.
        /// </summary>
        /// 
        /// <param name="nativePtr">
        /// The native pointer corresponding to the resource to be removed.
        /// </param>
        protected void RemoveFromCache(IntPtr nativePtr)
        {
            this._cache.Remove(nativePtr);
        }

        /// <summary>
        /// Remove a resource from the cache.
        /// </summary>
        /// 
        /// <param name="resource">
        /// A reference to the resource to be removed.
        /// </param>
        protected void RemoveFromCache(ZNativeResource resource)
        {
            this._cache.Remove(resource.NativePtr);
        }

        /// <summary>
        /// Gets a resource from the cache based on a specified native 
        /// pointer.
        /// </summary>
        /// 
        /// <param name="nativePtr">
        /// The native pointer corresponding to the resource to retrieve.
        /// </param>
        /// 
        /// <returns>
        /// A reference to a resource based on its corresponding native 
        /// pointer.
        /// </returns>
        protected T GetCachedResource(IntPtr nativePtr)
        {
            if (nativePtr == IntPtr.Zero)
            {
                return null;
            }

            ZNativeResource resource = null;
            if (!this._cache.TryGetValue(nativePtr, out resource))
            {
                return null;
            }

            return (resource as T);
        }

        /// <summary>
        /// Get a resource from the cache if it exists. Otherwise, create
        /// and return a new resource and add it to the cache.
        /// </summary>
        /// 
        /// <param name="nativePtr">
        /// The native pointer corresponding to the resource to create.
        /// </param>
        /// <param name="createFunc">
        /// Callback function used to create the resource if it doesn't
        /// already exist in the cache.
        /// </param>
        /// 
        /// <returns>
        /// The existing or newly created resource.
        /// </returns>
        protected T GetOrCreateCachedResource(
            IntPtr nativePtr, Func<IntPtr, T> createFunc)
        {
            if (nativePtr == IntPtr.Zero)
            {
                return null;
            }

            T resource = this.GetCachedResource(nativePtr);
            if (resource == null)
            {
                resource = createFunc(nativePtr);
                this.AddToCache(resource);
            }

            return resource;
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private Dictionary<IntPtr, ZNativeResource> _cache =
            new Dictionary<IntPtr, ZNativeResource>();
    }
}


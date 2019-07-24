﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Cflashshoft.Framework.Optimization
{
    /// <summary>
    /// Extension methods for the .NET MemoryCache class.
    /// </summary>
    public static class MemoryCacheExtensions
    {
        private static NamedSemaphoreSlimLockFactory _namedLocks = null;
        private static CacheEntryRemovedCallback _removedCallback = null;

        static MemoryCacheExtensions()
        {
            _namedLocks = new NamedSemaphoreSlimLockFactory();
            _removedCallback = new CacheEntryRemovedCallback(OnRemove);
        }

        /// <summary>
        /// Return an item from the memory cache or insert it using the provided function.
        /// </summary>
        public static object SyncedGetOrSet(this MemoryCache cache, string key, Func<object> getValue, int expirationSeconds = 0, string regionName = null)
        {
            return SyncedGetOrSet(cache, key, getValue, expirationSeconds > 0 ? DateTime.Now.AddSeconds(expirationSeconds) : DateTimeOffset.MinValue, regionName);
        }

        /// <summary>
        /// Return an item from the memory cache or insert it using the provided function.
        /// </summary>
        public static object SyncedGetOrSet(this MemoryCache cache, string key, Func<object> getValue, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            object result = cache.Get(key);

            if (result == null)
            {
                var keyLock = _namedLocks.Get($"{cache.Name}_{key}");

                try
                {
                    keyLock.Wait();

                    result = cache.Get(key);

                    if (result == null)
                    {
                        result = getValue();

                        Set(cache, key, result, absoluteExpiration, regionName);
                    }
                }
                finally
                {
                    keyLock.Release();
                }
            }

            return result;
        }

        /// <summary>
        /// Return an item from the memory cache or insert it using the provided function.
        /// </summary>
        public static Task<object> SyncedGetOrSetAsync(this MemoryCache cache, string key, Func<Task<object>> getValue, int expirationSeconds = 0, string regionName = null)
        {
            return SyncedGetOrSetAsync(cache, key, getValue, expirationSeconds > 0 ? DateTime.Now.AddSeconds(expirationSeconds) : DateTimeOffset.MinValue, regionName);
        }

        /// <summary>
        /// Return an item from the memory cache or insert it using the provided function.
        /// </summary>
        public static async Task<object> SyncedGetOrSetAsync(this MemoryCache cache, string key, Func<Task<object>> getValue, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            object result = cache.Get(key);

            if (result == null)
            {
                var keyLock = _namedLocks.Get($"{cache.Name}_{key}");

                try
                {
                    await keyLock.WaitAsync();

                    result = cache.Get(key);

                    if (result == null)
                    {
                        result = await getValue();

                        Set(cache, key, result, absoluteExpiration, regionName);
                    }
                }
                finally
                {
                    keyLock.Release();
                }
            }

            return result;
        }

        private static void Set(MemoryCache cache, string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            CacheItemPolicy cacheItemPolicy = null;

            if (absoluteExpiration > DateTimeOffset.MinValue)
            {
                cacheItemPolicy = new CacheItemPolicy() { AbsoluteExpiration = absoluteExpiration };
            }
            else
            {
                cacheItemPolicy = new CacheItemPolicy();
            }

            cacheItemPolicy.RemovedCallback = _removedCallback;

            cache.Set(key, value, cacheItemPolicy, regionName);
        }

        private static void OnRemove(CacheEntryRemovedArguments args)
        {
            _namedLocks.Remove($"{args.Source.Name}_{args.CacheItem.Key}");
        }
    }
}

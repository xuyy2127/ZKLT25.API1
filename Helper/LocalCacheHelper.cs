using System.Collections.Concurrent;

namespace ZKLT25.API.Helper
{
    public class LocalCacheHelper
    {
        private static ConcurrentDictionary<string, string> localCache = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetKeyValue(string key)
        {
            if (localCache.TryGetValue(key, out var val))
            {
                return val;
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetKeyValue(string key, string value)
        {
            if (localCache.ContainsKey(key))
            {
                localCache[key] = value;
                return true;
            }

            var res = localCache.TryAdd(key, value);
            if (!res)
            {
                return Remove(key);
            }
            return true;
        }

        /// <summary>
        /// 删除key
        /// </summary>
        public static bool Remove(string key)
        {
            var res = localCache.TryRemove(key, out _);
            if (!res)
            {
                Clear();
            }
            return true;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public static void Clear()
        {
            localCache.Clear();
        }
    }
}

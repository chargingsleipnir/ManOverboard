using System;
using System.Collections.Generic;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the dictionary class
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Tries to retrieve the value stored at the specified key. If
        /// failed, defaults the value and returns it
        /// </summary>
        /// <typeparam name="TKey">The type of the dictionary key</typeparam>
        /// <typeparam name="TVal">The type of the dictionary value</typeparam>
        /// <param name="thisDictionary">The dictionary to execute the extension on</param>
        /// <param name="key">The key to search for</param>
        /// <returns>The retrieved or created value</returns>
        public static TVal GetOrCreate<TKey, TVal>(this Dictionary<TKey, TVal> thisDictionary, TKey key)
        {
            TVal val;

            if (!thisDictionary.TryGetValue(key, out val))
            {
                val = Activator.CreateInstance<TVal>();
                thisDictionary.Add(key, val);
            }

            return val;
        }
    }
}
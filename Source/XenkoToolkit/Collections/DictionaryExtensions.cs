using System;
using System.Collections.Generic;

namespace XenkoToolkit.Collections
{
    public static class DictionaryExtensions
    {
        public static void MergeInto<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            foreach (var item in source)
                target[item.Key] = item.Value;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dicionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dicionary == null)
            {
                throw new ArgumentNullException(nameof(dicionary));
            }

            if (dicionary.TryGetValue(key, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dicionary, TKey key, Func<TKey, TValue> getValue)
        {
            if (dicionary == null)
            {
                throw new ArgumentNullException(nameof(dicionary));
            }

            if (getValue == null)
            {
                throw new ArgumentNullException(nameof(getValue));
            }

            if (!dicionary.TryGetValue(key, out var result))
            {
                dicionary[key] = result = getValue(key);
            }

            return result;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dicionary, TKey key, Func<TKey, TValue> getValue, Func<TValue, bool> shouldAdd)
        {
            if (dicionary == null)
            {
                throw new ArgumentNullException(nameof(dicionary));
            }

            if (getValue == null)
            {
                throw new ArgumentNullException(nameof(getValue));
            }

            if (!dicionary.TryGetValue(key, out var result))
            {
                result = getValue(key);

                if (shouldAdd(result))
                    dicionary[key] = result;
            }

            return result;
        }

        public static int Increment<TKey>(this IDictionary<TKey, int> dicionary, TKey key)
        {
            if (dicionary == null)
            {
                throw new ArgumentNullException(nameof(dicionary));
            }

            dicionary[key] = dicionary.TryGetValue(key, out var result) ? result += 1 : result = 1;
            return result;
        }
    }
}

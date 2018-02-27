using System;
using System.Collections.Generic;

namespace XenkoToolkit.Collections
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            foreach (var item in values)
            {
                collection.Add(item);
            }
        }

        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> collection)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }


            foreach (var item in collection)
            {
                queue.Enqueue(item);
            }
        }

        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> collection)
        {
            if (stack == null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }


            foreach (var item in collection)
            {
                stack.Push(item);
            }
        }        
    }
}

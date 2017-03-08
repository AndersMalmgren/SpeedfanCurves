using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedfanCurves.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> source)
        {
            var it = source.GetEnumerator();
            bool hasRemainingItems = false;
            bool isFirst = true;
            T item = default(T);

            do
            {
                hasRemainingItems = it.MoveNext();
                if (hasRemainingItems)
                {
                    if (!isFirst) yield return item;
                    item = it.Current;
                    isFirst = false;
                }
            } while (hasRemainingItems);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            var clone = new List<T>();
            foreach (var item in source)
            {
                clone.Add(item);
                action(item);
            }
            return clone;
        }

        public static TResult CreateResult<T, TResult>(this IEnumerable<T> source, TResult startValue, Func<T, TResult, TResult> action)
        {
            TResult result = startValue;
            foreach (var item in source)
            {
                result = action(item, result);
            }
            return result;
        }
    }
}

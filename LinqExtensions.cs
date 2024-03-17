using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.ConvertHelper
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Where data is even number
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<int> WhereEven(this IEnumerable<int> source)
        {
            foreach (int number in source)
            {
                if (number % 2 == 0)
                {
                    yield return number;
                }
            }
        }

        /// <summary>
        /// Where data is odd number
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<int> WhereOdd(this IEnumerable<int> source)
        {
            foreach (int number in source)
            {
                if (number % 2 != 0)
                {
                    yield return number;
                }
            }
        }

        /// <summary>
        /// Median
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static double Median(this IEnumerable<double> source)
        {
            if (!source.Any())
            {
                throw new InvalidOperationException("The source sequence is empty.");
            }

            var sortedNumbers = source.OrderBy(x => x).ToList();
            int count = sortedNumbers.Count;
            if (count % 2 == 0)
            {
                return (sortedNumbers[count / 2 - 1] + sortedNumbers[count / 2]) / 2.0;
            }
            else
            {
                return sortedNumbers[count / 2];
            }
        }

        /// <summary>
        /// Paging
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index must be greater than or equal to zero.");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            }

            return source.Skip(pageIndex * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Where data in list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="listContains"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IEnumerable<T> WhereIn<T>(this IEnumerable<T> source,  IEnumerable<object> listContains, string propertyName)
        {
            foreach (T data in source)
            {
                var value = data.GetType().GetProperty(propertyName).GetValue(data,null);
                if (listContains.Any(t=>t == value))
                {
                    yield return data;
                }
            }
        }

        /// <summary>
        /// Shuffle data
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source?.OrderBy(x => Guid.NewGuid());
        }

        /// <summary>
        /// Join string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinStringArray(this IEnumerable<string> source, string separator)
        {
            if (source == null)
                return null;

            return string.Join(separator, source);
        }

        /// <summary>
        /// Distinct by a property name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="items"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }
    }
}

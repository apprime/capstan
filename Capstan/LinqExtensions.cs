using System;
using System.Collections.Generic;

namespace Capstan
{
    internal static class LinqExtensions
    {
        /// <summary>
        /// Capstan extends Linq with a Where() that takes
        /// a Predicate, instead of a Func, to do the same thing
        /// as the func in standard linq extensions.
        /// </summary>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("capstans overloaded Linq:Where - source is null");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("capstans overloaded Linq:Where - predicate is null");
            }

            foreach (var item in source)
            {
                if(predicate(item))
                {
                    yield return item;
                }
            }
        }
    }
}

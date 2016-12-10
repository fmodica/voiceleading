using System;
using System.Collections.Generic;
using System.Linq;

namespace HelperExtensions
{
    public static class Validation
    {
        public static void ValidateIsNotNull<T>(this T item, string parameterName)
        {
            if (item == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void ValidateIsGreaterThan(this int number, int min, string parameterName, bool isInclusive = false)
        {
            if (isInclusive)
            {
                if (!(number >= min))
                {
                    throw new ArgumentOutOfRangeException(parameterName, "The value must be greater than or equal to " + min + ".");
                }
            }
            else
            {
                if (!(number > min))
                {
                    throw new ArgumentOutOfRangeException(parameterName, "The value must be greater than " + min + ".");
                }
            }
        }

        public static void ValidateIsLessThan(this int number, int max, string parameterName, bool isInclusive = false)
        {
            if (isInclusive)
            {
                if (!(number <= max))
                {
                    throw new ArgumentOutOfRangeException(parameterName, "The value must be less than or equal to " + max + ".");
                }
            }
            else
            {
                if (!(number < max))
                {
                    throw new ArgumentOutOfRangeException(parameterName, "The value must be less than " + max + ".");
                }
            }
        }

        public static void ValidateIsNotNullOrEmpty<T>(this IEnumerable<T> collection, string parameterName)
        {
            collection.ValidateIsNotNull(parameterName);

            if (!collection.Any())
            {
                throw new ArgumentException("The collection is empty.", parameterName);
            }
        }

        public static void ValidateIsNotNullOrEmptyOrHasNullItem<T>(this IEnumerable<T> collection, string parameterName) where T : class
        {
            collection.ValidateIsNotNullOrEmpty(parameterName);

            if (collection.Any(x => x == null))
            {
                throw new ArgumentNullException("An object in " + parameterName + " is null.");
            }
        }

        public static void ValidateDoesNotContainDuplicates<T>(this IEnumerable<T> collection, string parameterName)
        {
            if (collection.Count() != collection.Distinct().Count())
            {
                throw new ArgumentException("The collection contains duplicates.", parameterName);
            }
        }
    }
}
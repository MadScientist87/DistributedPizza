using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedPizza.Core
{
    public static class Extensions
    {
        private static readonly Random _random = new BetterRandom();
        public static TEnum Randomize<TEnum>()
         where TEnum : struct
        {
            var enums = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
            return enums[_random.Next(enums.Count)];
        }

        public static T GetValueOrDefault<T>(this NameValueCollection coll, string key, T defaultValue = default(T))
        {
            var value = coll.Get(key);
            return value == null ? defaultValue : ChangeType<T>(value);
        }

        public static T ChangeType<T>(string value)
        {
            if (String.IsNullOrEmpty(value))
                return default(T);

            Type type = typeof(T);
            Type desiredType = Nullable.GetUnderlyingType(type) ?? type;
            if (desiredType == typeof(TimeSpan)) return (T)(object)TimeSpan.Parse(value);
            if (desiredType.IsEnum) return (T)Enum.Parse(desiredType, value, true);
            return (T)Convert.ChangeType(value, desiredType);
        }

        public static void Replace<T>(this List<T> list, Predicate<T> oldItemSelector, T newItem)
        {
            //check for different situations here and throw exception
            //if list contains multiple items that match the predicate
            //or check for nullability of list and etc ...
            var oldItemIndex = list.FindIndex(oldItemSelector);
            list[oldItemIndex] = newItem;
        }

        public static void Swap<T>(this ICollection<T> collection, T oldValue, T newValue)
        {
            // In case the collection is ordered, we'll be able to preserve the order
            var collectionAsList = collection as IList<T>;
            if (collectionAsList != null)
            {
                var oldIndex = collectionAsList.IndexOf(oldValue);
                collectionAsList.RemoveAt(oldIndex);
                collectionAsList.Insert(oldIndex, newValue);
            }
            else
            {
                // No luck, so just remove then add
                collection.Remove(oldValue);
                collection.Add(newValue);
            }

        }
    }
}

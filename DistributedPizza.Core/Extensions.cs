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
    }
}

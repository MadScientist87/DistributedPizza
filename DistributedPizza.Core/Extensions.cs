using System;
using System.Collections.Generic;
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedPizza.Core
{
    public class BetterRandom : Random
    {
        /* Period parameters */
        private const int N = 624;
        private const int M = 397;

        private const uint UpperMask = 0x80000000; /* most significant w-r bits */
        private const uint LowerMask = 0x7fffffff; /* least significant r bits */
        private const double FloatScaler = (1.0 / 9007199254740991.0);

        /* Tempering parameters */
        private static readonly uint[] _mag01 = { 0x0, 0x9908b0df };

        private readonly uint[] _mt = new uint[N]; /* the array for the state vector  */

        private short _mti;

        public BetterRandom(uint seed)
        {
            _mt[0] = seed & 0xffffffffU;
            for (_mti = 1; _mti < N; ++_mti)
                _mt[_mti] = (69069 * _mt[_mti - 1]) & 0xffffffffU;
        }

        public BetterRandom()
            : this(4357)
        {
        }

        public virtual uint NextUInt32()
        {
            uint y;

            if (_mti >= N) /* generate N words at one time */
            {
                short kk = 0;

                for (; kk < N - M; ++kk)
                {
                    y = (_mt[kk] & UpperMask) | (_mt[kk + 1] & LowerMask);
                    _mt[kk] = _mt[kk + M] ^ (y >> 1) ^ _mag01[y & 0x1];
                }

                for (; kk < N - 1; ++kk)
                {
                    y = (_mt[kk] & UpperMask) | (_mt[kk + 1] & LowerMask);
                    _mt[kk] = _mt[kk + (M - N)] ^ (y >> 1) ^ _mag01[y & 0x1];
                }

                y = (_mt[N - 1] & UpperMask) | (_mt[0] & LowerMask);
                _mt[N - 1] = _mt[M - 1] ^ (y >> 1) ^ _mag01[y & 0x1];

                _mti = 0;
            }

            y = _mt[_mti++];
            y ^= y >> 11;
            y ^= y << 7 & 0x9d2c5680;
            y ^= y << 15 & 0xefc60000;
            y ^= y >> 18;

            return y;
        }

        public virtual uint NextUInt32(uint maxValue)
        {
            return (uint)(NextUInt32() / ((double)uint.MaxValue / maxValue));
        }

        public virtual uint NextUInt32(uint minValue, uint maxValue)
        {
            if (minValue < maxValue)
                return (uint)(NextUInt32() / ((double)uint.MaxValue / (maxValue - minValue)) + minValue);

            throw new ArgumentOutOfRangeException();
        }

        public override int Next()
        {
            return Next(int.MaxValue);
        }

        public override int Next(int maxValue)
        {
            if (maxValue > 1)
                return (int)(NextDouble() * maxValue);

            if (maxValue >= 0)
                return 0;

            throw new ArgumentOutOfRangeException();
        }

        public override int Next(int minValue, int maxValue)
        {
            if (maxValue >= minValue)
                return maxValue == minValue ? minValue : Next(maxValue - minValue) + minValue;

            throw new ArgumentOutOfRangeException();
        }

        public override void NextBytes(byte[] buffer) /* throws ArgumentNullException*/
        {
            if (buffer == null)
                throw new ArgumentNullException();

            for (int idx = 0; idx < buffer.Length; ++idx)
                buffer[idx] = (byte)Next(256);
        }

        public override Double NextDouble()
        {
            // get 27 pseudo-random bits
            UInt64 a = (UInt64)NextUInt32() >> 5;
            // get 26 pseudo-random bits
            UInt64 b = (UInt64)NextUInt32() >> 6;

            // shift the 27 pseudo-random bits (a) over by 26 bits (* 67108864.0) and
            // add another pseudo-random 26 bits (+ b).
            return ((a * 67108864.0 + b) + 0.5) * FloatScaler;
        }
    }
}

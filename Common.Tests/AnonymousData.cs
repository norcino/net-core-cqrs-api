using System;
using System.Threading;

namespace Common.Tests
{
    /// <summary>
    /// Anonymous Data is a random data generator to use for unit testing when the actual value is not relevant for the test itself
    /// </summary>
    public static class AnonymousData
    {
        private static int _seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> RandomWrapper =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        private static Random GetThreadRandom()
        {
            return RandomWrapper.Value;
        }

        /// <summary>
        /// Random number is generated with the specified maximum length of digits
        /// </summary>
        /// <param name="length">Maximum number of digits</param>
        /// <returns>Random number</returns>
        public static int Int(int length = 3)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException($"{nameof(length)} must be greater than zero");

            Random ran = GetThreadRandom();
            return length > 0 ? ran.Next((10 ^ length) - 1) : 1;
        }

        /// <summary>
        /// Generate a string with an optional prefix and a random suffix of the desired length
        /// </summary>
        /// <param name="length">Length of the random part of the string</param>
        /// <param name="prefix">Prefix of the generated string</param>
        /// <returns>Randomly generated string</returns>
        public static string String(string prefix = "", int length = 15)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException($"{nameof(length)} must be greater than zero");

            var casualString = string.Empty;
            while (casualString.Length < length)
            {
                casualString += Guid.NewGuid().ToString();
            }

            return string.IsNullOrWhiteSpace(prefix) ? casualString.Substring(0, length) : $"{prefix}_{casualString.Substring(0, length)}";
        }

        /// <summary>
        /// Generate a randomic boolean value
        /// </summary>
        /// <returns>Boolean value</returns>
        public static bool Bool()
        {
            return DateTime.Now.Ticks % 2 == 0;
        }
    }
}
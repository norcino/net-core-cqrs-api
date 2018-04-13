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

            return length > 0 ? GetThreadRandom().Next((10 ^ length) - 1) : 1;
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
            return GetThreadRandom().Next(0, 100) % 2 == 0;
        }

        /// <summary>
        /// Generate a randomic signed byte value
        /// </summary>
        /// <returns>Signed byte value</returns>
        public static sbyte Byte()
        {
            return (sbyte) GetThreadRandom().Next(byte.MaxValue);
        }

        /// <summary>
        /// Generate a randomic short value
        /// </summary>
        /// <returns>Short value</returns>
        public static short Short()
        {
            return (short)GetThreadRandom().Next(short.MaxValue);
        }

        /// <summary>
        /// Generate a randomic long value
        /// </summary>
        /// <returns>Long value</returns>
        public static object Long()
        {
            return (long)GetThreadRandom().Next(int.MaxValue);
        }

        /// <summary>
        /// Generate random double
        /// </summary>
        /// <returns>Double value</returns>
        public static double Double()
        {
            return GetThreadRandom().NextDouble();
        }

        /// <summary>
        /// Generate a random float
        /// </summary>
        /// <returns>Float value</returns>
        public static float Float()
        {
            return (float) GetThreadRandom().NextDouble();
        }

        public static TimeSpan TimeSpan()
        {
            return new TimeSpan(GetThreadRandom().Next(0, 100), GetThreadRandom().Next(0, 100), GetThreadRandom().Next(0, 100), GetThreadRandom().Next(0, 1000));
        }

        /// <summary>
        /// Generate a rondom date time in the future, it is possible to request a date time in the past.
        /// </summary>
        /// <param name="future">Optiona boolean value, default value true will generate a date in the future</param>
        /// <returns>Random DateTime</returns>
        public static DateTime DateTime(bool future  = true)
        {
            int minYear = 1900;
            int maxYear = 2100;

            var now = System.DateTime.Now;

            var year = GetThreadRandom().Next(future ? now.Year + 1 : minYear, future ? maxYear : now.Year);

            var month = GetThreadRandom().Next(1, 12);
            var day = 1;

            switch (month)
            {
                case 4:
                case 6:
                case 9:
                case 11:
                    day = GetThreadRandom().Next(1, 30);
                    break;
                case 2:
                    day = GetThreadRandom().Next(1, 28);
                    break;
                default:
                    day = GetThreadRandom().Next(1, 31);
                    break;
            }

            return new DateTime(
                year,
                month,
                day,
                GetThreadRandom().Next(0, 23),
                GetThreadRandom().Next(0, 60),
                GetThreadRandom().Next(0, 1000));
        }

        /// <summary>
        /// Generate a random Character
        /// </summary>
        /// <returns>Random character</returns>
        public static char Char()
        {
            return (char) GetThreadRandom().Next(0, UInt16.MaxValue);
        }
    }
}
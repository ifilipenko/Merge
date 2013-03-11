using System;
using System.Linq;

namespace Merge.Test
{
    public static class StringGenerator
    {
        private static readonly Random _random;
        private const string _whiteSpaces = "\t ";

        static StringGenerator()
        {
            _random = new Random((int)DateTime.Now.Ticks);
        }

        public static string[] GenerateStrings(int count, bool enableWhitespaces)
        {
            return Enumerable.Range(0, count)
                             .Select(_ => GenerateString(enableWhitespaces: enableWhitespaces))
                             .ToArray();
        }

        public static string GenerateString(int minLen = 1, int maxLen = 50, bool enableWhitespaces = false)
        {
            const byte min = (byte) 'a';
            const byte max = (byte) 'z';

            var length = _random.Next(minLen, maxLen);
            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                var whiteSpace = false;
                if (enableWhitespaces)
                {
                    whiteSpace = _random.NextDouble() > 0.5;
                }
                if (whiteSpace)
                {
                    chars[i] = _whiteSpaces[_random.Next(0, _whiteSpaces.Length)];
                }
                else
                {
                    chars[i] = (char) _random.Next(min, max + 1);
                }
            }
            return new string(chars);
        }
    }
}
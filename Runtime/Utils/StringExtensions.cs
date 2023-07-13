using System;
using System.Collections.Generic;
using System.Globalization;

namespace Gilzoide.KeyValueStore.Utils
{
    public static class StringExtensions
    {
#if NETSTANDARD2_1
        public static IEnumerable<ReadOnlyMemory<char>> SplitEnumerate(this string text, char separator)
        {
            int startIndex = 0;
            int separatorIndex;
            while ((separatorIndex = text.IndexOf(separator, startIndex)) >= 0)
            {
                int length = separatorIndex - startIndex;
                if (length > 0)
                {
                    yield return text.AsMemory(startIndex, separatorIndex - startIndex);
                }
                startIndex = separatorIndex + 1;
            }
            yield return text.AsMemory(startIndex);
        }
#else
        public static IEnumerable<string> SplitEnumerate(this string text, char separator)
        {
            int startIndex = 0;
            int separatorIndex;
            while ((separatorIndex = text.IndexOf(separator, startIndex)) >= 0)
            {
                int length = separatorIndex - startIndex;
                if (length > 0)
                {
                    yield return text.Substring(startIndex, separatorIndex - startIndex);
                }
                startIndex = separatorIndex + 1;
            }
            yield return text.Substring(startIndex);
        }
#endif

        public static IEnumerable<int> EnumerateInts(this string text, char separator)
        {
            foreach (var substring in text.SplitEnumerate(separator))
            {
#if NETSTANDARD2_1
                ReadOnlySpan<char> arg = substring.Span;
#else
                string arg = substring;
#endif
                if (int.TryParse(arg, NumberStyles.Integer, CultureInfo.InvariantCulture, out int number))
                {
                    yield return number;
                }
                else
                {
                    yield break;
                }
            }
        }

        public static IEnumerable<float> EnumerateFloats(this string text, char separator)
        {
            foreach (var substring in text.SplitEnumerate(separator))
            {
#if NETSTANDARD2_1
                ReadOnlySpan<char> arg = substring.Span;
#else
                string arg = substring;
#endif
                if (float.TryParse(arg, NumberStyles.Float, CultureInfo.InvariantCulture, out float number))
                {
                    yield return number;
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}

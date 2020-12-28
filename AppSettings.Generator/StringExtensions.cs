using System;
using System.Linq;

namespace AppSettings.Generator
{
    internal static class StringExtensions
    {
        public static string Sanitize(this string s)
        {
            return string.Join("", s.AsEnumerable()
                                    .Select(chr => Char.IsLetter(chr) || Char.IsDigit(chr)
                                                   ? chr.ToString()
                                                   : ""));
        }

        public static string GetTypeToGenerate(this string value)
        {
            if (bool.TryParse(value, out var boolResult))
            {
                return "bool";
            }

            if (int.TryParse(value, out var intResult))
            {
                return "int";
            }

            if (float.TryParse(value, out var floatResult))
            {
                return "float";
            }

            if (double.TryParse(value, out var doubleResult))
            {
                return "double";
            }

            if (byte.TryParse(value, out var byteResult))
            {
                return "byte";
            }

            if (DateTime.TryParse(value, out var dateTimeResult))
            {
                return "DateTime";
            }

            if (TimeSpan.TryParse(value, out var timeSpanResult))
            {
                return "TimeSpan";
            }

            if (Guid.TryParse(value, out var guidResult))
            {
                return "Guid";
            }

            return "string";
        }
    }
}

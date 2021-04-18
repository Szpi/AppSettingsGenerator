using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text.RegularExpressions;

namespace AppSettingsGenerator
{
    internal static class StringExtensions
    {
        private static readonly CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");

        public static bool IsIdentifierValid(this string s)
        {
            return provider.IsValidIdentifier(s) || string.IsNullOrWhiteSpace(s);
        }

        public static string Sanitize(this string s)
        {
            var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
            var validIdentifier = regex.Replace(s, "");

            validIdentifier = provider.CreateValidIdentifier(validIdentifier);

            return validIdentifier;
        }

        public static string GetTypeToGenerate(this string value)
        {
            if (bool.TryParse(value, out var _))
            {
                return "bool";
            }

            if (int.TryParse(value, out var _))
            {
                return "int";
            }

            if (float.TryParse(value, out var _))
            {
                return "float";
            }

            if (double.TryParse(value, out var _))
            {
                return "double";
            }

            if (byte.TryParse(value, out var _))
            {
                return "byte";
            }

            if (TimeSpan.TryParse(value, out var _))
            {
                return "System.TimeSpan";
            }

            if (DateTime.TryParse(value, out var _))
            {
                return "System.DateTime";
            }

            if (Guid.TryParse(value, out var _))
            {
                return "System.Guid";
            }

            return "string";
        }
    }
}

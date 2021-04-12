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

        //public static string CreateValidIdentifier(this string s)
        //{
        //    return s;
        //    //if(provider.IsValidIdentifier(s) || string.IsNullOrWhiteSpace(s))
        //    //{
        //    //    return s;
        //    //}

        //    //var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
        //    //var validIdentifier = regex.Replace(s, "");

        //    //validIdentifier = provider.CreateValidIdentifier(validIdentifier);

        //    //return validIdentifier;
        //}

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

            if (TimeSpan.TryParse(value, out var timeSpanResult))
            {
                return "System.TimeSpan";
            }

            if (DateTime.TryParse(value, out var dateTimeResult))
            {
                return "System.DateTime";
            }

            if (Guid.TryParse(value, out var guidResult))
            {
                return "System.Guid";
            }

            return "string";
        }
    }
}

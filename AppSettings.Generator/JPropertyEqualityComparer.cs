using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppSettingsGenerator
{
    internal class JPropertyEqualityComparer : IEqualityComparer<JProperty>
    {
        public bool Equals(JProperty x, JProperty y)
        {
            return x?.Name == y?.Name;
        }

        public int GetHashCode(JProperty obj)
        {
            return obj?.Name.GetHashCode() ?? obj.GetHashCode();
        }
    }
}

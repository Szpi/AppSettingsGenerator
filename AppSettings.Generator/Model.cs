using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppSettings.Generator
{
    public class Model
    {
        public Model(string className, IEnumerable<KeyValuePair<string, string>> properties)
        {
            Properties = properties.ToList();
            ClassName = className;
        }

        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public List<KeyValuePair<string, string>> Properties { get; }
        public string ClassName { get; }
    }
}

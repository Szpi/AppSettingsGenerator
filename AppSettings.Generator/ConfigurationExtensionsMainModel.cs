using System;
using System.Collections.Generic;
using System.Text;

namespace AppSettingsGenerator
{
    internal class ConfigurationExtensionsModel
    {
        public ConfigurationExtensionsModel(string type, string propertyPath, string sanitizedName, string name)
        {
            Type = type;
            PropertyPath = propertyPath;
            SanitizedName = sanitizedName;
            Name = name;
        }
        public string Type { get; }
        public string PropertyPath { get; }
        public string SanitizedName { get; }
        public string Name { get; }
    }

    internal class ConfigurationExtensionsMainModel
    {
        public List<ConfigurationExtensionsModel> Properties { get; }

        public ConfigurationExtensionsMainModel(List<ConfigurationExtensionsModel> properties)
        {
            Properties = properties;
        }
    }
}

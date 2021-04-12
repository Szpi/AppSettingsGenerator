using System;
using System.Collections.Generic;
using System.Text;

namespace AppSettingsGenerator
{
    internal class ConfigurationExtensionsModel
    {
        public ConfigurationExtensionsModel(string type, string propertyPath, string sanitizedName)
        {
            Type = type;
            PropertyPath = propertyPath;
            SanitizedName = sanitizedName;
        }
        public string Type { get; }
        public string PropertyPath { get; }
        public string SanitizedName { get; }
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

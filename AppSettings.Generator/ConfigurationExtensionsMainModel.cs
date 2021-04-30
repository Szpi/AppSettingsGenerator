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


    class ConfigurationExtensionsModelComparer : IEqualityComparer<ConfigurationExtensionsModel>
    {
        public bool Equals(ConfigurationExtensionsModel x, ConfigurationExtensionsModel y)
        {
            return x.Name == y.Name || x.SanitizedName == y.SanitizedName;
        }

        public int GetHashCode(ConfigurationExtensionsModel obj)
        {
            return obj.SanitizedName.GetHashCode();
        }
    }
}

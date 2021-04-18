using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;
using Scriban;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if DEBUG
[assembly: InternalsVisibleTo("AppSettingsGenerator.Tests")]
#endif

namespace AppSettingsGenerator
{
    internal class ConfigGenerator
    {
        public (IEnumerable<(string fileName, string generatedClass)> outputFiles,
                IEnumerable<(string invalidIdentifierName, string invalidIdentifierNamePath)> invalidIdentifiers) Generate(string filePath)
        {
            var config = JObject.Parse(File.ReadAllText(filePath));

            var configJProperties = config.Descendants()
                .OfType<JProperty>()
                .Where(x => x != null)
                .GroupBy(x => x.Ancestors().OfType<JProperty>().FirstOrDefault()?.Name ?? string.Empty)
                .ToList();

            var file = "AppSettings.sbntxt";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);

            var outputList = new List<(string fileName, string generatedClass)>();
            var (classNames, invalidIdentifiers) = GenerateConfig(configJProperties, template, outputList);

            outputList = GenerateServiceExtensions(outputList, classNames);

            return (outputList, invalidIdentifiers);
        }

        private (List<string> classNames, IEnumerable<(string invalidIdentifierName, string invalidIdentifierNamePath)> invalidIdentifiers) GenerateConfig(
            List<IGrouping<string, JProperty>> configJProperties, Template template, List<(string fileName, string generatedClass)> outputList)
        {
            var classNames = new List<string>();
            var configurationExtensionsModel = new List<ConfigurationExtensionsModel>();
            var invalidIdentifiersMain = new List<(string invalidIdentifierName, string invalidIdentifierNamePath)>();
            foreach (var grouppedProperties in configJProperties)
            {
                var (properties, configurationExtensions, invalidIdentifiers) = GenerateProperties(grouppedProperties);

                configurationExtensionsModel.AddRange(configurationExtensions);
                invalidIdentifiersMain.AddRange(invalidIdentifiers);

                if (!grouppedProperties.Key.IsIdentifierValid())
                {
                    var (invalidIdentifierName, invalidIdentifierNamePath) = invalidIdentifiersMain.FirstOrDefault(x => x.invalidIdentifierName == grouppedProperties.Key);
                    if (invalidIdentifierName == null)
                    {
                        invalidIdentifiersMain.Add((grouppedProperties.Key, grouppedProperties.Key));
                    }

                    continue;
                }

                if (!properties.Any())
                {
                    continue;
                }


                var className = grouppedProperties.Key;
                if (string.IsNullOrWhiteSpace(className))
                {
                    className = "AppSettings";
                }
                classNames.Add(className);
                var model = new Model(className, properties);
                var output = template.Render(model, member => member.Name);

                outputList.Add(($"{className}.cs", output));
            }

            outputList = GenerateInvalidIdentifiers(outputList, configurationExtensionsModel.Distinct().ToList());
            return (classNames, invalidIdentifiersMain);
        }

        private static List<(string fileName, string generatedClass)> GenerateServiceExtensions(List<(string fileName, string generatedClass)> outputList, List<string> classNames)
        {
            var fileServiceCollectionExtensions = "ServiceCollectionExtensions.sbntxt";
            var templateServiceCollectionExtensions = Template.Parse(EmbeddedResource.GetContent(fileServiceCollectionExtensions), fileServiceCollectionExtensions);
            var classNamesModel = new ClassNamesModel()
            {
                ClassNames = classNames
            };
            var outputServiceCollectionExtensions = templateServiceCollectionExtensions.Render(classNamesModel, member => member.Name);
            outputList.Add(("ServiceCollectionExtensions.cs", outputServiceCollectionExtensions));

            return outputList;
        }

        private List<(string fileName, string generatedClass)> GenerateInvalidIdentifiers(List<(string fileName, string generatedClass)> outputList, List<ConfigurationExtensionsModel> configurationExtensions)
        {
            if (!configurationExtensions.Any())
            {
                return outputList;
            }

            var fileConfigurationExtensions = "IConfigurationExtensions.sbntxt";
            var templateServiceCollectionExtensions = Template.Parse(EmbeddedResource.GetContent(fileConfigurationExtensions), fileConfigurationExtensions);
            var classNamesModel = new ConfigurationExtensionsMainModel(configurationExtensions);

            var outputConfigurationExtensions = templateServiceCollectionExtensions.Render(classNamesModel, member => member.Name);
            outputList.Add(("ConfigurationExtensions.cs", outputConfigurationExtensions));

            return outputList;
        }

        private (List<KeyValuePair<string, string>> properties,
            List<ConfigurationExtensionsModel> configurationExtensionsModels,
            IEnumerable<(string invalidIdentifierName, string invalidIdentifierNamePath)> invalidIdentifiers) GenerateProperties(IGrouping<string, JProperty> grouppedProperties)
        {
            var properties = new List<KeyValuePair<string, string>>();
            var configurationExtensions = new List<ConfigurationExtensionsModel>();
            var invalidIdentifiers = new List<(string invalidIdentifierName, string invalidIdentifierNamePath)>();

            foreach (var property in grouppedProperties.Distinct(new JPropertyEqualityComparer()))
            {
                var (propertyName, type) = GetTypeAndName(property);

                if (!property.Name.IsIdentifierValid())
                {
                    var index = property.Path.IndexOf($"['{property.Name}']");
                    if (index <= 0)
                    {
                        continue;
                    }

                    var sanitizedPath = property.Path.Remove(index);
                    sanitizedPath = sanitizedPath.Replace('.', ':');
                    sanitizedPath += sanitizedPath.Length > 0 ? $":{property.Name}" : string.Empty;

                    invalidIdentifiers.Add((property.Name, sanitizedPath));

                    if (property.Value.Type == JTokenType.Array || property.Value.Type == JTokenType.Object)
                    {
                        continue;
                    }

                    configurationExtensions.Add(new ConfigurationExtensionsModel(type, sanitizedPath, propertyName.Sanitize(), property.Name));
                    continue;
                }

                properties.Add(new KeyValuePair<string, string>(type, propertyName));
            }

            return (properties, configurationExtensions, invalidIdentifiers);
        }

        private static (string propertyName, string type) GetTypeAndName(JProperty property)
        {
            var propertyName = property.Name;

            var type = string.Empty;
            if (property.Value.Type == JTokenType.Array)
            {
                if (property.Value.Children().All(x => x.Type != JTokenType.Object))
                {
                    var childrenTypes = property.Value.Children().Cast<JValue>().Select(x => x.Value.ToString().GetTypeToGenerate());
                    type = childrenTypes.Distinct().Count() == 1 ? $"System.Generic.List<{childrenTypes.First()}>" : "System.Generic.List<string>";
                }
                else
                {
                    type = $"System.Generic.List<{propertyName}>";
                }
                return (propertyName, type);
            }
            else
            {
                type = property.Value.Children().Any() ? propertyName : property.Value.ToString().GetTypeToGenerate();
            }

            return (propertyName, type);
        }
    }

}
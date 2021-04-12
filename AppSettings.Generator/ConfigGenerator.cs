using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;
using Scriban;
using System.Collections.Generic;
using System.IO;
using System.Linq;
[assembly: InternalsVisibleTo("AppSettingsGenerator.Tests")]

namespace AppSettingsGenerator
{
    internal class ConfigGenerator
    {
        public IEnumerable<(string fileName, string generatedClass)> Generate(string filePath)
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

            var classNames = new List<string>();
            foreach (var grouppedProperties in configJProperties)
            {
                var properties = GenerateProperties(grouppedProperties);

                if (!properties.Any())
                {
                    continue;
                }

                if (!grouppedProperties.Key.IsIdentifierValid())
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

        private List<KeyValuePair<string, string>> GenerateProperties(IGrouping<string, JProperty> grouppedProperties)
        {
            var properties = new List<KeyValuePair<string, string>>();            

            foreach (var property in grouppedProperties.Distinct(new JPropertyEqualityComparer()))
            {
                if (!property.Name.IsIdentifierValid())
                {
                    continue;
                }

                var (sanitizedName, type) = GetTypeAndName(property);
                properties.Add(new KeyValuePair<string, string>(type, sanitizedName));
            }

            return properties;
        }

        private static (string sanitizedName, string type) GetTypeAndName(JProperty property)
        {            
            var sanitizedName = property.Name;
           
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
                    type = $"System.Generic.List<{sanitizedName}>";
                }
                return (sanitizedName, type);
            }
            else
            {
                type = property.Value.Children().Any() ? sanitizedName : property.Value.ToString().GetTypeToGenerate();
            }

            return (sanitizedName, type);
        }
    }    

}
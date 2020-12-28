using Newtonsoft.Json.Linq;
using Scriban;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AppSettings.Generator
{
    public class ConfigGenerator
    {
        public IEnumerable<(string fileName, string generatedClass)> Generate(string filePath)
        {
            var config = JObject.Parse(File.ReadAllText(filePath));
            var test = new List<string>();

            var configJProperties = config.Descendants()
                .OfType<JProperty>()
                .Where(x => x != null)
                .GroupBy(x => x.Ancestors().OfType<JProperty>().FirstOrDefault()?.Name ?? string.Empty)
                .ToList();

            var file = "CSharp.sbntxt";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);

            var outputList = new List<(string fileName, string generatedClass)>();

            foreach (var grouppedProperties in configJProperties)
            {
                var properties = new List<KeyValuePair<string, string>>();
                foreach (var property in grouppedProperties)
                {
                    var sanitizedName = property.Name.Sanitize();
                    var type = property.Value.Children().Any() ? sanitizedName : property.Value.ToString().GetTypeToGenerate();
                    properties.Add(new KeyValuePair<string, string>(type, sanitizedName));
                }

                var className = grouppedProperties.Key.Sanitize();
                if (string.IsNullOrWhiteSpace(className))
                {
                    className = "AppSettings";
                }
                var model = new Model(className, properties);
                var output = template.Render(model, member => member.Name);

                outputList.Add(($"{className}.cs", output));
            }

            return outputList;
        }
    }

}
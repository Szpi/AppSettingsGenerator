using AppSettingsGenerator;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppSettings.Generator.Tests
{
    class ConfigGeneratorInvalidIdentifierTests
    {
        private ConfigGenerator _configGenerator;

        [SetUp]
        public void SetUp()
        {
            _configGenerator = new ConfigGenerator();
        }

        [Test]
        public void Generate_ConfigurationExtensions_WhenPropertyHasInvalidIdentifier()
        {
            var generated = _configGenerator.Generate("InvalidIdentifier.json");

            generated.Select(x => x.fileName).Should().ContainSingle(x => x == "ConfigurationExtensions.cs");
            var configurationExtensions = generated.FirstOrDefault(x => x.fileName == "ConfigurationExtensions.cs");

            configurationExtensions.generatedClass.Should().Contain(@"return configuration.GetValue<int>(""MyArray:LogLevel2:Microsoft,[Host.ing,Lifetime1"")");
        }

        [Test]
        public void Generate_ConfigurationExtensions_WhenClassHasInvalidIdentifier()
        {
            var generated = _configGenerator.Generate("InvalidIdentifier.json");

            generated.Select(x => x.fileName).Should().ContainSingle(x => x == "ConfigurationExtensions.cs");
            var configurationExtensions = generated.FirstOrDefault(x => x.fileName == "ConfigurationExtensions.cs");

            configurationExtensions.generatedClass.Should().NotContain(@"testWrongClass");
        }

    }
}

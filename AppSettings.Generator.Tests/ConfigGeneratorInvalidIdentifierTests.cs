using AppSettingsGenerator;
using AppSettingsGenerator.Tests;
using FluentAssertions;
using FluentAssertions.Execution;
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
            var (generated,_) = _configGenerator.Generate("InvalidIdentifier.json");

            generated.Select(x => x.fileName).Should().ContainSingle(x => x == "ConfigurationExtensions.cs");
            var configurationExtensions = generated.FirstOrDefault(x => x.fileName == "ConfigurationExtensions.cs");

            configurationExtensions.generatedClass.Should().Contain(@"return configuration.GetValue<int>(""MyArray:LogLevel2:Microsoft,[Host.ing,Lifetime1"")");
            AssertCompilation(generated);
        }

        [Test]
        public void Generate_ConfigurationExtensions_WhenClassHasInvalidIdentifier()
        {
            var (generated, invalidIdentifiers) = _configGenerator.Generate("InvalidIdentifier.json");

            generated.Select(x => x.fileName).Should().ContainSingle(x => x == "ConfigurationExtensions.cs");
            var configurationExtensions = generated.FirstOrDefault(x => x.fileName == "ConfigurationExtensions.cs");

            configurationExtensions.generatedClass.Should().NotContain(@"testWrongClass");

            invalidIdentifiers.Should().HaveCount(4);
            invalidIdentifiers.First().invalidIdentifierName.Should().Be("Microsoft,[Host.ing,Lifetime1");
            invalidIdentifiers.First().invalidIdentifierNamePath.Should().Be("MyArray:LogLevel2:Microsoft,[Host.ing,Lifetime1");

            invalidIdentifiers.Last().invalidIdentifierName.Should().Be("Microsoft,[Host.ing,Lifetime1");
            invalidIdentifiers.Last().invalidIdentifierNamePath.Should().Be("['MyArra::::::y']['testWrongClass:::[][]2']:Microsoft,[Host.ing,Lifetime1");

            AssertCompilation(generated);
        }

        private void AssertCompilation(IEnumerable<(string fileName, string generatedClass)> generated)
        {
            var (compilationResult, reason) = new TestCodeCompiler().Compile(generated.Select(x => x.generatedClass));
            
            compilationResult.Should().BeTrue();
            reason.Should().BeEmpty();
        }
    }
}

using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using System.Linq;

namespace AppSettingsGenerator.Tests
{
    class ConfigGeneratorNLogTests
    {
        private ConfigGenerator _configGenerator;

        [SetUp]
        public void SetUp()
        {
            _configGenerator = new ConfigGenerator();
        }

        [Test]
        public void Generate_()
        {
            using var assertionScope = new AssertionScope();
            var (generated, _) = _configGenerator.Generate("NLog.json");

            generated.Select(x => x.fileName).Should().ContainSingle(x => x == "extensions.cs");
            var appsettings = generated.FirstOrDefault(x => x.fileName == "AppSettings.cs");

            appsettings.generatedClass.Should().Contain("public NLog NLog { get; set; }");

            var (compilationResult, reason) = new TestCodeCompiler().Compile(generated.Select(x => x.generatedClass));
            compilationResult.Should().BeTrue();
            reason.Should().BeEmpty();
        }
    }
}

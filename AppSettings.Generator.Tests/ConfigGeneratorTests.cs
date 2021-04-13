using AppSettingsGenerator;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppSettings.Generator.Tests
{
    class ConfigGeneratorTests
    {
        private ConfigGenerator _configGenerator;

        [SetUp]
        public void SetUp()
        {
            _configGenerator = new ConfigGenerator();
        }

        [Test]
        public void Generate_FromMyArrayFile_ShouldContains_ListOfMyArray()
        {
            var (generated, _) = _configGenerator.Generate("MyArray.json");

            generated.Select(x => x.fileName).Should().ContainSingle(x => x == "MyArray.cs");
            var appsettings = generated.FirstOrDefault(x => x.fileName == "AppSettings.cs");

            appsettings.generatedClass.Should().Contain("public System.Generic.List<MyArray> MyArray { get; set; }");
        }

        [Test]
        public void Generate_FromMyArrayFile_Should_CreateValidIdentifier()
        {
            var (generated, _) = _configGenerator.Generate("MyArray.json");

            generated.Select(x => x.fileName).Should().ContainSingle(x => x == "MyArray.cs");
            var logLevel = generated.FirstOrDefault(x => x.fileName == "LogLevel2.cs");

            logLevel.generatedClass.Should().NotContain("Microsoft,[Hosting,Lifetime1");
        }

        [Test]
        public void Generate_FromMultipleArrays_ShouldContains_ListOfMyArray()
        {
            var (generated, _) = _configGenerator.Generate("MultipleArrays.json");

            generated.Select(x => x.fileName).Should().ContainSingle(x => x == "cars.cs");
            generated.Select(x => x.fileName).Should().NotContain(x => x == "models.cs");
            var appsettings = generated.FirstOrDefault(x => x.fileName == "AppSettings.cs");
            var cars = generated.FirstOrDefault(x => x.fileName == "cars.cs");

            appsettings.generatedClass.Should().Contain("public System.Generic.List<cars> cars { get; set; }");
            cars.generatedClass.Should().Contain("public System.Generic.List<string> models { get; set; }");
        }

        [Test]
        public void Generate_FromMultipleArraysObjects_ShouldContains_ListOfMyArray()
        {
            var (generated, _) = _configGenerator.Generate("MultipleArraysObjects.json");

            generated.Select(x => x.fileName).Should().ContainSingle(x => x == "cars.cs");
            generated.Select(x => x.fileName).Should().ContainSingle(x => x == "models.cs");
            var appsettings = generated.FirstOrDefault(x => x.fileName == "AppSettings.cs");
            var cars = generated.FirstOrDefault(x => x.fileName == "cars.cs");

            appsettings.generatedClass.Should().Contain("public System.Generic.List<cars> cars { get; set; }");
            cars.generatedClass.Should().Contain("public System.Generic.List<models> models { get; set; }");
        }
        
    }
}

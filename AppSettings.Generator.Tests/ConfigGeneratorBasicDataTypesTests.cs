using AppSettingsGenerator;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppSettings.Generator.Tests
{
    class ConfigGeneratorBasicDataTypesTests
    {
        private ConfigGenerator _configGenerator;

        [SetUp]
        public void SetUp()
        {
            _configGenerator = new ConfigGenerator();
        }

        [Test]
        public void Generate_FromBasicDataTypes_Should_CreateBasicTypes()
        {
            var generated = _configGenerator.Generate("BasicDataTypes.json");

            var appsettings = generated.FirstOrDefault(x => x.fileName == "AppSettings.cs");

            appsettings.generatedClass.Should().Contain("public int number_1 { get; set; }");
            appsettings.generatedClass.Should().Contain("public int number_2 { get; set; }");
            appsettings.generatedClass.Should().Contain("public float number_3 { get; set; }");
            appsettings.generatedClass.Should().Contain("public int number_4 { get; set; }");
            
            appsettings.generatedClass.Should().Contain("public bool bool_1 { get; set; }");
            appsettings.generatedClass.Should().Contain("public bool bool_2 { get; set; }");

            appsettings.generatedClass.Should().Contain("public string popularity { get; set; }");

            appsettings.generatedClass.Should().Contain("public System.TimeSpan Timespan_1 { get; set; }");
            appsettings.generatedClass.Should().Contain("public System.DateTime DateTime_1 { get; set; }");

            appsettings.generatedClass.Should().Contain("public System.Generic.List<System.DateTime> Array_1 { get; set; }");
            appsettings.generatedClass.Should().Contain("public System.Generic.List<string> Array_2 { get; set; }");

            // invalid identifier skip
            appsettings.generatedClass.Should().NotContain("public _object _object { get; set; }");
            generated.Select(x => x.fileName).Should().NotContain(x => x == "_object.cs");

        }        
    }
}

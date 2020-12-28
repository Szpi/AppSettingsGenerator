using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        public void test()
        {
            _configGenerator.Generate("appsettings.Development.json");
        }
    }
}

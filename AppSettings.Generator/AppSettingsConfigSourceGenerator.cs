using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AppSettings.Generator
{
    [Generator]
    public class AppSettingsConfigSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!context.Compilation.ReferencedAssemblyNames.Any(ai => ai.Name.Equals("Newtonsoft.Json", StringComparison.OrdinalIgnoreCase)))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("APG001", "Newtonsoft.Json not found", "Newtonsoft.Json dependency not found", "Compiler", DiagnosticSeverity.Error, true)
                    , null));
                return;
            }

            //Debugger.Launch();
            var resourceFiles = context.AdditionalFiles
                    .Where(f =>
                    Path.GetFileNameWithoutExtension(f.Path)?
                    .StartsWith("appSettings", System.StringComparison.OrdinalIgnoreCase) ?? false);

            var resourcePath = resourceFiles.FirstOrDefault().Path;
            var configGenerator = new ConfigGenerator();
            var generatedClasses = configGenerator.Generate(resourcePath);

            foreach (var generatedClass in generatedClasses)
            {
                context.AddSource(generatedClass.fileName,
                    SourceText.From(generatedClass.generatedClass, Encoding.UTF8));
            }
        }
    }
}

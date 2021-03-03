using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AppSettingsGenerator
{
    [Generator]
    public class AppSettingsConfigSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var resourceFiles = context.AdditionalFiles
                    .Where(f =>
                    Path.GetFileNameWithoutExtension(f.Path)?
                    .StartsWith("appSettings", System.StringComparison.OrdinalIgnoreCase) ?? false);

            if (!resourceFiles.Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("APG001", "AppSettings configuration not found", "AppSettings configuration not found. Please add appsettings.json file as AdditionalFiles property in project configuration (.csproj)", "Compiler", DiagnosticSeverity.Error, true)
                    , null));
                return;
            }

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

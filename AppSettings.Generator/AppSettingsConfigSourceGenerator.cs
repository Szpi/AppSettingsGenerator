using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
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
            var resourceFile = context.AdditionalFiles
                    .FirstOrDefault(f =>
                    Path.GetFileName(f.Path)?
                    .EndsWith(".json", System.StringComparison.OrdinalIgnoreCase) ?? false);

            if (resourceFile is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("APG001",
                    "AppSettings configuration not found",
                    @"AppSettings configuration not found. Please add proper configuration: <ItemGroup><AdditionalFiles Include=""appsettings.json""/></ItemGroup> to .csproj",
                    "AppSettingsGenerator.Compiler", 
                    DiagnosticSeverity.Error, 
                    true)
                    , null));
                return;
            }

            var resourcePath = resourceFile.Path;
            var configGenerator = new ConfigGenerator();
            var (generatedClasses, invalidIdentifiers) = configGenerator.Generate(resourcePath);

            foreach (var generatedClass in generatedClasses)
            {
                context.AddSource(generatedClass.fileName,
                    SourceText.From(generatedClass.generatedClass, Encoding.UTF8));
            }

            foreach (var (invalidIdentifierName, invalidIdentifierNamePath, sanitizedIdentifier) in invalidIdentifiers)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("APG002", "AppSettings invalid identifier", $"Invalid identifier detected {invalidIdentifierName} in path {invalidIdentifierNamePath}. This property was skipped during settings generation. You can access it via extension method over IConfiguration.Get{sanitizedIdentifier}()", "AppSettingsGenerator.Naming", DiagnosticSeverity.Info, true)
                    , null));
            }
        }
    }
}

using AppSettingsGenerator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
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

            if (!resourceFiles.Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("APG002", "AppSettings configuration not found", "AppSettings configuration not found. Please add appsettings.json file as AdditionalFiles property in project configuration (.csproj)", "Compiler", DiagnosticSeverity.Error, true)
                    , null));
                return;
            }

            var resourcePath = resourceFiles.FirstOrDefault().Path;
            var configGenerator = new ConfigGenerator();
            var generatedClasses = configGenerator.Generate(resourcePath);

            //GetEnumsToCheck(context);

            foreach (var generatedClass in generatedClasses)
            {
                context.AddSource(generatedClass.fileName,
                    SourceText.From(generatedClass.generatedClass, Encoding.UTF8));
            }
        }

        private static List<Type> GetEnumsToCheck(GeneratorExecutionContext context)
        {
            var attributeSymbol = context.Compilation.GetTypeByMetadataName(typeof(AppSettingsEnumAttribute).FullName);

            var classWithAttributeList = context.Compilation.SyntaxTrees.
                Where(st => st.GetRoot().DescendantNodes().OfType<EnumDeclarationSyntax>()
                    .Any(p => p.DescendantNodes().OfType<AttributeSyntax>().Any()));

            var enumTypesToCheck = new List<Type>();
            foreach (var tree in classWithAttributeList)
            {
                var semanticModel = context.Compilation.GetSemanticModel(tree);
                foreach (var declaredEnum in tree
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<EnumDeclarationSyntax>()
                    .Where(cd => cd.DescendantNodes().OfType<AttributeSyntax>().Any()))
                {
                    var nodes = declaredEnum
                    .DescendantNodes()
                    .OfType<AttributeSyntax>()
                    .FirstOrDefault(a => a.DescendantTokens().Any(dt => dt.IsKind(SyntaxKind.IdentifierToken) && semanticModel.GetTypeInfo(dt.Parent).Type.Name == attributeSymbol.Name))
                    ?.DescendantTokens()
                    ?.Where(dt => dt.IsKind(SyntaxKind.IdentifierToken))
                    ?.ToList();

                    if (nodes == null)
                    {
                        continue;
                    }
                    var className = declaredEnum.Identifier.Text;
                    var values = declaredEnum.Members.Select(x => x.Identifier.Text).ToList();

                    var namespaceSyntax = declaredEnum.Parent as NamespaceDeclarationSyntax;
                    var namepsaceText = namespaceSyntax?.Name.ToString();
                    var enumType = context.Compilation.GetTypeByMetadataName($"{namepsaceText}.{className}");
                    var test = Type.GetType($"{namepsaceText}.{className}");
                    var tes = Enum.Parse(test, "Test1");

                    var asdasdadsa = true;
                    //enumTypesToCheck.Add(enumType);
                }
            }
            return enumTypesToCheck;
        }
    }
}

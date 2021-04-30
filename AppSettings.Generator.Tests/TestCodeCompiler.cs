using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsGenerator.Tests
{
    class TestCodeCompiler
    {
        public (bool success, string reason) Compile(IEnumerable<string> generatedClasses)
        {
            // https://docs.microsoft.com/en-us/archive/msdn-magazine/2017/may/net-core-cross-platform-code-generation-with-roslyn-and-net-core

            var tree = generatedClasses.Select(code => SyntaxFactory.ParseSyntaxTree(code));
            var a = typeof(object).GetTypeInfo().Assembly.Location;
            var references = new List<MetadataReference>()
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IConfiguration).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IServiceCollection).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(OptionsConfigurationServiceCollectionExtensions).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ConfigurationBinder).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.5\netstandard.dll"),
            };

            Assembly.GetEntryAssembly()
                .GetReferencedAssemblies()
                .ToList()
                .ForEach(x => references.Add(MetadataReference.CreateFromFile(Assembly.Load(x).Location)));

            // A single, immutable invocation to the compiler
            // to produce a library
            var compilation = CSharpCompilation.Create("test.dll")
              .WithOptions(
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
              .AddReferences(references)
              .AddSyntaxTrees(tree);
            //string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            using var stream = new MemoryStream();
            EmitResult compilationResult = compilation.Emit(stream);

            return (compilationResult.Success, 
                string.Join(',', compilationResult.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error).Select(x => x.ToString())));           
        }
    }
}

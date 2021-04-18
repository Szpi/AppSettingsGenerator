using System;
using System.IO;
using System.Reflection;

namespace AppSettingsGenerator
{
    internal static class EmbeddedResource
    {
        static readonly string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string GetContent(string relativePath)
        {
            var filePath = Path.Combine(baseDir, Path.GetFileName(relativePath));
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);

            var baseName = Assembly.GetExecutingAssembly().GetName().Name;
            var resourceName = relativePath
                .TrimStart('.')
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');

            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(baseName + "." + resourceName);
            using var reader = new StreamReader(stream);
            if (stream == null)
                throw new NotSupportedException();
            return reader.ReadToEnd();
        }
    }
}
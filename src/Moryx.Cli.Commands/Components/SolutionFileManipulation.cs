using Moryx.Cli.Templates.Models;
using System.Xml.Linq;

namespace Moryx.Cli.Commands.Components
{
    internal class SolutionFileManipulation
    {
        public static void AddProjectToSolution(TemplateSettings settings, string? projectFileName)
        {
            if (projectFileName == null)
                return;

            var slnxFile = Path.Combine(settings.TargetDirectory, $"{settings.AppName}.slnx");
            var slnFile = Path.Combine(settings.TargetDirectory, $"{settings.AppName}.sln");

            if (File.Exists(slnxFile))
                AddToSlnx(slnxFile, projectFileName, settings.TargetDirectory);
            else if (File.Exists(slnFile))
                AddToSln(slnFile, projectFileName, settings.TargetDirectory);
        }

        private static void AddToSlnx(string solutionFileName, string projectFileName, string targetDirectory)
        {
            var relativePath = Path.GetRelativePath(targetDirectory, projectFileName)
                .Replace('\\', '/');
            var doc = XDocument.Load(solutionFileName);
            doc.Root!.Add(new XElement("Project", new XAttribute("Path", relativePath)));
            doc.Save(solutionFileName);
        }

        private static void AddToSln(string solutionFileName, string projectFileName, string targetDirectory)
        {
            // This is 'the' GUID used for `.csproj` inside `.sln`s. Got it from
            // `SolutionFile` of `Microsoft.Build.Construction`
            const string csharpProjectGuid = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";
            var guid = Guid.NewGuid().ToString("B").ToUpper();

            var filename = Path.GetFileNameWithoutExtension(projectFileName);
            var relativePath = Path.GetRelativePath(targetDirectory, projectFileName);
            var str = $"Project(\"{csharpProjectGuid}\") = \"{filename}\", \"{relativePath}\", \"{{{guid}}}\"\nEndProject";

            var lines = File.ReadAllLines(solutionFileName);
            using var fileWriter = new StreamWriter(solutionFileName);
            foreach (var line in lines)
            {
                if (line.StartsWith("Global"))
                    fileWriter.WriteLine(str);
                fileWriter.WriteLine(line);
            }
        }
    }
}

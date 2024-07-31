using Moryx.Cli.Template;
using Moryx.Cli.Template.Models;
using System.Text.RegularExpressions;

namespace Moryx.Cli.Commands
{
    public static class AddModule
    {
        public static CommandResult Exec(TemplateSettings settings, string moduleName)
        {
            return CommandBase.Exec(settings, (filenames) =>
                AddThing.Exec(
                settings,
                new AddConfig
                {
                    SolutionName = settings.AppName,
                    ThingName = moduleName,
                    Thing = "module",
                    ThingPlaceholders = [Template.Template.ModulePlaceholder],
                },
                filenames.Module(),
                s => AddProjectsToSolution(settings, s)
                ));            
        }

        private static void AddProjectsToSolution(TemplateSettings settings, IEnumerable<string> fileNames)
        {
            var projectFiles = fileNames
                .Where(f => f.EndsWith(".csproj"))
                .ToList();
            var solutionFilename = Path.Combine(settings.TargetDirectory, $"{settings.AppName}.sln");
            var rootGuid = GetRootGuid(solutionFilename);
            if(projectFiles.Any())
            {

                foreach (var file in projectFiles)
                {
                    var filename = Path.GetFileNameWithoutExtension(file);
                    var relativePath = Path.GetRelativePath(settings.TargetDirectory, file);
                    var str = $"Project(\"{rootGuid}\") = \"{filename}\", \"{relativePath}\", \"{{{Guid.NewGuid()}}}\"\r\nEndProject\r\n";
                    using var fileWriter = File.AppendText(solutionFilename);
                    fileWriter.Write(str);
                }
            }
        }

        private static string GetRootGuid(string solutionFilename)
        {
            var lines = File.ReadAllText(Path.Combine(solutionFilename));
            if (lines.Any())
            {
                var match = Regex.Match(lines, @"Project.+(\{.+\}).+App");
                return match.Groups[1].Value;
            }
            return "";
        }
    }
}
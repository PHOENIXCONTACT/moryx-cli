using Moryx.Cli.Templates.Models;

namespace Moryx.Cli.Commands.Components
{
    internal class SolutionFileManipulation
    {


        public static void AddProjectToSolution(TemplateSettings settings, string? projectFileName)
        {
            if (projectFileName == null)
                return;
            var solutionFileName = Path.Combine($"{settings.AppName}.sln");


            // It'd be prefered to utilize Roslyn to add the project to the
            // solution, but loading the solution/workspace in the first place
            // involves some analyzing and takes way too much time for being
            // used in a simple CLI command.

            // This is 'the' GUID used for `.csproj` inside `.sln`s. Got it from
            // `SolutionFile` of `Microsoft.Build.Construction`
            const string csharpProjectGuid = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";
            var guid = Guid.NewGuid().ToString("B").ToUpper();


            var filename = Path.GetFileNameWithoutExtension(projectFileName);
            var relativePath = Path.GetRelativePath(settings.TargetDirectory, projectFileName);
            var str = $"Project(\"{csharpProjectGuid}\") = \"{filename}\", \"{relativePath}\", \"{{{guid}}}\"\nEndProject";

            var lines = File.ReadAllLines(solutionFileName);
            using var fileWriter = new StreamWriter(solutionFileName);
            foreach (var line in lines)
            {
                if (line.StartsWith("Global"))
                {
                    fileWriter.WriteLine(str);
                }
                fileWriter.WriteLine(line);
            }
        }
    }
}

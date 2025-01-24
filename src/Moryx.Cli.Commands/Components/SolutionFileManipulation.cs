using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis;
using Moryx.Cli.Template.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Cli.Commands.Components
{
    internal class SolutionFileManipulation
    {


        public static void AddProjectToSolution(TemplateSettings settings, string? projectFileName)
        {
            if (projectFileName == null)
                return;
            var solutionFileName = Path.Combine(settings.AppName, $"{settings.AppName}.sln");


            // It'd be prefered to utilize Roslyn to add the project to the
            // solution, but loading the solution/workspace in the first place
            // involves some analyzing and takes way too much time for being
            // used in a simple CLI command.

            // This is 'the' GUID used for `.csproj` inside `.sln`s. Got it from
            // `SolutionFile` of `Microsoft.Build.Construction`
            const string csharpProjectGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
            var guid = Guid.NewGuid().ToString("B").ToUpper();


            var filename = Path.GetFileNameWithoutExtension(projectFileName);
            var relativePath = Path.GetRelativePath(settings.TargetDirectory, projectFileName);
            var str = $"Project(\"{csharpProjectGuid}\") = \"{filename}\", \"{relativePath}\", \"{{{guid}}}\"\nEndProject\n";

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

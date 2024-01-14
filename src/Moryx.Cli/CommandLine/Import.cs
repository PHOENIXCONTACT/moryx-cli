using Microsoft.Extensions.Options;
using Moryx.Cli.Commands.Options;
using Moryx.Cli.ImportFpe;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Moryx.Cli.CommandLine.AddStep;
using static Moryx.Cli.CommandLine.ImportDreso;

namespace Moryx.Cli.CommandLine
{
    [Description("Adds a step to your MORYX solution.")]
    internal class ImportDreso : Command<ImportDresoSettings>
    {
        internal class ImportDresoSettings : AddSettings
        {
            [Description("Export .xlsx file ")]
            [CommandOption("-p|--products")]
            public string? ProductsFile { get; set; }

            [Description("A comma separated list of steps to be created with the application project.")]
            [CommandOption("-s|--steps")]
            public string? StepsFile { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] ImportDresoSettings settings)
        {
            var addSteps = ParseProcessExport(settings.StepsFile, settings);

            foreach (var stepOptions in addSteps)
            {
                Commands.Add.Step(stepOptions);
            }

            return 0;
        }

        public List<AddOptions> ParseProcessExport(string processFile, ImportDresoSettings settings) 
        {
            var steps = ExcelParser.ParseExcel<ProcessDefinition>(processFile);
            return steps.Select(pd => new AddOptions
            {
                Name = pd.Name,
                Template = settings.Template,
                Branch = settings.Branch,
                Pull = settings.Pull,
            }).ToList();
        }
    }
}

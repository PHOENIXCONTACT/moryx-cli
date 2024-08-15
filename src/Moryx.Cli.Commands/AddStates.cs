using Moryx.Cli.Template;
using Moryx.Cli.Template.Models;
using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Template.StateBaseTemplate;
using Moryx.Cli.Template.Exceptions;

namespace Moryx.Cli.Commands
{
    internal class AddStates
    {
        const string StateBase = "StateBase";
        const string SpecificState = "SpecificState";

        internal static CommandResult Exec(TemplateSettings settings, string resource, IEnumerable<string> states, List<string> transitions)
        {
            return CommandBase.Exec(settings, (fileNames) => Add(settings, fileNames, resource, states));
        }

        private static CommandResult Add(TemplateSettings settings, List<string> cleanedResourceNames, string resource, IEnumerable<string> states)
        {
            if (!ResourceExists(settings, resource))
            {
                return CommandResult.WithError($"Resource `{resource}` not found. Make sure that a `{resource}.cs` exists in the project.");
            }

            var projectFileNames = cleanedResourceNames.InitialProjects();
            var StateBaseFileName = cleanedResourceNames.StateBaseFile();

            var dictionary = Template.Template.PrepareFileStructure(settings.AppName, StateBaseFileName, projectFileNames);
            var targetPath = Path.Combine(settings.TargetDirectory, "src", $"{settings.AppName}.Resources", resource);
            var newStateBaseFileName = Path.Combine(targetPath, $"{resource}StateBase.cs");

            if (!File.Exists(newStateBaseFileName))
            {
                var files = Template.Template.WriteFilesToDisk(
                    dictionary,
                    settings,
                    _ => newStateBaseFileName);

                Template.Template.ReplacePlaceHoldersInsideFiles(
                    files,
                    new Dictionary<string, string>
                    {
                        { Template.Template.AppPlaceholder, settings.AppName },
                        { Template.Template.StateBasePlaceholder, $"{resource}StateBase" },
                        { Template.Template.ResourcePlaceholder, resource },
                    });
            }

            var msg = new List<string>();
            var warnings = new List<string>();
            var stateBaseTemplate = StateBaseTemplate.FromFile(newStateBaseFileName);

            foreach (var state in states)
            {
                var stateType = state.ToLower().EndsWith("state")
                    ? state
                    : state + "State";

                stateType = stateType.Capitalize();

                try
                {
                    var stateFileName = cleanedResourceNames.StateFile();

                    dictionary = Template.Template.PrepareFileStructure(settings.AppName, stateFileName, projectFileNames);
                    var filename = Path.Combine(targetPath, $"{stateType}.cs");
                    if (!File.Exists(filename))
                    {
                        var files = Template.Template.WriteFilesToDisk(
                        dictionary,
                        settings,
                        _ => filename);

                        Template.Template.ReplacePlaceHoldersInsideFiles(
                            files,
                            new Dictionary<string, string>
                            {
                            { Template.Template.AppPlaceholder, settings.AppName },
                            { Template.Template.StatePlaceholder, stateType },
                            { Template.Template.ResourcePlaceholder, resource },
                            { Template.Template.StateBasePlaceholder, $"{resource}StateBase" },
                            });

                        stateBaseTemplate = stateBaseTemplate.AddState(stateType);

                        msg.Add($"Successfully added {stateType} state");
                    }
                    else
                    {
                        warnings.Add($"Could not add {stateType}. `{Path.GetFileName(filename)}` already exists!");
                    }
                }
                catch (Exception ex)
                {
                    warnings.Add($"Failed to add state `{stateType}`! " + ex.Message);
                }
            }

            stateBaseTemplate.SaveToFile(newStateBaseFileName);

            return CommandResult.IsOk(string.Join("\n", msg), string.Join("\n", warnings));
        }

        private static bool ResourceExists(TemplateSettings settings, string resource)
        {
            var files = Directory.GetFiles(
                Path.Combine(settings.TargetDirectory),
                $"{resource}.cs",
                new EnumerationOptions
                {
                    ReturnSpecialDirectories = false,
                    RecurseSubdirectories = true
                });
            return files.Length > 0;
        }
    }
}
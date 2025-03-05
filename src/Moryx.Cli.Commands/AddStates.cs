using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;
using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates.StateBaseTemplate;
using Moryx.Cli.Templates.Exceptions;
using Moryx.Cli.Templates.StateTemplate;
using Moryx.Cli.Templates.Components;
using Moryx.AbstractionLayer.Resources;

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
            var resourceFile = FindResource(settings, resource);
            if (string.IsNullOrEmpty(resourceFile))
            {
                return CommandResult.WithError($"`{resource}` not found. Make sure that a type `{resource}` exists in the project.");
            }

            var projectFileNames = cleanedResourceNames.InitialProjects();
            var stateBaseFileName = cleanedResourceNames.StateBaseFile();

            var dictionary = Template.PrepareFileStructure(settings.AppName, stateBaseFileName, projectFileNames);
            var targetPath = Path.Combine(Path.GetDirectoryName(resourceFile)!, "States");
            var newStateBaseFileName = Path.Combine(targetPath, $"{resource}StateBase.cs");

            if (!File.Exists(newStateBaseFileName))
            {
                var files = Template.WriteFilesToDisk(
                    dictionary,
                    settings,
                    _ => newStateBaseFileName);

                Template.ReplacePlaceHoldersInsideFiles(
                    files,
                    new Dictionary<string, string>
                    {
                        { Template.AppPlaceholder, settings.AppName },
                        { Template.StateBasePlaceholder, $"{resource}StateBase" },
                        { Template.CellPlaceholder, $"{resource}" },
                        { Template.ResourcePlaceholder, resource },
                    });

                if (!states.Any())
                {
                    states =
                    [
                        "Idle",
                        "ReadyToWork",
                        "Running",
                        "ProcessAborting",
                        "SessionComplete",
                    ];
                }
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

                    dictionary = Template.PrepareFileStructure(settings.AppName, stateFileName, projectFileNames);
                    var filename = Path.Combine(targetPath, $"{stateType}.cs");
                    if (!File.Exists(filename))
                    {
                        var files = Template.WriteFilesToDisk(
                        dictionary,
                        settings,
                        _ => filename);

                        Template.ReplacePlaceHoldersInsideFiles(
                            files,
                            new Dictionary<string, string>
                            {
                            { Template.AppPlaceholder, settings.AppName },
                            { Template.StatePlaceholder, stateType },
                            { Template.ResourcePlaceholder, resource },
                            { Template.CellPlaceholder, resource },
                            { Template.StateBasePlaceholder, $"{resource}StateBase" },
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

            UpdateResource(
                settings,
                resource,
                success => msg.Add(success),
                warning => warnings.Add(warning));

            return CommandResult.IsOk(string.Join("\n", msg), string.Join("\n", warnings));
        }

        private static void UpdateResource(TemplateSettings settings, string resource, Action<string> onSuccess, Action<string> onWarning)
        {
            var candidate = FindResource(settings, resource);

            if (string.IsNullOrEmpty(candidate))
            {
                onWarning($"Type `{resource}` not found. Could not update resource.");
                return;
            }

            try
            {
                var template = StateTemplate.FromFile(candidate);
                template = template.ImplementIStateContext(resource);
                template.SaveToFile(candidate);
                onSuccess($"Updated `{Path.GetFileName(candidate)}`");
            }
            catch (Exception)
            {
                onWarning($"Failed to update `{Path.GetFileName(candidate)}`");
            }
        }

        private static string FindResource(TemplateSettings settings, string resourceName)
        {
            var dir = Path.Combine(settings.TargetDirectory, "src");
            var files = Directory.GetFiles(
                dir,
                $"*.cs",
                new EnumerationOptions
                {
                    ReturnSpecialDirectories = false,
                    RecurseSubdirectories = true
                })
                .AsEnumerable();

            var promisingCandidate = files.FirstOrDefault(f => f.EndsWith($"{resourceName}.cs"));
            if (!string.IsNullOrEmpty(promisingCandidate))
            {
                files = files.Prepend(promisingCandidate);
            }

            var candidate = string.Empty;
            foreach (var file in files)
            {
                if (ContainsType(file, resourceName))
                {
                    candidate = file;
                    break;
                }
            }
            return candidate;
        }

        private static bool ContainsType(string filename, string typeName)
        {
            try
            {
                var file = CSharpFile.FromFile(filename);
                return file.Types.Any(t => t == typeName);
            }
            catch
            {
                return false;
            }
        }
    }
}
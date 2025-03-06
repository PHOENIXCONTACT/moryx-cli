using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;
using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates.StateBaseTemplate;
using Moryx.Cli.Templates.StateTemplate;
using Moryx.Cli.Templates.Components;

namespace Moryx.Cli.Commands
{
    internal class AddStates
    {
        const string StateBase = "StateBase";
        const string SpecificState = "SpecificState";

        internal static CommandResult Exec(Template template, string resource, IEnumerable<string> states, List<string> transitions)
        {
            return CommandBase.Exec(template, () => Add(template, resource, states));
        }

        private static CommandResult Add(Template template, string resource, IEnumerable<string> states)
        {
            var resourceFile = FindResource(template.Settings, resource);
            if (string.IsNullOrEmpty(resourceFile))
            {
                return CommandResult.WithError($"`{resource}` not found. Make sure that a type `{resource}` exists in the project.");
            }


            var dictionary = template.StateBaseFile(resource);
            var targetPath = Path.Combine(Path.GetDirectoryName(resourceFile)!, "States");
            var newStateBaseFileName = Path.Combine(
                targetPath,
                Path.GetFileName(dictionary.Single().Value));

            dictionary = new Dictionary<string, string> { { dictionary.FirstOrDefault().Key, newStateBaseFileName } };
            
            if (!File.Exists(newStateBaseFileName))
            {
                var files = template.WriteFilesToDisk(dictionary);

                Template.ReplacePlaceHoldersInsideFiles(
                    files,
                    template.Configuration.Add.StateBase.Replacements);

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
                    var stateFiles = template.StateFile(state, resource);
                    stateFiles = stateFiles
                        .Select(file => new { file.Key, Value = Path.Combine(targetPath, $"{stateType}.cs")})
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                        

                            var filename = Path.Combine(targetPath, $"{stateType}.cs");
                    if (!File.Exists(filename))
                    {
                        var files = template.WriteFilesToDisk(stateFiles);

                        Template.ReplacePlaceHoldersInsideFiles(
                            files,
                            template.Configuration.Add.State.Replacements);

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
                template.Settings,
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
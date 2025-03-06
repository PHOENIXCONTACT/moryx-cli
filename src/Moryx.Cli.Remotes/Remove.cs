using Moryx.Cli.Commands;
using Moryx.Cli.Templates;

namespace Moryx.Cli.Remotes
{
    public class Remove
    {
        public static CommandResult Remote(RemoveOptions options, Action<string> onStatus)
        {
            CommandResult result = new();
            var dir = Directory.GetCurrentDirectory();
            var solutionNameError = string.Empty;
            Templates.Solution.AssertSolution(
                dir,
                then => result = RemoveRemote(dir, options, onStatus),
                error => result = CommandResult.WithError(error)
            );
            return result;
        }

        private static CommandResult RemoveRemote(string dir, RemoveOptions options, Action<string> onStatus)
        {
            var remote = options.Name!;
            var localConfig = Config.Models.Configuration.Load(dir);

            if (!localConfig.Profiles.ContainsKey(remote))
            {
                return CommandResult.WithError($"A remote `{remote}` doesn't exist.");
            }
            if (localConfig.Profiles.Count == 1)
            {
                return CommandResult.WithError($"There must be at least on remote configured.");
            }

            localConfig.Profiles.Remove(remote);
            if (localConfig.DefaultProfile == remote)
            {
                localConfig.DefaultProfile = localConfig.Profiles.First().Key;
                onStatus($"Set `{localConfig.DefaultProfile}` as default.");
            }
            localConfig.Save(dir);

            return CommandResult.IsOk($"Successfully removed `{remote}`.");
        }
    }
}

using Moryx.Cli.Commands;
using Moryx.Cli.Templates;

namespace Moryx.Cli.Remotes
{
    public class Use
    {
        public static CommandResult Remote(UseOptions options)
        {
            CommandResult result = new();
            var dir = Directory.GetCurrentDirectory();
            var solutionNameError = string.Empty;
            Solution.AssertSolution(
                dir,
                then => result = UseRemote(dir, options),
                error => result = CommandResult.WithError(error)
            );
            return result;
        }

        private static CommandResult UseRemote(string dir, UseOptions options)
        {
            var remote = options.Name!;
            var localConfig = Config.Models.Configuration.Load(dir);

            if (!localConfig.Profiles.ContainsKey(remote))
            {
                return CommandResult.WithError($"A remote `{remote}` doesn't exist.");
            }

            localConfig.DefaultProfile = remote;
            localConfig.Save(dir);

            return CommandResult.IsOk($"Now using `{remote}` by default.");
        }
    }
}
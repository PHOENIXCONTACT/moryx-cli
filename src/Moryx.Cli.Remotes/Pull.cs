using Moryx.Cli.Commands;
using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;

namespace Moryx.Cli.Remotes
{
    public class Pull
    {
        public static CommandResult Remote(PullOptions options, Action<string> onStatus)
        {
            CommandResult result = new();
            var dir = Directory.GetCurrentDirectory();
            Templates.Solution.AssertSolution(
                dir,
                then => result = PullRemote(dir, options, onStatus),
                error => result = CommandResult.WithError(error)
            );
            return result;
        }

        private static CommandResult PullRemote(string dir, PullOptions options, Action<string> onStatus)
        {
            string remote = options.Name ?? "";
            var config = Config.Models.Configuration.Load(dir);
            var solutionName = Templates.Solution.GetSolutionName(dir, _ => { });

            Templates.Models.TemplateSettings settings = config.AsTemplateSettings(dir, solutionName, remote);
            settings.Pull = true;

            var result = TemplateRepository.Clone(settings, onStatus);
            if (result == 0)
            {
                return CommandResult.IsOk("Updated remote");
            }
            else
            {
                return CommandResult.WithError("Could not updated remote");
            }
        }
    }
}
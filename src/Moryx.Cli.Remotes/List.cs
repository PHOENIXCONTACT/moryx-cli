using Moryx.Cli.Commands;
using Moryx.Cli.Templates;

namespace Moryx.Cli.Remotes
{
    public class List
    {
        public static CommandResult Get(Action<string> onStatus)
        {
            CommandResult result = new();
            var currentDir = Environment.CurrentDirectory;
            Templates.Solution.Assert(
                currentDir,
                then => result = ListRemotes(currentDir, onStatus),
                error => result = CommandResult.WithError(error)
            );

            return result;
        }

        private static CommandResult ListRemotes(string dir, Action<string> onStatus)
        {
            var config = Config.Models.Configuration.Load(dir);

            var stringList =
                config?.Profiles?.Select(p => (p.Key == config.DefaultProfile ? "* " : "  ")
                    + $"{p.Key}\t{p.Value.Repository}@{p.Value.Branch}").ToList() ?? [];

            foreach (var profile in stringList)
            {
                onStatus(profile);
            }

            return CommandResult.IsOk("");
        }
    }
}

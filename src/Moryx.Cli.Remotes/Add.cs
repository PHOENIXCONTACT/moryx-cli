using Moryx.Cli.Commands;
using Moryx.Cli.Templates;

namespace Moryx.Cli.Remotes
{
    public class Add
    {
        public static CommandResult Remote(AddOptions options)
        {
            CommandResult result = new();
            var dir = Directory.GetCurrentDirectory();
            Solution.AssertSolution(
                dir,
                then => result = AddRemote(dir, options),
                error => result = CommandResult.WithError(error)
            );
            return result;
        }

        private static CommandResult AddRemote(string dir, AddOptions options)
        {
            var remote = options.Name!;
            var localConfig = Config.Models.Configuration.Load(dir);

            if (localConfig.Profiles.ContainsKey(remote))
            {
                return CommandResult.WithError($"A remote `{remote}` already exists.");
            }

            localConfig.Profiles.Add(
                remote,
                new Config.Models.Profile
                {
                    Repository = options.Repository!,
                    Branch = options.Branch!
                });
            localConfig.Save(dir);

            return CommandResult.IsOk($"Successfully added `{remote}`.");
        }
    }
}

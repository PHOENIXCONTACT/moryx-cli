namespace Moryx.Cli.Commands.Extensions
{
    internal static class CommandResultExtensions
    {
        internal static CommandResult Flatten(this CommandResult[] results) {
            if (results.Any(r => r.Error != null))
            {
                return CommandResult.WithError(string.Join("\n", results.Select(r => r.Error)));
            }
            return CommandResult.IsOk(string.Join("\n", results.Select(r => r.Success)));
        }
    }
}

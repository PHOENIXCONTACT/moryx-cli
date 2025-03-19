namespace Moryx.Cli.Commands.Extensions
{
    internal static class CommandResultExtensions
    {
        internal static CommandResult Flatten(this CommandResult[] results)
        {
            if (results.Any(r => r.Errors.Count > 0))
            {
                return CommandResult.WithError(string.Join("\n", results.Select(r => string.Join("\n", r.Errors))));
            }
            return CommandResult.IsOk(string.Join("\n", results.Select(r => r.Success)), string.Join("\n", results.Select(r => r.Warning)));
        }
    }
}

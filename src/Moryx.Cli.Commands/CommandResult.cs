using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moryx.Tools;

namespace Moryx.Cli.Commands
{
    public class CommandResult
    {
        private int ReturnCode { get; set; }
        public string? Success { get; private set; }
        public string? Warning { get; private set; }
        public IList<string> Errors { get; private set; } = [];

        public static CommandResult WithError(string error)
            => new()
            {
                Success = null,
                Errors = [error],
                Warning = null,
                ReturnCode = 1,
            };

        public static CommandResult IsOk(string message, string? warning = null)
            => new()
            {
                Success = message,
                Warning = warning,
                ReturnCode = 0,
            };

        public CommandResult CouldHaveIssues(IReadOnlyCollection<CommandResult> results)
        {
            foreach (var result in results)
            {
                result.OnError(Errors.Add);
            }
            return this;
        }

        public CommandResult OnError(Action<string> action)
        {
            if (Errors.Count > 0)
                foreach (var error in Errors)
                    action(error);
            return this;
        }

        public CommandResult OnSuccess(Action<string, string?> action)
        {
            if (Success != null)
                action(Success, Warning);
            return this;
        }

        public int ReturnValue()
        {
            return ReturnCode;
        }
    }
}

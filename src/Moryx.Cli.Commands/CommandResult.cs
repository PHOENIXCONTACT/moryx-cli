namespace Moryx.Cli.Commands
{
    public class CommandResult
    {
        private int ReturnCode { get; set; }
        public string? Success { get; private set; }
        public string? Warning { get; private set; }
        public string? Error { get; private set; }

        public static CommandResult WithError(string error) 
            => new()
            {
                Success = null,
                Error = error,
                Warning = null,
                ReturnCode = 1,
            };

        public static CommandResult IsOk(string message, string? warning = null)
            => new()
            {
                Success = message,
                Warning = warning,
                Error = null,
                ReturnCode = 0,
            };

        public CommandResult OnError(Action<string> action)
        {
            if (Error != null)
                action(Error);
            return this;
        }

        public CommandResult OnSuccess(Action<string, string?> action)
        {
            if(Success != null)
                action(Success, Warning);
            return this;
        }

        public int ReturnValue()
        {
            return ReturnCode;
        }
    }
}

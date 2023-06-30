namespace Moryx.Cli.Commands
{
    public class CommandResult
    {
        private int ReturnCode { get; set; }
        public string? Success { get; private set; }
        public string? Error { get; private set; }

        public static CommandResult WithError(string error) 
            => new CommandResult
            {
                Success = null,
                Error = error,
                ReturnCode = 1,
            };
        
        public static CommandResult IsOk(string message) 
            => new CommandResult
            {
                Success = message,
                Error = null,
                ReturnCode = 0,
            };
        
        public CommandResult OnError(Action<string> action)
        {
            if (Error != null)
                action(Error);
            return this;
        }

        public CommandResult OnSuccess(Action<string> action)
        {
            if(Success != null)
                action(Success);
            return this;
        }

        public int ReturnValue()
        {
            return ReturnCode;
        }
    }
}

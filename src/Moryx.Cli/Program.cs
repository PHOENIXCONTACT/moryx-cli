using Moryx.Cli.CommandLine;
using Spectre.Console.Cli;

namespace Moryx.Cli
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var app = new CommandApp()
                .Setup();
            app.Run(args);
        }
    }
}
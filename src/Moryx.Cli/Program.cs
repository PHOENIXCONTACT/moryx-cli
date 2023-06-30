using CommandLine;
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
            //Parser.Default.ParseArguments<NewOptions, AddOptions, ExecOptions>(args)
            //    .WithParsed<NewOptions>(CreateNew.Solution)
            //    .WithParsed<AddOptions>(Add.Exec)
            //    .WithParsed<ExecOptions>(ExecCommand.Exec)
            //    .WithNotParsed(errors => { });
        }
    }
}
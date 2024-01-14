using Spectre.Console.Cli;

namespace Moryx.Cli.CommandLine
{
    internal static class CommandLineSetup
    {
        public static CommandApp Setup(this CommandApp app)
        {
            app.Configure(config =>
            {
                //config.AddDebugConfigIfDebug();

                config.AddCommand<New>("new")
                    .WithAlias("n")
                    .WithExample(new[] { "new", "<NAME>" , "--products", "<PRODUCTS>", "--steps", "<STEPS>", "--no-git-init" });
                config.AddBranch<AddSettings>("add", add =>
                {
                    add.AddCommand<AddProduct>("product")
                        .WithExample(new[] { "add", "product", "<NAME>" });
                    add.AddCommand<AddStep>("step")
                        .WithExample(new[] { "add", "step", "<NAME>" });
                    add.AddCommand<AddModule>("module")
                        .WithExample(new[] { "add", "module", "<NAME>" })
                        .WithExample(new[] { "add", "adapter", "<NAME>" })
                        ;
                    add.AddCommand<AddModule>("adapter")
                        .WithExample(new[] { "add", "adapter", "<NAME>" })
                        ;
                    add.SetDescription("Adds certain subjects to the project. See `moryx add --help` for more details.");
                })
                    .WithAlias("a");
                config.AddCommand<ImportDreso>("import")
                    .WithAlias("i")
                    .WithExample(new[] { "import", "-s", "Path/To/Steps/File.xlsx" });
                config.AddCommand<Exec>("exec")
                    .WithExample(new[] { "exec", "<COMMAND>", "--endpoint", "<ENDPOINT>" })
                    .IsHidden();
                config.SetApplicationName("moryx");
            });
            return app;
        }


        private static IConfigurator AddDebugConfigIfDebug(this IConfigurator config)
        {
#if DEBUG
            config.PropagateExceptions();
            config.ValidateExamples();
#endif
            return config;
        }
    }
}

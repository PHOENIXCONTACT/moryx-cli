using Spectre.Console.Cli;

namespace Moryx.Cli.CommandLine
{
    internal static class CommandLineSetup
    {
        public static CommandApp Setup(this CommandApp app)
        {
            app.Configure(config =>
            {
                config.AddDebugConfigIfDebug();

                config.AddCommand<New>("new")
                    .WithAlias("n")
                    .WithExample(new[] { "new", "<NAME>" , "--products", "<PRODUCTS>", "--steps", "<STEPS>", "--no-git-init" });
                config.AddBranch<AddSettings>("add", add =>
                {
                    add.AddCommand<AddProduct>("product")
                        .WithExample(new[] { "add", "product", "<NAME>" });
                    add.AddCommand<AddResources>("resource")
                        .WithExample(new[] { "add", "resource", "<NAME>" });
                    add.AddCommand<AddStep>("step")
                        .WithExample(new[] { "add", "step", "<NAME>" });
                    add.AddCommand<AddModule>("module")
                        .WithExample(new[] { "add", "module", "<NAME>" })
                        .WithExample(new[] { "add", "adapter", "<NAME>" })
                        ;
                    add.AddCommand<AddModule>("adapter")
                        .WithExample(new[] { "add", "adapter", "<NAME>" })
                        ;
                    add.AddCommand<AddStates>("states")
                        .WithExample(new[] { "add", "states", "<RESOURCE>" })
                        .WithExample(new[] { "add", "states", "<RESOURCE>", "--states", "\"Idle, Running, Failure\"" })
                        ;
                    add.SetDescription("Adds certain subjects to the project. See `moryx add --help` for more details.");
                })
                    .WithAlias("a");

                config.AddBranch<CommandSettings>("remotes", remotes =>
                {
                    remotes.SetDescription("Manages remote profiles");
                    remotes.SetDefaultCommand<RemotesList>();
                    remotes.AddCommand<RemotesAdd>("add")
                        .WithExample(["remotes", "add", "custom", "https://example.com/repo.git", "main"])
                        ;
                    remotes.AddCommand<RemotesRemove>("remove")
                        .WithExample(["remotes", "remove", "custom"])
                        ;
                    remotes.AddCommand<RemotesUse>("use")
                        .WithExample(["remotes", "use", "custom"])
                        ;
                });

                config.AddCommand<Pull>("pull")
                        .WithExample(["pull", "<REMOTE>"])
                        ;

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

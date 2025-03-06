using Moryx.Cli.Templates.Models;
using System.Text.Json;

namespace Moryx.Cli.Templates
{
    public class TemplateConfigurationFactory
    {
        public static TemplateConfiguration Default()
        {
            return new TemplateConfiguration
            {
                Version = "0.0.1",
                New = new NewConfiguration
                {
                    Files = [
                        @"*",
                        @"docs\*",
                        @"src\MyApplication\*",
                        @"src\MyApplication.App\*",
                        @"src\MyApplication.App\Config\*",
                        @"src\MyApplication.App\Properties\*",
                        @"src\MyApplication.App\wwwroot\*",
                        @"src\MyApplication.ControlSystem\*",
                        @"src\MyApplication.Orders\*",
                        @"src\MyApplication.Products\Importer\*",
                        @"src\MyApplication.Products\*",
                        @"src\MyApplication.Resources\*.csproj",
                        @"src\Tests\MyApplication.Tests\*.csproj",
                    ],
                    Replacements = {
                        { "MyApplication", "{solutionname}" },
                    }
                },
                Add = new AddConfigurations
                {
                    Product = new AddConfiguration
                    {
                        Files = [@"**\MyProductInstance.cs", @"**\MyProductType.cs"],
                        Replacements = {
                            { "MyProduct", "{id}" },
                        }
                    },
                    Resource = new AddConfiguration
                    {
                        Files = [@"**\*ISomeResource*", @"**\*MyResource*"],
                        Replacements = {
                            { "SomeResource", "{id}Resource" },
                            { "MyResource", "{id}Resource" }
                        }
                    },
                    Step = new AddConfiguration
                    {
                        Files = [
                            @"**\*Some*",
                            @"**\SimulatedInOutDriver.cs",
                            @"**\MyApplication.Resources.csproj",
],
                        Replacements = {
                            { "Some", "{id}" },
                            { "MyApplication.Resources", "{solutionname}.Resources.{id}" }
                        }
                    },
                    Module = new AddConfiguration
                    {
                        Files = [
                            @"**\*MyModule**\*",
                        ],
                        Replacements = {
                            { "MyModule", "{id}" },
                        }
                    },
                    State =
                    {
                        Files = [
                            @"**\*State.cs",
                        ],
                        Replacements = {
                            { "SpecificState", "{id}State" },
                            { "SomeResource", "{resource}" },
                            { "SomeCell", "{resource}" },
                            { "SomeStateBase", "{resource}StateBase" },
                        }, 
                    },
                    StateBase =
                    {
                        Files = [
                            @"**\StateBase.cs",
                        ],
                        Replacements = {
                            { "SomeResource", "{resource}" },
                            { "SomeCell", "{resource}" },
                            { "SomeStateBase", "{resource}StateBase" },
                            { "StateBase.cs", "{resource}StateBase.cs" },
                        }
                    },
                },
            };
        }

        public static TemplateConfiguration? Load(string filePath, Action<string>? onError = null)
            {
                var fileName = GetFileName(filePath);
                try
                {
                    using var file = File.OpenRead(fileName);
                    var template = JsonSerializer.Deserialize(file, typeof(TemplateConfiguration)) as TemplateConfiguration;
                    return template;
                }
                catch
                {
                    onError?.Invoke($"Failed to load template at `{fileName}`");
                }

                return null;
            }

            private static string GetFileName(string filePath)
            {
                if (!filePath.EndsWith(".moryxtpl"))
                {
                    return Path.Combine(filePath, ".moryxtpl");
                }

                return filePath;
            }
        }
    }

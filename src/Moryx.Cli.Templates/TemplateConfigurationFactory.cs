using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.Models;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Moryx.Cli.Templates
{
    public class TemplateConfigurationFactory
    {
        public static TemplateConfiguration Default()
        {
            return new TemplateConfiguration
            {
                Version = "0.0.1",
                Placeholders = {
                    SolutionName = "MyApplication" },
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
                        Files = ["*MyProductInstance.cs", "*MyProductType.cs"],
                        Replacements = {
                            { "MyProduct", "{product}" },
                        }
                    },
                    Resource = new AddConfiguration
                    {
                        Files = ["*ISomeResource*", "*MyResource*"],
                        Replacements = {
                            { "SomeResource", "{resource}Resource" },
                            { "MyResource", "{resource}Resource" }
                        }
                    }
                }
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

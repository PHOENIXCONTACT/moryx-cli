using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.Models;
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
    }
}

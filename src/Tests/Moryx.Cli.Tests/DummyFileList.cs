using Moryx.Cli.Templates.Extensions;

namespace Moryx.Cli.Tests
{
    internal class DummyFileList
    {
        internal static List<string> Get()
            => new List<string>()
            {
                @"C:\<path>\.gitignore",
                @"C:\<path>\.moryxtpl",
                @"C:\<path>\Directory.Build.props",
                @"C:\<path>\Directory.Build.targets",
                @"C:\<path>\MyApplication.sln",
                @"C:\<path>\NuGet.Config",
                @"C:\<path>\README.md",
                @"C:\<path>\docs\.gitkeep",
                @"C:\<path>\src\MyApplication\Activities\SomeStep\SomeActivity.cs",
                @"C:\<path>\src\MyApplication\Activities\SomeStep\SomeActivityResults.cs",
                @"C:\<path>\src\MyApplication\Activities\SomeStep\SomeParameters.cs",
                @"C:\<path>\src\MyApplication\Activities\SomeStep\SomeTask.cs",
                @"C:\<path>\src\MyApplication\Capabilities\SomeCapabilities.cs",
                @"C:\<path>\src\MyApplication\Products\MyProductInstance.cs",
                @"C:\<path>\src\MyApplication\Products\MyProductType.cs",
                @"C:\<path>\src\MyApplication\Recipes\MyApplicationRecipe.cs",
                @"C:\<path>\src\MyApplication\Resources\ISomeResource.cs",
                @"C:\<path>\src\MyApplication\MyApplication.csproj",
                @"C:\<path>\src\MyApplication.App\Config\Moryx.Products.Management.ModuleConfig.json",
                @"C:\<path>\src\MyApplication.App\Config\Moryx.Products.Model.ProductsContext.DbConfig.json",
                @"C:\<path>\src\MyApplication.App\Config\Moryx.Resources.Model.ResourcesContext.DbConfig.json",
                @"C:\<path>\src\MyApplication.App\Properties\launchSettings.json",
                @"C:\<path>\src\MyApplication.App\wwwroot\favicon.ico",
                @"C:\<path>\src\MyApplication.App\appsettings.Development.json",
                @"C:\<path>\src\MyApplication.App\appsettings.json",
                @"C:\<path>\src\MyApplication.App\MyApplication.App.csproj",
                @"C:\<path>\src\MyApplication.App\Program.cs",
                @"C:\<path>\src\MyApplication.App\Startup.cs",
                @"C:\<path>\src\MyApplication.ControlSystem\MyApplication.ControlSystem.csproj",
                @"C:\<path>\src\MyApplication.ControlSystem\CellSelector\MyCellSelector.cs",
                @"C:\<path>\src\MyApplication.ControlSystem\CellSelector\MyCellSelectorConfig.cs",
                @"C:\<path>\src\MyApplication.ControlSystem\SetupTriggers\MySetupTrigger.cs",
                @"C:\<path>\src\MyApplication.ControlSystem\SetupTriggers\MySetupTriggerConfig.cs",
                @"C:\<path>\src\MyApplication.MyModule\Components\IMyComponent.cs",
                @"C:\<path>\src\MyApplication.MyModule\Facade\IMyFacade.cs",
                @"C:\<path>\src\MyApplication.MyModule\Facade\MyFacade.cs",
                @"C:\<path>\src\MyApplication.MyModule\Implementation\MyComponent.cs",
                @"C:\<path>\src\MyApplication.MyModule\ModuleController\ModuleConfig.cs",
                @"C:\<path>\src\MyApplication.MyModule\ModuleController\ModuleConsole.cs",
                @"C:\<path>\src\MyApplication.MyModule\ModuleController\ModuleController.cs",
                @"C:\<path>\src\MyApplication.MyModule\MyApplication.MyModule.csproj",
                @"C:\<path>\src\MyApplication.Orders\MyApplication.Orders.csproj",
                @"C:\<path>\src\MyApplication.Orders\MyApplicationProductAssignment.cs",
                @"C:\<path>\src\MyApplication.Orders\MyApplicationRecipeAssignment.cs",
                @"C:\<path>\src\MyApplication.Products\Importer\MyApplicationImportParameters.cs",
                @"C:\<path>\src\MyApplication.Products\Importer\MyApplicationProductImporter.cs",
                @"C:\<path>\src\MyApplication.Products\Importer\MyApplicationProductImporterConfig.cs",
                @"C:\<path>\src\MyApplication.Products\MyApplication.Products.csproj",
                @"C:\<path>\src\MyApplication.Resources\MyApplication.Resources.csproj",
                @"C:\<path>\src\MyApplication.Resources\MyResource.cs",
                @"C:\<path>\src\MyApplication.Resources\StateBase.cs",
                @"C:\<path>\src\MyApplication.Resources\SpecificState.cs",
                @"C:\<path>\src\MyApplication.Resources\SimulatedInOutDriver.cs",
                @"C:\<path>\src\MyApplication.Resources\SomeCell.cs",
                @"C:\<path>\src\Tests\MyApplication.Tests\MyApplication.Tests.csproj",
                @"C:\<path>\src\Tests\MyApplication.Tests\SomeResourceTest.cs",
                @"C:\<path>\src\Tests\MyApplication.Tests\MyResourceTest.cs",
            }
            .Select(s => s.Replace('\\', Path.DirectorySeparatorChar))
            .ToList();

        internal static string SourceDir()
           => @"C:\<path>\".OsAware();
    }
}


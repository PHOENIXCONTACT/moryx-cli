using Moryx.Cli.Commands;
using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.StateBaseTemplate;
using Moryx.Cli.Tests.Extensions;

namespace Moryx.Cli.Tests.CommandTests
{
    public class CreateNewTests
    {
        private const int InitialProjectFilesCount = 28;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckInitialProjectFilesCount()
        {
            var filteredNames = CreateNew.FilteredResourceNames(DummyFileList.Get());

            Assert.That(filteredNames, Has.Count.EqualTo(InitialProjectFilesCount));

        }

        [Test]
        public void CheckInitialFiles()
        {
            var filteredNames = CreateNew.FilteredResourceNames(DummyFileList.Get());
            filteredNames = filteredNames.Select(s => s.Replace(@"C:\<path>\".OsAware(), "")).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(filteredNames[0], Is.EqualTo(@".gitignore".OsAware()));
                Assert.That(filteredNames[1], Is.EqualTo(@"Directory.Build.props".OsAware()));
                Assert.That(filteredNames[2], Is.EqualTo(@"Directory.Build.targets".OsAware()));
                Assert.That(filteredNames[3], Is.EqualTo(@"MyApplication.sln".OsAware()));
                Assert.That(filteredNames[4], Is.EqualTo(@"NuGet.Config".OsAware()));
                Assert.That(filteredNames[5], Is.EqualTo(@"README.md".OsAware()));
                Assert.That(filteredNames[6], Is.EqualTo(@"docs\.gitkeep".OsAware()));
                Assert.That(filteredNames[7], Is.EqualTo(@"src\MyApplication\MyApplication.csproj".OsAware()));
                Assert.That(filteredNames[8], Is.EqualTo(@"src\MyApplication.ControlSystem\MyApplication.ControlSystem.csproj".OsAware()));
                Assert.That(filteredNames[9], Is.EqualTo(@"src\MyApplication.App\appsettings.Development.json".OsAware()));
                Assert.That(filteredNames[10], Is.EqualTo(@"src\MyApplication.App\appsettings.json".OsAware()));
                Assert.That(filteredNames[11], Is.EqualTo(@"src\MyApplication.App\MyApplication.App.csproj".OsAware()));
                Assert.That(filteredNames[12], Is.EqualTo(@"src\MyApplication.App\Program.cs".OsAware()));
                Assert.That(filteredNames[13], Is.EqualTo(@"src\MyApplication.App\Startup.cs".OsAware()));
                Assert.That(filteredNames[14], Is.EqualTo(@"src\MyApplication.Orders\MyApplication.Orders.csproj".OsAware()));
                Assert.That(filteredNames[15], Is.EqualTo(@"src\MyApplication.Orders\MyApplicationProductAssignment.cs".OsAware()));
                Assert.That(filteredNames[16], Is.EqualTo(@"src\MyApplication.Orders\MyApplicationRecipeAssignment.cs".OsAware()));
                Assert.That(filteredNames[17], Is.EqualTo(@"src\MyApplication.Products\MyApplication.Products.csproj".OsAware()));
                Assert.That(filteredNames[18], Is.EqualTo(@"src\MyApplication.App\Config\Moryx.Products.Management.ModuleConfig.json".OsAware()));
                Assert.That(filteredNames[19], Is.EqualTo(@"src\MyApplication.App\Config\Moryx.Products.Model.ProductsContext.DbConfig.json".OsAware()));
                Assert.That(filteredNames[20], Is.EqualTo(@"src\MyApplication.App\Config\Moryx.Resources.Model.ResourcesContext.DbConfig.json".OsAware()));
                Assert.That(filteredNames[21], Is.EqualTo(@"src\MyApplication.App\Properties\launchSettings.json".OsAware()));
                Assert.That(filteredNames[22], Is.EqualTo(@"src\MyApplication.App\wwwroot\favicon.ico".OsAware()));
                Assert.That(filteredNames[23], Is.EqualTo(@"src\MyApplication.Products\Importer\MyApplicationImportParameters.cs".OsAware()));
                Assert.That(filteredNames[24], Is.EqualTo(@"src\MyApplication.Products\Importer\MyApplicationProductImporter.cs".OsAware()));
                Assert.That(filteredNames[25], Is.EqualTo(@"src\MyApplication.Products\Importer\MyApplicationProductImporterConfig.cs".OsAware()));
                Assert.That(filteredNames[26], Is.EqualTo(@"src\Tests\MyApplication.Tests\MyApplication.Tests.csproj".OsAware()));
                Assert.That(filteredNames[27], Is.EqualTo(@"src\MyApplication.Resources\MyApplication.Resources.csproj".OsAware()));
                
            });
        }
    }
}

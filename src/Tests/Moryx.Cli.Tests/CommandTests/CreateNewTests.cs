using Moryx.Cli.Commands;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;
using Moryx.Cli.Tests.Extensions;
using NUnit.Framework.Constraints;

namespace Moryx.Cli.Tests.CommandTests
{
    public class CreateNewTests
    {
        private List<string> _filteredNames;
        private string _sourceDirectory;
        private List<string> _files = [];
        private TemplateConfiguration _templateConfiguration = new();
        private const int InitialProjectFilesCount = 28;

        [SetUp]
        public void Setup()
        {
            _sourceDirectory = DummyFileList.SourceDir();
            _files = DummyFileList.Get();
            _templateConfiguration = TemplateConfigurationFactory.Default();
            _filteredNames = CreateNew.FilteredResourceNames(_sourceDirectory, _files, _templateConfiguration);
        }

        [Test]
        public void CheckInitialProjectFilesCount()
        {
            Assert.That(_filteredNames, Has.Count.EqualTo(InitialProjectFilesCount));

        }

        [Test]
        public void CheckInitialFiles()
        {
            var filteredNames = _filteredNames
                .Select(s => s.Replace(@"C:\<path>\".OsAware(), ""))
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(filteredNames, HasAny.EndingWith(@".gitignore"));
                Assert.That(filteredNames, HasAny.EndingWith(@"Directory.Build.props"));
                Assert.That(filteredNames, HasAny.EndingWith(@"Directory.Build.targets"));
                Assert.That(filteredNames, HasAny.EndingWith(@"MyApplication.sln"));
                Assert.That(filteredNames, HasAny.EndingWith(@"NuGet.Config"));
                Assert.That(filteredNames, HasAny.EndingWith(@"README.md"));
                Assert.That(filteredNames, HasAny.EndingWith(@"docs\.gitkeep"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication\MyApplication.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.App\Config\Moryx.Products.Management.ModuleConfig.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.App\Config\Moryx.Products.Model.ProductsContext.DbConfig.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.App\Config\Moryx.Resources.Model.ResourcesContext.DbConfig.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.App\Properties\launchSettings.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.App\wwwroot\favicon.ico"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.App\appsettings.Development.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.App\appsettings.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.App\MyApplication.App.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.App\Program.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.App\Startup.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.ControlSystem\MyApplication.ControlSystem.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.Orders\MyApplication.Orders.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.Orders\MyApplicationProductAssignment.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.Orders\MyApplicationRecipeAssignment.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.Products\Importer\MyApplicationImportParameters.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.Products\Importer\MyApplicationProductImporter.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.Products\Importer\MyApplicationProductImporterConfig.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.Products\MyApplication.Products.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\MyApplication.Resources\MyApplication.Resources.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Tests\MyApplication.Tests\MyApplication.Tests.csproj"));

            });
        }
    }

    public class PathConstraint : Constraint
    {
        private readonly string _path;

        public PathConstraint(string path)
        {
            _path = path;
            Description = $"At least one item ending with `{path}`";
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual) 
        {
            var list = actual as IEnumerable<string>;
            if (list == null)
            {
                var display = actual == null ? "null" : actual.GetType().Name;
                throw new ArgumentException($"Expected: {typeof(IEnumerable<string>).Name} But was: {display}", nameof(actual));
            }
            var doesMatch = list.FirstOrDefault(item => item.EndsWith(_path.OsAware())) != null;

            return new ConstraintResult(this, actual, doesMatch);
        }
    }

    public class HasAny
    {
        public static Constraint EndingWith(string path)
        {
            return new PathConstraint(path);
        }
    }
}

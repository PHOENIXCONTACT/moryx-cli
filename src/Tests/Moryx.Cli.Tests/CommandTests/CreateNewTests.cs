using Moq;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;
using Moryx.Cli.Templates.Extensions;
using NUnit.Framework.Constraints;

namespace Moryx.Cli.Tests.CommandTests
{
    public class CreateNewTests
    {
        private List<string> _files = [];
        private TemplateConfiguration _templateConfiguration = new();
        private Template _template;
        private const int InitialProjectFilesCount = 28;
        private const string ApplicationName = "Project1";

        [SetUp]
        public void Setup()
        {
            var settingsMock = new Mock<TemplateSettings>();
            settingsMock.SetupGet(m => m.SourceDirectory).Returns(DummyFileList.SourceDir());
            settingsMock.Object.AppName = ApplicationName;

            _files = DummyFileList.Get();
            _templateConfiguration = TemplateConfigurationFactory.Default();
            var templateConfiguration = TemplateConfigurationFactory.Default();
            _template = Template.Load(settingsMock.Object, templateConfiguration, _files);
        }

        [Test]
        public void CheckInitialProjectFilesCount()
        {
            var filesDictionary = _template.NewProject();
            
            Assert.That(filesDictionary, Has.Count.EqualTo(InitialProjectFilesCount));
        }

        [Test]
        public void CheckInitialFiles()
        {
            var filteredNames = _template.NewProject()
                .Select(s => s.Value.Replace(@"C:\<path>\".OsAware(), ""))
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(filteredNames, HasAny.EndingWith(@".gitignore"));
                Assert.That(filteredNames, HasAny.EndingWith(@"Directory.Build.props"));
                Assert.That(filteredNames, HasAny.EndingWith(@"Directory.Build.targets"));
                Assert.That(filteredNames, HasAny.EndingWith(@"Project1.sln"));
                Assert.That(filteredNames, HasAny.EndingWith(@"NuGet.Config"));
                Assert.That(filteredNames, HasAny.EndingWith(@"README.md"));
                Assert.That(filteredNames, HasAny.EndingWith(@"docs\.gitkeep"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1\Project1.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.App\Config\Moryx.Products.Management.ModuleConfig.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.App\Config\Moryx.Products.Model.ProductsContext.DbConfig.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.App\Config\Moryx.Resources.Model.ResourcesContext.DbConfig.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.App\Properties\launchSettings.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.App\wwwroot\favicon.ico"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.App\appsettings.Development.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.App\appsettings.json"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.App\Project1.App.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.App\Program.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.App\Startup.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.ControlSystem\Project1.ControlSystem.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.Orders\Project1.Orders.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.Orders\Project1ProductAssignment.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.Orders\Project1RecipeAssignment.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.Products\Importer\Project1ImportParameters.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.Products\Importer\Project1ProductImporter.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.Products\Importer\Project1ProductImporterConfig.cs"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.Products\Project1.Products.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Project1.Resources\Project1.Resources.csproj"));
                Assert.That(filteredNames, HasAny.EndingWith(@"src\Tests\Project1.Tests\Project1.Tests.csproj"));

            });
        }

        [Test]
        public void TestThatTemplateFileWillBeIgnored()
        {
            var filteredNames = _template.NewProject()
                .Select(s => s.Value.Replace(@"C:\<path>\".OsAware(), ""))
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(filteredNames, Does.Not.Contain(@".moryxtpl"));
            });
        }

        [Test]
        public void CheckPreparedFileStructure()
        {
            var filteredNames = _files
                .Select(s => s.Replace(@"C:\<path>\".OsAware(), ""))
                .ToList();

            var fileStructure = _template.NewProject();
            var values = fileStructure.Select(p => p.Value);

            Assert.Multiple(() =>
            {
                Assert.That(values, HasAny.EndingWith(@".gitignore"));
                Assert.That(values, HasAny.EndingWith(@"Directory.Build.props"));
                Assert.That(values, HasAny.EndingWith(@"Directory.Build.targets"));
                Assert.That(values, HasAny.EndingWith(@"Project1.sln"));
                Assert.That(values, HasAny.EndingWith(@"NuGet.Config"));
                Assert.That(values, HasAny.EndingWith(@"README.md"));
                Assert.That(values, HasAny.EndingWith(@"docs\.gitkeep"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1\Project1.csproj"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.App\Config\Moryx.Products.Management.ModuleConfig.json"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.App\Config\Moryx.Products.Model.ProductsContext.DbConfig.json"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.App\Config\Moryx.Resources.Model.ResourcesContext.DbConfig.json"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.App\Properties\launchSettings.json"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.App\wwwroot\favicon.ico"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.App\appsettings.Development.json"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.App\appsettings.json"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.App\Project1.App.csproj"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.App\Program.cs"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.App\Startup.cs"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.ControlSystem\Project1.ControlSystem.csproj"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.Orders\Project1.Orders.csproj"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.Orders\Project1ProductAssignment.cs"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.Orders\Project1RecipeAssignment.cs"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.Products\Importer\Project1ImportParameters.cs"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.Products\Importer\Project1ProductImporter.cs"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.Products\Importer\Project1ProductImporterConfig.cs"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.Products\Project1.Products.csproj"));
                Assert.That(values, HasAny.EndingWith(@"src\Project1.Resources\Project1.Resources.csproj"));
                Assert.That(values, HasAny.EndingWith(@"src\Tests\Project1.Tests\Project1.Tests.csproj"));

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

using Moq;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.Models;
using Moryx.Cli.Tests.CommandTests;

namespace Moryx.Cli.Tests
{
    public class TemplateTests
    {
        private Template _template;
        private const int NumberOfFiles = 57;
        private const int NumberOfStepFiles = 10;
        private const int NumberOfBareFiles = 28;
        private const int NumberOfProductFiles = 2;
        private const int NumberOfResourceFiles = 3;
        private const int NumberOfModuleFiles = 8;
        private const string SolutionName = "UnitTestSolution";
        private const string ModuleName = "ProcessEngine";
        private List<string> _resourceNames;


        [SetUp]
        public void Setup()
        {
            var settingsMock = new Mock<TemplateSettings>();
            settingsMock.SetupGet(m => m.SourceDirectory).Returns(DummyFileList.SourceDir());
            settingsMock.Object.AppName = "PencilFactory";
            var templateConfiguration = TemplateConfigurationFactory.Default();
            _resourceNames = DummyFileList.Get();

            _template = Template.Load(settingsMock.Object, templateConfiguration, _resourceNames);

        }

        [Test]
        public void CheckNumberOfInitialFiles()
        {
            var list = _resourceNames;
            Assert.That(list, Has.Count.EqualTo(NumberOfFiles));
        }

        [Test]
        public void CheckProductFilesGetReturned()
        {
            const string ProductName = "Pencil";
            var list = _template.Product(ProductName)
                .Select(kvp => kvp.Value)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(list, Has.Count.EqualTo(NumberOfProductFiles));
                Assert.That(list, HasAny.EndingWith(@"PencilInstance.cs"));
                Assert.That(list, HasAny.EndingWith(@"PencilType.cs"));
            });
        }

        [Test]
        public void CheckStepFilesCount()
        {
            var list = _template
                .Step("Assembling");

            Assert.That(list, Has.Count.EqualTo(NumberOfStepFiles));
        }

        [Test]
        public void CheckStepFilesGetReturned()
        {
            var step = _template.Step("Malforming");

            var list = step
                .Select(kvp => kvp.Key)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(list, Does.Contain(@"src\MyApplication.Resources\MyApplication.Resources.csproj".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.Resources\SimulatedInOutDriver.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.Resources\SomeCell.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication\Capabilities\SomeCapabilities.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication\Resources\ISomeResource.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\Tests\MyApplication.Tests\SomeResourceTest.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication\Activities\SomeStep\SomeActivity.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication\Activities\SomeStep\SomeActivityResults.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication\Activities\SomeStep\SomeParameters.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication\Activities\SomeStep\SomeTask.cs".OsAware()));
            });

            list = step
                .Select(kvp => kvp.Value)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(list, Does.Contain(@"src\PencilFactory.Resources.Malforming\PencilFactory.Resources.Malforming.csproj".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory.Resources.Malforming\SimulatedInOutDriver.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory.Resources.Malforming\MalformingCell.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory\Capabilities\MalformingCapabilities.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory\Resources\IMalformingResource.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\Tests\PencilFactory.Tests\MalformingResourceTest.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory\Activities\MalformingStep\MalformingActivity.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory\Activities\MalformingStep\MalformingActivityResults.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory\Activities\MalformingStep\MalformingParameters.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory\Activities\MalformingStep\MalformingTask.cs".OsAware()));
            });
        }

        [Test]
        public void CheckModuleFilesGetReturned()
        {
            var dictionary = _template
                .Module(ModuleName);

            var list = dictionary
                .Select(kvp => kvp.Value)
                .ToList();

            Assert.That(list, Has.Count.EqualTo(NumberOfModuleFiles));

            Assert.Multiple(() =>
            {
                Assert.That(list, Does.Contain(@"src\PencilFactory.ProcessEngine\Components\IMyComponent.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory.ProcessEngine\Facade\IMyFacade.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory.ProcessEngine\Facade\MyFacade.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory.ProcessEngine\Implementation\MyComponent.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory.ProcessEngine\ModuleController\ModuleConfig.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory.ProcessEngine\ModuleController\ModuleConsole.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory.ProcessEngine\ModuleController\ModuleController.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\PencilFactory.ProcessEngine\PencilFactory.ProcessEngine.csproj".OsAware()));
            });
        }

        [Test]
        public void CheckResourceFilesGetReturned()
        {
            var dictionary = _template.Resource("Camera");

            Assert.That(dictionary, Has.Count.EqualTo(NumberOfResourceFiles));

            var keys = dictionary
                .Select(kvp => kvp.Key)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(keys[0], Does.EndWith("ISomeResource.cs"));
                Assert.That(keys[1], Does.EndWith(Path.DirectorySeparatorChar + "MyResource.cs"));
                Assert.That(keys[2], Does.EndWith("MyResourceTest.cs"));
            });

            var values = dictionary
                .Select(kvp => kvp.Value)
                .ToList();
            Assert.Multiple(() =>
            {
                Assert.That(values[0], Does.EndWith(@"PencilFactory\Resources\ICameraResource.cs"));
                Assert.That(values[1], Does.EndWith(Path.DirectorySeparatorChar + "CameraResource.cs"));
                Assert.That(values[2], Does.EndWith("CameraResourceTest.cs"));
            });
        }

        [Test]
        public void CheckStateFilesGetReturned()
        {
            var list = _template
                .StateFile("Initializing", "CameraDriver")
                // The value does not need to be tested here, as it doesn't
                // serve as the target path for the State file
                .Select(kvp => kvp.Key)
                .ToList();

            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(@"src\MyApplication.Resources\SpecificState.cs"));
        }

        [Test]
        public void CheckStateBaseFilesGetReturned()
        {
            var list = _template
                .StateBaseFile("CameraDriver")
                // The value does not need to be tested here, as it doesn't
                // serve as the target path for the StateBase file
                .Select(kvp => kvp.Key)
                .ToList();

            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(@"src\MyApplication.Resources\StateBase.cs"));
        }
    }
}
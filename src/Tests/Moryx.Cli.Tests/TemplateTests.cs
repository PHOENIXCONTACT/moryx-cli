using Moryx.Cli.Template;

namespace Moryx.Cli.Tests
{
    public class TemplateTests
    {
        private const int NumberOfResources = 56;
        private const string SolutionName = "UnitTestSolution";
        private List<string> _resourceNames;

        [SetUp]
        public void Setup()
        {
            _resourceNames = DummyFileList.Get();
        }

        [Test]
        public void CheckNumberOfResourceNames()
        {
            var list = _resourceNames;
            Assert.That(list, Has.Count.EqualTo(NumberOfResources));
        }

        [Test]
        public void CheckProductFilesGetRemoved()
        {
            var list = _resourceNames
                .WithoutProduct();

            Assert.That(list, Has.Count.EqualTo(NumberOfResources - 2));
        }

        [Test]
        public void CheckProductFilesGetReturned()
        {
            var list = _resourceNames
                .Product();

            Assert.That(list, Has.Count.EqualTo(2));
        }

        [Test]
        public void CheckStepFilesGetRemoved()
        {
            var list = _resourceNames
                .WithoutStep();

            Assert.That(list, Has.Count.EqualTo(NumberOfResources - 9));
        }        
        
        [Test]
        public void CheckModuleFilesGetRemoved()
        {
            var list = _resourceNames
                .WithoutModule();

            Assert.That(list, Has.Count.EqualTo(NumberOfResources - 8));
        }

        [Test]
        public void CheckRecipeFilesGetRemoved()
        {
            var list = _resourceNames
                .WithoutRecipe();

            Assert.That(list, Has.Count.EqualTo(NumberOfResources - 1));
        }

        [Test]
        public void CheckSetupTriggerFilesGetRemoved()
        {
            var list = _resourceNames
                .WithoutSetupTrigger();

            Assert.That(list, Has.Count.EqualTo(NumberOfResources - 2));
        }

        [Test]
        public void CheckCellSelectorFilesGetRemoved()
        {
            var list = _resourceNames
                .WithoutCellSelector();

            Assert.That(list, Has.Count.EqualTo(NumberOfResources - 2));
        }

        [Test]
        public void CheckStepFilesGetReturned()
        {
            var list = _resourceNames
                .Step();

            Assert.That(list, Has.Count.EqualTo(9));
        }

        [Test]
        public void CheckModuleFilesGetReturned()
        {
            var list = _resourceNames
                .Module();

            Assert.That(list, Has.Count.EqualTo(8));
        }

        [Test]
        public void CheckResourceFilesGetRemoved()
        {
            var list = _resourceNames
                .WithoutResource();

            Assert.That(list, Has.Count.EqualTo(NumberOfResources - 3));
        }

        [Test]
        public void CheckResourceFilesGetReturned()
        {
            var list = _resourceNames
                .Resource();

            Assert.Multiple(() =>
            {
                Assert.That(list, Has.Count.EqualTo(3));
                Assert.That(list.First(s => s.EndsWith("ISomeResource.cs")), Is.Not.Null);
                Assert.That(list.First(s => s.EndsWith(Path.DirectorySeparatorChar + "MyResource.cs")), Is.Not.Null);
                Assert.That(list.First(s => s.EndsWith("MyResourceTest.cs")), Is.Not.Null);
            });
        }

        [Test]
        public void AllProjectFilesGetListed()
        {
            var list = _resourceNames;
            var projects = Template.Template.InitialProjects(list);

            Assert.That(projects.Count, Is.EqualTo(8));
        }

        [Test]
        public void ApplicationFilesGetCategorized()
        {
            var list = _resourceNames;
            var projects = Template.Template.InitialProjects(list);

            var fileStructure = Template.Template.PrepareFileStructure(SolutionName, list, projects);
            var project = projects.First(p => p.Name == "MyApplication");

            Assert.That(fileStructure[project], Has.Count.EqualTo(10));
        }


        [Test]
        public void CheckRootFileCount()
        {
            var list = _resourceNames;
            var projects = Template.Template.InitialProjects(list);

            var fileStructure = Template.Template.PrepareFileStructure(SolutionName, list, projects);

            Assert.That(fileStructure.Last().Value, Has.Count.EqualTo(7));
            Assert.That(fileStructure.Last().Key.Name, Is.EqualTo(""));
        }

        [Test]
        public void ModuleFilesGetCategorized()
        {
            var list = _resourceNames;
            var projects = Template.Template.InitialProjects(list);

            var fileStructure = Template.Template.PrepareFileStructure(SolutionName, list, projects);
            var project = projects.First(p => p.Name == "MyApplication.MyModule");

            Assert.That(fileStructure[project], Has.Count.EqualTo(8));
        }

        [Test]
        public void BareFileStructureHasApplicationFiles()
        {
            var resourceNames = _resourceNames;
            var projects = resourceNames.InitialProjects();
            var filteredResourceNames = resourceNames.BareProjectFiles();

            var fileStructure = Template.Template.PrepareFileStructure(SolutionName, filteredResourceNames, projects);

            var flattened = fileStructure.SelectMany(item => item.Value);
            Assert.That(flattened.Count, Is.EqualTo(28));
        }

        [Test]
        public void FileCountForBareSetup()
        {
            var resourceNames = _resourceNames;
            var filteredResourceNames = resourceNames.BareProjectFiles();

            Assert.That(filteredResourceNames, Has.Count.EqualTo(28));
        }

        [Test]
        public void CheckStateFileGetsReturned()
        {
            var list = _resourceNames
                .StateFile();

            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].EndsWith("SpecificState.cs"));
        }

        [Test]
        public void CheckStateBaseFileGetsReturned()
        {
            var list = _resourceNames
                .StateBaseFile();

            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].EndsWith("StateBase.cs"));
        }
    }
}
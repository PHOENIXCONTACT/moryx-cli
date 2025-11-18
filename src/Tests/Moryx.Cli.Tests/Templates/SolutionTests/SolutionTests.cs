using Moq;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace Moryx.Cli.Tests.Templates.SolutionTests
{
    public class SolutionTests
    {
        private const string SlnFilename = "Project1.sln";
        private const string SlnxFilename = "Project1x.slnx";
        private TemplateConfiguration _templateConfiguration = new();
        private Template _template;

        [SetUp]
        public void Setup()
        {
            var settingsMock = new Mock<TemplateSettings>();
            settingsMock.SetupGet(m => m.SourceDirectory).Returns(DummyFileList.SourceDir());
        }

        [Test]
        public void SingleSlnFileWillReturnFilename()
        {
            string[] files = [SlnFilename, "Class1.cs"];
            
            var result = Solution.GetSolutionName(files, Assert.Fail);

            Assert.That(result, Is.EqualTo("Project1"));
        }

        [Test]
        public void SingleSlnxFileWillReturnFilename()
        {
            string[] files = [SlnxFilename, "Class1.cs"];

            var result = Solution.GetSolutionName(files, Assert.Fail);

            Assert.That(result, Is.EqualTo("Project1x"));
        }

        [Test]
        public void SingleSlnAndSingleSlnxCanCoexistPrioritizingSlnx()
        {
            string[] files = [SlnxFilename, SlnFilename];

            var result = Solution.GetSolutionName(files, Assert.Fail);

            Assert.That(result, Is.EqualTo("Project1x"));
        }

        [Test]
        public void MultipleSlnFilesWillResultInError()
        {
            string[] files = [SlnFilename, "Project2.sln", "Project3.sln"];
            var errorDelegate = new Mock<Action<string>>();

            var result = Solution.GetSolutionName(files, errorDelegate.Object);

            errorDelegate.Verify(m => m("Too many _solutions_ found. Please make sure, there is only one solution."), Times.Once);
        }

        [Test]
        public void MultipleSlnxFilesWillResultInError()
        {
            string[] files = [SlnxFilename, "Project2.slnx", "Project3.slnx"];
            var errorDelegate = new Mock<Action<string>>();

            var result = Solution.GetSolutionName(files, errorDelegate.Object);

            errorDelegate.Verify(m => m("Too many _solutions_ found. Please make sure, there is only one solution."), Times.Once);
        }

        [Test]
        public void NoSlnOrSlnxFilesWillResultInError()
        {
            string[] files = ["Class1.cs", "Class2.cs"];
            var errorDelegate = new Mock<Action<string>>();

            var result = Solution.GetSolutionName(files, errorDelegate.Object);

            errorDelegate.Verify(m => m("No _solutions_ found. Please make sure, there is a VisualStudio solution in this directory."), Times.Once);
        }
    }
}

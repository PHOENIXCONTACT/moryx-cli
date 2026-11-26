using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates.Models;
using System.Xml.Linq;

namespace Moryx.Cli.Commands.Tests
{
    public class SolutionFileManipulationTests
    {
        private string _targetDir = "";
        private TemplateSettings _settings = null!;

        private const string AppName = "PencilFactory";

        [SetUp]
        public void Setup()
        {
            _targetDir = Path.Combine(Path.GetTempPath(), $"moryx_slntest_{Path.GetRandomFileName()}");
            Directory.CreateDirectory(_targetDir);

            _settings = new TemplateSettings
            {
                AppName = AppName,
                TargetDirectory = _targetDir,
            };
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_targetDir))
                Directory.Delete(_targetDir, true);
        }

        [Test]
        public void SlnxFile_ShouldAddProjectEntry()
        {
            var slnxFile = Path.Combine(_targetDir, $"{AppName}.slnx");
            File.WriteAllText(slnxFile, "<Solution>\n</Solution>");

            var projectFile = Path.Combine(_targetDir, "src", "PencilFactory.Steps", "PencilFactory.Steps.csproj");
            Directory.CreateDirectory(Path.GetDirectoryName(projectFile)!);
            File.WriteAllText(projectFile, "<Project />");

            new[] { projectFile }.AddProjectsToSolution(_settings);

            var doc = XDocument.Load(slnxFile);
            var projectElements = doc.Root!.Elements("Project").ToList();

            Assert.That(projectElements, Has.Count.EqualTo(1));
            Assert.That(projectElements[0].Attribute("Path")?.Value, Does.Contain("PencilFactory.Steps.csproj"));
        }

        [Test]
        public void SlnFile_ShouldAddProjectEntry()
        {
            var slnFile = Path.Combine(_targetDir, $"{AppName}.sln");
            File.WriteAllText(slnFile,
                "Microsoft Visual Studio Solution File, Format Version 12.00\n" +
                "Global\n" +
                "\tGlobalSection(SolutionConfigurationPlatforms) = preSolution\n" +
                "\t\tDebug|Any CPU = Debug|Any CPU\n" +
                "\tEndGlobalSection\n" +
                "EndGlobal\n");

            var projectFile = Path.Combine(_targetDir, "src", "PencilFactory.Steps", "PencilFactory.Steps.csproj");
            Directory.CreateDirectory(Path.GetDirectoryName(projectFile)!);
            File.WriteAllText(projectFile, "<Project />");

            new[] { projectFile }.AddProjectsToSolution(_settings);

            var content = File.ReadAllText(slnFile);

            Assert.That(content, Does.Contain("PencilFactory.Steps.csproj"));
            Assert.That(content, Does.Contain("EndProject"));
        }

        [Test]
        public void SlnxFile_ShouldUseForwardSlashesInPath()
        {
            var slnxFile = Path.Combine(_targetDir, $"{AppName}.slnx");
            File.WriteAllText(slnxFile, "<Solution>\n</Solution>");

            var projectFile = Path.Combine(_targetDir, "src", "PencilFactory.Steps", "PencilFactory.Steps.csproj");
            Directory.CreateDirectory(Path.GetDirectoryName(projectFile)!);
            File.WriteAllText(projectFile, "<Project />");

            new[] { projectFile }.AddProjectsToSolution(_settings);

            var doc = XDocument.Load(slnxFile);
            var path = doc.Root!.Elements("Project").Single().Attribute("Path")!.Value;

            Assert.That(path, Does.Not.Contain('\\'));
        }
    }
}

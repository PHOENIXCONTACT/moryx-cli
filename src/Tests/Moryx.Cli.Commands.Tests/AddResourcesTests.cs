using Moq;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;
using System.Text.RegularExpressions;

namespace Moryx.Cli.Commands.Tests
{
    public class AddResourcesTests
    {
        private Template _template = null!;
        private string _sourceDir = "";
        private string _targetDir = "";

        private const string AppName = "PencilFactory";
        private const string ResourceName = "Camera";

        [SetUp]
        public void Setup()
        {
            _sourceDir = Directory.CreateTempSubdirectory("moryx_source_").FullName;
            _targetDir = Directory.CreateTempSubdirectory("moryx_target_").FullName;

            const string templateContent =
                "namespace MyApplication.Resources;\n" +
                "\n" +
                "public class MyResource { }";

            Directory.CreateDirectory(Path.Combine(_sourceDir, "src", "MyApplication.Resources"));
            File.WriteAllText(Path.Combine(_sourceDir, "src", "MyApplication.Resources", "MyResource.cs"), templateContent);

            Directory.CreateDirectory(Path.Combine(_sourceDir, "src", "MyApplication", "Resources"));
            File.WriteAllText(Path.Combine(_sourceDir, "src", "MyApplication", "Resources", "ISomeResource.cs"), templateContent);

            Directory.CreateDirectory(Path.Combine(_sourceDir, "src", "Tests", "MyApplication.Tests"));
            File.WriteAllText(Path.Combine(_sourceDir, "src", "Tests", "MyApplication.Tests", "MyResourceTest.cs"), templateContent);

            var fileNames = Directory.GetFiles(_sourceDir, "*", new EnumerationOptions { RecurseSubdirectories = true }).ToList();

            var settingsMock = new Mock<TemplateSettings>();
            settingsMock.SetupGet(m => m.SourceDirectory).Returns(_sourceDir + Path.DirectorySeparatorChar);
            settingsMock.Object.AppName = AppName;
            settingsMock.Object.TargetDirectory = _targetDir;

            _template = Template.Load(settingsMock.Object, TemplateConfigurationFactory.Default(), fileNames);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_sourceDir)) Directory.Delete(_sourceDir, true);
            if (Directory.Exists(_targetDir)) Directory.Delete(_targetDir, true);
        }

        [Test]
        public void ShouldNotReturnErrors()
        {
            var result = AddResources.Exec(_template, [ResourceName]);

            Assert.That(result.Errors, Is.Empty, string.Join(", ", result.Errors));
        }

        [Test]
        public void ShouldCreateThreeFiles()
        {
            AddResources.Exec(_template, [ResourceName]);

            var writtenFiles = Directory.GetFiles(_template.Settings.TargetDirectory, "*.cs", new EnumerationOptions { RecurseSubdirectories = true });

            Assert.That(writtenFiles, Has.Length.EqualTo(3));
        }

        [Test]
        public void ShouldRenameTestFile()
        {
            AddResources.Exec(_template, [ResourceName]);

            var writtenFiles = Directory.GetFiles(_template.Settings.TargetDirectory, "*.cs", new EnumerationOptions { RecurseSubdirectories = true });

            Assert.That(writtenFiles.Select(Path.GetFileName), Has.Some.EqualTo($"{ResourceName}ResourceTest.cs"));
        }

        [Test]
        public void ShouldRenameNamespace()
        {
            AddResources.Exec(_template, [ResourceName]);

            var namespaces = Directory
                .GetFiles(_template.Settings.TargetDirectory, "*.cs", new EnumerationOptions { RecurseSubdirectories = true })
                .Select(f => ExtractNamespace(File.ReadAllText(f)));

            Assert.That(namespaces, Is.All.EqualTo($"{AppName}.Resources"));
        }

        private static string? ExtractNamespace(string content)
        {
            var match = Regex.Match(content, @"namespace\s+([\w.]+)");
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}

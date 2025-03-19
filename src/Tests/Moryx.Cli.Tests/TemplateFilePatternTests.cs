using Moq;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;
using Moryx.Cli.Templates.Extensions;

namespace Moryx.Cli.Tests
{
    public class TemplateFilePatternTests
    {
        private const int NumberOfAllFiles = 17;

        private string Root = @"C:\root";
        private Template _template;

        [SetUp]
        public void Setup()
        {
            Root = Root.OsAware();
            var fileNames = new List<string>()
            {
                @"C:\root\.gitignore",
                @"C:\root\Directory.Build.props",
                @"C:\root\Directory.Build.targets",
                @"C:\root\src\MyApplication.App\appsettings.Development.json",
                @"C:\root\src\MyApplication.App\.gitignore",
                @"C:\root\src\MyApplication.App\appsettings.json",
                @"C:\root\src\MyApplication.App\MyApplication.App.csproj",
                @"C:\root\src\MyApplication.App\Startup.cs",
                @"C:\root\src\MyApplication.Module\Subfolder1\Console.cs",
                @"C:\root\src\MyApplication.Module\Subfolder1\Module.cs",
                @"C:\root\src\MyApplication.Module\Subfolder1\Controller.cs",
                @"C:\root\src\Tests\Test1.txt",
                @"C:\root\src\Tests\Test10.txt",
                @"C:\root\src\Tests\Test2.txt",
                @"C:\root\src\Tests\Test20.txt",
                @"C:\root\src\Tests\Test3.txt",
                @"C:\root\src\Tests\Test30.txt",
            }
            .Select(s => s.OsAware())
            .ToList();

            var settingsMock = new Mock<TemplateSettings>();
            settingsMock.SetupGet(m => m.SourceDirectory).Returns(Root.OsAware());
            settingsMock.Object.AppName = "PencilFactory";
            var templateConfiguration = TemplateConfigurationFactory.Default();

            _template = Template.Load(settingsMock.Object, templateConfiguration, fileNames);
        }


        [Test]
        public void TestThatSingleStarMatchesTheFilesAtRoot()
        {
            var list = _template.FilterByPattern(Root, TextPattern("*".OsAware()));

            Assert.That(list, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(list[0], Is.EqualTo(@".gitignore".OsAware()));
                Assert.That(list[1], Is.EqualTo(@"Directory.Build.props".OsAware()));
                Assert.That(list[2], Is.EqualTo(@"Directory.Build.targets".OsAware()));
            });
        }

        [Test]
        public void TestThatSingleStarWithFilenameMatchesFileAtRoot()
        {
            var list = _template.FilterByPattern(Root, TextPattern("*.gitignore".OsAware()));

            Assert.That(list, Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(list[0], Is.EqualTo(@".gitignore".OsAware()));
            });
        }

        [Test]
        public void TestThatDoubleStarsCanMatchAllFiles()
        {
            var list = _template.FilterByPattern(Root, TextPattern(@"**\*".OsAware())); 

            Assert.That(list, Has.Count.EqualTo(NumberOfAllFiles));
        }

        [Test]
        public void TestThatDoubleStarsMatchesSubfolder()
        {
            var list = _template.FilterByPattern(Root, TextPattern(@"**\*Module*\**\*".OsAware()));

            Assert.That(list, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(list, Does.Contain(@"src\MyApplication.Module\Subfolder1\Console.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.Module\Subfolder1\Module.cs".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.Module\Subfolder1\Controller.cs".OsAware()));
            });
        }

        [Test]
        public void TestThatDoubleStarsWillBeInterruptedByTextPattern()
        {
            var list = _template.FilterByPattern(Root, TextPattern(@"**\*Module*\*".OsAware()));

            // `**\*Module*\*` would only math those paths, where `*Module*`
            // matches the deepest subfolder, but not where `*Module*` matches
            // 'any' folder before.
            //
            // MATCH: src\Subfolder\Subfolder \ MyApplication.Module \ Console.cs
            //                                                ^^^^^^
            // NO MATCH: src\MyApplication.Module \ Subfolder1 \ Console.cs
            //                             ^^^^^^ ^            ^

            Assert.That(list, Has.Count.EqualTo(0));
        }

        [Test]
        public void TestThatDoubleStarsWithTextMatchesSubfolder()
        {
            var list = _template.FilterByPattern(Root, TextPattern(@"**\*App\*".OsAware()));

            Assert.That(list, Has.Count.EqualTo(5));
            Assert.Multiple(() =>
            {
                Assert.That(list, Does.Contain(@"src\MyApplication.App\appsettings.Development.json".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\.gitignore".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\appsettings.json".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\MyApplication.App.csproj".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\Startup.cs".OsAware()));
            });
        }

        public void TestThatDoubleStarsWithTextAndMultiplePlaceholdersMatchesSubfolder()
        {
            var list = _template.FilterByPattern(Root, TextPattern(@"**\*App*\*".OsAware()));

            Assert.That(list, Has.Count.EqualTo(5));
            Assert.Multiple(() =>
            {
                Assert.That(list, Does.Contain(@"src\MyApplication.App\appsettings.Development.json".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\.gitignore".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\appsettings.json".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\MyApplication.App.csproj".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\Startup.cs".OsAware()));
            });
        }

        [Test]
        public void TestThatNoFollowingSeparatorWontReturnFolders()
        {
            var list = _template.FilterByPattern(Root, TextPattern(@"**\*Application*".OsAware()));

            // In the following hierarchy, the pattern `**\*Application*` would
            // only return the file entry `MyApplication.App.csproj`, even though
            // the substring is also part of the subfolder `MyApplication.App`.
            //
            //   src\MyApplication.App\.gitignore
            //   src\MyApplication.App\appsettings.json
            //   src\MyApplication.App\MyApplication.App.csproj
            //   src\MyApplication.App\Startup.cs


            Assert.That(list, Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(list, Does.Contain(@"src\MyApplication.App\MyApplication.App.csproj".OsAware()));
            });
        }

        [Test]
        public void TestThatOverlappingPatternsWontReturnDuplicates()
        {
            var list = _template.FilterByPattern(Root, TextPattern(
                [
                    @"**\*App\*".OsAware(), 
                    @"**\*Application*".OsAware()
                ]));

            Assert.That(list, Has.Count.EqualTo(5));
            Assert.Multiple(() =>
            {
                Assert.That(list, Does.Contain(@"src\MyApplication.App\appsettings.Development.json".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\.gitignore".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\appsettings.json".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\MyApplication.App.csproj".OsAware()));
                Assert.That(list, Does.Contain(@"src\MyApplication.App\Startup.cs".OsAware()));
            });
        }

        [Test]
        public void TestSingleQuestionMark()
        {
            var list = _template.FilterByPattern(Root, TextPattern(@"src\Tests\Test?.txt".OsAware()));

            Assert.That(list, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(list[0], Is.EqualTo(@"src\Tests\Test1.txt".OsAware()));
                Assert.That(list[1], Is.EqualTo(@"src\Tests\Test2.txt".OsAware()));
                Assert.That(list[2], Is.EqualTo(@"src\Tests\Test3.txt".OsAware()));
            });
        }

        [Test]
        public void TestSingleDigit()
        {
            var list = _template.FilterByPattern(Root, TextPattern(@"**\*1*".OsAware()));

            Assert.That(list, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(list[0], Is.EqualTo(@"src\Tests\Test1.txt".OsAware()));
                Assert.That(list[1], Is.EqualTo(@"src\Tests\Test10.txt".OsAware()));
            });
        }

        [Test]
        public void TestDoubleQuestionMark()
        {
            var list = _template.FilterByPattern(Root, TextPattern(@"src\Tests\Test??.txt".OsAware()));

            Assert.That(list, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(list[0], Is.EqualTo(@"src\Tests\Test10.txt".OsAware()));
                Assert.That(list[1], Is.EqualTo(@"src\Tests\Test20.txt".OsAware()));
                Assert.That(list[2], Is.EqualTo(@"src\Tests\Test30.txt".OsAware()));
            });
        }


        private static ConfigurationPattern TextPattern(string pattern)
            => new()
            {
                Files = [ pattern ],
            };

        private static ConfigurationPattern TextPattern(string[] pattern)
            => new()
            {
                Files = [.. pattern],
            };
    }
}
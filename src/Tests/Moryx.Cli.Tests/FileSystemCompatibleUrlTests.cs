using Moryx.Cli.Templates.Extensions;

namespace Moryx.Cli.Tests
{
    public class FileSystemCompatibleUrlTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [TestCase("https://github.com/microsoft/vscode.git", "https_github.com_microsoft_vscode.git")]
        [TestCase("ssh://git@github.com:microsoft/vscode.git", "ssh_git@github.com_microsoft_vscode.git")]
        public void CheckUrlConversion(string input, string expected)
        {
            var result = input.FileSystemCompatible();

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
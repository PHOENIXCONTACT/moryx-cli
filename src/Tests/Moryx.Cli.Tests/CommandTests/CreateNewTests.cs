using Moryx.Cli.Commands;
using Moryx.Cli.Template.Extensions;
using Moryx.Cli.Template.StateBaseTemplate;

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

            Assert.Multiple(() =>
            {
                filteredNames.ForEach(s =>
                {
                    Assert.That(s, Does.Not.Contain("MyResource"));
                    Assert.That(s, Does.Not.Contain("State.cs"));
                    Assert.That(s, Does.Not.Contain("StateBase.cs"));
                });

                Assert.That(filteredNames, Has.Count.EqualTo(InitialProjectFilesCount));
            });
        }
    }
}
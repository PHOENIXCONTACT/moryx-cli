using Moryx.Cli.Templates.Components;
using Moryx.Cli.Tests.Extensions;

namespace Moryx.Cli.Tests.CSharpFileTests
{
    public class CSharpFileTests
    {
        private CSharpFile _sut;

        [SetUp]
        public void Setup()
        {
            _sut = CSharpFile.FromFile("CSharpFileTests\\TestData\\AssemblingResourceStateBase.cs".OsAware());
        }

        [Test]
        public void ShouldFindNamespace()
        {
            Assert.That(_sut.NamespaceName, Is.EqualTo("TestApp.Resources.AssemblingResource.States"));
        }

        [Test]
        public void ShouldUpdateNamespace()
        {
            _sut.NamespaceName = "My.New.Namespace";

            var csFile = new CSharpFile(_sut.Content);

            Assert.That(csFile.NamespaceName, Is.EqualTo("My.New.Namespace"));
        }
    }
}
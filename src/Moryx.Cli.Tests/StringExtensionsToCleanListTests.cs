using Moryx.Cli.Template.Extensions;

namespace Moryx.Cli.Tests
{
    public class StringExtensionsToCleanListTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldReturnListOfStrings()
        {
            var str = "Abc, Def, Ghi, Jklmn, opqrs ";

            var list = str.ToCleanList();

            Assert.That(list, Has.Count.EqualTo(5));
            Assert.Multiple(() =>
            {
                Assert.That(list[0], Is.EqualTo("Abc"));
                Assert.That(list[1], Is.EqualTo("Def"));
                Assert.That(list[2], Is.EqualTo("Ghi"));
                Assert.That(list[3], Is.EqualTo("Jklmn"));
                Assert.That(list[4], Is.EqualTo("opqrs"));
            });
        }

        [Test]
        [TestCase("Abc", "Abc")]
        [TestCase(" Abc ", "Abc")]
        [TestCase(" Abc", "Abc")]
        [TestCase("Abc ", "Abc")]
        public void ShouldReturnSingleItem(string input, string expected)
        {
            var list = input.ToCleanList();

            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(expected));
        }

        [Test]
        [TestCase("")]
        [TestCase(",")]
        [TestCase(",,")]
        [TestCase(", ,")]
        [TestCase(",, ")]
        [TestCase(" ,,")]
        public void ShouldReturnEmptyList(string input)
        {
            var list = input.ToCleanList();

            Assert.That(list, Is.Empty);
        }

    }
}
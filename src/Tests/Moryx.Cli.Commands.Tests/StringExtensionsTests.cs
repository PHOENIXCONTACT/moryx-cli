using Moryx.Cli.Commands.Extensions;

namespace Moryx.Cli.Commands.Tests
{
    public class StringExtensionsTests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void ShouldReplaceDictionaryItems()
        {
            var str = "Abc, Def, Ghi, Jklmn, opqrs ";
            var dictionary = new Dictionary<string, string>{
                {"Abc", "AAA" },
                {"Ghi", "bbb" }
            };

            var actual = str.Replace(dictionary);


            Assert.That(actual, Is.EqualTo("AAA, Def, bbb, Jklmn, opqrs "));
        }
    }
}
using Microsoft.CodeAnalysis;
using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.StateBaseTemplate;
using Moryx.Cli.Tests.Extensions;

namespace Moryx.Cli.Tests.CSharpFileTests
{
    public class ExistingStateBaseTests
    {
        private StateBaseTemplate _sut;

        [SetUp]
        public void Setup()
        {
            _sut = StateBaseTemplate.FromFile("CSharpFileTests\\TestData\\AssemblingResourceStateBase.cs".OsAware());
        }

        [Test]
        public void ShouldFindConstructor()
        {
            var ctor = _sut.Constructors.First();
            Assert.NotNull(ctor);
            Assert.That(GetLine(ctor.GetLocation()), Is.EqualTo(16));
        }

        [Test]
        public void ShouldInitiallyContain_3_StateDefinitions()
        {
            Assert.That(_sut.StateDeclarations.Count, Is.EqualTo(3));
        }

        [TestCase("StateWaitForData", "WaitingForUserInputState", 10, 7, true)]
        [TestCase("StateRunning", "RunningState", 20, 10)]
        [TestCase("StateOrderFinished", "OrderFinishedState", 27, 13)]
        public void ShouldParse_3_StateDefinitions(string name, string type, int value, int line, bool isInitial = false)
        {
            var definition = _sut.StateDeclarations.Where(sd => sd.Name.Equals(name)).First();
            Assert.Multiple(() =>
            {
                Assert.That(definition?.IsInitial, Is.EqualTo(isInitial));
                Assert.That(GetLine(definition?.Node.GetLocation()), Is.EqualTo(line));
            });
        }

        [Test]
        public void StateGetsAddedBeforeConstructor()
        {
            var newStateBase = _sut.AddState("ReadyState");

            var expectedContent =
                "        [StateDefinition(typeof(ReadyState))]" + Environment.NewLine +
                "        protected const int StateReady = 30;" + Environment.NewLine +
                Environment.NewLine +
                "        public AssemblingResourceStateBase(AssemblingResource context, StateMap stateMap) : base(context, stateMap)";

            Assert.That(newStateBase.Content, Contains.Substring(expectedContent));
        }


        [Test]
        public void AddedStateConstantShouldBeNextTenFromLastState()
        {
            var newStateBase = _sut.AddState("ReadyState");
            var definition = newStateBase.StateDeclarations.Last();

            Assert.That(definition.Value, Is.EqualTo(30));
        }

        protected int GetLine(Location? location)
            => (location?.GetLineSpan().StartLinePosition.Line ?? 0) + 1;
    }
}
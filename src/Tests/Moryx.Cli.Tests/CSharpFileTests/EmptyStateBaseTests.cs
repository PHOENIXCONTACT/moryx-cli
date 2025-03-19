using Microsoft.CodeAnalysis;
using Moryx.Cli.Templates.Exceptions;
using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.StateBaseTemplate;

namespace Moryx.Cli.Tests.CSharpFileTests
{
    public class EmptyStateBaseTests
    {
        private StateBaseTemplate _sut;

        [SetUp]
        public void Setup()
        {
            _sut = StateBaseTemplate.FromFile("CSharpFileTests\\TestData\\EmptyResourceStateBase.cs".OsAware());
        }

        [Test]
        public void FirstStateShouldBeInitialState()
        {
            var newStateBase = _sut.AddState("ReadyState");
            var definition = newStateBase.StateDeclarations.Single();

            Assert.That(definition.IsInitial, Is.True);
        }

        [Test]
        public void StateAddedBeforeConstructor()
        {
            var newStateBase = _sut.AddState("ReadyState");

            var expectedContent =
                "        [StateDefinition(typeof(ReadyState), IsInitial = true)]" + Environment.NewLine +
                "        protected const int StateReady = 10;" + Environment.NewLine +
                Environment.NewLine +
                "        public EmptyResourceStateBase(EmptyResource context, StateMap stateMap) : base(context, stateMap)";

            Assert.That(newStateBase.Content, Contains.Substring(expectedContent));
        }

        [Test]
        public void FirstStateConstIs_10()
        {
            var newStateBase = _sut.AddState("ReadyState");

            Assert.That(newStateBase.StateDeclarations.First().Value, Is.EqualTo(10));
        }

        [Test]
        public void SecondStateConstIs_20()
        {
            _sut = _sut.AddState("ReadyState");
            _sut = _sut.AddState("BusyState");

            Assert.That(_sut.StateDeclarations.Last().Value, Is.EqualTo(20));
        }

        [Test]
        public void CanNotAddStateTwice()
        {
            _sut = _sut.AddState("ReadyState");
            Assert.Throws<StateAlreadyExistsException>(() => _sut.AddState("ReadyState"));
        }

        [Test]
        public void ConstructorFound()
        {
            var ctor = _sut.Constructors.First();
            Assert.NotNull(ctor);
            Assert.That(GetLine(ctor.GetLocation()), Is.EqualTo(7));

        }

        [Test]
        public void ShouldInitiallyHaveNoStateDefinitions()
        {
            Assert.That(_sut.StateDeclarations.Count, Is.EqualTo(0));
        }

        protected int GetLine(Location? location)
            => (location?.GetLineSpan().StartLinePosition.Line ?? 0) + 1;
    }
}
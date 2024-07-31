using Moryx.Cli.Template.Exceptions;
using Moryx.Cli.Template.Extensions;
using Moryx.Cli.Template.StateBaseTemplate;

namespace Moryx.Cli.Tests.StateBaseTests
{
    public class EmptyStateBaseTests
    {
        private StateBaseTemplate _sut;

        [SetUp]
        public void Setup()
        {
            _sut = StateBaseTemplate.FromFile("StateBaseTests\\TestData\\EmptyResourceStateBase.cs".Replace('\\', Path.DirectorySeparatorChar));
        }

        [Test]
        public void FirstStateShouldBeInitialState()
        {
            var newStateBase = _sut.AddState("ReadyState");
            var definition = newStateBase.StateDefinitions.Single();

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

            Assert.That(newStateBase.StateDefinitions.First().Value, Is.EqualTo(10));
        }

        [Test]
        public void SecondStateConstIs_20()
        {
            _sut = _sut.AddState("ReadyState");
            _sut = _sut.AddState("BusyState");

            Assert.That(_sut.StateDefinitions.Last().Value, Is.EqualTo(20));
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
            Assert.That(ctor.Line, Is.EqualTo(7));

        }

        [Test]
        public void ShouldInitiallyHaveNoStateDefinitions()
        {
            Assert.That(_sut.StateDefinitions.Count, Is.EqualTo(0));
        }
    }
}
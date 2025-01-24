using Moryx.StateMachines;

namespace TestApp.Resources.AssemblingResource.States
{
    internal abstract class AssemblingResourceStateBase : StateBase<MarkingResource>
    {
        [StateDefinition(typeof(WaitingForUserInputState), IsInitial = true)]
        protected const int StateWaitForData = 10;
        
        [StateDefinition(typeof(RunningState))]
        protected const int StateRunning = 20;

        [StateDefinition(typeof(OrderFinishedState))]
        protected const int StateOrderFinished = 27;
        
        public AssemblingResourceStateBase(AssemblingResource context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public virtual void WaitForInput()
        {

        }

        public virtual void ErrorOccured(string message)
        {

        }

        public virtual void SetRunningState()
        {

        }

        public virtual void OrderFinished()
        {

        }
    }
}

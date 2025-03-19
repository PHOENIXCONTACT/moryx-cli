using Moryx.StateMachines;

namespace TestApp.Resources.EmptyResource.States
{
    internal abstract class EmptyResourceStateBase : StateBase<MarkingResource>
    {      
        public EmptyResourceStateBase(EmptyResource context, StateMap stateMap) : base(context, stateMap)
        {
        }
    }
}

namespace Moryx.Cli.Template.StateBaseTemplate
{
    public partial class StateBaseTemplate
    {
        public class StateDefinition
        {
            /// <summary>
            /// Name of the state definition. E.g.: `StateReady`
            /// </summary>
            public string Name { get; init; }

            /// <summary>
            /// Type of the state. E.g.: `ReadyState`
            /// </summary>
            public string Type { get; init; }

            /// <summary>
            /// Integer value of the state, usually incremented in 10th
            /// </summary>
            public int Value { get; init; }

            /// <summary>
            /// Whether it is the initial state of the state machine
            /// </summary>
            public bool IsInitial { get; init; }

            /// <summary>
            /// Position, where the definition was found inside the `*StateBase.cs`
            /// </summary>
            public int Line { get; init; }


        }
    }
}

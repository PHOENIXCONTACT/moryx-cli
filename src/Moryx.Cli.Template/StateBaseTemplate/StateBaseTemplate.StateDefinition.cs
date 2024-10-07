using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Moryx.Cli.Template.StateBaseTemplate
{
    public partial class StateBaseTemplate
    {
        public class StateDefinition
        {
            /// <summary>
            /// Name of the state definition. E.g.: `StateReady`
            /// </summary>
            public required string Name { get; init; }

            /// <summary>
            /// Type of the state. E.g.: `ReadyState`
            /// </summary>
            public required string Type { get; init; }

            /// <summary>
            /// Integer value of the state, usually incremented in 10th
            /// </summary>
            public required int Value { get; init; }

            /// <summary>
            /// Whether it is the initial state of the state machine
            /// </summary>
            public required bool IsInitial { get; init; }

            /// <summary>
            /// Syntax node
            /// </summary>
            public required FieldDeclarationSyntax Node { get; init; }
        }
    }
}

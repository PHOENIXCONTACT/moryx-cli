using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Moryx.Cli.Templates.Components;
using Moryx.Cli.Templates.Exceptions;
using Moryx.Cli.Templates.Extensions;

namespace Moryx.Cli.Templates.StateTemplate
{
    public class StateTemplate : CSharpFileBase
    {
        private const string StateContextInterface = "IStateContext";

        public StateTemplate(string content) : base(content)
        {
        }

        public static StateTemplate FromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
            var content = File.ReadAllText(fileName);
            return new StateTemplate(content);
        }

        public StateTemplate ImplementIStateContext(string resource)
        {
            var root = _syntaxTree.GetRoot();
            var contextClass = root.FindClass(resource);

            if (contextClass == null)
            {
                throw new TypeNotFoundException(resource);
            }

            root = UpdateClassDefinition(root, resource);
            root = TryToAddInitializing(root, resource);
            root = AddStateProperty(root, resource);

            return new StateTemplate(root.ToFullString());
        }

        

        private SyntaxNode TryToAddInitializing(SyntaxNode root, string context)
        {
            var contextClass = root.FindClass(context);
            if (contextClass == null)
                return root;

            var initializationText = $"StateMachine.Initialize(this).With<{context.StateBase()}>();\n";
            if (contextClass.ToFullString().Contains("StateMachine.Initialize"))
                return root;

            var initializeMethod = contextClass.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.Text == "OnInitialize");
            if (initializeMethod != null)
            {
                var stateInitialization = SyntaxFactory.ParseStatement(initializationText);
                var updatedMethodBody = initializeMethod.Body?.AddStatements(stateInitialization) ?? null;
                var updatedMethod = initializeMethod.WithBody(updatedMethodBody);

                return root.ReplaceNode(initializeMethod, updatedMethod);
            }
            else
            {
                var lastMethod = contextClass.DescendantNodes().OfType<MethodDeclarationSyntax>().LastOrDefault();
                if (lastMethod != null)
                {
                    var comment = SyntaxFactory.Comment("\n\n// Please, remember to initialize the `StateMachine`:\n" + initializationText);
                    var updatedMethod = lastMethod.WithTrailingTrivia(comment);

                    return root.ReplaceNode(lastMethod, updatedMethod);
                }
            }
            return root;
        }

        private SyntaxNode AddStateProperty(SyntaxNode root, string context)
        {
            var contextClass = root.FindClass(context);
            if (contextClass == null)
                return root;

            var existingProperty = root.DescendantNodes().OfType<PropertyDeclarationSyntax>().FirstOrDefault(p => p.Identifier.Text == "State");
            if (existingProperty != null)
                return root;

            var stateProperty = SyntaxFactory
                .PropertyDeclaration(SyntaxFactory.ParseTypeName(context.StateBase()), "State")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ParseExpression($"({context.StateBase()})CurrentState")))
                .NormalizeWhitespace()
                ;

            return InsertMemberBeforeConstructor(root, contextClass, stateProperty);
        }


        private static SyntaxNode UpdateClassDefinition(SyntaxNode root, string context)
        {
            var contextClass = root.FindClass(context);
            if(contextClass == null)
                return root;

            if (!contextClass.BaseList?.Types.Any(t => ((IdentifierNameSyntax)t.Type).Identifier.ValueText == StateContextInterface) ?? false)
            {
                var newClass = contextClass.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(StateContextInterface)));
                root = root
                    .ReplaceNode(contextClass, newClass);

                root = Formatter.Format(root, new AdhocWorkspace());

                root = CSharpSyntaxTree.ParseText(root.ToFullString()).GetRoot();

                var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(" Moryx.StateMachines"));
                if (!((CompilationUnitSyntax)root).Usings.Any(u => u.Name?.ToString() == usingDirective.Name?.ToString()))
                {
                    root = ((CompilationUnitSyntax)root).AddUsings(usingDirective);
                    root = ((CompilationUnitSyntax)root).WithUsings(SyntaxFactory.List(((CompilationUnitSyntax)root).Usings.OrderBy(u => u.Name?.ToString())));
                    root = CSharpSyntaxTree.ParseText(root.ToFullString()).GetRoot();
                }

                root = Formatter.Format(root, new AdhocWorkspace());
            }

            return root;
        }
    }
}
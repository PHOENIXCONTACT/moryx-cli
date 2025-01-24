using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Moryx.Cli.Template.Components;
using Moryx.Cli.Template.Exceptions;

namespace Moryx.Cli.Template.StateBaseTemplate
{
    public partial class StateBaseTemplate : CSharpFileBase
    {
        private const string StateDefinitionAttributeName = "StateDefinition";
        private const string IsInitialParameterName = "IsInitial";

        public StateBaseTemplate(string content) : base(content)
        {
            Parse(_syntaxTree);
        }

        public IEnumerable<ConstructorDeclarationSyntax> Constructors { get; private set; } = [];
        public IEnumerable<StateDefinition> StateDeclarations { get; private set; } = [];

        public static StateBaseTemplate FromFile(string fileName)
        {
            var content = File.ReadAllText(fileName);
            return new StateBaseTemplate(content);
        }

        private void Parse(SyntaxTree syntaxTree)
        {
            var root = syntaxTree.GetRoot();

            StateDeclarations = ExtractStateDefinitions(root);
        }

        private IEnumerable<StateDefinition> ExtractStateDefinitions(SyntaxNode root)
        {
            var result = new List<StateDefinition>();
            var states = root
                .DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .Where(p => p.AttributeLists.Any(a => a.Attributes.Any(attr => attr.Name.ToString() == StateDefinitionAttributeName)));

            foreach (var state in states)
            {
                var attributeArguments = GetAttributeArguments(state, StateDefinitionAttributeName);
                var variable = state.Declaration.Variables.First();
                result.Add(new StateDefinition
                {
                    Name = variable.Identifier.Text,
                    Type = attributeArguments.First(a => a.Key == "").Value,
                    Value = Convert.ToInt32(variable.Initializer?.Value.ToString()),
                    IsInitial = attributeArguments.Any(a => a.Key == IsInitialParameterName && a.Value.ToString() == "true"),
                    Node = state,
                });

            }

            return result;
        }

        public List<KeyValuePair<string, string>> GetAttributeArguments(FieldDeclarationSyntax field, string attributeName)
        {
            var semanticModel = CSharpCompilation.Create("SemanticModelCompilation", [_syntaxTree]).GetSemanticModel(_syntaxTree);

            return field.AttributeLists.Select(
                list => list.Attributes
                    .Where(attribute => attribute.Name.ToString() == attributeName)
                    .Select(attribute =>
                        attribute.ArgumentList?.Arguments.Select(a =>
                            new KeyValuePair<string, string>(
                            a.NameEquals?.Name.Identifier.ValueText ?? "",
                            (a.Expression).ToString())
                            )
                        .ToList())
                    .FirstOrDefault()
             ).FirstOrDefault() ?? [];
        }

        public StateBaseTemplate AddState(string stateType)
        {
            if (StateDeclarations.Any(sd => sd.Type == $"typeof({stateType})"))
            {
                throw new StateAlreadyExistsException(stateType);
            }
            int value = NextConst(StateDeclarations);

            var parameters = new List<AttributeArgumentSyntax>
            {
                SyntaxFactory.AttributeArgument(
                    SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName(stateType)))
            };

            if (!StateDeclarations.Any(sd => sd.IsInitial))
            {
                parameters.Add(SyntaxFactory.AttributeArgument(
                    SyntaxFactory.NameEquals(IsInitialParameterName),
                    null,
                    SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)));

            }

            var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(StateDefinitionAttributeName))
                .WithArgumentList(SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(parameters)));

            var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute));

            var stateDeclaration = SyntaxFactory
                .FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("int"))
                .AddVariables(SyntaxFactory
                    .VariableDeclarator(TypeToConst(stateType))
                    .WithInitializer(SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value))))))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ConstKeyword))
                .AddAttributeLists(attributeList);

            var updatedRoot = InsertStateDeclaration(stateDeclaration);
            updatedRoot = Formatter.Format(updatedRoot, new AdhocWorkspace());
            return new StateBaseTemplate(updatedRoot.ToFullString());
        }

        private SyntaxNode InsertStateDeclaration(FieldDeclarationSyntax fieldDeclaration)
        {
            var root = _syntaxTree.GetRoot();
            var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            

            if (classDeclaration != null)
            {
                root =  InsertMemberBeforeConstructor(root, classDeclaration, fieldDeclaration);
            }
            return root;
        }

        private int NextConst(IEnumerable<StateDefinition> stateDefinitions)
        {
            var result = StateDeclarations
                .OrderByDescending(sd => sd.Value)
                .FirstOrDefault()?
                .Value ?? 0;

            return result - (result % 10) + 10;
        }

        private string TypeToConst(string type)
            => "State" + type.Replace("State", "");

    }
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Moryx.Cli.Template.Exceptions;

namespace Moryx.Cli.Template.StateTemplate
{
    public class StateTemplate
    {
        private string _content;

        public string Content { get => _content; } 

        public StateTemplate(string content)
        {
            _content = content;
        }

        public static StateTemplate FromFile(string fileName)
        {
            if(!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName); 
            }
            var content = File.ReadAllText(fileName);
            return new StateTemplate(content);
        }

        public StateTemplate ImplementIStateContext(string resource)
        {
            var root = CSharpSyntaxTree.ParseText(_content).GetRoot();
            var resourceClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(n => n.Identifier.Text == resource);

            if (resourceClass == null)
            {
                throw new TypeNotFoundException(resource);
            }

            if (!resourceClass.BaseList?.Types.Any(t => ((IdentifierNameSyntax)t.Type).Identifier.ValueText == "IStateContext") ?? false)
            {
                var newClass = resourceClass.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IStateContext")));
                root = root
                    .ReplaceNode(resourceClass, newClass);

                root = Formatter.Format(root, new AdhocWorkspace());

                root = CSharpSyntaxTree.ParseText(root.ToFullString()).GetRoot();

                var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(" Moryx.StateMachines"));
                if (!(root as CompilationUnitSyntax).Usings.Any(u => u.Name.ToString() == usingDirective.Name.ToString()))
                {
                    root = (root as CompilationUnitSyntax).AddUsings(usingDirective);
                    root = (root as CompilationUnitSyntax).WithUsings(SyntaxFactory.List((root as CompilationUnitSyntax).Usings.OrderBy(u => u.Name?.ToString())));
                    root = CSharpSyntaxTree.ParseText(root.ToFullString()).GetRoot();
                }

                root = Formatter.Format(root, new AdhocWorkspace());

            }

            return new StateTemplate(root.ToFullString());
        }

        public void SaveToFile(string filename)
        {
            File.WriteAllText(filename, Content);
        }
    }
}
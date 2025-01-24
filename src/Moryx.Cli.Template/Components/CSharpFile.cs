using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Moryx.Cli.Template.Components
{
    public class CSharpFile : CSharpFileBase
    {
        private SyntaxNode _root;
        
        private NamespaceDeclarationSyntax? _namespaceDeclaration;
        
        public List<string> Types { get; private set; }


        public string NamespaceName
        {
            get => _namespaceDeclaration?.Name.ToString() ?? "";
            set
            {
                UpdateNamespace(value);
            }
        }


        public CSharpFile(string content) : base(content)
        {
            _root = _syntaxTree.GetRoot();
            Types = ScanTypes();
            _namespaceDeclaration = ScanNamespace();
        }

        public static CSharpFile FromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
            var content = File.ReadAllText(fileName);
            return new CSharpFile(content);
        }

        public List<string> ScanTypes()
        {
            return _root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Select(n => n.Identifier.Text)
                .ToList();
        }

        private NamespaceDeclarationSyntax? ScanNamespace()
        {
            var root = _syntaxTree.GetRoot() as CompilationUnitSyntax;

            return root?.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        }

        private void UpdateNamespace(string @namespace)
        {
            if (_namespaceDeclaration != null)
            {
                var newNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(@namespace)).NormalizeWhitespace();
                _root = _root.ReplaceNode(_namespaceDeclaration, newNamespace);
                _content = _root.ToFullString();
            }
        }
    }
}
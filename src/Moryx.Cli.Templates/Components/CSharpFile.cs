using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Moryx.Cli.Templates.Components
{
    public class CSharpFile : CSharpFileBase
    {
        private SyntaxNode _root;
        
        private BaseNamespaceDeclarationSyntax? _namespaceDeclaration;
        
        public List<string> Types { get; private set; }


        public string NamespaceName
        {
            get => _namespaceDeclaration?.Name.ToString() ?? "";
            set
            {
                UpdateNamespace(value);
            }
        }


        public  CSharpFile(string content) : base(content)
        {
            _root = _syntaxTree.GetRoot();
            Types = ScanTypes();
            _namespaceDeclaration = ScanNamespace();
        }

        public static CSharpFile FromFile(string fileName)
        {
            return new CSharpFile(ReadContent(fileName));
        }

        protected static string ReadContent(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
            return File.ReadAllText(fileName);
        }

        public List<string> ScanTypes()
        {
            return _root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Select(n => n.Identifier.Text)
                .ToList();
        }

        private BaseNamespaceDeclarationSyntax? ScanNamespace()
        {
            var root = _syntaxTree.GetRoot() as CompilationUnitSyntax;
            var nds = root?.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            if(nds != null)
            {
                return nds;
            }
            return root?.DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
        }

        private void UpdateNamespace(string @namespace)
        {
            if (_namespaceDeclaration != null)
            {
                var newName = SyntaxFactory.ParseName(@namespace);
                var newNamespace = _namespaceDeclaration.WithName(newName);
                _root = _root.ReplaceNode(_namespaceDeclaration, newNamespace);
                _content = _root.ToFullString();
            }
        }
    }
}
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Moryx.Cli.Template.StateBaseTemplate.StateBaseTemplate;

namespace Moryx.Cli.Template.Components
{
    public class CSharpFileBase
    {
        protected string _content;

        public string Content { get => _content; }

        protected readonly SyntaxTree _syntaxTree;

        public IEnumerable<ConstructorDeclarationSyntax> Constructors { get; private set; }

        public CSharpFileBase(string content)
        {
            _content = content;
            _syntaxTree = CSharpSyntaxTree.ParseText(_content);
            Constructors = ExtractConstructors(_syntaxTree.GetRoot());
        }

        public void SaveToFile(string fileName)
        {
            File.WriteAllText(fileName, _content);
        }

        private IEnumerable<ConstructorDeclarationSyntax> ExtractConstructors(SyntaxNode root)
        {
            return root.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList();
        }

        protected SyntaxNode InsertMemberBeforeConstructor(SyntaxNode root, ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax memberDeclaration)
        {
            SyntaxNode? updatedClassDeclaration;

            var constructor = Constructors.FirstOrDefault();
            if (constructor != null)
            {
                var index = classDeclaration.Members.IndexOf(constructor);

                var members = classDeclaration.Members.Insert(index, UpdateTrivia(memberDeclaration, classDeclaration, index));
                updatedClassDeclaration = classDeclaration.WithMembers(members);
            }
            else
            {
                var firstMethod = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();
                if (firstMethod != null)
                {
                    var index = classDeclaration.Members.IndexOf(firstMethod);
                    var members = classDeclaration.Members.Insert(index, UpdateTrivia(memberDeclaration, classDeclaration, index));
                    updatedClassDeclaration = classDeclaration.WithMembers(members);
                }
                else
                {
                    updatedClassDeclaration = classDeclaration.AddMembers(memberDeclaration);
                }
            }

            if (updatedClassDeclaration != null)
            {
                return root.ReplaceNode(classDeclaration, updatedClassDeclaration);
            }

            return root;
        }

        protected MemberDeclarationSyntax UpdateTrivia(MemberDeclarationSyntax memberDeclaration, ClassDeclarationSyntax classDeclaration, int atIndex)
        {
            if (atIndex > 0)
            {
                var declaration = memberDeclaration
                    .WithLeadingTrivia(classDeclaration.Members[atIndex - 1].GetLeadingTrivia())
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);
                return declaration;
            }
            return memberDeclaration;
        }
    }
}

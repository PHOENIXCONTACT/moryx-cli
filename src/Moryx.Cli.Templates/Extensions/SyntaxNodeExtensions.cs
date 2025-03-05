using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Cli.Templates.Extensions
{
    internal static class SyntaxNodeExtensions
    {
        internal static ClassDeclarationSyntax? FindClass(this SyntaxNode root, string className)
        {
            return root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(n => n.Identifier.Text == className);
        }
    }
}

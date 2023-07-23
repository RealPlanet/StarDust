using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace StarDust.Generators
{
    [Generator]
    public class NodeGetChildrenGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //Debugger.Launch();
            CSharpCompilation compilation = (CSharpCompilation)context.Compilation;

            INamedTypeSymbol? immutableArrayType = compilation.GetTypeByMetadataName("System.Collections.Immutable.ImmutableArray`1");
            INamedTypeSymbol? separatedSyntaxListType = compilation.GetTypeByMetadataName("StarDust.Code.Syntax.SeparatedSyntaxList`1");
            INamedTypeSymbol? syntaxNodeType = compilation.GetTypeByMetadataName("StarDust.Code.Syntax.Node");
            //if (immutableArrayType == null || separatedSyntaxListType == null || syntaxNodeType == null)
            //    return;

            IEnumerable<INamedTypeSymbol> types = GetAllTypes(compilation.Assembly);
            IEnumerable<INamedTypeSymbol> syntaxNodeTypes = types.Where(t => !t.IsAbstract && IsPartial(t) && IsDerivedFrom(t, syntaxNodeType));

            SourceText sourceText;
            const string indentString = "    ";
            using (StringWriter stringWriter = new())
            using (IndentedTextWriter indentedTextWriter = new(stringWriter, indentString))
            {
                indentedTextWriter.WriteLine("using System;");
                indentedTextWriter.WriteLine("using System.Collections.Generic;");
                indentedTextWriter.WriteLine("using System.Collections.Immutable;");
                indentedTextWriter.WriteLine();

                foreach (INamedTypeSymbol type in syntaxNodeTypes)
                {
                    indentedTextWriter.WriteLine($"namespace {type.ContainingNamespace}");
                    OpenScope(indentedTextWriter);

                    indentedTextWriter.WriteLine($"public sealed partial class {type.Name}");
                    OpenScope(indentedTextWriter);

                    // ehe xD
                    indentedTextWriter.WriteLine("public override IEnumerable<Node> GetChildren()");
                    OpenScope(indentedTextWriter);
                    foreach (IPropertySymbol property in type.GetMembers().OfType<IPropertySymbol>())
                    {
                        if (property.Type is INamedTypeSymbol propertyType)
                        {
                            // property.Type derived from Node
                            // property.Type is ImmutableArray<T> where T : Node
                            // property.Type is SeparatedSyntaxList<T> where T : Node
                            if (IsDerivedFrom(propertyType, syntaxNodeType))
                            {
                                bool canBeNull = property.NullableAnnotation == NullableAnnotation.Annotated;
                                if (canBeNull)
                                {
                                    indentedTextWriter.WriteLine($"if({property.Name} is not null)");
                                    indentedTextWriter.Indent++;
                                }

                                indentedTextWriter.WriteLine($"yield return {property.Name};");
                                if (canBeNull)
                                    indentedTextWriter.Indent--;
                            }
                            else if (propertyType.TypeArguments.Length == 1 &&
                                             IsDerivedFrom(propertyType.TypeArguments[0], syntaxNodeType) &&
                                             SymbolEqualityComparer.Default.Equals(propertyType.OriginalDefinition, immutableArrayType)
                               )
                            {
                                indentedTextWriter.WriteLine($"foreach(var child in {property.Name})");
                                indentedTextWriter.WriteLine($"{indentString}yield return child;");
                            }
                            else if (SymbolEqualityComparer.Default.Equals(propertyType.OriginalDefinition, separatedSyntaxListType) &&
                                              IsDerivedFrom(propertyType.TypeArguments[0], syntaxNodeType))
                            {
                                indentedTextWriter.WriteLine($"foreach (var child in {property.Name}.GetWithSeparators())");
                                indentedTextWriter.WriteLine($"{indentString}yield return child;");
                            }
                            else
                            {
                                indentedTextWriter.WriteLine($"// {property.Name};");
                            }
                        }
                    }

                    CloseScope(indentedTextWriter);

                    CloseScope(indentedTextWriter);
                    indentedTextWriter.WriteLine();
                    CloseScope(indentedTextWriter);
                }

                //CloseScope(indentedTextWriter);
                indentedTextWriter.WriteLine();

                indentedTextWriter.Flush();
                sourceText = SourceText.From(stringWriter.ToString(), Encoding.UTF8);
            }

            string syntaxNodeFileName = syntaxNodeType.DeclaringSyntaxReferences[0].SyntaxTree.FilePath;
            string directory = Path.GetDirectoryName(syntaxNodeFileName);
            context.AddSource("GetChildrenImpl.g.cs", sourceText);
            //var fileName = Path.Combine(directory, "Node_GetChildren.txt");
            //using (var writer = new StreamWriter(fileName))
            //    sourceText.Write(writer);
        }

        private static void CloseScope(IndentedTextWriter indentedTextWriter)
        {
            indentedTextWriter.Indent--;
            indentedTextWriter.WriteLine("}");
        }

        private static void OpenScope(IndentedTextWriter indentedTextWriter)
        {
            indentedTextWriter.WriteLine("{");
            indentedTextWriter.Indent++;
        }

        private bool IsDerivedFrom(ITypeSymbol type, INamedTypeSymbol baseType)
        {
            ITypeSymbol? current = type;
            while (current != null)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                    return true;

                current = current.BaseType;
            }

            return false;
        }

        private bool IsPartial(INamedTypeSymbol type)
        {
            foreach (SyntaxReference declaration in type.DeclaringSyntaxReferences)
            {
                SyntaxNode syntax = declaration.GetSyntax();
                if (syntax is TypeDeclarationSyntax typeDeclaration)
                {
                    foreach (SyntaxToken mod in typeDeclaration.Modifiers)
                    {
                        if (mod.ValueText == "partial")
                            return true;
                    }
                }
            }
            return false;
        }

        private IEnumerable<INamedTypeSymbol> GetAllTypes(IAssemblySymbol symbol)
        {
            List<INamedTypeSymbol> result = new();
            GetAllTypes(result, symbol.GlobalNamespace);
            result.Sort((x, y) => x.MetadataName.CompareTo(y.MetadataName));
            return result;
        }

        private void GetAllTypes(List<INamedTypeSymbol> result, INamespaceOrTypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol type)
                result.Add(type);

            foreach (ISymbol child in symbol.GetMembers())
            {
                if (child is INamespaceOrTypeSymbol nsChild)
                    GetAllTypes(result, nsChild);
            }
        }
    }
}
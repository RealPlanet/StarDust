using StarDust.Code.IO;
using StarDust.Code.Syntax;

namespace StarDust.Code.Symbols
{
    internal sealed class SymbolPrinter
    {
        public static void WriteTo(Symbol symbol, TextWriter writer)
        {
            switch (symbol.SymbolType)
            {
                case SymbolType.FUNCTION:
                    WriteFunctionTo((FunctionSymbol)symbol, writer);
                    break;
                case SymbolType.GLOBAL_VARIABLE:
                    WriteGlobalVariableTo((GlobalVariableSymbol)symbol, writer);
                    break;
                case SymbolType.LOCAL_VARIABLE:
                    WriteLocalVariableTo((LocalVariableSymbol)symbol, writer);
                    break;
                case SymbolType.PARAMETER:
                    WriteParameterTo((ParameterSymbol)symbol, writer);
                    break;
                case SymbolType.TYPE:
                    WriteTypeTo((TypeSymbol)symbol, writer);
                    break;
                default:
                    throw new Exception($"Unexpected symbol {symbol.SymbolType}");
            }
        }
        private static void WriteFunctionTo(FunctionSymbol symbol, TextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.FUNCTION_KEYWORD);
            writer.WriteSpace();

            if (symbol.ReturnType != TypeSymbol.Void)
            {
                symbol.ReturnType.WriteTo(writer);
                writer.WriteSpace();
            }

            writer.WriteIdentifier(symbol.Name);
            writer.WritePunctuation(SyntaxType.OPEN_PARENTHESIS_TOKEN);

            for (int i = 0; i < symbol.Parameters.Length; i++)
            {
                ParameterSymbol? p = symbol.Parameters[i];
                if (i > 0)
                {
                    writer.WritePunctuation(SyntaxType.COMMA_TOKEN);
                    writer.WriteSpace();
                }

                p.WriteTo(writer);
            }

            writer.WritePunctuation(SyntaxType.CLOSE_PARENTHESIS_TOKEN);
        }
        private static void WriteGlobalVariableTo(GlobalVariableSymbol symbol, TextWriter writer)
        {
            writer.WriteKeyword(symbol.IsReadOnly ? SyntaxType.CONST_KEYWORD : SyntaxType.VAR_KEYWORD);
            writer.WriteSpace();
            symbol.Type?.WriteTo(writer);
            writer.WriteSpace();
            writer.WriteIdentifier(symbol.Name);
        }
        private static void WriteLocalVariableTo(LocalVariableSymbol symbol, TextWriter writer)
        {
            writer.WriteKeyword(symbol.IsReadOnly ? SyntaxType.CONST_KEYWORD : SyntaxType.VAR_KEYWORD);
            writer.WriteSpace();
            symbol.Type?.WriteTo(writer);
            writer.WriteSpace();
            writer.WriteIdentifier(symbol.Name);
        }
        private static void WriteParameterTo(ParameterSymbol symbol, TextWriter writer)
        {
            symbol.Type.WriteTo(writer);
            writer.WriteSpace();
            writer.WriteIdentifier(symbol.Name);
        }
        private static void WriteTypeTo(TypeSymbol symbol, TextWriter writer)
        {
            writer.WriteIdentifier(symbol.Name);
        }
    }
}

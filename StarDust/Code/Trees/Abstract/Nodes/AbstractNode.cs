using StarDust.Code.AST.Processing;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Nodes
{
    internal enum AbstractNodeType
    {
        // Statement
        SEQUENCE_POINT_STATEMENT,
        BLOCK_STATEMENT,
        NOP_STATEMENT,
        EXPRESSION_STATEMENT,
        VARIABLE_DECLARATION_STATEMENT,
        IF_STATEMENT,
        WHILE_STATEMENT,
        DO_WHILE_STATEMENT,
        FOR_STATEMENT,
        CONDITIONAL_GOTO_STATEMENT,
        GOTO_STATEMENT,
        LABEL_STATEMENT,
        RETURN_STATEMENT,

        // Expression
        ERROR_EXPRESSION,
        LITERAL_EXPRESSION,
        COMPOUND_ASSIGNMENT_EXPRESSION,
        UNARY_EXPRESSION,
        BINARY_EXPRESSION,
        VARIABLE_EXPRESSION,
        ASSIGNMENT_EXPRESSION,
        CALL_EXPRESSION,
        CONVERSION_EXPRESSION,
    }

    /// <summary>
    /// Rapresents a Node of the Bound Syntax Tree
    /// </summary>
    internal abstract class AbstractNode
    {
        /// <summary>
        /// The type of node rapresented by an enum value
        /// </summary>
        public abstract AbstractNodeType NodeType { get; }
        public Node Syntax { get; }

        protected AbstractNode(Node syntax)
        {
            Syntax = syntax;
        }

        /// <summary>
        /// Prints this node and all of it's children as a code representation similar to the source code
        /// </summary>
        /// <returns>The string containing the prettified tree repesentation</returns>
        public override string ToString()
        {
            using (StringWriter writer = new())
            {
                this.WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}

using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Factory;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Processing;
using StarDust.Code.AST.Statements;
using System.CodeDom.Compiler;

namespace StarDust.Code.AST.ControlFlow
{
    internal sealed class ControlFlowGraph
    {
        #region Properties
        public BasicBlock Start { get; }
        public BasicBlock End { get; }
        public List<BasicBlock> Blocks { get; }
        public List<BasicBlockBranch> Branches { get; }
        #endregion

        private ControlFlowGraph(
            BasicBlock start,
            BasicBlock end,
            List<BasicBlock> blocks,
            List<BasicBlockBranch> branches)
        {
            Start = start;
            End = end;
            Blocks = blocks;
            Branches = branches;
        }

        public sealed class BasicBlock
        {
            /// <summary>
            /// List of instructions (flattened) where only the first instruction can be targetted by a goto instruction and only the last instruction can escape the block.
            /// </summary>
            public List<AbstractStatement> Statements { get; } = new();

            /// <summary>
            /// Each node knows the nodes which are entering and exiting this block of instructions
            /// </summary>
            public List<BasicBlockBranch> Incoming { get; } = new();

            /// <summary>
            /// Each node knows the nodes which are entering and exiting this block of instructions
            /// </summary>
            public List<BasicBlockBranch> Outgoing { get; } = new();
            public bool IsStart { get; }
            public bool IsEnd { get; }
            public BasicBlock() { }
            public BasicBlock(bool isStart)
            {
                IsStart = isStart;
                IsEnd = !isStart;
            }
            public override string ToString()
            {
                if (IsStart)
                    return "<start>";

                if (IsEnd)
                    return "<end>";

                using (StringWriter writer = new())
                using (IndentedTextWriter indentedWriter = new(writer))
                {
                    foreach (AbstractStatement? statement in Statements)
                        statement.WriteTo(indentedWriter);

                    return writer.ToString();
                }
            }
        }
        public sealed class BasicBlockBranch
        {
            public BasicBlock From { get; }
            public BasicBlock To { get; }
            public AbstractExpression? Condition { get; }
            public BasicBlockBranch(BasicBlock from, BasicBlock to, AbstractExpression? condition)
            {
                From = from;
                To = to;
                Condition = condition;
            }
            public override string ToString()
            {
                if (Condition is null)
                    return string.Empty;

                return Condition.ToString();
            }
        }
        public sealed class BasicBlockBuilder
        {
            private readonly List<BasicBlock> Blocks = new();
            private readonly List<AbstractStatement> StatementList = new();
            public List<BasicBlock> Build(AbstractBlockStatement block)
            {
                foreach (AbstractStatement? statement in block.Statements)
                {
                    switch (statement.NodeType)
                    {
                        case AbstractNodeType.LABEL_STATEMENT:
                            StartBlock();
                            StatementList.Add(statement);
                            break;
                        case AbstractNodeType.NOP_STATEMENT:
                        case AbstractNodeType.SEQUENCE_POINT_STATEMENT:
                        case AbstractNodeType.EXPRESSION_STATEMENT:
                        case AbstractNodeType.VARIABLE_DECLARATION_STATEMENT:
                            StatementList.Add(statement);
                            break;
                        case AbstractNodeType.RETURN_STATEMENT:
                        case AbstractNodeType.CONDITIONAL_GOTO_STATEMENT:
                        case AbstractNodeType.GOTO_STATEMENT:
                            StatementList.Add(statement);
                            StartBlock();
                            break;

                        default:
                            throw new Exception($"Unexpected statement: {statement.NodeType}");
                    }
                }

                EndBlock();
                return Blocks.ToList();
            }

            private void StartBlock()
            {
                EndBlock();
            }

            private void EndBlock()
            {
                if (StatementList.Count > 0)
                {
                    BasicBlock block = new();
                    block.Statements.AddRange(StatementList);
                    Blocks.Add(block);
                    StatementList.Clear();
                }
            }
        }
        public sealed class GraphBuilder
        {
            private readonly Dictionary<AbstractStatement, BasicBlock> BlockFromStatement = new();
            private readonly Dictionary<AbstractLabel, BasicBlock> BlockFromLabel = new();
            private readonly List<BasicBlockBranch> Branches = new();
            private readonly BasicBlock Start = new(true);
            private readonly BasicBlock End = new(false);
            public ControlFlowGraph Build(List<BasicBlock> blocks)
            {
                if (blocks.Count == 0)
                    Connect(Start, End);
                else
                    Connect(Start, blocks[0]);

                foreach ((BasicBlock block, AbstractStatement statement) in from block in blocks
                                                                            from statement in block.Statements
                                                                            select (block, statement))
                {
                    BlockFromStatement.Add(statement, block);
                    if (statement is AbstractLabelStatement l)
                        BlockFromLabel.Add(l.Label, block);
                }

                for (int i = 0; i < blocks.Count; i++)
                {
                    BasicBlock? current = blocks[i];
                    BasicBlock next = i == blocks.Count - 1 ? End : blocks[i + 1];

                    foreach (AbstractStatement? statement in current.Statements)
                    {
                        bool isLastStatementInBlock = statement == current.Statements.Last();
                        switch (statement.NodeType)
                        {
                            case AbstractNodeType.NOP_STATEMENT:
                            case AbstractNodeType.VARIABLE_DECLARATION_STATEMENT:
                            case AbstractNodeType.EXPRESSION_STATEMENT:
                            case AbstractNodeType.SEQUENCE_POINT_STATEMENT:
                            case AbstractNodeType.LABEL_STATEMENT:
                                {
                                    if (isLastStatementInBlock)
                                        Connect(current, next);
                                    break;
                                }

                            case AbstractNodeType.RETURN_STATEMENT:
                                {
                                    Connect(current, End);
                                    break;
                                }
                            case AbstractNodeType.CONDITIONAL_GOTO_STATEMENT:
                                {
                                    AbstractConditionalGotoStatement cgs = (AbstractConditionalGotoStatement)statement;
                                    BasicBlock? thenBlock = BlockFromLabel[cgs.Label];
                                    BasicBlock? elseBlock = next;

                                    AbstractExpression? negatedCondition = Negate(cgs.Condition);
                                    AbstractExpression? thenCondition = cgs.JumpIfTrue ? cgs.Condition : negatedCondition;
                                    AbstractExpression? elseCondition = cgs.JumpIfTrue ? negatedCondition : cgs.Condition;

                                    Connect(current, thenBlock, thenCondition);
                                    Connect(current, elseBlock, elseCondition);
                                    break;
                                }
                            case AbstractNodeType.GOTO_STATEMENT:
                                {
                                    AbstractGotoStatement gs = (AbstractGotoStatement)statement;
                                    BasicBlock? toBlock = BlockFromLabel[gs.Label];
                                    Connect(current, toBlock);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected statement: {statement.NodeType}");
                        }
                    }
                }

            ScanAgain:
                foreach (BasicBlock? block in blocks)
                {
                    if (block.Incoming.Count == 0)
                    {
                        RemoveBlock(blocks, block);
                        goto ScanAgain;
                    }
                }

                blocks.Insert(0, Start);
                blocks.Add(End);

                return new(Start, End, blocks, Branches);
            }

            private void RemoveBlock(List<BasicBlock> blocks, BasicBlock block)
            {
                blocks.Remove(block);
                foreach (BasicBlockBranch? branch in block.Incoming)
                {
                    branch.From.Outgoing.Remove(branch);
                    Branches.Remove(branch);
                }

                foreach (BasicBlockBranch? branch in block.Outgoing)
                {
                    branch.To.Incoming.Remove(branch);
                    Branches.Remove(branch);
                }
            }

            private static AbstractExpression Negate(AbstractExpression condition)
            {
                AbstractUnaryExpression? negated = AbstractNodeFactory.Not(condition.Syntax, condition);
                if (negated.ConstantValue != null)
                    return new AbstractLiteralExpression(condition.Syntax, negated.ConstantValue.Value);

                return negated;
            }

            private void Connect(BasicBlock from, BasicBlock to, AbstractExpression? condition = null)
            {
                if (condition is AbstractLiteralExpression l)
                {
                    bool value = (bool)l.Value;
                    if (value)
                        condition = null;
                    else
                        return;
                }

                BasicBlockBranch branch = new(from, to, condition);
                from.Outgoing.Add(branch);
                to.Incoming.Add(branch);
                Branches.Add(branch);
            }
        }
        public void WriteTo(TextWriter writer)
        {
            static string Quote(string text)
            {
                return "\"" + text.TrimEnd().Replace("\\", "\\\\").Replace("\"", "\\\"").Replace(Environment.NewLine, "\\l") + "\"";
            }

            writer.WriteLine("digraph G {");
            Dictionary<BasicBlock, string> blocksIds = new();
            for (int i = 0; i < Blocks.Count; i++)
            {
                BasicBlock? node = Blocks[i];
                string id = $"N{i}";
                blocksIds.Add(node, id);
            }

            foreach (BasicBlock? block in Blocks)
            {
                string id = blocksIds[block];
                string label = Quote(block.ToString());
                writer.WriteLine($"     {id} [label = {label}, shape = box]");
            }

            foreach (BasicBlockBranch? branch in Branches)
            {
                string fromId = blocksIds[branch.From];
                string toId = blocksIds[branch.To];
                string label = Quote(branch.ToString());
                writer.WriteLine($"     {fromId} -> {toId} [label = {label}]");
            }

            writer.WriteLine("}");
        }
        public static ControlFlowGraph Create(AbstractBlockStatement body)
        {
            BasicBlockBuilder? basicBlockBuilder = new();
            List<BasicBlock> blocks = basicBlockBuilder.Build(body);

            GraphBuilder graphBuilder = new();
            return graphBuilder.Build(blocks);
        }
        public static bool AllPathsReturn(AbstractBlockStatement body)
        {
            ControlFlowGraph graph = Create(body);
            foreach (BasicBlockBranch? branch in graph.End.Incoming)
            {
                AbstractStatement? lastStatement = branch.From.Statements.LastOrDefault();
                if (lastStatement == null || lastStatement.NodeType != AbstractNodeType.RETURN_STATEMENT)
                    return false;
            }

            return true;
        }
    }
}

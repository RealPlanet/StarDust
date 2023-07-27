using StarDust.Code.AST;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Statements;
using StarDust.Code.Symbols;
using StarDust.Emitter;
using System.Collections.Immutable;

namespace StarDust.Code.Emit
{
    internal sealed class Emitter
    {
        private readonly ReportBag Report = new();
        private ModuleEmitter _InternalEmitter;
        private Dictionary<FunctionSymbol, MethodEmitter> _Methods = new();

        private Emitter(string moduleName)
        {
            _InternalEmitter = new ModuleEmitter(moduleName);

            #region Reference lookup

            #endregion

            //  Types
            //  Any     -> System.Object
            //  Bool    -> System.Boolean
            //  Int     -> System.Int32
            //  String  -> System.String
            //  Void    -> System.Void

            List<(TypeSymbol Type, string MetadataName)>? builtInType = new()
            {
              //(TypeSymbol.Any     , "Any"),
              (TypeSymbol.Bool    , "Boolean"),
              (TypeSymbol.Int     , "Int32"),
              (TypeSymbol.String  , "String"),
              (TypeSymbol.Void    , "Void"),
            };
        }

        ////
        ////    #region Build Type references
        //     foreach ((TypeSymbol typeSymbol, string metadataName) in builtInType)
        //     {
        //         TypeReference? typeRef = ResolveType(typeSymbol.Name, metadataName);
        //         KnownTypes.Add(typeSymbol, typeRef);
        //     }
        //     #endregion
        //
        //    ObjectEqualsReference = ResolveMethod("System.Object", "Equals", new[] { "System.Object", "System.Object" });
        //
        //    ConsoleWriteLineReference = ResolveMethod("System.Console", "WriteLine", new[] { "System.Object" });
        //    ConsoleReadLineReference = ResolveMethod("System.Console", "ReadLine", Array.Empty<string>());
        //
        //    StringConcat2Reference = ResolveMethod("System.String", "Concat", new[] { "System.String", "System.String" });
        //    StringConcat3Reference = ResolveMethod("System.String", "Concat", new[] { "System.String", "System.String", "System.String" });
        //    StringConcat4Reference = ResolveMethod("System.String", "Concat", new[] { "System.String", "System.String", "System.String", "System.String" });
        //    StringConcatArrayReference = ResolveMethod("System.String", "Concat", new[] { "System.String[]" });
        //
        //    ConvertToBooleanReference = ResolveMethod("System.Convert", "ToBoolean", new[] { "System.Object" });
        //    ConvertToInt32Reference = ResolveMethod("System.Convert", "ToInt32", new[] { "System.Object" });
        //    ConvertToStringReference = ResolveMethod("System.Convert", "ToString", new[] { "System.Object" });
        //
        //    RandomReference = ResolveType(null, "System.Random");
        //    RandomCtorReference = ResolveMethod("System.Random", ".ctor", Array.Empty<string>());
        //    RandomNextReference = ResolveMethod("System.Random", "Next", new[] { "System.Int32" });
        //
        //    DebuggableAttributeCtorReference = ResolveMethod("System.Diagnostics.DebuggableAttribute", ".ctor", new[] { "System.Boolean", "System.Boolean" });
        //
        //    TypeReference? objectType = KnownTypes[TypeSymbol.Any];
        //    if (objectType is null)
        //    {
        //        TypeDefinition = null!;
        //        return;
        //    }
        //
        //    TypeDefinition = new("", "Program", TypeAttributes.Abstract | TypeAttributes.Sealed, objectType);
        //    AssemblyDefinition.MainModule.Types.Add(TypeDefinition);
        //}
        //
        //public static ImmutableArray<Report> Emit(AbstractProgram program, string moduleName, string[] references, string outputPath)
        //{
        //    if (program.Report.HasErrors())
        //        return program.Report;
        //
        //    Emitter emitter = new(moduleName, references);
        //
        //    return emitter.Emit(program, outputPath);
        //}
        //
        public ImmutableArray<Report> Emit(AbstractProgram program, string outputPath)
        {
            if (Report.Any())
                return Report.ToImmutableArray();

            //TypeDefinition = new("", "Program", TypeAttributes.Abstract | TypeAttributes.Sealed, KnownTypes[TypeSymbol.Any]);
            //AssemblyDefinition.MainModule.Types.Add(TypeDefinition);

            foreach (KeyValuePair<FunctionSymbol, AbstractBlockStatement> functionWithBody in program.Functions)
                EmitFunctionDeclaration(functionWithBody.Key);

            foreach (KeyValuePair<FunctionSymbol, AbstractBlockStatement> functionWithBody in program.Functions)
                EmitFunctionBody(functionWithBody.Key, functionWithBody.Value);

            //if (program.MainFunction != null)
            //    AssemblyDefinition.EntryPoint = Methods[program.MainFunction];

            // System.Diagnostics
            //CustomAttribute attr = new(DebuggableAttributeCtorReference);
            //attr.ConstructorArguments.Add(new CustomAttributeArgument(KnownTypes[TypeSymbol.Bool], true));
            //attr.ConstructorArguments.Add(new CustomAttributeArgument(KnownTypes[TypeSymbol.Bool], true));
            //AssemblyDefinition.CustomAttributes.Add(attr);

            string? symbolsPath = Path.ChangeExtension(outputPath, ".pdb");
            using (FileStream? outputStream = File.Create(outputPath))
            using (FileStream? symbolStream = File.Create(symbolsPath))
            {
                //WriterParameters writerParameters = new()
                //{
                //    WriteSymbols = true,
                //    SymbolStream = symbolStream,
                //    SymbolWriterProvider = new PortablePdbWriterProvider()
                //};
                //
                //AssemblyDefinition.Write(outputStream, writerParameters);
            }   //



            return Report.ToImmutableArray();
        }

        private void EmitFunctionDeclaration(FunctionSymbol function)
        {
            var methodEmitter = _InternalEmitter.AddMethod(function.Name);

            //TypeReference? type = KnownTypes[function.ReturnType];
            //MethodDefinition method = new(function.Name, MethodAttributes.Static | MethodAttributes.Private, type);

            // TODO
            foreach (ParameterSymbol? parameter in function.Parameters)
            {
            //   TypeReference? parameterType = KnownTypes[parameter.Type];
            //   const ParameterAttributes parameterAttributes = ParameterAttributes.None;
            //   ParameterDefinition parameterDefinition = new(parameterType.Name, parameterAttributes, parameterType);
            //   method.Parameters.Add(parameterDefinition);
            }

            //TypeDefinition.Methods.Add(method);
            _Methods.Add(function, methodEmitter);
        }
        private readonly List<(int InstructionIndex, AbstractLabel Target)> LabelReferences = new();
        private readonly Dictionary<AbstractLabel, int> Labels = new();
        private readonly Dictionary<VariableSymbol, object /*VariableDefinition*/> Locals = new();

        private void EmitFunctionBody(FunctionSymbol function, AbstractBlockStatement body)
        {
            MethodEmitter? method = _Methods[function];

            Locals.Clear();
            LabelReferences.Clear();
            Labels.Clear();

            foreach (AbstractStatement? statement in body.Statements)
                EmitStatement(method, statement);

            foreach ((int InstructionIndex, AbstractLabel Target) in LabelReferences)
            {
                AbstractLabel? targetLabel = Target;
                int targetInstructionIndex = Labels[targetLabel];
               // Instruction? targetInstruction = ilProcessor.Body.Instructions[targetInstructionIndex];
               // Instruction? instructionToFix = ilProcessor.Body.Instructions[InstructionIndex];
               // instructionToFix.Operand = targetInstruction;
            }

            //method.Body.Optimize();
            //method.DebugInformation.Scope = new(method.Body.Instructions.First(), method.Body.Instructions.Last());
            //foreach (var local in Locals)
            //{
            //    var symbol = local.Key;
            //    var definition = local.Value;
            //    VariableDebugInformation varDebugInfo = new(definition, symbol.Name);
            //    method.DebugInformation.Scope.Variables.Add(varDebugInfo);
            //}
        }
        //
        private void EmitStatement(MethodEmitter ilProcessor, AbstractStatement node)
        {
            switch (node.NodeType)
            {
                case AbstractNodeType.SEQUENCE_POINT_STATEMENT:
                    throw new Exception();
                    //EmitSequencePointStatement(ilProcessor, (AbstractSequencePointStatement)node);
                    break;
                //case AbstractNodeType.NOP_STATEMENT:
                //    EmitNopStatement(ilProcessor, (AbstractNopStatement)node);
                //    break;
                //case AbstractNodeType.EXPRESSION_STATEMENT:
                //    EmitExpressionStatement(ilProcessor, (AbstractExpressionStatement)node);
                //    break;
                //case AbstractNodeType.VARIABLE_DECLARATION_STATEMENT:
                //    EmitVariableDeclaration(ilProcessor, (AbstractVariableDeclaration)node);
                //    break;
                //case AbstractNodeType.CONDITIONAL_GOTO_STATEMENT:
                //    EmitConditionalGotoStatement(ilProcessor, (AbstractConditionalGotoStatement)node);
                //    break;
                //case AbstractNodeType.GOTO_STATEMENT:
                //    EmitGotoStatement(ilProcessor, (AbstractGotoStatement)node);
                //    break;
                //case AbstractNodeType.LABEL_STATEMENT:
                //    EmitLabelStatement(ilProcessor, (AbstractLabelStatement)node);
                //    break;
                //case AbstractNodeType.RETURN_STATEMENT:
                //    EmitReturnStatement(ilProcessor, (AbstractReturnStatement)node);
                //    break;
                default:
                    throw new Exception($"Unexpected node type {node.NodeType}");
            }
        }
        //
        //private void EmitSequencePointStatement(ILProcessor ilProcessor, AbstractSequencePointStatement node)
        //{
        //    int index = ilProcessor.Body.Instructions.Count;
        //    EmitStatement(ilProcessor, node.Statement);
        //    Instruction instruction = ilProcessor.Body.Instructions[index];
        //    if (!Documents.TryGetValue(node.Location.Text, out Document? document))
        //    {
        //        var fullPath = Path.GetFullPath(node.Location.Text.FileName);
        //        document = new(fullPath);
        //        Documents.Add(node.Location.Text, document);
        //    }
        //
        //    SequencePoint sequencePoint = new(instruction, document)
        //    {
        //        // 1 BASED NOT 0 BASED
        //        StartLine = node.Location.StartLine + 1,
        //        StartColumn = node.Location.StartCharacter + 1,
        //        EndLine = node.Location.EndLine + 1,
        //        EndColumn = node.Location.EndCharacter + 1,
        //    };
        //
        //    // TODO: Set text info
        //    ilProcessor.Body.Method.DebugInformation.SequencePoints.Add(sequencePoint);
        //}
        //
        //private static void EmitNopStatement(ILProcessor ilProcessor, AbstractNopStatement _) => ilProcessor.Emit(OpCodes.Nop);
        //
        //private void EmitExpressionStatement(ILProcessor ilProcessor, AbstractExpressionStatement node)
        //{
        //    EmitExpression(ilProcessor, node.Expression);
        //    if (node.Expression.Type != TypeSymbol.Void)
        //        ilProcessor.Emit(OpCodes.Pop);
        //}
        //
        //private void EmitVariableDeclaration(ILProcessor ilProcessor, AbstractVariableDeclaration node)
        //{
        //    TypeReference typeReference = KnownTypes[node.Variable.Type];
        //    VariableDefinition variableDefinition = new(typeReference);
        //    Locals.Add(node.Variable, variableDefinition);
        //    ilProcessor.Body.Variables.Add(variableDefinition);
        //
        //    EmitExpression(ilProcessor, node.Initializer);
        //    ilProcessor.Emit(OpCodes.Stloc, variableDefinition);
        //}
        //
        //private void EmitConditionalGotoStatement(ILProcessor ilProcessor, AbstractConditionalGotoStatement node)
        //{
        //    EmitExpression(ilProcessor, node.Condition);
        //
        //    OpCode opCode = node.JumpIfTrue ? OpCodes.Brtrue : OpCodes.Brfalse;
        //    LabelReferences.Add((ilProcessor.Body.Instructions.Count, node.Label));
        //    ilProcessor.Emit(opCode, Instruction.Create(OpCodes.Nop));
        //}
        //
        //private void EmitGotoStatement(ILProcessor ilProcessor, AbstractGotoStatement node)
        //{
        //    LabelReferences.Add((ilProcessor.Body.Instructions.Count, node.Label));
        //    ilProcessor.Emit(OpCodes.Br, Instruction.Create(OpCodes.Nop));
        //}
        //
        //private void EmitLabelStatement(ILProcessor ilProcessor, AbstractLabelStatement node) => Labels.Add(node.Label, ilProcessor.Body.Instructions.Count);
        //
        //private void EmitReturnStatement(ILProcessor ilProcessor, AbstractReturnStatement node)
        //{
        //    if (node.Expression != null)
        //        EmitExpression(ilProcessor, node.Expression);
        //    ilProcessor.Emit(OpCodes.Ret);
        //}
        //
        //private void EmitExpression(ILProcessor ilProcessor, AbstractExpression node)
        //{
        //    if (node.ConstantValue != null)
        //    {
        //        EmitConstantExpression(ilProcessor, node);
        //        return;
        //    }
        //
        //    switch (node.AbstractType)
        //    {
        //        case AbstractNodeType.ERROR_EXPRESSION:
        //            EmitErrorExpression(ilProcessor, (AbstractErrorExpression)node);
        //            break;
        //        case AbstractNodeType.UNARY_EXPRESSION:
        //            EmitUnaryExpression(ilProcessor, (AbstractUnaryExpression)node);
        //            break;
        //        // Literal expressions are not processed and folded into constants
        //        //case AbstractNodeType.LITERAL_EXPRESSION:
        //        //    EmitLiteralExpression(ilProcessor, (AbstractLiteralExpression)node);
        //        //    break;
        //        case AbstractNodeType.BINARY_EXPRESSION:
        //            EmitBinaryExpression(ilProcessor, (AbstractBinaryExpression)node);
        //            break;
        //        case AbstractNodeType.VARIABLE_EXPRESSION:
        //            EmitVariableExpression(ilProcessor, (AbstractVariableExpression)node);
        //            break;
        //        case AbstractNodeType.ASSIGNMENT_EXPRESSION:
        //            EmitAssignmentExpression(ilProcessor, (AbstractAssignmentExpression)node);
        //            break;
        //        case AbstractNodeType.CALL_EXPRESSION:
        //            EmitCallExpression(ilProcessor, (AbstractCallExpression)node);
        //            break;
        //        case AbstractNodeType.CONVERSION_EXPRESSION:
        //            EmitConversionExpression(ilProcessor, (AbstractConversionExpression)node);
        //            break;
        //        default:
        //            throw new Exception($"Unexpected node type {node.AbstractType}");
        //    }
        //}
        //
        //private void EmitErrorExpression(ILProcessor ilProcessor, AbstractErrorExpression node) => throw new NotImplementedException();
        //
        //private static void EmitConstantExpression(ILProcessor ilProcessor, AbstractExpression node)
        //{
        //    // int
        //    // bool
        //    // string
        //
        //    // A constant cannot be null here
        //    Debug.Assert(node.ConstantValue is not null);
        //    if (node.Type == TypeSymbol.Int)
        //    {
        //        int value = (int)node.ConstantValue.Value;
        //        ilProcessor.Emit(OpCodes.Ldc_I4, value);
        //        return;
        //    }
        //
        //    if (node.Type == TypeSymbol.Bool)
        //    {
        //        bool value = (bool)node.ConstantValue.Value;
        //        OpCode instruction = value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
        //        ilProcessor.Emit(instruction);
        //        return;
        //    }
        //
        //    if (node.Type == TypeSymbol.String)
        //    {
        //        string? value = (string)node.ConstantValue.Value;
        //        ilProcessor.Emit(OpCodes.Ldstr, value);
        //        return;
        //    }
        //
        //    throw new Exception($"Unexpected constant expression type: {node.Type}");
        //}
        //
        //private void EmitUnaryExpression(ILProcessor ilProcessor, AbstractUnaryExpression node)
        //{
        //    EmitExpression(ilProcessor, node.Operand);
        //    switch (node.Operator.UnaryType)
        //    {
        //        case AbstractUnaryType.IDENTITY:
        //            // NOP
        //            break;
        //        case AbstractUnaryType.LOGICAL_NEGATION:
        //            ilProcessor.Emit(OpCodes.Ldc_I4_0);
        //            ilProcessor.Emit(OpCodes.Ceq);
        //            // !
        //            break;
        //        case AbstractUnaryType.NEGATION:
        //            ilProcessor.Emit(OpCodes.Neg);
        //            // -
        //            break;
        //        case AbstractUnaryType.ONES_COMPLEMENT:
        //            ilProcessor.Emit(OpCodes.Not);
        //            // ~
        //            break;
        //        default:
        //            throw new Exception($"Unexpected unary operator: ({SyntaxEx.GetText(node.Operator.SyntaxType)})({node.Operand.Type})");
        //    }
        //}
        //
        //private void EmitBinaryExpression(ILProcessor ilProcessor, AbstractBinaryExpression node)
        //{
        //    // +(string, string)
        //    if (node.Operator.BinaryType == AbstractBinaryType.ADDITION)
        //    {
        //        if (node.Left.Type == TypeSymbol.String && node.Right.Type == TypeSymbol.String)
        //        {
        //            //ilProcessor.Emit(OpCodes.Call, StringConcat2Reference);
        //            EmitStringConcatExpression(ilProcessor, node);
        //            return;
        //        }
        //    }
        //
        //    EmitExpression(ilProcessor, node.Left);
        //    EmitExpression(ilProcessor, node.Right);
        //
        //    // ==(string, string)
        //    // == (any, any)
        //    if (node.Operator.BinaryType == AbstractBinaryType.EQUALS)
        //    {
        //        if (node.Left.Type == TypeSymbol.String && node.Right.Type == TypeSymbol.String ||
        //            node.Left.Type == TypeSymbol.Any && node.Right.Type == TypeSymbol.Any)
        //        {
        //            ilProcessor.Emit(OpCodes.Call, ObjectEqualsReference);
        //            return;
        //        }
        //    }
        //
        //    // !=(string, string)
        //    // !=(any, any)
        //    if (node.Operator.BinaryType == AbstractBinaryType.NOT_EQUALS)
        //    {
        //        if (node.Left.Type == TypeSymbol.String && node.Right.Type == TypeSymbol.String ||
        //            node.Left.Type == TypeSymbol.Any && node.Right.Type == TypeSymbol.Any)
        //        {
        //            ilProcessor.Emit(OpCodes.Call, ObjectEqualsReference);
        //            ilProcessor.Emit(OpCodes.Ldc_I4_0);
        //            ilProcessor.Emit(OpCodes.Ceq);
        //            return;
        //        }
        //    }
        //
        //    switch (node.Operator.BinaryType)
        //    {
        //        case AbstractBinaryType.ADDITION:
        //            ilProcessor.Emit(OpCodes.Add);
        //            break;
        //        case AbstractBinaryType.SUBTRACTION:
        //            ilProcessor.Emit(OpCodes.Sub);
        //            break;
        //        case AbstractBinaryType.MULTIPLICATION:
        //            ilProcessor.Emit(OpCodes.Mul);
        //            break;
        //        case AbstractBinaryType.DIVISION:
        //            ilProcessor.Emit(OpCodes.Div);
        //            break;
        //        case AbstractBinaryType.LOGICAL_AND:
        //        case AbstractBinaryType.BITWISE_AND:
        //            ilProcessor.Emit(OpCodes.And);
        //            break;
        //        case AbstractBinaryType.BITWISE_OR:
        //        case AbstractBinaryType.LOGICAL_OR:
        //            ilProcessor.Emit(OpCodes.Or);
        //            break;
        //        case AbstractBinaryType.BITWISE_XOR:
        //            ilProcessor.Emit(OpCodes.Xor);
        //            break;
        //        case AbstractBinaryType.EQUALS:
        //            ilProcessor.Emit(OpCodes.Ceq);
        //            break;
        //        case AbstractBinaryType.NOT_EQUALS:
        //            ilProcessor.Emit(OpCodes.Ceq);
        //            ilProcessor.Emit(OpCodes.Ldc_I4_0);
        //            ilProcessor.Emit(OpCodes.Ceq);
        //            break;
        //        case AbstractBinaryType.LESS_THAN:
        //            ilProcessor.Emit(OpCodes.Clt);
        //            break;
        //        case AbstractBinaryType.LESS_THAN_OR_EQUAL:
        //            ilProcessor.Emit(OpCodes.Cgt);
        //            ilProcessor.Emit(OpCodes.Ldc_I4_0);
        //            ilProcessor.Emit(OpCodes.Ceq);
        //            break;
        //        case AbstractBinaryType.GREATER_THAN:
        //            ilProcessor.Emit(OpCodes.Cgt);
        //            break;
        //        case AbstractBinaryType.GREATER_THAN_OR_EQUAL:
        //            ilProcessor.Emit(OpCodes.Clt);
        //            ilProcessor.Emit(OpCodes.Ldc_I4_0);
        //            ilProcessor.Emit(OpCodes.Ceq);
        //            break;
        //        default:
        //            throw new Exception($"Unexpected binary operator {SyntaxEx.GetText(node.Operator.SyntaxType)}");
        //    }
        //}
        //
        //private void EmitVariableExpression(ILProcessor ilProcessor, AbstractVariableExpression node)
        //{
        //    if (node.Variable is ParameterSymbol parameter)
        //    {
        //        ilProcessor.Emit(OpCodes.Ldarg, parameter.OrdinalPosition);
        //        return;
        //    }
        //    // ldloc
        //    VariableDefinition? variableDefinition = Locals[node.Variable];
        //    ilProcessor.Emit(OpCodes.Ldloc, variableDefinition);
        //}
        //
        //private void EmitAssignmentExpression(ILProcessor ilProcessor, AbstractAssignmentExpression node)
        //{
        //    VariableDefinition? variableDefinition = Locals[node.Variable];
        //    EmitExpression(ilProcessor, node.Expression);
        //    ilProcessor.Emit(OpCodes.Dup); // Takes current value on stack and pushes it again
        //    ilProcessor.Emit(OpCodes.Stloc, variableDefinition); // Writes value into local
        //}
        //
        //private void EmitCallExpression(ILProcessor ilProcessor, AbstractCallExpression node)
        //{
        //    if (node.Function == BuiltinFunction.Random)
        //    {
        //        if (RandomFieldDefinition is null)
        //        {
        //            EmitRandomField();
        //        }
        //
        //        ilProcessor.Emit(OpCodes.Ldsfld, RandomFieldDefinition);
        //        foreach (AbstractExpression? argument in node.Arguments)
        //            EmitExpression(ilProcessor, argument);
        //        ilProcessor.Emit(OpCodes.Callvirt, RandomNextReference);
        //        return;
        //    }
        //
        //    foreach (AbstractExpression? argument in node.Arguments)
        //        EmitExpression(ilProcessor, argument);
        //
        //    if (node.Function == BuiltinFunction.Print)
        //    {
        //        ilProcessor.Emit(OpCodes.Call, ConsoleWriteLineReference);
        //    }
        //    else if (node.Function == BuiltinFunction.Input)
        //    {
        //        ilProcessor.Emit(OpCodes.Call, ConsoleReadLineReference);
        //    }
        //    else
        //    {
        //        MethodDefinition methodDefinition = Methods[node.Function];
        //        ilProcessor.Emit(OpCodes.Call, methodDefinition);
        //    }
        //}
        //
        //private void EmitConversionExpression(ILProcessor ilProcessor, AbstractConversionExpression node)
        //{
        //    EmitExpression(ilProcessor, node.Expression);
        //    // TODO: Could expose directly in the TypeSymbol to simplify this
        //    bool needsBoxing = node.Expression.Type == TypeSymbol.Bool || node.Expression.Type == TypeSymbol.Int;
        //
        //    if (needsBoxing)
        //    {
        //        TypeReference? type = KnownTypes[node.Expression.Type];
        //        ilProcessor.Emit(OpCodes.Box, type);
        //    }
        //
        //    if (node.Type == TypeSymbol.Any)
        //        return;
        //
        //    if (node.Type == TypeSymbol.Bool)
        //    {
        //        ilProcessor.Emit(OpCodes.Call, ConvertToBooleanReference);
        //        return;
        //    }
        //
        //    if (node.Type == TypeSymbol.Int)
        //    {
        //        ilProcessor.Emit(OpCodes.Call, ConvertToInt32Reference);
        //        return;
        //    }
        //
        //    if (node.Type == TypeSymbol.String)
        //    {
        //        ilProcessor.Emit(OpCodes.Call, ConvertToStringReference);
        //        return;
        //    }
        //
        //    throw new Exception($"Unexpected conversion from {node.Expression.Type} to {node.Type}");
        //}
        //
        //private void EmitRandomField()
        //{
        //    RandomFieldDefinition = new("$rnd", FieldAttributes.Private | FieldAttributes.Static, RandomReference);
        //    TypeDefinition.Fields.Add(RandomFieldDefinition);
        //    // TODO: Emit .cctor to init field
        //    MethodDefinition staticConstructior = new(
        //        ".cctor",
        //        MethodAttributes.Static |
        //        MethodAttributes.Private |
        //        MethodAttributes.RTSpecialName |
        //        MethodAttributes.SpecialName,
        //        KnownTypes[TypeSymbol.Void]
        //        );
        //    TypeDefinition.Methods.Insert(0, staticConstructior);
        //
        //    ILProcessor? ilProcessor = staticConstructior.Body.GetILProcessor();
        //    ilProcessor.Emit(OpCodes.Newobj, RandomCtorReference);
        //    ilProcessor.Emit(OpCodes.Stsfld, RandomFieldDefinition);
        //    ilProcessor.Emit(OpCodes.Ret);
        //}
        //
        //private void EmitStringConcatExpression(ILProcessor ilProcessor, AbstractBinaryExpression node)
        //{
        //    // Flatten the expression tree to a sequence of nodes to concatenate, then fold consecutive constants in that sequence.
        //    // This approach enables constant folding of non-sibling nodes, which cannot be done in the ConstantFolding class as it would require changing the tree.
        //    // Example: folding b and c in ((a + b) + c) if they are constant.
        //
        //    List<AbstractExpression>? nodes = FoldConstants(node.Syntax, Flatten(node)).ToList();
        //
        //    switch (nodes.Count)
        //    {
        //        case 0:
        //            ilProcessor.Emit(OpCodes.Ldstr, string.Empty);
        //            break;
        //
        //        case 1:
        //            EmitExpression(ilProcessor, nodes[0]);
        //            break;
        //
        //        case 2:
        //            EmitExpression(ilProcessor, nodes[0]);
        //            EmitExpression(ilProcessor, nodes[1]);
        //            ilProcessor.Emit(OpCodes.Call, StringConcat2Reference);
        //            break;
        //
        //        case 3:
        //            EmitExpression(ilProcessor, nodes[0]);
        //            EmitExpression(ilProcessor, nodes[1]);
        //            EmitExpression(ilProcessor, nodes[2]);
        //            ilProcessor.Emit(OpCodes.Call, StringConcat3Reference);
        //            break;
        //
        //        case 4:
        //            EmitExpression(ilProcessor, nodes[0]);
        //            EmitExpression(ilProcessor, nodes[1]);
        //            EmitExpression(ilProcessor, nodes[2]);
        //            EmitExpression(ilProcessor, nodes[3]);
        //            ilProcessor.Emit(OpCodes.Call, StringConcat4Reference);
        //            break;
        //
        //        default:
        //            ilProcessor.Emit(OpCodes.Ldc_I4, nodes.Count);
        //            ilProcessor.Emit(OpCodes.Newarr, KnownTypes[TypeSymbol.String]);
        //
        //            for (int i = 0; i < nodes.Count; i++)
        //            {
        //                ilProcessor.Emit(OpCodes.Dup);
        //                ilProcessor.Emit(OpCodes.Ldc_I4, i);
        //                EmitExpression(ilProcessor, nodes[i]);
        //                ilProcessor.Emit(OpCodes.Stelem_Ref);
        //            }
        //
        //            ilProcessor.Emit(OpCodes.Call, StringConcatArrayReference);
        //            break;
        //    }
        //
        //    // (a + b) + (c + d) --> [a, b, c, d]
        //    static IEnumerable<AbstractExpression> Flatten(AbstractExpression node)
        //    {
        //        if (node is AbstractBinaryExpression binaryExpression &&
        //            binaryExpression.Operator.BinaryType == AbstractBinaryType.ADDITION &&
        //            binaryExpression.Left.Type == TypeSymbol.String &&
        //            binaryExpression.Right.Type == TypeSymbol.String)
        //        {
        //            foreach (AbstractExpression? result in Flatten(binaryExpression.Left))
        //                yield return result;
        //
        //            foreach (AbstractExpression? result in Flatten(binaryExpression.Right))
        //                yield return result;
        //        }
        //        else
        //        {
        //            if (node.Type != TypeSymbol.String)
        //                throw new Exception($"Unexpected node type in string concatenation: {node.Type}");
        //
        //            yield return node;
        //        }
        //    }
        //
        //    // [a, "foo", "bar", b, ""] --> [a, "foobar", b]
        //    static IEnumerable<AbstractExpression> FoldConstants(Node syntax, IEnumerable<AbstractExpression> nodes)
        //    {
        //        StringBuilder? sb = null;
        //
        //        foreach (AbstractExpression? node in nodes)
        //        {
        //            if (node.ConstantValue != null)
        //            {
        //                string? stringValue = (string)node.ConstantValue.Value;
        //
        //                if (string.IsNullOrEmpty(stringValue))
        //                    continue;
        //
        //                sb ??= new StringBuilder();
        //                sb.Append(stringValue);
        //            }
        //            else
        //            {
        //                if (sb?.Length > 0)
        //                {
        //                    yield return new AbstractLiteralExpression(syntax, sb.ToString());
        //                    sb.Clear();
        //                }
        //
        //                yield return node;
        //            }
        //        }
        //
        //        if (sb?.Length > 0)
        //            yield return new AbstractLiteralExpression(syntax, sb.ToString());
        //    }
        //}
    }
}

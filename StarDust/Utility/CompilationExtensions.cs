using StarDust.Code;
using StarDust.Code.AST;
using StarDust.Code.AST.ControlFlow;
using StarDust.Code.AST.Statements;
using StarDust.Code.Evaluation;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarDust.Code.Extensions
{
    public static class CompilationExtensions
    {
        public static ImmutableArray<Report> WriteControlFlowGraph(this Compilation comp, string outputPath)
        {
            string cfgPath = Path.Combine(outputPath, "cfg.dot");
            var program = comp.GetProgram();
            if (program.Report.HasErrors())
                return program.Report;

            var startFunction = program.MainFunction ?? program.ScriptFunction;
            AbstractBlockStatement? cfgStatement = program.Functions[startFunction!];
            ControlFlowGraph cfg = ControlFlowGraph.Create(cfgStatement);
            using (StreamWriter streamWriter = new(cfgPath))
            {
                cfg.WriteTo(streamWriter);
            }

            return ImmutableArray<Report>.Empty;
        }

        public static EvaluationResult Evaluate(this Compilation comp, Dictionary<VariableSymbol, object> variables)
        {
            if (comp.GlobalScope.Report.Any())
                return new EvaluationResult(comp.GlobalScope.Report, null);

            AbstractProgram program = comp.GetProgram();
            if (program.Report.HasErrors())
                return new EvaluationResult(program.Report, null);

            Evaluator evaluator = new(program, variables);
            object? value = evaluator.Evaluate();
            return new EvaluationResult(program.Report, value);
        }

        // References should be part of the compilation not an argument
        public static ImmutableArray<Report> Emit(this Compilation comp, string moduleName, string[] references, string outputPath)
        {
            // Return parse errors before emitting
            IEnumerable<Report>? parseDiagnostics = comp.SyntaxTrees.SelectMany(st => st.Report);
            ImmutableArray<Report> report = parseDiagnostics.Concat(comp.GlobalScope.Report).ToImmutableArray();

            if (report.HasErrors())
                return report;

            AbstractProgram? program = comp.GetProgram();
            return ImmutableArray<Report>.Empty;
            //return Emitter.Emit(program, moduleName, references, outputPath);
        }
    }
}

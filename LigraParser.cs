using System;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Giza;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Ligra.Commands;
using MetaphysicsIndustries.Solus.Commands;
using MetaphysicsIndustries.Solus.Functions;
using MetaphysicsIndustries.Solus.Values;
using DeleteCommand = MetaphysicsIndustries.Ligra.Commands.DeleteCommand;
using FuncAssignCommandData = MetaphysicsIndustries.Ligra.Commands.FuncAssignCommandData;
using HelpCommandData = MetaphysicsIndustries.Ligra.Commands.HelpCommandData;
using VarAssignCommandData = MetaphysicsIndustries.Ligra.Commands.VarAssignCommandData;
using VarsCommand = MetaphysicsIndustries.Ligra.Commands.VarsCommand;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraParser : SolusParser
    {
        protected new LigraGrammar _grammar;
        protected new Parser _parser;

        public LigraParser()
            : base(new LigraGrammar())
        {
            this._grammar = (LigraGrammar)base._grammar;
            _parser = new Parser(_grammar.def_commands);
        }

        public new ICommandData[] GetCommands(string input,
            SolusEnvironment env = null, CommandSet commandSet = null)
        {
            if (env == null)
            {
                env = new SolusEnvironment();
            }

            var errors1 = new List<Error>();

            var spans = _parser.Parse(input.ToCharacterSource(), errors1);

            if (errors1.ContainsNonWarnings())
            {
                Solus.Expressions.Expression expr;

                try
                {
                    expr = base.GetExpression(input, env);
                }
                catch (Exception ex)
                {
                    // return errors1
                    throw new InvalidOperationException("Error getting the expression", ex);
                }

                return new[] {new ExprCommandData(expr)};
            }
            if (spans.Length < 1)
            {
                throw new InvalidOperationException();
            }
            if (spans.Length > 1)
            {
                throw new InvalidOperationException();
            }

            var span = spans[0];

            return GetCommandsFromCommands(span, env);
        }

        ICommandData[] GetCommandsFromCommands(Span span, SolusEnvironment env)
        {
            var commands = new List<ICommandData>();

            foreach (var sub in span.Subspans)
            {
                if (sub.DefRef != _grammar.def_command) continue;

                commands.Add(GetCommandFromCommand(sub, env));
            }

            return commands.ToArray();
        }

        public ICommandData GetSingleCommand(string input, LigraEnvironment env)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));
            var errors1 = new List<Error>();
            var spans = _parser.Parse(input.ToCharacterSource(), errors1);
            if (errors1.ContainsNonWarnings())
            {
                Solus.Expressions.Expression expr;
                try
                {
                    expr = base.GetExpression(input, env);
                }
                catch (Exception)
                {
                    // return errors1
                    throw new InvalidOperationException();
                }

                return new ExprCommandData(expr);
            }
            if (spans.Length < 1) throw new InvalidOperationException();
            if (spans.Length > 1) throw new InvalidOperationException();

            var span = spans[0];

            return GetCommandFromCommand(span, env);
        }

        ICommandData GetCommandFromCommand(Span span, SolusEnvironment env)
        {
            var sub = span.Subspans[0];
            var def = sub.DefRef;

            if (def == _grammar.def_help_002D_command)
            {
                return GetHelpCommandFromHelpCommand(sub, env);
            }
            else if (def == _grammar.def_clear_002D_command)
            {
                return GetClearCommandFromClearCommand(sub, env);
            }
            else if (def == _grammar.def_show_002D_command)
            {
                return GetShowCommandFromShowCommand(sub, env);
            }
            else if (def == _grammar.def_plot_002D_command)
            {
                return GetPlotCommandFromPlotCommand(sub, env);
            }
            else if (def == _grammar.def_paint_002D_command)
            {
                return GetPaintCommandFromPaintCommand(sub, env);
            }
            else if (def == _grammar.def_delete_002D_command)
            {
                return new SimpleCommandData(DeleteCommand.Value);
            }
            else if (def == _grammar.def_var_002D_assign_002D_command)
            {
                return GetVarAssignCommandFromVarAssignCommand(sub, env);
            }
            else if (def == _grammar.def_func_002D_assign_002D_command)
            {
                return GetFuncAssignCommandFromFuncAssignCommand(sub, env);
            }
            else if (def == _grammar.def_vars_002D_command)
            {
                return new SimpleCommandData(VarsCommand.Value);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        HelpCommandData GetHelpCommandFromHelpCommand(Span span,
            SolusEnvironment env)
        {
            string topic = "help";
            if (span.Subspans.Count >= 2)
            {
                topic = span.Subspans[1].Value;
            }

            return new HelpCommandData(topic);
        }

        ICommandData GetClearCommandFromClearCommand(Span span,
            SolusEnvironment env)
        {
            return new SimpleCommandData(ClearCommand.Value);
        }

        ICommandData GetShowCommandFromShowCommand(Span span,
            SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        public PlotCommandData GetPlotCommand(string input,
            LigraEnvironment env)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));
            var errors1 = new List<Error>();
            var spans = _parser.Parse(input.ToCharacterSource(), errors1);
            if (errors1.ContainsNonWarnings())
                throw new InvalidOperationException();
            Span span;
            if (spans.Length < 1) throw new InvalidOperationException();
            if (spans.Length > 1) throw new InvalidOperationException();
            span = spans[0];
            if (span.Subspans.Count < 1) throw new InvalidOperationException();
            if (span.Subspans.Count > 1) throw new InvalidOperationException();
            span = span.Subspans[0];
            if (span.Subspans.Count < 1) throw new InvalidOperationException();
            if (span.Subspans.Count > 1) throw new InvalidOperationException();
            span = span.Subspans[0];
            return GetPlotCommandFromPlotCommand(span, env);
        }

        PlotCommandData GetPlotCommandFromPlotCommand(Span span,
            SolusEnvironment env)
        {
            var exprs = new List<Solus.Expressions.Expression>();
            var intervals = new List<VarInterval>();

            foreach (var sub in span.Subspans)
            {
                if (sub.DefRef == _grammar.def_expr)
                {
                    exprs.Add(GetExpressionFromExpr(sub, env));
                }
                else if (sub.DefRef == _grammar.def_interval)
                {
                    intervals.Add(GetVarIntervalFromInterval(sub, env));
                }
            }

            return new PlotCommandData(exprs.ToArray(), intervals.ToArray());
        }

        public VarInterval GetVarIntervalFromInterval(Span span,
            SolusEnvironment env)
        {
            if (span.Subspans[0].Node == _grammar.node_interval_0_varref)
            {
                // var = [lower .. upper]
                string varname = span.Subspans[0].Subspans[0].Value;

                var lower = GetExpressionFromExpr(span.Subspans[3], env);
                var upper = GetExpressionFromExpr(span.Subspans[5], env);
                var lowerf = (float)Math.Round(
                    lower.Eval(env).ToNumber().Value);
                var upperf = (float)Math.Round(
                    upper.Eval(env).ToNumber().Value);

                return new VarInterval {
                    Variable = varname,
                    Interval = new Interval {
                        LowerBound = lowerf,
                        OpenLowerBound = false,
                        UpperBound = upperf,
                        OpenUpperBound = false,
                        IsIntegerInterval = true,
                    }
                };

            }
            else
            {
                // lower <= var <= upper

                var lower = GetExpressionFromExpr(span.Subspans[0], env);
                var lowerf = (float)Math.Round(
                    lower.Eval(env).ToNumber().Value);

                var openLower = (span.Subspans[1].Value == "<");

                string varname = span.Subspans[2].Subspans[0].Value;

                var openUpper = (span.Subspans[3].Value == "<");

                var upper = GetExpressionFromExpr(span.Subspans[4], env);
                var upperf = (float)Math.Round(
                    upper.Eval(env).ToNumber().Value);

                return new VarInterval {
                    Variable = varname,
                    Interval = new Interval {
                        LowerBound = lowerf,
                        OpenLowerBound = openLower,
                        UpperBound = upperf,
                        OpenUpperBound = openUpper,
                        IsIntegerInterval = false,
                    }
                };

            }
        }

        public PaintCommandData GetPaintCommand(string input,
            LigraEnvironment env)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));
            var errors1 = new List<Error>();
            var spans = _parser.Parse(input.ToCharacterSource(), errors1);
            if (errors1.ContainsNonWarnings())
                throw new InvalidOperationException();
            Span span;
            if (spans.Length < 1) throw new InvalidOperationException();
            if (spans.Length > 1) throw new InvalidOperationException();
            span = spans[0];
            if (span.Subspans.Count < 1) throw new InvalidOperationException();
            if (span.Subspans.Count > 1) throw new InvalidOperationException();
            span = span.Subspans[0];
            if (span.Subspans.Count < 1) throw new InvalidOperationException();
            if (span.Subspans.Count > 1) throw new InvalidOperationException();
            span = span.Subspans[0];
            return GetPaintCommandFromPaintCommand(span, env);
        }

        PaintCommandData GetPaintCommandFromPaintCommand(Span span,
            SolusEnvironment env)
        {
            var expr = GetExpressionFromExpr(span.Subspans[1], env);
            var interval1 = GetVarIntervalFromInterval(span.Subspans[3], env);
            var interval2 = GetVarIntervalFromInterval(span.Subspans[5], env);

            return new PaintCommandData(expr, interval1, interval2);
        }

        ICommandData GetDelCommandFromDelCommand(Span span,
            SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        VarAssignCommandData GetVarAssignCommandFromVarAssignCommand(Span span,
            SolusEnvironment env)
        {
            var varname = span.Subspans[0].Subspans[0].Value;
            var expr = GetExpressionFromExpr(span.Subspans[2], env);
            expr = expr.PreliminaryEval(env);

            return new VarAssignCommandData(varname, expr);
        }

        FuncAssignCommandData GetFuncAssignCommandFromFuncAssignCommand(
            Span span, SolusEnvironment env)
        {
            var funcname = span.Subspans[0].Value;

            var args = new List<string>();

            foreach (var sub in span.Subspans.Skip(1))
            {
                if (sub.DefRef == _grammar.def_identifier)
                {
                    args.Add(sub.Value);
                }
            }

            SolusEnvironment env2 = env.Clone();

            // create the function, with no expr
            var func = new UserDefinedFunction(funcname, args.ToArray(), null);
            if (env2.ContainsFunction(funcname))
                env2.RemoveFunction(funcname);
            env2.AddFunction(func);

            // read the expr. this order of things allows for recursion
            var expr = GetExpressionFromExpr(span.Subspans.Last(), env2);
            func.Expression = expr;

            return new FuncAssignCommandData(func);
        }
    }
}


using System;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Giza;
using System.Collections.Generic;
using System.Linq;

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

        public Command[] GetCommands(string input, SolusEnvironment env=null)
        {
            if (env == null)
            {
                env = new SolusEnvironment();
            }

            var errors1 = new List<Error>();

            var spans = _parser.Parse(input.ToCharacterSource(), errors1);

            if (errors1.ContainsNonWarnings())
            {
                Solus.Expression expr;

                try
                {
                    expr = base.GetExpression(input, env);
                }
                catch (Exception ignored)
                {
                    // return errors1
                    throw new InvalidOperationException();
                }

                return new Command[] { (input_, args, env_) => Commands.ExprCommand(input_, args, env_, expr) };
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

        Command[] GetCommandsFromCommands(Span span, SolusEnvironment env)
        {
            var commands = new List<Command>();

            foreach (var sub in span.Subspans)
            {
                if (sub.DefRef != _grammar.def_command) continue;

                commands.Add(GetCommandFromCommand(sub, env));
            }

            return commands.ToArray();
        }

        Command GetCommandFromCommand(Span span, SolusEnvironment env)
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
            else if (def == _grammar.def_del_002D_command)
            {
                return GetDelCommandFromDelCommand(sub, env);
            }
            else if (def == _grammar.def_var_002D_assign_002D_command)
            {
                return GetVarAssignCommandFromVarAssignCommand(sub, env);
            }
            else if (def == _grammar.def_func_002D_assign_002D_command)
            {
                return GetFuncAssignCommandFromFuncAssignCommand(sub, env);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        Command GetHelpCommandFromHelpCommand(Span span, SolusEnvironment env)
        {
            var topic = span.Subspans[1].Subspans[0].Value;

            return (input, args, env_) => Commands.HelpCommand(input, args, env_, topic);
        }

        Command GetClearCommandFromClearCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        Command GetShowCommandFromShowCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        Command GetPlotCommandFromPlotCommand(Span span, SolusEnvironment env)
        {
            var exprs = new List<Solus.Expression>();
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

            return (input, args, env_) => Commands.PlotCommand(input, args, env_, exprs.ToArray(), intervals.ToArray());
        }

        public VarInterval GetVarIntervalFromInterval(Span span, SolusEnvironment env)
        {
            if (span.Subspans[0].Node == _grammar.node_interval_0_varref)
            {
                // var = [lower .. upper]
                string varname = span.Subspans[0].Subspans[0].Value;

                var lower = GetExpressionFromExpr(span.Subspans[3], env);
                var upper = GetExpressionFromExpr(span.Subspans[5], env);
                var lowerf = (float)Math.Round(lower.Eval(env).Value);
                var upperf = (float)Math.Round(upper.Eval(env).Value);

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
                var lowerf = (float)Math.Round(lower.Eval(env).Value);

                var openLower = (span.Subspans[1].Value == "<");

                string varname = span.Subspans[2].Subspans[0].Value;

                var openUpper = (span.Subspans[3].Value == "<");

                var upper = GetExpressionFromExpr(span.Subspans[4], env);
                var upperf = (float)Math.Round(upper.Eval(env).Value);

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

        Command GetPaintCommandFromPaintCommand(Span span, SolusEnvironment env)
        {
            var expr = GetExpressionFromExpr(span.Subspans[1], env);
            var interval1 = GetVarIntervalFromInterval(span.Subspans[3], env);
            var interval2 = GetVarIntervalFromInterval(span.Subspans[5], env);

            return (input, args, env_) => Commands.PaintCommand(input, args, env_, expr, interval1, interval2);
        }

        Command GetDelCommandFromDelCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        Command GetVarAssignCommandFromVarAssignCommand(Span span, SolusEnvironment env)
        {
            var varname = span.Subspans[0].Subspans[0].Value;
            var expr = GetExpressionFromExpr(span.Subspans[2], env);
            expr = expr.PreliminaryEval(env);

            return (input, args, env_) => Commands.VarAssignCommand(input, args, env_, varname, expr);
        }

        Command GetFuncAssignCommandFromFuncAssignCommand(Span span, SolusEnvironment env)
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

            SolusEnvironment env2 = env.CreateChildEnvironment();

            // create the functino, with no expr
            var func = new UserDefinedFunction(funcname, args.ToArray(), null);
            if (env2.Functions.ContainsKey(funcname))
            {
                env2.Functions.Remove(funcname);
            }
            env2.AddFunction(func);

            // read the expr. this order of things allows for recursion
            var expr = GetExpressionFromExpr(span.Subspans.Last(), env2);
            func.Expression = expr;

            return (input, args_, env_) => Commands.FuncAssignCommand(input, args_, env_, func);
        }
    }
}


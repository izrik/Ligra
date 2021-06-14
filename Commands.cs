using System;
using MetaphysicsIndustries.Solus;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace MetaphysicsIndustries.Ligra
{
    public static class Commands
    {
        public static void DeleteCommand(string input, string[] args, LigraEnvironment env)
        {
            if (args.Length > 1)
            {
                List<string> unknownVars = new List<string>();

                int i;
                for (i = 1; i < args.Length; i++)
                {
                    if (!env.Variables.ContainsKey(args[i]))
                    {
                        unknownVars.Add(args[i]);
                    }
                }

                if (unknownVars.Count > 0)
                {
                    string error = "The following variables do not exist: \r\n";
                    foreach (string s in unknownVars)
                    {
                        error += s + "\r\n";
                    }
                    env.AddRenderItem(new ErrorItem(input, error, env.Font, LBrush.Red, env, input.IndexOf(args[0])));
                }
                else
                {
                    for (i = 1; i < args.Length; i++)
                    {
                        env.Variables.Remove(args[i]);
                    }

                    env.AddRenderItem(new InfoItem("The variables were deleted successfully.", env.Font, env));
                }
            }
            else
            {
                env.AddRenderItem(new ErrorItem(input, "Must specify variables to delete", env.Font, LBrush.Red, env, input.IndexOf(args[0])));
            }
        }

        public static void VarsCommand(string input, string[] args, LigraEnvironment env)
        {
            string s = string.Empty;
            foreach (string var in env.Variables.Keys)
            {
                Expression value = env.Variables[var];
                string valueString = value.ToString();

                if (value is SolusVector)
                {
                    valueString = "Vector (" + ((SolusVector)value).Length.ToString() + ")";
                }
                else if (value is SolusMatrix)
                {
                    SolusMatrix mat = (SolusMatrix)value;
                    valueString = "Matrix (" + mat.RowCount + ", " + mat.ColumnCount + ")";
                }

                s += var + " = " + valueString + "\r\n";
            }

            env.AddRenderItem(new InfoItem(s, env.Font, env));
        }

        public static void ClearCommand(string input, string[] args, LigraEnvironment env)
        {
            if (args.Length > 1)
            {
                if (args[1].ToLower() == "history")
                {
                    ClearHistory(env);
                }
                else if (args[1].ToLower() == "all")
                {
                    ClearHistory(env);
                    ClearOutput(env);
                }
                else
                {
                    ClearOutput(env);
                }
            }
            else
            {
                ClearOutput(env);
            }
        }

        public static void HelpCommand(string input, string[] args, LigraEnvironment env, string topic)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                env.AddRenderItem(new HelpItem(env.Font, env, topic));
            }
            else if (args.Length > 1)
            {
                env.AddRenderItem(new HelpItem(env.Font, env, args[1]));
            }
            else
            {
                env.AddRenderItem(new HelpItem(env.Font, env));
            }
        }

        public static void HistoryCommand(string input, string[] args, LigraEnvironment env)
        {
            if (args.Length > 1 && args[1].ToLower() == "clear")
            {
                ClearHistory(env);
            }
            else
            {
                string s = string.Join("\r\n", env.History.ToArray());
                env.AddRenderItem(new InfoItem(s + "\r\n", env.Font, env));
            }
        }

        public static void ExampleCommand(string input, string[] args, LigraEnvironment env)
        {
            var f = env.Font;
            var p = LPen.Blue;

            if (!env.Variables.ContainsKey("x")) env.Variables.Add("x", new Literal(0));
            if (!env.Variables.ContainsKey("y")) env.Variables.Add("y", new Literal(0));
            if (!env.Variables.ContainsKey("mu")) env.Variables.Add("mu", new Literal(0));
            if (!env.Variables.ContainsKey("sigma")) env.Variables.Add("sigma", new Literal(0));

            Expression expr;

            env.AddRenderItem(new InfoItem("A number:", f, env));
            env.AddRenderItem(new ExpressionItem(new Literal(123.45f), p, f, env));

            env.AddRenderItem(new InfoItem("A variable:", f, env));
            env.AddRenderItem(new ExpressionItem(new VariableAccess("x"), p, f, env));

            env.AddRenderItem(new InfoItem("A function call: ", f, env));
            env.AddRenderItem(new ExpressionItem(
                new FunctionCall(
                CosineFunction.Value,
                    new VariableAccess("x")), p, f, env));

            env.AddRenderItem(new InfoItem("A simple expression,  \"x + y/2\" :", f, env));
            env.AddRenderItem(new ExpressionItem(
                                new FunctionCall(
                                    AdditionOperation.Value,
                                    new VariableAccess("x"),
                                        new FunctionCall(
                                            DivisionOperation.Value,
                                                new VariableAccess("y"),
                                                    new Literal(2))), p, f, env));

            env.AddRenderItem(new InfoItem("Some derivatives, starting with x^3:", f, env));
            var parser = new SolusParser();
            expr = parser.GetExpression("x^3", env);
            env.AddRenderItem(new ExpressionItem(expr, p, f, env));
            DerivativeTransformer derive = new DerivativeTransformer();
            expr = derive.Transform(expr, new VariableTransformArgs("x"));
            env.AddRenderItem(new ExpressionItem(expr, p, f, env));
            expr = derive.Transform(expr, new VariableTransformArgs("x"));
            env.AddRenderItem(new ExpressionItem(expr, p, f, env));
            expr = derive.Transform(expr, new VariableTransformArgs("x"));
            env.AddRenderItem(new ExpressionItem(expr, p, f, env));

            env.AddRenderItem(new InfoItem("Some variable assignments: ", f, env));
            env.AddRenderItem(new ExpressionItem(
                new FunctionCall(
                    AssignOperation.Value,
                    new VariableAccess("mu"),
                    new Literal(0.5f)),
                p,
                f, env));
            env.AddRenderItem(new ExpressionItem(
                new FunctionCall(
                    AssignOperation.Value,
                    new VariableAccess("sigma"),
                    new Literal(0.2f)),
                p,
                f, env));

            env.Variables["mu"] = new Literal(0.5f);
            env.Variables["sigma"] = new Literal(0.2f);

            expr =
                new FunctionCall(
                    MultiplicationOperation.Value,
                    new FunctionCall(
                        DivisionOperation.Value,
                        new Literal(1),
                        new FunctionCall(
                            MultiplicationOperation.Value,
                            new FunctionCall(
                                ExponentOperation.Value,
                                new FunctionCall(
                                    MultiplicationOperation.Value,
                                    new Literal(2),
                                    new Literal((float)Math.PI)),
                                new Literal(0.5f)),
                        new VariableAccess("sigma"))),
                        new FunctionCall(
                            ExponentOperation.Value,
                            new Literal((float)Math.E),
                            new FunctionCall(
                                DivisionOperation.Value,
                                new FunctionCall(
                                    ExponentOperation.Value,
                                    new FunctionCall(
                                        AdditionOperation.Value,
                                        new VariableAccess("x"),
                                        new FunctionCall(
                                            MultiplicationOperation.Value,
                                            new Literal(-1),
                                            new VariableAccess("mu"))),
                                    new Literal(2)),
                                    new FunctionCall(
                                        MultiplicationOperation.Value,
                                        new Literal(-2),
                                        new FunctionCall(
                                            ExponentOperation.Value,
                                            new VariableAccess("sigma"),
                                            new Literal(2))))));

            env.AddRenderItem(new InfoItem("A complex expression, \"(1/(sigma*sqrt(2*pi))) * e ^ ( (x - mu)^2 / (-2 * sigma^2))\"", f, env));
            env.AddRenderItem(new ExpressionItem(expr, p, f, env));
            //(1/(sigma*sqrt(2*pi))) * e ^ ( (x - mu)^2 / (-2 * sigma^2))

            env.AddRenderItem(new InfoItem("A plot of the expression: ", f, env));
            env.AddRenderItem(new GraphItem(expr, p, "x", parser, env));

            env.AddRenderItem(new InfoItem("Multiple plots on the same axes, \"x^3\", \"3 * x^2\", \"6 * x\":", f, env));
            env.AddRenderItem(new GraphItem(
                parser, env,
                new GraphEntry(parser.GetExpression("x^3", env), LPen.Blue, "x"),
                new GraphEntry(parser.GetExpression("3*x^2", env), LPen.Green, "x"),
                new GraphEntry(parser.GetExpression("6*x", env), LPen.Red, "x")));

            env.AddRenderItem(new InfoItem("A plot that changes with time, \"sin(x+t)\":", f, env));
            env.AddRenderItem(new GraphItem(parser.GetExpression("sin(x+t)", env), p, "x", parser, env));

            expr = parser.GetExpression("unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)", env);
            env.AddRenderItem(new InfoItem("Another complex expression, \"unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)\",\r\nwhere t is time:", f, env));
            env.AddRenderItem(new ExpressionItem(expr, p, f, env));

            env.AddRenderItem(new InfoItem("A 3d plot: ", f, env));
            env.AddRenderItem(new Graph3dItem(expr, LPen.Black, LBrush.Green, -4, 4, -4, 4, -2, 6, "x", "y", env));
        }

        public static void Example2Command(string input, string[] args, LigraEnvironment env)
        {

            if (!env.Variables.ContainsKey("x")) env.Variables.Add("x", new Literal(0));
            if (!env.Variables.ContainsKey("y")) env.Variables.Add("y", new Literal(0));

            Expression expr;

            var parser = new LigraParser();
//            expr = parser.GetExpression("unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)", env);
//            env.AddRenderItem(new InfoItem("unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)", env.Font, env));
//            env.AddRenderItem(new Graph3dItem(expr, Pens.Black, Brushes.Green, -4, 4, -4, 4, -2, 6, "x", "y", env));

            var input2 = "factorial(n) := if (n, n * factorial(n-1), 1)";
            var input3 = "cos_taylor(x, n, sign) := if (n-8, sign * (x ^ n) / factorial(n) + cos_taylor(x, n+2, -sign), 0)";
            var input4 = "cos2(x) := cos_taylor(x, 0, 1)";
            parser.GetCommands(input2, env)[0](input2, null, env);
            parser.GetCommands(input3, env)[0](input3, null, env);
            parser.GetCommands(input4, env)[0](input4, null, env);

        }

        public static void PlotCommand(string input, string[] args, LigraEnvironment env, Expression[] exprs, VarInterval[] intervals)
        {
            if (env == null) throw new ArgumentNullException("env");
            if (exprs == null || exprs.Length < 1) throw new ArgumentNullException("exprs");
            if (intervals == null || intervals.Length < 1) throw new ArgumentNullException("intervals");

            if (intervals.Length > 2) throw new ArgumentOutOfRangeException("Too many intervals.");

            var literals = new List<Literal>();
            foreach (var interval in intervals)
            {
                float midpoint = (interval.Interval.LowerBound + interval.Interval.UpperBound) / 2;
                var literal = new Literal(midpoint);
                env.Variables[interval.Variable] = literal;
                literals.Add(literal);
            }

            var unboundVars = new HashSet<string>();
            foreach (var expr in exprs)
            {
                var expr2 = expr.PreliminaryEval(env);
                if (!(expr2 is Literal))
                {
                    unboundVars.UnionWith(SolusEngine.GatherVariables(expr2));
                }
            }

            if (intervals.Length == 1)
            {
                List<GraphEntry> entries = new List<GraphEntry>();

                var pens = new List<LPen>();
                pens.Add(LPen.Blue);
                pens.Add(LPen.Red);
                pens.Add(LPen.Green);
                pens.Add(LPen.Yellow);
                pens.Add(LPen.Cyan);
                pens.Add(LPen.Magenta);

                int i = 0;
                VarInterval interval = intervals.First();
                foreach (Expression entry in exprs)
                {
                    entries.Add(new GraphEntry(entry, pens[i % pens.Count], interval.Variable));
                    i++;
                }

                env.AddRenderItem(new GraphItem(new SolusParser(), env, entries.ToArray()));
            }
            else // intervals.Length == 2
            {
                if (exprs.Length > 1)
                {
                    throw new NotImplementedException("Can't plot more than one 3d surface at a time.");
                }

                var expr = exprs[0];
                var zs = new List<float>();

                literals[0].Value = intervals[0].Interval.LowerBound;
                literals[1].Value = intervals[1].Interval.LowerBound;
                zs.Add(expr.Eval(env).Value);

                literals[0].Value = intervals[0].Interval.LowerBound;
                literals[1].Value = intervals[1].Interval.UpperBound;
                zs.Add(expr.Eval(env).Value);

                literals[0].Value = intervals[0].Interval.UpperBound;
                literals[1].Value = intervals[1].Interval.LowerBound;
                zs.Add(expr.Eval(env).Value);

                literals[0].Value = intervals[0].Interval.UpperBound;
                literals[1].Value = intervals[1].Interval.UpperBound;
                zs.Add(expr.Eval(env).Value);

                float zmin = zs.Min();
                float zmax = zs.Max();

                env.AddRenderItem(new Graph3dItem(expr, LPen.Black, LBrush.Green,
                                                    intervals[0].Interval.LowerBound,
                                                    intervals[0].Interval.UpperBound,
                                                    intervals[1].Interval.LowerBound,
                                                    intervals[1].Interval.UpperBound,
                                                    zmin, zmax,
                                                    intervals[0].Variable,
                    intervals[1].Variable, env));
            }
        }

        public static void PaintCommand(string input, string[] args, LigraEnvironment env, Expression expr, VarInterval interval1, VarInterval interval2)
        {
            env.AddRenderItem(
                new MathPaintItem(
                    expr,
                    interval1,
                    interval2, env));
        }

        public static void VarAssignCommand(string input, string[] args, LigraEnvironment env, string varname, Expression expr)
        {
            env.Variables[varname] = expr;

            var expr2 = new FunctionCall(
                            AssignOperation.Value,
                            new VariableAccess(varname),
                            expr);

            env.AddRenderItem(new ExpressionItem(expr2, LPen.Blue, env.Font, env));
        }

        public static void FuncAssignCommand(string input, string[] args, LigraEnvironment env, UserDefinedFunction func)
        {
//            var func = new UserDefinedFunction(funcname, argnames, expr);
            if (env.Functions.ContainsKey(func.DisplayName))
            {
                env.Functions.Remove(func.DisplayName);
            }
            env.AddFunction(func);

            var varrefs = func.Argnames.Select(x => new VariableAccess(x));
            var fcall = new FunctionCall(func, varrefs);
            var expr2 = new FunctionCall(AssignOperation.Value, fcall, func.Expression);

            env.AddRenderItem(new ExpressionItem(expr2, LPen.Blue, env.Font, env));
        }

        public static void ExprCommand(string input, string[] args, LigraEnvironment env, Expression expr)
        {
            expr = expr.PreliminaryEval(env);

            env.AddRenderItem(new ExpressionItem(expr, LPen.Blue, env.Font, env));
        }

        public static void ClearHistory(LigraEnvironment env)
        {
            env.History.Clear();
            env.CurrentHistoryIndex = -1;
            env.AddRenderItem(new InfoItem("History cleared", env.Font, env));
        }

        public static void ClearOutput(LigraEnvironment env)
        {
            var items = env.RenderItems.ToArray();
            foreach (var item in items)
            {
                env.Control.RemoveRenderItem(item);
            }
            env.ClearCanvas();
        }
    }
}


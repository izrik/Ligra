using System;
using MetaphysicsIndustries.Solus;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MetaphysicsIndustries.Collections;

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
                    env.RenderItems.Add(new ErrorItem(input, error, env.Font, Brushes.Red, input.IndexOf(args[0])));
                }
                else
                {
                    for (i = 1; i < args.Length; i++)
                    {
                        env.Variables.Remove(args[i]);
                    }

                    env.RenderItems.Add(new InfoItem("The variables were deleted successfully.", env.Font));
                }
            }
            else
            {
                env.RenderItems.Add(new ErrorItem(input, "Must specify variables to delete", env.Font, Brushes.Red, input.IndexOf(args[0])));
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

            env.RenderItems.Add(new InfoItem(s, env.Font));
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

        public static void HelpCommand(string input, string[] args, LigraEnvironment env)
        {
            if (args.Length > 1)
            {
                env.RenderItems.Add(new HelpItem(env.Font, args[1]));
            }
            else
            {
                env.RenderItems.Add(new HelpItem(env.Font));
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
                env.RenderItems.Add(new InfoItem(s + "\r\n", env.Font));
            }
        }

        public static void ExampleCommand(string input, string[] args, LigraEnvironment env)
        {
            Font f = env.Font;
            Pen p = Pens.Blue;

            if (!env.Variables.ContainsKey("x")) env.Variables.Add("x", new Literal(0));
            if (!env.Variables.ContainsKey("y")) env.Variables.Add("y", new Literal(0));
            if (!env.Variables.ContainsKey("mu")) env.Variables.Add("mu", new Literal(0));
            if (!env.Variables.ContainsKey("sigma")) env.Variables.Add("sigma", new Literal(0));

            Expression expr;

            env.RenderItems.Add(new InfoItem("A number:", f));
            env.RenderItems.Add(new ExpressionItem(new Literal(123.45f), p, f));

            env.RenderItems.Add(new InfoItem("A variable:", f));
            env.RenderItems.Add(new ExpressionItem(new VariableAccess("x"), p, f));

            env.RenderItems.Add(new InfoItem("A function call: ", f));
            env.RenderItems.Add(new ExpressionItem(
                new FunctionCall(
                CosineFunction.Value,
                new VariableAccess("x")), p, f));

            env.RenderItems.Add(new InfoItem("A simple expression,  \"x + y/2\" :", f));
            env.RenderItems.Add(new ExpressionItem(
                new FunctionCall(
                AdditionOperation.Value,
                new VariableAccess("x"),
                new FunctionCall(
                DivisionOperation.Value,
                new VariableAccess("y"),
                new Literal(2))), p, f));

            env.RenderItems.Add(new InfoItem("Some derivatives, starting with x^3:", f));
            var parser = new SolusParser();
            expr = parser.GetExpression("x^3", env);
            env.RenderItems.Add(new ExpressionItem(expr, p, f));
            DerivativeTransformer derive = new DerivativeTransformer();
            expr = derive.Transform(expr, new VariableTransformArgs("x"));
            env.RenderItems.Add(new ExpressionItem(expr, p, f));
            expr = derive.Transform(expr, new VariableTransformArgs("x"));
            env.RenderItems.Add(new ExpressionItem(expr, p, f));
            expr = derive.Transform(expr, new VariableTransformArgs("x"));
            env.RenderItems.Add(new ExpressionItem(expr, p, f));

            env.RenderItems.Add(new InfoItem("Some variable assignments: ", f));
            env.RenderItems.Add(new ExpressionItem(new AssignExpression("mu", new Literal(0.5f)), p, f));
            env.RenderItems.Add(new ExpressionItem(new AssignExpression("sigma", new Literal(0.2f)), p, f));

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

            env.RenderItems.Add(new InfoItem("A complex expression, \"(1/(sigma*sqrt(2*pi))) * e ^ ( (x - mu)^2 / (-2 * sigma^2))\"", f));
            env.RenderItems.Add(new ExpressionItem(expr, p, f));
            //(1/(sigma*sqrt(2*pi))) * e ^ ( (x - mu)^2 / (-2 * sigma^2))

            env.RenderItems.Add(new InfoItem("A plot of the expression: ", f));
            env.RenderItems.Add(new GraphItem(expr, p, "x", parser));

            env.RenderItems.Add(new InfoItem("Multiple plots on the same axes, \"x^3\", \"3 * x^2\", \"6 * x\":", f));
            env.RenderItems.Add(new GraphItem(
                parser,
                new GraphEntry(parser.GetExpression("x^3", env), Pens.Blue, "x"),
                new GraphEntry(parser.GetExpression("3*x^2", env), Pens.Green, "x"),
                new GraphEntry(parser.GetExpression("6*x", env), Pens.Red, "x")));

            env.RenderItems.Add(new InfoItem("A plot that changes with time, \"sin(x+t)\":", f));
            env.RenderItems.Add(new GraphItem(parser.GetExpression("sin(x+t)", env), p, "x", parser));

            expr = parser.GetExpression("unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)", env);
            env.RenderItems.Add(new InfoItem("Another complex expression, \"unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)\",\r\nwhere t is time:", f));
            env.RenderItems.Add(new ExpressionItem(expr, p, f));

            env.RenderItems.Add(new InfoItem("A 3d plot: ", f));
            env.RenderItems.Add(new Graph3dItem(expr, Pens.Black, Brushes.Green, -4, 4, -4, 4, -2, 6, "x", "y"));
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

            var unboundVars = new Set<string>();
            foreach (var expr in exprs)
            {
                var expr2 = expr.PreliminaryEval(env);
                if (!(expr2 is Literal))
                {
                    unboundVars.AddRange(SolusEngine.GatherVariables(expr2));
                }
            }

            if (intervals.Length == 1)
            {
                List<GraphEntry> entries = new List<GraphEntry>();

                List<Pen> pens = new List<Pen>();
                pens.Add(ColorExpression.Blue.Pen);
                pens.Add(ColorExpression.Red.Pen);
                pens.Add(ColorExpression.Green.Pen);
                pens.Add(ColorExpression.Yellow.Pen);
                pens.Add(ColorExpression.Cyan.Pen);
                pens.Add(ColorExpression.Magenta.Pen);

                int i = 0;
                VarInterval interval = intervals.First();
                foreach (Expression entry in exprs)
                {
                    entries.Add(new GraphEntry(entry, pens[i % pens.Count], interval.Variable));
                    i++;
                }

                env.RenderItems.Add(new GraphItem(new SolusParser(), entries.ToArray()));
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

                env.RenderItems.Add(new Graph3dItem(expr, Pens.Black, Brushes.Green,
                                                    intervals[0].Interval.LowerBound,
                                                    intervals[0].Interval.UpperBound,
                                                    intervals[1].Interval.LowerBound,
                                                    intervals[1].Interval.UpperBound,
                                                    zmin, zmax,
                                                    intervals[0].Variable,
                                                    intervals[1].Variable));
            }
        }

        public static void ExprCommand(string input, string[] args, LigraEnvironment env, Expression expr)
        {
            if (expr != null)
            {
                if (expr is PlotExpression)
                {
                    PlotExpression expr2 = (PlotExpression)expr;

                    List<GraphEntry> entries = new List<GraphEntry>();

                    List<Pen> pens = new List<Pen>();
                    pens.Add(ColorExpression.Blue.Pen);
                    pens.Add(ColorExpression.Red.Pen);
                    pens.Add(ColorExpression.Green.Pen);
                    pens.Add(ColorExpression.Yellow.Pen);
                    pens.Add(ColorExpression.Cyan.Pen);
                    pens.Add(ColorExpression.Magenta.Pen);

                    int i = 0;
                    foreach (Expression entry in expr2.ExpressionsToPlot)
                    {
                        entries.Add(new GraphEntry(entry, pens[i % pens.Count], expr2.Variable));
                        i++;
                    }

                    env.RenderItems.Add(new GraphItem(new SolusParser(), entries.ToArray()));
                }
                else if (expr is Plot3dExpression)
                {
                    Plot3dExpression expr2 = (Plot3dExpression)expr;

                    env.RenderItems.Add(new Graph3dItem(expr2.ExpressionToPlot, expr2.WirePen, expr2.FillBrush,
                                                        expr2.XMin, expr2.XMax,
                                                        expr2.YMin, expr2.YMax,
                                                        expr2.ZMin, expr2.ZMax,
                                                        expr2.IndependentVariableX, expr2.IndependentVariableY));
                }
                else if (expr is MathPaintExpression)
                {
                    MathPaintExpression expr2 = (MathPaintExpression)expr;

                    env.RenderItems.Add(
                        new MathPaintItem(
                        expr2.Expression,
                        expr2.HorizontalCoordinate,
                        expr2.VerticalCoordinate,
                        expr2.Width,
                        expr2.Height));
                }
                //else if (expr is PlotMatrixExpression)
                //{
                //    throw new NotImplementedException();
                //    //PlotMatrixExpression expr2 = (PlotMatrixExpression)expr;
                //    //_renderItems.Add(
                //    //    new GraphMatrixItem(
                //    //        expr2.Matrix,
                //    //        string.Empty));
                //}
                //else if (expr is PlotVectorExpression)
                //{
                //    throw new NotImplementedException();
                //    //PlotVectorExpression expr2 = (PlotVectorExpression)expr;
                //    //_renderItems.Add(new GraphVectorItem(expr2.Vector, string.Empty));
                //}
                else
                {
                    if (expr is AssignExpression)
                    {
                        AssignExpression expr2 = (AssignExpression)expr;

                        env.Variables[expr2.Variable] = (Literal)(expr2.Value.Clone());
                    }
                    else if (expr is DelayAssignExpression)
                    {
                        expr.Eval(env);
                    }

                    env.RenderItems.Add(new ExpressionItem(expr, Pens.Blue, env.Font));
                }
            }
        }

        public static void ClearHistory(LigraEnvironment env)
        {
            env.History.Clear();
            env.CurrentHistoryIndex = -1;
            env.RenderItems.Add(new InfoItem("History cleared", env.Font));
        }

        public static void ClearOutput(LigraEnvironment env)
        {
            env.RenderItems.Clear();
            env.ClearCanvas();
        }
    }
}


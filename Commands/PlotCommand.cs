using System;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class PlotCommand : Command
    {
        public static readonly PlotCommand Value =
            new PlotCommand(null, null);

        public PlotCommand(Expression[] exprs, VarInterval[] intervals)
        {
            _exprs = exprs;
            _intervals = intervals;
        }

        private readonly Expression[] _exprs;
        private readonly VarInterval[] _intervals;

        public override string Name => "plot";

        public override string GetInputLabel(string input,
            LigraEnvironment env, ILigraUI control)
        {
            // TODO: don't create another instance of the class within the class.
            var cmd = control.Parser.GetPlotCommand(input, env);
            var label = string.Format("$ plot {0} for {1}",
                string.Join(", ", cmd._exprs.Select(Expression.ToString)),
                string.Join(", ", cmd._intervals.Select((VarInterval vi) => vi.ToString())));
            return label;
        }

        public override string DocString =>
@"plot - Draw graphs of expressions that vary over one or two variables

Plot one or more expressions that vary over one variable as a 2D graph:
  plot <expr1> [, <exprn> ...] for <interval>

  expr1
    Any expression

  expr2, expr3, exprn, etc.
    Any additional expressions

  interval
    An interval, of the form ""<start> < <var> < <end>"". It defines
    <var> as a variable within all expressions, to be assigned the successive
    values of the interval from <start> to <end>.

  example:
    plot sin(x) for -5 < x < 5

Plot one or more expressions that vary over two variable as a 3D graph:
  plot <expr1> [, <exprn> ...] for <interval1>, <interval2>

  expr1
    Any expression

  expr2, expr3, exprn, etc.
    Any additional expressions

  interval1
    An interval, of the form ""<start> < <var> < <end>"". It defines
    <var> as a variable within all expressions, to be assigned the successive
    values of the interval from <start> to <end>.

  interval2
    An interval, of the form ""<start> < <var> < <end>"". It defines
    <var> as a variable within all expressions, to be assigned the successive
    values of the interval from <start> to <end>.

  example:
    plot sin(x) + cos(y) for -5 < x < 5, -5 < y < 5
";

        public override void Execute(string input, SolusEnvironment env)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ILigraUI control)
        {
            // TODO: don't create another instance of the class within the class.
            var cmd = control.Parser.GetPlotCommand(input, env);
            Execute(input, args, env, control, cmd._exprs, cmd._intervals);
        }

        public static void Execute(string input, string[] args,
            LigraEnvironment env, ILigraUI control, Expression[] exprs,
            VarInterval[] intervals)
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

                control.AddRenderItem(
                    new GraphItem(new SolusParser(), env, entries.ToArray()));
            }
            else // intervals.Length == 2
            {
                if (exprs.Length > 1)
                {
                    throw new NotImplementedException("Can't plot more than one 3d surface at a time.");
                }

                var expr = exprs[0];
                var zs = new List<float>();

                literals[0].Value = intervals[0].Interval.LowerBound.ToNumber();
                literals[1].Value = intervals[1].Interval.LowerBound.ToNumber();
                zs.Add(expr.Eval(env).ToNumber().Value);

                literals[0].Value = intervals[0].Interval.LowerBound.ToNumber();
                literals[1].Value = intervals[1].Interval.UpperBound.ToNumber();
                zs.Add(expr.Eval(env).ToNumber().Value);

                literals[0].Value = intervals[0].Interval.UpperBound.ToNumber();
                literals[1].Value = intervals[1].Interval.LowerBound.ToNumber();
                zs.Add(expr.Eval(env).ToNumber().Value);

                literals[0].Value = intervals[0].Interval.UpperBound.ToNumber();
                literals[1].Value = intervals[1].Interval.UpperBound.ToNumber();
                zs.Add(expr.Eval(env).ToNumber().Value);

                float zmin = zs.Min();
                float zmax = zs.Max();

                control.AddRenderItem(new Graph3dItem(expr, LPen.Black, LBrush.Green,
                    intervals[0].Interval.LowerBound,
                    intervals[0].Interval.UpperBound,
                    intervals[1].Interval.LowerBound,
                    intervals[1].Interval.UpperBound,
                    zmin, zmax,
                    intervals[0].Variable,
                    intervals[1].Variable, env));
            }
        }
    }
}
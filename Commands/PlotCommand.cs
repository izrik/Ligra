using System;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Commands;
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
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            var data2 = (PlotCommandData) data;
            if (data2.Intervals == null || data2.Intervals.Length <= 0)
                return string.Format("$ plot {0}",
                    string.Join(", ",
                        data2.Exprs.Select(Expression.ToString)));
            var label = string.Format("$ plot {0} for {1}",
                string.Join(", ",
                    data2.Exprs.Select(Expression.ToString)),
                string.Join(", ",
                    data2.Intervals.Select(vi => vi.ToString())));
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

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            var data2 = (PlotCommandData) data;
            Execute(input, args, env, control, data2.Exprs, data2.Intervals);
        }

        public static void Execute(string input, string[] args,
            LigraEnvironment env, ILigraUI control, Expression[] exprs,
            VarInterval[] intervals)
        {
            if (env == null) throw new ArgumentNullException("env");
            if (exprs == null || exprs.Length < 1) throw new ArgumentNullException("exprs");
            if (intervals == null) intervals = new VarInterval[0];

            if (intervals.Length > 2) throw new ArgumentOutOfRangeException("Too many intervals.");

            int outputs = 0;
            if (exprs[0].Result.IsScalar(env))
                outputs = 1;
            else if (exprs[0].Result.IsVector(env))
                outputs = exprs[0].Result.GetVectorLength(env);
            else
                throw new NotImplementedException();

            if (outputs < 1 || outputs > 3)
                throw new InvalidOperationException("Bad number of outputs");

            var inputs = new HashSet<string>();
            foreach (var expr in exprs)
                GetUnboundVariables(expr, env, inputs);
            var intervalNames = new HashSet<string>();
            foreach (var interval in intervals)
                intervalNames.Add(interval.Variable);
            inputs.AddRange(intervalNames);

            if (inputs.Count < 1 || inputs.Count > 2)
                throw new InvalidOperationException("Bad number of inputs");

            /*
            0 inputs doesn't make sense
            0 outputs isn't possible

               | 1 | 2 | 3   outputs
            ---+---+---+---
             1 |2dc|2dc|3dc
            ---+---+---+---
             2 |3ds|2ds|3ds
            inputs

             */

            if (intervals.Length == 0)
            {
                var unboundVars = new HashSet<string>();
                foreach (var expr in exprs)
                    CountUnboundVariables(expr, env, unboundVars);
                var unboundVars2 = new List<string>(unboundVars);
                if (unboundVars.Count < 1 || unboundVars.Count > 2)
                    throw new NotImplementedException(
                        "Unbound vars with no interval should be 1 " +
                        $"or 2 in number. Got {unboundVars.Count}");
                intervals = new VarInterval[unboundVars.Count];
                for (int i = 0; i < unboundVars.Count; i++)
                    intervals[i] = new VarInterval
                    {
                        Interval = new Interval
                        {
                            IsIntegerInterval = false,
                            LowerBound = -5,
                            OpenLowerBound = false,
                            UpperBound = 5,
                            OpenUpperBound = false
                        },
                        Variable = unboundVars2[i]
                    };
            }

            var literals = new List<Literal>();
            foreach (var interval in intervals)
            {
                float midpoint = (interval.Interval.LowerBound + interval.Interval.UpperBound) / 2;
                var literal = new Literal(midpoint);
                env.SetVariable(interval.Variable, literal);
                literals.Add(literal);
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

        public static ISet<string> GetUnboundVariables(Expression expr,
            SolusEnvironment env, HashSet<string> unboundVars = null)
        {
            if (unboundVars == null)
                unboundVars = new HashSet<string>();
            var visitor = new DelegateExpressionVisitor
            {
                VarVisitor = va =>
                {
                    if (env.ContainsVariable(va.VariableName))
                        GetUnboundVariables(
                            env.GetVariable(va.VariableName),
                            env,
                            unboundVars);
                    else
                        unboundVars.Add(va.VariableName);
                }
            };
            expr.AcceptVisitor(visitor);
            return unboundVars;
        }

        public static int CountUnboundVariables(Expression expr,
            SolusEnvironment env)
        {
            return GetUnboundVariables(expr, env).Count;
        }
    }

    public class PlotCommandData : ICommandData
    {
        public PlotCommandData(Expression[] exprs, VarInterval[] intervals)
        {
            Exprs = exprs;
            Intervals = intervals;
        }

        public Solus.Commands.Command Command => PlotCommand.Value;
        public Expression[] Exprs { get; }
        public VarInterval[] Intervals { get; }
    }
}

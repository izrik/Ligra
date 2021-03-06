using System;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class PlotCommand : Command
    {
        public PlotCommand(Expression[] exprs, VarInterval[] intervals)
        {
            _exprs = exprs;
            _intervals = intervals;
        }

        private readonly Expression[] _exprs;
        private readonly VarInterval[] _intervals;
        
        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            Execute(input, args, env, _exprs, _intervals);
        }

        public override string GetInputLabel(string input)
        {
            var label = string.Format("$ plot {0} for {1}",
                string.Join(", ", _exprs.Select(Expression.ToString)),
                string.Join(", ", _intervals.Select((VarInterval vi) => vi.ToString())));
            return label;
        }

        public static void Execute(string input, string[] args, LigraEnvironment env, Expression[] exprs, VarInterval[] intervals)
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
    }
}
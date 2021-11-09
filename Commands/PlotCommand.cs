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

            var intervals2 = intervals.ToList();
            foreach (var name in inputs.Except(intervalNames))
            {
                intervals2.Add(new VarInterval()
                {
                    Interval = new Interval()
                    {
                        IsIntegerInterval = false,
                        LowerBound = -5,
                        OpenLowerBound = false,
                        UpperBound = 5,
                        OpenUpperBound = false
                    },
                    Variable = name
                });
            }

            var pens = new List<LPen>();
            pens.Add(LPen.Blue);
            pens.Add(LPen.Red);
            pens.Add(LPen.Green);
            pens.Add(LPen.Yellow);
            pens.Add(LPen.Cyan);
            pens.Add(LPen.Magenta);

            if (inputs.Count == 1)
            {
                if (outputs == 1)
                {
                    // "f(x)", "f(x) for x"
                    // -> [x, f(x)] for x
                    // 2d curve

                    var entries = new List<GraphEntry>();
                    float varMin0 = -1;
                    float varMax0 = 1;
                    float valueMin0 = -1;
                    float valueMax0 = 1;
                    bool first = true;
                    var interval = intervals2.First();
                    foreach (var expr in exprs)
                    {
                        var varname = interval.Variable;
                        var varMin = interval.Interval.LowerBound;
                        var varMax = interval.Interval.UpperBound;

                        if (first || varMin < varMin0)
                            varMin0 = varMin;
                        if (first || varMax > varMax0)
                            varMax0 = varMax;

                        EstimateBounds(expr, env, varname, varMin, varMax,
                            out float valueMin, out float valueMax);
                        if (first || valueMin < valueMin0)
                            valueMin0 = valueMin;
                        if (first || valueMax > valueMax0)
                            valueMax0 = valueMax;
                        first = false;

                        var expr1 = new VectorExpression(2,
                            new VariableAccess(varname),
                            expr);
                        entries.Add(
                            new GraphEntry(expr1,
                                pens[entries.Count % pens.Count], interval));
                    }

                    var dx = (varMax0 - varMin0) / 4;
                    varMin0 -= dx;
                    varMax0 += dx;
                    var dy = (valueMax0 - valueMin0) / 4;
                    valueMin0 -= dy;
                    valueMax0 += dy;

                    var item = new GraphItem(
                        new SolusParser(),
                        env, entries,
                        varMin0, varMax0,
                        valueMin0, valueMax0);
                    control.AddRenderItem(item);
                    return;
                }
                else if (outputs == 2)
                {
                    // "[f(x), g(x)]", "[f(x), g(x)] for x"
                    // -> [f(x), g(x)] for x
                    // 2d curve

                    var entries = new List<GraphEntry>();
                    float xMin0 = -1;
                    float xMax0 = 1;
                    float yMin0 = -1;
                    float yMax0 = 1;
                    bool first = true;
                    var interval = intervals2.First();
                    foreach (var expr in exprs)
                    {
                        var varname = interval.Variable;
                        var varMin = interval.Interval.LowerBound;
                        var varMax = interval.Interval.UpperBound;

                        EstimateBounds(expr, env, varname, varMin, varMax,
                            out float xMin, out float xMax,
                            out float yMin, out float yMax);
                        if (first || xMin < xMin0) xMin0 = xMin;
                        if (first || xMax > xMax0) xMax0 = xMax;
                        if (first || yMin < yMin0) yMin0 = yMin;
                        if (first || yMax > yMax0) yMax0 = yMax;
                        first = false;

                        entries.Add(
                            new GraphEntry(expr,
                                pens[entries.Count % pens.Count], interval));
                    }

                    var dx = (xMax0 - xMin0) / 4;
                    xMin0 -= dx;
                    xMax0 += dx;
                    var dy = (yMax0 - yMin0) / 4;
                    yMin0 -= dy;
                    yMax0 += dy;

                    var item = new GraphItem(
                        new SolusParser(),
                        env, entries,
                        xMin0, xMax0,
                        yMin0, yMax0);
                    control.AddRenderItem(item);
                    return;
                }
                else
                {
                }
            }
            else
            {
                if (outputs == 1)
                {
                    // "f(x, y)", "f(x, y) for x, y"
                    // -> [x, y, f(x, y)] for x, y
                    // 3d surface

                    float xMin0 = 0;
                    float xMax0 = 0;
                    float yMin0 = 0;
                    float yMax0 = 0;
                    float zMin0 = 0;
                    float zMax0 = 0;
                    bool first = true;
                    var interval1 = intervals2[0];
                    var interval2 = intervals2[1];
                    Expression expr0;
                    // foreach (var expr in exprs)
                    var expr = exprs[0];
                    {
                        expr0 = new VectorExpression(3,
                            new VariableAccess(interval1.Variable),
                            new VariableAccess(interval2.Variable),
                            expr);

                        EstimateBounds(expr0, env,  interval1, interval2,
                            out float xMin, out float xMax,
                            out float yMin, out float yMax,
                            out float zMin, out float zMax);
                        if (first || xMin < xMin0) xMin0 = xMin;
                        if (first || xMax < xMax0) xMax0 = xMax;
                        if (first || yMin < yMin0) yMin0 = yMin;
                        if (first || yMax < yMax0) yMax0 = yMax;
                        if (first || zMin < zMin0) zMin0 = zMin;
                        if (first || zMax < zMax0) zMax0 = zMax;
                        first = false;
                    }

                    var dx = (xMax0 - xMin0) / 4;
                    xMin0 -= dx;
                    xMax0 += dx;
                    var dy = (yMax0 - yMin0) / 4;
                    yMin0 -= dy;
                    yMax0 += dy;
                    var dz = (zMax0 - zMin0) / 4;
                    zMin0 -= dz;
                    zMax0 += dz;

                    var item = new Graph3dItem(expr0,
                        LPen.Black, LBrush.Green,
                        xMin0,xMax0,
                        yMin0,yMax0,
                        zMin0,zMax0,
                        interval1,
                        interval2,
                        env);
                    control.AddRenderItem(item);
                    return;
                }
                else if (outputs == 2)
                {
                }
                else
                {
                    // "[f(x,y), g(x,y), h(x,y)]",
                    //      "[f(x,y), g(x,y), h(x,y)] for x, y"
                    // -> [x, y, f(x)] for x, y
                    // 3d surface

                    float xMin0 = 0;
                    float xMax0 = 0;
                    float yMin0 = 0;
                    float yMax0 = 0;
                    float zMin0 = 0;
                    float zMax0 = 0;
                    bool first = true;
                    var interval1 = intervals2[0];
                    var interval2 = intervals2[1];
                    Expression expr0;
                    // foreach (var expr in exprs)
                    var expr = exprs[0];
                    {
                        EstimateBounds(expr, env,  interval1, interval2,
                            out float xMin, out float xMax,
                            out float yMin, out float yMax,
                            out float zMin, out float zMax);
                        if (first || xMin < xMin0) xMin0 = xMin;
                        if (first || xMax < xMax0) xMax0 = xMax;
                        if (first || yMin < yMin0) yMin0 = yMin;
                        if (first || yMax < yMax0) yMax0 = yMax;
                        if (first || zMin < zMin0) zMin0 = zMin;
                        if (first || zMax < zMax0) zMax0 = zMax;
                        first = false;
                    }

                    var dx = (xMax0 - xMin0) / 4;
                    xMin0 -= dx;
                    xMax0 += dx;
                    var dy = (yMax0 - yMin0) / 4;
                    yMin0 -= dy;
                    yMax0 += dy;
                    var dz = (zMax0 - zMin0) / 4;
                    zMin0 -= dz;
                    zMax0 += dz;

                    var item = new Graph3dItem(expr,
                        LPen.Black, LBrush.Green,
                        xMin0,xMax0,
                        yMin0,yMax0,
                        zMin0,zMax0,
                        interval1,
                        interval2,
                        env);
                    control.AddRenderItem(item);
                    return;
                }
            }


            if (intervals.Length == 0)
            {
                var unboundVars = new HashSet<string>();
                foreach (var expr in exprs)
                    GetUnboundVariables(expr, env, unboundVars);
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

                int i = 0;
                VarInterval interval = intervals.First();
                foreach (Expression entry in exprs)
                {
                    entries.Add(
                        new GraphEntry(entry, pens[i % pens.Count],
                            interval));
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
                    intervals[0],
                    intervals[1], env));
            }
        }

        private static void EstimateBounds(Expression expr,
            SolusEnvironment env, string varname, float varMin, float varMax,
            out float valueMin, out float valueMax, int numSteps=400)
        {
            var env2 = env.CreateChildEnvironment();
            int i;
            var delta = (varMax - varMin) / (numSteps - 1);
            var varliteral = new Literal(varMin);
            env2.SetVariable(varname, varliteral);

            float v;
            float result;
            var first = true;
            valueMin = -1;
            valueMax = 1;
            for (i = 0; i < numSteps; i++)
            {
                v = i * delta + varMin;
                varliteral.Value = v.ToNumber();
                result = expr.Eval(env2).ToNumber().Value;
                if (float.IsNaN(result) || float.IsInfinity(result))
                    continue;
                if (first)
                {
                    valueMax = result;
                    valueMin = result;
                    first = false;
                }

                if (result > valueMax) valueMax = result;
                if (result < valueMin) valueMin = result;
            }
        }

        private static void EstimateBounds(Expression expr,
            SolusEnvironment env,
            string varname, float varMin, float varMax,
            out float xMin, out float xMax,
            out float yMin, out float yMax,
            int numSteps=400)
        {
            var env2 = env.CreateChildEnvironment();
            int i;
            var delta = (varMax - varMin) / (numSteps - 1);
            var varliteral = new Literal(varMin);
            env2.SetVariable(varname, varliteral);

            float v;
            IMathObject result;
            var first = true;
            xMin = xMax = 0;
            yMin = yMax = 0;
            for (i = 0; i < numSteps; i++)
            {
                v = i * delta + varMin;
                varliteral.Value = v.ToNumber();
                result = expr.Eval(env2);
                // TODO: check that it's a 2-vector with scalar components
                var r2 = result.ToVector();
                var x = r2[0].ToNumber().Value;
                var y = r2[1].ToNumber().Value;
                if (float.IsNaN(x) || float.IsInfinity(x) ||
                    float.IsNaN(y) || float.IsInfinity(y))
                    continue;
                if (first)
                {
                    xMin = xMax = x;
                    yMin = yMax = y;
                    first = false;
                }

                if (x > xMax) xMax = x;
                if (x < xMin) xMin = x;
                if (y > yMax) yMax = y;
                if (y < yMin) yMin = y;
            }
        }

        private static void EstimateBounds(Expression expr,
            SolusEnvironment env,
            VarInterval interval1, VarInterval interval2,
            out float xMin, out float xMax,
            out float yMin, out float yMax,
            out float zMin, out float zMax,
            int numSteps=400)
        {
            var env2 = env.CreateChildEnvironment();

            var varMin1 = interval1.Interval.LowerBound;
            var varMax1 = interval1.Interval.UpperBound;
            var varMin2 = interval2.Interval.LowerBound;
            var varMax2 = interval2.Interval.UpperBound;
            var delta1 = (varMax1 - varMin1) / (numSteps - 1);
            var delta2 = (varMax2 - varMin2) / (numSteps - 1);
            var literal1 = new Literal(varMin1);
            var literal2 = new Literal(varMin2);
            env2.SetVariable(interval1.Variable, literal1);
            env2.SetVariable(interval2.Variable, literal2);

            var first = true;
            xMin = xMax = 0;
            yMin = yMax = 0;
            zMin = zMax = 0;
            int i, j;
            for (i = 0; i < numSteps; i++)
            {
                var v = i * delta1 + varMin1;
                literal1.Value = v.ToNumber();
                for (j = 0; j < numSteps; j++)
                {
                    v = j * delta2 + varMin2;
                    literal2.Value = v.ToNumber();

                    var result = expr.Eval(env2);
                    // TODO: check that it's a 3-vector with scalar components
                    var r2 = result.ToVector();
                    var x = r2[0].ToNumber().Value;
                    var y = r2[1].ToNumber().Value;
                    var z = r2[2].ToNumber().Value;
                    if (float.IsNaN(x) || float.IsInfinity(x) ||
                        float.IsNaN(y) || float.IsInfinity(y) ||
                        float.IsNaN(z) || float.IsInfinity(z))
                        continue;
                    if (first)
                    {
                        xMin = xMax = x;
                        yMin = yMax = y;
                        zMin = zMax = z;
                        first = false;
                    }

                    if (x > xMax) xMax = x;
                    if (x < xMin) xMin = x;
                    if (y > yMax) yMax = y;
                    if (y < yMin) yMin = y;
                    if (z > zMax) zMax = z;
                    if (z < zMin) zMin = z;
                }
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

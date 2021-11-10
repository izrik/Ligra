using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Exceptions;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class Graph2dCurveItem : RenderItem
    {
        public Graph2dCurveItem(SolusParser parser, LigraEnvironment env,
            IEnumerable<Graph2dCurveEntry> entries,
            float? xMin=null, float? xMax=null,
            float? yMin=null, float? yMax=null)
        {
            _timer = new System.Timers.Timer(250);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;

            _entries.AddRange(entries);

            _minX = -2;
            if (xMin.HasValue)
                _minX = xMin.Value;
            _maxX = 2;
            if (xMax.HasValue)
                _maxX = xMax.Value;
            _minY = -2;
            if (yMin.HasValue)
                _minY = yMin.Value;
            _maxY = 2;
            if (yMax.HasValue)
                _maxY = yMax.Value;

            _parser = parser;

            _env = env;
        }

        public override Vector2? DefaultSize => new Vector2(400, 400);

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invalidate();
        }

        System.Timers.Timer _timer;

        public float _maxX;
        public float _minX;
        public float _maxY;
        public float _minY;
        public SolusParser _parser;
        private readonly LigraEnvironment _env;

        public List<Graph2dCurveEntry> _entries = new List<Graph2dCurveEntry>();
        //private SizeF _size = new SizeF(400, 400);

        protected override void InternalRender(IRenderer g,
            DrawSettings drawSettings)
        {
            var location = new Vector2(0, 0);
            var boundsInClient = new RectangleF(location,
                Rect.Size);
            GraphItemUtil.DrawBoundaries2d(g, boundsInClient,
                _minX, _maxX, _minY, _maxY);
            foreach (var entry in _entries)
            {
                if (entry is GraphVectorEntry ve)
                {
                    EvaluateVectors(ve.X, ve.Y, _env,
                        ref entry.PointsCache);
                }
                else
                {
                    EvaluateGraph(entry.Expression,
                        _env, entry.Interval, ref entry.PointsCache);
                }

                LayoutGraph(boundsInClient,
                    _minX, _maxX,
                    _minY, _maxY,
                    entry.PointsCache,
                    ref entry.LayoutPointsCache);

                RenderPoints(g,
                    entry.Pen, entry.LayoutPointsCache);
            }
        }

        protected override Vector2 InternalCalcSize(IRenderer g,
            DrawSettings drawSettings)
        {
            return Rect.Size.ToVector2();
        }

        //public override bool HasChanged(VariableTable env)
        //{
        //    throw new NotImplementedException();
        //}

        protected override void AddVariablesForValueCollection(HashSet<string> vars)
        {
            HashSet<string> tempVars = new HashSet<string>();
            foreach (Graph2dCurveEntry entry in _entries)
            {
                tempVars.Clear();
                var ve = entry as GraphVectorEntry;
                if (ve != null)
                {
                    GatherVariablesForValueCollection(tempVars, ve.X);
                    GatherVariablesForValueCollection(tempVars, ve.Y);
                }
                else
                {
                    GatherVariablesForValueCollection(tempVars, entry.Expression);
                    UngatherVariableForValueCollection(tempVars,
                        entry.Interval.Variable);
                }
                vars.UnionWith(tempVars);
            }
        }

        public Expression ExpressionFromGraphEntry(Graph2dCurveEntry entry)
        {
            return entry.Expression;
        }

        public override void OpenPropertiesWindow(ILigraUI control)
        {
            control.OpenPlotProperties(this);
        }

        public override bool HasPropertyWindow
        {
            get { return true; }
        }

        public static void EvaluateGraph(
            Expression expr,
            SolusEnvironment env,
            VarInterval interval,
            ref Vector2[] points,
            int numSteps=400)
        {
            var varMin = interval.Interval.LowerBound;
            var varMax = interval.Interval.UpperBound;
            var varName = interval.Variable;

            var delta = (varMax - varMin) / (numSteps - 1);
            if (points == null || points.Length < numSteps)
                points = new Vector2[numSteps];

            int i;
            for (i = 0; i < numSteps; i++)
            {
                float x = varMin + delta * i;
                env.SetVariable(varName, new Literal(x));

                var vv = expr.Eval(env);
                if (!vv.IsConcrete)
                    // EvaluationException ?
                    throw new OperandException(
                        "Value is not concrete");

                Vector2 pt;
                if (vv.IsVector(null))
                {
                    if (vv.GetVectorLength(null) != 2)
                        // EvaluationException ?
                        throw new OperandException(
                            "Value is not a 2-vector");
                    var vvv = vv.ToVector();
                    // TODO: check for NaN
                    // TODO: ensure components of vvv are scalars
                    pt = new Vector2(
                        vvv[0].ToNumber().Value,
                        vvv[1].ToNumber().Value);
                }
                else
                {
                    throw new OperandException(
                        "Value is not a 2-vector");
                }

                points[i] = pt;
            }
        }

        public static void LayoutGraph(
            RectangleF boundsInClient,
            float xMin, float xMax,
            float yMin, float yMax,
            Vector2[] points,
            ref Vector2[] layoutPts)
        {
            float deltaX = (xMax - xMin) / boundsInClient.Width;
            float deltaY = (yMax - yMin) / boundsInClient.Height;

            if (layoutPts == null ||
                layoutPts.Length < points.Length)
            {
                layoutPts = new Vector2[points.Length];
            }

            int i, j;
            for (i = 0; i < points.Length; i++)
            {
                var v = Constrain(points[i],
                    xMin, xMax, yMin, yMax);
                layoutPts[i] = ClientFromGraph(v, boundsInClient,
                    xMin, deltaX, yMin, deltaY);
            }
        }

        public static Vector2 Constrain(Vector2 v,
            float xMin, float xMax, float yMin, float yMax)
        {
            return GraphItemUtil.Constrain2d(v, xMin, xMax, yMin, yMax);
        }

        public static void RenderPoints(
            IRenderer g,
            LPen pen,
            Vector2[] layoutPts)
        {
            int i;
            int N = layoutPts.Length;
            var lastPoint = Vector2.Zero;
            var first = true;
            for (i = 0; i < N; i++)
            {
                var next = layoutPts[i];
                // TODO: check for NaN
                if (first)
                    first = false;
                else
                    g.DrawLine(pen, lastPoint, next);
                lastPoint = next;
            }
        }

        private static Vector2 ClientFromGraph(
            Vector2 pt,
            RectangleF boundsInClient,
            float xMin, float deltaX,
            float yMin, float deltaY)
        {
            return GraphItemUtil.ClientFromGraph2d(pt, boundsInClient,
                xMin, deltaX, yMin, deltaY);
        }

        public static void EvaluateVectors(VectorExpression x,
            VectorExpression y, SolusEnvironment env, ref Vector2[] points)
        {
            var xs = x.Select(
                e => e.Eval(env).ToNumber().Value).ToArray();
            var ys = y.Select(
                e => e.Eval(env).ToNumber().Value).ToArray();

            int i;
            int N = Math.Min(xs.Length, ys.Length);
            for (i = 0; i < N; i++)
            {
                var next = new Vector2(xs[i], ys[i]);
                points[i] = next;
            }
        }

        public RectangleF Rect
        {
            get
            {
                if (_control != null)
                    return _control.Rect;
                if (_adapter != null)
                    return new RectangleF(
                        _adapter.Allocation.Left,
                        _adapter.Allocation.Top,
                        _adapter.Allocation.Width,
                        _adapter.Allocation.Height);

                throw new InvalidOperationException(
                    "No UI element available.");
            }
        }
    }

    public class Graph2dCurveEntry
    {
        public Graph2dCurveEntry(Expression expression, LPen pen,
            VarInterval interval)
        {
            Expression = expression;
            Pen = pen;
            Interval = interval;
        }
        protected Graph2dCurveEntry(LPen pen)
        {
            Pen = pen;
        }

        public Expression Expression { get; }
        public VarInterval Interval { get; }
        public LPen Pen { get; }
        public Vector2[] PointsCache;
        public Vector2[] LayoutPointsCache;
    }

    public class GraphVectorEntry : Graph2dCurveEntry
    {
        public GraphVectorEntry(VectorExpression x, VectorExpression y,
            LPen pen)
            : base(pen)
        {
            X = x;
            Y = y;
        }

        public readonly VectorExpression X;
        public readonly VectorExpression Y;
    }
}

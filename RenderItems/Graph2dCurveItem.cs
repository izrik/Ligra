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
            // g.DrawRectangle(LPen.Red, Rect.X, Rect.Y, Rect.Width, Rect.Height);
            bool first = true;
            foreach (Graph2dCurveEntry entry in _entries)
            {
                var ve = entry as GraphVectorEntry;
                var location = new Vector2(0, 0);
                var boundsInClient = new RectangleF(location,
                    Rect.Size);
                if (ve != null)
                {
                    EvaluateVectors(ve.X, ve.Y, _env,
                        ref entry.PointsCache);
                }
                else
                {
                    EvaluateGraph(ref entry.PointsCache, entry.Expression,
                        _env, entry.Interval, boundsInClient);
                }

                RenderPoints(g,
                    boundsInClient,
                    entry.Pen, entry.Pen.Brush,
                    _minX, _maxX, _minY, _maxY,
                    first, entry.PointsCache);
                first = false;
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

        public static void EvaluateGraph(ref Vector2[] points,
            Expression expr,
            SolusEnvironment env,
            VarInterval interval,
            RectangleF boundsInClient,
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

        public static void RenderPoints(IRenderer g,
            RectangleF boundsInClient,
            LPen pen, LBrush brush,
            float xMin, float xMax, float yMin, float yMax,
            bool drawBoundaries,
            Vector2[] points)
        {
            float deltaX = (xMax - xMin) / boundsInClient.Width;
            float deltaY = (yMax - yMin) / boundsInClient.Height;

            if (drawBoundaries)
            {
                GraphItemUtil.DrawBoundaries2d(g, boundsInClient,
                    xMin, xMax, yMin, yMax);
            }

            int i;
            int N = points.Length;
            var lastPoint = ClientFromGraph(points[0], boundsInClient,
                xMin, deltaX, yMin, deltaY);
            for (i = 1; i < N; i++)
            {
                var next = ClientFromGraph(points[i], boundsInClient,
                    xMin, deltaX, yMin, deltaY);
                // TODO: check for NaN
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

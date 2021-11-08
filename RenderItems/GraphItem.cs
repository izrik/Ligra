using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class GraphEntry
    {
        public GraphEntry(Expression expression, LPen pen, string independentVariable)
        {
            _expression = expression;
            _pen = pen;
            _independentVariable = independentVariable;
        }
        protected GraphEntry(LPen pen)
        {
            _pen = pen;
        }

        private Expression _expression;
        public Expression Expression
        {
            get { return _expression; }
        }

        private string _independentVariable;
        public string IndependentVariable
        {
            get { return _independentVariable; }
        }

        private LPen _pen;
        public LPen Pen
        {
            get { return _pen; }
        }

        public Vector2[] PointsCache;
    }

    public class GraphVectorEntry : GraphEntry
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

    public class GraphItem : RenderItem
    {
        public GraphItem(Expression expression, LPen pen,
            string independentVariable, SolusParser parser,
            LigraEnvironment env)
            : this(parser, env,
                new GraphEntry(expression, pen, independentVariable))
        {
        }
        public GraphItem(SolusParser parser, LigraEnvironment env,
            params GraphEntry[] entries)
            : this(parser, env, (IEnumerable<GraphEntry>)entries)
        {
        }
        public GraphItem(SolusParser parser, LigraEnvironment env,
            IEnumerable<GraphEntry> entries)
        {
            _timer = new System.Timers.Timer(250);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;

            _entries.AddRange(entries);

            _maxX = 2;
            _minX = -2;
            _maxY = 2;
            _minY = -2;
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

        public List<GraphEntry> _entries = new List<GraphEntry>();
        //private SizeF _size = new SizeF(400, 400);

        protected override void InternalRender(IRenderer g,
            DrawSettings drawSettings)
        {
            // g.DrawRectangle(LPen.Red, Rect.X, Rect.Y, Rect.Width, Rect.Height);
            bool first = true;
            foreach (GraphEntry entry in _entries)
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
                    EvaluateGraph(boundsInClient, _minX, _maxX,
                        entry.Expression, entry.IndependentVariable, _env,
                        ref entry.PointsCache);
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
            foreach (GraphEntry entry in _entries)
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
                    UngatherVariableForValueCollection(tempVars, entry.IndependentVariable);
                }
                vars.UnionWith(tempVars);
            }
        }

        public Expression ExpressionFromGraphEntry(GraphEntry entry)
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

        public static void EvaluateGraph(RectangleF boundsInClient,
            float xMin, float xMax,
            Expression expr, string independentVariable,
            SolusEnvironment env,
            ref Vector2[] points)
        {
            float deltaX = (xMax - xMin) / boundsInClient.Width;

            env.SetVariable(independentVariable, new Literal(xMin));

            if (points == null || points.Length < boundsInClient.Width)
                points = new Vector2[(int)boundsInClient.Width];

            int i;
            for (i = 0; i < boundsInClient.Width; i++)
            {
                float x = xMin + deltaX * i;
                env.SetVariable(independentVariable, new Literal(x));
                var vv = expr.Eval(env);
                double value = vv.ToNumber().Value;
                if (double.IsNaN(value))
                {
                    value = 0;
                }

                points[i] = new Vector2(x, (float)value);
            }
        }

        public static void RenderPoints(IRenderer g,
            RectangleF boundsInClient,
            LPen pen, LBrush brush,
            float xMin, float xMax, float yMin, float yMax,
            bool drawboundaries,
            Vector2[] points)
        {
            float deltaX = (xMax - xMin) / boundsInClient.Width;
            float deltaY = (yMax - yMin) / boundsInClient.Height;

            if (drawboundaries)
            {
                g.DrawRectangle(LPen.Black, boundsInClient.X, boundsInClient.Y, boundsInClient.Width, boundsInClient.Height);

                var zz = ClientFromGraph(Vector2.Zero,
                    boundsInClient, xMin, deltaX, yMin, deltaY);
                if (xMax > 0 && xMin < 0)
                {
                    g.DrawLine(LPen.DarkGray, zz.X, boundsInClient.Top,
                        zz.X, boundsInClient.Bottom);
                }

                if (yMax > 0 && yMin < 0)
                {
                    g.DrawLine(LPen.DarkGray, boundsInClient.Left, zz.Y,
                        boundsInClient.Right, zz.Y);
                }
            }

            int i;
            int N = points.Length;
            var lastPoint = ClientFromGraph(points[0], boundsInClient,
                xMin, deltaX, yMin, deltaY);
            for (i = 1; i < N; i++)
            {
                var next = ClientFromGraph(points[i], boundsInClient,
                    xMin, deltaX, yMin, deltaY);
                g.DrawLine(pen, lastPoint, next);
                lastPoint = next;
            }
        }

        private static Vector2 ClientFromGraph(Vector2 pt,
            RectangleF boundsInClient, float xMin, float deltaX,
            float yMin, float deltaY)
        {
            return new Vector2(boundsInClient.X + (pt.X - xMin) / deltaX,
                boundsInClient.Bottom - (pt.Y - yMin) / deltaY);
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
}

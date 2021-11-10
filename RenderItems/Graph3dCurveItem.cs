using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Solus.Exceptions;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class Graph3dCurveItem : RenderItem
    {
        public Graph3dCurveItem(IEnumerable<Graph3dCurveEntry> entries,
            LigraEnvironment env,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            VarInterval interval,
            string label1,
            string label2)
        {
            
            _timer = new System.Timers.Timer(250);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;

            _entries.AddRange(entries);

            _xMin = xMin;
            _xMax = xMax;
            _yMin = yMin;
            _yMax = yMax;
            _zMin = zMin;
            _zMax = zMax;
            _xMinLabel = xMin.ToString();
            _xMaxLabel = xMax.ToString();
            _yMinLabel = yMin.ToString();
            _yMaxLabel = yMax.ToString();
            _zMinLabel = zMin.ToString();
            _zMaxLabel = zMax.ToString();

            _env = env;

            Interval = interval;

            Label1 = label1;
            Label2 = label2;
        }

        private readonly System.Timers.Timer _timer;
        private readonly float _xMin;
        private readonly float _xMax;
        private readonly float _yMin;
        private readonly float _yMax;
        private readonly float _zMin;
        private readonly float _zMax;
        private readonly string _xMinLabel;
        private readonly string _xMaxLabel;
        private readonly string _yMinLabel;
        private readonly string _yMaxLabel;
        private readonly string _zMinLabel;
        private readonly string _zMaxLabel;
        private readonly LigraEnvironment _env;
        public string Label1 { get; }
        public string Label2 { get; }
        public VarInterval Interval { get; }

        private readonly List<Graph3dCurveEntry> _entries =
            new List<Graph3dCurveEntry>();

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invalidate();
        }

        public override Vector2? DefaultSize => new Vector2(400, 400);

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

        protected override void InternalRender(IRenderer g, DrawSettings drawSettings)
        {
            var location = new Vector2(0, 0);
            var boundsInClient = new RectangleF(location,
                Rect.Size);
            GraphItemUtil.DrawBoundaries3d(g, boundsInClient,
                _xMin, _xMax,
                _yMax, _yMin,
                _zMin, _zMax,
                _xMinLabel, _xMaxLabel,
                _yMaxLabel, _yMinLabel,
                _zMinLabel, _zMaxLabel,
                drawSettings.Font,
                Label1, Label2);
            foreach (var entry in _entries)
            {
                EvaluateGraph(entry.Expression,
                    _env, entry.Interval, ref entry.PointsCache);
                LayoutGraph(
                    boundsInClient,
                    _xMin,_xMax,
                    _yMin,_yMax,
                    _zMin,_zMax,
                    entry.PointsCache,
                    ref entry.LayoutPointsCache);
                RenderPoints(g, entry.Pen, entry.LayoutPointsCache);
            }
        }

        public static void EvaluateGraph(
            Expression expr,
            LigraEnvironment env,
            VarInterval interval,
            ref Vector3[] points,
            int numSteps = 400)
        {
            var varMin = interval.Interval.LowerBound;
            var varMax = interval.Interval.UpperBound;
            var varName = interval.Variable;

            var delta = (varMax - varMin) / (numSteps - 1);
            if (points == null || points.Length < numSteps)
                points = new Vector3[numSteps];

            int i;
            var literal = new Literal(0);
            env.SetVariable(varName, literal);
            for (i = 0; i < numSteps; i++)
            {
                float x = varMin + delta * i;
                literal.Value = x.ToNumber();

                var vv = expr.Eval(env);
                points[i] = GraphItemUtil.EvaluatePoint3d(vv);
            }
        }

        public static void LayoutGraph(
            RectangleF boundsInClient,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            Vector3[] points,
            ref Vector2[] layoutPts)
        {
            if (layoutPts == null ||
                layoutPts.Length < points.Length)
            {
                layoutPts = new Vector2[points.Length];
            }

            int i, j;
            for (i = 0; i < points.Length; i++)
            {
                var v = Constrain(points[i],
                    xMin, xMax, yMin, yMax, zMin, zMax);
                layoutPts[i] = ClientFromGraph(v, boundsInClient,
                    xMin, xMax, yMin, yMax, zMin, zMax);
            }
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

        public static Vector3 Constrain(Vector3 v,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax)
        {
            return GraphItemUtil.Constrain3d(v,
                xMin, xMax, yMin, yMax, zMin, zMax);
        }

        public static Vector2 ClientFromGraph(Vector3 v,
            RectangleF boundsInClient,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax)
        {
            return GraphItemUtil.ClientFromGraph3d(v, boundsInClient,
                xMin, xMax, yMin, yMax, zMin, zMax);
        }

        protected override Vector2 InternalCalcSize(IRenderer g, DrawSettings drawSettings)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class Graph3dCurveEntry
    {
        public Graph3dCurveEntry(Expression expression, LPen pen,
            VarInterval interval)
        {
            Expression = expression;
            Pen = pen;
            Interval = interval;
        }

        public Expression Expression { get; }
        public VarInterval Interval { get; }
        public LPen Pen { get; }

        public Vector3[] PointsCache;
        public Vector2[] LayoutPointsCache;
    }

}

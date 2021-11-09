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
            // g.DrawRectangle(LPen.Red, Rect.X, Rect.Y, Rect.Width, Rect.Height);
            bool first = true;
            foreach (var entry in _entries)
            {
                var location = new Vector2(0, 0);
                var boundsInClient = new RectangleF(location,
                    Rect.Size);
                EvaluateGraph(ref entry.PointsCache, entry.Expression,
                    _env, entry.Interval, boundsInClient);
                LayoutGraph(
                    boundsInClient,
                    _xMin,_xMax,
                    _yMin,_yMax,
                    _zMin,_zMax,
                    entry.PointsCache,
                    ref entry.LayoutPointsCache);
                RenderPoints(g,
                    boundsInClient,
                    entry.Pen, entry.Pen.Brush,
                    _xMin, _xMax,
                    _yMin, _yMax,
                    _zMin, _zMax,
                    _xMinLabel, _xMaxLabel,
                    _yMinLabel, _yMaxLabel,
                    _zMinLabel, _zMaxLabel,
                    entry.Expression,
                    entry.Interval,
                    _env,
                    first,
                    drawSettings.Font,
                    Label1,
                    Label2,
                    entry.PointsCache,
                    entry.LayoutPointsCache);
                first = false;
            }
        }

        public static void EvaluateGraph(ref Vector3[] points,
            Expression expr,
            LigraEnvironment env,
            VarInterval interval,
            RectangleF boundsInClient,
            int numSteps=400)
        {
            var varMin = interval.Interval.LowerBound;
            var varMax = interval.Interval.UpperBound;
            var varName = interval.Variable;

            var delta = (varMax - varMin) / (numSteps - 1);
            if (points == null || points.Length < numSteps)
                points = new Vector3[numSteps];

            int i;
            for (i = 0; i < numSteps; i++)
            {
                float x = varMin + delta * i;
                env.SetVariable(varName, new Literal(x));
                var vv = expr.Eval(env);
                if (!vv.IsConcrete)
                    // EvaluationException ?
                    throw new OperandException("Value is not concrete");

                Vector3 pt;
                if (vv.IsVector(null))
                {
                    if (vv.GetVectorLength(null) != 3)
                        // EvaluationException ?
                        throw new OperandException(
                            "Value is not a 3-vector");
                    var vvv = vv.ToVector();
                    // TODO: check for NaN
                    // TODO: ensure vvv[0] and vvv[1] are scalars
                    pt = new Vector3(
                        vvv[0].ToNumber().Value,
                        vvv[1].ToNumber().Value,
                        vvv[2].ToNumber().Value);
                }
                else
                {
                    throw new OperandException(
                        "Value is not a 3-vector");
                }

                points[i] = pt;
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
            RectangleF boundsInClient,
            LPen pen, LBrush brush,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            string xMinLabel, string xMaxLabel,
            string yMinLabel, string yMaxLabel,
            string zMinLabel, string zMaxLabel,
            Expression expr,
            VarInterval interval,
            LigraEnvironment env,
            bool drawboundaries,
            LFont font,
            string label1,
            string label2,
            Vector3[] points,
            Vector2[] layoutPts)
        {
            int xValues = 50;
            int yValues = 50;

            float x0 = boundsInClient.Left;
            float x1 = boundsInClient.Left + boundsInClient.Width / 2;
            float x2 = boundsInClient.Right;

            float y0 = boundsInClient.Top;
            float y1 = boundsInClient.Top + boundsInClient.Height / 4;
            float y2 = boundsInClient.Top + boundsInClient.Height / 2;
            float y3 = boundsInClient.Top + 3 * boundsInClient.Height / 4;
            float y4 = boundsInClient.Bottom;

            if (drawboundaries)
            {
                Vector2 size;

                g.DrawLine(LPen.Black, x1, y0, x2, y1); //top right
                g.DrawLine(LPen.Black, x2, y1, x2, y3); //right side
                g.DrawLine(LPen.Black, x2, y3, x1, y4); //bottom right
                g.DrawLine(LPen.Black, x1, y4, x0, y3); //bottom left
                g.DrawLine(LPen.Black, x0, y3, x0, y1); //left side
                g.DrawLine(LPen.Black, x0, y1, x1, y0); //top left

                g.DrawLine(LPen.Black, x1, y0, x1, y2); //z axis
                g.DrawLine(LPen.Black, x0, y3, x1, y2); //x axis
                g.DrawLine(LPen.Black, x2, y3, x1, y2); //y axis

                //xmin
                g.DrawLine(LPen.Black, x1, y4, x1 + 6, y4 + 3);
                g.DrawString(xMinLabel, font, LBrush.Black,
                    x1 + 6, y4 + 3);
                //xmax
                g.DrawLine(LPen.Black, x2, y3, x2 + 6, y3 + 3);
                g.DrawString(xMaxLabel, font, LBrush.Black,
                    x2 + 6, y3 + 3);

                //ymin
                g.DrawLine(LPen.Black, x1, y4, x1 - 6, y4 + 3);
                size = g.MeasureString(yMinLabel, font);
                g.DrawString(yMinLabel, font, LBrush.Black,
                    x1 - 6 - size.X, y4 + 3);
                //ymax
                g.DrawLine(LPen.Black, x0, y3, x0 - 6, y3 + 3);
                g.DrawString(yMaxLabel, font, LBrush.Black,
                    x0 - 6, y3 + 3);

                //zmin
                g.DrawLine(LPen.Black, x2, y3, x2 + 6, y3 - 3);
                g.DrawString(zMinLabel, font, LBrush.Black,
                    x2 + 6, y3 - 3 - 14);
                //zmax
                g.DrawLine(LPen.Black, x2, y1, x2 + 6, y1 - 3);
                g.DrawString(zMaxLabel, font, LBrush.Black,
                    x2 + 6, y1 - 3);

                g.DrawString(label1, font, LBrush.Black,
                    (x1 + x2) / 2, (y3 + y4) / 2);
                size = g.MeasureString(label2, font);
                g.DrawString(label2, font, LBrush.Black,
                    (x1 + x0) / 2 - size.X, (y3 + y4) / 2);

                //g.DrawRectangle(LPen.Black, boundsInClient.Left, boundsInClient.Top, boundsInClient.Width, boundsInClient.Height);
            }

            int i;
            int N = points.Length;
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
            if (v.X < xMin) v = new Vector3(xMin, v.Y, v.Z);
            if (v.X > xMax) v = new Vector3(xMax, v.Y, v.Z);
            if (v.Y < yMin) v = new Vector3(v.X, yMin, v.Z);
            if (v.Y > yMax) v = new Vector3(v.X, yMax, v.Z);
            if (v.Z < zMin) v = new Vector3(v.X, v.Y, zMin);
            if (v.Z > zMax) v = new Vector3(v.X, v.Y, zMax);
            return v;
        }

        public static Vector2 ClientFromGraph(Vector3 v,
            RectangleF boundsInClient,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax)
        {
            float x1 = boundsInClient.Left + boundsInClient.Width / 2;
            float y4 = boundsInClient.Bottom;
            float sx = (v.X - xMin) / (xMax - xMin);
            float sy = (v.Y - yMin) / (yMax - yMin);
            float sz = (v.Z - zMin) / (zMax - zMin);

            float xx = x1 + (sx - sy) * boundsInClient.Width * 0.5f;
            float yy = y4 - (sx + sy) * boundsInClient.Height / 4 -
                       sz * boundsInClient.Height / 2;
            return new Vector2(xx, yy);
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using System.Linq;
using Gtk;

namespace MetaphysicsIndustries.Ligra
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
    }

    public class GraphVectorEntry : GraphEntry
    {
        public GraphVectorEntry(SolusVector x, SolusVector y, LPen pen)
            : base(pen)
        {
            X = x;
            Y = y;
        }

        public readonly SolusVector X;
        public readonly SolusVector Y;
    }

    public class GraphItem : RenderItem
    {
        public GraphItem(Expression expression, LPen pen, string independentVariable, SolusParser parser, LigraEnvironment env)
            : this(parser, env, new GraphEntry(expression, pen, independentVariable))
        {
        }

        public GraphItem(SolusParser parser, LigraEnvironment env, params GraphEntry[] entries)
            : this(parser, env, (IEnumerable<GraphEntry>)entries)
        {
        }

        public GraphItem(SolusParser parser, LigraEnvironment env, IEnumerable<GraphEntry> entries)
            : base(env)
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

        public List<GraphEntry> _entries = new List<GraphEntry>();
        //private SizeF _size = new SizeF(400, 400);

        protected override void InternalRender(IRenderer g, SolusEnvironment env)
        {
            g.DrawRectangle(LPen.Red, Rect.X, Rect.Y, Rect.Width, Rect.Height);
            bool first = true;
            foreach (GraphEntry entry in _entries)
            {
                var ve = entry as GraphVectorEntry;
                var location = new Vector2(0, 0);
                if (ve != null)
                {
                    RenderVectors(g,
                        new RectangleF(location, Rect.Size),
                        entry.Pen, entry.Pen.Brush,
                        _minX, _maxX, _minY, _maxY,
                        ve.X, ve.Y,
                        env, first);
                }
                else
                {
                    RenderGraph(g,
                        new RectangleF(location, Rect.Size),
                        entry.Pen, entry.Pen.Brush,
                        _minX, _maxX, _minY, _maxY,
                        entry.Expression, entry.IndependentVariable, env, first);
                }
                first = false;
            }
        }

        protected override Vector2 InternalCalcSize(IRenderer g)
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

        private Expression ExpressionFromGraphEntry(GraphEntry entry)
        {
            return entry.Expression;
        }

        public override void OpenPropertiesWindow(ILigraUI control)
        {
            PlotPropertiesForm form = new PlotPropertiesForm(_parser);
            var graphUI = GetControl();

            form.PlotSize = graphUI.Rect.Size;
            form.PlotMaxX = _maxX;
            form.PlotMinX = _minX;
            form.PlotMaxY = _maxY;
            form.PlotMinY = _minY;

            form.SetExpressions(Array.ConvertAll<GraphEntry, Expression>(_entries.ToArray(), ExpressionFromGraphEntry));

            LigraControl _control = (LigraControl)control;
            if (form.ShowDialog(_control) == System.Windows.Forms.DialogResult.OK)
            {
                graphUI.Rect = new RectangleF(graphUI.Rect.Location, form.PlotSize);

                _maxX = form.PlotMaxX;
                _minX = form.PlotMinX;
                _maxY = form.PlotMaxY;
                _minY = form.PlotMinY;
            }
        }

        public override bool HasPropertyWindow
        {
            get { return true; }
        }

        public static void RenderGraph(IRenderer g, RectangleF boundsInClient,
            LPen pen, LBrush brush,
            float xMin, float xMax, float yMin, float yMax,
            Expression expr, string independentVariable,
            SolusEnvironment env,
            bool drawboundaries)
        {
            float deltaX = (xMax - xMin) / boundsInClient.Width;
            float deltaY = boundsInClient.Height / (yMax - yMin);

            if (drawboundaries)
            {
                g.DrawRectangle(LPen.Black, boundsInClient.X, boundsInClient.Y, boundsInClient.Width, boundsInClient.Height);

                //if (xMax > 0 && xMin < 0)
                //{
                //    float ii = -xMin / deltaX + boundsInClient.X;
                //    g.DrawLine(Pens.Black, ii, boundsInClient.Top, ii, boundsInClient.Bottom);
                //}

                //if (yMax > 0 && yMin < 0)
                //{
                //    float y = boundsInClient.Bottom + yMin * deltaY;
                //    g.DrawLine(Pens.Black, boundsInClient.Left, y, boundsInClient.Right, y);
                //}
            }

            env.Variables[independentVariable] = new Literal(xMin);//+deltaX*50);
            //PointF lastPoint = new PointF(boundsInClient.Left, boundsInClient.Bottom - (Math.Max(Math.Min(_engine.Eval(expr, env).Value, yMax), yMin) - yMin) * deltaY);

            double vvalue = expr.Eval(env).Value;
            if (double.IsNaN(vvalue))
            {
                vvalue = 0;
            }
            vvalue = Math.Min(vvalue, yMax);
            vvalue = Math.Max(vvalue, yMin);
            double yy = boundsInClient.Bottom - (vvalue - yMin) * deltaY;
            var lastPoint = new Vector2(boundsInClient.Left, (float)yy);

            int i;
            for (i = 0; i < boundsInClient.Width; i++)
            {
                float x = xMin + deltaX * i;
                env.Variables[independentVariable] = new Literal(x);
                double value = expr.Eval(env).Value;
                if (double.IsNaN(value))
                {
                    value = 0;
                }
                value = Math.Min(value, yMax);
                value = Math.Max(value, yMin);
                double y = boundsInClient.Bottom - (value - yMin) * deltaY;

                var pt = new Vector2(i + boundsInClient.X, (float)y);

                g.DrawLine(pen, lastPoint, pt);
                lastPoint = pt;
            }
        }

        public static void RenderVectors(IRenderer g, RectangleF boundsInClient,
            LPen pen, LBrush brush,
            float xMin, float xMax, float yMin, float yMax,
            SolusVector x, SolusVector y,
            SolusEnvironment env,
            bool drawboundaries)
        {
            var xs = x.Select(e => e.FastEval(env).Value).ToArray();
            var ys = y.Select(e => e.FastEval(env).Value).ToArray();

            float deltaX = (xMax - xMin) / boundsInClient.Width;
            float deltaY = (yMax - yMin) / boundsInClient.Height;

            Func<Vector2, Vector2> clientFromGraph = (pt) =>
                 new Vector2(boundsInClient.X + (pt.X - xMin) / deltaX,
                     boundsInClient.Bottom - (pt.Y - yMin) / deltaY);

            if (drawboundaries)
            {
                g.DrawRectangle(LPen.Black, boundsInClient.X, boundsInClient.Y, boundsInClient.Width, boundsInClient.Height);

                var zz = clientFromGraph(new Vector2(0, 0));
                if (xMax > 0 && xMin < 0)
                {
                    g.DrawLine(LPen.Black, zz.X, boundsInClient.Top, zz.X, boundsInClient.Bottom);
                }

                if (yMax > 0 && yMin < 0)
                {
                    g.DrawLine(LPen.Black, boundsInClient.Left, zz.Y, boundsInClient.Right, zz.Y);
                }
            }

            int i;
            int N = Math.Min(xs.Length, ys.Length);
            var lastPoint = clientFromGraph(new Vector2(xs[0], ys[0]));
            for (i = 1; i < N; i++)
            {
                var next = clientFromGraph(new Vector2(xs[i], ys[i]));
                g.DrawLine(pen, lastPoint, next);
                lastPoint = next;
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

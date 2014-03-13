using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;


namespace MetaphysicsIndustries.Ligra
{
    public class GraphEntry
    {
        public GraphEntry(Expression expression, Pen pen, string independentVariable)
        {
            _expression = expression;
            _pen = pen;
            _independentVariable = independentVariable;
        }
        protected GraphEntry(Pen pen)
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

        private Pen _pen;
        public Pen Pen
        {
            get { return _pen; }
        }
    }

    public class GraphVectorEntry : GraphEntry
    {
        public GraphVectorEntry(SolusVector x, SolusVector y, Pen pen)
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
        public GraphItem(Expression expression, Pen pen, string independentVariable, SolusParser parser, LigraEnvironment env)
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
            _entries.AddRange(entries);

            Rect = new RectangleF(0, 0, 400, 400);
            _maxX = 2;
            _minX = -2;
            _maxY = 2;
            _minY = -2;
            _parser = parser;
        }

        public float _maxX;
        public float _minX;
        public float _maxY;
        public float _minY;
        SolusParser _parser;

        private List<GraphEntry> _entries = new List<GraphEntry>();
        //private SizeF _size = new SizeF(400, 400);

        protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            bool first = true;
            foreach (GraphEntry entry in _entries)
            {
                var ve = entry as GraphVectorEntry;
                var location = new PointF(0, 0);
                if (ve != null)
                {
                    LigraControl.RenderVectors(g,
                        new RectangleF(location, Rect.Size),
                        entry.Pen, entry.Pen.Brush,
                        _minX, _maxX, _minY, _maxY,
                        ve.X, ve.Y,
                        env, first);
                }
                else
                {
                    LigraControl.RenderGraph(g,
                        new RectangleF(location, Rect.Size),
                        entry.Pen, entry.Pen.Brush,
                        _minX, _maxX, _minY, _maxY,
                        entry.Expression, entry.IndependentVariable, env, first);
                }
                first = false;
            }
        }

        protected override SizeF InternalCalcSize(Graphics g)
        {
            return Rect.Size;
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

        public override void OpenPropertiesWindow(LigraControl control)
        {
            PlotPropertiesForm form = new PlotPropertiesForm(_parser);

            form.PlotSize = Rect.Size;
            form.PlotMaxX = _maxX;
            form.PlotMinX = _minX;
            form.PlotMaxY = _maxY;
            form.PlotMinY = _minY;

            form.SetExpressions(Array.ConvertAll<GraphEntry, Expression>(_entries.ToArray(), ExpressionFromGraphEntry));

            if (form.ShowDialog(control) == System.Windows.Forms.DialogResult.OK)
            {
                Rect = new RectangleF(Rect.Location, form.PlotSize);

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
    }
}

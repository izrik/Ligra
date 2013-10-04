using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Ligra
{
    public class GraphEntry
    {
        public GraphEntry(Expression expression, Pen pen, Variable independentVariable)
        {
            _expression = expression;
            _pen = pen;
            _independentVariable = independentVariable;
        }

        private Expression _expression;
        public Expression Expression
        {
            get { return _expression; }
        }

        private Variable _independentVariable;
        public Variable IndependentVariable
        {
            get { return _independentVariable; }
        }

        private Pen _pen;
        public Pen Pen
        {
            get { return _pen; }
        }
    }

    public class GraphItem : RenderItem
    {
        public GraphItem(Expression expression, Pen pen, Variable independentVariable, SolusParser parser)
            : this(parser, new GraphEntry(expression, pen, independentVariable))
        {
        }

        public GraphItem(SolusParser parser, params GraphEntry[] entries)
            : this(parser, (IEnumerable<GraphEntry>)entries)
        {
        }

        public GraphItem(SolusParser parser, IEnumerable<GraphEntry> entries)
        {
            _entries.AddRange(entries);

            Rect = new RectangleF(0, 0, 400, 400);
            _maxX = 2;
            _minX = -2;
            _maxY = 2;
            _minY = -2;
            _parser = parser;
        }

        float _maxX;
        float _minX;
        float _maxY;
        float _minY;
        SolusParser _parser;

        private List<GraphEntry> _entries = new List<GraphEntry>();
        //private SizeF _size = new SizeF(400, 400);

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, VariableTable varTable)
        {
            bool first = true;
            foreach (GraphEntry entry in _entries)
            {
                control.RenderGraph(g,
                    new RectangleF(location, Rect.Size),
                    entry.Pen, entry.Pen.Brush,
                    _minX, _maxX, _minY, _maxY,
                    entry.Expression, entry.IndependentVariable, varTable, first);
                first = false;
            }
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return Rect.Size;
        }

        //public override bool HasChanged(VariableTable varTable)
        //{
        //    throw new NotImplementedException();
        //}

        protected override void AddVariablesForValueCollection(Set<string> vars)
        {
            Set<string> tempVars = new Set<string>();
            foreach (GraphEntry entry in _entries)
            {
                tempVars.Clear();
                GatherVariablesForValueCollection(tempVars, entry.Expression);
                UngatherVariableForValueCollection(tempVars, entry.IndependentVariable);
                vars.AddRange(tempVars);
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

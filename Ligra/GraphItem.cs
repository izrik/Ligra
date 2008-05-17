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
        public GraphItem(Expression expression, Pen pen, Variable independentVariable)
            : this(new GraphEntry(expression, pen, independentVariable))
        {
        }

        public GraphItem(params GraphEntry[] entries)
            : this((IEnumerable<GraphEntry>)entries)
        {
        }

        public GraphItem(IEnumerable<GraphEntry> entries)
        {
            _entries.AddRange(entries);
        }

        private List<GraphEntry> _entries = new List<GraphEntry>();
        //private SizeF _size = new SizeF(400, 400);

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, VariableTable varTable)
        {
            bool first = true;
            foreach (GraphEntry entry in _entries)
            {
                control.RenderGraph(g,
                    new RectangleF(location.X, location.Y, 400, 400),
                    entry.Pen, entry.Pen.Brush,
                    //-3.2f, 3.2f, -0.5f, 2.5f,
                    -2, 2, -2, 2,
                    entry.Expression, entry.IndependentVariable, varTable, first);
                first = false;
            }
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return new SizeF(400, 400);
        }

        //public override bool HasChanged(VariableTable varTable)
        //{
        //    throw new NotImplementedException();
        //}

        protected override void AddVariablesForValueCollection(Set<Variable> vars)
        {
            Set<Variable> tempVars = new Set<Variable>();
            foreach (GraphEntry entry in _entries)
            {
                tempVars.Clear();
                GatherVariablesForValueCollection(tempVars, entry.Expression);
                UngatherVariableForValueCollection(tempVars, entry.IndependentVariable);
                vars.AddRange(tempVars);
            }
        }
    }
}

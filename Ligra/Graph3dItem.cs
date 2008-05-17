using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Ligra
{
    public class Graph3dItem : RenderItem
    {
        public Graph3dItem(Expression expression, Pen pen, Brush brush,
            double xMin, double xMax,
            double yMin, double yMax,
            double zMin, double zMax,
            Variable independentVariableX,
            Variable independentVariableY)
        {
            _expression = expression;
            _pen = pen;
            _brush = brush;
            _independentVariableX = independentVariableX;
            _independentVariableY = independentVariableY;
            _xMin = xMin;
            _xMax = xMax;
            _yMin = yMin;
            _yMax = yMax;
            _zMin = zMin;
            _zMax = zMax;
        }

        private Expression _expression;
        private Pen _pen;
        private Brush _brush;
        private Variable _independentVariableX;
        private Variable _independentVariableY;
        private double _xMin;
        private double _xMax;
        private double _yMin;
        private double _yMax;
        private double _zMin;
        private double _zMax;


        protected override void InternalRender(LigraControl control, Graphics g, PointF location, VariableTable varTable)
        {
            control.Render3DGraph(g,
                new RectangleF(location.X, location.Y, 400, 400),
                _pen, _brush,
                _xMin, _xMax,
                _yMin, _yMax,
                _zMin, _zMax,
                _expression,
                _independentVariableX,
                _independentVariableY,
                varTable, true);
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
            GatherVariablesForValueCollection(vars, _expression);
        }

        protected override void RemoveVariablesForValueCollection(Set<Variable> vars)
        {
            UngatherVariableForValueCollection(vars, _independentVariableX);
            UngatherVariableForValueCollection(vars, _independentVariableY);
        }
    }
}

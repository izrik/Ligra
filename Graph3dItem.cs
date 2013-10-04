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
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            string independentVariableX,
                           string independentVariableY)
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
        private string _independentVariableX;
        private string _independentVariableY;
        private float _xMin;
        private float _xMax;
        private float _yMin;
        private float _yMax;
        private float _zMin;
        private float _zMax;


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

        protected override void AddVariablesForValueCollection(Set<string> vars)
        {
            GatherVariablesForValueCollection(vars, _expression);
        }

        protected override void RemoveVariablesForValueCollection(Set<string> vars)
        {
            UngatherVariableForValueCollection(vars, _independentVariableX);
            UngatherVariableForValueCollection(vars, _independentVariableY);
        }
    }
}

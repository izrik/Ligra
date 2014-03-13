using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;


namespace MetaphysicsIndustries.Ligra
{
    public class Graph3dItem : RenderItem
    {
        public Graph3dItem(Expression expression, Pen pen, Brush brush,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            string independentVariableX,
            string independentVariableY,
            LigraEnvironment env)
            : base(env)
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

        int lastTime = Environment.TickCount;
        int numRenders = 0;
        int numTicks = 0;
        string fps = "";

            protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            var stime = Environment.TickCount;

            LigraControl.Render3DGraph(g,
                new RectangleF(0, 0, 400, 400),
                _pen, _brush,
                _xMin, _xMax,
                _yMin, _yMax,
                _zMin, _zMax,
                _expression,
                _independentVariableX,
                _independentVariableY,
                env, true, this.Font);

            var dtime = Environment.TickCount - stime;
            numTicks += dtime;
            numRenders++;

            var time = Environment.TickCount;
            if (time > lastTime + 1000)
            {
                fps = string.Format("{0} fps", Math.Round(numRenders * 1000.0 / numTicks, 2));
                lastTime = time;
                numTicks = 0;
                numRenders = 0;
            }

            g.DrawString(fps, this.Font, Brushes.Blue, new PointF(0, 0));
        }

        protected override SizeF InternalCalcSize(Graphics g)
        {
            return new SizeF(400, 400);
        }

        //public override bool HasChanged(VariableTable env)
        //{
        //    throw new NotImplementedException();
        //}

        protected override void AddVariablesForValueCollection(HashSet<string> vars)
        {
            GatherVariablesForValueCollection(vars, _expression);
        }

        protected override void RemoveVariablesForValueCollection(HashSet<string> vars)
        {
            UngatherVariableForValueCollection(vars, _independentVariableX);
            UngatherVariableForValueCollection(vars, _independentVariableY);
        }
    }
}

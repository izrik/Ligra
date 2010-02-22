using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus;
//using MetaphysicsIndustries.Utilities;
using System.Drawing;
using MetaphysicsIndustries.Collections;
using MetaphysicsIndustries.Acuity;

namespace MetaphysicsIndustries.Ligra
{
    public class GraphVectorItem : RenderItem
    {
        public GraphVectorItem(Vector vector, string caption)
        {
            _vector = vector.Clone();
            _caption = caption;
        }

        private Vector _vector;
        public Vector Vector
        {
            get { return _vector; }
        }

        string _caption;

        //MemoryImage _image = null;

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, VariableTable varTable)
        {
            RectangleF boundsInClient = new RectangleF(location, InternalCalcSize(control, g));
            boundsInClient.Height = 276;

            control.RenderVector(g, boundsInClient, Pens.Blue, Brushes.Blue, _vector, true);

            RectangleF rect = new RectangleF(location.X + 10, location.Y + 276, _vector.Length, g.MeasureString(_caption, control.Font).Height);
            g.DrawString(_caption, control.Font, Brushes.Black, rect);

            //g.DrawImage(_image.Bitmap, boundsInClient);
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            double x = Math.Log(_vector.Length, 2);
            if (x < 8)
            {
                return new SizeF(276, 276);
            }
            return new SizeF(_vector.Length + 20, 296 + g.MeasureString(_caption, control.Font, _vector.Length).Height);
        }

        protected override void AddVariablesForValueCollection(Set<Variable> vars)
        {
            //foreach (Expression expr in _vector)
            //{
            //    GatherVariablesForValueCollection(vars, expr);
            //}
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus;
//using MetaphysicsIndustries.Utilities;
using System.Drawing;

using MetaphysicsIndustries.Acuity;

namespace MetaphysicsIndustries.Ligra
{
    public class GraphVectorItem : RenderItem
    {
        public GraphVectorItem(Vector vector, string caption, LigraEnvironment env)
            : base(env)
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

            protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            RectangleF boundsInClient = new RectangleF(new PointF(0, 0), InternalCalcSize(g));
            boundsInClient.Height = 276;

            LigraControl.RenderVector(g, boundsInClient, Pens.Blue, Brushes.Blue, _vector, true);

            RectangleF rect = new RectangleF(10, 276, _vector.Length, g.MeasureString(_caption, this.Font).Height);
            g.DrawString(_caption, this.Font, Brushes.Black, rect);

            //g.DrawImage(_image.Bitmap, boundsInClient);
        }

        protected override SizeF InternalCalcSize(Graphics g)
        {
            double x = Math.Log(_vector.Length, 2);
            if (x < 8)
            {
                return new SizeF(276, 276);
            }
            return new SizeF(_vector.Length + 20, 296 + g.MeasureString(_caption, this.Font, _vector.Length).Height);
        }

        protected override void AddVariablesForValueCollection(HashSet<string> vars)
        {
            //foreach (Expression expr in _vector)
            //{
            //    GatherVariablesForValueCollection(vars, expr);
            //}
        }
    }
}

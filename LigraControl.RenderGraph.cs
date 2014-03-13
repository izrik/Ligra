using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;
using System.Linq;


namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraControl : UserControl
    {

        public static void RenderGraph(Graphics g, RectangleF boundsInClient,
                        Pen pen, Brush brush,
                        float xMin, float xMax, float yMin, float yMax,
                        Expression expr, string independentVariable,
                        SolusEnvironment env,
                        bool drawboundaries)
        {
            float deltaX = (xMax - xMin) / boundsInClient.Width;
            float deltaY = boundsInClient.Height / (yMax - yMin);

            if (drawboundaries)
            {
                g.DrawRectangle(Pens.Black, boundsInClient.X, boundsInClient.Y, boundsInClient.Width, boundsInClient.Height);

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
            PointF lastPoint = new PointF(boundsInClient.Left, (float)yy);

            int i;
            for (i = 0; i < boundsInClient.Width; i++)
            {
                float x = xMin + deltaX * i;
                env.Variables[independentVariable] = new Literal(x);
                double value = expr.FastEval(env).Value;
                if (double.IsNaN(value))
                {
                    value = 0;
                }
                value = Math.Min(value, yMax);
                value = Math.Max(value, yMin);
                double y = boundsInClient.Bottom - (value - yMin) * deltaY;

                PointF pt = new PointF(i + boundsInClient.X, (float)y);

                g.DrawLine(pen, lastPoint, pt);
                lastPoint = pt;
            }
        }

        public static void RenderVectors(Graphics g, RectangleF boundsInClient,
            Pen pen, Brush brush,
            float xMin, float xMax, float yMin, float yMax,
            SolusVector x, SolusVector y,
            SolusEnvironment env,
            bool drawboundaries)
        {
            var xs = x.Select(e => e.FastEval(env).Value).ToArray();
            var ys = y.Select(e => e.FastEval(env).Value).ToArray();

            float deltaX = (xMax - xMin) / boundsInClient.Width;
            float deltaY = (yMax - yMin) / boundsInClient.Height;

            Func<PointF,PointF> clientFromGraph = (PointF pt) => 
                new PointF(boundsInClient.X +      (pt.X - xMin) / deltaX,
                           boundsInClient.Bottom - (pt.Y - yMin) / deltaY);

            if (drawboundaries)
            {
                g.DrawRectangle(Pens.Black, boundsInClient.X, boundsInClient.Y, boundsInClient.Width, boundsInClient.Height);

                var zz = clientFromGraph(new PointF(0, 0));
                if (xMax > 0 && xMin < 0)
                {
                    g.DrawLine(Pens.Black, zz.X, boundsInClient.Top, zz.X, boundsInClient.Bottom);
                }

                if (yMax > 0 && yMin < 0)
                {
                    g.DrawLine(Pens.Black, boundsInClient.Left, zz.Y, boundsInClient.Right, zz.Y);
                }
            }

            int i;
            int N = Math.Min(xs.Length,ys.Length);
            PointF lastPoint = clientFromGraph(new PointF(xs[0],ys[0]));
            for (i = 1; i < N; i++)
            {
                var next = clientFromGraph(new PointF(xs[i],ys[i]));
                g.DrawLine(pen, lastPoint, next);
                lastPoint = next;
            }
        }



    }
}
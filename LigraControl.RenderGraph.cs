using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Collections;
using Environment = MetaphysicsIndustries.Solus.Environment;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraControl : UserControl
    {

        public void RenderGraph(Graphics g, RectangleF boundsInClient,
                        Pen pen, Brush brush,
                        float xMin, float xMax, float yMin, float yMax,
                        Expression expr, string independentVariable,
                        Environment env,
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
                double value = expr.Eval(env).Value;
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

    }
}
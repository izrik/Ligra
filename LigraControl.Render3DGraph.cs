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

        public void Render3DGraph(Graphics g, RectangleF boundsInClient,
                        Pen pen, Brush brush,
                        float xMin, float xMax,
                        float yMin, float yMax,
                        float zMin, float zMax,
                        Expression expr,
                        string independentVariableX,
                        string independentVariableY,
                        Environment env,
                        bool drawboundaries)
        {
            int xValues = 50;
            int yValues = 50;

            float[,] values = new float[xValues, yValues];

            float deltaX = (xMax - xMin) / (xValues - 1);
            float deltaY = (yMax - yMin) / (yValues - 1);

            float x0 = boundsInClient.Left;
            float x1 = boundsInClient.Left + boundsInClient.Width / 2;
            float x2 = boundsInClient.Right;

            float y0 = boundsInClient.Top;
            float y1 = boundsInClient.Top + boundsInClient.Height / 4;
            float y2 = boundsInClient.Top + boundsInClient.Height / 2;
            float y3 = boundsInClient.Top + 3 * boundsInClient.Height / 4;
            float y4 = boundsInClient.Bottom;

            if (drawboundaries)
            {
                SizeF size;

                g.DrawLine(Pens.Black, x1, y0, x2, y1); //top right
                g.DrawLine(Pens.Black, x2, y1, x2, y3); //right side
                g.DrawLine(Pens.Black, x2, y3, x1, y4); //bottom right
                g.DrawLine(Pens.Black, x1, y4, x0, y3); //bottom left
                g.DrawLine(Pens.Black, x0, y3, x0, y1); //left side
                g.DrawLine(Pens.Black, x0, y1, x1, y0); //top left

                g.DrawLine(Pens.Black, x1, y0, x1, y2); //z axis
                g.DrawLine(Pens.Black, x0, y3, x1, y2); //x axis
                g.DrawLine(Pens.Black, x2, y3, x1, y2); //y axis

                //xmin
                g.DrawLine(Pens.Black, x1, y4, x1 + 6, y4 + 3);
                g.DrawString(xMin.ToString(), Font, Brushes.Black, x1 + 6, y4 + 3);
                //xmax
                g.DrawLine(Pens.Black, x2, y3, x2 + 6, y3 + 3);
                g.DrawString(xMax.ToString(), Font, Brushes.Black, x2 + 6, y3 + 3);

                //ymin
                g.DrawLine(Pens.Black, x1, y4, x1 - 6, y4 + 3);
                size = g.MeasureString(yMin.ToString(), Font);
                g.DrawString(yMin.ToString(), Font, Brushes.Black, x1 - 6 - size.Width, y4 + 3);
                //ymax
                g.DrawLine(Pens.Black, x0, y3, x0 - 6, y3 + 3);
                g.DrawString(yMax.ToString(), Font, Brushes.Black, x0 - 6, y3 + 3);

                //zmin
                g.DrawLine(Pens.Black, x2, y3, x2 + 6, y3 - 3);
                g.DrawString(zMin.ToString(), Font, Brushes.Black, x2 + 6, y3 - 3 - 14);
                //zmax
                g.DrawLine(Pens.Black, x2, y1, x2 + 6, y1 - 3);
                g.DrawString(zMax.ToString(), Font, Brushes.Black, x2 + 6, y1 - 3);


                g.DrawString(independentVariableX, Font, Brushes.Black, (x1 + x2) / 2, (y3 + y4) / 2);
                size = g.MeasureString(independentVariableY, Font);
                g.DrawString(independentVariableY, Font, Brushes.Black, (x1 + x0) / 2 - size.Width, (y3 + y4) / 2);


                //g.DrawRectangle(Pens.Black, boundsInClient.Left, boundsInClient.Top, boundsInClient.Width, boundsInClient.Height);
            }


            int i;
            int j;
            float x;
            float y;
            float z;

            Expression prelimEval;
            Expression prelimEval2;

            if (env.Variables.ContainsKey(independentVariableX))
            {
                env.Variables.Remove(independentVariableX);
            }
            if (env.Variables.ContainsKey(independentVariableY))
            {
                env.Variables.Remove(independentVariableY);
            }

            prelimEval = _engine.PreliminaryEval(expr, env);

            for (i = 0; i < xValues; i++)
            {
                x = xMin + i * deltaX;

                env.Variables[independentVariableX] = new Literal(x);
                if (env.Variables.ContainsKey(independentVariableY))
                {
                    env.Variables.Remove(independentVariableY);
                }

                prelimEval2 = _engine.PreliminaryEval(prelimEval, env);

                for (j = 0; j < yValues; j++)
                {
                    y = yMin + j * deltaY;
                    env.Variables[independentVariableY] = new Literal(y);

                    z = prelimEval2.Eval(env).Value;

                    if (double.IsNaN(z))
                    {
                        z = 0;
                    }
                    z = Math.Max(z, zMin);
                    z = Math.Min(z, zMax);

                    values[i, j] = z;
                }
            }

            PointF[,] pts = new PointF[xValues, yValues];

            for (i = 0; i < xValues; i++)
            {
                float ii = (i / (float)xValues);
                for (j = 0; j < yValues; j++)
                {
                    float jj = (j / (float)yValues);

                    z = values[i, j];
                    x = x1 + (ii - jj) * boundsInClient.Width * 0.5f;
                    y = y4 - (((ii + jj) * boundsInClient.Height) / 4) - ((((z - zMin) / (zMax - zMin)) * boundsInClient.Height) / 2);

                    pts[i, j] = new PointF((float)x, (float)y);
                }
            }

            //Brush[,] brushes = new Brush[xValues, yValues];
            //for (i = 0; i < xValues; i++)
            //{
            //    for (j = 0; j < yValues; j++)
            //    {
            //        z = values[i,j];
            //        if (z < 0) { z += (int)(Math.Ceiling(Math.Abs(z))); }
            //        Triple<double> hsl = new Triple<double>(z, 1, 0.5);
            //        Triple<double> rgb = SolusEngine.ConvertHslToRgb(hsl);
            //        Color c = Color.FromArgb(Math.Min((int)(rgb.First * 255), 255), Math.Min((int)(rgb.Second * 255), 255), Math.Min((int)(rgb.Third * 255), 255));
            //        brushes[i, j] = new SolidBrush(c);
            //    }
            //}

            for (i = xValues - 2; i >= 0; i--)
            {
                for (j = yValues - 2; j >= 0; j--)
                {
                    PointF[] poly = { pts[i, j], pts[i + 1, j], pts[i + 1, j + 1], pts[i, j + 1] };
                    g.FillPolygon(brush, poly);
                    //g.FillPolygon(brushes[i, j], poly);
                    g.DrawPolygon(pen, poly);
                }
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using Gtk;

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
            _timer = new System.Windows.Forms.Timer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = 250;
            _timer.Enabled = true;

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

        static readonly SolusEngine _engine = new SolusEngine();

        void _timer_Tick (object sender, EventArgs e)
        {
            this.Invalidate();
        }

        System.Windows.Forms.Timer _timer;

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

            Render3DGraph(g,
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

        public static void Render3DGraph(Graphics g, RectangleF boundsInClient,
            Pen pen, Brush brush,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            Expression expr,
            string independentVariableX,
            string independentVariableY,
            SolusEnvironment env,
            bool drawboundaries,
            Font font)
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
                g.DrawString(xMin.ToString(), font, Brushes.Black, x1 + 6, y4 + 3);
                //xmax
                g.DrawLine(Pens.Black, x2, y3, x2 + 6, y3 + 3);
                g.DrawString(xMax.ToString(), font, Brushes.Black, x2 + 6, y3 + 3);

                //ymin
                g.DrawLine(Pens.Black, x1, y4, x1 - 6, y4 + 3);
                size = g.MeasureString(yMin.ToString(), font);
                g.DrawString(yMin.ToString(), font, Brushes.Black, x1 - 6 - size.Width, y4 + 3);
                //ymax
                g.DrawLine(Pens.Black, x0, y3, x0 - 6, y3 + 3);
                g.DrawString(yMax.ToString(), font, Brushes.Black, x0 - 6, y3 + 3);

                //zmin
                g.DrawLine(Pens.Black, x2, y3, x2 + 6, y3 - 3);
                g.DrawString(zMin.ToString(), font, Brushes.Black, x2 + 6, y3 - 3 - 14);
                //zmax
                g.DrawLine(Pens.Black, x2, y1, x2 + 6, y1 - 3);
                g.DrawString(zMax.ToString(), font, Brushes.Black, x2 + 6, y1 - 3);


                g.DrawString(independentVariableX, font, Brushes.Black, (x1 + x2) / 2, (y3 + y4) / 2);
                size = g.MeasureString(independentVariableY, font);
                g.DrawString(independentVariableY, font, Brushes.Black, (x1 + x0) / 2 - size.Width, (y3 + y4) / 2);


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

        protected override Widget GetAdapterInternal()
        {
            throw new NotImplementedException();
        }
    }
}

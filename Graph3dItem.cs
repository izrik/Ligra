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
        public Graph3dItem(Expression expression, LPen pen, LBrush brush,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            string independentVariableX,
            string independentVariableY,
            LigraEnvironment env)
            : base(env)
        {
            _timer = new System.Timers.Timer(250);
            _timer.Elapsed += _timer_Elapsed;
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

        public static readonly SolusEngine _engine = new SolusEngine();

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invalidate();
        }

        System.Timers.Timer _timer;

        public Expression _expression;
        public LPen _pen;
        public LBrush _brush;
        public string _independentVariableX;
        public string _independentVariableY;
        public float _xMin;
        public float _xMax;
        public float _yMin;
        public float _yMax;
        public float _zMin;
        public float _zMax;

        int lastTime = Environment.TickCount;
        int numRenders = 0;
        int numTicks = 0;
        string fps = "";

        protected override void InternalRender(IRenderer g, SolusEnvironment env)
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
                env, true, _env.Font);

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

            g.DrawString(fps, _env.Font, LBrush.Blue, new Vector2(0, 0));
        }

        protected override Vector2 InternalCalcSize(IRenderer g)
        {
            return new Vector2(400, 400);
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

        public static void Render3DGraph(IRenderer g, RectangleF boundsInClient,
            LPen pen, LBrush brush,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            Expression expr,
            string independentVariableX,
            string independentVariableY,
            SolusEnvironment env,
            bool drawboundaries,
            LFont font)
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
                Vector2 size;

                g.DrawLine(LPen.Black, x1, y0, x2, y1); //top right
                g.DrawLine(LPen.Black, x2, y1, x2, y3); //right side
                g.DrawLine(LPen.Black, x2, y3, x1, y4); //bottom right
                g.DrawLine(LPen.Black, x1, y4, x0, y3); //bottom left
                g.DrawLine(LPen.Black, x0, y3, x0, y1); //left side
                g.DrawLine(LPen.Black, x0, y1, x1, y0); //top left

                g.DrawLine(LPen.Black, x1, y0, x1, y2); //z axis
                g.DrawLine(LPen.Black, x0, y3, x1, y2); //x axis
                g.DrawLine(LPen.Black, x2, y3, x1, y2); //y axis

                //xmin
                g.DrawLine(LPen.Black, x1, y4, x1 + 6, y4 + 3);
                g.DrawString(xMin.ToString(), font, LBrush.Black, x1 + 6, y4 + 3);
                //xmax
                g.DrawLine(LPen.Black, x2, y3, x2 + 6, y3 + 3);
                g.DrawString(xMax.ToString(), font, LBrush.Black, x2 + 6, y3 + 3);

                //ymin
                g.DrawLine(LPen.Black, x1, y4, x1 - 6, y4 + 3);
                size = g.MeasureString(yMin.ToString(), font);
                g.DrawString(yMin.ToString(), font, LBrush.Black, x1 - 6 - size.X, y4 + 3);
                //ymax
                g.DrawLine(LPen.Black, x0, y3, x0 - 6, y3 + 3);
                g.DrawString(yMax.ToString(), font, LBrush.Black, x0 - 6, y3 + 3);

                //zmin
                g.DrawLine(LPen.Black, x2, y3, x2 + 6, y3 - 3);
                g.DrawString(zMin.ToString(), font, LBrush.Black, x2 + 6, y3 - 3 - 14);
                //zmax
                g.DrawLine(LPen.Black, x2, y1, x2 + 6, y1 - 3);
                g.DrawString(zMax.ToString(), font, LBrush.Black, x2 + 6, y1 - 3);


                g.DrawString(independentVariableX, font, LBrush.Black, (x1 + x2) / 2, (y3 + y4) / 2);
                size = g.MeasureString(independentVariableY, font);
                g.DrawString(independentVariableY, font, LBrush.Black, (x1 + x0) / 2 - size.X, (y3 + y4) / 2);


                //g.DrawRectangle(LPen.Black, boundsInClient.Left, boundsInClient.Top, boundsInClient.Width, boundsInClient.Height);
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

            Vector2[,] pts = new Vector2[xValues, yValues];

            for (i = 0; i < xValues; i++)
            {
                float ii = (i / (float)xValues);
                for (j = 0; j < yValues; j++)
                {
                    float jj = (j / (float)yValues);

                    z = values[i, j];
                    x = x1 + (ii - jj) * boundsInClient.Width * 0.5f;
                    y = y4 - (((ii + jj) * boundsInClient.Height) / 4) - ((((z - zMin) / (zMax - zMin)) * boundsInClient.Height) / 2);

                    pts[i, j] = new Vector2((float)x, (float)y);
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
                    Vector2[] poly = { pts[i, j], pts[i + 1, j], pts[i + 1, j + 1], pts[i, j + 1] };
                    g.FillPolygon(brush, poly);
                    //g.FillPolygon(brushes[i, j], poly);
                    g.DrawPolygon(pen, poly);
                }
            }
        }
    }
}

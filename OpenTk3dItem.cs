using System;
using MetaphysicsIndustries.Solus;
using System.Drawing;
using MetaphysicsIndustries.Collections;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MetaphysicsIndustries.Ligra
{
    public class OpenTk3dItem: RenderItem
    {
        public OpenTk3dItem(Expression expression, Pen pen, Brush brush,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            string independentVariableX,
            string independentVariableY,
            Font font)
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
            _font = font;

            glcontrol = new GLControl(GraphicsMode.Default, 4,4,GraphicsContextFlags.Default);
            glcontrol.Paint += glcontrol_paint;
        }

        readonly Expression _expression;
        readonly Pen _pen;
        readonly Brush _brush;
        readonly string _independentVariableX;
        readonly string _independentVariableY;
        readonly float _xMin;
        readonly float _xMax;
        readonly float _yMin;
        readonly float _yMax;
        readonly float _zMin;
        readonly float _zMax;
        readonly Font _font;

        GLControl glcontrol;

        bool glloaded = false;

        int lastTime = Environment.TickCount;
        int numRenders = 0;
        int numTicks = 0;
        string fps = "";

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, SolusEnvironment env)
        {
            var stime = Environment.TickCount;

            Render3DGraph(g,
                new RectangleF(location.X, location.Y, 400, 400),
                _pen, _brush,
                _xMin, _xMax,
                _yMin, _yMax,
                _zMin, _zMax,
                _expression,
                _independentVariableX,
                _independentVariableY,
                env, true, _font);

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

            g.DrawString(fps, control.Font, Brushes.Blue, location);
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return new SizeF(400, 400);
        }


        protected override void AddVariablesForValueCollection(Set<string> vars)
        {
            GatherVariablesForValueCollection(vars, _expression);
        }

        protected override void RemoveVariablesForValueCollection(Set<string> vars)
        {
            UngatherVariableForValueCollection(vars, _independentVariableX);
            UngatherVariableForValueCollection(vars, _independentVariableY);
        }

        float[,] values;

        public void Render3DGraph(Graphics g, RectangleF boundsInClient,
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

            values = new float[xValues, yValues];

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

//            if (drawboundaries)
//            {
//                SizeF size;
//
//                g.DrawLine(Pens.Black, x1, y0, x2, y1); //top right
//                g.DrawLine(Pens.Black, x2, y1, x2, y3); //right side
//                g.DrawLine(Pens.Black, x2, y3, x1, y4); //bottom right
//                g.DrawLine(Pens.Black, x1, y4, x0, y3); //bottom left
//                g.DrawLine(Pens.Black, x0, y3, x0, y1); //left side
//                g.DrawLine(Pens.Black, x0, y1, x1, y0); //top left
//
//                g.DrawLine(Pens.Black, x1, y0, x1, y2); //z axis
//                g.DrawLine(Pens.Black, x0, y3, x1, y2); //x axis
//                g.DrawLine(Pens.Black, x2, y3, x1, y2); //y axis
//
//                //xmin
//                g.DrawLine(Pens.Black, x1, y4, x1 + 6, y4 + 3);
//                g.DrawString(xMin.ToString(), font, Brushes.Black, x1 + 6, y4 + 3);
//                //xmax
//                g.DrawLine(Pens.Black, x2, y3, x2 + 6, y3 + 3);
//                g.DrawString(xMax.ToString(), font, Brushes.Black, x2 + 6, y3 + 3);
//
//                //ymin
//                g.DrawLine(Pens.Black, x1, y4, x1 - 6, y4 + 3);
//                size = g.MeasureString(yMin.ToString(), font);
//                g.DrawString(yMin.ToString(), font, Brushes.Black, x1 - 6 - size.Width, y4 + 3);
//                //ymax
//                g.DrawLine(Pens.Black, x0, y3, x0 - 6, y3 + 3);
//                g.DrawString(yMax.ToString(), font, Brushes.Black, x0 - 6, y3 + 3);
//
//                //zmin
//                g.DrawLine(Pens.Black, x2, y3, x2 + 6, y3 - 3);
//                g.DrawString(zMin.ToString(), font, Brushes.Black, x2 + 6, y3 - 3 - 14);
//                //zmax
//                g.DrawLine(Pens.Black, x2, y1, x2 + 6, y1 - 3);
//                g.DrawString(zMax.ToString(), font, Brushes.Black, x2 + 6, y1 - 3);
//
//
//                g.DrawString(independentVariableX, font, Brushes.Black, (x1 + x2) / 2, (y3 + y4) / 2);
//                size = g.MeasureString(independentVariableY, font);
//                g.DrawString(independentVariableY, font, Brushes.Black, (x1 + x0) / 2 - size.Width, (y3 + y4) / 2);
//
//
//                //g.DrawRectangle(Pens.Black, boundsInClient.Left, boundsInClient.Top, boundsInClient.Width, boundsInClient.Height);
//            }


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

            var engine = new SolusEngine();

            prelimEval = engine.PreliminaryEval(expr, env);

            for (i = 0; i < xValues; i++)
            {
                x = xMin + i * deltaX;

                env.Variables[independentVariableX] = new Literal(x);
                if (env.Variables.ContainsKey(independentVariableY))
                {
                    env.Variables.Remove(independentVariableY);
                }

                prelimEval2 = engine.PreliminaryEval(prelimEval, env);

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

        }

        void glcontrol_paint (object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (!glloaded)
                return;

            GL.ClearColor(Color.SkyBlue);

            int w = glcontrol.Width;
            int h = glcontrol.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Color3(Color.Yellow);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex2(10, 20);
            GL.Vertex2(100, 20);
            GL.Vertex2(100, 50);
            GL.End();

            glcontrol.SwapBuffers();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Exceptions;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class Graph3dItem : RenderItem
    {
        public Graph3dItem(Expression expression, LPen pen, LBrush brush,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            VarInterval interval1,
            VarInterval interval2,
            LigraEnvironment env,
            string label1,
            string label2)
        {
            _timer = new System.Timers.Timer(250);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;

            _expression = expression;
            _pen = pen;
            _brush = brush;
            Interval1 = interval1;
            Interval2 = interval2;
            _xMin = xMin;
            _xMax = xMax;
            _yMin = yMin;
            _yMax = yMax;
            _zMin = zMin;
            _zMax = zMax;

            _env = env;

            Label1 = label1;
            Label2 = label2;
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
        public VarInterval Interval1 { get; }
        public VarInterval Interval2 { get; }
        public float _xMin;
        public float _xMax;
        public float _yMin;
        public float _yMax;
        public float _zMin;
        public float _zMax;
        private readonly LigraEnvironment _env;
        public string Label1 { get; }
        public string Label2 { get; }

        int lastTime = Environment.TickCount;
        int numRenders = 0;
        int numTicks = 0;
        string fps = "";

        private Vector3[,] _points;
        private Vector2[,] _layoutPts;
        protected override void InternalRender(IRenderer g,
            DrawSettings drawSettings)
        {
            var stime = Environment.TickCount;
            var boundsInClient = new RectangleF(0, 0, 400, 400);

            EvaluateGraph(_expression,
                Interval1,
                Interval2,
                _env, ref _points);
            LayoutGraph(boundsInClient,
                _xMin, _xMax,
                _yMin, _yMax,
                _zMin, _zMax,
                _points, ref _layoutPts);
            Render3DGraph(g,
                boundsInClient,
                _pen, _brush,
                _xMin, _xMax,
                _yMin, _yMax,
                _zMin, _zMax,
                _expression,
                Interval1,
                Interval2,
                _env, true, drawSettings.Font,
                Label1,
                Label2,
                _points,
                _layoutPts);

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

            g.DrawString(fps, drawSettings.Font, LBrush.Blue,
                new Vector2(0, 0));
        }

        protected override Vector2 InternalCalcSize(IRenderer g,
            DrawSettings drawSettings)
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
            UngatherVariableForValueCollection(vars, Interval1.Variable);
            UngatherVariableForValueCollection(vars, Interval2.Variable);
        }

        public static void EvaluateGraph(
            Expression expr,
            VarInterval interval1,
            VarInterval interval2,
            SolusEnvironment env,
            ref Vector3[,] points)
        {
            int xValues = 50;
            int yValues = 50;

            if (points == null ||
                points.GetLength(0) < xValues ||
                points.GetLength(1) < yValues)
            {
                points = new Vector3[xValues, yValues];
            }

            var varMin1 = interval1.Interval.LowerBound;
            var varMax1 = interval1.Interval.UpperBound;
            var varMin2 = interval2.Interval.LowerBound;
            var varMax2 = interval2.Interval.UpperBound;
            float delta1 = (varMax1 - varMin1) / (xValues - 1);
            float delta2 = (varMax2 - varMin2) / (yValues - 1);

            int i;
            int j;
            float x;
            float y;

            var literal1 = new Literal(0);
            var literal2 = new Literal(0);
            env.SetVariable(interval1.Variable, literal1);
            env.SetVariable(interval2.Variable, literal2);

            for (i = 0; i < xValues; i++)
            {
                x = varMin1 + i * delta1;

                literal1.Value = x.ToNumber();

                for (j = 0; j < yValues; j++)
                {
                    y = varMin2 + j * delta2;
                    literal2.Value = y.ToNumber();

                    var vv = expr.Eval(env);
                    if (!vv.IsConcrete)
                        // EvaluationException ?
                        throw new OperandException(
                            "Value is not concrete");

                    Vector3 pt;
                    if (vv.IsScalar(null))
                    {
                        double value = vv.ToNumber().Value;
                        if (double.IsNaN(value))
                        {
                            value = 0;
                        }

                        pt = new Vector3(x, y, (float)value);
                    }
                    else if (vv.IsVector(null))
                    {
                        if (vv.GetVectorLength(null) != 3)
                            // EvaluationException ?
                            throw new OperandException(
                                "Value is not a 3-vector");
                        var vvv = vv.ToVector();
                        // TODO: check for NaN
                        // TODO: ensure components of vvv are scalars
                        pt = new Vector3(
                            vvv[0].ToNumber().Value,
                            vvv[1].ToNumber().Value,
                            vvv[2].ToNumber().Value);
                    }
                    else
                    {
                        throw new OperandException(
                            "Value is not a vector or scalar");
                    }

                    points[i, j] = pt;
                }
            }
        }

        public static void LayoutGraph(
            RectangleF boundsInClient,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            Vector3[,] points,
            ref Vector2[,] layoutPts)
        {
            int xValues = 50;
            int yValues = 50;

            if (layoutPts == null ||
                layoutPts.GetLength(0) < xValues ||
                layoutPts.GetLength(1) < yValues)
            {
                layoutPts = new Vector2[xValues, yValues];
            }

            int i, j;
            for (i = 0; i < xValues; i++)
            for (j = 0; j < yValues; j++)
            {
                var v = Constrain(points[i, j],
                    xMin, xMax, yMin, yMax, zMin, zMax);
                layoutPts[i, j] = ClientFromGraph(v, boundsInClient,
                    xMin, xMax, yMin, yMax, zMin, zMax);
            }
        }

        public static Vector3 Constrain(Vector3 v,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax)
        {
            if (v.X < xMin) v = new Vector3(xMin, v.Y, v.Z);
            if (v.X > xMax) v = new Vector3(xMax, v.Y, v.Z);
            if (v.Y < yMin) v = new Vector3(v.X, yMin, v.Z);
            if (v.Y > yMax) v = new Vector3(v.X, yMax, v.Z);
            if (v.Z < zMin) v = new Vector3(v.X, v.Y, zMin);
            if (v.Z > zMax) v = new Vector3(v.X, v.Y, zMax);
            return v;
        }

        public static Vector2 ClientFromGraph(Vector3 v,
            RectangleF boundsInClient,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax)
        {
            float x1 = boundsInClient.Left + boundsInClient.Width / 2;
            float y4 = boundsInClient.Bottom;
            float sx = (v.X - xMin) / (xMax - xMin);
            float sy = (v.Y - yMin) / (yMax - yMin);
            float sz = (v.Z - zMin) / (zMax - zMin);

            float xx = x1 + (sx - sy) * boundsInClient.Width * 0.5f;
            float yy = y4 - (sx + sy) * boundsInClient.Height / 4 -
                       sz * boundsInClient.Height / 2;
            return new Vector2(xx, yy);
        }

        public static void Render3DGraph(IRenderer g,
            RectangleF boundsInClient,
            LPen pen, LBrush brush,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            Expression expr,
            VarInterval interval1,
            VarInterval interval2,
            SolusEnvironment env,
            bool drawboundaries,
            LFont font,
            string label1,
            string label2,
            Vector3[,] points,
            Vector2[,] layoutPts)
        {
            int xValues = 50;
            int yValues = 50;

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

                g.DrawString(label1, font, LBrush.Black,
                    (x1 + x2) / 2, (y3 + y4) / 2);
                size = g.MeasureString(label2, font);
                g.DrawString(label2, font, LBrush.Black,
                    (x1 + x0) / 2 - size.X, (y3 + y4) / 2);

                //g.DrawRectangle(LPen.Black, boundsInClient.Left, boundsInClient.Top, boundsInClient.Width, boundsInClient.Height);
            }

            int i;
            int j;

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
                    Vector2[] poly =
                    {
                        layoutPts[i, j],
                        layoutPts[i + 1, j],
                        layoutPts[i + 1, j + 1],
                        layoutPts[i, j + 1]
                    };
                    g.FillPolygon(brush, poly);
                    //g.FillPolygon(brushes[i, j], poly);
                    g.DrawPolygon(pen, poly);
                }
            }
        }
    }
}

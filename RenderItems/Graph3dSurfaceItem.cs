using System;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Exceptions;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class Graph3dSurfaceItem : RenderItem
    {
        public Graph3dSurfaceItem(Expression expression, LPen pen, LBrush brush,
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
            _xMinLabel = xMin.ToString();
            _xMaxLabel = xMax.ToString();
            _yMinLabel = yMin.ToString();
            _yMaxLabel = yMax.ToString();
            _zMinLabel = zMin.ToString();
            _zMaxLabel = zMax.ToString();

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
        public string _xMinLabel;
        public string _xMaxLabel;
        public string _yMinLabel;
        public string _yMaxLabel;
        public string _zMinLabel;
        public string _zMaxLabel;
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
                _xMinLabel, _xMaxLabel,
                _yMinLabel, _yMaxLabel,
                _zMinLabel, _zMaxLabel,
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
            return GraphItemUtil.Constrain3d(v,
                xMin, xMax, yMin, yMax, zMin, zMax);
        }

        public static Vector2 ClientFromGraph(Vector3 v,
            RectangleF boundsInClient,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax)
        {
            return GraphItemUtil.ClientFromGraph3d(v, boundsInClient,
                xMin, xMax, yMin, yMax, zMin, zMax);
        }

        private static readonly Vector2[] _polyCache = new Vector2[4];
        public static void Render3DGraph(IRenderer g,
            RectangleF boundsInClient,
            LPen pen, LBrush brush,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            string xMinLabel, string xMaxLabel,
            string yMinLabel, string yMaxLabel,
            string zMinLabel, string zMaxLabel,
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
                GraphItemUtil.DrawBoundaries3d(g, boundsInClient,
                    xMin, xMax,
                    yMax, yMin,
                    zMin, zMax,
                    xMinLabel, xMaxLabel,
                    yMaxLabel, yMinLabel,
                    zMinLabel, zMaxLabel,
                    font,
                    label1, label2);
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
                    _polyCache[0] = layoutPts[i, j];
                    _polyCache[1] = layoutPts[i + 1, j];
                    _polyCache[2] = layoutPts[i + 1, j + 1];
                    _polyCache[3] = layoutPts[i, j + 1];

                    g.FillPolygon(brush, _polyCache);
                    //g.FillPolygon(brushes[i, j], _polyCache);
                    g.DrawPolygon(pen, _polyCache);
                }
            }
        }
    }
}

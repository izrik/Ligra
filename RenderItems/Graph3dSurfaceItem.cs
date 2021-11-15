using System;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;

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
            string label2,
            Expression color=null)
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

            _color = color;
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
        private Expression _color;

        int lastTime = Environment.TickCount;
        int numRenders = 0;
        int numTicks = 0;
        string fps = "";

        private Vector3[,] _points;
        private Vector2[,] _layoutPts;
        private Vector3[,] _colorPts;
        protected override void InternalRender(IRenderer g,
            DrawSettings drawSettings)
        {
            var stime = Environment.TickCount;
            var boundsInClient = new RectangleF(0, 0, 400, 400);
            GraphItemUtil.DrawBoundaries3d(g, boundsInClient,
                _xMin, _xMax,
                _yMax, _yMin,
                _zMin, _zMax,
                _xMinLabel, _xMaxLabel,
                _yMaxLabel, _yMinLabel,
                _zMinLabel, _zMaxLabel,
                drawSettings.Font,
                Label1, Label2);

            EvaluateGraph(_expression,
                _env, Interval1, Interval2, ref _points,
                _color, ref _colorPts);
            LayoutGraph(boundsInClient,
                _xMin, _xMax,
                _yMin, _yMax,
                _zMin, _zMax,
                _points, ref _layoutPts);
            Render3DGraph(g, _pen, _brush, _layoutPts, _colorPts);

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
            SolusEnvironment env,
            VarInterval interval1,
            VarInterval interval2,
            ref Vector3[,] points,
            Expression color,
            ref Vector3[,] colorPts,
            int numSteps1=50,
            int numSteps2=50)
        {
            if (points == null ||
                points.GetLength(0) < numSteps1 ||
                points.GetLength(1) < numSteps2)
            {
                points = new Vector3[numSteps1, numSteps2];
            }
            if (color != null)
            {
                if (colorPts == null ||
                    colorPts.GetLength(0) < numSteps1 ||
                    colorPts.GetLength(1) < numSteps2)
                    colorPts = new Vector3[numSteps1, numSteps2];
            }

            var varMin1 = interval1.Interval.LowerBound;
            var varMax1 = interval1.Interval.UpperBound;
            var varMin2 = interval2.Interval.LowerBound;
            var varMax2 = interval2.Interval.UpperBound;
            float delta1 = (varMax1 - varMin1) / (numSteps1 - 1);
            float delta2 = (varMax2 - varMin2) / (numSteps2 - 1);

            int i;
            int j;
            float x;
            float y;

            var literal1 = new Literal(0);
            var literal2 = new Literal(0);
            env.SetVariable(interval1.Variable, literal1);
            env.SetVariable(interval2.Variable, literal2);

            for (i = 0; i < numSteps1; i++)
            {
                x = varMin1 + i * delta1;

                literal1.Value = x.ToNumber();

                for (j = 0; j < numSteps2; j++)
                {
                    y = varMin2 + j * delta2;
                    literal2.Value = y.ToNumber();

                    var vv = expr.Eval(env);
                    points[i, j] = GraphItemUtil.EvaluatePoint3d(vv);

                    if (color == null) continue;
                    vv = color.Eval(env);
                    colorPts[i, j] = GraphItemUtil.EvaluatePoint3d(vv);
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
            if (layoutPts == null ||
                layoutPts.GetLength(0) < points.GetLength(0) ||
                layoutPts.GetLength(1) < points.GetLength(1))
            {
                layoutPts = new Vector2[points.GetLength(0),
                    points.GetLength(1)];
            }

            int i, j;
            for (i = 0; i < points.GetLength(0); i++)
            for (j = 0; j < points.GetLength(1); j++)
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
        public static void Render3DGraph(
            IRenderer g,
            LPen pen,
            LBrush brush,
            Vector2[,] layoutPts,
            Vector3[,] colorPts)
        {
            GraphItemUtil.RenderSurface(g, pen, brush, layoutPts, colorPts,
                _polyCache);
        }
    }
}

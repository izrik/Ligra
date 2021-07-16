using System;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Acuity;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class GraphVectorItem : RenderItem
    {
        public GraphVectorItem(Vector vector, string caption, LigraEnvironment env)
            : base(env)
        {
            _vector = vector.Clone();
            _caption = caption;
        }

        private readonly Vector _vector;
        public Vector Vector
        {
            get { return _vector; }
        }

        public readonly string _caption;

        //MemoryImage _image = null;

        protected override void InternalRender(IRenderer g, SolusEnvironment env)
        {
            RectangleF boundsInClient = new RectangleF(new PointF(0, 0), InternalCalcSize(g));
            boundsInClient.Height = 276;

            RenderVector(g, boundsInClient, LPen.Blue, LBrush.Blue, _vector, true);

            RectangleF rect = new RectangleF(10, 276, _vector.Length, g.MeasureString(_caption, _env.Font).Y);
            g.DrawString(_caption, _env.Font, LBrush.Black, rect);

            //g.DrawImage(_image.Bitmap, boundsInClient);
        }

        protected override Vector2 InternalCalcSize(IRenderer g)
        {
            double x = Math.Log(_vector.Length, 2);
            if (x < 8)
            {
                return new Vector2(276, 276);
            }
            return new Vector2(_vector.Length + 20, 296 + g.MeasureString(_caption, _env.Font, _vector.Length).Y);
        }

        protected override void AddVariablesForValueCollection(HashSet<string> vars)
        {
            //foreach (Expression expr in _vector)
            //{
            //    GatherVariablesForValueCollection(vars, expr);
            //}
        }

        public void RenderVector(IRenderer g, RectangleF boundsInClient,
            LPen pen, LBrush brush,
            SolusVector vector,
            SolusEnvironment env,
            bool drawboundaries)
        {
            double yMax = 0;
            double yMin = 0;
            int i;
            //int xMax;
            float deltaX;
            double deltaY = 1;

            float margin = 10;
            float margin2 = 2 * margin;

            double[] values = new double[vector.Length];

            for (i = 0; i < vector.Length; i++)
            {
                values[i] = vector[i].Eval(env).Value;
            }

            if (vector.Length > 0)
            {
                yMin = values[0];
                yMax = values[0];

                foreach (double val in values)
                {
                    yMin = Math.Min(yMin, val);
                    yMax = Math.Max(yMax, val);
                }

                deltaY = (boundsInClient.Height - margin2) / (yMax - yMin);
            }

            //xMax = Math.Max(400, vector.Length);
            deltaX = (boundsInClient.Width - margin2) / vector.Length;

            if (drawboundaries)
            {
                g.FillRectangle(LBrush.White, boundsInClient);
                g.DrawRectangle(LPen.Black, boundsInClient.X, boundsInClient.Y, boundsInClient.Width, boundsInClient.Height);

                if (yMax > 0 && yMin < 0)
                {
                    float y = (float)(boundsInClient.Bottom + yMin * deltaY);
                    g.DrawLine(LPen.Black, boundsInClient.Left, y, boundsInClient.Right, y);
                }
            }

            var lastPoint = boundsInClient.Location.ToVector2();

            if (vector.Length > 0)
            {
                double vvalue = values[0];
                if (double.IsNaN(vvalue))
                {
                    vvalue = 0;
                }
                vvalue = Math.Min(vvalue, yMax);
                vvalue = Math.Max(vvalue, yMin);
                double yy = boundsInClient.Bottom - (vvalue - yMin) * deltaY - margin;
                lastPoint = new Vector2(boundsInClient.Left + margin, (float)yy);
            }

            for (i = 0; i < vector.Length; i++)
            {
                float x = deltaX * i;
                double value = values[i];
                if (double.IsNaN(value))
                {
                    value = 0;
                }
                value = Math.Min(value, yMax);
                value = Math.Max(value, yMin);
                double y = boundsInClient.Bottom - (value - yMin) * deltaY - margin;

                var pt = new Vector2(x + boundsInClient.X + margin, (float)y);

                g.DrawLine(pen, lastPoint, pt);
                g.DrawLine(pen, pt.X, pt.Y, pt.X + deltaX - 1, pt.Y);

                lastPoint = pt + new Vector2(deltaX - 1, 0);
            }
        }

        public static void RenderVector(IRenderer g, RectangleF boundsInClient,
            LPen pen, LBrush brush,
            Vector vector,
            bool drawboundaries)
        {
            double yMax = 0;
            double yMin = 0;
            int i;
            //int xMax;
            float deltaX;
            double deltaY = 1;

            float margin = 10;
            float margin2 = 2 * margin;

            if (vector.Length > 0)
            {
                yMin = vector[0];
                yMax = vector[0];

                foreach (double val in vector)
                {
                    yMin = Math.Min(yMin, val);
                    yMax = Math.Max(yMax, val);
                }

                deltaY = (boundsInClient.Height - margin2) / Math.Max(yMax - yMin, 1);
            }

            //xMax = Math.Max(400, vector.Length);
            deltaX = (boundsInClient.Width - margin2) / vector.Length;

            if (drawboundaries)
            {
                g.FillRectangle(LBrush.White, boundsInClient);
                g.DrawRectangle(LPen.Black, boundsInClient.X, boundsInClient.Y, boundsInClient.Width, boundsInClient.Height);

                if (yMax > 0 && yMin < 0)
                {
                    float y = (float)(boundsInClient.Bottom + yMin * deltaY);
                    g.DrawLine(LPen.Black, boundsInClient.Left, y, boundsInClient.Right, y);
                }
            }

            var lastPoint = boundsInClient.Location.ToVector2();

            if (vector.Length > 0)
            {
                double vvalue = vector[0];
                if (double.IsNaN(vvalue))
                {
                    vvalue = 0;
                }
                vvalue = Math.Min(vvalue, yMax);
                vvalue = Math.Max(vvalue, yMin);
                double yy = boundsInClient.Bottom - (vvalue - yMin) * deltaY - margin;
                lastPoint = new Vector2(boundsInClient.Left + margin, (float)yy);
            }

            for (i = 0; i < vector.Length; i++)
            {
                float x = deltaX * i;
                double value = vector[i];
                if (double.IsNaN(value))
                {
                    value = 0;
                }
                value = Math.Min(value, yMax);
                value = Math.Max(value, yMin);
                double y = boundsInClient.Bottom - (value - yMin) * deltaY - margin;

                var pt = new Vector2(x + boundsInClient.X + margin, (float)y);

                g.DrawLine(pen, lastPoint, pt);
                g.DrawLine(pen, pt.X, pt.Y, pt.X + deltaX - 1, pt.Y);

                lastPoint = pt + new Vector2(deltaX - 1, 0);
            }
        }
    }
}

using System.Collections.Generic;
using System.Drawing;
using Gtk;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Exceptions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public static class GraphItemUtil
    {
        public static void DrawBoundaries2d(
            IRenderer renderer, RectangleF boundsInClient,
            float xMin, float xMax,
            float yMin, float yMax)
        {
            float deltaX = (xMax - xMin) / boundsInClient.Width;
            float deltaY = (yMax - yMin) / boundsInClient.Height;

            renderer.DrawRectangle(LPen.Black,
                boundsInClient.X, boundsInClient.Y,
                boundsInClient.Width, boundsInClient.Height);

            var zz = ClientFromGraph2d(Vector2.Zero,
                boundsInClient, xMin, deltaX, yMin, deltaY);
            if (xMax > 0 && xMin < 0)
            {
                renderer.DrawLine(LPen.DarkGray,
                    zz.X, boundsInClient.Top,
                    zz.X, boundsInClient.Bottom);
            }

            if (yMax > 0 && yMin < 0)
            {
                renderer.DrawLine(LPen.DarkGray,
                    boundsInClient.Left, zz.Y,
                    boundsInClient.Right, zz.Y);
            }
        }
        
        public static Vector2 ClientFromGraph2d(
            Vector2 pt,
            RectangleF boundsInClient,
            float xMin, float deltaX,
            float yMin, float deltaY)
        {
            return new Vector2(boundsInClient.X + (pt.X - xMin) / deltaX,
                boundsInClient.Bottom - (pt.Y - yMin) / deltaY);
        }

        public static Vector2 Constrain2d(Vector2 v,
            float xMin, float xMax,
            float yMin, float yMax)
        {
            if (v.X < xMin) v = new Vector2(xMin, v.Y);
            if (v.X > xMax) v = new Vector2(xMax, v.Y);
            if (v.Y < yMin) v = new Vector2(v.X, yMin);
            if (v.Y > yMax) v = new Vector2(v.X, yMax);
            return v;
        }

        public static Vector2 EvaluatePoint2d(IMathObject vv)
        {
            if (!vv.IsConcrete)
                // EvaluationException ?
                throw new OperandException(
                    "Value is not concrete");
            if (!vv.IsIsVector(null))
                throw new OperandException(
                    "Value is not a 2-vector");
            if (vv.GetVectorLength(null) != 2)
                // EvaluationException ?
                throw new OperandException(
                    "Value is not a 2-vector");
            var vvv = vv.ToVector();
            // TODO: check for NaN
            // TODO: ensure components of vvv are scalars
            return new Vector2(
                vvv[0].ToNumber().Value,
                vvv[1].ToNumber().Value);
        }

        public static void DrawBoundaries3d(
            IRenderer renderer, RectangleF boundsInClient,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            string xMinLabel, string xMaxLabel,
            string yMinLabel, string yMaxLabel,
            string zMinLabel, string zMaxLabel,
            LFont font,
            string label1, string label2)
        {
            float x0 = boundsInClient.Left;
            float x1 = boundsInClient.Left + boundsInClient.Width / 2;
            float x2 = boundsInClient.Right;

            float y0 = boundsInClient.Top;
            float y1 = boundsInClient.Top + boundsInClient.Height / 4;
            float y2 = boundsInClient.Top + boundsInClient.Height / 2;
            float y3 = boundsInClient.Top + 3 * boundsInClient.Height / 4;
            float y4 = boundsInClient.Bottom;

            Vector2 size;

            renderer.DrawLine(LPen.Black, x1, y0, x2, y1); //top right
            renderer.DrawLine(LPen.Black, x2, y1, x2, y3); //right side
            renderer.DrawLine(LPen.Black, x2, y3, x1, y4); //bottom right
            renderer.DrawLine(LPen.Black, x1, y4, x0, y3); //bottom left
            renderer.DrawLine(LPen.Black, x0, y3, x0, y1); //left side
            renderer.DrawLine(LPen.Black, x0, y1, x1, y0); //top left

            renderer.DrawLine(LPen.Black, x1, y0, x1, y2); //z axis
            renderer.DrawLine(LPen.Black, x0, y3, x1, y2); //x axis
            renderer.DrawLine(LPen.Black, x2, y3, x1, y2); //y axis

            //xmin
            renderer.DrawLine(LPen.Black, x1, y4, x1 + 6, y4 + 3);
            renderer.DrawString(xMinLabel, font, LBrush.Black,
                x1 + 6, y4 + 3);
            //xmax
            renderer.DrawLine(LPen.Black, x2, y3, x2 + 6, y3 + 3);
            renderer.DrawString(xMaxLabel, font, LBrush.Black,
                x2 + 6, y3 + 3);

            //ymin
            renderer.DrawLine(LPen.Black, x1, y4, x1 - 6, y4 + 3);
            size = renderer.MeasureString(yMinLabel, font);
            renderer.DrawString(yMinLabel, font, LBrush.Black,
                x1 - 6 - size.X, y4 + 3);
            //ymax
            renderer.DrawLine(LPen.Black, x0, y3, x0 - 6, y3 + 3);
            renderer.DrawString(yMaxLabel, font, LBrush.Black,
                x0 - 6, y3 + 3);

            //zmin
            renderer.DrawLine(LPen.Black, x2, y3, x2 + 6, y3 - 3);
            renderer.DrawString(zMinLabel, font, LBrush.Black,
                x2 + 6, y3 - 3 - 14);
            //zmax
            renderer.DrawLine(LPen.Black, x2, y1, x2 + 6, y1 - 3);
            renderer.DrawString(zMaxLabel, font, LBrush.Black,
                x2 + 6, y1 - 3);

            renderer.DrawString(label1, font, LBrush.Black,
                (x1 + x2) / 2, (y3 + y4) / 2);
            size = renderer.MeasureString(label2, font);
            renderer.DrawString(label2, font, LBrush.Black,
                (x1 + x0) / 2 - size.X, (y3 + y4) / 2);

            //renderer.DrawRectangle(LPen.Black,
            //  boundsInClient.Left, boundsInClient.Top,
            //  boundsInClient.Width, boundsInClient.Height);
        }
        public static Vector2 ClientFromGraph3d(Vector3 v,
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

        public static Vector3 Constrain3d(Vector3 v,
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

        public static Vector3 EvaluatePoint3d(IMathObject vv)
        {
            if (!vv.IsConcrete)
                // EvaluationException ?
                throw new OperandException(
                    "Value is not concrete");
            if (!vv.IsIsVector(null))
                throw new OperandException(
                    "Value is not a 3-vector");
            if (vv.GetVectorLength(null) != 3)
                // EvaluationException ?
                throw new OperandException(
                    "Value is not a 3-vector");
            var vvv = vv.ToVector();
            // TODO: check for NaN
            // TODO: ensure components of vvv are scalars
            return new Vector3(
                vvv[0].ToNumber().Value,
                vvv[1].ToNumber().Value,
                vvv[2].ToNumber().Value);
        }

        public static void RenderCurve(IRenderer renderer, LPen pen,
            Vector2[] layoutPts, Vector3[] colorPts)
        {
            int i;
            int N = layoutPts.Length;
            var lastPoint = Vector2.Zero;
            var first = true;
            for (i = 0; i < N; i++)
            {
                var next = layoutPts[i];
                // TODO: check for NaN
                if (first)
                    first = false;
                else
                {
                    var pen2 = pen;
                    if (colorPts != null)
                    {
                        var c = colorPts[i];
                        var color = new LColor(c.X, c.Y, c.Z);
                        pen2 = LPen.FromColor(color);
                    }

                    renderer.DrawLine(pen2, lastPoint, next);
                }
                lastPoint = next;
            }
        }

        public static void RenderSurface(IRenderer renderer, LPen pen,
            LBrush brush, Vector2[,] layoutPts, Vector2[] polygonPts)
        {
            int i, j;
            for (i = layoutPts.GetLength(0) - 2; i >= 0; i--)
            for (j = layoutPts.GetLength(1) - 2; j >= 0; j--)
            {
                polygonPts[0] = layoutPts[i, j];
                polygonPts[1] = layoutPts[i + 1, j];
                polygonPts[2] = layoutPts[i + 1, j + 1];
                polygonPts[3] = layoutPts[i, j + 1];

                renderer.FillPolygon(brush, polygonPts);
                renderer.DrawPolygon(pen, polygonPts);
            }
        }
    }
}

using System;
using System.Drawing;

namespace MetaphysicsIndustries.Ligra
{
    public static class Vector2Helper
    {
        public static Vector2 ToVector2(this SizeF v)
        {
            return new Vector2(v.Width, v.Height);
        }
        public static Vector2 ToVector2(this PointF v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector2[] ToVector2(this PointF[] points)
        {
            if (points == null) return null;
            var v = new Vector2[points.Length];
            int i;
            for (i = 0; i < v.Length; i++)
                v[i] = points[i].ToVector2();
            return v;
        }

        public static PointF[] ToSwf(this Vector2[] points)
        {
            if (points == null) return null;
            var v = new PointF[points.Length];
            int i;
            for (i = 0; i < v.Length; i++)
                v[i] = new PointF(points[i].X, points[i].Y);
            return v;
        }
    }
}

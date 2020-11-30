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
    }
}

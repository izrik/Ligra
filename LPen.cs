﻿using System.Collections.Generic;

namespace MetaphysicsIndustries.Ligra
{
    public class LPen
    {
        private LPen(LColor color)
        {
            Color = color;
        }

        public static LPen FromColor(LColor color)
        {
            if (!Cache.ContainsKey(color))
                Cache[color] = new LPen(color);
            return Cache[color];
        }

        private static readonly Dictionary<LColor, LPen> Cache =
            new Dictionary<LColor, LPen>();

        public static void ClearCache() => Cache.Clear();

        public readonly LColor Color;

        public LBrush Brush => LBrush.FromColor(Color);

        public System.Drawing.Pen ToSwf()
        {
            return new System.Drawing.Pen(Color.ToSwf());
        }

        public static LPen Blue = FromColor(LColor.Blue);
        public static LPen Red = FromColor(LColor.Red);
        public static LPen Green = FromColor(LColor.Green);
        public static LPen Black = FromColor(LColor.Black);
        public static LPen Yellow = FromColor(LColor.Yellow);
        public static LPen Magenta = FromColor(LColor.Magenta);
        public static LPen Cyan = FromColor(LColor.Cyan);
        public static LPen DarkGray =
            FromColor(new LColor(0.25f, 0.25f, 0.25f));
        public static LPen Gray =
            FromColor(new LColor(0.5f, 0.5f, 0.5f));
        public static LPen LightGray =
            FromColor(new LColor(0.75f, 0.75f, 0.75f));
        public static LPen White = FromColor(new LColor(1, 1, 1));
    }
}

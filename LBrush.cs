using System.Collections.Generic;

namespace MetaphysicsIndustries.Ligra
{
    public class LBrush
    {
        private LBrush(LColor color)
        {
            Color = color;
        }

        public static LBrush FromColor(LColor color)
        {
            if (!Cache.ContainsKey(color))
                Cache[color] = new LBrush(color);
            return Cache[color];
        }

        private static readonly Dictionary<LColor, LBrush> Cache =
            new Dictionary<LColor, LBrush>();

        public static void ClearCache() => Cache.Clear();

        public readonly LColor Color;

        public System.Drawing.Brush ToSwf()
        {
            return new System.Drawing.SolidBrush(Color.ToSwf());
        }

        public static LBrush Magenta = FromColor(LColor.Magenta);
        public static LBrush Black = FromColor(LColor.Black);
        public static LBrush Blue = FromColor(LColor.Blue);
        public static LBrush White = FromColor(LColor.White);
        public static LBrush Red = FromColor(LColor.Red);
        public static LBrush Green = FromColor(LColor.Green);
    }
}

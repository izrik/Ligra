using System;
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
            return new LBrush(color);
        }

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

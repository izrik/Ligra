using System;
namespace MetaphysicsIndustries.Ligra
{
    public class LBrush
    {
        public LBrush(LColor color)
        {
            Color = color;
        }

        public readonly LColor Color;

        public System.Drawing.Brush ToSwf()
        {
            return new System.Drawing.SolidBrush(Color.ToSwf());
        }

        public static LBrush Magenta = new LBrush(LColor.Magenta);
        public static LBrush Black = new LBrush(LColor.Black);
        public static LBrush Blue = new LBrush(LColor.Blue);
        public static LBrush White = new LBrush(LColor.White);
        public static LBrush Red = new LBrush(LColor.Red);
        public static LBrush Green = new LBrush(LColor.Green);
    }
}

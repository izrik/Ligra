using System;
namespace MetaphysicsIndustries.Ligra
{
    public class LPen
    {
        public LPen(LColor color)
        {
            Color = color;
        }

        public readonly LColor Color;

        public LBrush Brush => new LBrush(Color);

        public System.Drawing.Pen ToSwf()
        {
            return new System.Drawing.Pen(Color.ToSwf());
        }

        public static LPen Blue = new LPen(LColor.Blue);
        public static LPen Red = new LPen(LColor.Red);
        public static LPen Green = new LPen(LColor.Green);
        public static LPen Black = new LPen(LColor.Black);
        public static LPen Yellow = new LPen(LColor.Yellow);
        public static LPen Magenta = new LPen(LColor.Magenta);
        public static LPen Cyan = new LPen(LColor.Cyan);
        public static LPen DarkGray = new LPen(
            new LColor(0.25f, 0.25f, 0.25f));
        public static LPen Gray = new LPen(
            new LColor(0.5f, 0.5f, 0.5f));
        public static LPen LightGray = new LPen(
            new LColor(0.75f, 0.75f, 0.75f));
        public static LPen White = new LPen(new LColor(1, 1, 1));
    }
}

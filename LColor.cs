using System;
using System.Drawing;

namespace MetaphysicsIndustries.Ligra
{
    public struct LColor
    {
        public LColor(float r, float g, float b, float a=1)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public readonly float R;
        public readonly float G;
        public readonly float B;
        public readonly float A;

        public static LColor FromSwf(Color color)
        {
            return new LColor(
                ((float)color.R) / 255,
                ((float)color.G) / 255,
                ((float)color.B) / 255,
                ((float)color.A) / 255);
        }

        public Color ToSwf()
        {
            return Color.FromArgb(
                (byte)clampByte((A * 255).RoundToInt()),
                (byte)clampByte((R * 255).RoundToInt()),
                (byte)clampByte((G * 255).RoundToInt()),
                (byte)clampByte((B * 255).RoundToInt()));
        }
        static int clampByte(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        public static LColor Magenta = LColor.FromSwf(Color.Magenta);
        public static LColor Black = LColor.FromSwf(Color.Black);
        public static LColor White = LColor.FromSwf(Color.White);
        public static LColor Red = LColor.FromSwf(Color.Red);
        public static LColor Yellow = LColor.FromSwf(Color.Yellow);
        public static LColor Blue = LColor.FromSwf(Color.Blue);
        public static LColor Green = LColor.FromSwf(Color.Green);
        public static LColor Cyan = LColor.FromSwf(Color.Cyan);
    }
}

using System;
namespace MetaphysicsIndustries.Ligra
{
    public static class FloatHelper
    {
        public static int RoundToInt(this float x)
        {
            return (int)Math.Round(x);
        }
    }
}

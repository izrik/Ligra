using System;

namespace MetaphysicsIndustries.Ligra
{
    public struct Interval
    {
        public float LowerBound;
        public float UpperBound;

        public bool OpenLowerBound;
        public bool OpenUpperBound;

        public bool IsIntegerInterval;

        public static Interval Integer(int lower, int upper)
        {
            return new Interval {
                LowerBound = lower,
                UpperBound = upper,
                OpenLowerBound = false,
                OpenUpperBound = false,
                IsIntegerInterval = true,
            };
        }
    }
}


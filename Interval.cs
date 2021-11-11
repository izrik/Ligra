using System;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public struct Interval
    {
        public float LowerBound;
        public float UpperBound;
        public float Length
        {
            get { return UpperBound - LowerBound; }
        }

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

        public Interval Round()
        {
            return Integer(LowerBound.RoundInt(), UpperBound.RoundInt());
        }

        public float CalcDelta(int numSteps) =>
            (UpperBound - LowerBound) / (numSteps - 1);
    }
}


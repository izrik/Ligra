using System;

namespace MetaphysicsIndustries.Ligra
{
    public struct VarInterval
    {
        public string Variable;
        public Interval Interval;

        public override string ToString()
        {
            return string.Format(
                "{0} {1} {2} {3} {4}",
                Interval.LowerBound,
                (Interval.OpenLowerBound ? "<" : "<="),
                Variable,
                (Interval.OpenUpperBound ? "<" : "<="),
                Interval.UpperBound);
        }
    }
}


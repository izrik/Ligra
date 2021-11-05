using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra.Expressions
{
    public class TimeExpression : Expression
    {
        public override IMathObject Eval(SolusEnvironment env)
        {
            return CurrentTime.ToNumber();
        }

        private float CurrentTime => System.Environment.TickCount / 1000.0f;

        public override string ToString()
        {
            return CurrentTime.ToString();
        }

        public override Expression Clone()
        {
            return new TimeExpression();
        }

        public override void AcceptVisitor(IExpressionVisitor visitor)
        {
        }

        public override IMathObject Result => ScalarMathObject.Value;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MetaphysicsIndustries.Solus
{
    public abstract class AssociativeCommutativOperation : Operation
    {
        private static AdditionOperation _addition = new AdditionOperation();
        public static AdditionOperation Addition
        {
            get { return _addition; }
        }
        private static MultiplicationOperation _multiplication = new MultiplicationOperation();
        public static MultiplicationOperation Multiplication
        {
            get { return _multiplication; }
        }

        protected override void CheckArguments(Expression[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidOperationException("Wrong number of arguments given to " + DisplayName + " (given " + args.Length.ToString() + ", require at least 2)");
            }

            if (args.Length != Types.Count)
            {
                Types.Clear();

                foreach (Expression arg in args)
                {
                    Types.Add(typeof(Expression));
                }
            }

            base.CheckArguments(args);
        }

        public override bool IsCommutative
        {
            get
            {
                return true;
            }
        }

        public override bool IsAssociative
        {
            get
            {
                return true;
            }
        }

        public virtual bool Collapses
        {
            get { return false; }
        }

        public virtual float CollapseValue
        {
            get { return 0; }
        }

        public virtual bool Culls
        {
            get { return true; }
        }

        public virtual float CullValue
        {
            get { return IdentityValue; }
        }
    }
}

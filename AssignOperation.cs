using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus.Functions;

namespace MetaphysicsIndustries.Solus
{
    public class AssignOperation : BinaryOperation
    {
        public static readonly AssignOperation Value = new AssignOperation();

        protected AssignOperation()
        {
            Name = ":=";
//            Types.Clear();
//            Types.Add(typeof(VariableAccess));
//            Types.Add(typeof(Expression));
        }

        public override bool HasIdentityValue { get { return false; } }

        protected override float InternalBinaryCall(float x, float y)
        {
            throw new NotImplementedException();
        }

        public override OperationPrecedence Precedence
        {
            get { return OperationPrecedence.Assignment; }
        }

        public override IMathObject GetResult(IEnumerable<IMathObject> args)
        {
            throw new NotImplementedException();
        }
    }
}

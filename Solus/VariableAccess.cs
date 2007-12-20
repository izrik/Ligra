using System;
using System.Collections.Generic;
using System.Text;

namespace MetaphysicsIndustries.Solus
{
    public class VariableAccess : Expression
    {
        public VariableAccess()
        {
        }

        public VariableAccess(Variable variable)
        {
            _variable = variable;
        }

        public override Expression Clone()
        {
            return new VariableAccess(Variable);
        }

        private Variable _variable;

        public Variable Variable
        {
            get { return _variable; }
            set { _variable = value; }
        }

        public override Literal Eval(VariableTable varTable)
        {
            if (varTable.ContainsKey(Variable))
            {
                return new Literal(varTable[Variable]);
            }
            else
            {
                //return new Literal(0);
                throw new InvalidOperationException("Undefined variable in VariableAccess: " + Variable.Name);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MetaphysicsIndustries.Solus
{
    public class DerivativeOfVariable : Variable
    {
        public DerivativeOfVariable(Variable variable)
        {
            if (variable is DerivativeOfVariable)
            {
                _variable = (variable as DerivativeOfVariable).Variable;
                _order = (variable as DerivativeOfVariable).Order + 1;
            }
            else
            {
                _variable = variable;
                _order = 1;
            }
        }

        private Variable _variable;

        public Variable Variable
        {
            get { return _variable; }
        }

        private int _order;

        public int Order
        {
            get { return _order; }
        }

        public override string Name
        {
            get
            {
                return "d" + (Order > 1 ? Order.ToString() : "") + Variable.Name;
            }
            set
            {
            }
        }

    }
}

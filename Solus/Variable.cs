using System;
using System.Collections.Generic;
using System.Text;

namespace MetaphysicsIndustries.Solus
{
    public class Variable
    {
        protected Variable()
        {
        }

        public Variable(string name)
        {
            _name = name;
        }

        private string _name;

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}


/*****************************************************************************
 *                                                                           *
 *  Literal.cs                                                               *
 *  24 September 2006                                                        *
 *  Project: Solus, Ligra                                                    *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2006 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  Converted from C++ to C# on 29 October 2007                              *
 *                                                                           *
 *  A number. No fancy moving parts.                                         *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Solus
{
	public class Literal : Expression
	{
		public Literal()
		{
			_value = 0;
		}

		public Literal(float v)
		{
			_value = v;
		}

        public override Expression Clone()
        {
            return new Literal(Value);
        }

		public override Literal Eval(VariableTable varTable)
		{
			return this;
		}

		public virtual float Value
		{
			get
			{
				return _value;
			}
			set
			{
                if (_value != value)
				{
                    _value = value;
					this.OnValueChanged(new EventArgs());
				}
			}
		}

		public  event EventHandler ValueChanged;

		protected virtual void OnValueChanged(EventArgs e)
		{
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
		}

		protected  float _value;
	}
}

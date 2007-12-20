
/*****************************************************************************
 *                                                                           *
 *  FunctionCall.cs                                                          *
 *  24 September 2006                                                        *
 *  Project: Solus, Ligra                                                    *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2006 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  Converted from C++ to C# on 29 October 2007                              *
 *                                                                           *
 *  A function call, providing arguments to the function.                    *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Solus
{
    public class FunctionCall : Expression
    {
        public FunctionCall()
        {
            this.Init(null, null);
        }

        public FunctionCall(Function f, ICollection<Expression> a)
        {
            if (f == null) { throw new ArgumentNullException("f"); }
            if (a == null) { throw new ArgumentNullException("a"); }

            Expression[] aa = new Expression[a.Count];
            a.CopyTo(aa, 0);
            Init(f, aa);
        }

        public FunctionCall(Function f, params Expression[] a)
        {
            if (f == null) { throw new ArgumentNullException("f"); }

            Init(f, a);
        }

        public override void Dispose()
        {
            _arguments.Clear();
            _arguments.Dispose();
            _arguments = null;
            _function = null;
        }

        public override Expression Clone()
        {
            FunctionCall ret = new FunctionCall(Function,
                                    Array.ConvertAll<Expression, Expression>(
                                        Arguments.ToArray(), Expression.Clone));

            return ret;
        }

        public override Literal Eval(VariableTable varTable)
        {
            return Call(varTable);
        }

        public virtual Literal Call(VariableTable varTable)
        {
            return Function.Call(varTable, Arguments.ToArray());
        }

        public virtual ExpressionCollection Arguments
        {
            get
            {
                return _arguments;
            }
        }

        public Solus.Function Function
        {
            get
            {
                return _function;
            }
            set
            {
                if (_function != value)
                {
                    _function = value;
                    this.OnFunctionChanged(new EventArgs());
                }
            }
        }

        public event EventHandler FunctionChanged;

        protected virtual void OnFunctionChanged(EventArgs e)
        {
            if (FunctionChanged != null)
            {
                this.FunctionChanged(this, e);
            }
        }

        protected void Init(Function f, Expression[] a)
        {
            _function = f;

            if (a != null)
            {
                _arguments.AddRange(a);
            }
        }

        protected Function _function;
        protected ExpressionCollection _arguments = new ExpressionCollection();
    }
}

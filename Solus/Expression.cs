
/*****************************************************************************
 *                                                                           *
 *  Expression.cs                                                            *
 *  24 September 2006                                                        *
 *  Project: Solus, Ligra                                                    *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2006 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  Converted from C++ to C# on 29 October 2007                              *
 *                                                                           *
 *  The basic unit of calculation and parse trees.                           *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Solus
{
	public abstract class Expression : IDisposable, ICloneable
	{
        //public Expression()
        //{
			
        //}

        public virtual void Dispose()
        {
        }

        public abstract Literal Eval(VariableTable varTable);
        //{
        //    return new Literal(0);
        //}
        public static Literal Eval(Expression expr, VariableTable varTable)
        {
            return expr.Eval(varTable);
        }

        public abstract Expression Clone();
        public static Expression Clone(Expression expr)
        {
            return expr.Clone();
        }

        //public delegate T Transformer<T>(Expression expr, VariableTable varTable);
        //public abstract T Transform<T>(VariableTable varTable, Transformer<T> transformer);
        //public static T Transform<T>(Expression expr, VariableTable varTable, Transformer<T> transformer)
        //{
        //    expr.Transform<T>(varTable, transformer);
        //}

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}

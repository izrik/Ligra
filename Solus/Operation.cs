
/*****************************************************************************
 *                                                                           *
 *  Operation.cs                                                             *
 *  17 November 2006                                                         *
 *  Project: Solus, Ligra                                                    *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2006 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  Converted from C++ to C# on 29 October 2007                              *
 *                                                                           *
 *  A specialized function which represents simple arithmetical operations   *
 *    such as addition of two numbers.                                       *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Solus
{
	public abstract class Operation : Function
	{
        public abstract OperationPrecedence Precedence
        {
            get;
        }


        public virtual bool IsCommutative   // a @ b == b @ a
        {
            get { return false; }
        }
        public virtual bool IsAssociative   // (a @ b) @ c == a @ (b @ c)
        {
            get { return false; }
        }

        public virtual float IdentityValue
        {
            get { return 1; }
        }
    }
}

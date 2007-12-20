
/*****************************************************************************
 *                                                                           *
 *  SolusEngine.cs                                                           *
 *  17 November 2006                                                         *
 *  Project: Solus, Ligra                                                    *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2006 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  Converted from C++ to C# on 29 October 2007                              *
 *                                                                           *
 *  The central core of processing in Solus. Does some rudimentary parsing   *
 *    and evaluation and stuff.                                              *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using System.Diagnostics;

namespace MetaphysicsIndustries.Solus
{
	public class SolusEngine
	{
		public SolusEngine()
		{
			
		}

		public  void Dispose()
		{
			
		}

		public  Expression Parse(string s)
		{
			string[]	tokens;
			FunctionCall	fc;
			int				i;
			int				j;
			fc = new FunctionCall();
			tokens = s.Split(' ');
			j = tokens.Length;
			for (i = 1; i < j; i++)
			{
				float		fl;
				Expression	e;
				fl = float.Parse(tokens[i]);
				e = new Literal(fl);
				fc.Arguments.Add(e);
			}
			fc.Function = GetFunctionByName(tokens[0]);
			return fc;
		}

		public  Literal Eval(Expression e, VariableTable varTable)
        {
            return e.Eval(varTable);
		}

        public Expression PreliminaryEval(Expression expr, VariableTable varTable)
        {
            Expression evalExpr = InternalPreliminaryEval(expr, varTable);
            Expression cleanExpr = CleanUp(evalExpr);
            return cleanExpr;
        }

        protected Expression InternalPreliminaryEval(Expression expr, VariableTable varTable)
        {
            if (expr is VariableAccess)
            {
                VariableAccess varAccess = expr as VariableAccess;
                if (varTable.ContainsKey(varAccess.Variable))
                {
                    return new Literal(varTable[varAccess.Variable]);
                }
            }
            else if (expr is FunctionCall)
            {
                FunctionCall call = expr as FunctionCall;
                List<Expression> args = new List<Expression>(call.Arguments.Count);

                bool allLiterals = true;
                foreach (Expression arg in call.Arguments)
                {
                    Expression arg2 = InternalPreliminaryEval(arg, varTable);
                    if (!(arg2 is Literal))
                    {
                        allLiterals = false;
                    }
                    args.Add(arg2);
                }

                if (allLiterals)
                {
                    return call.Function.Call(varTable, args.ToArray());
                    //return call.Call(varTable);
                }
                else
                {
                    return new FunctionCall(call.Function, args.ToArray());
                }
            }

            return expr.Clone();
        }

		protected  Function GetFunctionByName(string s)
		{
			
			
			
			
			Function[]	functions = {
												Function.Cosine,
												Function.Sine,
												Function.Tangent,
												Function.Cotangent,
												Function.Secant,
												Function.Cosecant,
												Function.Arccosine,
												Function.Arcsine,
												Function.Arctangent,
												Function.Arccotangent,
												Function.Arcsecant,
												Function.Arccosecant,
												Function.Floor,
												Function.Ceiling,
											};
			foreach (Function f in functions)
			{
				if (s == f.Name)
				{
					return f;
				}
			}
            foreach (Function f in functions)
            {
                if (s == f.DisplayName)
                {
                    return f;
                }
            }
			return null;
		}


        public Expression GetDerivative(Expression expr)
        {
            Expression derivative;

            derivative = InternalGetDerivative(expr);

            derivative = CleanUp(derivative);

            return derivative;
        }

        public Expression CleanUp(Expression expr)
        {
            if (expr is Literal)
            {
                return CleanUpLiteral(expr as Literal);
            }
            else if (expr is VariableAccess)
            {
                return CleanUpVariableAccess(expr as VariableAccess);
            }
            else if (expr is FunctionCall)
            {
                return CleanUpFunctionCall((expr as FunctionCall).Function, (expr as FunctionCall).Arguments.ToArray());
            }
            else
            {
                throw new InvalidOperationException("Unknown expression: " + expr.ToString());
            }
        }

        private Expression CleanUpLiteral(Literal literal)
        {
            return literal;
        }

        private Expression CleanUpFunctionCall(Function function, Expression[] args)
        {
            List<Expression> cleanArgs = new List<Expression>(args.Length);
            foreach (Expression arg in args)
            {
                cleanArgs.Add(CleanUp(arg));
            }
            args = cleanArgs.ToArray();

            if (function is Operation)
            {
                return CleanUpOperation(function as Operation, args);
            }
            else
            {
                return new FunctionCall(function, args);
            }
        }

        private Expression CleanUpOperation(Operation operation, Expression[] args)
        {
            if (operation is BinaryOperation)
            {
                return CleanUpBinaryOperation(operation as BinaryOperation, args);
            }
            else if (operation is AssociativeCommutativOperation)
            {
                return CleanUpAssociativeCommutativOperation(operation as AssociativeCommutativOperation, args);
            }
            //else if (operation == AssociativeCommutativOperation.Addition)
            //{
            //    return CleanUpAdditionOperation(operation as AdditionOperation, args);
            //}
            else
            {
                throw new InvalidOperationException("Unknown operation: " + operation.ToString());
            }
        }

        private Expression CleanUpAssociativeCommutativOperation(AssociativeCommutativOperation operation, Expression[] args)
        {
            args = CleanUpPartAssociativeOperation(operation, args);

            if (operation.Collapses)
            {
                foreach (Expression arg in args)
                {
                    if (arg is Literal && (arg as Literal).Value == operation.CollapseValue)
                    {
                        return new Literal(operation.CollapseValue);
                    }
                }
            }

            if (operation.Culls)
            {
                List<Expression> args2 = new List<Expression>(args.Length);
                foreach (Expression arg in args)
                {
                    if (!(arg is Literal) || (arg as Literal).Value != operation.CullValue)
                    {
                        args2.Add(arg);
                    }
                }

                if (args2.Count < args.Length)
                {
                    args = args2.ToArray();
                }
            }

            if (args.Length == 1)
            {
                return args[0];
            }

            return new FunctionCall(operation, args);
        }

        private Expression CleanUpBinaryOperation(BinaryOperation binaryOperation, Expression[] args)
        {
            if (args[0] is Literal &&
                args[1] is Literal)
            {
                return binaryOperation.Call(null, args);    //null = living on the edge
            }

            if (binaryOperation.IsAssociative)
            {
                args = CleanUpPartAssociativeOperation(binaryOperation, args);
            }

            //List<Expression> cleanArgs = new List<Expression>(args.Length);
            //foreach (Expression arg in args)
            //{
            //    cleanArgs.Add(CleanUp(arg));
            //}
            //functionCall = new FunctionCall(operation, cleanArgs.ToArray());

            if (binaryOperation.IsCommutative &&
                args[0] is Literal &&
                (args[0] as Literal).Value == binaryOperation.IdentityValue)
            {
                return args[1];
            }

            if (args[1] is Literal &&
                (args[1] as Literal).Value == binaryOperation.IdentityValue)
            {
                return args[0];
            }

            //if (operation == AssociativeCommutativOperation.Addition)
            //{
            //    return CleanUpAdditionOperation(operation as AdditionOperation);
            //}
            //else if (operation == AssociativeCommutativOperation.Multiplication)
            //{
            //    return CleanUpMultiplicationOperation(functionCall);
            //}
            //else if (operation == BinaryOperation.Division)
            //{
            //    return CleanUpDivisionOperation(functionCall);
            //}
            //else if (operation == BinaryOperation.Exponent)
            //{
            //    return CleanUpExponentOperation(functionCall);
            //}
            //else
            //{
            //    throw new InvalidOperationException("Unknown binary operation: " + operation.ToString());
            //}

            return new FunctionCall(binaryOperation, args);
        }

        private Expression[] CleanUpPartAssociativeOperation(Operation operation, Expression[] args)
        {
            List<FunctionCall> assocOps = new List<FunctionCall>();
            GatherMatchingFunctionCalls(new FunctionCall(operation, args), assocOps);

            Set<FunctionCall> assocOpsSet = new Set<FunctionCall>(assocOps);
            Literal combinedLiteral = null;

            combinedLiteral = new Literal(operation.IdentityValue);

            List<Expression> nonLiterals = new List<Expression>(assocOps.Count);

            foreach (FunctionCall opToCombine in assocOps)
            {
                foreach (Expression arg in opToCombine.Arguments)
                {
                    if (!(arg is FunctionCall) ||
                        !(assocOpsSet.Contains(arg as FunctionCall)))
                    {
                        if (arg is Literal)
                        {
                            combinedLiteral = operation.Call(null, combinedLiteral, arg);
                        }
                        else
                        {
                            nonLiterals.Add(arg);
                        }
                    }
                }
            }

            if (operation is AssociativeCommutativOperation)
            {
                List<Expression> newArgs = new List<Expression>(nonLiterals.Count + 1);
                newArgs.Add(combinedLiteral);
                newArgs.AddRange(nonLiterals);
                args = newArgs.ToArray();
            }
            else if (operation is BinaryOperation)
            {
                FunctionCall ret = new FunctionCall(operation, combinedLiteral);
                FunctionCall temp = ret;
                FunctionCall last = null;

                foreach (Expression expr in nonLiterals)
                {
                    //Expression cleanExpr = CleanUp(expr);
                    last = temp;
                    temp = new FunctionCall(operation, expr);//cleanExpr);
                    last.Arguments.Add(temp);
                }

                last.Arguments[1] = temp.Arguments[0];

                args = ret.Arguments.ToArray();
            }
            else
            {
                throw new InvalidOperationException("Unknown operation: " + operation.ToString());
            }

            return args;
        }

        private void GatherMatchingFunctionCalls(FunctionCall functionCall, ICollection<FunctionCall> matchingFunctionCalls)
        {
            bool first = true;

            foreach (Expression arg in functionCall.Arguments)
            {
                if (arg is FunctionCall &&
                    (arg as FunctionCall).Function == functionCall.Function)
                {
                    FunctionCall argCall = arg as FunctionCall;
                    GatherMatchingFunctionCalls(argCall, matchingFunctionCalls);
                }

                if (first)
                {
                    matchingFunctionCalls.Add(functionCall);
                    first = false;
                }
            }
        }

        //private Expression CleanUpAdditionOperation(AdditionOperation additionOperation, Expression[] args)
        //{
        //    return new FunctionCall(additionOperation, args);
        //}

        private Expression CleanUpMultiplicationOperation(MultiplicationOperation multiplicationOperation, Expression[] args)
        {
            return new FunctionCall(multiplicationOperation, args);
        }

        private Expression CleanUpDivisionOperation(DivisionOperation divisionOperation, Expression[] args)
        {
            return new FunctionCall(divisionOperation, args);
        }

        private Expression CleanUpExponentOperation(ExponentOperation exponentOperation, Expression[] args)
        {
            return new FunctionCall(exponentOperation, args);
        }

        private Expression CleanUpVariableAccess(VariableAccess variableAccess)
        {
            return variableAccess;
        }

        private Expression InternalGetDerivative(Expression expr)
        {
            Expression derivative;

            if (expr is Literal)
            {
                derivative = new Literal(0);
            }
            else if (expr is VariableAccess)
            {
                derivative = GetDerivativeOfVariableAccess(expr as VariableAccess);
            }
            else if (expr is FunctionCall)
            {
                derivative = GetDerivativeOfFunctionCall(expr as FunctionCall);
            }
            else
            {
                throw new InvalidOperationException("Unknown kind of expression: " + expr.ToString());
            }

            return derivative;
        }

        protected Expression GetDerivativeOfFunctionCall(FunctionCall functionCall)
        {
            if (functionCall.Function is Operation)
            {
                return GetDerivativeOfOperation(functionCall);
            }
            else if (functionCall.Arguments.Count == 1)
            {
                Expression functionDerivative;
                Expression argumentDerivative;

                Function function = functionCall.Function;

                if (function == Function.Cosine)
                {
                    functionDerivative = new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new Literal(-1),
                        new FunctionCall(Function.Sine, functionCall.Arguments[0]));
                }
                else if (function == Function.Sine)
                {
                    functionDerivative = new FunctionCall(Function.Cosine, functionCall.Arguments[0]);
                }
                else
                {
                    throw new NotImplementedException();
                }

                if (functionDerivative is Literal && (functionDerivative as Literal).Value == 0)
                {
                    return functionDerivative;
                }

                argumentDerivative = GetDerivative(functionCall.Arguments[0]);

                if (argumentDerivative is Literal && (argumentDerivative as Literal).Value == 0)
                {
                    return argumentDerivative;
                }

                return new FunctionCall(AssociativeCommutativOperation.Multiplication, functionDerivative, argumentDerivative);

            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected Expression GetDerivativeOfOperation(FunctionCall functionCall)
        {
            if (functionCall.Function is BinaryOperation)
            {
                return GetDerivativeOfBinaryOperation(functionCall);
            }
            else if (functionCall.Function is AssociativeCommutativOperation)
            {
                return GetDerivativeOfAssociativeCommutativOperation(functionCall);
            }
            else
            {
                throw new InvalidOperationException("Unknown operation: " + functionCall.Function.ToString());
            }
        }

        private Expression GetDerivativeOfAssociativeCommutativOperation(FunctionCall functionCall)
        {
            if (functionCall.Function == AssociativeCommutativOperation.Addition)
            {
                return GetDerivativeOfAdditionOperation(functionCall);
            }
            else if (functionCall.Function == AssociativeCommutativOperation.Multiplication)
            {
                return GetDerivativeOfMultiplicationOperation(functionCall);
            }
            else
            {
                throw new InvalidOperationException("Unknown operation: " + functionCall.Function.ToString());
            }
        }

        protected Expression GetDerivativeOfBinaryOperation(FunctionCall functionCall)
        {
            BinaryOperation binaryOperation = functionCall.Function as BinaryOperation;

            if (binaryOperation == BinaryOperation.Division)
            {
                return GetDerivativeOfDivisionOperation(functionCall);
            }
            else if (binaryOperation == BinaryOperation.Exponent)
            {
                return GetDerivativeOfExponentOperation(functionCall);
            }
            else
            {
                throw new InvalidOperationException("Unknown binary operation: " + functionCall.Function.ToString());
            }
        }

        private Expression GetDerivativeOfExponentOperation(FunctionCall functionCall)
        {
            Expression leftClone = functionCall.Arguments[0].Clone();
            Expression rightClone = functionCall.Arguments[1].Clone();
            Expression leftDerivative = GetDerivative(leftClone);
            Expression rightDerivative = GetDerivative(rightClone);

            if (leftClone is Literal && (leftClone as Literal).Value == 0)
            {
                return leftClone;
            }

            bool leftDerivZero = (leftDerivative is Literal && (leftDerivative as Literal).Value == 0);
            bool rightDerivZero = (rightDerivative is Literal && (rightDerivative as Literal).Value == 0);
            bool rightCloneZero = (rightClone is Literal && (rightClone as Literal).Value == 0);

            if ((leftDerivZero || rightCloneZero) && rightDerivZero)
            {
                return leftDerivative;
            }

            Expression rightMinusOne;
            if (rightClone is Literal)
            {
                rightMinusOne = rightClone.Clone();
                (rightMinusOne as Literal).Value--;
            }
            else
            {
                rightMinusOne =
                    new FunctionCall(AssociativeCommutativOperation.Addition,
                        rightClone.Clone(),
                        new Literal(-1));
            }

            Expression leftSide = null;

            if ((!leftDerivZero) && (!rightCloneZero))
            {
                Expression newExponent;

                if (rightMinusOne is Literal && (rightMinusOne as Literal).Value == 1)
                {
                    newExponent = leftClone.Clone();
                }
                else
                {
                    newExponent =
                        new FunctionCall(BinaryOperation.Exponent,
                            leftClone.Clone(),
                            rightMinusOne);
                }

                leftSide =
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new FunctionCall(AssociativeCommutativOperation.Multiplication,
                            rightClone,
                            newExponent),
                        leftDerivative);
            }

            if (rightDerivZero)
            {
                return leftSide;
            }

            Expression rightSide =
                new FunctionCall(AssociativeCommutativOperation.Multiplication,
                    new FunctionCall(Function.NaturalLogarithm,
                        leftClone.Clone()),
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new FunctionCall(BinaryOperation.Exponent,
                            leftClone.Clone(),
                            rightClone.Clone()),
                        rightDerivative));

            if (leftDerivZero || rightCloneZero)
            {
                return rightSide;
            }

            return new FunctionCall(AssociativeCommutativOperation.Addition,
                leftSide,
                rightSide);
        }

        private Expression GetDerivativeOfDivisionOperation(FunctionCall functionCall)
        {
            Expression highClone = functionCall.Arguments[0].Clone();
            Expression lowClone = functionCall.Arguments[1].Clone();
            Expression highDerivative = GetDerivative(highClone);
            Expression lowDerivative = GetDerivative(lowClone);

            return new FunctionCall(BinaryOperation.Division,
                        new FunctionCall(AssociativeCommutativOperation.Addition,
                            new FunctionCall(AssociativeCommutativOperation.Multiplication,
                                highDerivative,
                                lowClone),
                            new FunctionCall(AssociativeCommutativOperation.Multiplication,
                                new Literal(-1),
                                new FunctionCall(AssociativeCommutativOperation.Multiplication,
                                    highClone,
                                    lowDerivative))),
                        new FunctionCall(BinaryOperation.Exponent,
                            lowClone,
                            new Literal(2)));
        }

        private Expression GetDerivativeOfAdditionOperation(FunctionCall functionCall)
        {
            List<Expression> derivatives = new List<Expression>();

            foreach (Expression arg in functionCall.Arguments)
            {
                Expression deriv = GetDerivative(arg);

                if (!(deriv is Literal) || (deriv as Literal).Value != 0)
                {
                    derivatives.Add(deriv);
                }
            }

            if (derivatives.Count > 0)
            {
                return new FunctionCall(AssociativeCommutativOperation.Addition, derivatives.ToArray());
            }
            else
            {
                return new Literal(0);
            }
        }

        private Expression GetDerivativeOfMultiplicationOperation(FunctionCall functionCall)
        {
            MultiplicationOperation operation = functionCall.Function as MultiplicationOperation;

            List<Expression> args = functionCall.Arguments;

            if (operation.Collapses)
            {
                //we may expand this optimization to other 
                //functions or operations in the future, hence 
                //the use of Collapses and CollapseValue instead
                //of just 0
                foreach (Expression arg in args)
                {
                    if (arg is Literal && (arg as Literal).Value == operation.CollapseValue)
                    {
                        return new Literal(operation.CollapseValue);
                    }
                }
            }

            Dictionary<Expression, Expression> clones = new Dictionary<Expression, Expression>();
            Dictionary<Expression, Expression> derivs = new Dictionary<Expression, Expression>();

            foreach (Expression arg in args)
            {
                clones[arg] = arg.Clone();
                derivs[arg] = GetDerivative(arg);
            }

            List<Expression> addTerms = new List<Expression>(args.Count);
            List<Expression> multTerm = new List<Expression>(args.Count);

            foreach (Expression argd in args)
            {
                multTerm.Clear();

                foreach (Expression arg in args)
                {
                    if (argd == arg)
                    {
                        Expression deriv = derivs[argd];
                        if (operation.Collapses && deriv is Literal &&
                            (deriv as Literal).Value == operation.CollapseValue)
                        {
                            multTerm.Clear();
                            break;
                        }
                        else if (operation.Culls && deriv is Literal &&
                                (deriv as Literal).Value == operation.CullValue)
                        {
                            //a*1 = a, do nothing
                        }
                        else
                        {
                            multTerm.Add(deriv);
                        }
                    }
                    else
                    {
                        multTerm.Add(clones[arg]);
                    }
                }

                if (multTerm.Count >= 2)
                {
                    addTerms.Add(new FunctionCall(AssociativeCommutativOperation.Multiplication, multTerm.ToArray()));
                }
                else if (multTerm.Count == 1)
                {
                    addTerms.Add(multTerm[0]);
                }
            }

            if (addTerms.Count >= 2)
            {
                return new FunctionCall(AssociativeCommutativOperation.Addition, addTerms.ToArray());
            }
            else if (addTerms.Count == 1)
            {
                return addTerms[0];
            }
            else
            {
                return new Literal(AssociativeCommutativOperation.Addition.IdentityValue);
            }
            
        

            //Debug.WriteLine("GetDerivativeOfMultiplicationOperation is assuming two arguments");

            //Expression leftClone = functionCall.Arguments[0].Clone();

            //if (leftClone is Literal && (leftClone as Literal).Value == 0)
            //{
            //    return leftClone;
            //}

            //Expression rightClone = functionCall.Arguments[1].Clone();

            //if (rightClone is Literal && (rightClone as Literal).Value == 0)
            //{
            //    return rightClone;
            //}

            //Expression leftDerivative = GetDerivative(leftClone);

            ////if (

            //Expression rightDerivative = GetDerivative(rightClone);

            //bool leftDerivZero = (leftDerivative is Literal && (leftDerivative as Literal).Value == 0);
            //bool rightDerivZero = (rightDerivative is Literal && (rightDerivative as Literal).Value == 0);

            //if (leftDerivZero && rightDerivZero)
            //{
            //    return leftDerivative;
            //}
            //else if (leftDerivZero)
            //{
            //    return new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //                leftClone,
            //                rightDerivative);
            //}
            //else if (rightDerivZero)
            //{
            //    return new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //                leftDerivative,
            //                rightClone);
            //}

            //return new FunctionCall(AssociativeCommutativOperation.Addition,
            //            new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //                leftDerivative,
            //                rightClone),
            //            new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //                leftClone,
            //                rightDerivative));
        }

        protected Expression GetDerivativeOfVariableAccess(VariableAccess variableAccess)
        {
            //if (variableAccess.Variable is DerivativeOfVariable)
            //{
            //    return new Literal(0);
            //}
            //else
            {
                return new VariableAccess(new DerivativeOfVariable(variableAccess.Variable));
            }
        }
    }
}

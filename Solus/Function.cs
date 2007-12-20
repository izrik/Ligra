
/*****************************************************************************
 *                                                                           *
 *  Function.cs                                                              *
 *  24 September 2006                                                        *
 *  Project: Solus, Ligra                                                    *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2006 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  Converted from C++ to C# on 29 October 2007                              *
 *                                                                           *
 *  A mathematical function that can be evaluated with a set of              *
 *    parameters. This serves as a base class that is inherited by other,    *
 *    specialized classes, each representing a different mathematical        *
 *    function (e.g. "SineFunction : Function"). This base class performs    *
 *    all necessary type checking on given arguments based on information    *
 *    specified by the derived class.                                        *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Solus
{
	public abstract class Function : IDisposable
	{
        //static Function()
        //{
        //}

        private static ArccosecantFunction _arccosecant = new ArccosecantFunction();
        public static Function Arccosecant
        {
            get { return _arccosecant; }
        }
        private static ArccosineFunction _arccosine = new ArccosineFunction();
        public static Function Arccosine
        {
            get { return _arccosine; }
        }
        private static ArccotangentFunction _arccotangent = new ArccotangentFunction();
        public static Function Arccotangent
        {
            get { return _arccotangent; }
        }
        private static ArcsecantFunction _arcsecant = new ArcsecantFunction();
        public static Function Arcsecant
        {
            get { return _arcsecant; }
        }
        private static ArcsineFunction _arcsine = new ArcsineFunction();
        public static Function Arcsine
        {
            get { return _arcsine; }
        }
        private static ArctangentFunction _arctangent = new ArctangentFunction();
        public static Function Arctangent
        {
            get { return _arctangent; }
        }
        private static CeilingFunction _ceiling = new CeilingFunction();
        public static Function Ceiling
        {
            get { return _ceiling; }
        }
        private static CosecantFunction _cosecant = new CosecantFunction();
        public static Function Cosecant
        {
            get { return _cosecant; }
        }
        private static CosineFunction _cosine = new CosineFunction();
        public static Function Cosine
        {
            get { return _cosine; }
        }
        private static CotangentFunction _cotangent = new CotangentFunction();
        public static Function Cotangent
        {
            get { return _cotangent; }
        }
        private static FloorFunction _floor = new FloorFunction();
        public static Function Floor
        {
            get { return _floor; }
        }
        private static SecantFunction _secant = new SecantFunction();
        public static Function Secant
        {
            get { return _secant; }
        }
        private static SineFunction _sine = new SineFunction();
        public static Function Sine
        {
            get { return _sine; }
        }
        private static TangentFunction _tangent = new TangentFunction();
        public static Function Tangent
        {
            get { return _tangent; }
        }
        private static NaturalLogarithmFunction _naturalLogarithm = new NaturalLogarithmFunction();
        public static NaturalLogarithmFunction NaturalLogarithm
        {
            get { return _naturalLogarithm; }
        }
        private static UnitStepFunction _unitStep = new UnitStepFunction();
        public static UnitStepFunction UnitStep
        {
            get { return _unitStep; }
        }


        public Function()
        {
        }

        public Function(string name)
        {
            _name = name;
        }

		public virtual void Dispose()
		{
			
		}

		public virtual Literal Call(VariableTable varTable, params Expression[] args)
        {
            this.CheckArguments(args);
            List<Literal> evalArgs = new List<Literal>(args.Length);
            foreach (Expression arg in args)
            {
                evalArgs.Add(arg.Eval(varTable));
            }
			return this.InternalCall(varTable, evalArgs.ToArray());
		}


		public virtual string DisplayName
		{
			get
			{
				return Name;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			protected set
			{
				if (_name != value)
				{
					_name = value;
				}
			}
		}

		public List<Type> Types
		{
			get
			{
				return InternalTypes;
			}
		}

        protected abstract Literal InternalCall(VariableTable varTable, Literal[] args);
        //{
        //    //make this abstract?

        //    throw new NotImplementedException();
        //}

		protected virtual void CheckArguments(Expression[] args)
		{
			Expression[] _args = args;
			List<Type>	types;
			int				i;
			int				j;
			
			types = Types;
			if (args.Length != types.Count)
			{
				throw new InvalidOperationException("Wrong number of arguments given to " + DisplayName + " (given " + args.Length.ToString() + ", require " + Types.Count.ToString() + ")");
			}
			Type	e;
			e = typeof(Expression);
			j = args.Length;
			for (i = 0; i < j; i++)
			{
				if (!e.IsAssignableFrom(types[i]))
				{
					
					throw new InvalidOperationException("Required argument type " + i.ToString() + " is invalid (given \"" + types[i].Name + "\", require \"" + e.Name + ")");
				}
				if (!types[i].IsAssignableFrom(args[i].GetType()))
				{
					throw new InvalidOperationException("Argument " + ((i).ToString()) + " of wrong type (given \"" + args.GetType().Name + "\", require \"" + types[i].Name + ")");
				}
			}
		}

		protected List<Type> InternalTypes
		{
			get
			{
				return _internaltypes;
			}
		}

		private  List<Type> _internaltypes = new List<Type>();
		private  string     _name;
	}
}

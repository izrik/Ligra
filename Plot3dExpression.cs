using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;

namespace MetaphysicsIndustries.Solus
{
    public class Plot3dExpression : Expression
    {
        //public Plot3dExpression(Variable independentVariableX, Variable independentVariableY, Expression expressionToPlot)
        //    : this(independentVariableX, independentVariableY, expressionToPlot, Pens.Black, Brushes.Green)
        //{
        //}
        public Plot3dExpression(
            string independentVariableX,
            string independentVariableY,
            Expression expressionToPlot,
            float xMin, float xMax,
            float yMin, float yMax,
            float zMin, float zMax,
            Pen wirePen, Brush fillBrush)
        {
            _independentVariableX = independentVariableX;
            _independentVariableY = independentVariableY;
            _expressionToPlot = expressionToPlot;
            _wirePen = wirePen;
            _fillBrush = fillBrush;
            _xMin = xMin;
            _xMax = xMax;
            _yMin = yMin;
            _yMax = yMax;
            _zMin = zMin;
            _zMax = zMax;
        }

        private string _independentVariableX;
        public string IndependentVariableX
        {
            get { return _independentVariableX; }
            set { _independentVariableX = value; }
        }

        private string _independentVariableY;
        public string IndependentVariableY
        {
            get { return _independentVariableY; }
            set { _independentVariableY = value; }
        }

        private Expression _expressionToPlot;
        public Expression ExpressionToPlot
        {
            get { return _expressionToPlot; }
            set { _expressionToPlot = value; }
        }


        private Pen _wirePen;
        public Pen WirePen
        {
            get { return _wirePen; }
            set { _wirePen = value; }
        }

        private Brush _fillBrush;
        public Brush FillBrush
        {
            get { return _fillBrush; }
            set { _fillBrush = value; }
        }

        private float _xMin;
        public float XMin
        {
            get { return _xMin; }
            set { _xMin = value; }
        }
        private float _xMax;
        public float XMax
        {
            get { return _xMax; }
            set { _xMax = value; }
        }
        private float _yMin;
        public float YMin
        {
            get { return _yMin; }
            set { _yMin = value; }
        }
        private float _yMax;
        public float YMax
        {
            get { return _yMax; }
            set { _yMax = value; }
        }
        private float _zMin;
        public float ZMin
        {
            get { return _zMin; }
            set { _zMin = value; }
        }
        private float _zMax;
        public float ZMax
        {
            get { return _zMax; }
            set { _zMax = value; }
        }


        public override Literal Eval(SolusEnvironment env)
        {
            if (ExpressionToPlot != null)
            {
                return ExpressionToPlot.Eval(env);
            }
            else
            {
                return new Literal(0);
            }
        }

        public override Expression Clone()
        {
            return new Plot3dExpression(
                IndependentVariableX,
                IndependentVariableY,
                ExpressionToPlot,
                XMin, XMax,
                YMin, YMax,
                ZMin, ZMax,
                WirePen, FillBrush);
        }

        public static Brush GetBrushFromExpression(Expression expression, SolusEnvironment env)
        {
            if (expression is ColorExpression)
            {
                return ((ColorExpression)expression).Brush;
            }
            else //if (arg is Literal)
            {
                float value = expression.Eval(env).Value;// ((Literal)arg).Value;
                int iValue = (int)(value);
                Color color = Color.FromArgb(255, Color.FromArgb(iValue));
                return new SolidBrush(color);
            }
        }

        public static Pen GetPenFromExpression(Expression arg, SolusEnvironment env)
        {
            if (arg is ColorExpression)
            {
                return ((ColorExpression)arg).Pen;
            }
            else //if (arg is Literal)
            {
                float value = arg.Eval(env).Value;// ((Literal)arg).Value;
                int iValue = (int)(value);
                Color color = Color.FromArgb(255, Color.FromArgb(iValue));
                return new Pen(color);
            }
        }

        public class Plot3dMacro : Macro
        {
            public static readonly Plot3dMacro Value = new Plot3dMacro();

            protected Plot3dMacro()
            {
                Name = "plot3d";
                HasVariableNumArgs = true;
            }

            public override Expression InternalCall(IEnumerable<Expression> _args, SolusEnvironment env)
            {
                List<Expression> args = _args.ToList();

                if (args.Count < 3 ||
                    !(args[0] is VariableAccess) ||
                    !(args[1] is VariableAccess))
                {
                    throw new SolusParseException(-1, "Plot command requires two variables and one expression to plot");
                }

                if ((args.Count > 5 && args.Count < 9) ||
                    args.Count == 10 ||
                    args.Count > 11)
                {
                    throw new SolusParseException(-1, "Incorrect number of arguments");
                }

                Brush fillBrush = Brushes.Green;
                Pen wirePen = Pens.Black;
                float xMin = -4;
                float xMax = 4;
                float yMin = -4;
                float yMax = 4;
                float zMin = -2;
                float zMax = 6;

                if (args.Count == 4 || args.Count == 5)
                {
                    fillBrush = GetBrushFromExpression(args[3], env);

                    if (args.Count == 5)
                    {
                        wirePen = GetPenFromExpression(args[4], env);
                    }
                }
                else if (args.Count == 9)
                {
                    //3 --> xMin

                    xMin = args[3].Eval(env).Value;
                    xMax = args[4].Eval(env).Value;
                    yMin = args[5].Eval(env).Value;
                    yMax = args[6].Eval(env).Value;
                    zMin = args[7].Eval(env).Value;
                    zMax = args[8].Eval(env).Value;
                }
                else if (args.Count == 11)
                {
                    xMin = args[3].Eval(env).Value;
                    xMax = args[4].Eval(env).Value;
                    yMin = args[5].Eval(env).Value;
                    yMax = args[6].Eval(env).Value;
                    zMin = args[7].Eval(env).Value;
                    zMax = args[8].Eval(env).Value;
                    fillBrush = GetBrushFromExpression(args[9], env);
                    wirePen = GetPenFromExpression(args[10], env);
                }
                else if (args.Count != 3)
                {
                    throw new SolusParseException(-1, "Incorrect number of arguments");
                }

                return new Plot3dExpression(
                    ((VariableAccess)args[0]).VariableName,
                    ((VariableAccess)args[1]).VariableName,
                    args[2],
                    xMin, xMax,
                    yMin, yMax,
                    zMin, zMax,
                    wirePen, fillBrush);
            }
        }
    }
}

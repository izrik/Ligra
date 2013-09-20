using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MetaphysicsIndustries.Solus
{
    public class MathPaintExpression : Expression
    {
        public MathPaintExpression(
            Variable horizontalCoordinate, Variable verticalCoordinate,
            int width, int height,
            Expression expression
            )
        {
            _expression = expression;
            _horizontalCoordinate = horizontalCoordinate;
            _verticalCoordinate = verticalCoordinate;
            _width = width;
            _height = height;
        }


        private Expression _expression;
        public Expression Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }
        private Variable _horizontalCoordinate;
        public Variable HorizontalCoordinate
        {
            get { return _horizontalCoordinate; }
            set { _horizontalCoordinate = value; }
        }
        private Variable _verticalCoordinate;
        public Variable VerticalCoordinate
        {
            get { return _verticalCoordinate; }
            set { _verticalCoordinate = value; }
        }
        private int _width;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        private int _height;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public override Literal Eval(VariableTable varTable)
        {
            if (Expression != null)
            {
                return Expression.Eval(varTable);
            }
            else
            {
                return new Literal(0);
            }
        }

        public override Expression Clone()
        {
            return new MathPaintExpression(
                HorizontalCoordinate,
                VerticalCoordinate,
                Width, Height,
                Expression
                );
        }

        public static Expression Convert(IEnumerable<Expression> _args, VariableTable varTable)
        {
            List<Expression> args = _args.ToList();

            if (!(args[0] is VariableAccess))
            {
                throw new SolusParseException(null, "First argument to MathPaint command must be a variable reference.");
            }
            if (!(args[1] is VariableAccess))
            {
                throw new SolusParseException(null, "Second argument to MathPaint command must be a variable reference.");
            }

            return new MathPaintExpression(
                ((VariableAccess)args[0]).Variable,
                ((VariableAccess)args[1]).Variable,
                (int)(args[2].Eval(varTable).Value),
                (int)(args[3].Eval(varTable).Value),
                args[4]);
        }
    }
}
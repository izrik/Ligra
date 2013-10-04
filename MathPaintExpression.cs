using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MetaphysicsIndustries.Solus
{
    public class MathPaintExpression : Expression
    {
        public MathPaintExpression(
            string horizontalCoordinate,
            string verticalCoordinate,
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
        private string _horizontalCoordinate;
        public string HorizontalCoordinate
        {
            get { return _horizontalCoordinate; }
            set { _horizontalCoordinate = value; }
        }
        private string _verticalCoordinate;
        public string VerticalCoordinate
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
                throw new SolusParseException(-1, "First argument to MathPaint command must be a variable reference.");
            }
            if (!(args[1] is VariableAccess))
            {
                throw new SolusParseException(-1, "Second argument to MathPaint command must be a variable reference.");
            }

            return new MathPaintExpression(
                ((VariableAccess)args[0]).VariableName,
                ((VariableAccess)args[1]).VariableName,
                (int)(args[2].Eval(varTable).Value),
                (int)(args[3].Eval(varTable).Value),
                args[4]);
        }
    }
}

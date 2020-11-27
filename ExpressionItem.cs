using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus;
using System.Drawing;
using MetaphysicsIndustries.Acuity;
using Gtk;

namespace MetaphysicsIndustries.Ligra
{
    public class ExpressionItem : RenderItem
    {
        public ExpressionItem(Expression expression, Pen pen, Font font, LigraEnvironment env)
            : base(env)
        {
            _expression = expression;
            _pen = pen;
            _font = font;
        }
        public ExpressionItem(Vector vector, Pen pen, Font font, LigraEnvironment env)
            : this(GenerateVector(vector), pen, font, env)
        {
        }
        public ExpressionItem(Matrix matrix, Pen pen, Font font, LigraEnvironment env)
            : this(GenerateMatrix(matrix), pen, font, env)
        {
        }

        private static Expression GenerateVector(Vector vector)
        {
            SolusVector v = new SolusVector(vector.Length);
            int i;
            for (i = 0; i < vector.Length; i++)
            {
                v[i] = new Literal(vector[i]);
            }
            return v;
        }

        private static Expression GenerateMatrix(Matrix matrix)
        {
            SolusMatrix m = new SolusMatrix(matrix.RowCount, matrix.ColumnCount);
            int i;
            int j;
            for (i = 0; i < matrix.RowCount; i++)
            {
                for (j = 0; j < matrix.ColumnCount; j++)
                {
                    m[i, j] = new Literal(matrix[i, j]);
                }
            }
            return m;
        }

        private Expression _expression;
        public Expression Expression
        {
            get { return _expression; }
        }

        private object _pen;
        public object Pen
        {
            get { return _pen; }
        }

        //private string _name;
        //public string Name
        //{
        //    get { return _name; }
        //}

        private object _font;
        public object Font
        {
            get { return _font; }
        }

        public static bool IsRootOperation(FunctionCall functionCall)
        {
            if (!(functionCall.Function is ExponentOperation)) { return false; }

            if (functionCall.Arguments[1] is Literal)
            {
                Literal literal = (Literal)functionCall.Arguments[1];

                if (literal.Value < 0) { return false; }

                double value = 1 / literal.Value;

                if (value == Math.Floor(value))
                {
                    return true;
                }
            }
            //else if (functionCall.Arguments[1] is FunctionCall &&
            //         (functionCall.Arguments[1] as FunctionCall).Function == DivisionOperation.Value &&
            //         (functionCall.Arguments[1] as FunctionCall).Arguments[0] is Literal &&
            //         ((functionCall.Arguments[1] as FunctionCall).Arguments[0] as Literal).Value == 1 &&
            //          ((functionCall.Arguments[1] as FunctionCall).Arguments[1] is VariableAccess ||
            //           (functionCall.Arguments[1] as FunctionCall).Arguments[1] is Literal))
            //{
            //    return true;
            //}

            return false;
        }

        protected override Widget GetAdapterInternal()
        {
            return new Gtk.Label(Expression.ToString());
        }

        protected override RenderItemControl GetControlInternal()
        {
            return new ExpressionItemControl(this);
        }
    }

    public class ExpressionItemControl : RenderItemControl
    {
        public ExpressionItemControl(ExpressionItem owner)
            : base(owner)
        {

        }

        public new ExpressionItem _owner => (ExpressionItem)base._owner;


        Expression _expression => _owner.Expression;
        Pen _pen => (Pen)_owner.Pen;
        Font _font => (Font)_owner.Font;

        protected override void InternalRender(Graphics g,
            SolusEnvironment env)
        {
            SizeF exprSize = CalcExpressionSize(_owner.Expression, g, Font);
            float xx = 0;

            //if (!string.IsNullOrEmpty(Name))
            //{
            //    SizeF textSize = g.MeasureString(Name + " = ", Font);
            //    xx += textSize.Width + 10;
            //    g.DrawString(Name + " = ", Font, Pen.Brush, new PointF(location.X, location.Y + (exprSize.Height - textSize.Height) / 2));
            //}

            RenderExpression(g, _expression, new PointF(xx, 0), _pen, _pen.Brush, Font, false);
        }

        protected override SizeF InternalCalcSize(Graphics g)
        {
            SizeF exprSize = CalcExpressionSize(_owner.Expression, g, Font);

            //if (!string.IsNullOrEmpty(Name))
            //{
            //    SizeF textSize = g.MeasureString(Name + " = ", Font);
            //    exprSize.Width += textSize.Width + 10;
            //    exprSize.Height = Math.Max(exprSize.Height, textSize.Height);
            //}

            return exprSize;
        }
        public static void RenderExpression(Graphics g, Expression expr, PointF pt, Pen pen, Brush brush, Font font, bool drawBoxes)
        {

            InternalRenderExpression(g, expr, pt, pen, brush, new Dictionary<Expression, SizeF>(), font, drawBoxes);
        }

        protected static void InternalRenderExpression(Graphics g, Expression expr, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font, bool drawBoxes)
        {

            SizeF size = CalcExpressionSize(expr, g, font, expressionSizeCache);

            if (drawBoxes)
            {
                g.DrawRectangle(Pens.Red, pt.X, pt.Y, size.Width, size.Height);
            }

            pt += new SizeF(2, 2);

            if (expr is FunctionCall)
            {
                RenderFunctionCallExpression(g, expr as FunctionCall, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else if (expr is Literal)
            {
                RenderLiteralExpression(g, expr as Literal, pt, pen, brush, expressionSizeCache, font);
            }
            else if (expr is VariableAccess)
            {
                RenderVariableAccess(g, expr as VariableAccess, pt, pen, brush, expressionSizeCache, font);
            }
            else if (expr is DerivativeOfVariable)
            {
                RenderDerivativeOfVariable(g, expr as DerivativeOfVariable, pt, pen, brush, expressionSizeCache, font);
            }
            else if (expr is ColorExpression)
            {
                InternalRenderExpression(g, expr.Eval(null), pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else if (expr is RandomExpression)
            {
                RenderRandomExpression(g, expr as RandomExpression, pt, pen, brush, expressionSizeCache, font);
            }
            else if (expr is SolusMatrix)
            {
                RenderMatrix(g, (SolusMatrix)expr, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else if (expr is SolusVector)
            {
                RenderVector(g, (SolusVector)expr, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else
            {
                throw new InvalidOperationException("Unknown expression type: " + expr.ToString());
            }
        }

        protected static void RenderVector(Graphics g, SolusVector vector, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font, bool drawBoxes)
        {

            SizeF size = CalcExpressionSize(vector, g, font, expressionSizeCache);
            SizeF size2;
            float x = pt.X + 2;

            g.DrawLine(pen, pt.X, pt.Y, pt.X, pt.Y + size.Height);
            g.DrawLine(pen, pt.X, pt.Y, pt.X + size.Width, pt.Y);
            g.DrawLine(pen, pt.X, pt.Y + size.Height, pt.X + size.Width, pt.Y + size.Height);
            foreach (Expression expr in vector)
            {
                size2 = CalcExpressionSize(expr, g, font, expressionSizeCache);
                InternalRenderExpression(g, expr, new PointF(x, pt.Y + (size.Height - size2.Height) / 2), pen, brush, expressionSizeCache, font, drawBoxes);
                x += size2.Width + 2;
                g.DrawLine(pen, x, pt.Y, x, pt.Y + size.Height);
                x += 2;
            }
        }

        protected static void RenderMatrix(Graphics g, SolusMatrix matrix, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font, bool drawBoxes)
        {


            List<float> maxWidthPerColumn = new List<float>();
            List<float> maxHeightPerRow = new List<float>();

            CalcMatrixWidthsAndHeights(g, matrix, maxWidthPerColumn, maxHeightPerRow, expressionSizeCache, font);
            SizeF size = CalcMatrixSizeFromMaxWidthsAndHeights(maxWidthPerColumn, maxHeightPerRow);

            int i;
            int j;

            float x = pt.X;
            float y = pt.Y;
            //float d = 100;

            y = pt.Y;
            for (i = 0; i < matrix.RowCount; i++)
            {
                g.DrawLine(pen, pt.X, y, pt.X + size.Width, y);
                y += maxHeightPerRow[i];
            }
            g.DrawLine(pen, pt.X, y, pt.X + size.Width, y);

            x = pt.X;
            for (j = 0; j < matrix.ColumnCount; j++)
            {
                g.DrawLine(pen, x, pt.Y, x, pt.Y + size.Height);
                x += maxWidthPerColumn[j];
            }
            g.DrawLine(pen, x, pt.Y, x, pt.Y + size.Height);

            SizeF exprSize;
            Expression expr;
            float width;
            float height;
            PointF pt2;

            y = pt.Y;
            for (i = 0; i < matrix.RowCount; i++)
            {
                height = maxHeightPerRow[i];
                x = pt.X;
                for (j = 0; j < matrix.ColumnCount; j++)
                {
                    expr = matrix[i, j];
                    exprSize = CalcExpressionSize(expr, g, font, expressionSizeCache);
                    width = maxWidthPerColumn[j];
                    pt2 = new PointF(
                        x + (width - exprSize.Width) / 2,
                        y + (height - exprSize.Height) / 2);

                    InternalRenderExpression(g, expr, pt2, pen, brush, expressionSizeCache, font, drawBoxes);
                    x += width;
                }
                y += height;
            }
        }

        protected static void RenderRandomExpression(Graphics g, RandomExpression randomExpression, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font)
        {
            g.DrawString("rand()", font, brush, pt);
        }

        protected static void RenderVariableAccess(Graphics g, VariableAccess variableAccess, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font)
        {
            g.DrawString(variableAccess.VariableName, font, brush, pt);
        }

        protected static void RenderDerivativeOfVariable(Graphics g, DerivativeOfVariable derivativeOfVariable, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font)
        {
            int upperOrder = 0;

            upperOrder = derivativeOfVariable.Order;

            string upperString = "d" + (upperOrder > 1 ? upperOrder.ToString() : string.Empty) + derivativeOfVariable.Variable;
            string lowerString = "d" + derivativeOfVariable.LowerVariable + (upperOrder > 1 ? upperOrder.ToString() : string.Empty);

            SizeF size = g.MeasureString(upperString, font);
            size += new SizeF(4, 4);
            g.DrawString(upperString, font, brush, pt + new SizeF(2, 2));
            g.DrawLine(pen, pt.X, pt.Y + size.Height, pt.X + size.Width, pt.Y + size.Height);
            g.DrawString(lowerString, font, brush, pt.X + 2, pt.Y + size.Height + 2);
        }

        protected static void RenderLiteralExpression(Graphics g, Literal literal, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font)
        {

            string str;
            if (Math.Abs(literal.Value - (float)Math.PI) < 1e-6)
            {
                str = "π";
            }
            else if (Math.Abs(literal.Value - (float)Math.E) < 1e-6)
            {
                str = "e";
            }
            else
            {
                str = literal.Value.ToString("G");
            }

            g.DrawString(str, font, brush, pt);
        }

        protected static void RenderFunctionCallExpression(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font, bool drawBoxes)
        {


            if (functionCall.Function is Operation)
            {
                RenderOperation(g, functionCall, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else
            {
                SizeF displayNameSize = g.MeasureString(functionCall.Function.DisplayName, font) + new SizeF(2, 0);
                float parenWidth = g.MeasureString("(", font).Width;// 10;
                SizeF commaSize = g.MeasureString(", ", font);

                bool first;
                float frontWidth = displayNameSize.Width + parenWidth;
                SizeF allArgSize = new SizeF(0, 0);

                first = true;
                foreach (Expression argument in functionCall.Arguments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        allArgSize.Width += commaSize.Width;
                    }

                    SizeF argSize = CalcExpressionSize(argument, g, font, expressionSizeCache);

                    allArgSize.Width += argSize.Width;
                    allArgSize.Height = Math.Max(allArgSize.Height, argSize.Height);
                }

                float currentXOffset = 0;

                first = true;
                foreach (Expression argument in functionCall.Arguments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        g.DrawString(", ", font, brush, pt + new SizeF(frontWidth + currentXOffset, (allArgSize.Height - commaSize.Height) / 2));
                        currentXOffset += commaSize.Width;
                    }

                    SizeF argSize = CalcExpressionSize(argument, g, font, expressionSizeCache);

                    InternalRenderExpression(g, argument, pt + new SizeF(frontWidth + currentXOffset, (allArgSize.Height - argSize.Height) / 2), pen, brush, expressionSizeCache, font, drawBoxes);
                    currentXOffset += argSize.Width;
                }

                RectangleF rect = new RectangleF(pt.X + displayNameSize.Width + 2, pt.Y, parenWidth, allArgSize.Height);
                RenderOpenParenthesis(g, rect, pen, brush);
                rect.X += allArgSize.Width + parenWidth;
                RenderCloseParenthesis(g, rect, pen, brush);

                g.DrawString(functionCall.Function.DisplayName, font, brush, pt + new SizeF(2, allArgSize.Height / 2 - displayNameSize.Height / 2));
            }
        }

        protected static void RenderOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font, bool drawBoxes)
        {

            if (functionCall.Function is BinaryOperation)
            {
                RenderBinaryOperation(g, functionCall, pt, pen, brush, expressionSizeCache, drawBoxes, font);
            }
            else if (functionCall.Function is AssociativeCommutativeOperation)
            {
                RenderAssociativeCommutativOperation(g, functionCall, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else if (functionCall.Function == NegationOperation.Value)
            {
                RenderNegationOperation(g, functionCall, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected static void RenderAssociativeCommutativOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font, bool drawBoxes)
        {


            AssociativeCommutativeOperation operation = functionCall.Function as AssociativeCommutativeOperation;

            string symbol = operation.DisplayName;
            SizeF symbolSize = g.MeasureString(symbol, font);

            float parenWidth = 10;
            RectangleF parenRect = new RectangleF(0, 0, parenWidth, 0);

            SizeF allArgSize = new SizeF(0, 0);

            HashSet<Expression> hasParens = new HashSet<Expression>();

            bool first = true;
            foreach (Expression arg in functionCall.Arguments)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    allArgSize.Width += symbolSize.Width;
                }

                SizeF argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);

                if (arg is FunctionCall &&
                    (arg as FunctionCall).Function is Operation &&
                    ((arg as FunctionCall).Function as Operation).Precedence < (operation as Operation).Precedence)
                {
                    hasParens.Add(arg);
                    argSize.Width += 2 * parenWidth;
                }

                allArgSize.Width += argSize.Width;
                allArgSize.Height = Math.Max(allArgSize.Height, argSize.Height);
            }

            float x = pt.X;

            first = true;
            foreach (Expression arg in functionCall.Arguments)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    g.DrawString(symbol, font, brush, x, pt.Y + (allArgSize.Height - symbolSize.Height) / 2);
                    x += symbolSize.Width;
                }

                SizeF argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);

                if (hasParens.Contains(arg))
                {
                    parenRect.X = x;
                    parenRect.Y = pt.Y + (allArgSize.Height - argSize.Height) / 2;
                    parenRect.Height = argSize.Height;
                    RenderOpenParenthesis(g, parenRect, pen, brush);
                    x += parenRect.Width;
                }

                InternalRenderExpression(g, arg, new PointF(x, pt.Y + (allArgSize.Height - argSize.Height) / 2), pen, brush, expressionSizeCache, font, drawBoxes);
                x += argSize.Width;

                if (hasParens.Contains(arg))
                {
                    parenRect.X = x;
                    RenderCloseParenthesis(g, parenRect, pen, brush);
                    x += parenRect.Width;
                }
            }
        }

        //private void RenderAdditionOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache)
        //{

        //}

        protected static void RenderBinaryOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, bool drawBoxes, Font font)
        {

            if (functionCall.Function is DivisionOperation)
            {
                RenderDivisionOperation(g, functionCall, pt, pen, brush, expressionSizeCache, drawBoxes, font);
            }
            else if (functionCall.Function is ExponentOperation)
            {
                RenderExponentOperation(g, functionCall, pt, pen, brush, expressionSizeCache, drawBoxes, font);
            }
            else
            {
                RenderPartBinaryOperation(g, functionCall, pt, pen, brush, expressionSizeCache, drawBoxes, font);
            }
        }

        static void RenderNegationOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font, bool drawBoxes)
        {
            string symbol = NegationOperation.Value.DisplayName;
            SizeF symbolSize = g.MeasureString(symbol, font);

            float parenWidth = 10;
            RectangleF parenRect = new RectangleF(0, 0, parenWidth, 0);

            bool first = true;
            bool hasParens;
            var arg = functionCall.Arguments[0];
            float widthWithParens;

            SizeF argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);

            if (arg is FunctionCall &&
                (arg as FunctionCall).Function is Operation &&
                ((arg as FunctionCall).Function as Operation).Precedence < NegationOperation.Value.Precedence)
            {
                hasParens = true;
                widthWithParens = argSize.Width + 2 * parenWidth;
            }
            else
            {
                hasParens = false;
                widthWithParens = argSize.Width;
            }

            float x = pt.X;

            arg = functionCall.Arguments[0];

            g.DrawString(symbol, font, brush, x, pt.Y + (argSize.Height - symbolSize.Height) / 2);
            x += symbolSize.Width;

            if (hasParens)
            {
                parenRect.X = x;
                parenRect.Y = pt.Y + (argSize.Height - argSize.Height) / 2;
                parenRect.Height = argSize.Height;
                RenderOpenParenthesis(g, parenRect, pen, brush);
                x += parenRect.Width;
            }

            InternalRenderExpression(g, arg, new PointF(x, pt.Y), pen, brush, expressionSizeCache, font, drawBoxes);
            x += argSize.Width;

            if (hasParens)
            {
                parenRect.X = x;
                RenderCloseParenthesis(g, parenRect, pen, brush);
                x += parenRect.Width;
            }
        }

        protected static void RenderExponentOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, bool drawBoxes, Font font)
        {

            if (ExpressionItem.IsRootOperation(functionCall))
            {
                RenderRootOperation(g, functionCall, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else
            {
                RenderPartBinaryOperation(g, functionCall, pt, pen, brush, expressionSizeCache, drawBoxes, font);
            }
        }

        protected static void RenderRootOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font, bool drawBoxes)
        {

            Literal root = (Literal)functionCall.Arguments[1];
            Literal invRoot = new Literal((float)Math.Round(1 / root.Value));
            Expression arg = functionCall.Arguments[0];
            SizeF argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);
            SizeF rootSize = new SizeF(0, 0);

            if (invRoot.Value > 2)
            {
                rootSize = CalcExpressionSize(invRoot, g, font);
            }

            SizeF size = CalcExpressionSize(functionCall, g, font);

            float shortRadicalLineWidth = 5;
            float shortRadicalLineHeight = 5;
            float longRadicalLineWidth = 10;

            g.DrawLine(pen,
                pt.X + rootSize.Width + 2 - shortRadicalLineWidth,
                pt.Y + size.Height - shortRadicalLineHeight,
                pt.X + rootSize.Width,
                pt.Y + size.Height);
            g.DrawLine(pen,
                pt.X + rootSize.Width,
                pt.Y + size.Height,
                pt.X + rootSize.Width + longRadicalLineWidth,
                pt.Y);
            g.DrawLine(pen,
                pt.X + rootSize.Width + longRadicalLineWidth,
                pt.Y,
                pt.X + size.Width,
                pt.Y);

            if (invRoot.Value > 2)
            {
                InternalRenderExpression(g, invRoot, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            InternalRenderExpression(g, arg, pt + new SizeF(size.Width - argSize.Width - 2, 2), pen, brush, expressionSizeCache, font, drawBoxes);
        }

        protected static void RenderPartBinaryOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, bool drawBoxes, Font font)
        {


            string symbol = //" " + 
                functionCall.Function.DisplayName
                //+ " "
                ;

            SizeF leftOperandSize = CalcExpressionSize(functionCall.Arguments[0], g, font, expressionSizeCache);
            SizeF rightOperandSize = CalcExpressionSize(functionCall.Arguments[1], g, font, expressionSizeCache);
            SizeF symbolSize = g.MeasureString(symbol, font);
            float maxHeight = Math.Max(Math.Max(leftOperandSize.Height, rightOperandSize.Height), symbolSize.Height);

            float parenWidth = 10;

            bool leftParens = false;
            bool rightParens = false;

            if (NeedsLeftParen(functionCall))
            {
                //leftOperandSize.Width += parenWidth * 2;
                leftParens = true;
            }

            if (NeedsRightParen(functionCall))
            {
                //rightOperandSize.Width += parenWidth * 2;
                rightParens = true;
            }

            float x = pt.X;
            float y;
            RectangleF parenRect = new RectangleF(0, 0, 0, 0);

            parenRect.Width = parenWidth;

            y = pt.Y + (maxHeight - leftOperandSize.Height) / 2;
            parenRect.Y = y;
            parenRect.Height = leftOperandSize.Height;

            if (leftParens)
            {
                parenRect.X = x;
                RenderOpenParenthesis(g, parenRect, pen, brush);
                x += parenWidth;
            }

            InternalRenderExpression(g, functionCall.Arguments[0], new PointF(x, y), pen, brush, expressionSizeCache, font, drawBoxes);
            x += leftOperandSize.Width;

            if (leftParens)
            {
                parenRect.X = x;
                RenderCloseParenthesis(g, parenRect, pen, brush);
                x += parenWidth;
            }




            y = pt.Y + (maxHeight - symbolSize.Height) / 2;
            g.DrawString(symbol, font, brush, new PointF(x, y));
            if (drawBoxes)
            {
                g.DrawRectangle(Pens.Yellow, x, y, symbolSize.Width, symbolSize.Height);
            }
            x += symbolSize.Width;




            y = pt.Y + (maxHeight - rightOperandSize.Height) / 2;
            parenRect.Y = y;
            parenRect.Height = rightOperandSize.Height;

            if (rightParens)
            {
                parenRect.X = x;
                RenderOpenParenthesis(g, parenRect, pen, brush);
                x += parenWidth;
            }

            InternalRenderExpression(g, functionCall.Arguments[1], new PointF(x, y), pen, brush, expressionSizeCache, font, drawBoxes);
            x += rightOperandSize.Width;

            if (rightParens)
            {
                parenRect.X = x;
                RenderCloseParenthesis(g, parenRect, pen, brush);
                x += parenWidth;
            }
        }

        private static bool NeedsRightParen(FunctionCall functionCall)
        {
            return (functionCall.Arguments[1] is FunctionCall &&
                (functionCall.Arguments[1] as FunctionCall).Function is Operation &&
                ((functionCall.Arguments[1] as FunctionCall).Function as Operation).Precedence < (functionCall.Function as Operation).Precedence) ||
                (functionCall.Arguments[1] is FunctionCall && (functionCall.Arguments[1] as FunctionCall).Function is ExponentOperation) ||
                (functionCall.Arguments[1] is FunctionCall && (functionCall.Arguments[1] as FunctionCall).Function is DivisionOperation);
        }

        private static bool NeedsLeftParen(FunctionCall functionCall)
        {
            return (functionCall.Arguments[0] is FunctionCall &&
                (functionCall.Arguments[0] as FunctionCall).Function is Operation &&
                ((functionCall.Arguments[0] as FunctionCall).Function as Operation).Precedence < (functionCall.Function as Operation).Precedence) ||
                (functionCall.Arguments[0] is FunctionCall && (functionCall.Arguments[0] as FunctionCall).Function is ExponentOperation) ||
                (functionCall.Arguments[0] is FunctionCall && (functionCall.Arguments[0] as FunctionCall).Function is DivisionOperation);
        }

        protected static void RenderDivisionOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, bool drawBoxes, Font font)
        {


            SizeF size1 = CalcExpressionSize(functionCall.Arguments[0], g, font, expressionSizeCache);
            SizeF size2 = CalcExpressionSize(functionCall.Arguments[1], g, font, expressionSizeCache);
            SizeF size = new SizeF(Math.Max(size1.Width, size2.Width), size1.Height + size2.Height);

            float lineExtraWidth = 4;
            float lineHeightSpacing = 4;

            InternalRenderExpression(g, functionCall.Arguments[0], pt + new SizeF(lineExtraWidth + (size.Width - size1.Width) / 2, 0), pen, brush, expressionSizeCache, font, drawBoxes);
            InternalRenderExpression(g, functionCall.Arguments[1], pt + new SizeF(lineExtraWidth + (size.Width - size2.Width) / 2, size1.Height + lineHeightSpacing), pen, brush, expressionSizeCache, font, drawBoxes);

            g.DrawLine(pen,
                pt.X,
                pt.Y + size1.Height + lineHeightSpacing / 2,
                pt.X + size.Width + 2 * lineExtraWidth,
                pt.Y + size1.Height + lineHeightSpacing / 2);
        }

        protected static void RenderCloseParenthesis(Graphics g, RectangleF rect, Pen pen, Brush brush)
        {
            rect.Width -= 2;

            //g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            //g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
            //g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);

            g.DrawArc(pen, rect.Left, rect.Top, rect.Width, 2 * rect.Width, 270, 90);
            g.DrawLine(pen, rect.Right, rect.Top + rect.Width, rect.Right, rect.Bottom - rect.Width);
            g.DrawArc(pen, rect.Left, rect.Bottom - 2 * rect.Width, rect.Width, 2 * rect.Width, 0, 90);

            //float x = rect.Width;
            //float y = rect.Height;
            //float r = y * y / (8 * x) + x / 2;
            //float theta = (float)(Math.Asin(r / (2 * y)) * 180 / Math.PI);

            //RectangleF r2 = new RectangleF(rect.X - 2 * r + x, rect.Y + y / 2 - r, 2 * r, 2 * r);
            //g.DrawArc(pen, r2, -theta, 2 * theta);
        }

        protected static void RenderOpenParenthesis(Graphics g, RectangleF rect, Pen pen, Brush brush)
        {
            rect.X += 2;
            rect.Width -= 2;

            //g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            //g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
            //g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);

            g.DrawArc(pen, rect.Left, rect.Top, rect.Width, 2 * rect.Width, 180, 90);
            g.DrawLine(pen, rect.Left, rect.Top + rect.Width, rect.Left, rect.Bottom - rect.Width);
            g.DrawArc(pen, rect.Left, rect.Bottom - 2 * rect.Width, rect.Width, 2 * rect.Width, 90, 90);

            //float x = rect.Width;
            //float y = rect.Height;
            //float r = y * y / (8 * x) + x / 2;
            //float theta = (float)(Math.Asin(r / (2 * y)) * 180 / Math.PI);

            //RectangleF r2 = new RectangleF(rect.X, rect.Y + y / 2 - r, 2 * r, 2 * r);
            //g.DrawArc(pen, r2, 180 - theta, 2 * theta);
        }

        public static SizeF CalcExpressionSize(Expression expr, Graphics g, Font font)
        {
            return CalcExpressionSize(expr, g, font, new Dictionary<Expression, SizeF>());
        }

        protected static SizeF CalcExpressionSize(Expression expr, Graphics g, Font font, Dictionary<Expression, SizeF> expressionSizeCache)
        {

            if (expressionSizeCache.ContainsKey(expr))
            {
                return expressionSizeCache[expr];
            }

            SizeF size;

            if (expr is FunctionCall)
            {
                size = CalcFunctionCallSize(g, expr, expressionSizeCache, font);
            }
            else if (expr is Literal)
            {
                size = g.MeasureString((expr as Literal).ToString(), font);
            }
            else if (expr is VariableAccess)
            {
                size = g.MeasureString((expr as VariableAccess).VariableName, font);
            }
            else if (expr is DerivativeOfVariable)
            {
                DerivativeOfVariable derivativeOfVariable = (DerivativeOfVariable)expr;

                int upperOrder = 0;

                upperOrder = derivativeOfVariable.Order;

                string upperString = "d" + (upperOrder > 1 ? upperOrder.ToString() : string.Empty) + derivativeOfVariable.Variable;
                string lowerString = "d" + derivativeOfVariable.LowerVariable + (upperOrder > 1 ? upperOrder.ToString() : string.Empty);

                SizeF size2 = g.MeasureString(upperString, font);
                SizeF size3 = g.MeasureString(lowerString, font);

                size = new SizeF(Math.Max(size2.Width, size3.Width), size2.Height + size3.Height + 2);
            }
            else if (expr is ColorExpression)
            {
                size = CalcExpressionSize(expr.Eval(null), g, font, expressionSizeCache);
            }
            else if (expr is RandomExpression)
            {
                size = g.MeasureString("rand()", font);
            }
            else if (expr is SolusMatrix)
            {
                SolusMatrix expr2 = (SolusMatrix)expr;
                List<float> maxWidthPerColumn = new List<float>();
                List<float> maxHeightPerRow = new List<float>();

                CalcMatrixWidthsAndHeights(g, expr2, maxWidthPerColumn, maxHeightPerRow, expressionSizeCache, font);
                size = CalcMatrixSizeFromMaxWidthsAndHeights(maxWidthPerColumn, maxHeightPerRow);
            }
            else if (expr is SolusVector)
            {
                SolusVector vector = (SolusVector)expr;
                int i;

                float width = 0;
                float height = 0;
                SizeF exprSize;

                for (i = 0; i < vector.Length; i++)
                {
                    exprSize = CalcExpressionSize(vector[i], g, font, expressionSizeCache);
                    width += exprSize.Width + 2;
                    height = Math.Max(height, exprSize.Height);
                }

                size = new SizeF(width + 4, height + 4);
            }
            else
            {
                throw new InvalidOperationException();
            }

            //margin
            size += new SizeF(4, 4);

            expressionSizeCache[expr] = size;

            return size;
        }

        private static SizeF CalcMatrixSizeFromMaxWidthsAndHeights(List<float> maxWidthPerColumn, List<float> maxHeightPerRow)
        {
            SizeF size;

            float width = 0;
            float height = 0;

            foreach (float f in maxWidthPerColumn)
            {
                width += f;
            }
            foreach (float f in maxHeightPerRow)
            {
                height += f;
            }

            size = new SizeF(width, height);
            return size;
        }

        private static void CalcMatrixWidthsAndHeights(Graphics g, SolusMatrix matrix, List<float> maxWidthPerColumn, List<float> maxHeightPerRow, Dictionary<Expression, SizeF> expressionSizeCache, Font font)
        {
            int i;
            int j;

            maxWidthPerColumn.Clear();
            maxHeightPerRow.Clear();

            for (j = 0; j < matrix.ColumnCount; j++)
            {
                maxWidthPerColumn.Add(0);
            }
            for (i = 0; i < matrix.RowCount; i++)
            {
                maxHeightPerRow.Add(0);

                for (j = 0; j < matrix.ColumnCount; j++)
                {
                    SizeF size = CalcExpressionSize(matrix[i, j], g, font, expressionSizeCache);

                    size += new SizeF(4, 4);

                    maxWidthPerColumn[j] = Math.Max(maxWidthPerColumn[j], size.Width);
                    maxHeightPerRow[i] = Math.Max(maxHeightPerRow[i], size.Height);
                }
            }
        }

        private static SizeF CalcFunctionCallSize(Graphics g, Expression expr, Dictionary<Expression, SizeF> expressionSizeCache, Font font)
        {

            SizeF size;
            FunctionCall functionCall = expr as FunctionCall;

            if (functionCall.Function is Operation)
            {
                //if (call.Function is UnaryOperation)
                //{
                //}
                //else
                if (functionCall.Function is BinaryOperation)
                {
                    if (functionCall.Function is DivisionOperation)
                    {
                        SizeF topOperandSize = CalcExpressionSize(functionCall.Arguments[0], g, font, expressionSizeCache);
                        SizeF bottomOperandSize = CalcExpressionSize(functionCall.Arguments[1], g, font, expressionSizeCache);

                        float width = Math.Max(topOperandSize.Width, bottomOperandSize.Width);
                        float height = topOperandSize.Height + bottomOperandSize.Height;
                        float lineExtraWidth = 2 * 4;
                        float lineHeightSpacing = 4;

                        size = new SizeF(width + lineExtraWidth, height + lineHeightSpacing);
                    }
                    else if (ExpressionItem.IsRootOperation(functionCall))
                    {
                        Literal root = (Literal)functionCall.Arguments[1];
                        Literal invRoot = new Literal((float)Math.Round(1 / root.Value));
                        Expression arg = functionCall.Arguments[0];
                        SizeF argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);
                        SizeF rootSize = new SizeF(0, 0);

                        if (invRoot.Value > 2)
                        {
                            rootSize = CalcExpressionSize(invRoot, g, font);
                        }

                        //float shortRadicalLineWidth = 5;
                        //float shortRadicalLineHeight = 5;
                        float longRadicalLineWidth = 10;

                        size = new SizeF(rootSize.Width + longRadicalLineWidth + argSize.Width + 2, argSize.Height + 4);
                    }
                    else
                    {
                        string operatorSymbol = //" " + 
                            functionCall.Function.DisplayName
                            //+ " "
                            ;

                        SizeF leftOperandSize = CalcExpressionSize(functionCall.Arguments[0], g, font, expressionSizeCache);
                        SizeF rightOperandSize = CalcExpressionSize(functionCall.Arguments[1], g, font, expressionSizeCache);
                        SizeF operatorSymbolSize = g.MeasureString(operatorSymbol, font);

                        float parenWidth = 10;

                        //if (functionCall.Arguments[0] is FunctionCall &&
                        //    (functionCall.Arguments[0] as FunctionCall).Function is Operation &&
                        //    ((functionCall.Arguments[0] as FunctionCall).Function as Operation).Precedence < (functionCall.Function as Operation).Precedence)
                        if (NeedsLeftParen(functionCall))
                        {
                            leftOperandSize.Width += parenWidth * 2;
                        }

                        //if (functionCall.Arguments[1] is FunctionCall &&
                        //    (functionCall.Arguments[1] as FunctionCall).Function is Operation &&
                        //    ((functionCall.Arguments[1] as FunctionCall).Function as Operation).Precedence < (functionCall.Function as Operation).Precedence)
                        if (NeedsRightParen(functionCall))
                        {
                            rightOperandSize.Width += parenWidth * 2;
                        }

                        float width = leftOperandSize.Width + rightOperandSize.Width + operatorSymbolSize.Width;
                        float height = Math.Max(Math.Max(leftOperandSize.Height, rightOperandSize.Height), operatorSymbolSize.Height);

                        size = new SizeF(width, height);
                    }
                }
                else if (functionCall.Function is AssociativeCommutativeOperation)
                {
                    AssociativeCommutativeOperation operation = functionCall.Function as AssociativeCommutativeOperation;
                    string symbol = operation.DisplayName;
                    SizeF symbolSize = g.MeasureString(symbol, font);

                    float parenWidth = 10;

                    size = new SizeF(0, 0);

                    bool first = true;
                    foreach (Expression arg in functionCall.Arguments)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            size.Width += symbolSize.Width;
                        }

                        SizeF argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);

                        if (arg is FunctionCall &&
                            (arg as FunctionCall).Function is Operation &&
                            ((arg as FunctionCall).Function as Operation).Precedence < (operation as Operation).Precedence)
                        {
                            argSize.Width += parenWidth * 2;
                        }


                        size.Width += argSize.Width;
                        size.Height = Math.Max(size.Height, argSize.Height);
                    }

                }
                else if (functionCall.Function == NegationOperation.Value)
                {
                    string symbol = NegationOperation.Value.DisplayName;
                    SizeF symbolSize = g.MeasureString(symbol, font);

                    float parenWidth = 10;

                    var arg = functionCall.Arguments[0];
                    SizeF argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);

                    float widthWithParen;
                    if (arg is FunctionCall &&
                        (arg as FunctionCall).Function is Operation &&
                        ((arg as FunctionCall).Function as Operation).Precedence < NegationOperation.Value.Precedence)
                    {
                        widthWithParen = argSize.Width + parenWidth * 2;
                    }
                    else
                    {
                        widthWithParen = argSize.Width;
                    }

                    return new SizeF(widthWithParen, argSize.Height);
                }
                else
                {
                    throw new InvalidOperationException("Unknown Operation: " + functionCall.Function.ToString());
                }
            }
            else
            {
                SizeF displayNameSize;
                SizeF openParenSize;
                SizeF closeParenSize;
                SizeF commaSize;
                SizeF allArgSize;

                displayNameSize = g.MeasureString(functionCall.Function.DisplayName, font) + new SizeF(2, 0);
                openParenSize = g.MeasureString("(", font);
                closeParenSize = g.MeasureString(")", font);
                commaSize = g.MeasureString(", ", font);

                allArgSize = new SizeF(0, 0);

                bool first = true;
                foreach (Expression arg in functionCall.Arguments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        allArgSize.Width += commaSize.Width;
                    }
                    SizeF argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);
                    allArgSize.Width += argSize.Width;
                    allArgSize.Height = Math.Max(allArgSize.Height, argSize.Height);
                }

                float width;
                float height;

                width = displayNameSize.Width + openParenSize.Width + allArgSize.Width + closeParenSize.Width;
                height = Math.Max(Math.Max(Math.Max(displayNameSize.Height, openParenSize.Height), commaSize.Height), allArgSize.Height);

                size = new SizeF(width, height);
            }
            return size;
        }
    }
}

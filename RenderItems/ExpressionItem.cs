using System;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Functions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class ExpressionItem : RenderItem
    {
        public ExpressionItem(Expression expression, LPen pen, LFont font, LigraEnvironment env)
            : base(env)
        {
            _expression = expression;
            _pen = pen;
            _font = font;
        }
        public ExpressionItem(Acuity.Vector vector, LPen pen, LFont font,
            LigraEnvironment env)
            : this(GenerateVector(vector), pen, font, env)
        {
        }
        public ExpressionItem(Acuity.Matrix matrix, LPen pen, LFont font,
            LigraEnvironment env)
            : this(GenerateMatrix(matrix), pen, font, env)
        {
        }

        private static Expression GenerateVector(Acuity.Vector vector)
        {
            var v = new VectorExpression(vector.Length);
            int i;
            for (i = 0; i < vector.Length; i++)
            {
                v[i] = new Literal(vector[i]);
            }
            return v;
        }

        private static Expression GenerateMatrix(Acuity.Matrix matrix)
        {
            var m = new MatrixExpression(matrix.RowCount, matrix.ColumnCount);
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

        private LPen _pen;
        public LPen Pen
        {
            get { return _pen; }
        }

        private LFont _font;
        public LFont Font
        {
            get { return _font; }
        }

        protected override void InternalRender(IRenderer g, SolusEnvironment env)
        {
            var exprSize = CalcExpressionSize(Expression, g, Font);
            float xx = 0;
            RenderExpression(g, _expression, new Vector2(xx, 0), _pen, _pen.Brush, Font, false);
        }

        protected override Vector2 InternalCalcSize(IRenderer g)
        {
            var exprSize = CalcExpressionSize(Expression, g, Font);
            return exprSize;
        }

        public static void RenderExpression(IRenderer g, Expression expr, Vector2 pt, LPen pen, LBrush brush, LFont font, bool drawBoxes)
        {
            InternalRenderExpression(g, expr, pt, pen, brush, new Dictionary<Expression, Vector2>(), font, drawBoxes);
        }

        protected static void InternalRenderExpression(IRenderer g, Expression expr, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, LFont font, bool drawBoxes)
        {
            var size = CalcExpressionSize(expr, g, font, expressionSizeCache);

            if (drawBoxes)
            {
                g.DrawRectangle(LPen.Red, pt.X, pt.Y, size.X, size.Y);
            }

            pt += new Vector2(2, 2);

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
                InternalRenderExpression(g,
                    new Literal(expr.Eval(null)), pt, pen, brush,
                    expressionSizeCache, font, drawBoxes);
            }
            else if (expr is RandomExpression)
            {
                RenderRandomExpression(g, expr as RandomExpression, pt, pen, brush, expressionSizeCache, font);
            }
            else if (expr is MatrixExpression)
            {
                RenderMatrix(g, (MatrixExpression)expr, pt, pen, brush,
                    expressionSizeCache, font, drawBoxes);
            }
            else if (expr is VectorExpression)
            {
                RenderVector(g, (VectorExpression)expr, pt, pen, brush,
                    expressionSizeCache, font, drawBoxes);
            }
            else
            {
                throw new InvalidOperationException("Unknown expression type: " + expr.ToString());
            }
        }

        protected static void RenderVector(IRenderer g,
            VectorExpression vector, Vector2 pt, LPen pen, LBrush brush,
            Dictionary<Expression, Vector2> expressionSizeCache, LFont font,
            bool drawBoxes)
        {
            var size = CalcExpressionSize(vector, g, font, expressionSizeCache);
            Vector2 size2;
            float x = pt.X + 2;

            g.DrawLine(pen, pt.X, pt.Y, pt.X, pt.Y + size.Y);
            g.DrawLine(pen, pt.X, pt.Y, pt.X + size.X, pt.Y);
            g.DrawLine(pen, pt.X, pt.Y + size.Y, pt.X + size.X, pt.Y + size.Y);
            foreach (Expression expr in vector)
            {
                size2 = CalcExpressionSize(expr, g, font, expressionSizeCache);
                InternalRenderExpression(g, expr, new Vector2(x, pt.Y + (size.Y - size2.Y) / 2), pen, brush, expressionSizeCache, font, drawBoxes);
                x += size2.X + 2;
                g.DrawLine(pen, x, pt.Y, x, pt.Y + size.Y);
                x += 2;
            }
        }

        protected static void RenderMatrix(IRenderer g,
            MatrixExpression matrix, Vector2 pt, LPen pen, LBrush brush,
            Dictionary<Expression, Vector2> expressionSizeCache, LFont font,
            bool drawBoxes)
        {
            List<float> maxWidthPerColumn = new List<float>();
            List<float> maxHeightPerRow = new List<float>();

            CalcMatrixWidthsAndHeights(g, matrix, maxWidthPerColumn, maxHeightPerRow, expressionSizeCache, font);
            var size = CalcMatrixSizeFromMaxWidthsAndHeights(maxWidthPerColumn, maxHeightPerRow);

            int i;
            int j;

            float x = pt.X;
            float y = pt.Y;

            y = pt.Y;
            for (i = 0; i < matrix.RowCount; i++)
            {
                g.DrawLine(pen, pt.X, y, pt.X + size.X, y);
                y += maxHeightPerRow[i];
            }
            g.DrawLine(pen, pt.X, y, pt.X + size.X, y);

            x = pt.X;
            for (j = 0; j < matrix.ColumnCount; j++)
            {
                g.DrawLine(pen, x, pt.Y, x, pt.Y + size.Y);
                x += maxWidthPerColumn[j];
            }
            g.DrawLine(pen, x, pt.Y, x, pt.Y + size.Y);

            Vector2 exprSize;
            Expression expr;
            float width;
            float height;
            Vector2 pt2;

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
                    pt2 = new Vector2(
                        x + (width - exprSize.X) / 2,
                        y + (height - exprSize.Y) / 2);

                    InternalRenderExpression(g, expr, pt2, pen, brush, expressionSizeCache, font, drawBoxes);
                    x += width;
                }
                y += height;
            }
        }

        protected static void RenderRandomExpression(IRenderer g, RandomExpression randomExpression, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, LFont font)
        {
            g.DrawString("rand()", font, brush, pt);
        }

        protected static void RenderVariableAccess(IRenderer g, VariableAccess variableAccess, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, LFont font)
        {
            g.DrawString(variableAccess.VariableName, font, brush, pt);
        }

        protected static void RenderDerivativeOfVariable(IRenderer g, DerivativeOfVariable derivativeOfVariable, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, LFont font)
        {
            int upperOrder = 0;

            upperOrder = derivativeOfVariable.Order;

            string upperString = "d" + (upperOrder > 1 ? upperOrder.ToString() : string.Empty) + derivativeOfVariable.Variable;
            string lowerString = "d" + derivativeOfVariable.LowerVariable + (upperOrder > 1 ? upperOrder.ToString() : string.Empty);

            var size = g.MeasureString(upperString, font);
            size += new Vector2(4, 4);
            g.DrawString(upperString, font, brush, pt + new Vector2(2, 2));
            g.DrawLine(pen, pt.X, pt.Y + size.Y, pt.X + size.X, pt.Y + size.Y);
            g.DrawString(lowerString, font, brush, pt.X + 2, pt.Y + size.Y + 2);
        }

        protected static void RenderLiteralExpression(IRenderer g, Literal literal, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, LFont font)
        {
            string str;
            var value = literal.Value.ToNumber().Value;
            if (Math.Abs(value - (float)Math.PI) < 1e-6)
            {
                str = "π";
            }
            else if (Math.Abs(value - (float)Math.E) <
                     1e-6)
            {
                str = "e";
            }
            else
            {
                str = value.ToString("G");
            }

            g.DrawString(str, font, brush, pt);
        }

        protected static void RenderFunctionCallExpression(IRenderer g, FunctionCall functionCall, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, LFont font, bool drawBoxes)
        {
            if (functionCall.Function is Operation)
            {
                RenderOperation(g, functionCall, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else
            {
                var displayNameSize = g.MeasureString(functionCall.Function.DisplayName, font) + new Vector2(2, 0);
                float parenWidth = g.MeasureString("(", font).X;
                var commaSize = g.MeasureString(", ", font);

                bool first;
                float frontWidth = displayNameSize.X + parenWidth;
                var allArgSize = new Vector2(0, 0);

                first = true;
                foreach (Expression argument in functionCall.Arguments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        allArgSize = allArgSize.AddX(commaSize.X);
                    }

                    var argSize = CalcExpressionSize(argument, g, font, expressionSizeCache);

                    allArgSize = new Vector2(
                        allArgSize.X + argSize.X,
                        Math.Max(allArgSize.Y, argSize.Y));
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
                        g.DrawString(", ", font, brush, pt + new Vector2(frontWidth + currentXOffset, (allArgSize.Y - commaSize.Y) / 2));
                        currentXOffset += commaSize.X;
                    }

                    var argSize = CalcExpressionSize(argument, g, font, expressionSizeCache);

                    InternalRenderExpression(g, argument, pt + new Vector2(frontWidth + currentXOffset, (allArgSize.Y - argSize.Y) / 2), pen, brush, expressionSizeCache, font, drawBoxes);
                    currentXOffset += argSize.X;
                }

                RectangleF rect = new RectangleF(pt.X + displayNameSize.X + 2, pt.Y, parenWidth, allArgSize.Y);
                RenderOpenParenthesis(g, rect, pen, brush);
                rect.X += allArgSize.X + parenWidth;
                RenderCloseParenthesis(g, rect, pen, brush);

                g.DrawString(functionCall.Function.DisplayName, font, brush, pt + new Vector2(2, allArgSize.Y / 2 - displayNameSize.Y / 2));
            }
        }

        protected static void RenderOperation(IRenderer g, FunctionCall functionCall, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, LFont font, bool drawBoxes)
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

        protected static void RenderAssociativeCommutativOperation(IRenderer g, FunctionCall functionCall, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, LFont font, bool drawBoxes)
        {
            AssociativeCommutativeOperation operation = functionCall.Function as AssociativeCommutativeOperation;

            string symbol = operation.DisplayName;
            var symbolSize = g.MeasureString(symbol, font);

            float parenWidth = 10;
            RectangleF parenRect = new RectangleF(0, 0, parenWidth, 0);

            var allArgSize = new Vector2(0, 0);

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
                    allArgSize = allArgSize.AddX(symbolSize.X);
                }

                var argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);

                if (arg is FunctionCall &&
                    (arg as FunctionCall).Function is Operation &&
                    ((arg as FunctionCall).Function as Operation).Precedence < (operation as Operation).Precedence)
                {
                    hasParens.Add(arg);
                    argSize = argSize.AddX(2 * parenWidth);
                }

                allArgSize = new Vector2(
                    allArgSize.X + argSize.X,
                    Math.Max(allArgSize.Y, argSize.Y));
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
                    g.DrawString(symbol, font, brush, x, pt.Y + (allArgSize.Y - symbolSize.Y) / 2);
                    x += symbolSize.X;
                }

                var argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);

                if (hasParens.Contains(arg))
                {
                    parenRect.X = x;
                    parenRect.Y = pt.Y + (allArgSize.Y - argSize.Y) / 2;
                    parenRect.Height = argSize.Y;
                    RenderOpenParenthesis(g, parenRect, pen, brush);
                    x += parenRect.Width;
                }

                InternalRenderExpression(g, arg, new Vector2(x, pt.Y + (allArgSize.Y - argSize.Y) / 2), pen, brush, expressionSizeCache, font, drawBoxes);
                x += argSize.X;

                if (hasParens.Contains(arg))
                {
                    parenRect.X = x;
                    RenderCloseParenthesis(g, parenRect, pen, brush);
                    x += parenRect.Width;
                }
            }
        }

        protected static void RenderBinaryOperation(IRenderer g, FunctionCall functionCall, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, bool drawBoxes, LFont font)
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

        static void RenderNegationOperation(IRenderer g, FunctionCall functionCall, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, LFont font, bool drawBoxes)
        {
            string symbol = NegationOperation.Value.DisplayName;
            var symbolSize = g.MeasureString(symbol, font);

            float parenWidth = 10;
            RectangleF parenRect = new RectangleF(0, 0, parenWidth, 0);

            bool first = true;
            bool hasParens;
            var arg = functionCall.Arguments[0];
            float widthWithParens;

            var argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);

            if (arg is FunctionCall &&
                (arg as FunctionCall).Function is Operation &&
                ((arg as FunctionCall).Function as Operation).Precedence < NegationOperation.Value.Precedence)
            {
                hasParens = true;
                widthWithParens = argSize.X + 2 * parenWidth;
            }
            else
            {
                hasParens = false;
                widthWithParens = argSize.X;
            }

            float x = pt.X;

            arg = functionCall.Arguments[0];

            g.DrawString(symbol, font, brush, x, pt.Y + (argSize.Y - symbolSize.Y) / 2);
            x += symbolSize.X;

            if (hasParens)
            {
                parenRect.X = x;
                parenRect.Y = pt.Y + (argSize.Y - argSize.Y) / 2;
                parenRect.Height = argSize.Y;
                RenderOpenParenthesis(g, parenRect, pen, brush);
                x += parenRect.Width;
            }

            InternalRenderExpression(g, arg, new Vector2(x, pt.Y), pen, brush, expressionSizeCache, font, drawBoxes);
            x += argSize.X;

            if (hasParens)
            {
                parenRect.X = x;
                RenderCloseParenthesis(g, parenRect, pen, brush);
                x += parenRect.Width;
            }
        }

        protected static void RenderExponentOperation(IRenderer g, FunctionCall functionCall, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, bool drawBoxes, LFont font)
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

        public static bool IsRootOperation(FunctionCall functionCall)
        {
            if (!(functionCall.Function is ExponentOperation)) { return false; }

            if (functionCall.Arguments[1] is Literal)
            {
                Literal literal = (Literal)functionCall.Arguments[1];
                var value1 = literal.Value.ToNumber().Value;

                if (value1 < 0) { return false; }

                double value = 1 / value1;

                if (value == Math.Floor(value))
                {
                    return true;
                }
            }

            return false;
        }

        protected static void RenderRootOperation(IRenderer g, FunctionCall functionCall, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, LFont font, bool drawBoxes)
        {
            Literal root = (Literal)functionCall.Arguments[1];
            var invRoot = (float)Math.Round(1 / root.Value.ToNumber().Value);
            Expression arg = functionCall.Arguments[0];
            var argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);
            var rootSize = new Vector2(0, 0);
            var invRootLiteral = new Literal(invRoot.ToNumber());

            if (invRoot > 2)
            {
                rootSize = CalcExpressionSize(
                    invRootLiteral, g, font);
            }

            var size = CalcExpressionSize(functionCall, g, font);

            float shortRadicalLineWidth = 5;
            float shortRadicalLineHeight = 5;
            float longRadicalLineWidth = 10;

            g.DrawLine(pen,
                pt.X + rootSize.X + 2 - shortRadicalLineWidth,
                pt.Y + size.Y - shortRadicalLineHeight,
                pt.X + rootSize.X,
                pt.Y + size.Y);
            g.DrawLine(pen,
                pt.X + rootSize.X,
                pt.Y + size.Y,
                pt.X + rootSize.X + longRadicalLineWidth,
                pt.Y);
            g.DrawLine(pen,
                pt.X + rootSize.X + longRadicalLineWidth,
                pt.Y,
                pt.X + size.X,
                pt.Y);

            if (invRoot > 2)
            {
                InternalRenderExpression(g, invRootLiteral, pt, pen, brush,
                    expressionSizeCache, font, drawBoxes);
            }
            InternalRenderExpression(g, arg, pt + new Vector2(size.X - argSize.X - 2, 2), pen, brush, expressionSizeCache, font, drawBoxes);
        }

        protected static void RenderPartBinaryOperation(IRenderer g, FunctionCall functionCall, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, bool drawBoxes, LFont font)
        {
            string symbol = functionCall.Function.DisplayName;

            var leftOperandSize = CalcExpressionSize(functionCall.Arguments[0], g, font, expressionSizeCache);
            var rightOperandSize = CalcExpressionSize(functionCall.Arguments[1], g, font, expressionSizeCache);
            var symbolSize = g.MeasureString(symbol, font);
            float maxHeight = Math.Max(Math.Max(leftOperandSize.Y, rightOperandSize.Y), symbolSize.Y);

            float parenWidth = 10;

            bool leftParens = false;
            bool rightParens = false;

            if (NeedsLeftParen(functionCall))
            {
                leftParens = true;
            }

            if (NeedsRightParen(functionCall))
            {
                rightParens = true;
            }

            float x = pt.X;
            float y;
            RectangleF parenRect = new RectangleF(0, 0, 0, 0);

            parenRect.Width = parenWidth;

            y = pt.Y + (maxHeight - leftOperandSize.Y) / 2;
            parenRect.Y = y;
            parenRect.Height = leftOperandSize.Y;

            if (leftParens)
            {
                parenRect.X = x;
                RenderOpenParenthesis(g, parenRect, pen, brush);
                x += parenWidth;
            }

            InternalRenderExpression(g, functionCall.Arguments[0], new Vector2(x, y), pen, brush, expressionSizeCache, font, drawBoxes);
            x += leftOperandSize.X;

            if (leftParens)
            {
                parenRect.X = x;
                RenderCloseParenthesis(g, parenRect, pen, brush);
                x += parenWidth;
            }




            y = pt.Y + (maxHeight - symbolSize.Y) / 2;
            g.DrawString(symbol, font, brush, new Vector2(x, y));
            if (drawBoxes)
            {
                g.DrawRectangle(LPen.Yellow, x, y, symbolSize.X, symbolSize.Y);
            }
            x += symbolSize.X;




            y = pt.Y + (maxHeight - rightOperandSize.Y) / 2;
            parenRect.Y = y;
            parenRect.Height = rightOperandSize.Y;

            if (rightParens)
            {
                parenRect.X = x;
                RenderOpenParenthesis(g, parenRect, pen, brush);
                x += parenWidth;
            }

            InternalRenderExpression(g, functionCall.Arguments[1], new Vector2(x, y), pen, brush, expressionSizeCache, font, drawBoxes);
            x += rightOperandSize.X;

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

        protected static void RenderDivisionOperation(IRenderer g, FunctionCall functionCall, Vector2 pt, LPen pen, LBrush brush, Dictionary<Expression, Vector2> expressionSizeCache, bool drawBoxes, LFont font)
        {
            var size1 = CalcExpressionSize(functionCall.Arguments[0], g, font, expressionSizeCache);
            var size2 = CalcExpressionSize(functionCall.Arguments[1], g, font, expressionSizeCache);
            var size = new Vector2(Math.Max(size1.X, size2.X), size1.Y + size2.Y);

            float lineExtraWidth = 4;
            float lineHeightSpacing = 4;

            InternalRenderExpression(g, functionCall.Arguments[0], pt + new Vector2(lineExtraWidth + (size.X - size1.X) / 2, 0), pen, brush, expressionSizeCache, font, drawBoxes);
            InternalRenderExpression(g, functionCall.Arguments[1], pt + new Vector2(lineExtraWidth + (size.X - size2.X) / 2, size1.Y + lineHeightSpacing), pen, brush, expressionSizeCache, font, drawBoxes);

            g.DrawLine(pen,
                pt.X,
                pt.Y + size1.Y + lineHeightSpacing / 2,
                pt.X + size.X + 2 * lineExtraWidth,
                pt.Y + size1.Y + lineHeightSpacing / 2);
        }

        protected static void RenderCloseParenthesis(IRenderer g, RectangleF rect, LPen pen, LBrush brush)
        {
            rect.Width -= 2;

            g.DrawArc(pen, rect.Left, rect.Top, rect.Width, 2 * rect.Width, 270, 90);
            g.DrawLine(pen, rect.Right, rect.Top + rect.Width, rect.Right, rect.Bottom - rect.Width);
            g.DrawArc(pen, rect.Left, rect.Bottom - 2 * rect.Width, rect.Width, 2 * rect.Width, 0, 90);
        }

        protected static void RenderOpenParenthesis(IRenderer g, RectangleF rect, LPen pen, LBrush brush)
        {
            rect.X += 2;
            rect.Width -= 2;

            g.DrawArc(pen, rect.Left, rect.Top, rect.Width, 2 * rect.Width, 180, 90);
            g.DrawLine(pen, rect.Left, rect.Top + rect.Width, rect.Left, rect.Bottom - rect.Width);
            g.DrawArc(pen, rect.Left, rect.Bottom - 2 * rect.Width, rect.Width, 2 * rect.Width, 90, 90);
        }

        public static Vector2 CalcExpressionSize(Expression expr, IRenderer g, LFont font)
        {
            return CalcExpressionSize(expr, g, font, new Dictionary<Expression, Vector2>());
        }

        protected static Vector2 CalcExpressionSize(Expression expr, IRenderer g, LFont font, Dictionary<Expression, Vector2> expressionSizeCache)
        {
            if (expressionSizeCache.ContainsKey(expr))
            {
                return expressionSizeCache[expr];
            }

            Vector2 size;

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

                var size2 = g.MeasureString(upperString, font);
                var size3 = g.MeasureString(lowerString, font);

                size = new Vector2(Math.Max(size2.X, size3.X), size2.Y + size3.Y + 2);
            }
            else if (expr is ColorExpression)
            {
                var literal = new Literal(expr.Eval(null));
                size = CalcExpressionSize(literal, g, font, expressionSizeCache);
            }
            else if (expr is RandomExpression)
            {
                size = g.MeasureString("rand()", font);
            }
            else if (expr is MatrixExpression expr2)
            {
                List<float> maxWidthPerColumn = new List<float>();
                List<float> maxHeightPerRow = new List<float>();

                CalcMatrixWidthsAndHeights(g, expr2, maxWidthPerColumn, maxHeightPerRow, expressionSizeCache, font);
                size = CalcMatrixSizeFromMaxWidthsAndHeights(maxWidthPerColumn, maxHeightPerRow);
            }
            else if (expr is VectorExpression vector)
            {
                int i;

                float width = 0;
                float height = 0;
                Vector2 exprSize;

                for (i = 0; i < vector.Length; i++)
                {
                    exprSize = CalcExpressionSize(vector[i], g, font, expressionSizeCache);
                    width += exprSize.X + 2;
                    height = Math.Max(height, exprSize.Y);
                }

                size = new Vector2(width + 4, height + 4);
            }
            else
            {
                throw new InvalidOperationException();
            }

            //margin
            size += new Vector2(4, 4);

            expressionSizeCache[expr] = size;

            return size;
        }

        private static Vector2 CalcMatrixSizeFromMaxWidthsAndHeights(List<float> maxWidthPerColumn, List<float> maxHeightPerRow)
        {
            Vector2 size;

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

            size = new Vector2(width, height);
            return size;
        }

        private static void CalcMatrixWidthsAndHeights(IRenderer g,
            MatrixExpression matrix, List<float> maxWidthPerColumn,
            List<float> maxHeightPerRow,
            Dictionary<Expression, Vector2> expressionSizeCache, LFont font)
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
                    var size = CalcExpressionSize(matrix[i, j], g, font, expressionSizeCache);

                    size += new Vector2(4, 4);

                    maxWidthPerColumn[j] = Math.Max(maxWidthPerColumn[j], size.X);
                    maxHeightPerRow[i] = Math.Max(maxHeightPerRow[i], size.Y);
                }
            }
        }

        private static Vector2 CalcFunctionCallSize(IRenderer g, Expression expr, Dictionary<Expression, Vector2> expressionSizeCache, LFont font)
        {
            Vector2 size;
            FunctionCall functionCall = expr as FunctionCall;

            if (functionCall.Function is Operation)
            {
                if (functionCall.Function is BinaryOperation)
                {
                    if (functionCall.Function is DivisionOperation)
                    {
                        var topOperandSize = CalcExpressionSize(functionCall.Arguments[0], g, font, expressionSizeCache);
                        var bottomOperandSize = CalcExpressionSize(functionCall.Arguments[1], g, font, expressionSizeCache);

                        float width = Math.Max(topOperandSize.X, bottomOperandSize.X);
                        float height = topOperandSize.Y + bottomOperandSize.Y;
                        float lineExtraWidth = 2 * 4;
                        float lineHeightSpacing = 4;

                        size = new Vector2(width + lineExtraWidth, height + lineHeightSpacing);
                    }
                    else if (ExpressionItem.IsRootOperation(functionCall))
                    {
                        Literal root = (Literal)functionCall.Arguments[1];
                        var invRoot = (float) Math.Round(
                            1 / root.Value.ToNumber().Value);
                        var invRootLiteral = new Literal(invRoot.ToNumber());
                        Expression arg = functionCall.Arguments[0];
                        var argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);
                        var rootSize = new Vector2(0, 0);

                        if (invRoot > 2)
                        {
                            rootSize = CalcExpressionSize(invRootLiteral, g,
                                font);
                        }

                        float longRadicalLineWidth = 10;

                        size = new Vector2(rootSize.X + longRadicalLineWidth + argSize.X + 2, argSize.Y + 4);
                    }
                    else
                    {
                        var operatorSymbol = functionCall.Function.DisplayName;

                        var leftOperandSize = CalcExpressionSize(functionCall.Arguments[0], g, font, expressionSizeCache);
                        var rightOperandSize = CalcExpressionSize(functionCall.Arguments[1], g, font, expressionSizeCache);
                        var operatorSymbolSize = g.MeasureString(operatorSymbol, font);

                        float parenWidth = 10;
                        if (NeedsLeftParen(functionCall))
                        {
                            leftOperandSize = leftOperandSize.AddX(parenWidth * 2);
                        }
                        if (NeedsRightParen(functionCall))
                        {
                            rightOperandSize = rightOperandSize.AddX(parenWidth * 2);
                        }

                        float width = leftOperandSize.X + rightOperandSize.X + operatorSymbolSize.X;
                        float height = Math.Max(Math.Max(leftOperandSize.Y, rightOperandSize.Y), operatorSymbolSize.Y);

                        size = new Vector2(width, height);
                    }
                }
                else if (functionCall.Function is AssociativeCommutativeOperation)
                {
                    AssociativeCommutativeOperation operation = functionCall.Function as AssociativeCommutativeOperation;
                    string symbol = operation.DisplayName;
                    var symbolSize = g.MeasureString(symbol, font);

                    float parenWidth = 10;

                    size = new Vector2(0, 0);

                    bool first = true;
                    foreach (Expression arg in functionCall.Arguments)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            size = size.AddX(symbolSize.X);
                        }

                        var argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);

                        if (arg is FunctionCall &&
                            (arg as FunctionCall).Function is Operation &&
                            ((arg as FunctionCall).Function as Operation).Precedence < (operation as Operation).Precedence)
                        {
                            argSize = argSize.AddX(parenWidth * 2);
                        }

                        size = new Vector2(
                            size.X + argSize.X,
                            Math.Max(size.Y, argSize.Y));
                    }

                }
                else if (functionCall.Function == NegationOperation.Value)
                {
                    string symbol = NegationOperation.Value.DisplayName;
                    var symbolSize = g.MeasureString(symbol, font);

                    float parenWidth = 10;

                    var arg = functionCall.Arguments[0];
                    var argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);

                    float widthWithParen;
                    if (arg is FunctionCall &&
                        (arg as FunctionCall).Function is Operation &&
                        ((arg as FunctionCall).Function as Operation).Precedence < NegationOperation.Value.Precedence)
                    {
                        widthWithParen = argSize.X + parenWidth * 2;
                    }
                    else
                    {
                        widthWithParen = argSize.X;
                    }

                    return new Vector2(widthWithParen, argSize.Y);
                }
                else
                {
                    throw new InvalidOperationException("Unknown Operation: " + functionCall.Function.ToString());
                }
            }
            else
            {
                Vector2 displayNameSize;
                Vector2 openParenSize;
                Vector2 closeParenSize;
                Vector2 commaSize;
                Vector2 allArgSize;

                displayNameSize = g.MeasureString(functionCall.Function.DisplayName, font) + new Vector2(2, 0);
                openParenSize = g.MeasureString("(", font);
                closeParenSize = g.MeasureString(")", font);
                commaSize = g.MeasureString(", ", font);

                allArgSize = new Vector2(0, 0);

                bool first = true;
                foreach (Expression arg in functionCall.Arguments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        allArgSize = allArgSize.AddX(commaSize.X);
                    }
                    var argSize = CalcExpressionSize(arg, g, font, expressionSizeCache);
                    allArgSize = new Vector2(
                        allArgSize.X + argSize.X,
                        Math.Max(allArgSize.Y, argSize.Y));
                }

                float width;
                float height;

                width = displayNameSize.X + openParenSize.X + allArgSize.X + closeParenSize.X;
                height = Math.Max(Math.Max(Math.Max(displayNameSize.Y, openParenSize.Y), commaSize.Y), allArgSize.Y);

                size = new Vector2(width, height);
            }
            return size;
        }
    }
}

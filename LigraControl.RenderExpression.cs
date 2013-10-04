using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraControl : UserControl
    {

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
            else if (expr is AssignExpression)
            {
                RenderAssignExpression(g, (AssignExpression)expr, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else if (expr is DelayAssignExpression)
            {
                RenderDelayAssignExpression(g, (DelayAssignExpression)expr, pt, pen, brush, expressionSizeCache, font, drawBoxes);
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

        protected static void RenderDelayAssignExpression(Graphics g, DelayAssignExpression delayAssignExpression, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font, bool drawBoxes)
        {

            string varText = delayAssignExpression.Variable.Name + " := ";
            SizeF varTextSize = g.MeasureString(varText, font) + new SizeF(2, 0);
            float frontWidth = varTextSize.Width;
            SizeF valueSize = CalcExpressionSize(delayAssignExpression.Expression, g, font, expressionSizeCache);

            g.DrawString(varText, font, brush, pt + new SizeF(2, (valueSize.Height - varTextSize.Height) / 2));
            InternalRenderExpression(g, delayAssignExpression.Expression, pt + new SizeF(frontWidth, 0), pen, brush, expressionSizeCache, font, drawBoxes);
        }

        protected static void RenderAssignExpression(Graphics g, AssignExpression assignExpression, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, Font font, bool drawBoxes)
        {

            string varText = assignExpression.Variable + " = ";
            SizeF varTextSize = g.MeasureString(varText, font) + new SizeF(2, 0);
            float frontWidth = varTextSize.Width;
            SizeF valueSize = CalcExpressionSize(assignExpression.Value, g, font, expressionSizeCache);

            g.DrawString(varText, font, brush, pt + new SizeF(2, (valueSize.Height - varTextSize.Height) / 2));
            InternalRenderExpression(g, assignExpression.Value, pt + new SizeF(frontWidth, 0), pen, brush, expressionSizeCache, font, drawBoxes);
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
            Dictionary<Variable, int> lowerOrders = new Dictionary<Variable, int>();

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
            if (literal.Value == Math.PI)
            {
                str = "π";
            }
            else if (literal.Value == Math.E)
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

            Set<Expression> hasParens = new Set<Expression>();

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
                RenderDivisionOperation(g, functionCall, pt, pen, brush, expressionSizeCache, drawBoxes,font);
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

        protected static void RenderExponentOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache, bool drawBoxes, Font font)
        {

            if (IsRootOperation(functionCall))
            {
                RenderRootOperation(g, functionCall, pt, pen, brush, expressionSizeCache, font, drawBoxes);
            }
            else
            {
                RenderPartBinaryOperation(g, functionCall, pt, pen, brush, expressionSizeCache, drawBoxes, font);
            }
        }

        private static bool IsRootOperation(FunctionCall functionCall)
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
                (functionCall.Arguments[1] is FunctionCall && (functionCall.Arguments[1] as FunctionCall).Function is DivisionOperation) ||
                (functionCall.Arguments[1] is DelayAssignExpression);
        }

        private static bool NeedsLeftParen(FunctionCall functionCall)
        {
            return (functionCall.Arguments[0] is FunctionCall &&
                (functionCall.Arguments[0] as FunctionCall).Function is Operation &&
                ((functionCall.Arguments[0] as FunctionCall).Function as Operation).Precedence < (functionCall.Function as Operation).Precedence) ||
                (functionCall.Arguments[0] is FunctionCall && (functionCall.Arguments[0] as FunctionCall).Function is ExponentOperation) ||
                (functionCall.Arguments[0] is FunctionCall && (functionCall.Arguments[0] as FunctionCall).Function is DivisionOperation) || 
                (functionCall.Arguments[0] is DelayAssignExpression);
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

    }
}
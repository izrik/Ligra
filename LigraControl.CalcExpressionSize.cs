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
            else if (expr is AssignExpression)
            {
                AssignExpression expr2 = (AssignExpression)expr;

                size = g.MeasureString(expr2.Variable + " = ", font);

                SizeF size2 = CalcExpressionSize(expr2.Value, g, font, expressionSizeCache);
                size.Width += size2.Width;
                size.Height = Math.Max(size.Height, size2.Height);
            }
            else if (expr is DelayAssignExpression)
            {
                DelayAssignExpression expr2 = (DelayAssignExpression)expr;

                size = g.MeasureString(expr2.Variable + " := ", font);

                SizeF size2 = CalcExpressionSize(expr2.Expression, g, font, expressionSizeCache);
                size.Width += size2.Width;
                size.Height = Math.Max(size.Height, size2.Height);
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
                    else if (IsRootOperation(functionCall))
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
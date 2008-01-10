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
        public LigraControl()
        {
            InitializeComponent();
        }

        private bool _drawBoxes = false;
        public bool DrawBoxes
        {
            get { return _drawBoxes; }
            set { _drawBoxes = value; }
        }


        public virtual void RenderExpression(Graphics g, Expression expr, PointF pt, Pen pen, Brush brush)
        {
            InternalRenderExpression(g, expr, pt, pen, brush, new Dictionary<Expression,SizeF>());
        }

        protected virtual void InternalRenderExpression(Graphics g, Expression expr, PointF pt, Pen pen, Brush brush, Dictionary<Expression,SizeF> expressionSizeCache)
        {
            SizeF size = CalcExpressionSize(g, expr, expressionSizeCache);

            if (DrawBoxes)
            {
                g.DrawRectangle(Pens.Red, pt.X, pt.Y, size.Width, size.Height);
            }

            pt += new SizeF(2, 2);

            if (expr is FunctionCall)
            {
                RenderFunctionCallExpression(g, expr as FunctionCall, pt, pen, brush, expressionSizeCache);
            }
            else if (expr is Literal)
            {
                RenderLiteralExpression(g, expr as Literal, pt, pen, brush, expressionSizeCache);
            }
            else if (expr is VariableAccess)
            {
                RenderVariableAccess(g, expr as VariableAccess, pt, pen, brush, expressionSizeCache);
            }
            else
            {
                throw new InvalidOperationException("Unknown expression type: " + expr.ToString());
            }
        }

        protected void RenderVariableAccess(Graphics g, VariableAccess variableAccess, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache)
        {
            g.DrawString(variableAccess.Variable.Name, Font, brush, pt);
        }

        protected void RenderLiteralExpression(Graphics g, Literal literal, PointF pt, Pen pen, Brush brush, Dictionary<Expression,SizeF> expressionSizeCache)
        {
            g.DrawString(literal.Value.ToString("G"), Font, new SolidBrush(pen.Color), pt);
        }

        protected void RenderFunctionCallExpression(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression,SizeF> expressionSizeCache)
        {
            if (functionCall.Function is Operation)
            {
                RenderOperation(g, functionCall, pt, pen, brush, expressionSizeCache);
            }
            else
            {
                SizeF displayNameSize = g.MeasureString(functionCall.Function.DisplayName, Font) + new SizeF(2 ,0);
                float parenWidth = g.MeasureString("(", Font).Width;// 10;
                SizeF commaSize = g.MeasureString(", ", Font);

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

                    SizeF argSize = CalcExpressionSize(g, argument, expressionSizeCache);

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
                        g.DrawString(", ", Font, brush, pt + new SizeF(frontWidth + currentXOffset, (allArgSize.Height - commaSize.Height) / 2));
                        currentXOffset += commaSize.Width;
                    }

                    SizeF argSize = CalcExpressionSize(g, argument, expressionSizeCache);

                    InternalRenderExpression(g, argument, pt + new SizeF(frontWidth + currentXOffset, (allArgSize.Height - argSize.Height) / 2), pen, brush, expressionSizeCache);
                    currentXOffset += argSize.Width;
                }

                RectangleF rect = new RectangleF(pt.X + displayNameSize.Width + 2, pt.Y, parenWidth, allArgSize.Height);
                RenderOpenParenthesis(g, rect, pen, brush);
                rect.X += allArgSize.Width + parenWidth;
                RenderCloseParenthesis(g, rect, pen, brush);

                g.DrawString(functionCall.Function.DisplayName, Font, brush, pt + new SizeF(2, allArgSize.Height / 2 - displayNameSize.Height / 2));
            }
        }

        private void RenderOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache)
        {
            if (functionCall.Function is BinaryOperation)
            {
                RenderBinaryOperation(g, functionCall, pt, pen, brush, expressionSizeCache);
            }
            else if (functionCall.Function is AssociativeCommutativOperation)
            {
                RenderAssociativeCommutativOperation(g, functionCall, pt, pen, brush, expressionSizeCache);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void RenderAssociativeCommutativOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache)
        {
            AssociativeCommutativOperation operation = functionCall.Function as AssociativeCommutativOperation;

            string symbol = operation.DisplayName;
            SizeF symbolSize = g.MeasureString(symbol, Font);

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

                SizeF argSize = CalcExpressionSize(g, arg, expressionSizeCache);

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
                    g.DrawString(symbol, Font, brush, x, pt.Y + (allArgSize.Height - symbolSize.Height) / 2);
                    x += symbolSize.Width;
                }

                SizeF argSize = CalcExpressionSize(g, arg, expressionSizeCache);

                if (hasParens.Contains(arg))
                {
                    parenRect.X = x;
                    parenRect.Y = pt.Y + (allArgSize.Height - argSize.Height) / 2;
                    parenRect.Height = argSize.Height;
                    RenderOpenParenthesis(g, parenRect, pen, brush);
                    x += parenRect.Width;
                }

                InternalRenderExpression(g, arg, new PointF(x, pt.Y + (allArgSize.Height - argSize.Height) / 2), pen, brush, expressionSizeCache);
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

        private void RenderBinaryOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache)
        {
            if (functionCall.Function is DivisionOperation)
            {
                RenderDivisionOperation(g, functionCall, pt, pen, brush, expressionSizeCache);
            }
            else
            {
                string symbol = //" " + 
                    functionCall.Function.DisplayName
                    //+ " "
                    ;

                SizeF leftOperandSize = CalcExpressionSize(g, functionCall.Arguments[0], expressionSizeCache);
                SizeF rightOperandSize = CalcExpressionSize(g, functionCall.Arguments[1], expressionSizeCache);
                SizeF symbolSize = g.MeasureString(symbol, Font);
                float maxHeight = Math.Max(Math.Max(leftOperandSize.Height, rightOperandSize.Height), symbolSize.Height);

                float parenWidth = 10;

                bool leftParens = false;
                bool rightParens = false;

                if (functionCall.Arguments[0] is FunctionCall &&
                    (functionCall.Arguments[0] as FunctionCall).Function is Operation &&
                    ((functionCall.Arguments[0] as FunctionCall).Function as Operation).Precedence < (functionCall.Function as Operation).Precedence)
                {
                    //leftOperandSize.Width += parenWidth * 2;
                    leftParens = true;
                }

                if (functionCall.Arguments[1] is FunctionCall &&
                    (functionCall.Arguments[1] as FunctionCall).Function is Operation &&
                    ((functionCall.Arguments[1] as FunctionCall).Function as Operation).Precedence < (functionCall.Function as Operation).Precedence)
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

                InternalRenderExpression(g, functionCall.Arguments[0], new PointF(x, y), pen, brush, expressionSizeCache);
                x += leftOperandSize.Width;

                if (leftParens)
                {
                    parenRect.X = x;
                    RenderCloseParenthesis(g, parenRect, pen, brush);
                    x += parenWidth;
                }




                y = pt.Y + (maxHeight - symbolSize.Height) / 2;
                g.DrawString(symbol, Font, brush, new PointF(x, y));
                if (DrawBoxes)
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

                InternalRenderExpression(g, functionCall.Arguments[1], new PointF(x, y), pen, brush, expressionSizeCache);
                x += rightOperandSize.Width;

                if (rightParens)
                {
                    parenRect.X = x;
                    RenderCloseParenthesis(g, parenRect, pen, brush);
                    x += parenWidth;
                }
            }
        }

        private void RenderDivisionOperation(Graphics g, FunctionCall functionCall, PointF pt, Pen pen, Brush brush, Dictionary<Expression, SizeF> expressionSizeCache)
        {
            SizeF size1 = CalcExpressionSize(g, functionCall.Arguments[0], expressionSizeCache);
            SizeF size2 = CalcExpressionSize(g, functionCall.Arguments[1], expressionSizeCache);
            SizeF size = new SizeF(Math.Max(size1.Width, size2.Width), size1.Height + size2.Height);

            float lineExtraWidth = 4;
            float lineHeightSpacing = 4;

            InternalRenderExpression(g, functionCall.Arguments[0], pt + new SizeF(lineExtraWidth + (size.Width - size1.Width) / 2, 0), pen, brush, expressionSizeCache);
            InternalRenderExpression(g, functionCall.Arguments[1], pt + new SizeF(lineExtraWidth + (size.Width - size2.Width) / 2, size1.Height + lineHeightSpacing), pen, brush, expressionSizeCache);

            g.DrawLine(pen,
                pt.X, 
                    pt.Y + size1.Height + lineHeightSpacing / 2,
                pt.X + size.Width +  2 *lineExtraWidth, 
                    pt.Y + size1.Height + lineHeightSpacing / 2);
        }

        protected void RenderCloseParenthesis(Graphics g, RectangleF rect, Pen pen, Brush brush)
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

        protected void RenderOpenParenthesis(Graphics g, RectangleF rect, Pen pen, Brush brush)
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

        public SizeF CalcExpressionSize(Graphics g, Expression expr)
        {
            return CalcExpressionSize(g, expr, new Dictionary<Expression, SizeF>());
        }

        protected SizeF CalcExpressionSize(Graphics g, Expression expr, Dictionary<Expression,SizeF> expressionSizeCache)
        {
            if (expressionSizeCache.ContainsKey(expr))
            {
                return expressionSizeCache[expr];
            }

            SizeF size;

            if (expr is FunctionCall)
            {
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
                            SizeF topOperandSize = CalcExpressionSize(g, functionCall.Arguments[0], expressionSizeCache);
                            SizeF bottomOperandSize = CalcExpressionSize(g, functionCall.Arguments[1], expressionSizeCache);

                            float width = Math.Max(topOperandSize.Width, bottomOperandSize.Width);
                            float height = topOperandSize.Height + bottomOperandSize.Height;
                            float lineExtraWidth = 2 * 4;
                            float lineHeightSpacing = 4;

                            size = new SizeF(width + lineExtraWidth, height + lineHeightSpacing);
                        }
                        else
                        {
                            string operatorSymbol = //" " + 
                                functionCall.Function.DisplayName
                                //+ " "
                                ;

                            SizeF leftOperandSize = CalcExpressionSize(g, functionCall.Arguments[0], expressionSizeCache);
                            SizeF rightOperandSize = CalcExpressionSize(g, functionCall.Arguments[1], expressionSizeCache);
                            SizeF operatorSymbolSize = g.MeasureString(operatorSymbol, Font);

                            float parenWidth = 10;

                            if (functionCall.Arguments[0] is FunctionCall &&
                                (functionCall.Arguments[0] as FunctionCall).Function is Operation &&
                                ((functionCall.Arguments[0] as FunctionCall).Function as Operation).Precedence < (functionCall.Function as Operation).Precedence)
                            {
                                leftOperandSize.Width += parenWidth * 2;
                            }

                            if (functionCall.Arguments[1] is FunctionCall &&
                                (functionCall.Arguments[1] as FunctionCall).Function is Operation &&
                                ((functionCall.Arguments[1] as FunctionCall).Function as Operation).Precedence < (functionCall.Function as Operation).Precedence)
                            {
                                rightOperandSize.Width += parenWidth * 2;
                            }

                            float width = leftOperandSize.Width + rightOperandSize.Width + operatorSymbolSize.Width;
                            float height = Math.Max(Math.Max(leftOperandSize.Height, rightOperandSize.Height), operatorSymbolSize.Height);

                            size = new SizeF(width, height);
                        }
                    }
                    else if (functionCall.Function is AssociativeCommutativOperation)
                    {
                        AssociativeCommutativOperation operation = functionCall.Function as AssociativeCommutativOperation;
                        string symbol = operation.DisplayName;
                        SizeF symbolSize = g.MeasureString(symbol, Font);

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

                            SizeF argSize = CalcExpressionSize(g, arg, expressionSizeCache);

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

                    displayNameSize = g.MeasureString(functionCall.Function.DisplayName, Font) + new SizeF(2, 0);
                    openParenSize = g.MeasureString("(", Font);
                    closeParenSize = g.MeasureString(")", Font);
                    commaSize = g.MeasureString(", ", Font);

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
                        SizeF argSize = CalcExpressionSize(g, arg, expressionSizeCache);
                        allArgSize.Width += argSize.Width;
                        allArgSize.Height = Math.Max(allArgSize.Height, argSize.Height);
                    }

                    float width;
                    float height;

                    width = displayNameSize.Width + openParenSize.Width + allArgSize.Width + closeParenSize.Width;
                    height = Math.Max(Math.Max(Math.Max(displayNameSize.Height, openParenSize.Height), commaSize.Height), allArgSize.Height);

                    size = new SizeF(width, height);
                }
            }
            else if (expr is Literal)
            {
                size = g.MeasureString((expr as Literal).Value.ToString("G"), Font);
            }
            else if (expr is VariableAccess)
            {
                size = g.MeasureString((expr as VariableAccess).Variable.Name, Font);
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

        private void drawBoxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawBoxes = !DrawBoxes;
            drawBoxesToolStripMenuItem.Checked = DrawBoxes;
            Invalidate();
        }

        public void RenderGraph(Graphics g, RectangleF boundsInClient, 
                        Pen pen, Brush brush, 
                        float xMin, float xMax, float yMin, float yMax, 
                        Expression expr, Variable independentVariable,
                        VariableTable varTable, 
                        bool drawboundaries)
        {
            SolusEngine engine = new SolusEngine();

            float deltaX = (xMax - xMin) / boundsInClient.Width;
            float deltaY = boundsInClient.Height / (yMax - yMin);

            if (drawboundaries)
            {
                g.DrawRectangle(Pens.Black, boundsInClient.X, boundsInClient.Y, boundsInClient.Width, boundsInClient.Height);

                if (xMax > 0 && xMin < 0)
                {
                    float ii = -xMin / deltaX + boundsInClient.X;
                    g.DrawLine(Pens.Black, ii, boundsInClient.Top, ii, boundsInClient.Bottom);
                }

                if (yMax > 0 && yMin < 0)
                {
                    float y = boundsInClient.Bottom + yMin * deltaY;
                    g.DrawLine(Pens.Black, boundsInClient.Left, y, boundsInClient.Right, y);
                }
            }

            varTable[independentVariable] = xMin;
            PointF lastPoint = new PointF(boundsInClient.Left, boundsInClient.Bottom - (Math.Max(Math.Min(engine.Eval(expr, varTable).Value, yMax), yMin) - yMin) * deltaY);

            int i;
            for (i = 0; i < boundsInClient.Width; i++)
            {
                float x = xMin + deltaX * i;
                varTable[independentVariable] = x;
                float value = engine.Eval(expr, varTable).Value;
                value = Math.Min(value, yMax);
                value = Math.Max(value, yMin);
                float y = boundsInClient.Bottom - (value - yMin) * deltaY;

                PointF pt = new PointF(i + boundsInClient.X, y);

                g.DrawLine(pen, lastPoint, pt);
                lastPoint = pt;
            }
        }

        public void Render3DGraph(Graphics g, RectangleF boundsInClient,
                        Pen pen, Brush brush,
                        float xMin, float xMax, 
                        float yMin, float yMax,
                        float zMin, float zMax,
                        Expression expr,
                        Variable independentVariableX,
                        Variable independentVariableY,
                        VariableTable varTable,
                        bool drawboundaries)
        {
            SolusEngine engine = new SolusEngine();

            int xValues = 50;
            int yValues = 50;

            float[,] values = new float[xValues, yValues];

            float deltaX = (xMax - xMin) / (xValues - 1);
            float deltaY = (yMax - yMin) / (yValues - 1);

            float x0 = boundsInClient.Left;
            float x1 = boundsInClient.Left + boundsInClient.Width / 2;
            float x2 = boundsInClient.Right;

            float y0 = boundsInClient.Top;
            float y1 = boundsInClient.Top + boundsInClient.Height / 4;
            float y2 = boundsInClient.Top + boundsInClient.Height / 2;
            float y3 = boundsInClient.Top + 3 * boundsInClient.Height / 4;
            float y4 = boundsInClient.Bottom;

            if (drawboundaries)
            {
                g.DrawLine(Pens.Black, x1, y0, x2, y1);
                g.DrawLine(Pens.Black, x2, y1, x2, y3);
                g.DrawLine(Pens.Black, x2, y3, x1, y4);
                g.DrawLine(Pens.Black, x1, y4, x0, y3);
                g.DrawLine(Pens.Black, x0, y3, x0, y1);
                g.DrawLine(Pens.Black, x0, y1, x1, y0);

                g.DrawLine(Pens.Black, x1, y0, x1, y2);
                g.DrawLine(Pens.Black, x0, y3, x1, y2);
                g.DrawLine(Pens.Black, x2, y3, x1, y2);

                //g.DrawRectangle(Pens.Black, boundsInClient.Left, boundsInClient.Top, boundsInClient.Width, boundsInClient.Height);
            }


            int i;
            int j;
            float x;
            float y;
            float z;

            Expression prelimEval;
            Expression prelimEval2;

            if (varTable.ContainsKey(independentVariableX))
            {
                varTable.Remove(independentVariableX);
            }
            if (varTable.ContainsKey(independentVariableY))
            {
                varTable.Remove(independentVariableY);
            }

            prelimEval = engine.PreliminaryEval(expr, varTable);

            for (i = 0; i < xValues; i++)
            {
                x = xMin + i * deltaX;

                varTable[independentVariableX] = x;
                if (varTable.ContainsKey(independentVariableY))
                {
                    varTable.Remove(independentVariableY);
                }

                prelimEval2 = engine.PreliminaryEval(prelimEval, varTable);

                for (j = 0; j < yValues; j++)
                {
                    y = yMin + j * deltaY;
                    varTable[independentVariableY] = y;

                    z = engine.Eval(prelimEval2, varTable).Value;

                    z = Math.Max(z, zMin);
                    z = Math.Min(z, zMax);

                    values[i, j] = z;
                }
            }

            PointF[,] pts = new PointF[xValues, yValues];

            for (i = 0; i < xValues; i++)
            {
                float ii = (i / (float)xValues);
                for (j = 0; j < yValues; j++)
                {
                    float jj = (j / (float)yValues);

                    z = values[i, j];
                    x = x1 + (ii - jj) * boundsInClient.Width * 0.5f;
                    y = y4 - (((ii + jj) * boundsInClient.Height) / 4) - ((((z - zMin) / (zMax - zMin)) *  boundsInClient.Height) / 2);

                    pts[i, j] = new PointF(x, y);
                }
            }

            for (i = xValues - 2; i >= 0; i--)
            {
                for (j = yValues - 2; j >= 0; j--)
                {
                    PointF[] poly = { pts[i, j], pts[i+1, j], pts[i+1, j+1], pts[i, j+1] };
                    g.FillPolygon(brush, poly);
                    g.DrawPolygon(Pens.Black, poly);
                }
            }
        }
    }
}

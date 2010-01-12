/*****************************************************************************
 *                                                                           *
 *  LigraControl.cs                                                          *
 *  8 February 2008                                                          *
 *  Project: Ligra                                                           *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2008 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  Methods for rendering MathPaint images in Ligra.                         *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Collections;
using MetaphysicsIndustries.Utilities;
using System.Diagnostics;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraControl : UserControl
    {
        public void RenderMathPaint(Graphics g, RectangleF boundsInClient,
                        Expression expression,
                        Variable independentVariableX,
                        Variable independentVariableY,
                        int xStart, int xEnd,
                        int yStart, int yEnd,
                        VariableTable varTable)
        {
            MemoryImage image =
                RenderMathPaintToMemoryImage(
                    expression, 
                    independentVariableX, 
                    independentVariableY, 
                    xStart, xEnd, 
                    yStart, yEnd, 
                    varTable);

            g.DrawImage(image.Bitmap, boundsInClient);
        }

        public MemoryImage RenderMathPaintToMemoryImage(
            Expression expression, 
            Variable independentVariableX, 
            Variable independentVariableY, 
            int xStart, int xEnd,
            int yStart, int yEnd,
            VariableTable varTable)
        {
            int xValues = xEnd - xStart + 1;
            int yValues = yEnd - yStart + 1;

            double[,] values = new double[xValues, yValues];

            Expression prelimEval1;
            Expression prelimEval2;

            if (varTable.ContainsKey(independentVariableX))
            {
                varTable.Remove(independentVariableX);
            }
            if (varTable.ContainsKey(independentVariableY))
            {
                varTable.Remove(independentVariableY);
            }

            prelimEval1 = _engine.PreliminaryEval(expression, varTable);

            int i;
            int j;
            double z;

            MemoryImage image = new MemoryImage(xValues, yValues);
            //image.Size = new Size(xValues, yValues);

            for (i = 0; i < xValues; i++)
            {
                varTable[independentVariableX] = new Literal(i);
                if (varTable.ContainsKey(independentVariableY))
                {
                    varTable.Remove(independentVariableY);
                }

                prelimEval2 = _engine.PreliminaryEval(prelimEval1, varTable);

                for (j = 0; j < yValues; j++)
                {
                    varTable[independentVariableY] = new Literal(j);

                    z = prelimEval2.Eval(varTable).Value;

                    if (double.IsNaN(z))
                    {
                        z = 0;
                    }

                    image[j, i] = Color.FromArgb(255, Color.FromArgb((int)z));
                    //values[i, j] = z;
                }
            }

            image.CopyPixelsToBitmap();
            return image;
        }
    }
}
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

using MetaphysicsIndustries.Utilities;
using System.Diagnostics;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraControl : UserControl
    {
        public void RenderMathPaint(Graphics g, RectangleF boundsInClient,
                        Expression expression,
                        string independentVariableX,
                        string independentVariableY,
                        int xStart, int xEnd,
                        int yStart, int yEnd,
                        SolusEnvironment env)
        {
            MemoryImage image =
                RenderMathPaintToMemoryImage(
                    expression, 
                    independentVariableX, 
                    independentVariableY, 
                    xStart, xEnd, 
                    yStart, yEnd, 
                    env);

            g.DrawImage(image.Bitmap, boundsInClient);
        }

        public static MemoryImage RenderMathPaintToMemoryImage(
            Expression expression, 
            string independentVariableX, 
            string independentVariableY, 
            int xStart, int xEnd,
            int yStart, int yEnd,
            SolusEnvironment env)
        {
            int xValues = xEnd - xStart + 1;
            int yValues = yEnd - yStart + 1;

            double[,] values = new double[xValues, yValues];

            Expression prelimEval1;
            Expression prelimEval2;

            if (env.Variables.ContainsKey(independentVariableX))
            {
                env.Variables.Remove(independentVariableX);
            }
            if (env.Variables.ContainsKey(independentVariableY))
            {
                env.Variables.Remove(independentVariableY);
            }

            prelimEval1 = _engine.PreliminaryEval(expression, env);

            int i;
            int j;
            double z;

            MemoryImage image = new MemoryImage(xValues, yValues);
            //image.Size = new Size(xValues, yValues);

            for (i = 0; i < xValues; i++)
            {
                env.Variables[independentVariableX] = new Literal(i);
                if (env.Variables.ContainsKey(independentVariableY))
                {
                    env.Variables.Remove(independentVariableY);
                }

                prelimEval2 = _engine.PreliminaryEval(prelimEval1, env);

                for (j = 0; j < yValues; j++)
                {
                    env.Variables[independentVariableY] = new Literal(j);

                    z = prelimEval2.Eval(env).Value;

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
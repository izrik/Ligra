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
        public void RenderMatrix(Graphics g, RectangleF boundsInClient,
                        SolusMatrix matrix,
                        VariableTable varTable)
        {
            MemoryImage image =
                RenderMatrixToMemoryImage(
                    matrix,
                    varTable);

            g.DrawImage(image.Bitmap, Rectangle.Truncate( boundsInClient));
        }

        public MemoryImage RenderMatrixToMemoryImage(
            SolusMatrix matrix,
            VariableTable varTable)
        {
            int i;
            int j;
            double z;
            int r;
            int g;
            int b;

            MemoryImage image = new MemoryImage();
            image.Size = new Size(matrix.RowCount, matrix.ColumnCount);

            for (i = 0; i < matrix.RowCount; i++)
            {
                for (j = 0; j < matrix.ColumnCount; j++)
                {
                    z = matrix[i, j].Eval(varTable).Value;

                    if (double.IsNaN(z))
                    {
                        z = 0;
                    }

                    b = 0x000000FF & (int)z;
                    g = (0x0000FF00 & (int)z) >> 8;
                    r = (0x00FF0000 & (int)z) >> 16;

                    image[i, j] = Color.FromArgb(255, r, g, b);
                }
            }

            image.CopyPixelsToBitmap();
            return image;
        }

        public void RenderMatrix(Graphics g, RectangleF boundsInClient, Matrix matrix)
        {
            MemoryImage image = RenderMatrixToMemoryImage(matrix);

            g.DrawImage(image.Bitmap, Rectangle.Truncate(boundsInClient));
        }


        public MemoryImage RenderMatrixToMemoryImage(Matrix matrix)
        {
            return RenderMatrixToMemoryImageS(matrix);
        }

        public static Bitmap RenderMatrixToBitmapS(Matrix matrix)
        {
            return RenderMatrixToMemoryImageS(matrix).Bitmap;
        }

        public static MemoryImage RenderMatrixToMemoryImageS(Matrix matrix)
        {
            int i;
            int j;
            double z;
            int r;
            int g;
            int b;

            MemoryImage image = new MemoryImage();
            image.Size = new Size(matrix.RowCount, matrix.ColumnCount);

            for (i = 0; i < matrix.RowCount; i++)
            {
                for (j = 0; j < matrix.ColumnCount; j++)
                {
                    z = matrix[i, j];

                    if (double.IsNaN(z))
                    {
                        z = 0;
                    }

                    b = 0x000000FF & (int)z;
                    g = (0x0000FF00 & (int)z) >> 8;
                    r = (0x00FF0000 & (int)z) >> 16;

                    image[i, j] = Color.FromArgb(255, r, g, b);
                }
            }

            image.CopyPixelsToBitmap();
            return image;
        }
    }
}
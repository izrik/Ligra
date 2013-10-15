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
using MetaphysicsIndustries.Acuity;
using Environment = MetaphysicsIndustries.Solus.Environment;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraControl : UserControl
    {
        public void RenderMatrix(Graphics g, RectangleF boundsInClient,
                        SolusMatrix matrix,
                        Environment env)
        {
            MemoryImage image =
                RenderMatrixToMemoryImage(
                    matrix,
                    env);

            g.DrawImage(image.Bitmap, Rectangle.Truncate( boundsInClient));
        }

        public MemoryImage RenderMatrixToMemoryImage(
            SolusMatrix matrix,
            Environment env)
        {
            int i;
            int j;
            double z;
            int r;
            int g;
            int b;

            MemoryImage image = new MemoryImage(matrix.ColumnCount, matrix.RowCount);
            //image.Size = new Size(matrix.RowCount, matrix.ColumnCount);

            for (i = 0; i < matrix.RowCount; i++)
            {
                for (j = 0; j < matrix.ColumnCount; j++)
                {
                    z = matrix[i, j].Eval(env).Value;

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
        public static Bitmap RenderMatrixToColorBitmapS(Matrix r, Matrix g, Matrix b)
        {
            return RenderMatrixToMemoryImageColorS(r, g, b).Bitmap;
        }

        public static MemoryImage RenderMatrixToMemoryImageColorS(Matrix rr, Matrix gg, Matrix bb)
        {
            int i;
            int j;
            double zr;
            double zg;
            double zb;
            int r;
            int g;
            int b;

            if (rr.ColumnCount != gg.ColumnCount ||
                rr.ColumnCount != bb.ColumnCount ||
                gg.ColumnCount != bb.ColumnCount ||
                rr.RowCount != gg.RowCount ||
                rr.RowCount != bb.RowCount ||
                gg.RowCount != bb.RowCount)
            {
                throw new InvalidOperationException("Input channels must be the same size");
            }

            MemoryImage image = new MemoryImage(rr.ColumnCount, rr.RowCount);
            //image.Size = new Size(rr.RowCount, rr.ColumnCount);

            for (i = 0; i < rr.RowCount; i++)
            {
                for (j = 0; j < rr.ColumnCount; j++)
                {
                    zr = rr[i, j];
                    zg = gg[i, j];
                    zb = bb[i, j];

                    if (double.IsNaN(zr)) { zr = 0; }
                    if (double.IsNaN(zg)) { zg = 0; }
                    if (double.IsNaN(zb)) { zb = 0; }

                    b = 0xFF & (int)(zb*255);
                    g = 0xFF & (int)(zg*255);
                    r = 0xFF & (int)(zr*255);

                    image[i, j] = Color.FromArgb(255, r, g, b);
                }
            }

            image.CopyPixelsToBitmap();
            return image;
        }

        public static MemoryImage RenderMatrixToMemoryImageS(Matrix matrix)
        {
            int i;
            int j;
            double z;
            int r;
            int g;
            int b;

            MemoryImage image = new MemoryImage(matrix.ColumnCount,matrix.RowCount);
            //image.Size = new Size(matrix.RowCount, matrix.ColumnCount);

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

                    image[i,j] = Color.FromArgb(255, r, g, b);
                }
            }

            image.CopyPixelsToBitmap();
            return image;
        }
    }
}
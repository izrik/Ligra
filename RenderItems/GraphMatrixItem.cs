using System;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Acuity;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class GraphMatrixItem : RenderItem
    {
        public GraphMatrixItem(Matrix matrix, string caption,
            LigraEnvironment env)
            : base(env)
        {
            _matrix = matrix.Clone();
            _caption = caption;
        }

        private Matrix _matrix;
        public Matrix Matrix
        {
            get { return _matrix; }
        }

        public string _caption;

        public MemoryImage _image = null;

        protected override void InternalRender(IRenderer g,
            SolusEnvironment env)
        {
            RectangleF boundsInClient = new RectangleF(0, 0,
                Matrix.ColumnCount, Matrix.RowCount);

            if (_image == null || HasChanged(env))
            {
                MemoryImage image =
                    GraphMatrixItem.RenderMatrixToMemoryImage(Matrix);

                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }

                _image = image;
            }

            RectangleF rect = boundsInClient;
            rect.Size = new SizeF(GetImageWidth(),
                GetImageHeight());
            g.DrawImage(_image, rect);

            SizeF textSize = g.MeasureString(_caption, _env.Font,
                GetImageWidth());
            float textWidth = textSize.Width;
            float textHeight = textSize.Height;
            rect = new RectangleF(0, GetImageHeight() + 2, textWidth,
                textHeight);
            g.DrawString(_caption, _env.Font, LBrush.Black, rect);
        }

        protected override Vector2 InternalCalcSize(IRenderer g)
        {
            var width = GetImageWidth();
            var captionSize = g.MeasureString(_caption, _env.Font,
                GetImageWidth());

            return new Vector2(
                width,
                GetImageHeight() + captionSize.Y + 2);
        }

        public int GetImageHeight()
        {
            if (_matrix.RowCount < 256)
            {
                return _matrix.RowCount * 2;
            }
            else
            {
                return _matrix.RowCount;
            }
        }

        public int GetImageWidth()
        {
            if (_matrix.ColumnCount < 256)
            {
                return _matrix.ColumnCount * 2;
            }
            else
            {
                return _matrix.ColumnCount;
            }
        }

        protected override void AddVariablesForValueCollection(
            HashSet<string> vars)
        {
            //foreach (Expression expr in _matrix)
            //{
            //    GatherVariablesForValueCollection(vars, expr);
            //}
        }

        public override bool HasChanged(SolusEnvironment env)
        {
            return false;
        }

        public static void RenderMatrix(Graphics g, RectangleF boundsInClient,
            SolusMatrix matrix,
            SolusEnvironment env)
        {
            MemoryImage image =
                GraphMatrixItem.RenderMatrixToMemoryImage(
                    matrix,
                    env);

            g.DrawImage(image.Bitmap, Rectangle.Truncate(boundsInClient));
        }

        public static MemoryImage RenderMatrixToMemoryImage(
            SolusMatrix matrix,
            SolusEnvironment env)
        {
            int i;
            int j;
            double z;
            int r;
            int g;
            int b;

            MemoryImage image = new MemoryImage(matrix.ColumnCount,
                matrix.RowCount);
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

        public static void RenderMatrix(Graphics g, RectangleF boundsInClient,
            Matrix matrix)
        {
            MemoryImage image =
                GraphMatrixItem.RenderMatrixToMemoryImage(matrix);

            g.DrawImage(image.Bitmap, Rectangle.Truncate(boundsInClient));
        }

        public static MemoryImage RenderMatrixToMemoryImage(Matrix matrix)
        {
            return RenderMatrixToMemoryImageS(matrix);
        }

        public static Bitmap RenderMatrixToBitmapS(Matrix matrix)
        {
            return RenderMatrixToMemoryImageS(matrix).Bitmap;
        }
        public static Bitmap RenderMatrixToColorBitmapS(Matrix r, Matrix g,
            Matrix b)
        {
            var image = RenderMatrixToMemoryImageColorS(r, g, b);
            return image.Bitmap;
        }

        public static MemoryImage RenderMatrixToMemoryImageColorS(Matrix rr,
            Matrix gg, Matrix bb)
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
                throw new InvalidOperationException(
                    "Input channels must be the same size");
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

            MemoryImage image =
                new MemoryImage(matrix.ColumnCount,matrix.RowCount);
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

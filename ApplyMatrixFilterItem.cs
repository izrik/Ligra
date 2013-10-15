using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Utilities;
using MetaphysicsIndustries.Collections;
using MetaphysicsIndustries.Acuity;
using Environment = MetaphysicsIndustries.Solus.Environment;

namespace MetaphysicsIndustries.Ligra
{
    public class ApplyMatrixFilterItem : RenderItem
    {
        public ApplyMatrixFilterItem(Matrix matrix, MatrixFilter filter, string caption)
        {
            _matrix = matrix.Clone();
            _filter = filter;
            _caption = caption;
        }

        private SizeF _lastSize = new SizeF(0, 0);
        private Matrix _matrix;
        private MatrixFilter _filter;

        string _caption;

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, Environment env)
        {
            Matrix mat = _filter.Apply(_matrix);
            mat.ApplyToAll(AcuityEngine.ConvertFloatTo24g);

            MemoryImage image = control.RenderMatrixToMemoryImage(mat);

            RectangleF boundsInClient = new RectangleF(location.X, location.Y, mat.ColumnCount, mat.RowCount);



            RectangleF rect = boundsInClient;
            rect.Size = new SizeF(GetImageWidth(mat), GetImageHeight(mat));
            g.DrawImage(image.Bitmap, rect);

            SizeF textSize = g.MeasureString(_caption, control.Font, GetImageWidth(mat));
            float textWidth = textSize.Width;
            float textHeight = textSize.Height;
            rect = new RectangleF(location.X, location.Y + GetImageHeight(mat) + 2, textWidth, textHeight);
            g.DrawString(_caption, control.Font, Brushes.Black, rect);

            _lastSize = new SizeF(GetImageWidth(mat), GetImageHeight(mat));

            if (image != null)
            {
                image.Dispose();
                image = null;
            }
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return _lastSize + new SizeF(0, g.MeasureString(_caption, control.Font, (int)_lastSize.Width).Height + 2);
        }

        private int GetImageHeight(Matrix mat)
        {
            if (mat.RowCount < 256)
            {
                return mat.RowCount * 2;
            }
            else
            {
                return mat.RowCount;
            }
        }

        private int GetImageWidth(Matrix mat)
        {
            if (mat.ColumnCount < 256)
            {
                return mat.ColumnCount * 2;
            }
            else
            {
                return mat.ColumnCount;
            }
        }

        protected override void AddVariablesForValueCollection(Set<string> vars)
        {
            //foreach (Expression expr in _matrix)
            //{
            //    GatherVariablesForValueCollection(vars, expr);
            //}
        }

        public override bool HasChanged(Environment env)
        {
            return false;
        }    }
}

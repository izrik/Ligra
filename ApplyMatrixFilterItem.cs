using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

using MetaphysicsIndustries.Acuity;

namespace MetaphysicsIndustries.Ligra
{
    public class ApplyMatrixFilterItem : RenderItem
    {
        public ApplyMatrixFilterItem(Matrix matrix, MatrixFilter filter, string caption, LigraEnvironment env)
            : base(env)
        {
            _matrix = matrix.Clone();
            _filter = filter;
            _caption = caption;
        }

        private SizeF _lastSize = new SizeF(0, 0);
        private Matrix _matrix;
        private MatrixFilter _filter;

        string _caption;

            protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            Matrix mat = _filter.Apply(_matrix);
            mat.ApplyToAll(AcuityEngine.ConvertFloatTo24g);

            MemoryImage image = GraphMatrixItem.RenderMatrixToMemoryImage(mat);

            RectangleF boundsInClient = new RectangleF(0, 0, mat.ColumnCount, mat.RowCount);



            RectangleF rect = boundsInClient;
            rect.Size = new SizeF(GetImageWidth(mat), GetImageHeight(mat));
            g.DrawImage(image.Bitmap, rect);

            SizeF textSize = g.MeasureString(_caption, this.Font, GetImageWidth(mat));
            float textWidth = textSize.Width;
            float textHeight = textSize.Height;
            rect = new RectangleF(0, GetImageHeight(mat) + 2, textWidth, textHeight);
            g.DrawString(_caption, this.Font, Brushes.Black, rect);

            _lastSize = new SizeF(GetImageWidth(mat), GetImageHeight(mat));

            if (image != null)
            {
                image.Dispose();
                image = null;
            }
        }

        protected override SizeF InternalCalcSize(Graphics g)
        {
            return _lastSize + new SizeF(0, g.MeasureString(_caption, this.Font, (int)_lastSize.Width).Height + 2);
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

        protected override void AddVariablesForValueCollection(HashSet<string> vars)
        {
            //foreach (Expression expr in _matrix)
            //{
            //    GatherVariablesForValueCollection(vars, expr);
            //}
        }

        public override bool HasChanged(SolusEnvironment env)
        {
            return false;
        }    }
}

using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Acuity;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class ApplyMatrixFilterItem : RenderItem
    {
        public ApplyMatrixFilterItem(Matrix matrix, MatrixFilter filter,
            string caption)
        {
            _matrix = matrix.Clone();
            _filter = filter;
            _caption = caption;
        }

        public Vector2 _lastSize = new Vector2(0, 0);
        public Matrix _matrix;
        public MatrixFilter _filter;

        public string _caption;

        protected override void InternalRender(IRenderer g,
            DrawSettings drawSettings)
        {
            Matrix mat = _filter.Apply(_matrix);
            mat.ApplyToAll(AcuityEngine.ConvertFloatTo24g);

            MemoryImage image = GraphMatrixItem.RenderMatrixToMemoryImage(mat);

            RectangleF boundsInClient = new RectangleF(0, 0, mat.ColumnCount, mat.RowCount);



            RectangleF rect = boundsInClient;
            rect.Size = new SizeF(GetImageWidth(mat), GetImageHeight(mat));
            g.DrawImage(image, rect);

            SizeF textSize = g.MeasureString(_caption, drawSettings.Font,
                GetImageWidth(mat));
            float textWidth = textSize.Width;
            float textHeight = textSize.Height;
            rect = new RectangleF(0, GetImageHeight(mat) + 2, textWidth, textHeight);
            g.DrawString(_caption, drawSettings.Font, LBrush.Black, rect);

            _lastSize = new Vector2(GetImageWidth(mat), GetImageHeight(mat));

            if (image != null)
            {
                image.Dispose();
                image = null;
            }
        }

        protected override Vector2 InternalCalcSize(IRenderer g,
            DrawSettings drawSettings)
        {
            return _lastSize + new Vector2(0,
                g.MeasureString(_caption, drawSettings.Font,
                    (int)_lastSize.X).Y + 2);
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
        }
    }
}

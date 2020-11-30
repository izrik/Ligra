using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

using MetaphysicsIndustries.Acuity;
using Gtk;

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

        public Vector2 _lastSize = new Vector2(0, 0);
        public Matrix _matrix;
        public MatrixFilter _filter;

        public string _caption;

        protected override Widget GetAdapterInternal()
        {
            throw new NotImplementedException();
        }

        protected override RenderItemControl GetControlInternal()
        {
            return new ApplyMatrixFilterItemControl(this);
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

    public class ApplyMatrixFilterItemControl : RenderItemControl
    {
        public ApplyMatrixFilterItemControl(ApplyMatrixFilterItem owner)
            : base(owner)
        {
        }

        public new ApplyMatrixFilterItem _owner => (ApplyMatrixFilterItem)base._owner;

        private Vector2 _lastSize => _owner._lastSize;
        private Matrix _matrix => _owner._matrix;
        private MatrixFilter _filter => _owner._filter;
        string _caption => _owner._caption;

        protected override void InternalRender(IRenderer g, SolusEnvironment env)
        {
            Matrix mat = _filter.Apply(_matrix);
            mat.ApplyToAll(AcuityEngine.ConvertFloatTo24g);

            MemoryImage image = GraphMatrixItem.RenderMatrixToMemoryImage(mat);

            RectangleF boundsInClient = new RectangleF(0, 0, mat.ColumnCount, mat.RowCount);



            RectangleF rect = boundsInClient;
            rect.Size = new SizeF(GetImageWidth(mat), GetImageHeight(mat));
            g.DrawImage(new SwfImage(image.Bitmap), rect);

            SizeF textSize = g.MeasureString(_caption, LFont.FromSwf(this.Font), GetImageWidth(mat));
            float textWidth = textSize.Width;
            float textHeight = textSize.Height;
            rect = new RectangleF(0, GetImageHeight(mat) + 2, textWidth, textHeight);
            g.DrawString(_caption, LFont.FromSwf(this.Font), LBrush.Black, rect);

            _owner._lastSize = new Vector2(GetImageWidth(mat), GetImageHeight(mat));

            if (image != null)
            {
                image.Dispose();
                image = null;
            }
        }

        protected override Vector2 InternalCalcSize(IRenderer g)
        {
            return _lastSize + new Vector2(0, g.MeasureString(_caption, LFont.FromSwf(this.Font), (int)_lastSize.X).Y + 2);
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
    }
}

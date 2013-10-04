using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus;
using System.Drawing;
using MetaphysicsIndustries.Collections;
using MetaphysicsIndustries.Utilities;
using MetaphysicsIndustries.Acuity;

namespace MetaphysicsIndustries.Ligra
{
    public class GraphMatrixItem : RenderItem
    {
        public GraphMatrixItem(Matrix matrix, string caption)
        {
            _matrix = matrix.Clone();
            _caption = caption;
        }

        private Matrix _matrix;
        public Matrix Matrix
        {
            get { return _matrix; }
        }

        string _caption;

        MemoryImage _image = null;

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, Dictionary<string, Expression> varTable)
        {
            RectangleF boundsInClient = new RectangleF(location.X, location.Y, _matrix.ColumnCount, _matrix.RowCount);

            if (_image == null || HasChanged(varTable))
            {
                MemoryImage image = control.RenderMatrixToMemoryImage(_matrix);

                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }

                _image = image;
            }

            RectangleF rect = boundsInClient;
            rect.Size = new SizeF(GetImageWidth(), GetImageHeight());
            g.DrawImage(_image.Bitmap, rect);

            SizeF textSize = g.MeasureString(_caption, control.Font, GetImageWidth());
            float textWidth = textSize.Width;
            float textHeight = textSize.Height;
            rect = new RectangleF(location.X, location.Y + GetImageHeight() + 2, textWidth, textHeight);
            g.DrawString(_caption, control.Font, Brushes.Black, rect);
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return new SizeF(GetImageWidth(), GetImageHeight() + g.MeasureString(_caption, control.Font, GetImageWidth()).Height + 2);
        }

        private int GetImageHeight()
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

        private int GetImageWidth()
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

        protected override void AddVariablesForValueCollection(Set<string> vars)
        {
            //foreach (Expression expr in _matrix)
            //{
            //    GatherVariablesForValueCollection(vars, expr);
            //}
        }

        public override bool HasChanged(Dictionary<string, Expression> varTable)
        {
            return false;
        }
    }
}

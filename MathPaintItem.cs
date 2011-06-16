using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus;
using System.Drawing;
using MetaphysicsIndustries.Collections;
using MetaphysicsIndustries.Utilities;

namespace MetaphysicsIndustries.Ligra
{
    public class MathPaintItem : RenderItem
    {
        public MathPaintItem(Expression expression, 
            Variable horizontalCoordinate, Variable verticalCoordinate,
            int width, int height)
        {
            _expression = expression;
            _horizontalCoordinate = horizontalCoordinate;
            _verticalCoordinate = verticalCoordinate;
            _width = width;
            _height = height;
        }

        private Expression _expression;
        private Variable _horizontalCoordinate;
        private Variable _verticalCoordinate;
        private int _width;
        private int _height;
        MemoryImage _image;

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, VariableTable varTable)
        {
            RectangleF boundsInClient = new RectangleF(location.X, location.Y, _width, _height);

            if (_image == null || HasChanged(varTable))
            {
                MemoryImage image =
                    control.RenderMathPaintToMemoryImage(
                            _expression,
                            _horizontalCoordinate,
                            _verticalCoordinate,
                            0, _width,
                            0, _height,
                            varTable);

                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }

                _image = image;
            }

            g.DrawImage(_image.Bitmap, boundsInClient);
        }

        protected override void RemoveVariablesForValueCollection(Set<Variable> vars)
        {
            UngatherVariableForValueCollection(vars, _horizontalCoordinate);
            UngatherVariableForValueCollection(vars, _verticalCoordinate);
        }

        protected override void AddVariablesForValueCollection(Set<Variable> vars)
        {
            GatherVariablesForValueCollection(vars, _expression);
        }


        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return new SizeF(_width, _height);
        }

    }
}
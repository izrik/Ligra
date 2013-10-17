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
                             string horizontalCoordinate,
                             string verticalCoordinate,
                            int width,
                             int height)
        {
            _expression = expression;
            _horizontalCoordinate = horizontalCoordinate;
            _verticalCoordinate = verticalCoordinate;
            _width = width;
            _height = height;
        }

        private Expression _expression;
        private string _horizontalCoordinate;
        private string _verticalCoordinate;
        private int _width;
        private int _height;
        MemoryImage _image;

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, SolusEnvironment env)
        {
            RectangleF boundsInClient = new RectangleF(location.X, location.Y, _width, _height);

            if (_image == null || HasChanged(env))
            {
                MemoryImage image =
                    control.RenderMathPaintToMemoryImage(
                            _expression,
                            _horizontalCoordinate,
                            _verticalCoordinate,
                            0, _width,
                            0, _height,
                            env);

                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }

                _image = image;
            }

            g.DrawImage(_image.Bitmap, boundsInClient);
        }

        protected override void RemoveVariablesForValueCollection(Set<string> vars)
        {
            UngatherVariableForValueCollection(vars, _horizontalCoordinate);
            UngatherVariableForValueCollection(vars, _verticalCoordinate);
        }

        protected override void AddVariablesForValueCollection(Set<string> vars)
        {
            GatherVariablesForValueCollection(vars, _expression);
        }


        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return new SizeF(_width, _height);
        }

    }
}

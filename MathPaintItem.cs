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
            _hStart = 0;
            _width = width;
            _vStart = 0;
            _height = height;
        }
        public MathPaintItem(Expression expression,
                             VarInterval horizontalCoordinate,
                             VarInterval verticalCoordinate)
        {
            _expression = expression;
            _horizontalCoordinate = horizontalCoordinate.Variable;
            _verticalCoordinate = verticalCoordinate.Variable;
            var horiz = horizontalCoordinate.Interval.Round();
            _hStart = (int)horiz.LowerBound;
            _width = (int)horiz.Length;
            var vert = verticalCoordinate.Interval.Round();
            _vStart = (int)vert.LowerBound;
            _height = (int)vert.Length;
        }

        private Expression _expression;
        private string _horizontalCoordinate;
        private string _verticalCoordinate;
        int _hStart;
        int _width;
        int _vStart;
        int _height;
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
                            _hStart,
                            _width,
                            _vStart,
                            _height,
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

using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus;
using System.Drawing;

using MetaphysicsIndustries.Utilities;

namespace MetaphysicsIndustries.Ligra
{
    public class MathPaintItem : RenderItem
    {
        public MathPaintItem(Expression expression, 
                             string horizontalCoordinate,
                             string verticalCoordinate,
                             int width,
                             int height,
                             LigraEnvironment env)
            : base(env)
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
                             VarInterval verticalCoordinate,
                             LigraEnvironment env)
            : base(env)
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

        protected override void InternalRender(LigraControl control, Graphics g, SolusEnvironment env)
        {
            RectangleF boundsInClient = new RectangleF(0, 0, _width, _height);

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

        protected override void RemoveVariablesForValueCollection(HashSet<string> vars)
        {
            UngatherVariableForValueCollection(vars, _horizontalCoordinate);
            UngatherVariableForValueCollection(vars, _verticalCoordinate);
        }

        protected override void AddVariablesForValueCollection(HashSet<string> vars)
        {
            GatherVariablesForValueCollection(vars, _expression);
        }


        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return new SizeF(_width, _height);
        }

    }
}

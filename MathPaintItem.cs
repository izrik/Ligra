using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus;
using System.Drawing;

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

            protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            RectangleF boundsInClient = new RectangleF(0, 0, _width, _height);

            if (_image == null || HasChanged(env))
            {
                MemoryImage image =
                    RenderMathPaintToMemoryImage(
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


        protected override SizeF InternalCalcSize(Graphics g)
        {
            return new SizeF(_width, _height);
        }

        static readonly SolusEngine _engine = new SolusEngine();

        public static MemoryImage RenderMathPaintToMemoryImage(
            Expression expression, 
            string independentVariableX, 
            string independentVariableY, 
            int xStart, int xEnd,
            int yStart, int yEnd,
            SolusEnvironment env)
        {
            int xValues = xEnd - xStart + 1;
            int yValues = yEnd - yStart + 1;

            double[,] values = new double[xValues, yValues];

            Expression prelimEval1;
            Expression prelimEval2;

            if (env.Variables.ContainsKey(independentVariableX))
            {
                env.Variables.Remove(independentVariableX);
            }
            if (env.Variables.ContainsKey(independentVariableY))
            {
                env.Variables.Remove(independentVariableY);
            }

            prelimEval1 = _engine.PreliminaryEval(expression, env);

            int i;
            int j;
            double z;

            MemoryImage image = new MemoryImage(xValues, yValues);
            //image.Size = new Size(xValues, yValues);

            for (i = 0; i < xValues; i++)
            {
                env.Variables[independentVariableX] = new Literal(i);
                if (env.Variables.ContainsKey(independentVariableY))
                {
                    env.Variables.Remove(independentVariableY);
                }

                prelimEval2 = _engine.PreliminaryEval(prelimEval1, env);

                for (j = 0; j < yValues; j++)
                {
                    env.Variables[independentVariableY] = new Literal(j);

                    z = prelimEval2.Eval(env).Value;

                    if (double.IsNaN(z))
                    {
                        z = 0;
                    }

                    image[j, i] = Color.FromArgb(255, Color.FromArgb((int)z));
                    //values[i, j] = z;
                }
            }

            image.CopyPixelsToBitmap();
            return image;
        }
    }
}

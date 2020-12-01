using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus;
using System.Drawing;
using Gtk;

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

        public Expression _expression;
        public string _horizontalCoordinate;
        public string _verticalCoordinate;
        public int _hStart;
        public int _width;
        public int _vStart;
        public int _height;
        public MemoryImage _image;

        protected override void RemoveVariablesForValueCollection(HashSet<string> vars)
        {
            UngatherVariableForValueCollection(vars, _horizontalCoordinate);
            UngatherVariableForValueCollection(vars, _verticalCoordinate);
        }

        protected override void AddVariablesForValueCollection(HashSet<string> vars)
        {
            GatherVariablesForValueCollection(vars, _expression);
        }

        static readonly SolusEngine _engine = new SolusEngine();

        public MemoryImage RenderMathPaintToMemoryImage(SolusEnvironment env)
        {
            var image = RenderMathPaintToMemoryImage(
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
            return image;
        }
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

        protected override Widget GetAdapterInternal()
        {
            throw new NotImplementedException();
        }

        protected override RenderItemControl GetControlInternal()
        {
            return new MathPaintItemControl(this);
        }
    }

    public class MathPaintItemControl : RenderItemControl
    {
        public MathPaintItemControl(MathPaintItem owner)
            : base(owner)
        {
        }

        public new MathPaintItem _owner => (MathPaintItem)base._owner;

        public Expression _expression => _owner._expression;
        public string _horizontalCoordinate => _owner._horizontalCoordinate;
        public string _verticalCoordinate => _owner._verticalCoordinate;
        public int _hStart => _owner._hStart;
        public int _width => _owner._width;
        public int _vStart => _owner._vStart;
        public int _height => _owner._height;
        public MemoryImage _image => _owner._image;

        bool HasChanged(SolusEnvironment env) => _owner.HasChanged(env);
        MemoryImage RenderMathPaintToMemoryImage(SolusEnvironment env) =>
            _owner.RenderMathPaintToMemoryImage(env);

        public override void InternalRender(IRenderer g, SolusEnvironment env)
        {
            RectangleF boundsInClient = new RectangleF(0, 0, _width, _height);

            if (_image == null || HasChanged(env))
            {
                RenderMathPaintToMemoryImage(env);
            }

            g.DrawImage(_image, boundsInClient);
        }

        public override Vector2 InternalCalcSize(IRenderer g)
        {
            return new Vector2(_width, _height);
        }
    }
}

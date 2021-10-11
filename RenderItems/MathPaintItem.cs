using System;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class MathPaintItem : RenderItem
    {
        public MathPaintItem(Expression expression, 
                             string horizontalCoordinate,
                             string verticalCoordinate,
                             int width,
                             int height,
                             LigraEnvironment env)
            : this(env)
        {
            // TODO: push the following dow to the protected constructor
            _expression = expression;
            _horizontalCoordinate = horizontalCoordinate;
            _verticalCoordinate = verticalCoordinate;
            _hStart = 0;
            _width = width;
            _vStart = 0;
            _height = height;
            _image = RenderMathPaintToMemoryImage(
                _expression,
                _horizontalCoordinate,
                _verticalCoordinate,
                _hStart,
                _width,
                _vStart,
                _height,
                env);
            _data = new GtkRenderer.GtkDrawImageData(_image);
        }
        public MathPaintItem(Expression expression,
                             VarInterval horizontalCoordinate,
                             VarInterval verticalCoordinate,
                             LigraEnvironment env)
            : this(env)
        {
            // TODO: push the following dow to the protected constructor
            _expression = expression;
            _horizontalCoordinate = horizontalCoordinate.Variable;
            _verticalCoordinate = verticalCoordinate.Variable;
            var horiz = horizontalCoordinate.Interval.Round();
            _hStart = (int)horiz.LowerBound;
            _width = (int)horiz.Length;
            var vert = verticalCoordinate.Interval.Round();
            _vStart = (int)vert.LowerBound;
            _height = (int)vert.Length;
            _image = RenderMathPaintToMemoryImage(
                _expression,
                _horizontalCoordinate,
                _verticalCoordinate,
                _hStart,
                _width,
                _vStart,
                _height,
                env);
            _data = new GtkRenderer.GtkDrawImageData(_image);
        }
        protected MathPaintItem(LigraEnvironment env)
            : base(env)
        {
            _timer = new System.Timers.Timer(250);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;
        }

        public Expression _expression;
        public string _horizontalCoordinate;
        public string _verticalCoordinate;
        public int _hStart;
        public int _width;
        public int _vStart;
        public int _height;
        public MemoryImage _image;
        private GtkRenderer.GtkDrawImageData _data;

        System.Timers.Timer _timer;

        protected override void InternalRender(IRenderer g,
            SolusEnvironment env, DrawSettings drawSettings)
        {
            RectangleF boundsInClient = new RectangleF(0, 0, _width, _height);

            if (_image == null || HasChanged(env))
            {
                RenderMathPaintToMemoryImage(env);
            }

            ((GtkRenderer) g).DrawImage(_data, boundsInClient);
        }

        public void RenderMathPaintToMemoryImage(SolusEnvironment env)
        {
            RenderMathPaintToMemoryImage(
                _image,
                _expression,
                _horizontalCoordinate,
                _verticalCoordinate,
                _hStart,
                _width,
                _vStart,
                _height,
                env);
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

        protected override Vector2 InternalCalcSize(IRenderer g,
            DrawSettings drawSettings)
        {
            return new Vector2(_width, _height);
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
            var image = new MemoryImage(xValues, yValues);
            RenderMathPaintToMemoryImage(
                image,
                expression,
                independentVariableX,
                independentVariableY,
                xStart, xEnd,
                yStart, yEnd,
                env);
            return image;
        }

        public static void RenderMathPaintToMemoryImage(
            MemoryImage image,
            Expression expression,
            string independentVariableX,
            string independentVariableY,
            int xStart, int xEnd,
            int yStart, int yEnd,
            SolusEnvironment env)
        {
            int xValues = xEnd - xStart + 1;
            int yValues = yEnd - yStart + 1;

            if (image.Width < xValues || image.Height < yValues)
                throw new InvalidOperationException(
                    "MemoryImage not large enough.");

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

                    z = prelimEval2.Eval(env).ToNumber().Value;

                    if (double.IsNaN(z))
                    {
                        z = 0;
                    }

                    image[j, i] = Color.FromArgb(255, Color.FromArgb((int)z));
                    //values[i, j] = z;
                }
            }
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invalidate();
        }
    }
}

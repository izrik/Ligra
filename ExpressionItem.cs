using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Solus;
using System.Drawing;
using MetaphysicsIndustries.Acuity;

namespace MetaphysicsIndustries.Ligra
{
    public class ExpressionItem : RenderItem
    {
        public ExpressionItem(Expression expression, Pen pen, Font font, LigraEnvironment env)
            : base(env)
        {
            _expression = expression;
            _pen = pen;
            _font = font;
        }
        public ExpressionItem(Vector vector, Pen pen, Font font, LigraEnvironment env)
            : this(GenerateVector(vector), pen, font, env)
        {
        }
        public ExpressionItem(Matrix matrix, Pen pen, Font font, LigraEnvironment env)
            : this(GenerateMatrix(matrix), pen, font, env)
        {
        }

        private static Expression GenerateVector(Vector vector)
        {
            SolusVector v = new SolusVector(vector.Length);
            int i;
            for (i = 0; i < vector.Length; i++)
            {
                v[i] = new Literal(vector[i]);
            }
            return v;
        }

        private static Expression GenerateMatrix(Matrix matrix)
        {
            SolusMatrix m = new SolusMatrix(matrix.RowCount, matrix.ColumnCount);
            int i;
            int j;
            for (i = 0; i < matrix.RowCount; i++)
            {
                for (j = 0; j < matrix.ColumnCount; j++)
                {
                    m[i, j] = new Literal(matrix[i, j]);
                }
            }
            return m;
        }

        private Expression _expression;
        public Expression Expression
        {
            get { return _expression; }
        }

        private Pen _pen;
        public Pen Pen
        {
            get { return _pen; }
        }

        //private string _name;
        //public string Name
        //{
        //    get { return _name; }
        //}

        private Font _font;
        public Font Font
        {
            get { return _font; }
        }


            protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            SizeF exprSize = LigraControl.CalcExpressionSize(Expression, g, Font);
            float xx = 0;

            //if (!string.IsNullOrEmpty(Name))
            //{
            //    SizeF textSize = g.MeasureString(Name + " = ", Font);
            //    xx += textSize.Width + 10;
            //    g.DrawString(Name + " = ", Font, Pen.Brush, new PointF(location.X, location.Y + (exprSize.Height - textSize.Height) / 2));
            //}

            LigraControl.RenderExpression(g, Expression, new PointF(xx, 0), Pen, Pen.Brush, Font, false);
        }

        protected override SizeF InternalCalcSize(Graphics g)
        {
            SizeF exprSize = LigraControl.CalcExpressionSize(Expression, g, Font);

            //if (!string.IsNullOrEmpty(Name))
            //{
            //    SizeF textSize = g.MeasureString(Name + " = ", Font);
            //    exprSize.Width += textSize.Width + 10;
            //    exprSize.Height = Math.Max(exprSize.Height, textSize.Height);
            //}

            return exprSize;
        }
    }
}

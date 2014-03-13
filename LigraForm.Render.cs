using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;
//using MetaphysicsIndustries.Sandbox;
using System.Drawing.Printing;


namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraForm : Form
    {
        private void ligraControl1_Paint(object sender, PaintEventArgs e)
        {
            Render(e.Graphics);
        }

        private void Render(Graphics g)
        {
            //VariableTable env = new VariableTable();

            float time = System.Environment.TickCount / 1000.0f;

//            PointF scroll = ligraControl1.AutoScrollPosition;
//            g.TranslateTransform(scroll.X, scroll.Y);

            float x = 10;// +ligraControl1.AutoScrollPosition.X;
            float y = 10;// +ligraControl1.AutoScrollPosition.Y;
            float maxWidth = 0;

            //g.DrawString("min size = " + ligraControl1.AutoScrollMinSize.ToString(), Font, Brushes.Black, x + 5, y + 5);
            //g.DrawString("offset =   " + ligraControl1.AutoScrollOffset.ToString(), Font, Brushes.Black, x + 5, y + 20);
            //g.DrawString("position = " + ligraControl1.AutoScrollPosition.ToString(), Font, Brushes.Black, x + 5, y + 35);
            //g.DrawString("margin =   " + ligraControl1.AutoScrollMargin.ToString(), Font, Brushes.Black, x + 5, y + 50);

            //y += 70;

            //bool stopTimer = (_renderItems.Count <= 0);

            foreach (RenderItem item in _env.RenderItems)
            {
                SizeF itemSize = item.CalcSize(g);

                //if (y + scroll.Y + itemSize.Height > 0 &&
                //    y + scroll.Y < ligraControl1.ClientSize.Height)
                //{
                    _env.Variables["t"] = new Literal(time);
                    item.Refresh();
                //}

                item.Location = Point.Truncate(new PointF(x, y));

                maxWidth = Math.Max(maxWidth, itemSize.Width);
                y += itemSize.Height + 25;
            }

            //foreach (Formula formula in _formulas)
            //{
            //    float xx = x;

            //    SizeF exprSize = ligraControl1.CalcExpressionSize(g, formula.Expr);

            //    if (y + ligraControl1.AutoScrollPosition.Y <= ligraControl1.ClientSize.Height &&
            //        y + ligraControl1.AutoScrollPosition.Y + exprSize.Height >= 0)
            //    {

            //        if (!string.IsNullOrEmpty(formula.Name))
            //        {
            //            SizeF textSize = g.MeasureString(formula.Name + " = ", Font);
            //            xx += textSize.Width + 10;
            //            g.DrawString(formula.Name + " = ", Font, Brushes.Blue, new PointF(x, y + (exprSize.Height - textSize.Height) / 2));
            //        }

            //        ligraControl1.RenderExpression(g, formula.Expr, new PointF(xx, y), Pens.Blue, Brushes.Blue);
            //    }

            //    y += exprSize.Height + 25;
            //    maxWidth = Math.Max(exprSize.Width + xx, maxWidth);
            //}

            //if (_graphs.Count > 0)
            //{
            //    if (y + ligraControl1.AutoScrollPosition.Y <= ligraControl1.ClientSize.Height &&
            //        y + ligraControl1.AutoScrollPosition.Y + 400 >= 0)
            //    {
            //        bool first = true;
            //        foreach (Graph graph in _graphs)
            //        {
            //            ligraControl1.RenderGraph(g,
            //                new RectangleF(x, y, 400, 400),
            //                //new RectangleF(600, 10, 400, 400),
            //                //new RectangleF(75, 50, 500, 750),
            //                new Pen(graph.Color), new SolidBrush(graph.Color),
            //                -2, 2, -2, 2,
            //                //0, 1, 0, 1.5f, 
            //                graph.Formula.Expr, _x, env, first);
            //            first = false;

            //            //float value = graph.Formula.Expr.Eval(env).Value;
            //            //float yy = 750 - value * 500;

            //            //g.DrawString(value.ToString(), Font, new SolidBrush(graph.Color), 575 + 10, yy+50);

            //        }
            //    }

            //    y += 410;
            //}

            //string phi = new string((char)(0x03A6), 1);
            //g.DrawString(phi + "1", Font, Pens.Green.Brush, 500, 165);
            //g.DrawString(phi + "2", Font, Pens.Red.Brush, 545, 295);
            //g.DrawString(phi + "3", Font, Pens.Blue.Brush, 545, 185);
            //g.DrawString("Plots for problem 1 part (b)", Font, Pens.Black.Brush, 10, 10);

            //if (_graphs3d.Count > 0)
            //{
            //    if (y + ligraControl1.AutoScrollPosition.Y <= ligraControl1.ClientSize.Height &&
            //        y + ligraControl1.AutoScrollPosition.Y + 400 >= 0)
            //    {
            //        bool first = true;
            //        foreach (Graph graph in _graphs3d)
            //        {
            //            Graph3dItem item = new Graph3dItem(graph.Formula.Expr, new Pen(graph.Color), new SolidBrush(graph.Color), _x, _y);
            //            item.Render(ligraControl1, g, new PointF(x, y), env);
            //            //ligraControl1.Render3DGraph(g,
            //            //    new RectangleF(x, y, 400, 400),
            //            //    //new RectangleF(600, 420, 400, 400),
            //            //    new Pen(graph.Color), new SolidBrush(graph.Color),
            //            //    -4, 0, -4, 0, -2, 6,
            //            //    graph.Formula.Expr, _x, _y, env, first);
            //            //first = false;
            //        }
            //    }

            //    y += 410;
            //}

//            Size s = (new SizeF(maxWidth, y)).ToSize();

//            if (ligraControl1.AutoScrollMinSize != s)
//            {
//                ligraControl1.AutoScrollMinSize = s;
//                Point pt = ligraControl1.AutoScrollPosition;
//                pt.Y = s.Height - ligraControl1.ClientSize.Height;
//                ligraControl1.AutoScrollPosition = pt;
//            }
        }

    }
}
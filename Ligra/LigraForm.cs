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
        public LigraForm()
        {
            InitializeComponent();
        }

        struct Formula
        {
            public Formula(string name, Expression expr)
            {
                _name = name;
                _expr = expr;
            }

            private string _name;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            private Expression _expr;

            public Expression Expr
            {
                get { return _expr; }
                set { _expr = value; }
            }

        }

        struct Graph
        {
            public Graph(Formula formula, Color color)
            {
                _formula = formula;
                _color = color;
            }

            private Formula _formula;

            public Formula Formula
            {
                get { return _formula; }
                set { _formula = value; }
            }


            private Color _color;

            public Color Color
            {
                get { return _color; }
                set { _color = value; }
            }

        }

        List<Formula> _formulas = new List<Formula>();
        List<Graph> _graphs = new List<Graph>();
        List<Graph> _graphs3d = new List<Graph>();
        Variable _x;
        Variable _t = new Variable("t");
        Variable _y;

        //Expression _expression;
        //Expression _expression2;
        //Expression _expression3;
        //Expression _expression4;
        //Expression _expression5;
        //Expression _expression6;
        //Expression _expression7;
        //Expression _expression8;

        class TempFunction : Function
        {
            public TempFunction(string name)
                : base(name)
            {
            }

            protected override Literal InternalCall(VariableTable varTable, Literal[] args)
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        private void LigraForm_Load(object sender, EventArgs e)
        {
            Expression expr;
            Expression expr2 = null;
            Formula formula = new Formula();
            Graph graph = new Graph();

            Variable x = new Variable("x");
            Variable y = new Variable("y");
            Variable t = _t;

            VariableTable varTable = new VariableTable();

            _x = x;
            _y = y;

            SolusEngine engine = new SolusEngine();

            Function f = new TempFunction("f");
            //f.Name = "f";

            expr2 = new FunctionCall(AssociativeCommutativOperation.Multiplication,
                    new FunctionCall(AssociativeCommutativOperation.Addition,
                        new Literal(7),
                        new Literal(8)),
                    new FunctionCall(AssociativeCommutativOperation.Addition,
                        new Literal(9),
                        new VariableAccess(x)));

            expr = new FunctionCall(f,
                new Literal(1),
                new Literal(2),
                new Literal(3),
                new FunctionCall(BinaryOperation.Division,
                    new Literal(4),
                    new Literal(5)),
                new Literal(6),
                expr2.Clone());

            _formulas.Add(new Formula("a", expr));
            _formulas.Add(new Formula("b", expr2));

            expr = new FunctionCall(AssociativeCommutativOperation.Addition,
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new Literal(7),
                        new Literal(8)),
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new Literal(9),
                        new VariableAccess(new Variable("x"))));
            _formulas.Add(new Formula("c", expr));

            expr =
                new FunctionCall(Function.Cosine, new VariableAccess(x));
            _formulas.Add(new Formula("f", expr));

            expr = engine.GetDerivative(expr);
            _formulas.Add(new Formula("df", expr));

            expr = engine.GetDerivative(expr);
            _formulas.Add(new Formula("d2f", expr));

            expr = engine.GetDerivative(expr);
            _formulas.Add(new Formula("d3f", expr));

            expr = engine.GetDerivative(expr);
            _formulas.Add(new Formula("d4f", expr));

            expr =
                new FunctionCall(BinaryOperation.Division,
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new Literal(2),
                        new FunctionCall(Function.Cosine,
                            new VariableAccess(x))),
                    new VariableAccess(y));
            _formulas.Add(new Formula("g", expr));

            expr = engine.GetDerivative(expr);
            _formulas.Add(new Formula("dg", expr));

            expr =
                new FunctionCall(AssociativeCommutativOperation.Multiplication,
                    new Literal(2),
                    new FunctionCall(Function.Cosine,
                        new VariableAccess(x)));
            _formulas.Add(new Formula("h", expr));

            expr = engine.GetDerivative(expr);
            _formulas.Add(new Formula("dh", expr));

            Variable s = new Variable("s");
            Variable k = new Variable("K");

            expr = new FunctionCall(BinaryOperation.Division,
                        new FunctionCall(AssociativeCommutativOperation.Multiplication,
                            new FunctionCall(AssociativeCommutativOperation.Multiplication,
                                new VariableAccess(k),
                                new FunctionCall(AssociativeCommutativOperation.Addition,
                                    new VariableAccess(s),
                                    new Literal(1))),
                            new FunctionCall(BinaryOperation.Exponent,
                                new FunctionCall(AssociativeCommutativOperation.Addition,
                                    new VariableAccess(s),
                                    new Literal(10)),
                                new Literal(2))),
                        new FunctionCall(AssociativeCommutativOperation.Multiplication,
                            new FunctionCall(AssociativeCommutativOperation.Multiplication,
                                new FunctionCall(BinaryOperation.Exponent,
                                    new VariableAccess(s),
                                    new Literal(3)),
                                new FunctionCall(AssociativeCommutativOperation.Addition,
                                    new VariableAccess(s),
                                    new Literal(4))),
                            new FunctionCall(AssociativeCommutativOperation.Multiplication,
                                new FunctionCall(AssociativeCommutativOperation.Addition,
                                    new VariableAccess(s),
                                    new Literal(5)),
                                new FunctionCall(AssociativeCommutativOperation.Addition,
                                    new VariableAccess(s),
                                    new Literal(6)))));
            _formulas.Add(new Formula("T(s)", expr));

            expr =
                new FunctionCall(Function.Sine,
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new Literal(2),
                        new FunctionCall(AssociativeCommutativOperation.Multiplication,
                            new Literal(2),
                            new FunctionCall(AssociativeCommutativOperation.Multiplication,
                                new VariableAccess(x),
                                new VariableAccess(y)))));
            _formulas.Add(new Formula("z", expr));

            _formulas.Add(new Formula("dz", engine.GetDerivative(expr)));

            expr =
                new FunctionCall(BinaryOperation.Exponent,
                    new VariableAccess(x),
                    new VariableAccess(y));
            _formulas.Add(new Formula("u", expr));
            _formulas.Add(new Formula("du", engine.GetDerivative(expr)));

            Variable Rx = new Variable("Rx");
            Variable Ry = new Variable("Ry");

            expr =
                new FunctionCall(BinaryOperation.Division,
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new VariableAccess(Rx),
                        new VariableAccess(Ry)),
                    new FunctionCall(AssociativeCommutativOperation.Addition,
                        new VariableAccess(Rx),
                        new VariableAccess(Ry)));
            _formulas.Add(new Formula("Req", expr));
            _formulas.Add(new Formula("dReq", engine.GetDerivative(expr)));








            //expr2 = engine.GetDerivative(expr);
            //_expressions.Add(expr2);

            expr =
                new FunctionCall(AssociativeCommutativOperation.Addition,
                    new FunctionCall(BinaryOperation.Exponent,
                        new VariableAccess(x),
                        new Literal(3)),
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new Literal(3),
                        new FunctionCall(BinaryOperation.Exponent,
                            new VariableAccess(x),
                            new Literal(2))),
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new Literal(3),
                        new VariableAccess(x)),
                    new Literal(1));

            _formulas.Add(new Formula("q", expr));
            expr = engine.GetDerivative(expr);
            _formulas.Add(new Formula("dq", expr));
            expr = engine.GetDerivative(expr);
            _formulas.Add(new Formula("d2q", expr));
            expr = engine.GetDerivative(expr);
            _formulas.Add(new Formula("d3q", expr));

            expr =
                new FunctionCall(BinaryOperation.Exponent,
                    new FunctionCall(AssociativeCommutativOperation.Addition,
                        new Literal(2),
                        new Literal(3)),
                    new Literal(4));
            _formulas.Add(new Formula("", expr));
            expr =
                new FunctionCall(BinaryOperation.Exponent,
                    new Literal(4),
                    new FunctionCall(AssociativeCommutativOperation.Addition,
                        new Literal(2),
                        new Literal(3)));
            _formulas.Add(new Formula("", expr));


            expr =
                new FunctionCall(BinaryOperation.Exponent,
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new Literal(2),
                        new Literal(3)),
                    new Literal(4));
            _formulas.Add(new Formula("", expr));
            expr =
                new FunctionCall(BinaryOperation.Exponent,
                    new Literal(4),
                    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                        new Literal(2),
                        new Literal(3)));
            _formulas.Add(new Formula("", expr));            expr =
                new FunctionCall(BinaryOperation.Exponent,
                    new FunctionCall(BinaryOperation.Division,
                        new Literal(2),
                        new Literal(3)),
                    new Literal(4));
            _formulas.Add(new Formula("", expr));
            expr =
                new FunctionCall(BinaryOperation.Exponent,
                    new Literal(4),
                    new FunctionCall(BinaryOperation.Division,
                        new Literal(2),
                        new Literal(3)));
            _formulas.Add(new Formula("", expr));

            expr =
                new FunctionCall(BinaryOperation.Exponent,
                    new FunctionCall(BinaryOperation.Exponent,
                        new Literal(2),
                        new Literal(3)),
                    new Literal(4));
            _formulas.Add(new Formula("", expr));
            expr =
                new FunctionCall(BinaryOperation.Exponent,
                    new Literal(4),
                    new FunctionCall(BinaryOperation.Exponent,
                        new Literal(2),
                        new Literal(3)));
            _formulas.Add(new Formula("", expr));

            expr =
                //new FunctionCall(AssociativeCommutativOperation.Addition,
                //    new FunctionCall(BinaryOperation.Exponent,
                //        new FunctionCall(AssociativeCommutativOperation.Addition,
                //            new VariableAccess(x),
                //            new Literal(-3)),
                //        new Literal(3)),
                //    new FunctionCall(AssociativeCommutativOperation.Multiplication,
                //        new Literal(-1),
                //        new VariableAccess(x)));

                new FunctionCall(BinaryOperation.Exponent,
                    new VariableAccess(x),
                    new Literal(3));

                //new FunctionCall(BinaryOperation.Exponent,
                //    new Literal((float)Math.E),
                //    new VariableAccess(x));

            formula = new Formula("y", expr);
            _formulas.Add(formula);
            graph = new Graph(formula, Color.Blue);
            _graphs.Add(graph);

            expr =
                new FunctionCall(AssociativeCommutativOperation.Multiplication,
                    new Literal(3),
                    new FunctionCall(BinaryOperation.Exponent,
                        new VariableAccess(x),
                        new Literal(2)));
            formula = new Formula("y'", expr);
            _formulas.Add(formula);
            graph = new Graph(formula, Color.Green);
            _graphs.Add(graph);

            expr =
                new FunctionCall(AssociativeCommutativOperation.Multiplication,
                    new Literal(6),
                    new VariableAccess(x));
            formula = new Formula("y''", expr);
            _formulas.Add(formula);
            graph = new Graph(formula, Color.Red);
            _graphs.Add(graph);

            expr =
                new FunctionCall(Function.Cosine,
                    new FunctionCall(AssociativeCommutativOperation.Addition,
                        new VariableAccess(x),
                        new VariableAccess(t)));
            formula = new Formula("cos(x + t)", expr);
            _formulas.Add(formula);
            graph = new Graph(formula, Color.Cyan);
            _graphs.Add(graph);

            expr =
                new FunctionCall(Function.Sine,
                    new FunctionCall(AssociativeCommutativOperation.Addition,
                        new VariableAccess(x),
                        new FunctionCall(AssociativeCommutativOperation.Multiplication,
                            new Literal(2),
                            new VariableAccess(t))));
            formula = new Formula("sin(x + 2t)", expr);
            _formulas.Add(formula);
            graph = new Graph(formula, Color.Magenta);
            _graphs.Add(graph);

            expr =
                new FunctionCall(Function.Tangent,
                    new FunctionCall(AssociativeCommutativOperation.Addition,
                        new VariableAccess(x),
                        new FunctionCall(AssociativeCommutativOperation.Multiplication,
                            new Literal(0.5f),
                            new VariableAccess(t))));
            formula = new Formula("tan(x + t/2)", expr);
            _formulas.Add(formula);
            graph = new Graph(formula, Color.Yellow);
            _graphs.Add(graph);


            expr =
                new FunctionCall(AssociativeCommutativOperation.Multiplication,
                    new FunctionCall(Function.UnitStep,
                        new FunctionCall(AssociativeCommutativOperation.Addition,
                            new FunctionCall(BinaryOperation.Exponent,
                                new FunctionCall(AssociativeCommutativOperation.Addition,
                                    new FunctionCall(BinaryOperation.Exponent,
                                        new VariableAccess(x),
                                        new Literal(2)),
                                    new FunctionCall(BinaryOperation.Exponent,
                                        new VariableAccess(y),
                                        new Literal(2))),
                                new Literal(0.5f)),
                            new Literal(-1))),
                    new FunctionCall(Function.Cosine,
                        new FunctionCall(AssociativeCommutativOperation.Multiplication,
                            new Literal(5),
                            new FunctionCall(AssociativeCommutativOperation.Addition,
                                new VariableAccess(y),
                                new FunctionCall(AssociativeCommutativOperation.Multiplication,
                                    new Literal(-1),
                                    new VariableAccess(t))))));
            formula = new Formula("z(x,y)", expr);
            _formulas.Add(formula);
            graph = new Graph(formula, Color.Green);
            _graphs3d.Add(graph);

            //(new ObjectMapForm(expr)).ShowDialog(this);

            varTable.Clear();
            varTable[x] = 0;
            expr2 = engine.PreliminaryEval(expr, varTable);
            //(new ObjectMapForm(expr2)).ShowDialog(this);
            formula = new Formula("z(0,y)", expr2);
            _formulas.Add(formula);

            varTable[y] = 0;
            expr2 = engine.PreliminaryEval(expr2, varTable);
            //(new ObjectMapForm(expr2)).ShowDialog(this);
            formula = new Formula("z(0,0)", expr2);
            _formulas.Add(formula);

            varTable[y] = 1;
            expr2 = engine.PreliminaryEval(expr2, varTable);
            //(new ObjectMapForm(expr2)).ShowDialog(this);
            formula = new Formula("z(0,1)", expr2);
            _formulas.Add(formula);

            varTable.Clear();
            varTable[x] = 1;
            expr2 = engine.PreliminaryEval(expr, varTable);
            //(new ObjectMapForm(expr2)).ShowDialog(this);
            formula = new Formula("z(1,y)", expr2);
            _formulas.Add(formula);

            varTable[y] = 0;
            expr2 = engine.PreliminaryEval(expr2, varTable);
            //(new ObjectMapForm(expr2)).ShowDialog(this);
            formula = new Formula("z(1,0)", expr2);
            _formulas.Add(formula);

            varTable[y] = 1;
            expr2 = engine.PreliminaryEval(expr2, varTable);
            //(new ObjectMapForm(expr2)).ShowDialog(this);
            formula = new Formula("z(1,1)", expr2);
            _formulas.Add(formula);

            expr =
                new FunctionCall(AssociativeCommutativOperation.Addition,
                    new VariableAccess(x),
                    new VariableAccess(y),
                    new Literal(1));
            formula = new Formula("f(x,y)", expr);
            _formulas.Add(formula);

            varTable.Clear();
            varTable[x] = 5;
            expr = engine.PreliminaryEval(expr, varTable);
            formula = new Formula("f(5,y)", expr);
            _formulas.Add(formula);

            varTable[y] = 3;
            expr = engine.PreliminaryEval(expr, varTable);
            formula = new Formula("f(5,3)", expr);
            _formulas.Add(formula);

            //_graphs.Clear();
            //_graphs3d.Clear();

            //expr2 =
            //    new FunctionCall(AssociativeCommutativOperation.Addition,
            //        new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //            new Literal(1.5f),
            //            new FunctionCall(BinaryOperation.Exponent,
            //                new VariableAccess(x),
            //                new Literal(2))),
            //        new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //            new Literal(-0.45f),
            //            new FunctionCall(BinaryOperation.Exponent,
            //                new VariableAccess(x),
            //                new Literal(5))));
            //expr =
            //    new FunctionCall(AssociativeCommutativOperation.Addition,
            //        expr2,
            //        new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //            new Literal(0.16875f),
            //            new FunctionCall(BinaryOperation.Exponent,
            //                new VariableAccess(x),
            //                new Literal(8))),
            //        new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //            new Literal(-0.018409091f),
            //            new FunctionCall(BinaryOperation.Exponent,
            //                new VariableAccess(x),
            //                new Literal(11))));
            //expr = engine.CleanUp(expr);
            //formula = new Formula("y3", expr);
            //_formulas.Add(formula);
            //graph = new Graph(formula, Color.Blue);
            //_graphs.Add(graph);

            //formula = new Formula("y2", expr2);
            //_formulas.Add(formula);
            //graph = new Graph(formula, Color.Red);
            //_graphs.Add(graph);

            //expr =
            //    new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //        new Literal(1.5f),
            //        new FunctionCall(BinaryOperation.Exponent,
            //            new VariableAccess(x),
            //            new Literal(2)));
            //formula = new Formula("y1", expr);
            //_formulas.Add(formula);
            //graph = new Graph(formula, Color.Green);
            //_graphs.Add(graph);
        }

        private void ligraControl1_Paint(object sender, PaintEventArgs e)
        {
            Render(e.Graphics);
        }

        private void Render(Graphics g)
        {
            float y = 10;
            float maxWidth = 0;

            foreach (Formula formula in _formulas)
            {
                float x = 0;

                SizeF exprSize = ligraControl1.CalcExpressionSize(g, formula.Expr);
                if (!string.IsNullOrEmpty(formula.Name))
                {
                    SizeF textSize = g.MeasureString(formula.Name + " = ", Font);
                    x = textSize.Width + 10;
                    g.DrawString(formula.Name + " = ", Font, Brushes.Blue, new PointF(10, y + (exprSize.Height - textSize.Height) / 2));
                }

                ligraControl1.RenderExpression(g, formula.Expr, new PointF(x, y), Pens.Blue, Brushes.Blue);
                y += exprSize.Height + 25;
                maxWidth = Math.Max(exprSize.Width + x, maxWidth);
            }

            AutoScrollMinSize = (new SizeF(maxWidth, y)).ToSize();

            VariableTable varTable = new VariableTable();
            varTable[_t] = Environment.TickCount / 5000.0f;

            bool first = true;
            foreach (Graph graph in _graphs)
            {
                ligraControl1.RenderGraph(g,
                    new RectangleF(600, 10, 400, 400),
                    //new RectangleF(75, 50, 500, 750),
                    new Pen(graph.Color), new SolidBrush(graph.Color),
                    -2, 2, -2, 2,
                    //0, 1, 0, 1.5f, 
                    graph.Formula.Expr, _x, varTable, first);
                first = false;

                //float value = graph.Formula.Expr.Eval(varTable).Value;
                //float yy = 750 - value * 500;

                //g.DrawString(value.ToString(), Font, new SolidBrush(graph.Color), 575 + 10, yy+50);
            }

            //string phi = new string((char)(0x03A6), 1);
            //g.DrawString(phi + "1", Font, Pens.Green.Brush, 500, 165);
            //g.DrawString(phi + "2", Font, Pens.Red.Brush, 545, 295);
            //g.DrawString(phi + "3", Font, Pens.Blue.Brush, 545, 185);
            //g.DrawString("Plots for problem 1 part (b)", Font, Pens.Black.Brush, 10, 10);

            first = true;
            foreach (Graph graph in _graphs3d)
            {
                ligraControl1.Render3DGraph(g, new RectangleF(600, 420, 400, 400),
                                            new Pen(graph.Color), new SolidBrush(graph.Color),
                                            -4, 0, -4, 0, -2, 6,
                                            graph.Formula.Expr, _x, _y, varTable, first);
                first = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Invalidate();
            Refresh();
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Render(e.Graphics);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog(this) == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }



    }
}
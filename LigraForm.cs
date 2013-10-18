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
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraForm : Form
    {
        public LigraForm()
        {
            _env.AddMacro(PlotExpression.PlotMacro.Value);
            _env.AddMacro(Plot3dExpression.Plot3dMacro.Value);
            _env.AddMacro(MathPaintExpression.MathpaintMacro.Value);

            InitializeComponent();

			this.Controls.Remove(this.toolStripContainer1);
			this.Controls.Add (this.splitContainer1);
        }

        private static SolusEngine _engine = new SolusEngine();
        private static LigraParser _parser = new LigraParser();

        //struct Formula
        //{
        //    public Formula(string name, Expression expr)
        //    {
        //        _name = name;
        //        _expr = expr;
        //    }

        //    private string _name;

        //    public string Name
        //    {
        //        get { return _name; }
        //        set { _name = value; }
        //    }

        //    private Expression _expr;

        //    public Expression Expr
        //    {
        //        get { return _expr; }
        //        set { _expr = value; }
        //    }

        //}

        //struct Graph
        //{
        //    public Graph(Formula formula, Color color)
        //    {
        //        _formula = formula;
        //        _color = color;
        //    }

        //    private Formula _formula;

        //    public Formula Formula
        //    {
        //        get { return _formula; }
        //        set { _formula = value; }
        //    }


        //    private Color _color;

        //    public Color Color
        //    {
        //        get { return _color; }
        //        set { _color = value; }
        //    }

        //}


        //List<Formula> _formulas = new List<Formula>();
        //List<Graph> _graphs = new List<Graph>();
        //List<Graph> _graphs3d = new List<Graph>();

        //Variable _x;
        //Variable _y;
        LigraEnvironment _env = new LigraEnvironment();
        //Set<Variable> _vars = new Set<Variable>();


        ////class TempFunction : Function
        ////{
        ////    public TempFunction(string name)
        ////        : base(name)
        ////    {
        ////    }

        ////    protected override Literal InternalCall(VariableTable env, Literal[] args)
        ////    {
        ////        throw new Exception("The method or operation is not implemented.");
        ////    }
        ////}

        //public class DoubleComparer : IComparer<double>
        //{
        //    #region IComparer<double> Members

        //    public int Compare(double x, double y)
        //    {
        //        return x.CompareTo(y);
        //    }

        //    #endregion
        //}

        ToolStripMenuItem _renderItemItem = new ToolStripMenuItem("Render Item");
        ToolStripMenuItem _propertiesItem = new ToolStripMenuItem("Properties");
        private void LigraForm_Load(object sender, EventArgs e)
        {
            InitializeCommands();

            evalTextBox.Focus();

            // u(pow(x*x+y*y,0.5)-2)*cos(5*(t-y))

            //Expression expr;
            //Expression expr2 = null;
            //Formula formula = new Formula();
            //Graph graph = new Graph();

            SetupContextMenu();


            //Variable x = new Variable("x");
            //Variable y = new Variable("y");

            //_vars.Add(x);
            //_vars.Add(y);
            if (!_env.Variables.ContainsKey("t"))
            {
                _env.Variables.Add("t", new Literal(0));
            }

            //VariableTable env = new VariableTable();

            //_x = x;
            //_y = y;

            //ProcessCommand("tsolve", null, "tsolve");

            //ProcessInput("cd \"C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images\"");
            //ProcessInput("loadimage a \"test.bmp\"");
            //ProcessInput("getrow2(b,a,0)");
            //ProcessInput("filters");

            ligraControl1.AutoScrollPosition = new Point(0, 0);

            //Function f = new TempFunction("f");
            ////f.Name = "f";

            //Variable s = new Variable("s");
            //Variable k = new Variable("K");
            //Variable Rx = new Variable("Rx");
            //Variable Ry = new Variable("Ry");

            //
            ////return;

            ////expr2 = new FunctionCall(MultiplicationOperation.Value,
            ////        new FunctionCall(AdditionOperation.Value,
            ////            new Literal(7),
            ////            new Literal(8)),
            ////        new FunctionCall(AdditionOperation.Value,
            ////            new Literal(9),
            ////            new VariableAccess(x)));

            ////expr = new FunctionCall(f,
            ////    new Literal(1),
            ////    new Literal(2),
            ////    new Literal(3),
            ////    new FunctionCall(DivisionOperation.Value,
            ////        new Literal(4),
            ////        new Literal(5)),
            ////    new Literal(6),
            ////    expr2.Clone());

            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "a"));
            ////_renderItems.Add(new ExpressionItem(expr2, Pens.Blue, Font, "b"));
            //////_formulas.Add(new Formula("a", expr));
            //////_formulas.Add(new Formula("b", expr2));

            ////expr = new FunctionCall(AdditionOperation.Value,
            ////        new FunctionCall(MultiplicationOperation.Value,
            ////            new Literal(7),
            ////            new Literal(8)),
            ////        new FunctionCall(MultiplicationOperation.Value,
            ////            new Literal(9),
            ////            new VariableAccess(new Variable("x"))));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "c"));
            //////_formulas.Add(new Formula("c", expr));

            ////expr =
            ////    new FunctionCall(Function.Cosine, new VariableAccess(x));
            //////_formulas.Add(new Formula("f", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "f"));

            ////expr = _engine.GetDerivative(expr,x);
            //////_formulas.Add(new Formula("df", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "df"));

            ////expr = _engine.GetDerivative(expr,x);
            //////_formulas.Add(new Formula("d2f", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "d2f"));

            ////expr = _engine.GetDerivative(expr,x);
            //////_formulas.Add(new Formula("d3f", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "d3f"));

            ////expr = _engine.GetDerivative(expr,x);
            //////_formulas.Add(new Formula("d4f", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "d4f"));

            ////expr =
            ////    new FunctionCall(DivisionOperation.Value,
            ////        new FunctionCall(MultiplicationOperation.Value,
            ////            new Literal(2),
            ////            new FunctionCall(Function.Cosine,
            ////                new VariableAccess(x))),
            ////        new VariableAccess(y));
            //////_formulas.Add(new Formula("g", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "g"));

            ////expr = _engine.GetDerivative(expr,x);
            //////_formulas.Add(new Formula("dg", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "dg"));

            ////expr =
            ////    new FunctionCall(MultiplicationOperation.Value,
            ////        new Literal(2),
            ////        new FunctionCall(Function.Cosine,
            ////            new VariableAccess(x)));
            //////_formulas.Add(new Formula("h", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "h"));

            ////expr = _engine.GetDerivative(expr);
            //////_formulas.Add(new Formula("dh", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "dh"));


            ////expr = new FunctionCall(DivisionOperation.Value,
            ////            new FunctionCall(MultiplicationOperation.Value,
            ////                new FunctionCall(MultiplicationOperation.Value,
            ////                    new VariableAccess(k),
            ////                    new FunctionCall(AdditionOperation.Value,
            ////                        new VariableAccess(s),
            ////                        new Literal(1))),
            ////                new FunctionCall(ExponentOperation.Value,
            ////                    new FunctionCall(AdditionOperation.Value,
            ////                        new VariableAccess(s),
            ////                        new Literal(10)),
            ////                    new Literal(2))),
            ////            new FunctionCall(MultiplicationOperation.Value,
            ////                new FunctionCall(MultiplicationOperation.Value,
            ////                    new FunctionCall(ExponentOperation.Value,
            ////                        new VariableAccess(s),
            ////                        new Literal(3)),
            ////                    new FunctionCall(AdditionOperation.Value,
            ////                        new VariableAccess(s),
            ////                        new Literal(4))),
            ////                new FunctionCall(MultiplicationOperation.Value,
            ////                    new FunctionCall(AdditionOperation.Value,
            ////                        new VariableAccess(s),
            ////                        new Literal(5)),
            ////                    new FunctionCall(AdditionOperation.Value,
            ////                        new VariableAccess(s),
            ////                        new Literal(6)))));
            //////_formulas.Add(new Formula("T(s)", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "T(s)"));

            ////expr =
            ////    new FunctionCall(Function.Sine,
            ////        new FunctionCall(MultiplicationOperation.Value,
            ////            new Literal(2),
            ////            new FunctionCall(MultiplicationOperation.Value,
            ////                new Literal(2),
            ////                new FunctionCall(MultiplicationOperation.Value,
            ////                    new VariableAccess(x),
            ////                    new VariableAccess(y)))));
            //////_formulas.Add(new Formula("z", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "z"));

            //////_formulas.Add(new Formula("dz", _engine.GetDerivative(expr)));
            ////_renderItems.Add(new ExpressionItem(_engine.GetDerivative(expr), Pens.Blue, Font, "dz"));

            ////expr =
            ////    new FunctionCall(ExponentOperation.Value,
            ////        new VariableAccess(x),
            ////        new VariableAccess(y));
            //////_formulas.Add(new Formula("u", expr));
            //////_formulas.Add(new Formula("du", _engine.GetDerivative(expr)));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "u"));
            ////_renderItems.Add(new ExpressionItem(engine.GetDerivative(expr), Pens.Blue, Font, "du"));

            ////expr =
            ////    new FunctionCall(DivisionOperation.Value,
            ////        new FunctionCall(MultiplicationOperation.Value,
            ////            new VariableAccess(Rx),
            ////            new VariableAccess(Ry)),
            ////        new FunctionCall(AdditionOperation.Value,
            ////            new VariableAccess(Rx),
            ////            new VariableAccess(Ry)));
            //////_formulas.Add(new Formula("Req", expr));
            //////_formulas.Add(new Formula("dReq", engine.GetDerivative(expr)));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "Req"));
            ////_renderItems.Add(new ExpressionItem(engine.GetDerivative(expr), Pens.Blue, Font, "dReq"));








            //////expr2 = engine.GetDerivative(expr);
            //////_expressions.Add(expr2);

            ////expr =
            ////    new FunctionCall(AdditionOperation.Value,
            ////        new FunctionCall(ExponentOperation.Value,
            ////            new VariableAccess(x),
            ////            new Literal(3)),
            ////        new FunctionCall(MultiplicationOperation.Value,
            ////            new Literal(3),
            ////            new FunctionCall(ExponentOperation.Value,
            ////                new VariableAccess(x),
            ////                new Literal(2))),
            ////        new FunctionCall(MultiplicationOperation.Value,
            ////            new Literal(3),
            ////            new VariableAccess(x)),
            ////        new Literal(1));

            //////_formulas.Add(new Formula("q", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "q"));
            ////expr = engine.GetDerivative(expr);
            //////_formulas.Add(new Formula("dq", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "dq"));
            ////expr = engine.GetDerivative(expr);
            //////_formulas.Add(new Formula("d2q", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "d2q"));
            ////expr = engine.GetDerivative(expr);
            //////_formulas.Add(new Formula("d3q", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font, "d3q"));

            ////expr =
            ////    new FunctionCall(ExponentOperation.Value,
            ////        new FunctionCall(AdditionOperation.Value,
            ////            new Literal(2),
            ////            new Literal(3)),
            ////        new Literal(4));
            //////_formulas.Add(new Formula("", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font));
            ////expr =
            ////    new FunctionCall(ExponentOperation.Value,
            ////        new Literal(4),
            ////        new FunctionCall(AdditionOperation.Value,
            ////            new Literal(2),
            ////            new Literal(3)));
            //////_formulas.Add(new Formula("", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font));


            ////expr =
            ////    new FunctionCall(ExponentOperation.Value,
            ////        new FunctionCall(MultiplicationOperation.Value,
            ////            new Literal(2),
            ////            new Literal(3)),
            ////        new Literal(4));
            //////_formulas.Add(new Formula("", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font));
            ////expr =
            ////    new FunctionCall(ExponentOperation.Value,
            ////        new Literal(4),
            ////        new FunctionCall(MultiplicationOperation.Value,
            ////            new Literal(2),
            ////            new Literal(3)));
            //////_formulas.Add(new Formula("", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font));
            ////expr =
            ////    new FunctionCall(ExponentOperation.Value,
            ////        new FunctionCall(DivisionOperation.Value,
            ////            new Literal(2),
            ////            new Literal(3)),
            ////        new Literal(4));
            //////_formulas.Add(new Formula("", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font));
            ////expr =
            ////    new FunctionCall(ExponentOperation.Value,
            ////        new Literal(4),
            ////        new FunctionCall(DivisionOperation.Value,
            ////            new Literal(2),
            ////            new Literal(3)));
            //////_formulas.Add(new Formula("", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font));

            ////expr =
            ////    new FunctionCall(ExponentOperation.Value,
            ////        new FunctionCall(ExponentOperation.Value,
            ////            new Literal(2),
            ////            new Literal(3)),
            ////        new Literal(4));
            //////_formulas.Add(new Formula("", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font));
            ////expr =
            ////    new FunctionCall(ExponentOperation.Value,
            ////        new Literal(4),
            ////        new FunctionCall(ExponentOperation.Value,
            ////            new Literal(2),
            ////            new Literal(3)));
            //////_formulas.Add(new Formula("", expr));
            ////_renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font));

            ////expr =
            ////    //new FunctionCall(AssociativeCommutativOperation.Addition,
            ////    //    new FunctionCall(ExponentOperation.Value,
            ////    //        new FunctionCall(AssociativeCommutativOperation.Addition,
            ////    //            new VariableAccess(x),
            ////    //            new Literal(-3)),
            ////    //        new Literal(3)),
            ////    //    new FunctionCall(AssociativeCommutativOperation.Multiplication,
            ////    //        new Literal(-1),
            ////    //        new VariableAccess(x)));

            ////    new FunctionCall(ExponentOperation.Value,
            ////        new VariableAccess(x),
            ////        new Literal(3));

            ////    //new FunctionCall(ExponentOperation.Value,
            ////    //    new Literal((float)Math.E),
            ////    //    new VariableAccess(x));






            ////formula = new Formula("y", expr);
            ////_formulas.Add(formula);
            ////graph = new Graph(formula, Color.Blue);
            ////_graphs.Add(graph);

            ////expr =
            ////    new FunctionCall(MultiplicationOperation.Value,
            ////        new Literal(3),
            ////        new FunctionCall(ExponentOperation.Value,
            ////            new VariableAccess(x),
            ////            new Literal(2)));
            ////formula = new Formula("y'", expr);
            ////_formulas.Add(formula);
            ////graph = new Graph(formula, Color.Green);
            ////_graphs.Add(graph);

            ////expr =
            ////    new FunctionCall(MultiplicationOperation.Value,
            ////        new Literal(6),
            ////        new VariableAccess(x));
            ////formula = new Formula("y''", expr);
            ////_formulas.Add(formula);
            ////graph = new Graph(formula, Color.Red);
            ////_graphs.Add(graph);

            ////expr =
            ////    new FunctionCall(Function.Cosine,
            ////        new FunctionCall(AdditionOperation.Value,
            ////            new VariableAccess(x),
            ////            new VariableAccess(t)));
            ////formula = new Formula("cos(x + t)", expr);
            ////_formulas.Add(formula);
            ////graph = new Graph(formula, Color.Cyan);
            ////_graphs.Add(graph);

            ////expr =
            ////    new FunctionCall(Function.Sine,
            ////        new FunctionCall(AdditionOperation.Value,
            ////            new VariableAccess(x),
            ////            new FunctionCall(MultiplicationOperation.Value,
            ////                new Literal(2),
            ////                new VariableAccess(t))));
            ////formula = new Formula("sin(x + 2t)", expr);
            ////_formulas.Add(formula);
            ////graph = new Graph(formula, Color.Magenta);
            ////_graphs.Add(graph);

            ////expr =
            ////    new FunctionCall(Function.Tangent,
            ////        new FunctionCall(AdditionOperation.Value,
            ////            new VariableAccess(x),
            ////            new FunctionCall(MultiplicationOperation.Value,
            ////                new Literal(0.5f),
            ////                new VariableAccess(t))));
            ////formula = new Formula("tan(x + t/2)", expr);
            ////_formulas.Add(formula);
            ////graph = new Graph(formula, Color.Yellow);
            ////_graphs.Add(graph);


            ////expr =
            ////    new FunctionCall(MultiplicationOperation.Value,
            ////        new FunctionCall(Function.UnitStep,
            ////            new FunctionCall(AdditionOperation.Value,
            ////                new FunctionCall(ExponentOperation.Value,
            ////                    new FunctionCall(AdditionOperation.Value,
            ////                        new FunctionCall(ExponentOperation.Value,
            ////                            new VariableAccess(x),
            ////                            new Literal(2)),
            ////                        new FunctionCall(ExponentOperation.Value,
            ////                            new VariableAccess(y),
            ////                            new Literal(2))),
            ////                    new Literal(0.5f)),
            ////                new Literal(-1))),
            ////        new FunctionCall(Function.Cosine,
            ////            new FunctionCall(MultiplicationOperation.Value,
            ////                new Literal(5),
            ////                new FunctionCall(AdditionOperation.Value,
            ////                    new VariableAccess(y),
            ////                    new FunctionCall(MultiplicationOperation.Value,
            ////                        new Literal(-1),
            ////                        new VariableAccess(t))))));
            ////formula = new Formula("z(x,y)", expr);
            ////_formulas.Add(formula);
            ////graph = new Graph(formula, Color.Green);
            ////_graphs3d.Add(graph);

            //////(new ObjectMapForm(expr)).ShowDialog(this);

            ////env.Clear();
            ////env.Variables[x] = 0;
            ////expr2 = engine.PreliminaryEval(expr, env);
            //////(new ObjectMapForm(expr2)).ShowDialog(this);
            ////formula = new Formula("z(0,y)", expr2);
            ////_formulas.Add(formula);

            ////env.Variables[y] = 0;
            ////expr2 = engine.PreliminaryEval(expr2, env);
            //////(new ObjectMapForm(expr2)).ShowDialog(this);
            ////formula = new Formula("z(0,0)", expr2);
            ////_formulas.Add(formula);

            ////env.Variables[y] = 1;
            ////expr2 = engine.PreliminaryEval(expr2, env);
            //////(new ObjectMapForm(expr2)).ShowDialog(this);
            ////formula = new Formula("z(0,1)", expr2);
            ////_formulas.Add(formula);

            ////env.Clear();
            ////env.Variables[x] = 1;
            ////expr2 = engine.PreliminaryEval(expr, env);
            //////(new ObjectMapForm(expr2)).ShowDialog(this);
            ////formula = new Formula("z(1,y)", expr2);
            ////_formulas.Add(formula);

            ////env.Variables[y] = 0;
            ////expr2 = engine.PreliminaryEval(expr2, env);
            //////(new ObjectMapForm(expr2)).ShowDialog(this);
            ////formula = new Formula("z(1,0)", expr2);
            ////_formulas.Add(formula);

            ////env.Variables[y] = 1;
            ////expr2 = engine.PreliminaryEval(expr2, env);
            //////(new ObjectMapForm(expr2)).ShowDialog(this);
            ////formula = new Formula("z(1,1)", expr2);
            ////_formulas.Add(formula);

            ////expr =
            ////    new FunctionCall(AdditionOperation.Value,
            ////        new VariableAccess(x),
            ////        new VariableAccess(y),
            ////        new Literal(1));
            ////formula = new Formula("f(x,y)", expr);
            ////_formulas.Add(formula);

            ////env.Clear();
            ////env.Variables[x] = 5;
            ////expr = engine.PreliminaryEval(expr, env);
            ////formula = new Formula("f(5,y)", expr);
            ////_formulas.Add(formula);

            ////env.Variables[y] = 3;
            ////expr = engine.PreliminaryEval(expr, env);
            ////formula = new Formula("f(5,3)", expr);
            ////_formulas.Add(formula);

            //////_graphs.Clear();
            //////_graphs3d.Clear();

            //////expr2 =
            //////    new FunctionCall(AssociativeCommutativOperation.Addition,
            //////        new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //////            new Literal(1.5f),
            //////            new FunctionCall(ExponentOperation.Value,
            //////                new VariableAccess(x),
            //////                new Literal(2))),
            //////        new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //////            new Literal(-0.45f),
            //////            new FunctionCall(ExponentOperation.Value,
            //////                new VariableAccess(x),
            //////                new Literal(5))));
            //////expr =
            //////    new FunctionCall(AssociativeCommutativOperation.Addition,
            //////        expr2,
            //////        new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //////            new Literal(0.16875f),
            //////            new FunctionCall(ExponentOperation.Value,
            //////                new VariableAccess(x),
            //////                new Literal(8))),
            //////        new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //////            new Literal(-0.018409091f),
            //////            new FunctionCall(ExponentOperation.Value,
            //////                new VariableAccess(x),
            //////                new Literal(11))));
            //////expr = engine.CleanUp(expr);
            //////formula = new Formula("y3", expr);
            //////_formulas.Add(formula);
            //////graph = new Graph(formula, Color.Blue);
            //////_graphs.Add(graph);

            //////formula = new Formula("y2", expr2);
            //////_formulas.Add(formula);
            //////graph = new Graph(formula, Color.Red);
            //////_graphs.Add(graph);

            //////expr =
            //////    new FunctionCall(AssociativeCommutativOperation.Multiplication,
            //////        new Literal(1.5f),
            //////        new FunctionCall(ExponentOperation.Value,
            //////            new VariableAccess(x),
            //////            new Literal(2)));
            //////formula = new Formula("y1", expr);
            //////_formulas.Add(formula);
            //////graph = new Graph(formula, Color.Green);
            //////_graphs.Add(graph);
        }

        private ToolStripMenuItem _clearItem = new ToolStripMenuItem("Clear");
        private void SetupContextMenu()
        {
            _clearItem.Click += new EventHandler(ClearItem_Click);
            ligraControl1.ContextMenuStrip.Items.Add(_clearItem);

            ligraControl1.ContextMenuStrip.Items.Add(_renderItemItem);

            _propertiesItem.Click += new EventHandler(PropertiesItem_Click);
            ligraControl1.ContextMenuStrip.Items.Add(_propertiesItem);

            ligraControl1.ContextMenuStrip.Opening += new CancelEventHandler(ligraControl1_ContextMenuStrip_Opening);
        }

        void PropertiesItem_Click(object sender, EventArgs e)
        {
            if (_selectedRenderItem != null)
            {
                _selectedRenderItem.OpenPropertiesWindow(ligraControl1);

                //reset render item positions?
            }
        }

        RenderItem _selectedRenderItem;
        void ligraControl1_ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menu = ligraControl1.ContextMenuStrip;
            Point pt = menu.ClientRectangle.Location;
            pt = new Point(menu.Left, menu.Top);

            RenderItem ri = GetRenderItemFromPoint(pt);
            _selectedRenderItem = ri;

            if (ri != null)
            {
                _renderItemItem.DropDownItems.Clear();

                ToolStripItem[] menuItems = ri.GetMenuItems();

                if (menuItems.Length > 0)
                {
                    _renderItemItem.DropDownItems.AddRange(menuItems);
                    _renderItemItem.Enabled = true;
                }
                else
                {
                    _renderItemItem.Enabled = false;
                }

                _propertiesItem.Enabled = ri.HasPropertyWindow;
            }
            else
            {
                _propertiesItem.Enabled = false;
            }
        }

        private RenderItem GetRenderItemFromPoint(PointF pt)
        {
            return GetRenderItemInCollectionFromPoint(_env.RenderItems, pt);
        }

        private RenderItem GetRenderItemInCollectionFromPoint(IEnumerable<RenderItem> items, PointF pt)
        {
            foreach (RenderItem ri in items)
            {
                if (ri is RenderItemContainer)
                {
                    return GetRenderItemInCollectionFromPoint(((RenderItemContainer)ri).Items, pt);
                }
                else if (ri.Rect.Contains(pt))
                {
                    return ri;
                }
            }

            return null;
        }


        void ClearItem_Click(object sender, EventArgs e)
        {
            ClearOutput();
        }

        private void ClearOutput()
        {
            _env.RenderItems.Clear();
            ligraControl1.Invalidate();
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_env.RenderItems.Count > 0 && WindowState != FormWindowState.Minimized)
            {
                //Invalidate();
                //Refresh();
                ligraControl1.Refresh();
            }
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

        private void evalButton_Click(object sender, EventArgs e)
        {
            //SolusParser parser = new SolusParser();

            string input = evalTextBox.Text;

            if (string.IsNullOrEmpty(input)) { input = string.Empty; }

            input = input.Trim();

            if (input.Length > 0)
            {
                try
                {
                    ProcessInput(input);
                }
                catch (Exception ee)
                {
                    if (ee is SolusParseException)
                    {
                        SolusParseException ee2 = (SolusParseException)ee;
                        _env.RenderItems.Add(new ErrorItem(input, ee2.Error, ligraControl1.Font, Brushes.Red, ee2.Location));
                    }
                    else
                    {
                        _env.RenderItems.Add(new ErrorItem(input, "There was an error: " + ee.ToString(), Font, Brushes.Red));
                    }
                }
            }
            
            Invalidate();
        }

        private void evalTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (_env.History.Count > 0)
                {
                    if (_env.CurrentHistoryIndex < 0)
                    {
                        _env.CurrentHistoryIndex = _env.History.Count - 1;
                    }
                    else
                    {
                        _env.CurrentHistoryIndex--;
                        if (_env.CurrentHistoryIndex < 0) _env.CurrentHistoryIndex = 0;
                    }

                    evalTextBox.Text = _env.History[_env.CurrentHistoryIndex];
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (_env.History.Count > 0)
                {
                    if (_env.CurrentHistoryIndex < 0)
                    {
                        _env.CurrentHistoryIndex = _env.History.Count - 1;
                    }
                    else
                    {
                        _env.CurrentHistoryIndex++;
                        if (_env.CurrentHistoryIndex >= _env.History.Count) _env.CurrentHistoryIndex = _env.History.Count - 1;
                    }

                    evalTextBox.Text = _env.History[_env.CurrentHistoryIndex];
                }
            }
        }

        private void LigraForm_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDown(e);
        }

        private void ProcessKeyDown(KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Down)
            //{
            //    Point pt = ligraControl1.AutoScrollPosition;
            //    pt.Y += 20;
            //    ligraControl1.AutoScrollPosition = pt;
            //}
            //else if (e.KeyCode == Keys.Up)
            //{
            //}
            //else if (e.KeyCode == Keys.PageDown)
            //{
            //}
            //else if (e.KeyCode == Keys.PageUp)
            //{
            //}
        }

        private void ligraControl1_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDown(e);
        }



    }
}
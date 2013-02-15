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

        protected delegate void Command(string input, SolusParser.Ex[] exTokens);

        Dictionary<string, Command> _commands = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);

        private void InitializeCommands()
        {
            _commands["help"] = new Command(HelpCommand);
            _commands["clear"] = new Command(ClearCommand);
            _commands["vars"] = new Command(VarsCommand);
            _commands["delete"] = new Command(DeleteCommand);
            _commands["history"] = new Command(HistoryCommand);
            _commands["example"] = new Command(ExampleCommand);
            _commands["tsolve"] = new Command(TSolveCommand);
            _commands["loadimage"] = new Command(LoadImageCommand);
            _commands["cd"] = new Command(CdCommand);
            _commands["filters"] = new Command(FiltersCommand);
            _commands["filters2"] = new Command(FiltersCommand2);
            _commands["filters3"] = new Command(FiltersCommand3);
            _commands["fourier"] = new Command(FourierCommand);
            _commands["filters4"] = new Command(FiltersCommand4);
            _commands["filtersexperiment"] = new Command(FiltersExperiment);
            _commands["saveimage"] = new Command(SaveImageCommand);
            _commands["p2t1"] = new Command(FiltersProject2Test1);
            _commands["ssim"] = new Command(SsimTest);
            _commands["p2t2"] = new Command(FiltersProject2Trial2);
            _commands["mv"] = new Command(MvCommand);
            _commands["p3t1"] = new Command(Project3Test1Command);
            _commands["p2t3"] = new Command(FiltersProject2Trial3);
            _commands["p3t2"] = new Command(Project3Test2Command);
            _commands["p3t3"] = new Command(Project3Test3Command);
            _commands["atmmse"] = AtmmseCommand;
            _commands["ztmmse"] = ZtmmseCommand;
        }

        private void ProcessCommand(string input, SolusParser.Ex[] exTokens, string cmd)
        {
            if (IsCommand(cmd))
            {
                _commands[cmd](input, exTokens);
            }
        }

        private void DeleteCommand(string input, SolusParser.Ex[] exTokens)
        {
            if (exTokens.Length > 1)
            {
                List<string> unknownVars = new List<string>();

                int i;
                for (i = 1; i < exTokens.Length; i++)
                {
                    if (!_vars.ContainsKey(exTokens[i].Token))
                    {
                        unknownVars.Add(exTokens[i].Token);
                    }
                }

                if (unknownVars.Count > 0)
                {
                    string error = "The following variables do not exist: \r\n";
                    foreach (string s in unknownVars)
                    {
                        error += s + "\r\n";
                    }
                    _renderItems.Add(new ErrorItem(input, error, ligraControl1.Font, Brushes.Red, exTokens[0].Location));
                }
                else
                {
                    for (i = 1; i < exTokens.Length; i++)
                    {
                        _vars.Remove(exTokens[i].Token);
                    }

                    _renderItems.Add(new InfoItem("The variables were deleted successfully.", ligraControl1.Font));
                }
            }
            else
            {
                _renderItems.Add(new ErrorItem(input, "Must specify variables to delete", ligraControl1.Font, Brushes.Red, exTokens[0].Location));
            }
        }

        private void VarsCommand(string input, SolusParser.Ex[] exTokens)
        {
            string s = string.Empty;
            foreach (Variable var in _vars)
            {
                Expression value = _vars[var];
                string valueString = value.ToString();

                if (value is SolusVector)
                {
                    valueString = "Vector (" + ((SolusVector)value).Length.ToString() + ")";
                }
                else if (value is SolusMatrix)
                {
                    SolusMatrix mat = (SolusMatrix)value;
                    valueString = "Matrix (" + mat.RowCount + ", " + mat.ColumnCount + ")";
                }

                s += var.Name + " = " + valueString + "\r\n";
            }

            _renderItems.Add(new InfoItem(s, ligraControl1.Font));
        }

        private void ClearCommand(string input, SolusParser.Ex[] exTokens)
        {
            if (exTokens.Length > 1)
            {
                if (exTokens[1].Token.ToLower() == "history")
                {
                    ClearHistory();
                }
                else if (exTokens[1].Token.ToLower() == "all")
                {
                    ClearHistory();
                    ClearOutput();
                }
                else
                {
                    ClearOutput();
                }
            }
            else
            {
                ClearOutput();
            }
        }

        private void HelpCommand(string input, SolusParser.Ex[] exTokens)
        {
            if (exTokens.Length > 1)
            {
                _renderItems.Add(new HelpItem(ligraControl1.Font, exTokens[1].Token));
            }
            else
            {
                _renderItems.Add(new HelpItem(ligraControl1.Font));
            }
        }

        private void HistoryCommand(string input, SolusParser.Ex[] exTokens)
        {
            if (exTokens.Length > 1 && exTokens[1].Token.ToLower() == "clear")
            {
                ClearHistory();
            }
            else
            {
                string s = string.Join("\r\n", _inputHistory.ToArray());
                _renderItems.Add(new InfoItem(s + "\r\n", ligraControl1.Font));
            }
        }

        private void ExampleCommand(string input, SolusParser.Ex[] exTokens)
        {
            Font f = ligraControl1.Font;
            Pen p = Pens.Blue;

            if (!_vars.ContainsKey("x")) _vars.Add(new Variable("x"));
            if (!_vars.ContainsKey("y")) _vars.Add(new Variable("y"));
            if (!_vars.ContainsKey("mu")) _vars.Add(new Variable("mu"));
            if (!_vars.ContainsKey("sigma")) _vars.Add(new Variable("sigma"));

            Expression expr;

            _renderItems.Add(new InfoItem("A number:", f));
            _renderItems.Add(new ExpressionItem(new Literal(123.45f), p, f));

            _renderItems.Add(new InfoItem("A variable:", f));
            _renderItems.Add(new ExpressionItem(new VariableAccess(_vars["x"]), p, f));

            _renderItems.Add(new InfoItem("A function call: ", f));
            _renderItems.Add(new ExpressionItem(
                new FunctionCall(
                    Function.Cosine,
                    new VariableAccess(_vars["x"])), p, f));

            _renderItems.Add(new InfoItem("A simple expression,  \"x + y/2\" :", f));
            _renderItems.Add(new ExpressionItem(
                new FunctionCall(
                    AssociativeCommutativeOperation.Addition,
                    new VariableAccess(_vars["x"]),
                    new FunctionCall(
                        BinaryOperation.Division,
                        new VariableAccess(_vars["y"]),
                        new Literal(2))), p, f));

            _renderItems.Add(new InfoItem("Some derivatives, starting with x^3:", f));
            expr = _parser.Compile("x^3", _vars);
            _renderItems.Add(new ExpressionItem(expr, p, f));
            DerivativeTransformer derive = new DerivativeTransformer();
            expr = derive.Transform(expr, new VariableTransformArgs(_vars["x"]));
            _renderItems.Add(new ExpressionItem(expr, p, f));
            expr = derive.Transform(expr, new VariableTransformArgs(_vars["x"]));
            _renderItems.Add(new ExpressionItem(expr, p, f));
            expr = derive.Transform(expr, new VariableTransformArgs(_vars["x"]));
            _renderItems.Add(new ExpressionItem(expr, p, f));

            _renderItems.Add(new InfoItem("Some variable assignments: ", f));
            _renderItems.Add(new ExpressionItem(new AssignExpression(_vars["mu"], new Literal(0.5f)), p, f));
            _renderItems.Add(new ExpressionItem(new AssignExpression(_vars["sigma"], new Literal(0.2f)), p, f));

            _vars[_vars["mu"]] = new Literal(0.5f);
            _vars[_vars["sigma"]] = new Literal(0.2f);

            expr =
                new FunctionCall(
                    AssociativeCommutativeOperation.Multiplication,
                    new FunctionCall(
                        BinaryOperation.Division,
                            new Literal(1),
                            new FunctionCall(
                                AssociativeCommutativeOperation.Multiplication,
                                new FunctionCall(
                                    BinaryOperation.Exponent,
                                    new FunctionCall(
                                        AssociativeCommutativeOperation.Multiplication,
                                        new Literal(2),
                                        new Literal((float)Math.PI)),
                                    new Literal(0.5f)),
                                new VariableAccess(_vars["sigma"]))),
                    new FunctionCall(
                        BinaryOperation.Exponent,
                        new Literal((float)Math.E),
                        new FunctionCall(
                            BinaryOperation.Division,
                            new FunctionCall(
                                BinaryOperation.Exponent,
                                new FunctionCall(
                                    AssociativeCommutativeOperation.Addition,
                                    new VariableAccess(_vars["x"]),
                                    new FunctionCall(
                                        AssociativeCommutativeOperation.Multiplication,
                                        new Literal(-1),
                                        new VariableAccess(_vars["mu"]))),
                                new Literal(2)),
                            new FunctionCall(
                                AssociativeCommutativeOperation.Multiplication,
                                new Literal(-2),
                                new FunctionCall(
                                    BinaryOperation.Exponent,
                                    new VariableAccess(_vars["sigma"]),
                                    new Literal(2))))));

            _renderItems.Add(new InfoItem("A complex expression, \"(1/(sigma*sqrt(2*pi))) * e ^ ( (x - mu)^2 / (-2 * sigma^2))\"", f));
            _renderItems.Add(new ExpressionItem(expr, p, f));
            //(1/(sigma*sqrt(2*pi))) * e ^ ( (x - mu)^2 / (-2 * sigma^2))

            _renderItems.Add(new InfoItem("A plot of the expression: ", f));
            _renderItems.Add(new GraphItem(expr, p, _vars["x"], _parser));

            _renderItems.Add(new InfoItem("Multiple plots on the same axes, \"x^3\", \"3 * x^2\", \"6 * x\":", f));
            _renderItems.Add(new GraphItem(
                _parser,
                new GraphEntry(_parser.Compile("x^3", _vars), Pens.Blue, _vars["x"]),
                new GraphEntry(_parser.Compile("3*x^2", _vars), Pens.Green, _vars["x"]),
                new GraphEntry(_parser.Compile("6*x", _vars), Pens.Red, _vars["x"])));

            _renderItems.Add(new InfoItem("A plot that changes with time, \"sin(x+t)\":", f));
            _renderItems.Add(new GraphItem(_parser.Compile("sin(x+t)", _vars), p, _vars["x"], _parser));

            expr = _parser.Compile("unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)", _vars);
            _renderItems.Add(new InfoItem("Another complex expression, \"unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)\",\r\nwhere t is time:", f));
            _renderItems.Add(new ExpressionItem(expr, p, f));

            _renderItems.Add(new InfoItem("A 3d plot: ", f));
            _renderItems.Add(new Graph3dItem(expr, Pens.Black, Brushes.Green, -4, 4, -4, 4, -2, 6, _vars["x"], _vars["y"]));
        }

        private void TSolveCommand(string input, SolusParser.Ex[] exTokens)
        {
            SolusEngine _engine = new SolusEngine();
            SolusMatrix m = new SolusMatrix(7, 7);

            Variable k;
            Variable r;
            Variable cs;

            if (!_vars.ContainsKey("k")) { _vars.Add(new Variable("k")); }
            if (!_vars.ContainsKey("R")) { _vars.Add(new Variable("R")); }
            if (!_vars.ContainsKey("Cs")) { _vars.Add(new Variable("Cs")); }

            k = _vars["k"];
            r = _vars["R"];
            cs = _vars["Cs"];

            int i;
            int j;

            Literal zero = new Literal(0);
            Literal one = new Literal(1);
            Literal negOne = new Literal(-1);
            VariableAccess kk = new VariableAccess(k);
            VariableAccess rr = new VariableAccess(r);
            VariableAccess cc = new VariableAccess(cs);
            Function mult = AssociativeCommutativeOperation.Multiplication;
            Function div = BinaryOperation.Division;
            Function add = AssociativeCommutativeOperation.Addition;

            for (i = 0; i < 7; i++)
            {
                for (j = 0; j < 7; j++)
                {
                    m[i, j] = zero;
                }
            }

            m[0, 0] = one;
            m[0, 2] = new FunctionCall(mult, negOne, kk);
            m[1, 1] = new FunctionCall(div, one, rr);
            m[1, 3] = new FunctionCall(div, negOne, rr);
            m[1, 4] = one;
            m[2, 4] = one;
            m[2, 5] = negOne;
            m[2, 6] = negOne;
            m[3, 0] = cc;
            m[3, 1] = new FunctionCall(mult, negOne, cc);
            m[3, 5] = one;
            m[4, 1] = new FunctionCall(div, negOne, rr);
            m[4, 2] = new FunctionCall(div, one, rr);
            m[4, 6] = one;
            m[5, 1] =
                new FunctionCall(div, negOne,
                    new FunctionCall(add, rr,
                        new FunctionCall(div, one, cc)));
            m[5, 6] = one;
            m[6, 2] = new FunctionCall(mult, negOne, cc);
            m[6, 6] = one;


            Expression factor = null;


            //factor = 
            //    new FunctionCall(div, negOne,
            //        new FunctionCall(add, rr,
            //            new FunctionCall(div, one, cc)));
            //row = 3;
            //MultiplyRow(_engine, m, mult, factor, row);

            //factor = new FunctionCall(mult, negOne, cc);
            //row = 5;
            //MultiplyRow(_engine, m, mult, factor, row);

            factor = new FunctionCall(mult, negOne, cc);
            AddMultRow(_engine, m, 0, 3, factor);

            AddRow(_engine, m, 1, 4);
            factor = rr;
            MultiplyRow(_engine, m, factor, 1);
            AddMultRow(_engine, m, 1, 3, cc);
            factor =
                new FunctionCall(div, one,
                    new FunctionCall(add, rr,
                        new FunctionCall(div, one, cc)));
            AddMultRow(_engine, m, 1, 5, factor);

            m[0, 3] = zero;
            m[1, 1] = one;
            m[3, 0] = zero;
            m[3, 1] = zero;
            m[4, 1] = zero;
            m[5, 1] = zero;

            MultiplyRow(_engine, m, rr, 4);

            m[1, 3] = one;
            m[4, 2] = one;
            m[4, 3] = negOne;

            _renderItems.Add(new ExpressionItem(m, Pens.Blue, ligraControl1.Font));
            ligraControl1.Invalidate();
        }

        private static void AddMultRow(SolusEngine engine, SolusMatrix m, int rowFrom, int rowTo, Expression factor)
        {
            int i;
            for (i = 0; i < 7; i++)
            {
                Expression expr;
                CleanUpTransformer cleanup = new CleanUpTransformer();
                expr = cleanup.CleanUp(new FunctionCall(AssociativeCommutativeOperation.Multiplication, m[rowFrom, i], factor));
                expr = cleanup.CleanUp(new FunctionCall(AssociativeCommutativeOperation.Addition, expr, m[rowTo, i]));
                m[rowTo, i] = expr;
            }
        }

        private static void AddRow(SolusEngine engine, SolusMatrix m, int rowFrom, int rowTo)
        {
            int i;
            for (i = 0; i < 7; i++)
            {
                Expression expr;
                CleanUpTransformer cleanup = new CleanUpTransformer();
                expr = cleanup.CleanUp(new FunctionCall(AssociativeCommutativeOperation.Addition, m[rowFrom, i], m[rowTo, i]));
                m[rowTo, i] = expr;
            }
        }

        private static void MultiplyRow(SolusEngine engine, SolusMatrix m, Expression factor, int row)
        {
            int i;
            for (i = 0; i < 7; i++)
            {
                Expression expr = m[row, i];
                CleanUpTransformer cleanup = new CleanUpTransformer();
                expr = cleanup.CleanUp(new FunctionCall(AssociativeCommutativeOperation.Multiplication, expr, factor));
                m[row, i] = expr;
            }
        }

        private bool IsCommand(string cmd)
        {
            return _commands.ContainsKey(cmd);
        }

        private void LoadImageCommand(string input, SolusParser.Ex[] exTokens)
        {
            Font font = ligraControl1.Font;
            Brush brush = Brushes.Red;

            if (exTokens.Length < 3)
            {
                _renderItems.Add(new ErrorItem(input, "Too few parameters", font, brush, exTokens[0].Location));
            }
            else if (exTokens[1].Type != SolusParser.NodeType.Var)
            {
                _renderItems.Add(new ErrorItem(input, "Parameter must be a variable", font, brush, exTokens[1].Location));
            }
            else if (exTokens[2].Type != SolusParser.NodeType.String)
            {
                _renderItems.Add(new ErrorItem(input, "Parameter must be a filename", font, brush, exTokens[2].Location));
            }
            else
            {
                string filename = exTokens[2].GetStringContents();
                string varName = exTokens[1].Token;
                try
                {
                    SolusMatrix mat = SolusEngine.LoadImage(filename);

                    if (!_vars.ContainsKey(varName))
                    {
                        _vars.Add(new Variable(varName));
                    }

                    _vars[_vars[varName]] = mat;

                    _renderItems.Add(new InfoItem("Image loaded successfully", font));
                }
                catch (Exception e)
                {
                    _renderItems.Add(new ErrorItem(input, "There was an error while loading the file: \r\n" + filename + "\r\n" + e.ToString(), font, brush));
                }
            }
        }

        private void CdCommand(string input, SolusParser.Ex[] exTokens)
        {
            if (exTokens.Length <= 1)
            {
                //print the current directory
                string dir = System.IO.Directory.GetCurrentDirectory();
                _renderItems.Add(new InfoItem(dir, ligraControl1.Font));
            }
            else if (exTokens[1].Type != SolusParser.NodeType.String)
            {
                _renderItems.Add(new ErrorItem(input, "Parameter must be a string", ligraControl1.Font, Brushes.Red, exTokens[1].Location));
            }
            else
            {
                //set the current directory
                string dir = exTokens[1].GetStringContents();

                try
                {
                    System.IO.Directory.SetCurrentDirectory(dir);
                    _renderItems.Add(new InfoItem("Directory changed to \"" + dir + "\"", ligraControl1.Font));
                }
                catch (Exception e)
                {
                    _renderItems.Add(new ErrorItem(input, "There was an error: \r\n" + e.ToString(), ligraControl1.Font, Brushes.Red));
                }
            }
        }

        private void ProcessInput(string input)
        {
            SolusParser.Ex[] exTokens = SolusParser.Tokenize(input);

            if (exTokens.Length > 0)
            {
                string cmd = exTokens[0].Token.ToLower();

                if (IsCommand(cmd))
                {
                    ProcessCommand(input, exTokens, cmd);
                }
                else
                {
                    Expression expr = _parser.Compile(exTokens, _vars);

                    if (expr != null)
                    {
                        if (expr is PlotExpression)
                        {
                            PlotExpression expr2 = (PlotExpression)expr;

                            List<GraphEntry> entries = new List<GraphEntry>();

                            List<Pen> pens = new List<Pen>();
                            pens.Add(ColorExpression.Blue.Pen);
                            pens.Add(ColorExpression.Red.Pen);
                            pens.Add(ColorExpression.Green.Pen);
                            pens.Add(ColorExpression.Yellow.Pen);
                            pens.Add(ColorExpression.Cyan.Pen);
                            pens.Add(ColorExpression.Magenta.Pen);

                            int i = 0;
                            foreach (Expression entry in expr2.ExpressionsToPlot)
                            {
                                entries.Add(new GraphEntry(entry, pens[i % pens.Count], expr2.Variable));
                                i++;
                            }

                            _renderItems.Add(new GraphItem(_parser, entries.ToArray()));
                        }
                        else if (expr is Plot3dExpression)
                        {
                            Plot3dExpression expr2 = (Plot3dExpression)expr;

                            _renderItems.Add(new Graph3dItem(expr2.ExpressionToPlot, expr2.WirePen, expr2.FillBrush,
                                expr2.XMin, expr2.XMax,
                                expr2.YMin, expr2.YMax,
                                expr2.ZMin, expr2.ZMax,
                                expr2.IndependentVariableX, expr2.IndependentVariableY));
                        }
                        else if (expr is MathPaintExpression)
                        {
                            MathPaintExpression expr2 = (MathPaintExpression)expr;

                            _renderItems.Add(
                                new MathPaintItem(
                                    expr2.Expression,
                                    expr2.HorizontalCoordinate,
                                    expr2.VerticalCoordinate,
                                    expr2.Width,
                                    expr2.Height));
                        }
                        //else if (expr is PlotMatrixExpression)
                        //{
                        //    throw new NotImplementedException();
                        //    //PlotMatrixExpression expr2 = (PlotMatrixExpression)expr;
                        //    //_renderItems.Add(
                        //    //    new GraphMatrixItem(
                        //    //        expr2.Matrix,
                        //    //        string.Empty));
                        //}
                        //else if (expr is PlotVectorExpression)
                        //{
                        //    throw new NotImplementedException();
                        //    //PlotVectorExpression expr2 = (PlotVectorExpression)expr;
                        //    //_renderItems.Add(new GraphVectorItem(expr2.Vector, string.Empty));
                        //}
                        else
                        {
                            if (expr is AssignExpression)
                            {
                                AssignExpression expr2 = (AssignExpression)expr;

                                _vars[expr2.Variable] = (Literal)(expr2.Value.Clone());
                            }
                            else if (expr is DelayAssignExpression)
                            {
                                expr.Eval(_vars);
                            }

                            _renderItems.Add(new ExpressionItem(expr, Pens.Blue, Font));
                        }
                    }
                }
            }

            if (_inputHistory.Count <= 0 || input != _inputHistory[_inputHistory.Count - 1])
            {
                _inputHistory.Add(input);
            }
            evalTextBox.SelectAll();
            _currentHistoryIndex = -1;
        }

        private void ClearHistory()
        {
            _inputHistory.Clear();
            _currentHistoryIndex = -1;
            _renderItems.Add(new InfoItem("History cleared", ligraControl1.Font));
        }
    }
}
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
        Dictionary<string, Command> _commands = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);

        private void InitializeCommands()
        {
            InitializeCommands(_commands);
        }
        public static void InitializeCommands(Dictionary<string, Command> commands)
        {
            //commands["help"] = new Command(Commands.HelpCommand);
            commands["clear"] = new Command(Commands.ClearCommand);
            commands["vars"] = new Command(Commands.VarsCommand);
            commands["delete"] = new Command(Commands.DeleteCommand);
            commands["history"] = new Command(Commands.HistoryCommand);
            commands["example"] = new Command(Commands.ExampleCommand);
            commands["example2"] = new Command(Commands.Example2Command);
            //commands["tsolve"] = new Command(LigraCommands.TSolveCommand);
            //commands["loadimage"] = new Command(LoadImageCommand);
            commands["cd"] = new Command(CdCommand);
        }

        static void TSolveCommand(string input, string[] args, LigraEnvironment env)
        {
            SolusEngine _engine = new SolusEngine();
            SolusMatrix m = new SolusMatrix(7, 7);

            string k;
            string r;
            string cs;

            if (!env.Variables.ContainsKey("k")) { env.Variables.Add("k", new Literal(0)); }
            if (!env.Variables.ContainsKey("R")) { env.Variables.Add("R", new Literal(0)); }
            if (!env.Variables.ContainsKey("Cs")) { env.Variables.Add("Cs", new Literal(0)); }

            k = "k";
            r = "R";
            cs = "Cs";

            int i;
            int j;

            Literal zero = new Literal(0);
            Literal one = new Literal(1);
            Literal negOne = new Literal(-1);
            VariableAccess kk = new VariableAccess("k");
            VariableAccess rr = new VariableAccess("R");
            VariableAccess cc = new VariableAccess("Cs");
            Function mult = MultiplicationOperation.Value;
            Function div = DivisionOperation.Value;
            Function add = AdditionOperation.Value;

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

            env.AddRenderItem(new ExpressionItem(m, LPen.Blue, env.Font, env));
            env.ClearCanvas();
        }

        private static void AddMultRow(SolusEngine engine, SolusMatrix m, int rowFrom, int rowTo, Expression factor)
        {
            int i;
            for (i = 0; i < 7; i++)
            {
                Expression expr;
                CleanUpTransformer cleanup = new CleanUpTransformer();
                expr = cleanup.CleanUp(new FunctionCall(MultiplicationOperation.Value, m[rowFrom, i], factor));
                expr = cleanup.CleanUp(new FunctionCall(AdditionOperation.Value, expr, m[rowTo, i]));
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
                expr = cleanup.CleanUp(new FunctionCall(AdditionOperation.Value, m[rowFrom, i], m[rowTo, i]));
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
                expr = cleanup.CleanUp(new FunctionCall(MultiplicationOperation.Value, expr, factor));
                m[row, i] = expr;
            }
        }

        public static bool IsCommand(string cmd,
            Dictionary<string, Command> availableCommands)
        {
            return availableCommands.ContainsKey(cmd);
        }

        private void LoadImageCommand(string input, string[] args, LigraEnvironment env)
        {
            var font = LFont.FromSwf(ligraControl1.Font);
            var brush = LBrush.Red;

            if (args.Length < 3)
            {
                _env.AddRenderItem(new ErrorItem(input, "Too few parameters", font, brush, env, input.IndexOf(args[0])));
            }
            else if (!_env.Variables.ContainsKey(args[1]))
            {
                _env.AddRenderItem(new ErrorItem(input, "Parameter must be a variable", font, brush, env, input.IndexOf(args[1])));
            }
            else if (!System.IO.File.Exists(args[2]))
            {
                _env.AddRenderItem(new ErrorItem(input, "Parameter must be a file name", font, brush, env, input.IndexOf(args[1])));
            }
            else
            {
                string filename = args[2];
                string varName = args[1];
                try
                {
                    SolusMatrix mat = SolusEngine.LoadImage(filename);

                    if (!_env.Variables.ContainsKey(varName))
                    {
                        _env.Variables.Add(varName, new Literal(0));
                    }

                    _env.Variables[varName] = mat;

                    _env.AddRenderItem(new InfoItem("Image loaded successfully", font, env));
                }
                catch (Exception e)
                {
                    _env.AddRenderItem(new ErrorItem(input, "There was an error while loading the file: \r\n" + filename + "\r\n" + e.ToString(), font, brush, env));
                }
            }
        }

        private void CdCommand(string input, string[] args)
        {
            CdCommand(input, args, _env);
        }
        public static void CdCommand(string input, string[] args,
            LigraEnvironment env)
        {
            if (args.Length <= 1)
            {
                //print the current directory
                string dir = System.IO.Directory.GetCurrentDirectory();
                env.AddRenderItem(new InfoItem(dir, env.Font, env));
            }
            else if (!System.IO.Directory.Exists(args[1]))
            {
                env.AddRenderItem(
                    new ErrorItem(input, "Parameter must be a folder name",
                    env.Font, LBrush.Red, env, input.IndexOf(args[1])));
            }
            else
            {
                //set the current directory
                string dir = args[1];

                try
                {
                    System.IO.Directory.SetCurrentDirectory(dir);
                    env.AddRenderItem(
                        new InfoItem(
                            "Directory changed to \"" + dir + "\"",
                            env.Font, env));
                }
                catch (Exception e)
                {
                    env.AddRenderItem(
                        new ErrorItem(
                            input, "There was an error: \r\n" + e.ToString(),
                            env.Font, LBrush.Red, env));
                }
            }
        }

        private void ProcessInput(string input)
        {
            ProcessInput(input, _env, _commands, evalTextBox.SelectAll);
        }
        public static void ProcessInput(string input, LigraEnvironment env,
            Dictionary<string, Command> availableCommands,
            Action selectAllInputText)
        {
            var args = input.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length > 0)
            {
                string cmd = args[0].Trim().ToLower();

                Command[] commands;

                if (IsCommand(cmd, availableCommands))
                {
                    commands = new Command[] { availableCommands[cmd] };
                }
                else
                {
                    commands = _parser.GetCommands(input, env);
                }

                foreach (var command in commands)
                {
                    command(input, args, env);
                }
            }

            if (env.History.Count <= 0 || input != env.History[env.History.Count - 1])
            {
                env.History.Add(input);
            }
            selectAllInputText();
            env.CurrentHistoryIndex = -1;
        }
    }
}

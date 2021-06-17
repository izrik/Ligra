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
            Commands.InitializeCommands(_commands);
        }

        public static bool IsCommand(string cmd,
            Dictionary<string, Command> availableCommands)
        {
            return availableCommands.ContainsKey(cmd);
        }
        
        private void CdCommand(string input, string[] args)
        {
            Commands.CdCommand(input, args, _env);
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
                    commands = env.Parser.GetCommands(input, env);
                }

                var label = string.Format("$ {0}", input);
                foreach (var command in commands)
                {
                    env.AddRenderItem(new TextItem(env, label, env.Font));
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

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
using MetaphysicsIndustries.Ligra.Commands;


namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraForm : Form
    {
        Dictionary<string, Command> _commands = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);

        private void InitializeCommands()
        {
            Commands.Commands.InitializeCommands(_commands);
        }

        private void CdCommand(string input, string[] args)
        {
            new CdCommand().Execute(input, args, _env);
        }

        private void ProcessInput(string input)
        {
            LigraWindow.ProcessInput(input, _env, _commands, evalTextBox.SelectAll);
        }
    }
}

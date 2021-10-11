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
        private void CdCommand(string input, string[] args, ILigraUI control)
        {
            new CdCommand().Execute(input, args, control.Env, control);
        }

        private void ProcessInput(string input, ILigraUI control)
        {
            LigraWindow.ProcessInput(input, control.Env, control.Commands,
                evalTextBox.SelectAll, control);
        }
    }
}

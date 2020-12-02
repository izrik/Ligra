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
using Gtk;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraForm : Form
    {
        public LigraForm()
        {
            InitializeComponent();

            if (Environment.OSVersion.Platform == PlatformID.Unix ||
                Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                this.Controls.Remove(this.toolStripContainer1);
                this.Controls.Add(this.splitContainer1);
            }

            _env = new LigraEnvironment(this.ligraControl1);
        }

        private static SolusEngine _engine = new SolusEngine();
        private static LigraParser _parser = new LigraParser();

        LigraEnvironment _env;

        ToolStripMenuItem _renderItemItem = new ToolStripMenuItem("Render Item");
        ToolStripMenuItem _propertiesItem = new ToolStripMenuItem("Properties");
        private void LigraForm_Load(object sender, EventArgs e)
        {
            InitializeCommands();

            evalTextBox.Focus();

            SetupContextMenu();

            if (!_env.Variables.ContainsKey("t"))
            {
                _env.Variables.Add("t", new Literal(0));
            }

            _env.Font = LFont.FromSwf(ligraControl1.Font);
            _env.ClearCanvas = ligraControl1.Invalidate;
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
                else if (ri.GetControl().Rect.Contains(pt))
                {
                    return ri;
                }
            }

            return null;
        }


        void ClearItem_Click(object sender, EventArgs e)
        {
            Commands.ClearOutput(_env);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            float time = System.Environment.TickCount / 1000.0f;
            _env.Variables["t"] = new Literal(time);
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            throw new NotImplementedException();
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
                        _env.AddRenderItem(new ErrorItem(input, ee2.Error, ligraControl1.Font, LBrush.Red, _env, ee2.Location));
                    }
                    else
                    {
                        _env.AddRenderItem(new ErrorItem(input, "There was an error: " + ee.ToString(), Font, LBrush.Red, _env));
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

    public class LigraWindow : Window
    {
        public LigraWindow()
            : base(WindowType.Toplevel)
        {
            InitializeComponent();
            LigraForm.InitializeCommands(availableCommands);
            env = new LigraEnvironment(this.output);
            env.Font = new LFont(LFont.Families.CourierNew, 12,
                LFont.Styles.Regular);
        }

        LigraEnvironment env;
        Dictionary<string, Command> availableCommands =
            new Dictionary<string, Command>(
                StringComparer.InvariantCultureIgnoreCase);

        object font = null;

        Gtk.Button evalButton;
        Gtk.Entry input;
        LigraWidget output;

        void InitializeComponent()
        {
            Title = "Ligra";

            var vbox = new VBox(false, 1);
            this.Add(vbox);

            output = new LigraWidget();
            output.SetSizeRequest(400, 314);
            vbox.PackStart(output, true, true, 0);

            var hbox = new HBox(false, 1);
            vbox.PackEnd(hbox, false, false, 0);
            input = new Entry();
            input.SetSizeRequest(308, 25);
            input.Activated += (o, e) => EvaluateInput();
            hbox.PackStart(input, true, true, 0);

            evalButton = new Gtk.Button("Eval");
            evalButton.SetSizeRequest(75, 23);
            evalButton.Clicked += (o, e) => EvaluateInput();
            hbox.PackEnd(evalButton, false, false, 0);
        }

        void EvaluateInput()
        {
            var s = (input.Text ?? string.Empty).Trim();

            if (s.Length > 0)
            {
                try
                {
                    LigraForm.ProcessInput(s, env, availableCommands,
                        () => input.SelectRegion(0, input.Text.Length));
                }
                catch (SolusParseException e)
                {
                    env.AddRenderItem(new ErrorItem(s, e.Error, null, LBrush.Red, env, e.Location));
                }
                catch (Exception e)
                {
                    env.AddRenderItem(new ErrorItem(s, "There was an error: " + e.ToString(), null, LBrush.Red, env));
                }
            }

            // Invalidate(); ??
        }
    }
}

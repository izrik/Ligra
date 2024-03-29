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
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus.Expressions;

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
        }

        private static SolusEngine _engine = new SolusEngine();

        ToolStripMenuItem _renderItemItem = new ToolStripMenuItem("Render Item");
        ToolStripMenuItem _propertiesItem = new ToolStripMenuItem("Properties");
        private void LigraForm_Load(object sender, EventArgs e)
        {
            evalTextBox.Focus();

            SetupContextMenu();

            if (!ligraControl1.Env.ContainsVariable("t"))
            {
                ligraControl1.Env.SetVariable("t", new Literal(0));
            }
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

                var menuItems0 = ri.GetMenuItems();
                var menuItems = new ToolStripMenuItem[menuItems0.Length];
                int i;

                ToolStripMenuItem ToSwf(LMenuItem mi)
                {
                    var item = new ToolStripMenuItem(mi.Text);
                    if (mi.Clicked != null)
                        item.Click += (o, _e) => mi.Clicked();
                    foreach (var child in mi.Children)
                        item.DropDownItems.Add(ToSwf(child));
                    item.Enabled = mi.Enabled;
                    return item;
                }

                for (i = 0; i < menuItems0.Length; i++)
                {
                    var mi = menuItems0[i];
                    menuItems[i] = ToSwf(mi);
                }

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
            return GetRenderItemInCollectionFromPoint(ligraControl1.RenderItems, pt);
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
            Commands.Command.ClearOutput(ligraControl1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            float time = System.Environment.TickCount / 1000.0f;
            ligraControl1.Env.SetVariable("t", new Literal(time));
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
                    ProcessInput(input, ligraControl1);
                }
                catch (Exception ee)
                {
                    var font = LFont.FromSwf(ligraControl1.Font);
                    if (ee is Solus.Exceptions.ParseException ee2)
                    {
                        ligraControl1.AddRenderItem(
                            new ErrorItem(input, ee2.Error, font, LBrush.Red,
                                ee2.Location));
                    }
                    else
                    {
                        ligraControl1.AddRenderItem(
                            new ErrorItem(input,
                                "There was an error: " + ee.ToString(), font,
                                LBrush.Red));
                    }
                }
            }
            
            Invalidate();
        }

        private void evalTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (ligraControl1.History.Count > 0)
                {
                    if (ligraControl1.CurrentHistoryIndex < 0)
                    {
                        ligraControl1.CurrentHistoryIndex = 
                            ligraControl1.History.Count - 1;
                    }
                    else
                    {
                        ligraControl1.CurrentHistoryIndex--;
                        if (ligraControl1.CurrentHistoryIndex < 0)
                            ligraControl1.CurrentHistoryIndex = 0;
                    }

                    evalTextBox.Text =
                        ligraControl1.History[
                            ligraControl1.CurrentHistoryIndex];
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (ligraControl1.History.Count > 0)
                {
                    if (ligraControl1.CurrentHistoryIndex < 0)
                    {
                        ligraControl1.CurrentHistoryIndex =
                            ligraControl1.History.Count - 1;
                    }
                    else
                    {
                        ligraControl1.CurrentHistoryIndex++;
                        if (ligraControl1.CurrentHistoryIndex >=
                            ligraControl1.History.Count)
                            ligraControl1.CurrentHistoryIndex =
                                ligraControl1.History.Count - 1;
                    }

                    evalTextBox.Text =
                        ligraControl1.History[
                            ligraControl1.CurrentHistoryIndex];
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

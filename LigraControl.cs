/*****************************************************************************
 *                                                                           *
 *  LigraControl.cs                                                          *
 *  28 November 2007                                                         *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2008 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  The control that does most of the rendering in Ligra.                    *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraControl : UserControl, ILigraUI
    {
        public LigraControl()
        {
            InitializeComponent();
        }

        static readonly SolusEngine _engine = new SolusEngine();

        private bool _drawBoxes = false;
        public bool DrawBoxes
        {
            get { return _drawBoxes; }
            set { _drawBoxes = value; }
        }


        private void drawBoxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawBoxes = !DrawBoxes;
            drawBoxesToolStripMenuItem.Checked = DrawBoxes;
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Down ||
                keyData == Keys.Up ||
                keyData == Keys.PageDown ||
                keyData == Keys.PageUp ||
                keyData == Keys.Home)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
//                Point pt = AutoScrollPosition;
//                AutoScrollPosition = new Point(-pt.X, -pt.Y + 20);
            }                                              
            else if (e.KeyCode == Keys.Up)                 
            {                                              
//                Point pt = AutoScrollPosition;             
//                AutoScrollPosition = new Point(-pt.X, -pt.Y - 20);
            }                                              
            else if (e.KeyCode == Keys.PageDown)           
            {                                              
//                Point pt = AutoScrollPosition;             
//                AutoScrollPosition = new Point(-pt.X, -pt.Y + ClientSize.Height);
            }                                              
            else if (e.KeyCode == Keys.PageUp)             
            {                                              
//                Point pt = AutoScrollPosition;
//                AutoScrollPosition = new Point(-pt.X, -pt.Y - ClientSize.Height);
            }
            else if (e.KeyCode == Keys.Home)
            {
//                AutoScrollPosition = new Point(0, 0);
            }

            base.OnKeyDown(e);
        }

        public readonly List<RenderItem> _items = new List<RenderItem>();
        public IList<RenderItem> RenderItems => _items;

        public void AddRenderItem(RenderItem item)
        {
            _items.Add(item);
            this.flowLayoutPanel1.Controls.Add(item.GetControl());
        }

        public void RemoveRenderItem(RenderItem item)
        {
            _items.Remove(item);
            this.flowLayoutPanel1.Controls.Remove(item.GetControl());
        }

        Vector2 ILigraUI.ClientSize => this.ClientSize.ToVector2();

        public void OpenPlotProperties(GraphItem item)
        {
            PlotPropertiesForm form = new PlotPropertiesForm(item._parser);
            var graphUI = item.GetControl();

            form.PlotSize = graphUI.Rect.Size;
            form.PlotMaxX = item._maxX;
            form.PlotMinX = item._minX;
            form.PlotMaxY = item._maxY;
            form.PlotMinY = item._minY;

            form.SetExpressions(
                Array.ConvertAll(item._entries.ToArray(),
                    item.ExpressionFromGraphEntry));

            if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                graphUI.Rect = new RectangleF(graphUI.Rect.Location,
                    form.PlotSize);

                item._maxX = form.PlotMaxX;
                item._minX = form.PlotMinX;
                item._maxY = form.PlotMaxY;
                item._minY = form.PlotMinY;
            }
        }
    }
}

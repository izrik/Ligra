using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using Gtk;

namespace MetaphysicsIndustries.Ligra
{
    public class RenderItemContainer : RenderItem
    {
        public RenderItemContainer(string caption, LigraEnvironment env)
            : base(env)
        {
            _caption = caption;
        }

        public readonly string _caption;

        private List<RenderItem> _items = new List<RenderItem>();
        public List<RenderItem> Items
        {
            get { return _items; }
        }

        protected override Widget GetAdapterInternal()
        {
            throw new NotImplementedException();
        }

        protected override RenderItemControl GetControlInternal()
        {
            return new RenderItemContainerControl(this);
        }
    }

    public class RenderItemContainerControl : RenderItemControl
    {
        public RenderItemContainerControl(RenderItemContainer owner)
            : base(owner)
        {
        }

        public new RenderItemContainer _owner =>
            (RenderItemContainer)base._owner;

        string _caption => _owner._caption;
        List<RenderItem> Items => _owner.Items;

        protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            Font font2 = new Font(this.Font.FontFamily, this.Font.Size * 2, FontStyle.Bold);

            float width = this.Parent.ClientSize.Width - 20;
            float height = g.MeasureString(_caption, font2, (int)width).Height;

            g.DrawString(_caption, font2, Brushes.Black, new PointF(2, 2));
            g.DrawRectangle(Pens.Black, 0, 0, this.Width, this.Height- 250);

            float x = 20;
            List<float> currentHeights = new List<float>();
            float maxCurrentHeight = 0;
            foreach (RenderItem ri in Items)
            {
                var ric = ri.GetControl();
                var size = ric.Size;
                if (x + size.Width > width)
                {
                    height += maxCurrentHeight;
                    x = 20;
                    maxCurrentHeight = 0;
                }

                ric.Refresh();

                x += size.Width + 10;
                maxCurrentHeight = Math.Max(maxCurrentHeight, size.Height);
            }
            height += maxCurrentHeight;

//            return new SizeF(width, height);
        }

        protected override Vector2 InternalCalcSize(Graphics g)
        {
            Font font2 = new Font(this.Font.FontFamily, this.Font.Size * 2);

            float width = this.Parent.ClientSize.Width - 20;
            float height = g.MeasureString(_caption, font2, (int)width).Height;

            float x = 20;
            List<float> currentHeights = new List<float>();
            float maxCurrentHeight = 0;
            foreach (RenderItem ri in Items)
            {
                var ric = ri.GetControl();
                SizeF size = ric.Size;
                if (x + size.Width > width)
                {
                    height += maxCurrentHeight;
                    x = 20;
                    maxCurrentHeight = 0;
                }
                x += size.Width + 10;
                maxCurrentHeight = Math.Max(maxCurrentHeight, size.Height);
            }
            height += maxCurrentHeight;

            height += 260;

            return new Vector2(width, height);
        }
    }
}

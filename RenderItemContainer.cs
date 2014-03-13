using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class RenderItemContainer : RenderItem
    {
        public RenderItemContainer(string caption, LigraEnvironment env)
            : base(env)
        {
            _caption = caption;
        }

        string _caption;

            protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            Font font2 = new Font(this.Font.FontFamily, this.Font.Size * 2, FontStyle.Bold);

            float width = this.Parent.ClientSize.Width - 20;
            float height = g.MeasureString(_caption, font2, (int)width).Height;

            g.DrawString(_caption, font2, Brushes.Black, new PointF(2, 2));
            SizeF size = CalcSize(g);
            g.DrawRectangle(Pens.Black, 0, 0, size.Width, size.Height- 250);

            float x = 20;
            List<float> currentHeights = new List<float>();
            float maxCurrentHeight = 0;
            foreach (RenderItem ri in Items)
            {
                size = ri.CalcSize(g);
                if (x + size.Width > width)
                {
                    height += maxCurrentHeight;
                    x = 20;
                    maxCurrentHeight = 0;
                }

                ri.Refresh();

                x += size.Width + 10;
                maxCurrentHeight = Math.Max(maxCurrentHeight, size.Height);
            }
            height += maxCurrentHeight;

//            return new SizeF(width, height);
        }

        private List<RenderItem> _items = new List<RenderItem>();
        public List<RenderItem> Items
        {
            get { return _items; }
        }


        protected override SizeF InternalCalcSize(Graphics g)
        {
            Font font2 = new Font(this.Font.FontFamily, this.Font.Size * 2);

            float width = this.Parent.ClientSize.Width - 20;
            float height = g.MeasureString(_caption, font2, (int)width).Height;

            float x = 20;
            List<float> currentHeights = new List<float>();
            float maxCurrentHeight = 0;
            foreach (RenderItem ri in Items)
            {
                SizeF size = ri.CalcSize(g);
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

            return new SizeF(width, height);
        }
    }
}

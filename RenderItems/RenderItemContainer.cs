using System;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class RenderItemContainer : RenderItem
    {
        public RenderItemContainer(string caption)
        {
            _caption = caption;
        }

        public readonly string _caption;

        protected override void InternalRender(IRenderer g,
            DrawSettings drawSettings)
        {
            var font2 = new LFont(
                drawSettings.Font.Family,
                drawSettings.Font.Size * 2,
                LFont.Styles.Bold);

            float width = this.Container.ClientSize.X - 20;
            float height = g.MeasureString(_caption, font2, (int)width).Y;

            g.DrawString(_caption, font2, LBrush.Black, new Vector2(2, 2));
            var size1 = InternalCalcSize(g, drawSettings);
            g.DrawRectangle(LPen.Black, 0, 0, size1.X, size1.Y - 250);

            float x = 20;
            List<float> currentHeights = new List<float>();
            float maxCurrentHeight = 0;
            foreach (RenderItem ri in Items)
            {
                var size = ri.CalculateSize(g, drawSettings);
                if (x + size.X > width)
                {
                    height += maxCurrentHeight;
                    x = 20;
                    maxCurrentHeight = 0;
                }

                ri.Invalidate();

                x += size.X + 10;
                maxCurrentHeight = Math.Max(maxCurrentHeight, size.Y);
            }
            height += maxCurrentHeight;

            //return new SizeF(width, height);
        }

        private List<RenderItem> _items = new List<RenderItem>();
        public List<RenderItem> Items
        {
            get { return _items; }
        }

        protected override Vector2 InternalCalcSize(IRenderer g,
            DrawSettings drawSettings)
        {
            var font2 = new LFont(
                drawSettings.Font.Family,
                drawSettings.Font.Size * 2,
                LFont.Styles.Bold);

            float width = this.Container.ClientSize.X - 20;
            float height = g.MeasureString(_caption, font2, (int)width).Y;

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

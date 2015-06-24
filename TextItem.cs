using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class TextItem : RenderItem
    {
        public TextItem(LigraEnvironment env, string text="", Font font=null)
            : base(env)
        {
            Text = text;
            if (font != null)
            {
                Font = font;
            }
        }

        public virtual StringFormat Format
        {
            get
            {
                return null;
            }
        }

            protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            RectangleF rect = new RectangleF(new PointF(0, 0), InternalCalcSize(g));

            StringFormat fmt = Format;
            if (fmt == null)
            {
                g.DrawString(Text, this.Font, Brushes.Black, rect);
            }
            else
            {
                g.DrawString(Text, this.Font, Brushes.Black, rect, fmt);
            }
        }

        protected override SizeF InternalCalcSize(Graphics g)
        {
            return g.MeasureString(Text, Font, this.Parent.ClientSize.Width - 25);
        }

        public override bool HasChanged(SolusEnvironment env)
        {
            return false;
        }
    }
}

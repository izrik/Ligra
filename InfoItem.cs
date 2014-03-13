using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class InfoItem : RenderItem
    {
        public InfoItem(string text, Font font, LigraEnvironment env)
            : base(env)
        {
            _text = text;
            _font = font;
        }

        string _text;
        Font _font;


        protected override void InternalRender(LigraControl control, Graphics g, SolusEnvironment env)
        {
            g.DrawString(_text, _font, Brushes.Black, new PointF(0, 0));
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return g.MeasureString(_text, _font);
        }
    }
}

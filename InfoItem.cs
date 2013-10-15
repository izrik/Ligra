using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using Environment = MetaphysicsIndustries.Solus.Environment;

namespace MetaphysicsIndustries.Ligra
{
    public class InfoItem : RenderItem
    {
        public InfoItem(string text, Font font)
        {
            _text = text;
            _font = font;
        }

        string _text;
        Font _font;


        protected override void InternalRender(LigraControl control, Graphics g, PointF location, Environment env)
        {
            g.DrawString(_text, _font, Brushes.Black, location);
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return g.MeasureString(_text, _font);
        }
    }
}

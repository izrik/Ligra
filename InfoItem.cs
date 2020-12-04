using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using Gtk;

namespace MetaphysicsIndustries.Ligra
{
    public class InfoItem : RenderItem
    {
        public InfoItem(string text, LFont font, LigraEnvironment env)
            : base(env)
        {
            _text = text;
            _font = font;
        }

        public string _text;
        public LFont _font;

        public override void InternalRender2(IRenderer g, SolusEnvironment env)
        {
            g.DrawString(_text, _font, LBrush.Black, new Vector2(0, 0));
        }

        public override Vector2 InternalCalcSize2(IRenderer g)
        {
            return g.MeasureString(_text, _font);
        }
    }
}

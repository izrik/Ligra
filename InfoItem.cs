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
        public InfoItem(string text, object font, LigraEnvironment env)
            : base(env)
        {
            _text = text;
            _font = font;
        }

        public string _text;
        public object _font;

        protected override Widget GetAdapterInternal()
        {
            throw new NotImplementedException();
        }

        protected override RenderItemControl GetControlInternal()
        {
            return new InfoItemControl(this);
        }
    }

    public class InfoItemControl : RenderItemControl
    {
        public InfoItemControl(InfoItem owner)
            : base(owner)
        {
        }

        public new InfoItem _owner => (InfoItem)base._owner;

        string _text => _owner._text;
        Font _font => (Font)_owner._font;

        public override void InternalRender(IRenderer g, SolusEnvironment env)
        {
            g.DrawString(_text, LFont.FromSwf(_font), new LBrush(LColor.FromSwf(Color.Black)), new Vector2(0, 0));
        }

        public override Vector2 InternalCalcSize(IRenderer g)
        {
            return g.MeasureString(_text, LFont.FromSwf(_font));
        }
    }
}

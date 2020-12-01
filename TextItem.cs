using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using Gtk;

namespace MetaphysicsIndustries.Ligra
{
    public class TextItem : RenderItem
    {
        public TextItem(LigraEnvironment env, string text = "", object font = null)
            : base(env)
        {
            _text = text;
            _font = font;
        }

        public readonly string _text;
        public readonly object _font;

        public override bool HasChanged(SolusEnvironment env)
        {
            return false;
        }

        protected override Widget GetAdapterInternal()
        {
            throw new NotImplementedException();
        }

        protected override RenderItemControl GetControlInternal()
        {
            return new TextItemControl(this);
        }
    }

    public class TextItemControl : RenderItemControl
    {
        public TextItemControl(TextItem owner)
            : base(owner)
        {
        }

        public new TextItem _owner => (TextItem)base._owner;

        string _text => _owner._text;
        Font _font => (Font)_owner._font;

        public virtual StringFormat Format
        {
            get
            {
                return null;
            }
        }

        public override void InternalRender(IRenderer g, SolusEnvironment env)
        {
            RectangleF rect = new RectangleF(new PointF(0, 0),
                InternalCalcSize(g));

            StringFormat fmt = Format;
            var font = LFont.FromSwf(this.Font);
            var black = LBrush.Black;
            if (fmt == null)
            {
                g.DrawString(Text, font, black, rect);
            }
            else
            {
                g.DrawString(Text, font, black, rect, fmt);
            }
        }

        public override Vector2 InternalCalcSize(IRenderer g)
        {
            return g.MeasureString(Text, LFont.FromSwf(Font),
                this.Parent.ClientSize.Width - 25);
        }
    }
}

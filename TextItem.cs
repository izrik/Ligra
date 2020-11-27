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

        protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            RectangleF rect = new RectangleF(new PointF(0, 0),
                InternalCalcSize(g));

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
            return g.MeasureString(Text, Font,
                this.Parent.ClientSize.Width - 25);
        }
    }
}

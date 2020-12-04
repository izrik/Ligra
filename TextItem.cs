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
        public TextItem(LigraEnvironment env, string text = "", LFont font = null)
            : base(env)
        {
            _text = text;
            _font = font;
        }

        public readonly string _text;
        public readonly LFont _font;

        public override bool HasChanged(SolusEnvironment env)
        {
            return false;
        }

        protected override Widget GetAdapterInternal()
        {
            return new TextItemWidget(this);
        }

        protected override RenderItemControl GetControlInternal()
        {
            return new TextItemControl(this);
        }

        public override void InternalRender2(IRenderer g, SolusEnvironment env)
        {
            RectangleF rect = new RectangleF(new PointF(0, 0),
                InternalCalcSize(g));

            StringFormat fmt = Format;
            if (fmt == null)
            {
                g.DrawString(_text, _font, LBrush.Black, rect);
            }
            else
            {
                g.DrawString(_text, _font, LBrush.Black, rect, fmt);
            }
        }

        public override Vector2 InternalCalcSize2(IRenderer g)
        {
            return g.MeasureString(_text, _font, Container.ClientSize.X - 25);
        }

        public virtual StringFormat Format
        {
            get
            {
                return null;
            }
        }
    }

    public class TextItemControl : RenderItemControl
    {
        public TextItemControl(TextItem owner)
            : base(owner)
        {
        }
    }

    public class TextItemWidget : RenderItemWidget
    {
        public TextItemWidget(TextItem owner)
            : base(owner)
        {
        }
    }
}

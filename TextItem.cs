using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class TextItem : RenderItem
    {
        public TextItem()
        {
        }

        public TextItem(string text)
            : this()
        {
            _text = text;
        }

        public TextItem(string text, Font font)
            : this(text)
        {
            _font = font;
        }

        public virtual string Text 
        {
            get
            {
                return _text;
            }
        }

        private Font _font;// = new Font("Courier New", 12);

        public Font Font
        {
            get { return _font; }
            set { _font = value; }
        }


        string _text = string.Empty;

        public virtual StringFormat Format
        {
            get
            {
                return null;
            }
        }

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, Dictionary<string, Expression> varTable)
        {
            RectangleF rect = new RectangleF(location, InternalCalcSize(control, g));

            Font font2 = GetFont(control);

            StringFormat fmt = Format;
            if (fmt == null)
            {
                g.DrawString(Text, font2, Brushes.Black, rect);
            }
            else
            {
                g.DrawString(Text, font2, Brushes.Black, rect, fmt);
            }
        }

        public virtual Font GetFont(LigraControl control)
        {
            if (_font != null) { return _font; }

            Font font2 = new Font(control.Font.FontFamily, control.Font.Size * 2, FontStyle.Bold);
            return font2;
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            if (_font != null)
            {
                return g.MeasureString(Text, Font, control.ClientSize.Width - 25);
            }

            return control.ClientSize - new SizeF(25, 25);
        }

        public override bool HasChanged(Dictionary<string, Expression> varTable)
        {
            return false;
        }
    }
}

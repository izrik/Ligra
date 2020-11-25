using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using Gtk;

namespace MetaphysicsIndustries.Ligra
{
    public class ErrorItem : RenderItem
    {
        public ErrorItem(string inputText, string errorText, Font font, Brush brush, LigraEnvironment env, int location=-1)
            : base(env)
        {
            _errorText = errorText;
            _inputText = inputText;
            _font = font;
            _brush = brush;
            _location = location;
        }

        string _errorText;
        string _inputText;
        Font _font;
        Brush _brush;
        int _location;

            protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            float y = 0;
            if (!string.IsNullOrEmpty(_inputText))
            {
                g.DrawString(_inputText, _font, _brush, new PointF(0, y));

                if (_location >= 0)
                {
                    string prefix = _inputText.Substring(0, _location + 1);
                    string first = _inputText.Substring(_location, 1);
                    float prefixWidth = g.MeasureString(prefix, _font).Width;
                    float firstWidth = g.MeasureString(first, _font).Width;

                    float xx = prefixWidth - firstWidth;

                    g.DrawString("_", _font, _brush, xx, y + 2);
                }

                y += g.MeasureString(_inputText, _font).Height + 10;
            }

            g.DrawString(_errorText, _font, _brush, new PointF(0, y));
        }

        protected override SizeF InternalCalcSize(Graphics g)
        {
            float y = 0;

            if (!string.IsNullOrEmpty(_inputText))
            {
                y += g.MeasureString(_inputText, _font).Height + 10;
            }

            return g.MeasureString(_errorText, _font) + new SizeF(0, y);
        }

        protected override Widget GetAdapterInternal()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class ErrorItem : RenderItem
    {
        public ErrorItem(string inputText, string errorText, Font font, Brush brush)
            : this(inputText, errorText, font, brush, -1)
        {
        }
        public ErrorItem(string inputText, string errorText, Font font, Brush brush, int location)
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

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, SolusEnvironment env)
        {
            if (!string.IsNullOrEmpty(_inputText))
            {
                g.DrawString(_inputText, _font, _brush, location);

                if (_location >= 0)
                {
                    string prefix = _inputText.Substring(0, _location + 1);
                    string first = _inputText.Substring(_location, 1);
                    float prefixWidth = g.MeasureString(prefix, _font).Width;
                    float firstWidth = g.MeasureString(first, _font).Width;

                    float xx = prefixWidth - firstWidth;

                    g.DrawString("_", _font, _brush, location.X + xx, location.Y + 2);
                }

                location.Y += g.MeasureString(_inputText, _font).Height + 10;
            }

            g.DrawString(_errorText, _font, _brush, location);
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            float y = 0;

            if (!string.IsNullOrEmpty(_inputText))
            {
                y += g.MeasureString(_inputText, _font).Height + 10;
            }

            return g.MeasureString(_errorText, _font) + new SizeF(0, y);
        }
    }
}

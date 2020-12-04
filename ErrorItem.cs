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
        public ErrorItem(string inputText, string errorText, LFont font,
            LBrush brush, LigraEnvironment env, int location = -1)
            : base(env)
        {
            _errorText = errorText;
            _inputText = inputText;
            _font = font;
            _brush = brush;
            _location = location;
        }

        public string _errorText;
        public string _inputText;
        public LFont _font;
        public LBrush _brush;
        public int _location;

        protected override Widget GetAdapterInternal()
        {
            return new RenderItemWidget(this);
        }

        protected override RenderItemControl GetControlInternal()
        {
            return new RenderItemControl(this);
        }

        public override void InternalRender2(IRenderer g, SolusEnvironment env)
        {
            var font = _font;

            float y = 0;
            if (!string.IsNullOrEmpty(_inputText))
            {
                g.DrawString(_inputText, font, _brush, new Vector2(0, y));

                if (_location >= 0)
                {
                    string prefix = _inputText.Substring(0, _location + 1);
                    string first = _inputText.Substring(_location, 1);
                    float prefixWidth = g.MeasureString(prefix, font).X;
                    float firstWidth = g.MeasureString(first, font).X;

                    float xx = prefixWidth - firstWidth;

                    g.DrawString("_", font, _brush, xx, y + 2);
                }

                y += g.MeasureString(_inputText, font).Y + 10;
            }

            g.DrawString(_errorText, font, _brush, new Vector2(0, y));
        }

        public override Vector2 InternalCalcSize2(IRenderer g)
        {
            float y = 0;
            var font = _font;

            if (!string.IsNullOrEmpty(_inputText))
            {
                y += g.MeasureString(_inputText, font).Y + 10;
            }

            return g.MeasureString(_errorText, font) + new Vector2(0, y);
        }
    }
}

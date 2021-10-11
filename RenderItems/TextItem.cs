using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class TextItem : RenderItem
    {
        public TextItem(LigraEnvironment env, string text="", LFont font=null)
            : base(env)
        {
            _text = text;
            _font = font;
        }

        public readonly string _text;
        public readonly LFont _font;

        public virtual StringFormat Format
        {
            get
            {
                return null;
            }
        }

        protected override void InternalRender(IRenderer g,
            SolusEnvironment env, DrawSettings drawSettings)
        {
            RectangleF rect = new RectangleF(new PointF(0, 0),
                InternalCalcSize(g, drawSettings));

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

        protected override Vector2 InternalCalcSize(IRenderer g,
            DrawSettings drawSettings)
        {
            return g.MeasureString(_text, _font, Container.ClientSize.X - 25);
        }

        public override bool HasChanged(SolusEnvironment env)
        {
            return false;
        }
    }
}

using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class HelpItem : RenderItem
    {
        public HelpItem(LFont font, string text)
        {
            _font = font;
            _text = text;
        }

        public readonly string _text;
        public readonly LFont _font;

        protected override void InternalRender(IRenderer g,
            DrawSettings drawSettings)
        {
            g.DrawString(_text, _font, LBrush.Magenta, new Vector2(0, 0));
        }

        protected override Vector2 InternalCalcSize(IRenderer g,
            DrawSettings drawSettings)
        {
            return g.MeasureString(_text, _font);//, 500);
        }

    }
}

using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class HelpItem : RenderItem
    {
        public HelpItem(LFont font, LigraEnvironment env, string text)
            : base(env)
        {
            _font = font;
            _text = text;
        }

        public readonly string _text;
        public readonly LFont _font;

        protected override void InternalRender(IRenderer g, SolusEnvironment env)
        {
            g.DrawString(_text, _font, LBrush.Magenta, new Vector2(0, 0));
        }

        protected override Vector2 InternalCalcSize(IRenderer g)
        {
            return g.MeasureString(_text, _font);//, 500);
        }

    }
}

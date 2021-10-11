using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class InfoItem : RenderItem
    {
        public InfoItem(string text, LFont font, LigraEnvironment env)
            : base(env)
        {
            _text = text;
            _font = font;
        }

        public string _text;
        public LFont _font;

        protected override void InternalRender(IRenderer g,
            SolusEnvironment env, DrawSettings drawSettings)
        {
            g.DrawString(_text, _font, LBrush.Black, new Vector2(0, 0));
        }

        protected override Vector2 InternalCalcSize(IRenderer g,
            DrawSettings drawSettings)
        {
            return g.MeasureString(_text, _font);
        }
    }
}

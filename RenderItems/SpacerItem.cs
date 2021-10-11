using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public class SpacerItem : RenderItem
    {
        public SpacerItem(Vector2 size)
        {
            _size = size;
        }

        public SpacerItem(float width, float height)
            : this(new Vector2(width, height))
        {
        }

        public readonly Vector2 _size;

        protected override void InternalRender(IRenderer g,
            DrawSettings drawSettings)
        {
        }

        protected override Vector2 InternalCalcSize(IRenderer g,
            DrawSettings drawSettings)
        {
            return _size;
        }
    }
}

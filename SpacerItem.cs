using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class SpacerItem : RenderItem
    {
        public SpacerItem(Vector2 size, LigraEnvironment env)
            : base(env)
        {
            _size = size;
        }

        public SpacerItem(float width, float height, LigraEnvironment env)
            : this(new Vector2(width, height), env)
        {
        }

        public readonly Vector2 _size;

        protected override void InternalRender(IRenderer g, SolusEnvironment env)
        {
        }

        protected override Vector2 InternalCalcSize(IRenderer g)
        {
            return _size;
        }
    }
}

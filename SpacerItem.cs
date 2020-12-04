using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using Gtk;

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
        }

        public override Vector2 InternalCalcSize2(IRenderer g)
        {
            return _size;
        }
    }
}

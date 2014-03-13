using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class SpacerItem : RenderItem
    {
        public SpacerItem(SizeF size, LigraEnvironment env)
            : base(env)
        {
            _size = size;
        }

        public SpacerItem(float width, float height, LigraEnvironment env)
            : this(new SizeF(width, height), env)
        {
        }

        readonly SizeF _size;

        protected override void InternalRender(System.Drawing.Graphics g, SolusEnvironment env)
        {
        }

        protected override System.Drawing.SizeF InternalCalcSize(System.Drawing.Graphics g)
        {
            return _size;
        }
    }
}

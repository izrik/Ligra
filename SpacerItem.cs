using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using Environment = MetaphysicsIndustries.Solus.Environment;

namespace MetaphysicsIndustries.Ligra
{
    public class SpacerItem : RenderItem
    {
        public SpacerItem(SizeF size)
        {
            _size = size;
        }

        public SpacerItem(float width, float height)
            : this(new SizeF(width, height))
        {
        }

        SizeF _size;

        protected override void InternalRender(LigraControl control, System.Drawing.Graphics g, System.Drawing.PointF location, Environment env)
        {
        }

        protected override System.Drawing.SizeF InternalCalcSize(LigraControl control, System.Drawing.Graphics g)
        {
            return _size;
        }
    }
}

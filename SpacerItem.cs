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
        public SpacerItem(SizeF size, LigraEnvironment env)
            : base(env)
        {
            _size = size;
        }

        public SpacerItem(float width, float height, LigraEnvironment env)
            : this(new SizeF(width, height), env)
        {
        }

        public readonly SizeF _size;

        protected override Widget GetAdapterInternal()
        {
            throw new NotImplementedException();
        }

        protected override RenderItemControl GetControlInternal()
        {
            return new SpacerItemControl(this);
        }
    }

    public class SpacerItemControl : RenderItemControl
    {
        public SpacerItemControl(SpacerItem owner)
            : base(owner)
        {
        }

        public new SpacerItem _owner => (SpacerItem)base._owner;

        public SizeF _size => _owner._size;

        protected override void InternalRender(Graphics g,
            SolusEnvironment env)
        {
        }

        protected override SizeF InternalCalcSize(Graphics g)
        {
            return _size;
        }
    }
}

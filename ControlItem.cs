using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class ControlItem : RenderItem
    {
        public ControlItem(LigraFormsControl control, LigraControl parent, LigraEnvironment env)
            : base(env)
        {
            if (control == null) { throw new ArgumentNullException("control"); }
            if (parent == null) { throw new ArgumentNullException("parent"); }

            _control = control;
            _parent = parent;

            _parent.SuspendLayout();
            _control.Parent = _parent;
            _parent.ResumeLayout();
            _parent.PerformLayout();
        }

        LigraFormsControl _control;
        LigraControl _parent;

        protected override void InternalRender(Graphics g, SolusEnvironment env)
        {
            g.DrawRectangle(Pens.Black, 0, 0, this.Width, this.Height);
        }

        protected override SizeF InternalCalcSize(Graphics g)
        {
            return _control.Size;
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            _control.Location = Location;//Point.Round(Location) + new Size(_parent.AutoScrollPosition);

            base.OnLocationChanged(e);
        }
    }
}

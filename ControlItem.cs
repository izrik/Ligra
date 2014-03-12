using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class ControlItem : RenderItem
    {
        public ControlItem(LigraFormsControl control, LigraControl parent)
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

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, SolusEnvironment env)
        {
            SizeF size = CalcSize(control, g);
            g.DrawRectangle(Pens.Black, location.X, location.Y, size.Width, size.Height);
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
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

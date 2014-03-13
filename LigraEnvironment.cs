using System;
using MetaphysicsIndustries.Solus;
using System.Collections.Generic;
using System.Drawing;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraEnvironment : SolusEnvironment
    {
        public LigraEnvironment(LigraControl control)
        {
            if (control == null) throw new ArgumentNullException("control");

            Control = control;
        }

        public readonly LigraControl Control;

        public readonly List<RenderItem> RenderItems = new List<RenderItem>();
        public void AddRenderItem(RenderItem item)
        {
            RenderItems.Add(item);
            Control.AddRenderItem(item);
        }

        public readonly List<string> History = new List<string>();
        public int CurrentHistoryIndex = -1;

        public Font Font;
        public Action ClearCanvas;
    }
}


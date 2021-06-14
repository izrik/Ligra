using System;
using MetaphysicsIndustries.Solus;
using System.Collections.Generic;
using System.Drawing;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraEnvironment : SolusEnvironment
    {
        public LigraEnvironment(ILigraUI control)
        {
            if (control == null) throw new ArgumentNullException("control");

            Control = control;
        }

        public readonly ILigraUI Control;

        public IList<RenderItem> RenderItems => Control.RenderItems;
        public void AddRenderItem(RenderItem item)
        {
            Control.AddRenderItem(item);
        }

        public readonly List<string> History = new List<string>();
        public int CurrentHistoryIndex = -1;

        public LFont Font;
        public Action ClearCanvas;

        public readonly LigraParser Parser = new LigraParser();
    }
}


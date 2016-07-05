using System;
using MetaphysicsIndustries.Solus;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraEnvironment : SolusEnvironment
    {
        public LigraEnvironment(LigraControl control)
        {
            if (control == null) throw new ArgumentNullException("control");

            Control = control;

            RenderItems = new ReadOnlyList<RenderItem>(_renderItems);
        }

        public readonly LigraControl Control;

        readonly List<RenderItem> _renderItems = new List<RenderItem>();
        public readonly IReadOnlyList<RenderItem> RenderItems;
        public void AddRenderItem(RenderItem item)
        {
            _renderItems.Add(item);
            Control.AddRenderItem(item);
        }
        public void RemoveRenderItem(RenderItem item)
        {
            _renderItems.Remove(item);
            Control.RemoveRenderItem(item);
        }
        public void ClearRenderItems()
        {
            foreach (var ri in RenderItems.ToArray())
            {
                RemoveRenderItem(ri);
            }
            _renderItems.Clear();
        }

        public readonly List<string> History = new List<string>();
        public int CurrentHistoryIndex = -1;

        public Font Font;
        public Action ClearCanvas;
    }
}


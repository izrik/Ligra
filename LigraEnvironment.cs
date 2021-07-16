using System;
using MetaphysicsIndustries.Solus;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Ligra.Commands;
using MetaphysicsIndustries.Ligra.RenderItems;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraEnvironment : SolusEnvironment
    {
        public LigraEnvironment(ILigraUI control, Dictionary<string, Command> commands)
        {
            if (control == null) throw new ArgumentNullException("control");

            Control = control;
            Commands = commands;
        }

        public readonly ILigraUI Control;
        public readonly Dictionary<string, Command> Commands;

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


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

            _control = control;

            this.Commands.Clear();
            if (commands == null) return;
            foreach (var kvp in commands)
            {
                var command = kvp.Value;
                AddCommand(command);
            }
        }

        private readonly ILigraUI _control;

        public IList<RenderItem> RenderItems => _control.RenderItems;
        public void AddRenderItem(RenderItem item)
        {
            _control.AddRenderItem(item);
        }

        public readonly List<string> History = new List<string>();
        public int CurrentHistoryIndex = -1;

        public LFont Font;
        public Action ClearCanvas;

        public readonly LigraParser Parser = new LigraParser();
    }
}


using System;
using MetaphysicsIndustries.Solus;
using System.Collections.Generic;
using MetaphysicsIndustries.Ligra.Commands;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraEnvironment : SolusEnvironment
    {
        public LigraEnvironment(ILigraUI control, Dictionary<string, Command> commands)
        {
            if (control == null) throw new ArgumentNullException("control");

            this.Commands.Clear();
            if (commands == null) return;
            foreach (var kvp in commands)
            {
                var command = kvp.Value;
                AddCommand(command);
            }
        }

        public readonly LigraParser Parser = new LigraParser();
    }
}


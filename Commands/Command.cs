using System.Collections.Generic;
using System.Linq;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public abstract class Command
    {
        public abstract void Execute(string input, string[] args, LigraEnvironment env);

        public virtual string GetInputLabel(string input)
        {
            return string.Format("$ {0}", input);
        }

        public static void InitializeCommands(Dictionary<string, Command> commands)
        {
            //commands["help"] = new HelpCommand();
            commands["clear"] = new ClearCommand();
            commands["vars"] = new VarsCommand();
            commands["delete"] = new DeleteCommand();
            commands["history"] = new HistoryCommand();
            commands["example"] = new ExampleCommand();
            commands["example2"] = new Example2Command();
            //commands["tsolve"] = new TSolveCommand();
            //commands["loadimage"] = new LoadImageCommand();
            commands["cd"] = new CdCommand();
        }

        public static void ClearHistory(LigraEnvironment env)
        {
            env.History.Clear();
            env.CurrentHistoryIndex = -1;
            env.AddRenderItem(new InfoItem("History cleared", env.Font, env));
        }

        public static void ClearOutput(LigraEnvironment env)
        {
            var items = env.RenderItems.ToArray();
            foreach (var item in items)
            {
                env.Control.RemoveRenderItem(item);
            }

            env.ClearCanvas();
        }
    }
}


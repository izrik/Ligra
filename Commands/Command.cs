using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Ligra.RenderItems;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public abstract class Command
    {
        public abstract void Execute(string input, string[] args, LigraEnvironment env);

        public virtual string GetInputLabel(string input, LigraEnvironment env)
        {
            return string.Format("$ {0}", input);
        }

        public virtual string DocString => null;

        public static void InitializeCommands(Dictionary<string, Command> commands)
        {
            // dummy commands
            // normally, these are instantiated in the parser, but we add some
            // dummies here so that the help command can do the lookup.

            commands["clear"] = ClearCommand.Value;
            commands["vars"] = VarsCommand.Value;
            commands["delete"] = DeleteCommand.Value;
            commands["history"] = HistoryCommand.Value;
            commands["example"] = ExampleCommand.Value;
            commands["example2"] = Example2Command.Value;
            //commands["tsolve"] = TSolveCommand.Value;
            //commands["loadimage"] = LoadImageCommand.Value;
            commands["cd"] = CdCommand.Value;

            commands["plot"] = PlotCommand.Value;
            commands["help"] = HelpCommand.Value;
            commands["paint"] = PaintCommand.Value;

            //commands["expr"] = ExprCommand.Value;
            commands["varassign"] = VarAssignCommand.Value;
            commands["funcassign"] = FuncAssignCommand.Value;
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


using System;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Commands;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public abstract class Command : Solus.Commands.Command
    {
        public override void Execute(string input, SolusEnvironment env,
            ICommandData data)
        {
            throw new NotImplementedException();
        }

        public abstract void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control);

        public virtual string GetInputLabel(string input, LigraEnvironment env,
            ICommandData data, ILigraUI control)
        {
            return string.Format("$ {0}", input);
        }

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
            commands["var_assign"] = VarAssignCommand.Value;
            commands["func_assign"] = FuncAssignCommand.Value;
        }

        public static void ClearHistory(LigraEnvironment env, ILigraUI control)
        {
            control.History.Clear();
            control.CurrentHistoryIndex = -1;
            control.AddRenderItem(
                new InfoItem("History cleared", control.DrawSettings.Font));
        }

        public static void ClearOutput(ILigraUI control)
        {
            var items = control.RenderItems.ToArray();
            foreach (var item in items)
            {
                control.RemoveRenderItem(item);
            }

            control.ClearCanvas();
        }

        public static implicit operator SimpleCommandData(Command command)
        {
            return new SimpleCommandData(command);
        }
    }

    public class SimpleCommandData : ICommandData
    {
        public SimpleCommandData(Command command)
        {
            Command = command;
        }

        public Solus.Commands.Command Command { get; }
    }

    public static class CommandHelper
    {
        public static void Execute(this Solus.Commands.Command command,
            string input, string[] args, LigraEnvironment env,
            ICommandData data, ILigraUI control)
        {
            ((Command) command).Execute(input, args, env, data, control);
        }

        public static void Execute(this ICommandData data,
            string input, string[] args, LigraEnvironment env,
            ILigraUI control)
        {
            ((Command) data.Command).Execute(input, args, env, data, control);
        }

        public static string GetInputLabel(
            this Solus.Commands.Command command, string input,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            return ((Command) command).GetInputLabel(input, env, data,
                control);
        }

        public static string GetInputLabel( this ICommandData data,
            string input, LigraEnvironment env, ILigraUI control)
        {
            return ((Command) data.Command).GetInputLabel(input, env, data,
                control);
        }
    }
}

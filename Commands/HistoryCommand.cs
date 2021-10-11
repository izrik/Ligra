using System.Linq;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class HistoryCommand : Command
    {
        public static readonly HistoryCommand Value = new HistoryCommand();

        public override string Name => "history";
        public override string DocString =>
@"history - Show or clear the command history

Show the command history:
  history

Clear the command history:
  history clear

  (Equivalent to ""clear history"" command)
";

        public override void Execute(string input, SolusEnvironment env)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ILigraUI control)
        {
            if (args.Length > 1 && args[1].ToLower() == "clear")
            {
                ClearHistory(env, control);
            }
            else
            {
                var s = string.Join("\r\n", control.History.ToArray());
                control.AddRenderItem(
                    new InfoItem(s + "\r\n", control.DrawSettings.Font));
            }
        }
    }
}